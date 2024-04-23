using AutoMapper;
using ProductService.Domain.Entities.Products;
using ProductService.Domain.Entities.Repositories;
using ProductService.Domain.Models;

namespace ProductService.Domain.Services;

public class ProductServiceApplication : IProductServiceApplication
{
    private readonly IProductRepository _productRepository;

    public ProductServiceApplication(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }
    
    public async Task<long> AddAsync(Product product)
    {
        return await _productRepository.AddAsync(product);
    }

    public async Task RemoveAsync(long productId)
    {
        await _productRepository.RemoveAsync(productId);
    }

    public async Task<Product> GetAsync(long productId)
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