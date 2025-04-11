using Microsoft.AspNetCore.Mvc;
using Models;
using static Services.Services.IVacationSpotServices;

namespace Server.Controllers
{
    public class VacationSpotController : Controller
    {
        private readonly IVacationSpotService _vacationSpotService;

        public VacationSpotController(IVacationSpotService vacationSpotService)
        {
            _vacationSpotService = vacationSpotService;
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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VacationSpot spot)
        {
            if (ModelState.IsValid)
            {
                await _vacationSpotService.CreateAsync(spot);
                return RedirectToAction(nameof(Index));
            }
            return View(spot);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var spot = await _vacationSpotService.GetByIdAsync(id);
            if (spot == null)
                return NotFound();

            return View(spot);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VacationSpot spot)
        {
            if (id != spot.SpotId)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _vacationSpotService.UpdateAsync(spot);
                return RedirectToAction(nameof(Index));
            }

            return View(spot);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var spot = await _vacationSpotService.GetByIdAsync(id);
            if (spot == null)
                return NotFound();

            return View(spot);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _vacationSpotService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
