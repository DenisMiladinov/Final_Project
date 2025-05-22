using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Server.Controllers;
using Services.Services;
using Models;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace UnitTests.Controllers
{
    public class SearchControllerTests
    {
        private readonly Mock<ISearchService> _searchServiceMock;
        private readonly SearchController _controller;

        public SearchControllerTests()
        {
            _searchServiceMock = new Mock<ISearchService>();
            _controller = new SearchController(_searchServiceMock.Object);
        }

        [Fact]
        public async Task Index_ReturnsViewWithResults()
        {
            // Arrange
            var mockResults = new List<VacationSpot>
            {
                new VacationSpot { SpotId = 1 },
                new VacationSpot { SpotId = 2 }
            };

            _searchServiceMock.Setup(s => s.SearchAsync(
                "sofia", 50, 300, null, null)).ReturnsAsync(mockResults);

            // Act
            var result = await _controller.Index("sofia", 50, 300, null, null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<VacationSpot>>(viewResult.Model);

            Assert.Equal(2, ((List<VacationSpot>)model).Count);
        }
    }
}
