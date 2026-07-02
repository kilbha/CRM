using CRM.Application.Common.Validators;
using FluentValidation;
using CRM.Domain.Enums;

namespace CRM.Application.Features.Customers.UpdateCustomer;

public class UpdateCustomerValidator
    : AbstractValidator<UpdateCustomerRequest>
{
    public UpdateCustomerValidator()
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

        RuleFor(x => x.Status)
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