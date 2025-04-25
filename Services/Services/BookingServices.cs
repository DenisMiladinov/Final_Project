using Models;
using Services.Repositories;

namespace Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId)
        {
            return await _bookingRepository.GetBookingsByUserIdAsync(userId);
        }

        public async Task<bool> IsSpotAvailableAsync(int spotId, DateTime startDate, DateTime endDate)
        {
            return await _bookingRepository.IsSpotAvailableAsync(spotId, startDate, endDate);
        }

        public async Task CreateBookingAsync(Booking booking)
        {
            await _bookingRepository.AddAsync(booking);
            await _bookingRepository.SaveAsync();
        }

        public async Task CancelBookingAsync(int bookingId)
        {
            var booking = await _bookingRepository.GetByIdAsync(bookingId);
            if (booking != null)
            {
                _bookingRepository.Delete(booking);
                await _bookingRepository.SaveAsync();
            }
        }

        public Task<string?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Booking booking)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<string?> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
