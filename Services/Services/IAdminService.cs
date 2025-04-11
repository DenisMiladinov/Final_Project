using Microsoft.AspNetCore.Identity;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<VacationSpot>> GetAllVacationSpotsAsync();
        Task<IEnumerable<Booking>> GetAllBookingsAsync();
        Task<IEnumerable<IdentityUser>> GetAllUsersAsync();
    }
}
