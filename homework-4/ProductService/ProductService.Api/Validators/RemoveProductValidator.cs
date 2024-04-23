using FluentValidation;

namespace ProductService.Api.Validators;

public class RemoveProductValidator : AbstractValidator<RemoveProductRequest>
{
    public RemoveProductValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThan(0)
            .WithMessage("Product Id should be positive");
    }
}