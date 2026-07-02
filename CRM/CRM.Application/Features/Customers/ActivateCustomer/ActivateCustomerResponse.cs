using CRM.Domain.Enums;

namespace CRM.Application.Features.Customers.ActivateCustomer;

public class ActivateCustomerResponse
{
    public Guid CustomerId { get; set; }

    public CustomerStatus Status { get; set; }

    public string Message { get; set; } = string.Empty;
}