namespace CRM.API.Models;

public sealed class ValidationErrorResponse
    : ApiResponseBase
{
    public Dictionary<string, string[]> Errors
    {
        get;
        init;
    } = new();
}