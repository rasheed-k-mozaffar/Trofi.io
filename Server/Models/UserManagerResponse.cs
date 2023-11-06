namespace Trofi.io.Server.Models;

public class UserManagerResponse
{
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }
    public string? JWT { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
}

