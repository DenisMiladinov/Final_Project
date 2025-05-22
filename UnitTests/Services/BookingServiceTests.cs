using Moq;
using Xunit;
using Services.Services;
using Services.Repositories;
using Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace UnitTests.Services
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _bookingRepoMock;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _bookingRepoMock = new Mock<IBookingRepository>();
            _bookingService = new BookingService(_bookingRepoMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsBooking_WhenFound()
        {
            var booking = new Booking { BookingId = 1 };
            _bookingRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(booking);

            var result = await _bookingService.GetByIdAsync(1);

            Assert.Equal(1, result?.BookingId);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllBookings()
        {
            var bookings = new List<Booking> { new Booking(), new Booking() };
            _bookingRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(bookings);

            var result = await _bookingService.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetBookingsByUserIdAsync_ReturnsCorrectBookings()
        {
            var bookings = new List<Booking> { new Booking(), new Booking() };
            _bookingRepoMock.Setup(r => r.GetBookingsByUserIdAsync("user123")).ReturnsAsync(bookings);

            var result = await _bookingService.GetBookingsByUserIdAsync("user123");

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task IsSpotAvailableAsync_ReturnsAvailability()
        {
            _bookingRepoMock.Setup(r => r.IsSpotAvailableAsync(1, It.IsAny<DateTime>(), It.IsAny<DateTime>())).ReturnsAsync(true);

            var result = await _bookingService.IsSpotAvailableAsync(1, DateTime.Now, DateTime.Now.AddDays(2));

            Assert.True(result);
        }

        [Fact]
        public async Task CreateBookingAsync_CallsAddAndSave()
        {
            var booking = new Booking();

            await _bookingService.CreateBookingAsync(booking);

            _bookingRepoMock.Verify(r => r.AddAsync(booking), Times.Once);
            _bookingRepoMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CallsUpdateAndSave()
        {
            var booking = new Booking();

            await _bookingService.UpdateAsync(booking);

            _bookingRepoMock.Verify(r => r.Update(booking), Times.Once);
            _bookingRepoMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenBookingExists_CallsDeleteAndSave()
        {
            var booking = new Booking { BookingId = 1 };
            _bookingRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(booking);

            await _bookingService.DeleteAsync(1);

            _bookingRepoMock.Verify(r => r.Delete(booking), Times.Once);
            _bookingRepoMock.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenBookingDoesNotExist_DoesNothing()
        {
            _bookingRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((Booking)null);

            await _bookingService.DeleteAsync(1);

            _bookingRepoMock.Verify(r => r.Delete(It.IsAny<Booking>()), Times.Never);
            _bookingRepoMock.Verify(r => r.SaveAsync(), Times.Never);
        }
    }
}
