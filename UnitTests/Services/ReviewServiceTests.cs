using Xunit;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class ReviewServiceTests
{
    private ApplicationDbContext _context;
    private ReviewService _reviewService;

    public ReviewServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "ReviewServiceTests")
            .Options;

        _context = new ApplicationDbContext(options);
        _reviewService = new ReviewService(_context);
    }

    [Fact]
    public async Task AddReviewAsync_AddsReview()
    {
        var review = new Review { ReviewId = 1, SpotId = 10, Rating = 4 };

        await _reviewService.AddReviewAsync(review);

        var result = await _context.Reviews.FindAsync(1);
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetBySpotIdAsync_ReturnsReviewsForSpot()
    {
        _context.Reviews.Add(new Review { ReviewId = 2, SpotId = 20 });
        _context.Reviews.Add(new Review { ReviewId = 3, SpotId = 20 });
        await _context.SaveChangesAsync();

        var result = await _reviewService.GetBySpotIdAsync(20);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task DeleteReviewAsync_RemovesReview_WhenExists()
    {
        var review = new Review { ReviewId = 4, SpotId = 30 };
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        await _reviewService.DeleteReviewAsync(4);

        var result = await _context.Reviews.FindAsync(4);
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteReviewAsync_DoesNothing_WhenNotFound()
    {
        await _reviewService.DeleteReviewAsync(999); // does not exist

        var count = await _context.Reviews.CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task GetAverageRatingAsync_ReturnsCorrectAverage()
    {
        _context.Reviews.Add(new Review { SpotId = 40, Rating = 4 });
        _context.Reviews.Add(new Review { SpotId = 40, Rating = 2 });
        await _context.SaveChangesAsync();

        var average = await _reviewService.GetAverageRatingAsync(40);

        Assert.Equal(3, average);
    }

    [Fact]
    public async Task GetAverageRatingAsync_ReturnsZero_WhenNoReviews()
    {
        var average = await _reviewService.GetAverageRatingAsync(999);
        Assert.Equal(0, average);
    }

    [Fact]
    public async Task GetReviewCountAsync_ReturnsCorrectCount()
    {
        _context.Reviews.Add(new Review { SpotId = 50 });
        _context.Reviews.Add(new Review { SpotId = 50 });
        await _context.SaveChangesAsync();

        var count = await _reviewService.GetReviewCountAsync(50);

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectReview()
    {
        var review = new Review { ReviewId = 100, SpotId = 60 };
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var result = await _reviewService.GetByIdAsync(100);

        Assert.NotNull(result);
        Assert.Equal(100, result.ReviewId);
    }
}
