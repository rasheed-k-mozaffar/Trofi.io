
using Trofi.io.Server.Options;

namespace Trofi.io.Server.Repositories;

public class CustomerReviewsRepository : ICustomerReviewsRepository
{
    private readonly AppDbContext _context;
    private readonly UserInfo _userInfo;

    public CustomerReviewsRepository(AppDbContext context, UserInfo userInfo)
    {
        _context = context;
        _userInfo = userInfo;
    }

    public async Task AddReviewAsync(Guid menuItemId, CustomerReview review, CancellationToken cancellationToken)
    {
        var menuItem = await _context
                            .MenuItems
                            .FindAsync(menuItemId, cancellationToken);

        if (menuItem is null)
        {
            throw new NotFoundException(message: "The menu item you're looking for was not found");
        }

        // map the foreign key properties for the new review entry
        review.MenuItemID = menuItem.Id;
        review.AppUserId = _userInfo.UserId;

        review.ReviwerName = $"{_userInfo.FirstName} {_userInfo.LastName}";

        review.WrittenOn = DateTime.UtcNow;

        var creationResult = await _context.CustomerReviews.AddAsync(review, cancellationToken);

        if (creationResult.State == EntityState.Added)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<CustomerReview> GetReviewAsync(Guid reviewId, CancellationToken cancellationToken)
    {
        var review = await _context
                            .CustomerReviews
                            .FirstOrDefaultAsync(r => r.Id == reviewId, cancellationToken);

        if (review is null)
        {
            throw new NotFoundException(message: "The review you're looking for was not found");
        }

        return review;
    }

    public async Task<IEnumerable<CustomerReview>> GetReviewsAsync(Guid menuItemId, CancellationToken cancellationToken)
    {
        var reviews = await _context
                            .CustomerReviews
                            .Where(r => r.MenuItemID == menuItemId)
                            .ToListAsync(cancellationToken);

        return reviews.Any() ? reviews : Enumerable.Empty<CustomerReview>();
    }

    public async Task RemoveReviewAsync(Guid reviewId, CancellationToken cancellationToken)
    {
        var review = await _context
                            .CustomerReviews
                            .FindAsync(reviewId, cancellationToken);

        if (review is null)
        {
            throw new NotFoundException(message: "The review you're trying to delete was not found");
        }

        var deletionResult = _context.CustomerReviews.Remove(review);

        if (deletionResult.State == EntityState.Deleted)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public Task UpdateReviewAsync(Guid reviewId, CustomerReview review)
    {
        throw new NotImplementedException();
    }
}
