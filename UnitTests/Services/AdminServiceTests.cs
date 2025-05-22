using Moq;
using Services.Services;
using Services.Repositories;
using Microsoft.AspNetCore.Identity;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace UnitTests.Services
{
    public class AdminServiceTests
    {
        private readonly Mock<IVacationSpotRepository> _vacationRepoMock;
        private readonly Mock<IBookingRepository> _bookingRepoMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly AdminService _adminService;

        public AdminServiceTests()
        {
            _vacationRepoMock = new Mock<IVacationSpotRepository>();
            _bookingRepoMock = new Mock<IBookingRepository>();

            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object, null, null, null, null, null, null, null, null);

            _adminService = new AdminService(_vacationRepoMock.Object, _bookingRepoMock.Object, _userManagerMock.Object);
        }

        [Fact]
        public async Task GetAllVacationSpotsAsync_ReturnsAllSpots()
        {
            var vacationSpots = new List<VacationSpot> { new VacationSpot(), new VacationSpot() };
            _vacationRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(vacationSpots);

            var result = await _adminService.GetAllVacationSpotsAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllBookingsAsync_ReturnsAllBookings()
        {
            var bookings = new List<Booking> { new Booking(), new Booking(), new Booking() };
            _bookingRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(bookings);

            var result = await _adminService.GetAllBookingsAsync();

            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsAllUsers()
        {
            var users = new List<ApplicationUser> { new ApplicationUser(), new ApplicationUser() }.AsQueryable();
            _userManagerMock.Setup(u => u.Users).Returns(users);

            var result = await _adminService.GetAllUsersAsync();

            Assert.Equal(2, result.Count());
        }
    }
}
