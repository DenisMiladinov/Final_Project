using Xunit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Models;
using Services.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace UnitTests.Services
{
    public class VacationSpotServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IReviewService> _reviewServiceMock;
        private readonly VacationSpotService _vacationSpotService;

        public VacationSpotServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "VacationSpotServiceTests")
                .Options;

            _context = new ApplicationDbContext(options);
            _reviewServiceMock = new Mock<IReviewService>();
            _vacationSpotService = new VacationSpotService(_context, _reviewServiceMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllVacationSpots()
        {
            _context.VacationSpots.Add(new VacationSpot { SpotId = 1 });
            _context.VacationSpots.Add(new VacationSpot { SpotId = 2 });
            await _context.SaveChangesAsync();

            var result = await _vacationSpotService.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectSpot()
        {
            var spot = new VacationSpot { SpotId = 100 };
            _context.VacationSpots.Add(spot);
            await _context.SaveChangesAsync();

            var result = await _vacationSpotService.GetByIdAsync(100);

            Assert.NotNull(result);
            Assert.Equal(100, result.SpotId);
        }

        [Fact]
        public async Task CreateAsync_AddsSpot()
        {
            var spot = new VacationSpot { SpotId = 200 };

            await _vacationSpotService.CreateAsync(spot);

            var result = await _context.VacationSpots.FindAsync(200);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateAsync_ModifiesSpot()
        {
            var spot = new VacationSpot { SpotId = 300, Title = "Old Name" };
            _context.VacationSpots.Add(spot);
            await _context.SaveChangesAsync();

            spot.Title = "New Name";
            await _vacationSpotService.UpdateAsync(spot);

            var updated = await _context.VacationSpots.FindAsync(300);
            Assert.Equal("New Name", updated.Title);
        }
    }
}
