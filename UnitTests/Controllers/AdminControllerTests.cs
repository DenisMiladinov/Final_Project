using Xunit;
using Moq;
using Server.Controllers;
using Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests.Controllers
{
    public class AdminControllerTests
    {
        private readonly Mock<IVacationSpotService> _spotServiceMock;
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly AdminController _controller;

        public AdminControllerTests()
        {
            _spotServiceMock = new Mock<IVacationSpotService>();
            _bookingServiceMock = new Mock<IBookingService>();

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                userStore.Object, null, null, null, null, null, null, null, null
            );

            _controller = new AdminController(
                _spotServiceMock.Object,
                _bookingServiceMock.Object,
                _userManagerMock.Object,
                null, // context
                null  // env
            );
        }

        [Fact]
        public async Task Index_ReturnsViewWithCorrectViewModel()
        {
            // Arrange
            _spotServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<VacationSpot> { new VacationSpot(), new VacationSpot() });
            _bookingServiceMock.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Booking> { new Booking() });
            var users = new List<ApplicationUser> { new ApplicationUser(), new ApplicationUser(), new ApplicationUser() }.AsQueryable();
            _userManagerMock.Setup(u => u.Users).Returns(users);

            // Act
            var result = await _controller.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<Models.ViewModels.AdminDashboardViewModel>(viewResult.Model);

            Assert.Equal(2, model.TotalSpots);
            Assert.Equal(1, model.TotalBookings);
            Assert.Equal(3, model.TotalUsers);
        }
    }
}
