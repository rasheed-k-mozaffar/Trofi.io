namespace Trofi.io.Server.Repositories;

public interface ICustomerReviewsRepository
{
    Task<IEnumerable<CustomerReview>> GetReviewsAsync(Guid menuItemId, CancellationToken cancellationToken);
    Task<CustomerReview> GetReviewAsync(Guid reviewId, CancellationToken cancellationToken);
    Task AddReviewAsync(Guid menuItemId, CustomerReview review, CancellationToken cancellationToken);
    Task RemoveReviewAsync(Guid reviewId, CancellationToken cancellationToken);
    Task UpdateReviewAsync(Guid reviewId, CustomerReview review);
}
