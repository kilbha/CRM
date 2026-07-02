using CRM.Domain.Enums;

namespace CRM.Application.Features.Customers.DeactivateCustomer;

public class DeactivateCustomerResponse
{
    public Guid CustomerId { get; set; }

    public CustomerStatus Status { get; set; }

    public string Message { get; set; } = string.Empty;
}