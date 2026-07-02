namespace CRM.API.Models;

public abstract class ApiResponseBase
{
    public bool Success { get; init; }

    public int StatusCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public string TraceId { get; init; } = string.Empty;

    public DateTime Timestamp { get; init; }
        = DateTime.UtcNow;
}