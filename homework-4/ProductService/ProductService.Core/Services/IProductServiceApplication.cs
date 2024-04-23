using ProductService.Domain.Entities.Products;
using ProductService.Domain.Models;

namespace ProductService.Domain.Services;

public interface IProductServiceApplication
{
    Task<long> AddAsync(Product productDto);
    Task RemoveAsync(long productId);
    Task<Product> GetAsync(long productId);
    Task<Product> UpdatePriceAsync(long productId, double newPrice);
    Task<IReadOnlyCollection<Product>> List(ProductFilter productFilter);
}