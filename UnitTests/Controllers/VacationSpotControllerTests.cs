using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Models;
using Server.Controllers;
using Services.Services;
using Xunit;

namespace UnitTests.Controllers
{
    public class VacationSpotControllerTests
    {
        private VacationSpotController CreateController(Mock<IVacationSpotService> mockService)
        {
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(""); // not used in tested methods

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;
            var context = new ApplicationDbContext(options);

            return new VacationSpotController(mockService.Object, envMock.Object, context);
        }

        [Fact]
        public async Task Index_ReturnsViewWithAllSpots()
        {
            var spots = new List<VacationSpot>
            {
                new VacationSpot { SpotId = 1, Title = "A" },
                new VacationSpot { SpotId = 2, Title = "B" }
            };
            var mockSvc = new Mock<IVacationSpotService>();
            mockSvc.Setup(s => s.GetAllAsync()).ReturnsAsync(spots);

            var ctrl = CreateController(mockSvc);

            var result = await ctrl.Index(null, null, null) as ViewResult;
            var model = Assert.IsAssignableFrom<IEnumerable<VacationSpot>>(result.Model);
            Assert.Equal(2, model.AsList().Count);
        }

        [Fact]
        public async Task Details_InvalidId_ReturnsNotFound()
        {
            var mockSvc = new Mock<IVacationSpotService>();
            mockSvc.Setup(s => s.GetByIdAsync(99))
                   .ReturnsAsync((VacationSpot)null);

            var ctrl = CreateController(mockSvc);

            var result = await ctrl.Details(99);
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Details_ValidId_ReturnsViewWithSpot()
        {
            var spot = new VacationSpot { SpotId = 5, Title = "TestSpot" };
            var mockSvc = new Mock<IVacationSpotService>();
            mockSvc.Setup(s => s.GetByIdAsync(5))
                   .ReturnsAsync(spot);

            var ctrl = CreateController(mockSvc);

            var result = await ctrl.Details(5) as ViewResult;
            Assert.Equal(spot, result.Model);
        }

        [Fact]
        public async Task DeleteConfirmed_CallsServiceAndRedirects()
        {
            var mockSvc = new Mock<IVacationSpotService>();
            var ctrl = CreateController(mockSvc);

            var result = await ctrl.DeleteConfirmed(3) as RedirectToActionResult;

            mockSvc.Verify(s => s.DeleteAsync(3), Times.Once);
            Assert.Equal("Index", result.ActionName);
        }
    }

    internal static class EnumerableExtensions
    {
        public static List<T> AsList<T>(this IEnumerable<T> src) => new List<T>(src);
    }
}
