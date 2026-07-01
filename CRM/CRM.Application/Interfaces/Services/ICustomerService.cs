using CRM.Application.Features.Customers.CreateCustomer;

namespace CRM.Application.Interfaces.Services;
public interface ICustomerService
{
    Task<CreateCustomerResponse> CreateAsync(
        CreateCustomerRequest request);

    Task<GetCustomerResponse> GetByIdAsync(Guid id);
}