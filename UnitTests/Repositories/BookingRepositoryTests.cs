using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Repositories;
using Xunit;

namespace UnitTests.Repositories
{
    public class BookingRepositoryTests
    {
        private ApplicationDbContext CreateContext(string name)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(name)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetBookingsByUserIdAsync_ReturnsOnlyUserBookings()
        {
            var context = CreateContext(nameof(GetBookingsByUserIdAsync_ReturnsOnlyUserBookings));
            var user1 = new ApplicationUser { Id = "user1", UserName = "user1" };
            var user2 = new ApplicationUser { Id = "user2", UserName = "user2" };
            context.Users.AddRange(user1, user2);
            context.Bookings.AddRange(
                new Booking { BookingId = 1, UserId = "user1", SpotId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1) },
                new Booking { BookingId = 2, UserId = "user2", SpotId = 1, StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(1) }
            );
            await context.SaveChangesAsync();

            var repo = new BookingRepository(context);
            var result = await repo.GetBookingsByUserIdAsync("user1");

            Assert.Single(result);
            Assert.All(result, b => Assert.Equal("user1", b.UserId));
        }

        [Fact]
        public async Task IsSpotAvailableAsync_NoBookings_ReturnsTrue()
        {
            var context = CreateContext(nameof(IsSpotAvailableAsync_NoBookings_ReturnsTrue));
            var repo = new BookingRepository(context);

            var available = await repo.IsSpotAvailableAsync(1, DateTime.Today, DateTime.Today.AddDays(1));

            Assert.True(available);
        }

        [Fact]
        public async Task IsSpotAvailableAsync_OverlappingDates_ReturnsFalse()
        {
            var context = CreateContext(nameof(IsSpotAvailableAsync_OverlappingDates_ReturnsFalse));
            context.Bookings.Add(new Booking
            {
                BookingId = 1,
                SpotId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2)
            });
            await context.SaveChangesAsync();

            var repo = new BookingRepository(context);
            var available = await repo.IsSpotAvailableAsync(1, DateTime.Today.AddDays(1), DateTime.Today.AddDays(3));

            Assert.False(available);
        }

        [Fact]
        public async Task IsSpotAvailableAsync_NonOverlapping_ReturnsTrue()
        {
            var context = CreateContext(nameof(IsSpotAvailableAsync_NonOverlapping_ReturnsTrue));
            context.Bookings.Add(new Booking
            {
                BookingId = 1,
                SpotId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            });
            await context.SaveChangesAsync();

            var repo = new BookingRepository(context);
            var availableBefore = await repo.IsSpotAvailableAsync(1, DateTime.Today.AddDays(-2), DateTime.Today.AddDays(-1));
            var availableAfter = await repo.IsSpotAvailableAsync(1, DateTime.Today.AddDays(2), DateTime.Today.AddDays(3));

            Assert.True(availableBefore);
            Assert.True(availableAfter);
        }

        [Fact]
        public async Task GetAllAsync_IncludesRelatedEntities()
        {
            var context = CreateContext(nameof(GetAllAsync_IncludesRelatedEntities));
            var spot = new VacationSpot { SpotId = 1, Title = "Spot1" };
            var user = new ApplicationUser { Id = "user1", UserName = "user1" };
            context.VacationSpots.Add(spot);
            context.Users.Add(user);
            context.Bookings.Add(new Booking
            {
                BookingId = 1,
                UserId = "user1",
                SpotId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            });
            await context.SaveChangesAsync();

            var repo = new BookingRepository(context);
            var all = await repo.GetAllAsync();
            var booking = Assert.Single(all);

            Assert.Equal("user1", booking.UserId);
            Assert.NotNull(booking.User);
            Assert.Equal("Spot1", booking.VacationSpot.Title);
        }
    }
}
