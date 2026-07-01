using CRM.Domain.Enums;
namespace CRM.Application.Features.Customers.GetCustomers;

public class CustomerListItem
{
    public Guid Id { get; set; }

    public string CustomerCode { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public CustomerStatus Status { get; set; }

    public CustomerSource Source { get; set; }
}