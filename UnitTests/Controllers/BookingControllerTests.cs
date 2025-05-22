/*using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Models;
using Server.Controllers;
using Services.Services;
using UnitTests.Utils;
using Xunit;

namespace UnitTests.Controllers
{
    public class BookingControllerTests
    {
        [Fact]
        public async Task MyBookings_ReturnsViewWithUserBookings()
        {
            var bookingServiceMock = new Mock<IBookingService>();
            bookingServiceMock
                .Setup(s => s.GetBookingsByUserIdAsync("user1"))
                .ReturnsAsync(new List<Booking> { new Booking { BookingId = 1 } });
            var spotServiceMock = new Mock<IVacationSpotService>();
            var userManagerMock = TestHelpers.MockUserManager<ApplicationUser>().Object;
            var configMock = new Mock<IConfiguration>().Object;
            var controller = new BookingController(
                bookingServiceMock.Object,
                spotServiceMock.Object,
                userManagerMock,
                configMock)
            {
                ControllerContext = TestHelpers.GetControllerContext("user1")
            };
            var result = await controller.MyBookings() as ViewResult;
            var model = Assert.IsAssignableFrom<IEnumerable<Booking>>(result.Model);
            Assert.Single(model);
        }

        [Fact]
        public async Task Create_Post_InvalidModel_ReturnsView()
        {
            var bookingServiceMock = new Mock<IBookingService>();
            var spotServiceMock = new Mock<IVacationSpotService>();
            var userManagerMock = TestHelpers.MockUserManager<ApplicationUser>().Object;
            var configMock = new Mock<IConfiguration>().Object;
            var controller = new BookingController(
                bookingServiceMock.Object,
                spotServiceMock.Object,
                userManagerMock,
                configMock)
            {
                ControllerContext = TestHelpers.GetControllerContext("user1")
            };
            controller.ModelState.AddModelError("Risk", "Error");
            var result = await controller.Create(new Booking()) as ViewResult;
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async Task Cancel_Post_RedirectsToMyBookings()
        {
            var bookingServiceMock = new Mock<IBookingService>();
            var spotServiceMock = new Mock<IVacationSpotService>();
            var userManagerMock = TestHelpers.MockUserManager<ApplicationUser>().Object;
            var configMock = new Mock<IConfiguration>().Object;
            var controller = new BookingController(
                bookingServiceMock.Object,
                spotServiceMock.Object,
                userManagerMock,
                configMock)
            {
                ControllerContext = TestHelpers.GetControllerContext("user1")
            };
            var result = await controller.Cancel(1) as RedirectToActionResult;
            Assert.Equal("MyBookings", result.ActionName);
        }

        [Fact]
        public async Task Edit_Get_ReturnsViewWithBooking()
        {
            var booking = new Booking { BookingId = 2 };
            var bookingServiceMock = new Mock<IBookingService>();
            bookingServiceMock
                .Setup(s => s.GetByIdAsync(2))
                .ReturnsAsync(booking);
            var spotServiceMock = new Mock<IVacationSpotService>();
            var userManagerMock = TestHelpers.MockUserManager<ApplicationUser>().Object;
            var configMock = new Mock<IConfiguration>().Object;
            var controller = new BookingController(
                bookingServiceMock.Object,
                spotServiceMock.Object,
                userManagerMock,
                configMock)
            {
                ControllerContext = TestHelpers.GetControllerContext("admin1", "Admin")
            };
            var result = await controller.Edit(2) as ViewResult;
            Assert.Equal(booking, result.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_DeletesBookingAndRedirects()
        {
            var booking = new Booking { BookingId = 3 };
            var bookingServiceMock = new Mock<IBookingService>();
            bookingServiceMock
                .Setup(s => s.GetByIdAsync(3))
                .ReturnsAsync(booking);
            var spotServiceMock = new Mock<IVacationSpotService>();
            var userManagerMock = TestHelpers.MockUserManager<ApplicationUser>().Object;
            var configMock = new Mock<IConfiguration>().Object;
            var controller = new BookingController(
                bookingServiceMock.Object,
                spotServiceMock.Object,
                userManagerMock,
                configMock)
            {
                ControllerContext = TestHelpers.GetControllerContext("admin1", "Admin")
            };
            var result = await controller.DeleteConfirmed(3) as RedirectToActionResult;
            Assert.Equal("ManageBookings", result.ActionName);
            Assert.Equal("Admin", result.ControllerName);
        }
    }
}*/