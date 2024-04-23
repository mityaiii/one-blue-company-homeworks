using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using ProductService.Domain.Entities.Products;

namespace ProductService.Api.AutoMapper;
public class ProductReplyToProductMapper : Profile
{
    public ProductReplyToProductMapper()
    {
        CreateMap<Product, ProductReply>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
            .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Weight))
            .ForMember(dest => dest.DateOfCreation, opt => opt.MapFrom(src => Timestamp.FromDateTime(src.DateOfCreation)))
            .ForMember(dest => dest.WarehouseId, opt => opt.MapFrom(src => src.WarehouseId))
            .ForMember(dest => dest.ProductType, opt => opt.MapFrom(src => src.ProductType))
            .ReverseMap();
    }
}
