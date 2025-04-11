using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface IBookingService
    {
        Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId);
        Task<bool> IsSpotAvailableAsync(int spotId, DateTime startDate, DateTime endDate);
        Task CreateBookingAsync(Booking booking);
        Task CancelBookingAsync(int bookingId);
    }
}
