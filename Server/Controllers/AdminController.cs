using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Models;
using Models.Models;
using Models.ViewModels;
using Server.Areas.Admin.ViewModels;
using Services.Services;

namespace Server.Controllers
{
    [Authorize(Roles = "Admin,Receptionist")]
    public class AdminController : Controller
    {
        private readonly IVacationSpotService _spotService;
        private readonly IBookingService _bookingService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(
            IVacationSpotService spotService,
            IBookingService bookingService,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IWebHostEnvironment env
        )
        {
            _spotService = spotService;
            _bookingService = bookingService;
            _userManager = userManager;
            _context = context;
            _env = env;
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
        private async Task PopulateCategoriesAsync(VacationSpotFormViewModel vm)
        {
            var cats = await _context.Categories.OrderBy(c => c.Name).ToListAsync();
            vm.CategoriesList = new MultiSelectList(
                items: cats,
                dataValueField: "CategoryId",
                dataTextField: "Name",
                selectedValues: vm.SelectedCategoryIds
            );
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Spots()
        {
            var model = await _spotService.GetAllAsync();
            return View("VacationSpot/Spots", model);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSpot()
        {
            var vm = new VacationSpotFormViewModel();
            await PopulateCategoriesAsync(vm);
            return View("VacationSpot/CreateSpot", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSpot(VacationSpotFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategoriesAsync(vm);
                return View("VacationSpot/CreateSpot", vm);
            }

            var spot = new VacationSpot
            {
                Title = vm.Title,
                Description = vm.Description,
                Location = vm.Location,
                PricePerNight = vm.PricePerNight,
                OwnerId = _userManager.GetUserId(User)
            };

            spot.VacationSpotCategories = vm.SelectedCategoryIds
                .Select(id => new VacationSpotCategory { CategoryId = id })
                .ToList();

            if (vm.ImageFile != null && vm.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid() + Path.GetExtension(vm.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await vm.ImageFile.CopyToAsync(stream);
                spot.ImageUrl = $"/uploads/{fileName}";
            }

            await _spotService.CreateAsync(spot);
            return RedirectToAction(nameof(Spots));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditSpot(int id)
        {
            var spot = await _spotService.GetByIdAsync(id);
            if (spot == null) return NotFound();

            var vm = new VacationSpotFormViewModel
            {
                SpotId = spot.SpotId,
                Title = spot.Title,
                Description = spot.Description,
                Location = spot.Location,
                PricePerNight = spot.PricePerNight,
                SelectedCategoryIds = spot.VacationSpotCategories.Select(vc => vc.CategoryId).ToList()
            };
            await PopulateCategoriesAsync(vm);
            return View("VacationSpot/EditSpot", vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditSpot(VacationSpotFormViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateCategoriesAsync(vm);
                return View("VacationSpot/EditSpot", vm);
            }

            var spot = await _spotService.GetByIdAsync(vm.SpotId!.Value);
            if (spot == null) return NotFound();

            spot.Title = vm.Title;
            spot.Description = vm.Description;
            spot.Location = vm.Location;
            spot.PricePerNight = vm.PricePerNight;

            spot.VacationSpotCategories.Clear();
            spot.VacationSpotCategories = vm.SelectedCategoryIds
                .Select(id => new VacationSpotCategory { VacationSpotId = spot.SpotId, CategoryId = id })
                .ToList();


            if (vm.ImageFile != null && vm.ImageFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid() + Path.GetExtension(vm.ImageFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await vm.ImageFile.CopyToAsync(stream);
                spot.ImageUrl = $"/uploads/{fileName}";
            }

            await _spotService.UpdateAsync(spot);
            return RedirectToAction(nameof(Spots));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSpot(int id)
        {
            var m = await _spotService.GetByIdAsync(id);
            if (m == null) return NotFound();
            return View("VacationSpot/DeleteSpot", m);
        }

        [HttpPost, ActionName("DeleteSpot"), ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSpotConfirmed(int id)
        {
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
        public async Task<IActionResult> EditBooking(int id, Booking model)
        {
            if (id != model.BookingId) return BadRequest();
            if (!ModelState.IsValid)
                return View("Booking/EditBooking", model);

            await _bookingService.UpdateAsync(model);
            return RedirectToAction(nameof(Bookings));
        }

        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await _bookingService.GetByIdAsync(id);
            if (booking == null) return NotFound();
            return View("Booking/DeleteBooking", booking);
        }

        [HttpPost, ActionName("DeleteBooking"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBookingConfirmed(int id)
        {
            await _bookingService.DeleteAsync(id);
            return RedirectToAction(nameof(Bookings));
        }


        //---------------------------------------------------USER---------------------------------------------------
        [Authorize(Roles = "Admin")]
        public IActionResult Users()
        {
            var model = _userManager.Users.ToList();
            return View("User/Users", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SetUserRole(string userId, string role)
        {
            var u = await _userManager.FindByIdAsync(userId);
            if (u != null && !await _userManager.IsInRoleAsync(u, role))
                await _userManager.AddToRoleAsync(u, role);
            return RedirectToAction(nameof(Users));
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
