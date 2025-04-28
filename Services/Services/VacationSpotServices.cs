using Microsoft.EntityFrameworkCore;
using Models;

namespace Services.Services
{
    public class VacationSpotService : IVacationSpotService
    {
        private readonly ApplicationDbContext _context;

        public VacationSpotService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VacationSpot>> GetAllAsync()
        {
            return await _context.VacationSpots
                                 .Include(v => v.Category)
                                 .Include(v => v.Images)
                                 .ToListAsync();
        }

        public async Task<VacationSpot?> GetByIdAsync(int id)
        {
            return await _context.VacationSpots
                                 .Include(v => v.Category)
                                 .Include(v => v.Images)
                                 .FirstOrDefaultAsync(v => v.SpotId == id);
        }

        public async Task CreateAsync(VacationSpot spot)
        {
            _context.VacationSpots.Add(spot);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(VacationSpot spot)
        {
            _context.VacationSpots.Update(spot);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var spot = await _context.VacationSpots.FindAsync(id);
            if (spot != null)
            {
                _context.VacationSpots.Remove(spot);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<VacationSpot>> GetByLocationAsync(string location)
        {
            return await _context.VacationSpots
                                 .Where(v => v.Location.Contains(location))
                                 .Include(v => v.Images)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<VacationSpot>> GetAvailableSpotsAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.VacationSpots
                                 .Where(v => !v.Bookings.Any(b =>
                                       (startDate >= b.StartDate && startDate < b.EndDate) ||
                                       (endDate > b.StartDate && endDate <= b.EndDate)))
                                 .Include(v => v.Images)
                                 .ToListAsync();
        }
    }
}
