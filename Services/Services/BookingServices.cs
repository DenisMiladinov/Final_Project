using Models;

namespace Services.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public async Task<Booking?> GetByIdAsync(int id)
        {
            return await _bookingRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _bookingRepository.GetAllAsync();
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

        public async Task UpdateAsync(Booking booking)
        {
            _bookingRepository.Update(booking);
            await _bookingRepository.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking != null)
            {
                _bookingRepository.Delete(booking);
                await _bookingRepository.SaveAsync();
            }
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
    }
}
