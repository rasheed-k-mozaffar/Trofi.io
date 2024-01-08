using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trofi.io.Server.Options;
using Trofi.io.Shared;

namespace Trofi.io.Server.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ReviewsController : ControllerBase
{
    #region Dependencies
    private readonly ICustomerReviewsRepository _reviewsRepository;
    private readonly ILogger<ReviewsController> _logger;
    private readonly AppDbContext _context;
    private readonly UserInfo _userInfo;

    public ReviewsController
    (
        ICustomerReviewsRepository reviewsRepository,
        ILogger<ReviewsController> logger,
        AppDbContext context,
        UserInfo userInfo
    )
    {
        _reviewsRepository = reviewsRepository;
        _logger = logger;
        _context = context;
        _userInfo = userInfo;
    }
    #endregion

    [HttpGet("get-reviews/{itemId}")]
    public async Task<IActionResult> GetReviewsForItem(Guid itemId, CancellationToken cancellationToken)
    {
        try
        {
            var reviews = await _reviewsRepository.GetReviewsAsync(itemId, cancellationToken);
            var reviewsAsDtos = reviews.Select(r => r.ToReviewDto());

            return Ok(new ApiResponse<IEnumerable<ReviewDto>>
            {
                Message = "Reviews retrieved successfully",
                Body = reviewsAsDtos,
                IsSuccess = true
            });
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex.Message);
            return NoContent();
        }
    }

    [HttpGet("get-review/{reviewId}")]
    public async Task<IActionResult> GetReviewById(Guid reviewId, CancellationToken cancellationToken)
    {
        try
        {
            var review = await _reviewsRepository.GetReviewAsync(reviewId, cancellationToken);
            var reviewAsDto = review.ToReviewDto();

            return Ok(new ApiResponse<ReviewDto>
            {
                Message = "Review retrieved successfully",
                Body = reviewAsDto,
                IsSuccess = true
            });

        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiErrorResponse
            {
                ErrorMessage = ex.Message
            });
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex.Message);
            return NoContent();
        }
    }

    [HttpPost("add-review/{itemId}")]
    [Authorize]
    public async Task<IActionResult> AddReview(Guid itemId, CreateReviewDto review, CancellationToken cancellationToken)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _reviewsRepository
                        .AddReviewAsync(itemId, review.ToReviewCreate(), cancellationToken);

                return Ok(new ApiResponse
                {
                    Message = "Your review has been submitted successfully!",
                    IsSuccess = true
                });
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        catch (ResourceCreationFailedException ex)
        {
            return BadRequest(new ApiErrorResponse
            {
                ErrorMessage = ex.Message
            });
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex.Message);
            return NoContent();
        }
    }

    [HttpDelete("delete-review/{reviewId}")]
    [Authorize] //TODO: Add Roles
    public async Task<IActionResult> DeleteReview(Guid reviewId, CancellationToken cancellationToken)
    {
        try
        {
            await _reviewsRepository.RemoveReviewAsync(reviewId, cancellationToken);

            return Ok(new ApiResponse
            {
                Message = "The review has been deleted successfully",
                IsSuccess = true
            });
        }
        catch (NotFoundException ex)
        {
            return NotFound(new ApiErrorResponse
            {
                ErrorMessage = ex.Message
            });
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex.Message);
            return NoContent();
        }
    }

    [HttpPut("edit-review/{reviewId}")]
    [Authorize] // TODO: Add Roles
    public async Task<IActionResult> EditReview(Guid reviewId, UpdateReviewDto updatedReview, CancellationToken cancellationToken)
    {
        var review = await _context
                            .CustomerReviews
                            .FindAsync(reviewId);

        if (review is null)
        {
            return NotFound(new ApiErrorResponse
            {
                ErrorMessage = "The review you're looking for was not found"
            });
        }

        if (review.AppUserId != _userInfo.UserId) // user attempting to edit a review not written by them
        {
            return BadRequest(new ApiErrorResponse
            {
                ErrorMessage = "You are not allowed to edit other reviews"
            });
        }

        try
        {
            // map the updated properties
            review.Title = updatedReview.Title;
            review.Review = updatedReview.Review;
            review.Rating = updatedReview.Rating;
            review.EditedOn = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return Ok(new ApiResponse<ReviewDto>
            {
                Message = "The review was updated successfully",
                Body = review.ToReviewDto(), // send back the updated details
                IsSuccess = true
            });
        }
        catch (OperationCanceledException ex)
        {
            _logger.LogError(ex.Message);
            return NoContent();
        }
    }
}
