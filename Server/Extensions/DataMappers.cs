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
}

public static class ModelMappers
{

}
