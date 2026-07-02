using CRM.Domain.Enums;
using FluentValidation;
using CRM.Application.Common.Validators;

namespace CRM.Application.Features.Customers.CreateCustomer;

public class CreateCustomerValidator
    : AbstractValidator<CreateCustomerRequest>
{
    public CreateCustomerValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);

        RuleFor(x => x.Phone)
            .NotEmpty()
            .MaximumLength(20);

        RuleFor(x => x.Company)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Website)
            .MaximumLength(200);

        RuleFor(x => x.Source)
            .IsInEnum();

        RuleFor(x => x.Address)
            .NotNull();

        When(x => x.Address != null, () =>
        {
            RuleFor(x => x.Address!)
                .SetValidator(new AddressValidator());
        });
    }
}