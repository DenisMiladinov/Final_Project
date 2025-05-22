using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Services.Services;
using Models;
using Models.ViewModels;
using System.Threading.Tasks;
using Server.Controllers;

namespace UnitTests.Controllers
{
    public class ReviewControllerTests
    {
        private readonly Mock<IReviewService> _reviewServiceMock;
        private readonly Mock<IVacationSpotService> _vacationSpotServiceMock;
        private readonly ReviewController _controller;

        public ReviewControllerTests()
        {
            _reviewServiceMock = new Mock<IReviewService>();
            _vacationSpotServiceMock = new Mock<IVacationSpotService>();

            _controller = new ReviewController(_reviewServiceMock.Object, _vacationSpotServiceMock.Object);
        }

        private void SetUserIdentity(string userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task Add_ReturnsRedirect_WhenModelIsValid()
        {
            // Arrange
            SetUserIdentity("user123");
            var vm = new ReviewViewModel
            {
                SpotId = 5,
                Rating = 4,
                Comment = "Good place"
            };

            _controller.ModelState.Clear();

            // Act
            var result = await _controller.Add(vm);

            // Assert
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal("VacationSpot", redirect.ControllerName);
            Assert.Equal(5, redirect.RouteValues["id"]);
        }

        [Fact]
        public async Task Add_ReturnsDetailsView_WhenModelIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("Comment", "Required");

            var vm = new ReviewViewModel { SpotId = 10 };

            _vacationSpotServiceMock
                .Setup(s => s.BuildDetailsViewModelAsync(10))
                .ReturnsAsync(new VacationSpotDetailsViewModel());

            // Act
            var result = await _controller.Add(vm);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("Details", viewResult.ViewName);
        }

        [Fact]
        public async Task Delete_RedirectsToDetailsAfterDelete()
        {
            // Act
            var result = await _controller.Delete(1, 99);

            // Assert
            _reviewServiceMock.Verify(r => r.DeleteReviewAsync(1), Times.Once);

            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Details", redirect.ActionName);
            Assert.Equal("VacationSpot", redirect.ControllerName);
            Assert.Equal(99, redirect.RouteValues["id"]);
        }
    }
}
