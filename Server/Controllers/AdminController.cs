using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Services;
using Models;

namespace Server.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IVacationSpotService _vacationSpotService;
        private readonly IBookingService _bookingService;

        public AdminController(
            IAdminService adminService,
            IVacationSpotService vacationSpotService,
            IBookingService bookingService)
        {
            _adminService = adminService;
            _vacationSpotService = vacationSpotService;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var spots = await _adminService.GetAllVacationSpotsAsync();
            var bookings = await _adminService.GetAllBookingsAsync();
            var users = await _adminService.GetAllUsersAsync();

            ViewBag.BookingCount = bookings?.Count() ?? 0;
            ViewBag.UserCount = users?.Count() ?? 0;
            ViewBag.TotalSpots = spots?.Count() ?? 0;

            return View("Dashboard");
        }

        public async Task<IActionResult> ManageVacationSpots()
        {
            var spots = await _vacationSpotService.GetAllAsync();
            return View(spots);
        }

        public async Task<IActionResult> EditVacationSpot(int id)
        {
            var spot = await _vacationSpotService.GetByIdAsync(id);
            if (spot == null) return NotFound();
            return View(spot);
        }

        [HttpPost]
        public async Task<IActionResult> EditVacationSpot(VacationSpot spot)
        {
            if (!ModelState.IsValid) return View(spot);
            await _vacationSpotService.UpdateAsync(spot);
            return RedirectToAction(nameof(ManageVacationSpots));
        }

        public async Task<IActionResult> DeleteVacationSpot(int id)
        {
            await _vacationSpotService.DeleteAsync(id);
            return RedirectToAction(nameof(ManageVacationSpots));
        }

        /*public async Task<IActionResult> ManageBookings()
        {
            var bookings = await _bookingService.GetAllAsync();
            return View(bookings);
        }

        public async Task<IActionResult> EditBooking(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpPost]
        public async Task<IActionResult> EditBooking(Booking booking)
        {
            if (!ModelState.IsValid) return View(booking);
            await _bookingService.UpdateAsync(booking);
            return RedirectToAction(nameof(ManageBookings));
        }*/

        public async Task<IActionResult> ManageUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return View(users);
        }
    }
}
