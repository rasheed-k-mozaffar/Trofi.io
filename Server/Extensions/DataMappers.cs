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
            Name = menuItem.Name,
            Price = menuItem.Price,
            UpdatedPrice = menuItem.UpdatedPrice,
            IsAvailable = menuItem.IsAvailable,
            IsSpecial = menuItem.IsSpecial,
            Images = menuItem.DishImages?.Select(i => i.ToDishImageDto())
        };
    }
    #endregion
}

public static class ModelMappers
{
    #region 
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
    #endregion
}
