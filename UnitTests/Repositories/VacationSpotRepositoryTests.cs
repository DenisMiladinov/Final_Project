/*using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Repositories;
using Xunit;

namespace UnitTests.Repositories
{
    public class VacationSpotRepositoryTests
    {
        private ApplicationDbContext CreateContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task GetByLocationAsync_ReturnsOnlyMatchingSpots_WithImages()
        {
            var context = CreateContext(nameof(GetByLocationAsync_ReturnsOnlyMatchingSpots_WithImages));
            var spot1 = new VacationSpot
            {
                SpotId = 1,
                Title = "Beachside",
                Location = "Sunny Beach"
            };
            var spot2 = new VacationSpot
            {
                SpotId = 2,
                Title = "Mountain Cabin",
                Location = "High Mountains"
            };
            context.VacationSpots.AddRange(spot1, spot2);
            context.Images.Add(new Image { ImageId = 1, SpotId = 1, ImageUrl = "/img1.jpg" });
            await context.SaveChangesAsync();

            var repo = new VacationSpotRepository(context);
            var result = await repo.GetByLocationAsync("Beach");

            Assert.Single(result);
            var returned = result.First();
            Assert.Equal(1, returned.SpotId);
            Assert.Single(returned.Images);
        }

        [Fact]
        public async Task GetAvailableSpotsAsync_NoBookings_ReturnsAllSpots()
        {
            var context = CreateContext(nameof(GetAvailableSpotsAsync_NoBookings_ReturnsAllSpots));
            context.VacationSpots.Add(new VacationSpot { SpotId = 1, Title = "A" });
            context.VacationSpots.Add(new VacationSpot { SpotId = 2, Title = "B" });
            await context.SaveChangesAsync();

            var repo = new VacationSpotRepository(context);
            var result = await repo.GetAvailableSpotsAsync(
                DateTime.Today, DateTime.Today.AddDays(1));

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAvailableSpotsAsync_ExcludesOccupiedSpots()
        {
            var context = CreateContext(nameof(GetAvailableSpotsAsync_ExcludesOccupiedSpots));
            var spot1 = new VacationSpot { SpotId = 1, Title = "A" };
            var spot2 = new VacationSpot { SpotId = 2, Title = "B" };
            context.VacationSpots.AddRange(spot1, spot2);
            context.Bookings.Add(new Booking
            {
                BookingId = 1,
                SpotId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(2)
            });
            await context.SaveChangesAsync();

            var repo = new VacationSpotRepository(context);
            var result = await repo.GetAvailableSpotsAsync(
                DateTime.Today.AddDays(1), DateTime.Today.AddDays(3));

            Assert.Single(result);
            Assert.Equal(2, result.First().SpotId);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllSpots()
        {
            var context = CreateContext(nameof(GetAllAsync_ReturnsAllSpots));
            context.VacationSpots.AddRange(
                new VacationSpot { SpotId = 1, Title = "X" },
                new VacationSpot { SpotId = 2, Title = "Y" }
            );
            await context.SaveChangesAsync();

            var repo = new VacationSpotRepository(context);
            var result = await repo.GetAllAsync();

            Assert.Equal(2, result.Count());
        }
    }
}*/