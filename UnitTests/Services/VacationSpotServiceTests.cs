using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.EntityFrameworkCore;
using Moq;
using Models;
using Services.Services;

namespace UnitTests.Services
{
    public class VacationSpotServiceTests
    {
        private readonly ApplicationDbContext _context;
        private readonly Mock<IReviewService> _reviewServiceMock;
        private readonly VacationSpotService _vacationSpotService;

        public VacationSpotServiceTests()
        {
            // Use a GUID for the DB name so each test run is isolated
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _reviewServiceMock = new Mock<IReviewService>();
            _vacationSpotService = new VacationSpotService(_context, _reviewServiceMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllVacationSpots()
        {
            var spot1 = new VacationSpot
            {
                SpotId = 1,
                Title = "Spot One",
                OwnerId = "owner1",
                Owner = new ApplicationUser { Id = "owner1", UserName = "owner1" }
            };
            var spot2 = new VacationSpot
            {
                SpotId = 2,
                Title = "Spot Two",
                OwnerId = "owner2",
                Owner = new ApplicationUser { Id = "owner2", UserName = "owner2" }
            };

            _context.VacationSpots.AddRange(spot1, spot2);
            await _context.SaveChangesAsync();

            var result = await _vacationSpotService.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectSpot()
        {
            var spot = new VacationSpot
            {
                SpotId = 100,
                Title = "Test Spot",
                OwnerId = "owner100",
                Owner = new ApplicationUser { Id = "owner100", UserName = "owner100" }
            };
            _context.VacationSpots.Add(spot);
            await _context.SaveChangesAsync();

            var result = await _vacationSpotService.GetByIdAsync(100);

            Assert.NotNull(result);
            Assert.Equal(100, result.SpotId);
            Assert.Equal("Test Spot", result.Title);
        }

        [Fact]
        public async Task CreateAsync_AddsSpot()
        {
            var spot = new VacationSpot
            {
                SpotId = 200,
                Title = "New Spot",
                OwnerId = "owner200",
                Owner = new ApplicationUser { Id = "owner200", UserName = "owner200" }
            };

            await _vacationSpotService.CreateAsync(spot);

            var result = await _context.VacationSpots.FindAsync(200);
            Assert.NotNull(result);
            Assert.Equal("New Spot", result.Title);
        }

        [Fact]
        public async Task UpdateAsync_ModifiesSpot()
        {
            var spot = new VacationSpot
            {
                SpotId = 300,
                Title = "Old Name",
                OwnerId = "owner300",
                Owner = new ApplicationUser { Id = "owner300", UserName = "owner300" }
            };
            _context.VacationSpots.Add(spot);
            await _context.SaveChangesAsync();

            spot.Title = "New Name";
            await _vacationSpotService.UpdateAsync(spot);

            var updated = await _context.VacationSpots.FindAsync(300);
            Assert.Equal("New Name", updated.Title);
        }
    }
}
