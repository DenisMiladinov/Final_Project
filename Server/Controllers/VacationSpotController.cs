using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Services.Services;
using Models.ViewModels;
using Stripe;

namespace Server.Controllers
{
    public class VacationSpotController : Controller
    {
        private readonly IVacationSpotService _vacationSpotService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IReviewService _reviewService;

        public VacationSpotController(IVacationSpotService vacationSpotService, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context, IReviewService reviewService)
        {
            _vacationSpotService = vacationSpotService;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
            _reviewService = reviewService;
        }

        private async Task PopulateCategoriesAsync(List<int> selectedIds = null!)
        {
            var cats = await _context.Categories
                .OrderBy(c => c.Name)
                .ToListAsync();

            ViewBag.Categories = new MultiSelectList(
                items: cats,
                dataValueField: "CategoryId",
                dataTextField: "Name",
                selectedValues: selectedIds
            );
        }


        public async Task<IActionResult> Index
            (
                string? search,
                string? locationFilter,
                int[]? categoryFilter
            )
        {
            ViewData["CurrentFilter"] = search;
            ViewData["CurrentLocationFilter"] = locationFilter;
            ViewData["CurrentCategoryFilter"] = categoryFilter;

            await PopulateCategoriesAsync();

            var spots = await _vacationSpotService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lower = search.Trim().ToLower();
                spots = spots.Where(s =>
                    s.Title.ToLower().Contains(lower) ||
                    s.Location.ToLower().Contains(lower));
            }

            if (!string.IsNullOrWhiteSpace(locationFilter))
            {
                spots = spots.Where(s =>
                    s.Location.Contains(locationFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (categoryFilter != null && categoryFilter.Any())
            {
                spots = spots.Where(s => s.VacationSpotCategories
                         .Any(vc => categoryFilter.Contains(vc.CategoryId)));

            }

            return View(spots);
        }
        public async Task<IActionResult> Details(int id)
        {
            var spot = await _vacationSpotService.GetByIdAsync(id);
            var reviews = await _reviewService.GetBySpotIdAsync(id);
            var avg = await _reviewService.GetAverageRatingAsync(id);
            var count = await _reviewService.GetReviewCountAsync(id);

            var vm = new VacationSpotDetailsViewModel
            {
                Spot = spot,
                Reviews = reviews,
                AverageRating = avg,
                ReviewCount = count,
                NewReview = new ReviewViewModel { SpotId = id }
            };
            return View(vm);
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            await PopulateCategoriesAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VacationSpot spot, IFormFile? ImageFile)
        {
            await PopulateCategoriesAsync();
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    spot.ImageUrl = "/images/" + uniqueFileName;
                }

                await PopulateCategoriesAsync();
                await _vacationSpotService.CreateAsync(spot);
                return RedirectToAction(nameof(Index));
            }

            return View(spot);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            await PopulateCategoriesAsync();
            var m = await _vacationSpotService.GetByIdAsync(id);
            if (m == null) return NotFound();
            var selectedCategoryIds = m.VacationSpotCategories.Select(vc => vc.CategoryId).ToList();
            await PopulateCategoriesAsync(selectedCategoryIds);
            return View("VacationSpot/EditSpot", m);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VacationSpot spot, IFormFile? ImageFile)
        {
            await PopulateCategoriesAsync();
            if (id != spot.SpotId)
                return BadRequest();

            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    Directory.CreateDirectory(uploadsFolder);
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(stream);
                    }

                    spot.ImageUrl = "/images/" + uniqueFileName;
                }

                await _vacationSpotService.UpdateAsync(spot);
                return View("VacationSpot/EditSpot", spot);
            }

            return View(spot);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var spot = await _vacationSpotService.GetByIdAsync(id);
            if (spot == null)
                return NotFound();

            return View(spot);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _vacationSpotService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
