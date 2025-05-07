using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using Stripe;

namespace Services.Services
{
    public class VacationSpotService : IVacationSpotService
    {
        private readonly ApplicationDbContext _context;
        private readonly IReviewService _reviewService;

        public VacationSpotService(ApplicationDbContext context, IReviewService reviewService)
        {
            _context = context;
            _reviewService = reviewService;
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
        public async Task<VacationSpotDetailsViewModel> BuildDetailsViewModelAsync(int spotId)
        {
            var spot = await GetByIdAsync(spotId);
            var reviews = await _reviewService.GetBySpotIdAsync(spotId);
            var avg = await _reviewService.GetAverageRatingAsync(spotId);
            var count = await _reviewService.GetReviewCountAsync(spotId);

            return new VacationSpotDetailsViewModel
            {
                Spot = spot,
                Reviews = reviews,
                AverageRating = avg,
                ReviewCount = count,
                NewReview = new ReviewViewModel { SpotId = spotId }
            };
        }
    }
}
