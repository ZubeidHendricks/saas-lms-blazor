namespace SaasLMS.Shared.DTOs;

public record ResponseDTO<T>
{
    public bool Success { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }

    public static ResponseDTO<T> CreateSuccess(T data, string? message = null)
    {
        return new ResponseDTO<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ResponseDTO<T> CreateError(string message)
    {
        return new ResponseDTO<T>
        {
            Success = false,
            Message = message,
            Data = default
        };
    }
}