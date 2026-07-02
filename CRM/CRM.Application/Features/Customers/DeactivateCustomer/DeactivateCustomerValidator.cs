using FluentValidation;

namespace CRM.Application.Features.Customers.DeactivateCustomer;

public class DeactivateCustomerValidator
    : AbstractValidator<DeactivateCustomerRequest>
{
    public DeactivateCustomerValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();
    }
}