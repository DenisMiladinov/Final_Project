using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Moq;
using Models;
using Services.Repositories;
using Services.Services;
using Xunit;

namespace UnitTests.Services
{
    public class AdminServiceTests
    {
        private AdminService CreateService(
            IEnumerable<VacationSpot> spots,
            IEnumerable<Booking> bookings,
            IEnumerable<ApplicationUser> users)
        {
            var mockSpotRepo = new Mock<IVacationSpotRepository>();
            var mockBookingRepo = new Mock<IBookingRepository>();
            var mockUserStore = new Mock<IUserStore<ApplicationUser>>();
            var mockUserManager = new Mock<UserManager<ApplicationUser>>(
                mockUserStore.Object, null, null, null, null, null, null, null, null
            );

            mockSpotRepo
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(spots);

            mockBookingRepo
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(bookings);

            mockUserManager
                .Setup(um => um.Users)
                .Returns(users.AsQueryable());

            return new AdminService(
                mockSpotRepo.Object,
                mockBookingRepo.Object,
                mockUserManager.Object
            );
        }

        [Fact]
        public async Task GetAllVacationSpotsAsync_ReturnsAllSpots()
        {
            var spots = new List<VacationSpot>
            {
                new VacationSpot { SpotId = 1 },
                new VacationSpot { SpotId = 2 }
            };
            var svc = CreateService(spots, new List<Booking>(), new List<ApplicationUser>());

            var result = await svc.GetAllVacationSpotsAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, s => s.SpotId == 1);
            Assert.Contains(result, s => s.SpotId == 2);
        }

        [Fact]
        public async Task GetAllBookingsAsync_ReturnsAllBookings()
        {
            var bookings = new List<Booking>
            {
                new Booking { BookingId = 10 },
                new Booking { BookingId = 20 }
            };
            var svc = CreateService(new List<VacationSpot>(), bookings, new List<ApplicationUser>());

            var result = await svc.GetAllBookingsAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, b => b.BookingId == 10);
            Assert.Contains(result, b => b.BookingId == 20);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "user1" },
                new ApplicationUser { Id = "user2" }
            };
            var svc = CreateService(new List<VacationSpot>(), new List<Booking>(), users);

            var result = await svc.GetAllUsersAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, u => u.Id == "user1");
            Assert.Contains(result, u => u.Id == "user2");
        }
    }
}
