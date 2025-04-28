using Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface IBookingService
    {
        Task<Booking?> GetByIdAsync(int id);
        Task<IEnumerable<Booking>> GetAllAsync();
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId);
        Task<bool> IsSpotAvailableAsync(int spotId, DateTime startDate, DateTime endDate);
        Task CreateBookingAsync(Booking booking);
        Task UpdateAsync(Booking booking);
        Task DeleteAsync(int id);
        Task CancelBookingAsync(int bookingId);
    }
}
