using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;

namespace Server.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var spots = await _adminService.GetAllVacationSpotsAsync();
            var bookings = await _adminService.GetAllBookingsAsync();
            var users = await _adminService.GetAllUsersAsync();

            ViewBag.BookingCount = bookings?.Count() ?? 0;
            ViewBag.UserCount = users?.Count() ?? 0;
            ViewBag.TotalSpots = spots?.Count() ?? 0;

            return View(spots);
        }
    }
}
