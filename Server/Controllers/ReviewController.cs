using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using Services.Services;
using System.Security.Claims;

[Authorize]
public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;
    private readonly IVacationSpotService _vacationSpotService;

    public ReviewController(IReviewService reviewService, IVacationSpotService vacationSpotService)
    {
        _reviewService = reviewService;
        _vacationSpotService = vacationSpotService;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(
    [Bind(Prefix = "NewReview")] ReviewViewModel vm
)
    {
        if (!ModelState.IsValid)
        {

            var detailsVm = await _vacationSpotService
                                 .BuildDetailsViewModelAsync(vm.SpotId);
            detailsVm.NewReview = vm;
            return View("Details", detailsVm);
        }

        var review = new Review
        {
            SpotId = vm.SpotId,
            Rating = vm.Rating,
            Comment = vm.Comment,
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            CreatedAt = DateTime.UtcNow
        };

        await _reviewService.AddReviewAsync(review);
        return RedirectToAction("Details", "VacationSpot", new { id = vm.SpotId });
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Delete(int id, int spotId)
    {
        var review = await _reviewService.GetByIdAsync(id);
        if (review == null)
            return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (review.UserId != userId && !User.IsInRole("Admin"))
            return Forbid();

        await _reviewService.DeleteReviewAsync(id);

        return RedirectToAction("Details", "VacationSpot", new { id = spotId });
    }
}
