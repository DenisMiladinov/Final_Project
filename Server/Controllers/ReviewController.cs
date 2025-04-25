using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using System.Security.Claims;

[Authorize]
public class ReviewController : Controller
{
    private readonly IReviewService _reviewService;

    public ReviewController(IReviewService reviewService)
    {
        _reviewService = reviewService;
    }

    [HttpPost]
    public async Task<IActionResult> Add(Review review)
    {
        review.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _reviewService.AddReviewAsync(review);
        return RedirectToAction("Details", "VacationSpot", new { id = review.SpotId });
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id, int spotId)
    {
        await _reviewService.DeleteReviewAsync(id);
        return RedirectToAction("Details", "VacationSpot", new { id = spotId });
    }
}
