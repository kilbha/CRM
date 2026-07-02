using CRM.Domain.Common;
using CRM.Domain.Enums;
using CRM.Domain.ValueObjects;

namespace CRM.Domain.Entities;

public class Customer : BaseEntity
{
    public string CustomerCode { get; private set; } = string.Empty;

    public string Name { get; private set; } = string.Empty;

    public string Email { get; private set; } = string.Empty;

    public string Phone { get; private set; } = string.Empty;

    public string Company { get; private set; } = string.Empty;

    public string Website { get; private set; } = string.Empty;

    public CustomerStatus Status { get; private set; }

    public CustomerSource Source { get; private set; }

    public Guid? AssignedUserId { get; private set; }
    public Address Address { get; private set; } = null!;

    private Customer()
    {
    }

    public static Customer Create(
        string customerCode,
        string name,
        string email,
        string phone,
        string company,
        string website,
        CustomerSource source,
        Guid? assignedUserId,
        Address address)
    {
        return new Customer
        {
            Id = Guid.NewGuid(),
            CustomerCode = customerCode,
            Name = name,
            Email = email,
            Phone = phone,
            Company = company,
            Website = website,
            Source = source,
            Status = CustomerStatus.Active,
            AssignedUserId = assignedUserId,
            Address = address,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    public void UpdateBasicInformation(
        string name,
        string email,
        string phone,
        string company,
        string website)
    {
        Name = name;
        Email = email;
        Phone = phone;
        Company = company;
        Website = website;

        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeAddress(Address address)
    {
        Address = address;

        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignTo(Guid? userId)
    {
        AssignedUserId = userId;

        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeStatus(CustomerStatus status)
    {
        if (IsDeleted)
        {
            throw new InvalidOperationException(
                "Deleted customers cannot change status.");
        }

        if (Status == status)
        {
            return;
        }

        Status = status;

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        ChangeStatus(CustomerStatus.Active);
    }

    public void Deactivate()
    {
        ChangeStatus(CustomerStatus.Inactive);
    }

}