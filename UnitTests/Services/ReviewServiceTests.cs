/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models;
using Services.Services;
using Xunit;

namespace UnitTests.Services
{
    public class ReviewServiceTests
    {
        private ApplicationDbContext CreateContext(string dbName)
        {
            var opts = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            return new ApplicationDbContext(opts);
        }

        [Fact]
        public async Task GetBySpotIdAsync_ReturnsOnlyThatSpotReviews()
        {
            var ctx = CreateContext(nameof(GetBySpotIdAsync_ReturnsOnlyThatSpotReviews));
            var reviews = new[]
            {
                new Review { ReviewId = 1, SpotId = 10, Rating = 5, Comment = "Great!" },
                new Review { ReviewId = 2, SpotId = 20, Rating = 4, Comment = "Good" },
                new Review { ReviewId = 3, SpotId = 10, Rating = 3, Comment = "Okay" }
            };
            ctx.Reviews.AddRange(reviews);
            await ctx.SaveChangesAsync();

            var svc = new ReviewService(ctx);
            var result = await svc.GetBySpotIdAsync(10);

            Assert.Equal(2, result.Count());
            Assert.All(result, r => Assert.Equal(10, r.SpotId));
        }

        [Fact]
        public async Task AddReviewAsync_InsertsReview()
        {
            var ctx = CreateContext(nameof(AddReviewAsync_InsertsReview));
            var review = new Review
            {
                ReviewId = 1,
                SpotId = 5,
                UserId = "user1",
                Rating = 4,
                Comment = "Nice place"
            };
            var svc = new ReviewService(ctx);

            await svc.AddReviewAsync(review);

            var stored = await ctx.Reviews.FindAsync(1);
            Assert.NotNull(stored);
            Assert.Equal("user1", stored.UserId);
            Assert.Equal(4, stored.Rating);
            Assert.Equal("Nice place", stored.Comment);
        }

        [Fact]
        public async Task DeleteReviewAsync_RemovesExistingReview()
        {
            var ctx = CreateContext(nameof(DeleteReviewAsync_RemovesExistingReview));
            ctx.Reviews.Add(new Review { ReviewId = 2, SpotId = 5, Rating = 2, Comment = "Bad" });
            await ctx.SaveChangesAsync();
            var svc = new ReviewService(ctx);

            await svc.DeleteReviewAsync(2);

            var deleted = await ctx.Reviews.FindAsync(2);
            Assert.Null(deleted);
        }

        [Fact]
        public async Task DeleteReviewAsync_NoThrowForNonexistent()
        {
            var ctx = CreateContext(nameof(DeleteReviewAsync_NoThrowForNonexistent));
            var svc = new ReviewService(ctx);

            // Should not throw
            await svc.DeleteReviewAsync(999);

            Assert.Empty(ctx.Reviews);
        }
    }
}*/