using Models;

public interface IReviewService
{
    Task<IEnumerable<Review>> GetBySpotIdAsync(int spotId);
    Task AddReviewAsync(Review review);
    Task DeleteReviewAsync(int id);
    Task<double> GetAverageRatingAsync(int spotId);
    Task<int> GetReviewCountAsync(int spotId);
    Task<Review?> GetByIdAsync(int reviewId);
}
