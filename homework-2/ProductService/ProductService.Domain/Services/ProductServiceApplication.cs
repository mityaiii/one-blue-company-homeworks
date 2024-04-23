using AutoMapper;
using ProductService.Domain.Entities.Products;
using ProductService.Domain.Entities.Repositories;
using ProductService.Domain.Models;

namespace ProductService.Domain.Services;

public class ProductServiceApplication : IProductServiceApplication
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductServiceApplication(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }
    
    public async Task<long> Add(Product productDto)
    {
        Product product = _mapper.Map<Product>(productDto);
        return await _productRepository.AddAsync(product);
    }

    public async Task Remove(long productId)
    {
        await _productRepository.RemoveAsync(productId);
    }

    public async Task<Product> Get(long productId)
    {
        return await _productRepository.GetAsync(productId);
    }

    public async Task<Product> UpdatePriceAsync(long productId, double newPrice)
    {
        return await _productRepository.UpdatePriceAsync(productId, newPrice);
    }
    
    public async Task<IReadOnlyCollection<Product>> List(ProductFilter productFilter)
    {
        return await _productRepository.GetAllWithFilterAsync(productFilter);
    }
}