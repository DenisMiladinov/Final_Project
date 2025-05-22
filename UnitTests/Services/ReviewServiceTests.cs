using Xunit;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ReviewServiceTests
{
    private ApplicationDbContext _context;
    private ReviewService _reviewService;

    public ReviewServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _reviewService = new ReviewService(_context);
    }

    [Fact]
    public async Task AddReviewAsync_AddsReview()
    {
        var review = new Review
        {
            ReviewId = 1,
            SpotId = 100,
            UserId = "user1",
            Rating = 5,
            Comment = "Excellent stay!",
            CreatedAt = DateTime.UtcNow
        };

        await _reviewService.AddReviewAsync(review);

        var result = await _context.Reviews.FindAsync(1);
        Assert.NotNull(result);
        Assert.Equal("user1", result.UserId);
    }

    [Fact]
    public async Task DeleteReviewAsync_RemovesReview_WhenExists()
    {
        var review = new Review
        {
            ReviewId = 10,
            SpotId = 300,
            UserId = "user1",
            Rating = 4,
            Comment = "Pretty good",
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        await _reviewService.DeleteReviewAsync(10);

        var deleted = await _context.Reviews.FindAsync(10);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteReviewAsync_DoesNothing_WhenNotFound()
    {
        // ensure empty
        _context.Reviews.RemoveRange(_context.Reviews);
        await _context.SaveChangesAsync();

        await _reviewService.DeleteReviewAsync(999);

        var count = await _context.Reviews.CountAsync();
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task GetAverageRatingAsync_ReturnsCorrectAverage()
    {
        var spotId = 400;
        var reviews = new List<Review>
        {
            new Review
            {
                ReviewId = 20,
                SpotId = spotId,
                UserId = "user1",
                Rating = 4,
                Comment = "Nice",
                CreatedAt = DateTime.UtcNow
            },
            new Review
            {
                ReviewId = 21,
                SpotId = spotId,
                UserId = "user2",
                Rating = 2,
                Comment = "Okay",
                CreatedAt = DateTime.UtcNow
            }
        };

        _context.Reviews.AddRange(reviews);
        await _context.SaveChangesAsync();

        var average = await _reviewService.GetAverageRatingAsync(spotId);

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
        var spotId = 500;
        var reviews = new List<Review>
        {
            new Review
            {
                ReviewId = 30,
                SpotId = spotId,
                UserId = "user1",
                Rating = 4,
                Comment = "Solid",
                CreatedAt = DateTime.UtcNow
            },
            new Review
            {
                ReviewId = 31,
                SpotId = spotId,
                UserId = "user2",
                Rating = 3,
                Comment = "Decent",
                CreatedAt = DateTime.UtcNow
            }
        };

        _context.Reviews.AddRange(reviews);
        await _context.SaveChangesAsync();

        var count = await _reviewService.GetReviewCountAsync(spotId);
        Assert.Equal(2, count);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCorrectReview()
    {
        var review = new Review
        {
            ReviewId = 100,
            SpotId = 60,
            UserId = "userX",
            Rating = 5,
            Comment = "Test",
            CreatedAt = DateTime.UtcNow
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var result = await _reviewService.GetByIdAsync(100);

        Assert.NotNull(result);
        Assert.Equal(100, result.ReviewId);
        Assert.Equal("userX", result.UserId);
    }
}
