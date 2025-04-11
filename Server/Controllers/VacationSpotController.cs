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

        // GET: /VacationSpot/
        public async Task<IActionResult> Index()
        {
            var spots = await _vacationSpotService.GetAllAsync();
            return View(spots);
        }

        // GET: /VacationSpot/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var spot = await _vacationSpotService.GetByIdAsync(id);
            if (spot == null)
                return NotFound();

            return View(spot);
        }

        // GET: /VacationSpot/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /VacationSpot/Create
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

        // GET: /VacationSpot/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var spot = await _vacationSpotService.GetByIdAsync(id);
            if (spot == null)
                return NotFound();

            return View(spot);
        }

        // POST: /VacationSpot/Edit/5
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

        // GET: /VacationSpot/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var spot = await _vacationSpotService.GetByIdAsync(id);
            if (spot == null)
                return NotFound();

            return View(spot);
        }

        // POST: /VacationSpot/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _vacationSpotService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
