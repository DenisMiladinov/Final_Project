using Xunit;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using Server.Controllers;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace UnitTests.Controllers
{
    public class BookingControllerTests
    {
        private readonly Mock<IBookingService> _bookingServiceMock;
        private readonly Mock<IVacationSpotService> _spotServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly BookingController _controller;

        public BookingControllerTests()
        {
            _bookingServiceMock = new Mock<IBookingService>();
            _spotServiceMock = new Mock<IVacationSpotService>();

            var store = new Mock<IUserStore<ApplicationUser>>();
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(store.Object, null, null, null, null, null, null, null, null);

            var configMock = new Mock<IConfiguration>();

            _controller = new BookingController(
                _bookingServiceMock.Object,
                _spotServiceMock.Object,
                _userManagerMock.Object,
                configMock.Object
            );
        }

        [Fact]
        public async Task MyBookings_ReturnsViewWithUserBookings()
        {
            // Arrange: mock identity
            var userId = "test-user-id";
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            var mockBookings = new List<Booking> { new Booking(), new Booking() };
            _bookingServiceMock.Setup(s => s.GetBookingsByUserIdAsync(userId)).ReturnsAsync(mockBookings);

            // Act
            var result = await _controller.MyBookings();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Booking>>(viewResult.Model);
            Assert.Equal(2, model.Count());
        }
    }
}
