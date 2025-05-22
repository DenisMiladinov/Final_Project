using Xunit;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

public class BookingRepositoryTests
{
    private ApplicationDbContext _context;
    private BookingRepository _repository;

    public BookingRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "BookingRepoTests")
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new BookingRepository(_context);
    }

    [Fact]
    public async Task GetBookingsByUserIdAsync_ReturnsOnlyUserBookings()
    {
        _context.Bookings.Add(new Booking { BookingId = 1, UserId = "user1" });
        _context.Bookings.Add(new Booking { BookingId = 2, UserId = "user2" });
        await _context.SaveChangesAsync();

        var result = await _repository.GetBookingsByUserIdAsync("user1");

        Assert.Single(result);
        Assert.All(result, b => Assert.Equal("user1", b.UserId));
    }

    [Fact]
    public async Task IsSpotAvailableAsync_ReturnsFalse_WhenOverlappingBookingExists()
    {
        var booking = new Booking
        {
            SpotId = 5,
            StartDate = new DateTime(2025, 6, 10),
            EndDate = new DateTime(2025, 6, 15)
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var available = await _repository.IsSpotAvailableAsync(
            5,
            new DateTime(2025, 6, 12),
            new DateTime(2025, 6, 17)
        );

        Assert.False(available);
    }

    [Fact]
    public async Task IsSpotAvailableAsync_ReturnsTrue_WhenNoOverlap()
    {
        var booking = new Booking
        {
            SpotId = 6,
            StartDate = new DateTime(2025, 6, 1),
            EndDate = new DateTime(2025, 6, 5)
        };

        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var available = await _repository.IsSpotAvailableAsync(
            6,
            new DateTime(2025, 6, 10),
            new DateTime(2025, 6, 15)
        );

        Assert.True(available);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllBookingsWithIncludes()
    {
        var vacationSpot = new VacationSpot { SpotId = 1 };
        var user = new ApplicationUser { Id = "u1" };
        var booking = new Booking { BookingId = 10, VacationSpot = vacationSpot, User = user };

        _context.VacationSpots.Add(vacationSpot);
        _context.Users.Add(user);
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        var result = await _repository.GetAllAsync();

        Assert.Single(result);
        Assert.NotNull(result.First().VacationSpot);
        Assert.NotNull(result.First().User);
    }
}
