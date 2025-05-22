using Xunit;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace UnitTests.Repositories
{
    public class GenericRepositoryTests
    {
        private readonly ApplicationDbContext _context;
        private readonly GenericRepository<Booking> _repository;

        public GenericRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("GenericRepoTests")
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new GenericRepository<Booking>(_context);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllEntities()
        {
            _context.Bookings.Add(new Booking { BookingId = 1 });
            _context.Bookings.Add(new Booking { BookingId = 2 });
            await _context.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsEntity_WhenExists()
        {
            var booking = new Booking { BookingId = 3 };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(3);

            Assert.NotNull(result);
            Assert.Equal(3, result.BookingId);
        }

        [Fact]
        public async Task AddAsync_AddsEntity()
        {
            var booking = new Booking { BookingId = 4 };

            await _repository.AddAsync(booking);
            await _repository.SaveAsync();

            var result = await _context.Bookings.FindAsync(4);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task Update_UpdatesEntity()
        {
            var booking = new Booking { BookingId = 5, StripeSessionId = "old" };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            booking.StripeSessionId = "new";
            _repository.Update(booking);
            await _repository.SaveAsync();

            var updated = await _context.Bookings.FindAsync(5);
            Assert.Equal("new", updated.StripeSessionId);
        }

        [Fact]
        public async Task Delete_RemovesEntity()
        {
            var booking = new Booking { BookingId = 6 };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            _repository.Delete(booking);
            await _repository.SaveAsync();

            var result = await _context.Bookings.FindAsync(6);
            Assert.Null(result);
        }
    }
}
