using CRM.Application.Features.Customers.CreateCustomer;
using CRM.Application.Features.Customers.UpdateCustomer;
using CRM.Application.Common.Models;
using CRM.Application.Features.Customers.GetCustomers;
using CRM.Application.Features.Customers.ActivateCustomer;
using CRM.Application.Features.Customers.DeactivateCustomer;
using CRM.Domain.Entities;

namespace CRM.Application.Interfaces.Services;
public interface ICustomerService
{
    Task<CreateCustomerResponse> CreateAsync(
        CreateCustomerRequest request);

    Task<GetCustomerResponse> GetByIdAsync(Guid id);

    Task UpdateAsync(Guid id,UpdateCustomerRequest request);

    Task DeleteAsync(Guid id);

    Task<PagedResponse<CustomerListItem>> GetAllAsync(
        GetCustomersRequest request);

    Task<ActivateCustomerResponse> ActivateAsync(
        ActivateCustomerRequest request);

    Task<DeactivateCustomerResponse> DeactivateAsync(
    DeactivateCustomerRequest request);

    
}