using Xunit;
using Moq;
using Services.Services;
using Services.Repositories;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;

namespace UnitTests.Services
{
    public class SearchServiceTests
    {
        private readonly Mock<IVacationSpotRepository> _vacationRepoMock;
        private readonly SearchService _searchService;

        public SearchServiceTests()
        {
            _vacationRepoMock = new Mock<IVacationSpotRepository>();
            _searchService = new SearchService(_vacationRepoMock.Object);
        }

        [Fact]
        public async Task SearchAsync_FiltersByLocation()
        {
            var spots = new List<VacationSpot>
            {
                new VacationSpot { Location = "Paris" },
                new VacationSpot { Location = "Berlin" },
                new VacationSpot { Location = "paris central" }
            };

            _vacationRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(spots);

            var result = await _searchService.SearchAsync("paris", null, null, null, null);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task SearchAsync_FiltersByMinPrice()
        {
            var spots = new List<VacationSpot>
            {
                new VacationSpot { PricePerNight = 100 },
                new VacationSpot { PricePerNight = 200 }
            };

            _vacationRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(spots);

            var result = await _searchService.SearchAsync(null, 150, null, null, null);

            Assert.Single(result);
            Assert.Equal(200, result.First().PricePerNight);
        }

        [Fact]
        public async Task SearchAsync_FiltersByMaxPrice()
        {
            var spots = new List<VacationSpot>
            {
                new VacationSpot { PricePerNight = 100 },
                new VacationSpot { PricePerNight = 200 }
            };

            _vacationRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(spots);

            var result = await _searchService.SearchAsync(null, null, 150, null, null);

            Assert.Single(result);
            Assert.Equal(100, result.First().PricePerNight);
        }

        [Fact]
        public async Task SearchAsync_FiltersByDateAvailability()
        {
            var startDate = new DateTime(2025, 6, 10);
            var endDate = new DateTime(2025, 6, 15);

            var spots = new List<VacationSpot>
            {
                new VacationSpot
                {
                    Bookings = new List<Booking> // not available
                    {
                        new Booking { StartDate = new DateTime(2025, 6, 12), EndDate = new DateTime(2025, 6, 20) }
                    }
                },
                new VacationSpot
                {
                    Bookings = new List<Booking>() // available
                }
            };

            _vacationRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(spots);

            var result = await _searchService.SearchAsync(null, null, null, startDate, endDate);

            Assert.Single(result);
        }

        [Fact]
        public async Task SearchAsync_ReturnsAll_WhenNoFiltersApplied()
        {
            var spots = new List<VacationSpot>
            {
                new VacationSpot(),
                new VacationSpot()
            };

            _vacationRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(spots);

            var result = await _searchService.SearchAsync(null, null, null, null, null);

            Assert.Equal(2, result.Count());
        }
    }
}
