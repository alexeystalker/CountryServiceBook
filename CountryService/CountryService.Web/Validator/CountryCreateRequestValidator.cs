using CountryService.Web.gRPC.v1;
using FluentValidation;

namespace CountryService.Web.Validator;

public class CountryCreateRequestValidator: AbstractValidator<CountryCreationRequest>
{
    public CountryCreateRequestValidator()
    {
        RuleFor(request => request.Name).NotEmpty().WithMessage("Name is mandatory.");
        RuleFor(request => request.Description).MinimumLength(5)
            .WithMessage("Description is mandatory and should be longer than 4 characters");
    }
}