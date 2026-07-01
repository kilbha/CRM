namespace CRM.Application.Interfaces.Generators;

public interface ICustomerCodeGenerator
{
    Task<string> GenerateAsync();
}