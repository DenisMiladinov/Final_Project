using Models;

namespace Services.Repositories
{
    public interface IVacationSpotRepository : IGenericRepository<VacationSpot>
    {
        Task<IEnumerable<VacationSpot>> GetByLocationAsync(string location);
        Task<IEnumerable<VacationSpot>> GetAvailableSpotsAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<VacationSpot>> GetAllAsync();
    }
}
