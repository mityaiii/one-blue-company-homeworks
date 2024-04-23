using FluentValidation;

namespace ProductService.Api.Validators;

public class FilteredListProductValidator : AbstractValidator<FilteredListRequest>
{
    public FilteredListProductValidator()
    {
        RuleFor(request => request.WarehouseId)
            .GreaterThan(0)
            .When(request => request.WarehouseId is not null)
            .WithMessage("WarehouseId should be positive");
        
        RuleFor(request => request.PageNumber)
            .GreaterThan(0)
            .When(request => request.PageNumber is not null)
            .WithMessage("PageNumber should be positive");
        
        RuleFor(request => request.PageSize)
            .GreaterThan(0)
            .When(request => request.PageSize is not null)
            .WithMessage("PageSize should be positive");
    }
}