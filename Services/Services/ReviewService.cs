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

    public async Task DeleteReviewAsync(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }
}
