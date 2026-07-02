using FluentValidation;

namespace CRM.Application.Features.Customers.GetCustomers;

public class GetCustomersRequestValidator
    : AbstractValidator<GetCustomersRequest>
{
    public GetCustomersRequestValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 100);

        RuleFor(x => x.SortBy)
            .Must(BeAValidSortColumn)
            .WithMessage("SortBy must be one of: Name, Company, CreatedAt.");
    }

    private static bool BeAValidSortColumn(string sortBy)
    {
        return sortBy.Equals("Name", StringComparison.OrdinalIgnoreCase)
            || sortBy.Equals("Company", StringComparison.OrdinalIgnoreCase)
            || sortBy.Equals("CreatedAt", StringComparison.OrdinalIgnoreCase);
    }
}