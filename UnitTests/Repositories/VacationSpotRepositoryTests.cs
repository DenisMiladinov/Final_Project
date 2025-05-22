using Xunit;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace UnitTests.Repositories
{
    public class VacationSpotRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly VacationSpotRepository _repository;

        public VacationSpotRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("VacationSpotRepoTests")
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new VacationSpotRepository(_context);
        }

        [Fact]
        public async Task GetByLocationAsync_ReturnsMatchingSpots()
        {
            _context.VacationSpots.Add(new VacationSpot { SpotId = 1, Location = "Paris" });
            _context.VacationSpots.Add(new VacationSpot { SpotId = 2, Location = "paris central" });
            _context.VacationSpots.Add(new VacationSpot { SpotId = 3, Location = "Berlin" });
            await _context.SaveChangesAsync();

            var result = await _repository.GetByLocationAsync("paris");

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAvailableSpotsAsync_ReturnsOnlyAvailableSpots()
        {
            var spotWithConflict = new VacationSpot
            {
                SpotId = 1,
                Bookings = new List<Booking>
                {
                    new Booking
                    {
                        StartDate = new DateTime(2025, 6, 10),
                        EndDate = new DateTime(2025, 6, 15)
                    }
                }
            };

            var spotAvailable = new VacationSpot
            {
                SpotId = 2,
                Bookings = new List<Booking>()
            };

            _context.VacationSpots.AddRange(spotWithConflict, spotAvailable);
            await _context.SaveChangesAsync();

            var result = await _repository.GetAvailableSpotsAsync(
                new DateTime(2025, 6, 12),
                new DateTime(2025, 6, 14)
            );

            Assert.Single(result);
            Assert.Equal(2, result.First().SpotId);
        }
    }
}
