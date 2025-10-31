namespace OrbitSpace.WebApi.Models;

public class ApiResponse
{
    public bool IsSuccess { get; protected set; }
    public string? ErrorMessage { get; protected set; }
    public string Timestamp => DateTime.UtcNow.ToString("O");

    public static ApiResponse Success() 
        => new ApiResponse { IsSuccess = true };

    public static ApiResponse Failure(string? errorMessage) 
        => new ApiResponse { IsSuccess = false, ErrorMessage = errorMessage };
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; protected set; }

    public static ApiResponse<T> Success(T data) 
        => new ApiResponse<T> { IsSuccess = true, Data = data };

    public new static ApiResponse<T> Failure(string? errorMessage) 
        => new ApiResponse<T> { IsSuccess = false, ErrorMessage = errorMessage };
}