using Models;
using Services.Repositories;

namespace Services.Services
{
    public class SearchService : ISearchService
    {
        private readonly IVacationSpotRepository _vacationSpotRepo;

        public SearchService(IVacationSpotRepository vacationSpotRepo)
        {
            _vacationSpotRepo = vacationSpotRepo;
        }

        public async Task<IEnumerable<VacationSpot>> SearchAsync(string location, decimal? minPrice, decimal? maxPrice, DateTime? startDate, DateTime? endDate)
        {
            var spots = await _vacationSpotRepo.GetAllAsync();

            if (!string.IsNullOrEmpty(location))
                spots = spots.Where(s => s.Location.ToLower().Contains(location.ToLower()));

            if (minPrice.HasValue)
                spots = spots.Where(s => s.PricePerNight >= minPrice.Value);

            if (maxPrice.HasValue)
                spots = spots.Where(s => s.PricePerNight <= maxPrice.Value);

            if (startDate.HasValue && endDate.HasValue)
            {
                spots = spots.Where(s =>
                    !s.Bookings.Any(b =>
                        (startDate >= b.StartDate && startDate < b.EndDate) ||
                        (endDate > b.StartDate && endDate <= b.EndDate)));
            }

            return spots;
        }
    }
}
