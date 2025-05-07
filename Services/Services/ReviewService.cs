using Microsoft.EntityFrameworkCore;
using Models;

public class ReviewService : IReviewService
{
    private readonly ApplicationDbContext _context;

    public ReviewService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Review>> GetBySpotIdAsync(int spotId)
    {
        return await _context.Reviews
            .Where(r => r.SpotId == spotId)
            .Include(r => r.User)
            .ToListAsync();
    }

    public async Task AddReviewAsync(Review review)
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteReviewAsync(int reviewId)
    {
        var r = await GetByIdAsync(reviewId);
        if (r != null)
        {
            _context.Reviews.Remove(r);
            await _context.SaveChangesAsync();
        }
    }
    public async Task<double> GetAverageRatingAsync(int spotId)
    {
        var count = await _context.Reviews
                                  .CountAsync(r => r.SpotId == spotId);

        if (count == 0)
            return 0;

        var sum = await _context.Reviews
                                .Where(r => r.SpotId == spotId)
                                .SumAsync(r => r.Rating);

        return (double)sum / count;
    }


    public Task<int> GetReviewCountAsync(int spotId)
      => _context.Reviews
                 .CountAsync(r => r.SpotId == spotId);

    public async Task<Review?> GetByIdAsync(int reviewId)
    {
        return await _context.Reviews
                             .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
    }

}
