using Microsoft.AspNetCore.Identity;
using Models;

namespace Services.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<VacationSpot>> GetAllVacationSpotsAsync();
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<IEnumerable<IdentityUser>> GetAllUsersAsync();
    }
}
