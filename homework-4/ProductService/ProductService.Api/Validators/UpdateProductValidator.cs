using FluentValidation;

namespace ProductService.Api.Validators;

public class UpdateProductValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductValidator()
    {
        RuleFor(request => request.Id)
            .GreaterThan(0)
            .WithMessage("Product Id should be positive");
        
        RuleFor(request => request.Price)
            .GreaterThan(0)
            .WithMessage("Price should be positive");
    }
}