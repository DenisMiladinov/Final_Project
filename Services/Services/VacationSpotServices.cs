using Models;
using Services.Repositories;
using static Services.Services.IVacationSpotServices;

namespace Services.Services
{
    public class VacationSpotService : IVacationSpotService
    {
        private readonly IVacationSpotRepository _vacationSpotRepository;

        public VacationSpotService(IVacationSpotRepository vacationSpotRepository)
        {
            _vacationSpotRepository = vacationSpotRepository;
        }

        public async Task<IEnumerable<VacationSpot>> GetAllAsync()
        {
            return await _vacationSpotRepository.GetAllAsync();
        }

        public async Task<VacationSpot> GetByIdAsync(int id)
        {
            return await _vacationSpotRepository.GetByIdAsync(id);
        }

        public async Task CreateAsync(VacationSpot spot)
        {
            await _vacationSpotRepository.AddAsync(spot);
            await _vacationSpotRepository.SaveAsync();
        }

        public async Task UpdateAsync(VacationSpot spot)
        {
            _vacationSpotRepository.Update(spot);
            await _vacationSpotRepository.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var spot = await _vacationSpotRepository.GetByIdAsync(id);
            if (spot != null)
            {
                _vacationSpotRepository.Delete(spot);
                await _vacationSpotRepository.SaveAsync();
            }
        }

        public async Task<IEnumerable<VacationSpot>> GetByLocationAsync(string location)
        {
            return await _vacationSpotRepository.GetByLocationAsync(location);
        }

        public async Task<IEnumerable<VacationSpot>> GetAvailableSpotsAsync(DateTime startDate, DateTime endDate)
        {
            return await _vacationSpotRepository.GetAvailableSpotsAsync(startDate, endDate);
        }
    }
}
