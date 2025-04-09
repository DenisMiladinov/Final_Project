using Models;

namespace Server.Repositories
{
    public interface IVacationSpotRepository
    {
        public interface IVacationSpotRepository : IGenericRepository<VacationSpot>
        {
            Task<IEnumerable<VacationSpot>> GetByLocationAsync(string location);
            Task<IEnumerable<VacationSpot>> GetAvailableSpotsAsync(DateTime startDate, DateTime endDate);
        }
    }
}
