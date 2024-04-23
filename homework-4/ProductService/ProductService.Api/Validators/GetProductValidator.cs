using FluentValidation;

namespace ProductService.Api.Validators;

public class GetProductValidator : AbstractValidator<GetProductRequest>
{
    public GetProductValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThan(0)
            .WithMessage("Id should be positive");
    }   
}