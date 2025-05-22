using Xunit;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests.Repositories
{
    public class VacationSpotRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly VacationSpotRepository _repository;

        public VacationSpotRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new VacationSpotRepository(_context);
        }

        [Fact]
        public async Task GetByLocationAsync_ReturnsMatchingSpots()
        {
            var s1 = new VacationSpot
            {
                SpotId = 1,
                Location = "Paris",
                OwnerId = "o1",
                Title = "Paris Spot"
            };
            var s2 = new VacationSpot
            {
                SpotId = 2,
                Location = "paris central",
                OwnerId = "o2",
                Title = "Central Paris"
            };
            var s3 = new VacationSpot
            {
                SpotId = 3,
                Location = "Berlin",
                OwnerId = "o3",
                Title = "Berlin Base"
            };
            _context.VacationSpots.AddRange(s1, s2, s3);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByLocationAsync("paris");

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAvailableSpotsAsync_ReturnsOnlyAvailableSpots()
        {
            var conflict = new VacationSpot
            {
                SpotId = 4,
                OwnerId = "o4",
                Title = "Busy Spot",
                Bookings = new List<Booking> {
                    new Booking {
                        BookingId = 100,
                        SpotId = 4,
                        UserId = "u100",
                        StartDate = new DateTime(2025,6,10),
                        EndDate = new DateTime(2025,6,15)
                    }
                }
            };
            var free = new VacationSpot
            {
                SpotId = 5,
                OwnerId = "o5",
                Title = "Free Spot",
                Bookings = new List<Booking>()
            };
            _context.VacationSpots.AddRange(conflict, free);
            await _context.SaveChangesAsync();

            var avail = await _repository.GetAvailableSpotsAsync(
                new DateTime(2025, 6, 12),
                new DateTime(2025, 6, 14)
            );

            Assert.Single(avail);
            Assert.Equal(5, avail.First().SpotId);
        }
    }
}
