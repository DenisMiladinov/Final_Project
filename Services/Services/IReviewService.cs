using Models;

public interface IReviewService
{
    Task<IEnumerable<Review>> GetBySpotIdAsync(int spotId);
    Task AddReviewAsync(Review review);
    Task DeleteReviewAsync(int id);
}
