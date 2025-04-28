using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.ViewModels;
using Server.Areas.Admin.ViewModels;
using Services.Services;

namespace Server.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IVacationSpotService _spotService;
        private readonly IBookingService _bookingService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            IVacationSpotService spotService,
            IBookingService bookingService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context
        )
        {
            _spotService = spotService;
            _bookingService = bookingService;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new AdminDashboardViewModel
            {
                TotalSpots = (await _spotService.GetAllAsync()).Count(),
                TotalBookings = (await _bookingService.GetAllAsync()).Count(),
                TotalUsers = await _userManager.Users.CountAsync(),
                TotalCategories = await _context.Categories.CountAsync()
            };
            return View(vm);
        }


        //---------------------------------------------------SPOTS---------------------------------------------------
        private async Task PopulateCategoriesAsync()
        {
            var cats = await _context.Categories
                                     .OrderBy(c => c.Name)
                                     .ToListAsync();
            ViewBag.Categories = cats;
        }
        public async Task<IActionResult> Spots()
        {
            var model = await _spotService.GetAllAsync();
            return View("VacationSpot/Spots", model);
        }
        public async Task<IActionResult> CreateSpot()
        {
            await PopulateCategoriesAsync();
            return View("VacationSpot/CreateSpot", new VacationSpot());
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSpot(VacationSpot m, IFormFile? ImageFile)
        {
            await PopulateCategoriesAsync();
            if (!ModelState.IsValid) return View("VacationSpot/CreateSpot", m);
            await _spotService.CreateAsync(m);
            return RedirectToAction(nameof(Spots));
        }

        public async Task<IActionResult> EditSpot(int id)
        {
            await PopulateCategoriesAsync();
            var m = await _spotService.GetByIdAsync(id);
            if (m == null) return NotFound();
            return View("VacationSpot/EditSpot", m);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSpot(int id, VacationSpot m, IFormFile? ImageFile)
        {
            await PopulateCategoriesAsync();
            if (id != m.SpotId) return BadRequest();
            if (!ModelState.IsValid) return View("VacationSpot/EditSpot", m);
            await _spotService.UpdateAsync(m);
            return RedirectToAction(nameof(Spots));
        }

        public async Task<IActionResult> DeleteSpot(int id)
        {
            await PopulateCategoriesAsync();
            var m = await _spotService.GetByIdAsync(id);
            if (m == null) return NotFound();
            return View("VacationSpot/DeleteSpot", m);
        }

        [HttpPost, ActionName("DeleteSpot"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSpotConfirmed(int id)
        {
            await PopulateCategoriesAsync();
            await _spotService.DeleteAsync(id);
            return RedirectToAction(nameof(Spots));
        }
        //---------------------------------------------------BOOKINGS---------------------------------------------------
        public async Task<IActionResult> Bookings()
        {
            var model = await _bookingService.GetAllAsync();
            return View("Booking/Bookings", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int id)
        {
            await _bookingService.CancelBookingAsync(id);
            return RedirectToAction(nameof(Bookings));
        }

        public async Task<IActionResult> EditBooking(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return View("Booking/EditBooking", booking);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditBooking(int id, Booking model)
        {
            if (id != model.BookingId) return BadRequest();
            if (!ModelState.IsValid)
                return View("Booking/EditBooking", model);

            await _bookingService.UpdateAsync(model);
            return RedirectToAction(nameof(Bookings));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return View("Booking/DeleteBooking", booking);
        }

        [HttpPost, ActionName("DeleteBooking"), ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteBookingConfirmed(int id)
        {
            await _bookingService.DeleteAsync(id);
            return RedirectToAction(nameof(Bookings));
        }


        //---------------------------------------------------USER---------------------------------------------------
        public IActionResult Users()
        {
            var model = _userManager.Users.ToList();
            return View("User/Users", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SetUserRole(string userId, string role)
        {
            var u = await _userManager.FindByIdAsync(userId);
            if (u != null && !await _userManager.IsInRoleAsync(u, role))
                await _userManager.AddToRoleAsync(u, role);
            return RedirectToAction(nameof(Users));
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
                await _userManager.RemoveFromRolesAsync(user, roles);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {                
                return BadRequest(result.Errors);
            }

            return RedirectToAction(nameof(Users));
        }

        [HttpGet]
        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var vm = new EditUserViewModel
             {
                 Id = user.Id,
                 UserName = user.UserName,
                 Email = user.Email
            };
            return View("User/EditUser", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUser(EditUserViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("User/EditUser", vm);

            var user = await _userManager.FindByIdAsync(vm.Id);
            if (user == null) return NotFound();

            user.UserName = vm.UserName;
            user.Email    = vm.Email;
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var e in updateResult.Errors)
                    ModelState.AddModelError("", e.Description);
                return View("User/EditUser", vm);
            }

            if (!string.IsNullOrWhiteSpace(vm.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var pwResult = await _userManager.ResetPasswordAsync(user, token, vm.NewPassword);
                if (!pwResult.Succeeded)
                {
                    foreach (var e in pwResult.Errors)
                    ModelState.AddModelError("", e.Description);
                    return View("User/EditUser", vm);
                }
            }

            return RedirectToAction(nameof(Users));
        }
    }
}
