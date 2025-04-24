using Microsoft.AspNetCore.Identity;
using Models;
using Services.Repositories;

namespace Services.Services
{
    public class AdminService : IAdminService
    {
        private readonly IVacationSpotRepository _vacationRepo;
        private readonly IBookingRepository _bookingRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(
            IVacationSpotRepository vacationRepo,
            IBookingRepository bookingRepo,
            UserManager<ApplicationUser> userManager)
        {
            _vacationRepo = vacationRepo;
            _bookingRepo = bookingRepo;
            _userManager = userManager;
        }


        public async Task<IEnumerable<VacationSpot>> GetAllVacationSpotsAsync()
        {
            return await _vacationRepo.GetAllAsync();
        }

        public async Task<IEnumerable<Booking>> GetAllBookingsAsync()
        {
            return await _bookingRepo.GetAllAsync();
        }

        public async Task<IEnumerable<IdentityUser>> GetAllUsersAsync()
        {
            return await Task.FromResult(_userManager.Users); // Queryable to IEnumerable
        }

    }
}
