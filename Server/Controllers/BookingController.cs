using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;
using Services.Services;

namespace Server.Controllers
{
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

        // POST: /Booking/Cancel/5
        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            await _bookingService.CancelBookingAsync(id);
            return RedirectToAction(nameof(MyBookings));
        }
    }
}
