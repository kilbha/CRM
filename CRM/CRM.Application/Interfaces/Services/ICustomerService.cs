using CRM.Application.Features.Customers.CreateCustomer;
using CRM.Application.Features.Customers.UpdateCustomer;

namespace CRM.Application.Interfaces.Services;
public interface ICustomerService
{
    Task<CreateCustomerResponse> CreateAsync(
        CreateCustomerRequest request);

    Task<GetCustomerResponse> GetByIdAsync(Guid id);

    Task UpdateAsync(Guid id,UpdateCustomerRequest request);

    Task DeleteAsync(Guid id);
}