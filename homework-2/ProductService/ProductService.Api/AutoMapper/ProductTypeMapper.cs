using AutoMapper;
using ProductService.Domain.Entities.Products;

namespace ProductService.Api.AutoMapper;

public class ProductTypeMapper : Profile
{
    public ProductTypeMapper()
    {
        CreateMap<ProductReplyType, ProductType>()
            .ConvertUsing((value, description) =>
        {
            return value switch
            {
                ProductReplyType.General => ProductType.General,
                ProductReplyType.Household => ProductType.Household,
                ProductReplyType.Products => ProductType.Products,
                ProductReplyType.ChemicalsAppliances => ProductType.ChemicalsAppliances,
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        });
    }
}