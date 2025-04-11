using Models;

namespace Services.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<VacationSpot>> SearchAsync(string location, decimal? minPrice, decimal? maxPrice, DateTime? startDate, DateTime? endDate);
    }
}
