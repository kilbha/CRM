namespace CRM.Shared.Responses;

public class ErrorResponse
{
    public bool Success { get; init; } = false;

    public int StatusCode { get; init; }

    public string Message { get; init; } = string.Empty;

    public string? TraceId { get; init; }

    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}