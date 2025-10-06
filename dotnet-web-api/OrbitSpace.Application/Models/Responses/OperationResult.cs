namespace OrbitSpace.Application.Models.Responses;

public class OperationResult<T> where T : class
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public T? Data { get; private set; }
    
    public OperationResult(T data)
    {
        IsSuccess = true;
        Data = data;
    }
    
    public OperationResult(string errorMessage)
    {
        IsSuccess = false;
        ErrorMessage = errorMessage;
    }
}