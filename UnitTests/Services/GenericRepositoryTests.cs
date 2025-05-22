using Xunit;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTests.Repositories
{
    public class GenericRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly GenericRepository<Booking> _repository;

        public GenericRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new GenericRepository<Booking>(_context);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            var b1 = new Booking
            {
                BookingId = 1,
                SpotId = 1,
                UserId = "u1",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(1)
            };
            var b2 = new Booking
            {
                BookingId = 2,
                SpotId = 2,
                UserId = "u2",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(2)
            };
            _context.Bookings.AddRange(b1, b2);
            await _context.SaveChangesAsync();

            var all = await _repository.GetAllAsync();

            Assert.Equal(2, all.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsEntity_WhenExists()
        {
            var booking = new Booking
            {
                BookingId = 3,
                SpotId = 3,
                UserId = "u3",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(3)
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var found = await _repository.GetByIdAsync(3);

            Assert.NotNull(found);
            Assert.Equal(3, found.BookingId);
        }

        [Fact]
        public async Task AddAsync_AddsEntity()
        {
            var booking = new Booking
            {
                BookingId = 4,
                SpotId = 4,
                UserId = "u4",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(4)
            };

            await _repository.AddAsync(booking);
            await _repository.SaveAsync();

            var inserted = await _context.Bookings.FindAsync(4);
            Assert.NotNull(inserted);
        }

        [Fact]
        public async Task Update_UpdatesEntity()
        {
            var booking = new Booking
            {
                BookingId = 5,
                SpotId = 5,
                UserId = "u5",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(5),
                StripeSessionId = "old-session"
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            booking.StripeSessionId = "new-session";
            _repository.Update(booking);
            await _repository.SaveAsync();

            var updated = await _context.Bookings.FindAsync(5);
            Assert.Equal("new-session", updated.StripeSessionId);
        }

        [Fact]
        public async Task Delete_RemovesEntity()
        {
            var booking = new Booking
            {
                BookingId = 6,
                SpotId = 6,
                UserId = "u6",
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(6)
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            _repository.Delete(booking);
            await _repository.SaveAsync();

            var gone = await _context.Bookings.FindAsync(6);
            Assert.Null(gone);
        }
    }
}
