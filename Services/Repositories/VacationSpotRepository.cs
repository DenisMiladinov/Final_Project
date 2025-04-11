using Microsoft.EntityFrameworkCore;
using Models;

namespace Services.Repositories
{
    public class VacationSpotRepository : GenericRepository<VacationSpot>, IVacationSpotRepository
    {
        public VacationSpotRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<VacationSpot>> GetByLocationAsync(string location)
        {
            return await _dbSet
                .Where(v => v.Location.Contains(location))
                .ToListAsync();
        }

        public async Task<IEnumerable<VacationSpot>> GetAvailableSpotsAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(v => !v.Bookings.Any(b =>
                    (startDate >= b.StartDate && startDate < b.EndDate) ||
                    (endDate > b.StartDate && endDate <= b.EndDate)))
                .Include(v => v.Images)
                .ToListAsync();
        }
    }
}
