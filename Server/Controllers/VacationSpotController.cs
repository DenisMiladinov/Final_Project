using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Services.Services;

namespace Server.Controllers
{
    public class VacationSpotController : Controller
    {
        private readonly IVacationSpotService _vacationSpotService;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _context;

        public VacationSpotController(IVacationSpotService vacationSpotService, IWebHostEnvironment webHostEnvironment, ApplicationDbContext context)
        {
            _vacationSpotService = vacationSpotService;
            _webHostEnvironment = webHostEnvironment;
            _context = context;
        }

        private async Task PopulateCategoriesAsync()
        {
            var cats = await _context.Categories
                                      .OrderBy(c => c.Name)
                                      .ToListAsync();
            ViewBag.Categories = new SelectList(cats, "CategoryId", "Name");
        }

        public async Task<IActionResult> Index(string search, string locationFilter)
        {
            ViewData["CurrentFilter"] = search;
            ViewData["CurrentLocationFilter"] = locationFilter;

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

            return View(spots);
        }
        public async Task<IActionResult> Details(int id)
        {
            var spot = await _vacationSpotService.GetByIdAsync(id);
            if (spot == null)
                return NotFound();

            return View(spot);
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
            var spot = await _vacationSpotService.GetByIdAsync(id);
            if (spot == null)
                return NotFound();

            return View(spot);
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
                return RedirectToAction(nameof(Index));
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
