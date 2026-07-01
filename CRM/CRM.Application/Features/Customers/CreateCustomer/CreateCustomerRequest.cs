using CRM.Domain.Enums;

namespace CRM.Application.Features.Customers.CreateCustomer;

public class CreateCustomerRequest
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Company { get; set; } = string.Empty;

    public string Website { get; set; } = string.Empty;

    public CustomerSource Source { get; set; }

    public Guid? AssignedUserId { get; set; }

    public AddressDto Address { get; set; } = new();
}