using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using Moq;
using Server.Controllers;
using Services.Services;
using UnitTests.Utils;
using Xunit;

namespace UnitTests.Controllers
{
    public class AdminControllerTests
    {
        private ApplicationDbContext CreateContext(string name)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(name)
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task Index_ReturnsDashboardViewModel()
        {
            var context = CreateContext(nameof(Index_ReturnsDashboardViewModel));
            context.Categories.AddRange(
                new Category { CategoryId = 1 },
                new Category { CategoryId = 2 }
            );
            await context.SaveChangesAsync();
            var spotServiceMock = new Mock<IVacationSpotService>();
            spotServiceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<VacationSpot> { new VacationSpot(), new VacationSpot(), new VacationSpot() });
            var bookingServiceMock = new Mock<IBookingService>();
            bookingServiceMock.Setup(s => s.GetAllAsync())
                .ReturnsAsync(new List<Booking> { new Booking(), new Booking() });
            var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            var userManagerMock = TestHelpers.MockUserManager<ApplicationUser>();
            userManagerMock.Setup(u => u.Users)
                .Returns(new List<ApplicationUser>
                {
                    new ApplicationUser(),
                    new ApplicationUser(),
                    new ApplicationUser(),
                    new ApplicationUser()
                }.AsQueryable());
            var controller = new AdminController(
                spotServiceMock.Object,
                bookingServiceMock.Object,
                userManagerMock.Object,
                context
            );
            var result = await controller.Index() as ViewResult;
            var model = Assert.IsType<AdminDashboardViewModel>(result.Model);
            Assert.Equal(3, model.TotalSpots);
            Assert.Equal(2, model.TotalBookings);
            Assert.Equal(4, model.TotalUsers);
            Assert.Equal(2, model.TotalCategories);
        }

        [Fact]
        public async Task Spots_ReturnsVacationSpotList()
        {
            var mockService = new Mock<IVacationSpotService>();
            var spots = new List<VacationSpot> { new VacationSpot(), new VacationSpot() };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(spots);
            var userManagerMock = TestHelpers.MockUserManager<ApplicationUser>().Object;
            var context = CreateContext(nameof(Spots_ReturnsVacationSpotList));
            var controller = new AdminController(
                mockService.Object,
                new Mock<IBookingService>().Object,
                userManagerMock,
                context
            );
            var result = await controller.Spots() as ViewResult;
            var model = Assert.IsAssignableFrom<IEnumerable<VacationSpot>>(result.Model);
            Assert.Equal(2, model.Count());
            Assert.Equal("VacationSpot/Spots", result.ViewName);
        }

        [Fact]
        public async Task Bookings_ReturnsBookingList()
        {
            var mockService = new Mock<IBookingService>();
            var bookings = new List<Booking> { new Booking(), new Booking(), new Booking() };
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(bookings);
            var userManagerMock = TestHelpers.MockUserManager<ApplicationUser>().Object;
            var context = CreateContext(nameof(Bookings_ReturnsBookingList));
            var controller = new AdminController(
                new Mock<IVacationSpotService>().Object,
                mockService.Object,
                userManagerMock,
                context
            );
            var result = await controller.Bookings() as ViewResult;
            var model = Assert.IsAssignableFrom<IEnumerable<Booking>>(result.Model);
            Assert.Equal(3, model.Count());
            Assert.Equal("Booking/Bookings", result.ViewName);
        }

        [Fact]
        public async Task CancelBooking_RedirectsToBookings()
        {
            var mockService = new Mock<IBookingService>();
            var userManagerMock = TestHelpers.MockUserManager<ApplicationUser>().Object;
            var context = CreateContext(nameof(CancelBooking_RedirectsToBookings));
            var controller = new AdminController(
                new Mock<IVacationSpotService>().Object,
                mockService.Object,
                userManagerMock,
                context
            );
            var result = await controller.CancelBooking(5) as RedirectToActionResult;
            Assert.Equal("Bookings", result.ActionName);
        }

        [Fact]
        public async Task DeleteBookingConfirmed_RedirectsToBookings()
        {
            var mockService = new Mock<IBookingService>();
            var userManagerMock = TestHelpers.MockUserManager<ApplicationUser>().Object;
            var context = CreateContext(nameof(DeleteBookingConfirmed_RedirectsToBookings));
            var controller = new AdminController(
                new Mock<IVacationSpotService>().Object,
                mockService.Object,
                userManagerMock,
                context
            );
            var result = await controller.DeleteBookingConfirmed(7) as RedirectToActionResult;
            Assert.Equal("Bookings", result.ActionName);
        }
    }
}