using ProductService.Domain.Entities.Products;
using ProductService.Domain.Models;

namespace ProductService.Domain.Entities.Repositories;

public interface IProductRepository
{
    Task<long> AddAsync(Product product);
    Task RemoveAsync(long productId);
    Task<Product> UpdatePriceAsync(long productId, double newPrice);
    Task<Product> GetAsync(long productId);
    Task<IReadOnlyCollection<Product>> GetAllWithFilterAsync(ProductFilter productFilter);
}