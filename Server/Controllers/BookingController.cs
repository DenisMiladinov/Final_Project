using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;
using Services.Services;
using Microsoft.AspNetCore.Identity;
using Stripe.Checkout;
using Models.ViewModels;
using Microsoft.Extensions.Configuration;

namespace Server.Controllers
{
    [Authorize]
    public class BookingController : Controller
    {
        private readonly IBookingService _bookingService;
        private readonly IVacationSpotService _spotService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _config;

        public BookingController(IBookingService bookingService, IVacationSpotService spotService, UserManager<ApplicationUser> userManager, IConfiguration config)
        {
            _bookingService = bookingService;
            _spotService = spotService;
            _userManager = userManager;
            _config = config;
        }

        public async Task<IActionResult> MyBookings()
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            return View(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(int spotId, DateTime startDate, DateTime endDate)
        {
            var spot = await _spotService.GetByIdAsync(spotId);
            if (spot == null) return NotFound();

            Stripe.StripeConfiguration.ApiKey = _config["Stripe:SecretKey"];

            var domain = $"{Request.Scheme}://{Request.Host}";
            var successUrl = $"{domain}/Booking/Success?spotId={spotId}&session_id={{CHECKOUT_SESSION_ID}}";
            var cancelUrl = $"{domain}/VacationSpot/Details/{spotId}";
            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {
            new SessionLineItemOptions
            {
                PriceData = new SessionLineItemPriceDataOptions
                {
                    UnitAmountDecimal = spot.PricePerNight * 100,
                    Currency = "bgn",
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = spot.Title
                    }
                },
                Quantity = 1
            }
        },
                Mode = "payment",
                SuccessUrl = successUrl,
                CancelUrl = cancelUrl
            };
            var service = new SessionService();
            var session = await service.CreateAsync(options);
            return Redirect(session.Url);
        }

        public async Task<IActionResult> Success(int spotId, string session_id)
        {
            var service = new SessionService();
            var session = service.Get(session_id);

            if (session.PaymentStatus != "paid")
                return RedirectToAction("Create", new { spotId });

            var spot = await _spotService.GetByIdAsync(spotId);
            var userId = _userManager.GetUserId(User);
            var booking = new Booking
            {
                SpotId = spotId,
                UserId = _userManager.GetUserId(User),
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1),
                TotalPrice = spot.PricePerNight,
                StripeSessionId = session_id,
                CreatedAt = DateTime.Now
            };

            await _bookingService.CreateBookingAsync(booking);
            return View("Confirmation", booking);
        }


        public IActionResult Create(int spotId)
        {
            var m = new Booking
            {
                SpotId = spotId,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            };
            return View(m);
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

            return RedirectToAction(nameof(Checkout), 
                new{
                    spotId = booking.SpotId,
                    startDate = booking.StartDate,
                    endDate = booking.EndDate
                });
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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Confirmation(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return View("Confirmation", booking);
        }

    }
}