/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Models;
using Services.Services;
using Server.Controllers;
using Xunit;

namespace UnitTests.Controllers
{
    public class SearchControllerTests
    {
        [Fact]
        public async Task Index_WithNullParameters_ReturnsAllResults()
        {
            // Arrange
            var sampleSpots = new List<VacationSpot>
            {
                new VacationSpot { SpotId = 1, Title = "A", Location = "X" },
                new VacationSpot { SpotId = 2, Title = "B", Location = "Y" }
            };
            var mockSvc = new Mock<ISearchService>();
            mockSvc
                .Setup(s => s.SearchAsync(null, null, null, null, null))
                .ReturnsAsync(sampleSpots);

            var controller = new SearchController(mockSvc.Object);

            
            var result = await controller.Index(null, null, null, null, null) as ViewResult;

            
            Assert.NotNull(result);
            var model = Assert.IsAssignableFrom<IEnumerable<VacationSpot>>(result.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Index_PassesParametersToService()
        {
            
            var mockSvc = new Mock<ISearchService>();
            mockSvc
                .Setup(s => s.SearchAsync(
                    "beach",
                    50m,
                    200m,
                    new DateTime(2025, 1, 1),
                    new DateTime(2025, 1, 5)
                ))
                .ReturnsAsync(new List<VacationSpot>())
                .Verifiable();

            var controller = new SearchController(mockSvc.Object);

            
            var result = await controller.Index(
                "beach",
                50m,
                200m,
                new DateTime(2025, 1, 1),
                new DateTime(2025, 1, 5)
            );

            
            mockSvc.Verify();
            Assert.IsType<ViewResult>(result);
        }
    }
}*/