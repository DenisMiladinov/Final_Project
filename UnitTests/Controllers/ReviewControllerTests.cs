/*using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Models;
using Services.Services;
using Server.Controllers;
using Xunit;

namespace UnitTests.Controllers
{
    public class ReviewControllerTests
    {
        private ReviewController CreateController(
            Mock<IReviewService> mockSvc,
            string userId = "user1",
            bool isAdmin = false)
        {
            var controller = new ReviewController(mockSvc.Object);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, userId)
            };
            if (isAdmin)
            {
                var list = new List<Claim>(claims)
                {
                    new Claim(ClaimTypes.Role, "Admin")
                };
                claims = list.ToArray();
            }

            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
            return controller;
        }

        [Fact]
        public async Task Add_SetsUserIdAndCallsService_ThenRedirects()
        {
            var mockSvc = new Mock<IReviewService>();
            var review = new Review
            {
                SpotId = 5,
                Rating = 4,
                Comment = "Great!"
            };

            var controller = CreateController(mockSvc, userId: "u123");
            var result = await controller.Add(review) as RedirectToActionResult;

            mockSvc.Verify(s =>
                s.AddReviewAsync(It.Is<Review>(r =>
                    r.SpotId == 5 &&
                    r.UserId == "u123" &&
                    r.Rating == 4 &&
                    r.Comment == "Great!"
                )), Times.Once);

            Assert.NotNull(result);
            Assert.Equal("Details", result.ActionName);
            Assert.Equal("VacationSpot", result.ControllerName);
            Assert.Equal(5, result.RouteValues["id"]);
        }

        [Fact]
        public async Task Delete_AsAdmin_CallsServiceAndRedirects()
        {
            var mockSvc = new Mock<IReviewService>();
            var controller = CreateController(mockSvc, userId: "admin", isAdmin: true);

            var result = await controller.Delete(id: 7, spotId: 9) as RedirectToActionResult;

            mockSvc.Verify(s => s.DeleteReviewAsync(7), Times.Once);

            Assert.NotNull(result);
            Assert.Equal("Details", result.ActionName);
            Assert.Equal("VacationSpot", result.ControllerName);
            Assert.Equal(9, result.RouteValues["id"]);
        }
    }
}*/