using CRM.Domain.Enums;

namespace CRM.Application.Features.Customers.GetCustomers;

public class GetCustomersRequest
{
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 10;

    public string? Search { get; set; }

    public CustomerStatus? Status { get; set; }

    public CustomerSource? Source { get; set; }

    public Guid? AssignedUserId { get; set; }

    public string SortBy { get; set; } = "CreatedAt";

    public bool Descending { get; set; } = true;
}