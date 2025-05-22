using Xunit;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Linq;
using System.Threading.Tasks;

public class BookingRepositoryTests
{
    private ApplicationDbContext _context;
    private BookingRepository _repository;

    public BookingRepositoryTests()
    {
        // Unique DB per test run
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new BookingRepository(_context);
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_ReturnsCorrectBookings()
    {
        var userId = "user123";
        var spot = new VacationSpot
        {
            SpotId = 1,
            OwnerId = "admin1",
            Title = "Test Spot"
        };

        var booking1 = new Booking
        {
            BookingId = 1,
            SpotId = 1,
            VacationSpot = spot,
            UserId = userId,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(2)
        };

        var booking2 = new Booking
        {
            BookingId = 2,
            SpotId = 1,
            VacationSpot = spot,
            UserId = "other-user",
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(3)
        };

        _context.VacationSpots.Add(spot);
        _context.Bookings.AddRange(booking1, booking2);
        await _context.SaveChangesAsync();

        var result = await _repository.GetBookingsByUserIdAsync(userId);

        Assert.Single(result);
        Assert.Equal(userId, result.First().UserId);
    }

    [Fact]
    public async Task IsSpotAvailableAsync_ReturnsFalse_WhenOverlapExists()
    {
        var booking = new Booking
        {
            BookingId = 1,
            SpotId = 10,
            UserId = "user1",
            StartDate = new DateTime(2025, 6, 10),
            EndDate = new DateTime(2025, 6, 15)
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var result = await _repository.IsSpotAvailableAsync(
            10,
            new DateTime(2025, 6, 12),
            new DateTime(2025, 6, 14)
        );

        Assert.False(result);
    }

    [Fact]
    public async Task IsSpotAvailableAsync_ReturnsTrue_WhenNoOverlap()
    {
        var booking = new Booking
        {
            BookingId = 1,
            SpotId = 20,
            UserId = "user1",
            StartDate = new DateTime(2025, 6, 1),
            EndDate = new DateTime(2025, 6, 5)
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var result = await _repository.IsSpotAvailableAsync(
            20,
            new DateTime(2025, 6, 10),
            new DateTime(2025, 6, 12)
        );

        Assert.True(result);
    }
}