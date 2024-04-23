using FluentValidation;

namespace ProductService.Api.Validators;

public class AddProductValidator : AbstractValidator<AddProductRequest>
{
    public AddProductValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty()
            .WithMessage("Name is mandatory");

        RuleFor(request => request.WarehouseId)
            .GreaterThan(0)
            .WithMessage("WarehouseId should be positive");
        
        RuleFor(request => request.Price)
            .GreaterThan(0)
            .WithMessage("Price should be positive");
        
        RuleFor(request => request.Weight)
            .GreaterThan(0)
            .WithMessage("Weight should be positive");
    }
}