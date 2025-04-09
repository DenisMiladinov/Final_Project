using Final_Project.Repositories;
using Microsoft.EntityFrameworkCore;
using Models;

namespace Server.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Booking>> GetBookingsByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(b => b.VacationSpot)
                .Where(b => b.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> IsSpotAvailableAsync(int spotId, DateTime startDate, DateTime endDate)
        {
            return !await _dbSet.AnyAsync(b =>
                b.SpotId == spotId &&
                ((startDate >= b.StartDate && startDate < b.EndDate) ||
                 (endDate > b.StartDate && endDate <= b.EndDate)));
        }
    }
}
