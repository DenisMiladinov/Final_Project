using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;
using Services.Services;

namespace Server.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;

        public BookingController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        public async Task<IActionResult> MyBookings()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            return View(bookings);
        }

        public IActionResult Create(int spotId)
        {
            ViewBag.SpotId = spotId;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Booking booking)
        {
            if (!ModelState.IsValid) return View(booking);

            var available = await _bookingService.IsSpotAvailableAsync(booking.SpotId, booking.StartDate, booking.EndDate);
            if (!available)
            {
                ModelState.AddModelError("", "Selected dates are not available.");
                return View(booking);
            }

            booking.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            booking.TotalPrice = (decimal)(booking.EndDate - booking.StartDate).TotalDays * booking.VacationSpot?.PricePerNight ?? 0;

            await _bookingService.CreateBookingAsync(booking);
            return RedirectToAction(nameof(MyBookings));
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            await _bookingService.CancelBookingAsync(id);
            return RedirectToAction(nameof(MyBookings));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Booking booking)
        {
            if (id != booking.BookingId) return BadRequest();
            if (!ModelState.IsValid) return View(booking);

            await _bookingService.UpdateAsync(booking);
            return RedirectToAction("ManageBookings", "Admin");
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return View(booking);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _bookingService.DeleteAsync(id);
            return RedirectToAction("ManageBookings", "Admin");
        }
    }
}