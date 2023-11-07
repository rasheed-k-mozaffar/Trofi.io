namespace Trofi.io.Shared.Auth;

public class RevokeTokenRequest
{
    // The reason it's not required is cause the client can either send it 
    // through the cookies, or through this data model
    public string? Token { get; set; }
}
