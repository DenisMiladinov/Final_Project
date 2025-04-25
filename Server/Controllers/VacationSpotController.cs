using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Services.Services;

namespace Server.Controllers
{
    public class VacationSpotController : Controller
    {
        private readonly IVacationSpotService _vacationSpotService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public VacationSpotController(IVacationSpotService vacationSpotService, IWebHostEnvironment webHostEnvironment)
        {
            _vacationSpotService = vacationSpotService;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var spots = await _vacationSpotService.GetAllAsync();
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VacationSpot spot, IFormFile? ImageFile)
        {
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

                await _vacationSpotService.CreateAsync(spot);
                return RedirectToAction(nameof(Index));
            }

            return View(spot);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
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
