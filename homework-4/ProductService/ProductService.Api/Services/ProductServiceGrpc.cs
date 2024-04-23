using AutoMapper;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using ProductService.Domain.Entities.Products;
using ProductService.Domain.Models;
using ProductService.Domain.Services;

namespace ProductService.Api.Services;

public class ProductServiceGrpc : ProductService.ProductServiceBase
{
    private readonly IProductServiceApplication _productServiceApplication;
    private readonly IMapper _mapper;

    public ProductServiceGrpc(IProductServiceApplication productServiceApplication, IMapper mapper)
    {
        _productServiceApplication = productServiceApplication;
        _mapper = mapper;
    }
    
    public override async Task<AddProductResponse> Add(AddProductRequest request, ServerCallContext context)
    {
        var product = new Product()
        {
            DateOfCreation = request.DateOfCreation.ToDateTime(),
            Name = request.Name,
            ProductType = _mapper.Map<ProductType>(request.ProductType),
            Weight = request.Weight,
            WarehouseId = request.WarehouseId,
            Price = request.Price,
        };
        
        var productId = await _productServiceApplication.AddAsync(product);

        return new AddProductResponse()
        {
            Id = productId
        };
    }

    public override async Task<Empty> Remove(RemoveProductRequest request, ServerCallContext context)
    {
        await _productServiceApplication.RemoveAsync(request.Id);
        return new Empty();
    }

    public override async Task<ProductReply> Update(UpdateProductRequest request, ServerCallContext context)
    {
        var updatedProduct = await _productServiceApplication.UpdatePriceAsync(request.Id, request.Price);
        var product = _mapper.Map<ProductReply>(updatedProduct);
        return product;
    }

    public override async Task<ProductReply> Get(GetProductRequest request, ServerCallContext context)
    {
        var product = await _productServiceApplication.GetAsync(request.Id);

        return _mapper.Map<ProductReply>(product);
    }
    
    public override async Task<ListProductResponse> List(FilteredListRequest request, ServerCallContext context)
    {
        ProductType? productType = request.HasProductType 
            ? _mapper.Map<ProductType>(request.ProductType)
            : null;

        DateTime? dateTime = request.DateOfCreation?.ToDateTime();
        
        var productPage = new ProductFilter()
        {
            Name = request.Name,
            WarehouseId = request.WarehouseId, 
            DateOfCreation = dateTime,
            ProductType = productType,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };
        
        var products = await _productServiceApplication.List(productPage);

        return new ListProductResponse 
        {
            Products =
            {
                products.Select(p => _mapper.Map<ProductReply>(p))
            }
        };
    }
}