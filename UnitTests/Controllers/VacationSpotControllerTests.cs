using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Services;
using Server.Controllers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace UnitTests.Controllers
{
    public class VacationSpotControllerTests
    {
        private readonly Mock<IVacationSpotService> _vacationSpotServiceMock;
        private readonly VacationSpotController _controller;

        public VacationSpotControllerTests()
        {
            _vacationSpotServiceMock = new Mock<IVacationSpotService>();

            // other dependencies can be null for this test
            _controller = new VacationSpotController(
                _vacationSpotServiceMock.Object,
                null, null, null
            );
        }

        [Fact]
        public async Task Index_ReturnsFilteredSortedSpots()
        {
            // Arrange
            var spots = new List<VacationSpot>
            {
                new VacationSpot { SpotId = 1, Title = "Beach" },
                new VacationSpot { SpotId = 2, Title = "Mountain" }
            };

            _vacationSpotServiceMock
                .Setup(s => s.GetAllAsync())
                .ReturnsAsync(spots);

            // Act
            var result = await _controller.Index("Beach", "Name", null);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<VacationSpot>>(viewResult.Model);
            Assert.Single(model);
            Assert.Equal("Beach", ((VacationSpot[])model)[0].Title);
        }
    }
}
