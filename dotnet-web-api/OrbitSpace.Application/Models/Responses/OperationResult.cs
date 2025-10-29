using System.Diagnostics.CodeAnalysis;

namespace OrbitSpace.Application.Models.Responses;

public class OperationResult
{
    [MemberNotNullWhen(false,  nameof(Error))]
    public virtual bool IsSuccess { get; protected set; }
    
    public OperationResultError? Error { get; protected set; }
    
    protected OperationResult() { }
    
    public static OperationResult Success() => new() { IsSuccess = true };
    
    private static OperationResult Failure(OperationResultError error) => new() { IsSuccess = false, Error = error };
    
    public static implicit operator OperationResult(OperationResultError value) => Failure(value);
}

public class OperationResult<T> : OperationResult
{
    [MemberNotNullWhen(true, nameof(Data))]
    public override bool IsSuccess
    {
        get => base.IsSuccess;
        protected set => base.IsSuccess = value;
    }

    public T? Data { get; private set; }
    
    private OperationResult() { }
    
    private static OperationResult<T> Success(T data) => new() { IsSuccess = true, Data = data };
    
    private static OperationResult<T> Failure(OperationResultError error) => new() { IsSuccess = false, Error = error };

    public static implicit operator OperationResult<T>(T value) => Success(value);
    public static implicit operator OperationResult<T>(OperationResultError value) => Failure(value);
}

public class OperationResultError(OperationResultErrorType errorType, string? errorMessage)
{
    public OperationResultErrorType ErrorType { get; private set; } = errorType;
    public string? ErrorMessage { get; private set; } = errorMessage;
    
    public static OperationResultError NotFound(string? errorMessage = null) =>
        new(OperationResultErrorType.NotFound, errorMessage);
    
    public static OperationResultError Validation(string? errorMessage = null) =>
        new(OperationResultErrorType.Validation, errorMessage);
    
    public static OperationResultError Unauthorized(string? errorMessage = null) =>
        new(OperationResultErrorType.Unauthorized, errorMessage);
}

public enum OperationResultErrorType
{
    NotFound = 1,
    Validation = 2,
    Unauthorized = 3
}