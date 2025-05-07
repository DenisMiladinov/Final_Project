using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Models;
using Services.Repositories;
using Services.Services;

namespace UnitTests.Services
{
    public class SearchServiceTests
    {
        private SearchService CreateService(IEnumerable<VacationSpot> spots)
        {
            var mockRepo = new Mock<IVacationSpotRepository>();
            mockRepo.Setup(r => r.GetAllAsync())
                    .ReturnsAsync(spots);
            return new SearchService(mockRepo.Object);
        }

        [Fact]
        public async Task SearchAsync_NoFilters_ReturnsAllSpots()
        {
            var spot1 = new VacationSpot { SpotId = 1, Location = "A", PricePerNight = 100m, Bookings = new List<Booking>() };
            var spot2 = new VacationSpot { SpotId = 2, Location = "B", PricePerNight = 200m, Bookings = new List<Booking>() };
            var svc = CreateService(new[] { spot1, spot2 });

            var result = await svc.SearchAsync(null, null, null, null, null);

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task SearchAsync_ByLocation_IsCaseInsensitive()
        {
            var spot1 = new VacationSpot { SpotId = 1, Location = "Beach", PricePerNight = 100m, Bookings = new List<Booking>() };
            var spot2 = new VacationSpot { SpotId = 2, Location = "Mountain", PricePerNight = 200m, Bookings = new List<Booking>() };
            var svc = CreateService(new[] { spot1, spot2 });

            var result = await svc.SearchAsync("beach", null, null, null, null);

            Assert.Single(result);
            Assert.Equal(1, result.First().SpotId);
        }

        [Fact]
        public async Task SearchAsync_ByMinPrice_FiltersBelow()
        {
            var spot1 = new VacationSpot { SpotId = 1, PricePerNight = 50m, Bookings = new List<Booking>() };
            var spot2 = new VacationSpot { SpotId = 2, PricePerNight = 150m, Bookings = new List<Booking>() };
            var svc = CreateService(new[] { spot1, spot2 });

            var result = await svc.SearchAsync(null, 100m, null, null, null);

            Assert.Single(result);
            Assert.Equal(2, result.First().SpotId);
        }

        [Fact]
        public async Task SearchAsync_ByMaxPrice_FiltersAbove()
        {
            var spot1 = new VacationSpot { SpotId = 1, PricePerNight = 50m, Bookings = new List<Booking>() };
            var spot2 = new VacationSpot { SpotId = 2, PricePerNight = 150m, Bookings = new List<Booking>() };
            var svc = CreateService(new[] { spot1, spot2 });

            var result = await svc.SearchAsync(null, null, 100m, null, null);

            Assert.Single(result);
            Assert.Equal(1, result.First().SpotId);
        }

        [Fact]
        public async Task SearchAsync_ByDateAvailability_ExcludesOverlapping()
        {
            var spotFree = new VacationSpot
            {
                SpotId = 1,
                PricePerNight = 100m,
                Bookings = new List<Booking>()
            };
            var spotBusy = new VacationSpot
            {
                SpotId = 2,
                PricePerNight = 100m,
                Bookings = new List<Booking>
                {
                    new Booking
                    {
                        BookingId = 1,
                        SpotId = 2,
                        StartDate = DateTime.Today,
                        EndDate   = DateTime.Today.AddDays(2)
                    }
                }
            };
            var svc = CreateService(new[] { spotFree, spotBusy });

            var start = DateTime.Today.AddDays(1);
            var end = DateTime.Today.AddDays(3);
            var result = await svc.SearchAsync(null, null, null, start, end);

            Assert.Single(result);
            Assert.Equal(1, result.First().SpotId);
        }

        [Fact]
        public async Task SearchAsync_PartialDates_NoDateFiltering()
        {
            var spot1 = new VacationSpot
            {
                SpotId = 1,
                PricePerNight = 100m,
                Bookings = new List<Booking>
                {
                    new Booking
                    {
                        BookingId = 1,
                        SpotId = 1,
                        StartDate = DateTime.Today,
                        EndDate = DateTime.Today.AddDays(2)
                    }
                }
            };
            var svc = CreateService(new[] { spot1 });

            // only startDate provided => should ignore date filter
            var result = await svc.SearchAsync(null, null, null, DateTime.Today, null);

            Assert.Single(result);
        }
    }
}
