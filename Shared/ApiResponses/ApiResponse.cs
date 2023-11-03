namespace Trofi.io.Shared.ApiResponses;

public class ApiResponse
{
    public string? Message { get; set; }
    public bool? IsSuccess { get; set; }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Body { get; set; }
}