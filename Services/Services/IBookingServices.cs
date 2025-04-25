using Models;

namespace Services.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId);
        Task<bool> IsSpotAvailableAsync(int spotId, DateTime startDate, DateTime endDate);
        Task CreateBookingAsync(Booking booking);
        Task CancelBookingAsync(int bookingId);
        Task<string?> GetByIdAsync(int id);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(int id);
        Task<string?> GetAllAsync();
    }
}
