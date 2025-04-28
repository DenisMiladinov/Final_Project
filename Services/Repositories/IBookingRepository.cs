using Models;
using Services.Repositories;

public interface IBookingRepository : IGenericRepository<Booking>
{
    Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId);
    Task<bool> IsSpotAvailableAsync(int spotId, DateTime startDate, DateTime endDate);

    Task<IEnumerable<Booking>> GetAllAsync();
}
