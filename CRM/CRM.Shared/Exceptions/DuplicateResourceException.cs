namespace CRM.Shared.Exceptions;

public class DuplicateResourceException : AppException
{
    public DuplicateResourceException(string message)
        : base(message)
    {
    }
}