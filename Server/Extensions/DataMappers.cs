using Trofi.io.Shared;

namespace Trofi.io.Server;

/// <summary>
/// This class provides extensions for all the DTOs in the project so that
/// domain models can be mapped to DTOs more easily
/// </summary>
public static class DataMappers
{
    #region Image Mappers
    public static ImageDto ToDishImageDto(this DishImage dishImage)
    {
        return new ImageDto
        {
            Id = dishImage.Id,
            URL = dishImage.URL
        };
    }
    #endregion

    #region Menu Item Mappers
    public static MenuItemDto ToMenuItemDto(this MenuItem menuItem)
    {
        return new MenuItemDto
        {
            Id = menuItem.Id,
            Description = menuItem.Description,
            Name = menuItem.Name,
            Price = menuItem.Price,
            UpdatedPrice = menuItem.UpdatedPrice,
            IsAvailable = menuItem.IsAvailable,
            IsSpecial = menuItem.IsSpecial,
            Images = menuItem.DishImages?.Select(i => i.ToDishImageDto()),
            Category = menuItem.Category?.ToCategorySummaryDto(),
            Rating = menuItem.Rating
        };
    }
    #endregion

    #region Cart Item Mappers
    public static CartItemDto ToCartItemDto(this CartItem cartItem)
    {
        return new CartItemDto
        {
            Id = cartItem.Id,
            ProductId = cartItem.ProductId,
            ProductName = cartItem.ProductName,
            Quantity = cartItem.Quantity,
            Price = cartItem.Price,
            UpdatedPrice = cartItem.UpdatedPrice,
            CoverImageURL = cartItem.CoverImageURL
        };
    }
    #endregion

    #region Category Mappers
    public static CategoryDto ToCategoryDto(this Category category)
    {
        return new CategoryDto
        {
            CategoryId = category.Id,
            Name = category.Name,
            Description = category.Description,
            LogoUrl = category.LogoUrl,
            Items = category.Items?.Select(i => i.ToMenuItemDto())
        };
    }

    public static CategorySummaryDto ToCategorySummaryDto(this Category category)
    {
        return new CategorySummaryDto
        {
            CategoryId = category.Id,
            Name = category.Name,
        };
    }
    #endregion

    #region Review Mappers
    public static ReviewDto ToReviewDto(this CustomerReview review)
    {
        return new ReviewDto
        {
            ReviewId = review.Id,
            Title = review.Title,
            Review = review.Review,
            Rating = review.Rating,
            UpVotes = review.UpVotes,
            WrittenBy = review.ReviwerName,
            WrittenOn = review.WrittenOn,
            EditedOn = review.EditedOn
        };
    }
    #endregion
}


/// <summary>
/// This class provides extensions for all the Data models so that DTOs can be 
/// mapped quickly into data models.
/// </summary>
public static class ModelMappers
{
    public static MenuItem ToMenuItemCreate(this DishCreateDto dish)
    {
        return new MenuItem
        {
            Id = dish.Id,
            Name = dish.Name,
            Description = dish.Description,
            Price = dish.Price,
            UpdatedPrice = null,
            IsAvailable = dish.IsAvailable,
            IsSpecial = dish.IsSpecial
        };
    }

    public static Category ToCategoryCreate(this CreateCategoryDto category)
    {
        return new Category
        {
            Name = category.Name,
            Description = category.Description,
            LogoUrl = category.LogoUrl
        };
    }

    public static CustomerReview ToReviewCreate(this CreateReviewDto review)
    {
        return new CustomerReview
        {
            Title = review.Title,
            Review = review.Review,
            Rating = review.Rating
        };
    }
}
