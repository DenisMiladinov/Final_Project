using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Xunit;
using Models;
using Services.Repositories;
using Services.Services;

namespace UnitTests.Services
{
    public class BookingServiceTests
    {
        private BookingService CreateService(Mock<IBookingRepository> mockRepo)
            => new BookingService(mockRepo.Object);

        [Fact]
        public async Task GetByIdAsync_DelegatesToRepository()
        {
            var mockRepo = new Mock<IBookingRepository>();
            var booking = new Booking { BookingId = 42 };
            mockRepo.Setup(r => r.GetByIdAsync(42)).ReturnsAsync(booking);

            var svc = CreateService(mockRepo);
            var result = await svc.GetByIdAsync(42);

            Assert.Equal(42, result.BookingId);
            mockRepo.Verify(r => r.GetByIdAsync(42), Times.Once);
        }

        [Fact]
        public async Task GetAllAsync_DelegatesToRepository()
        {
            var list = new List<Booking> { new Booking(), new Booking() };
            var mockRepo = new Mock<IBookingRepository>();
            mockRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(list);

            var svc = CreateService(mockRepo);
            var result = await svc.GetAllAsync();

            Assert.Equal(2, ((List<Booking>)result).Count);
            mockRepo.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task GetBookingsByUserIdAsync_DelegatesToRepository()
        {
            var list = new List<Booking> { new Booking { UserId = "u1" } };
            var mockRepo = new Mock<IBookingRepository>();
            mockRepo.Setup(r => r.GetBookingsByUserIdAsync("u1")).ReturnsAsync(list);

            var svc = CreateService(mockRepo);
            var result = await svc.GetBookingsByUserIdAsync("u1");

            Assert.Single(result);
            Assert.All(result, b => Assert.Equal("u1", b.UserId));
            mockRepo.Verify(r => r.GetBookingsByUserIdAsync("u1"), Times.Once);
        }

        [Fact]
        public async Task IsSpotAvailableAsync_DelegatesToRepository()
        {
            var mockRepo = new Mock<IBookingRepository>();
            mockRepo.Setup(r => r.IsSpotAvailableAsync(5, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .ReturnsAsync(true);

            var svc = CreateService(mockRepo);
            var available = await svc.IsSpotAvailableAsync(5, DateTime.Today, DateTime.Today.AddDays(1));

            Assert.True(available);
            mockRepo.Verify(r => r.IsSpotAvailableAsync(5, It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task CreateBookingAsync_CallsAddAndSave()
        {
            var mockRepo = new Mock<IBookingRepository>();
            var booking = new Booking { BookingId = 7 };
            var svc = CreateService(mockRepo);

            await svc.CreateBookingAsync(booking);

            mockRepo.Verify(r => r.AddAsync(booking), Times.Once);
            mockRepo.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CallsUpdateAndSave()
        {
            var mockRepo = new Mock<IBookingRepository>();
            var booking = new Booking { BookingId = 8 };
            var svc = CreateService(mockRepo);

            await svc.UpdateAsync(booking);

            mockRepo.Verify(r => r.Update(booking), Times.Once);
            mockRepo.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_RemovesAndSaves_WhenExists()
        {
            var mockRepo = new Mock<IBookingRepository>();
            var booking = new Booking { BookingId = 9 };
            mockRepo.Setup(r => r.GetByIdAsync(9)).ReturnsAsync(booking);

            var svc = CreateService(mockRepo);
            await svc.DeleteAsync(9);

            mockRepo.Verify(r => r.GetByIdAsync(9), Times.Once);
            mockRepo.Verify(r => r.Delete(booking), Times.Once);
            mockRepo.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DoesNothing_WhenNotExists()
        {
            var mockRepo = new Mock<IBookingRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Booking)null);

            var svc = CreateService(mockRepo);
            await svc.DeleteAsync(99);

            mockRepo.Verify(r => r.GetByIdAsync(99), Times.Once);
            mockRepo.Verify(r => r.Delete(It.IsAny<Booking>()), Times.Never);
            mockRepo.Verify(r => r.SaveAsync(), Times.Never);
        }

        [Fact]
        public async Task CancelBookingAsync_RemovesAndSaves_WhenExists()
        {
            var mockRepo = new Mock<IBookingRepository>();
            var booking = new Booking { BookingId = 10 };
            mockRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(booking);

            var svc = CreateService(mockRepo);
            await svc.CancelBookingAsync(10);

            mockRepo.Verify(r => r.GetByIdAsync(10), Times.Once);
            mockRepo.Verify(r => r.Delete(booking), Times.Once);
            mockRepo.Verify(r => r.SaveAsync(), Times.Once);
        }

        [Fact]
        public async Task CancelBookingAsync_DoesNothing_WhenNotExists()
        {
            var mockRepo = new Mock<IBookingRepository>();
            mockRepo.Setup(r => r.GetByIdAsync(123)).ReturnsAsync((Booking)null);

            var svc = CreateService(mockRepo);
            await svc.CancelBookingAsync(123);

            mockRepo.Verify(r => r.GetByIdAsync(123), Times.Once);
            mockRepo.Verify(r => r.Delete(It.IsAny<Booking>()), Times.Never);
            mockRepo.Verify(r => r.SaveAsync(), Times.Never);
        }
    }
}
