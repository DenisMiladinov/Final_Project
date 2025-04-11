using Models;

namespace Services.Services
{
    public interface IVacationSpotService
    {
        Task<IEnumerable<VacationSpot>> GetAllAsync();
        Task<VacationSpot> GetByIdAsync(int id);
        Task CreateAsync(VacationSpot spot);
        Task UpdateAsync(VacationSpot spot);
        Task DeleteAsync(int id);
        Task<IEnumerable<VacationSpot>> GetByLocationAsync(string location);
        Task<IEnumerable<VacationSpot>> GetAvailableSpotsAsync(DateTime startDate, DateTime endDate);
    }
}
