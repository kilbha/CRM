using FluentValidation;

namespace CRM.Application.Features.Customers.ActivateCustomer;

public class ActivateCustomerValidator
    : AbstractValidator<ActivateCustomerRequest>
{
    public ActivateCustomerValidator()
    {
        RuleFor(x => x.CustomerId)
            .NotEmpty();
    }
}