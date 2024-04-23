using ProductService.Domain.Entities.Products;
using ProductService.Domain.Models;

namespace ProductService.Domain.Services;

public interface IProductServiceApplication
{
    Task<long> Add(Product productDto);
    Task Remove(long productId);
    Task<Product> Get(long productId);
    Task<Product> UpdatePriceAsync(long productId, double newPrice);
    Task<IReadOnlyCollection<Product>> List(ProductFilter productFilter);
}