using Microsoft.Extensions.DependencyInjection;
using ProductService.Domain.Services;

namespace ProductService.Core.DependencyInjection;

public static class ProductServiceExtensions
{
    public static IServiceCollection AddProductService(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IProductServiceApplication, ProductServiceApplication>();
        return serviceCollection;
    }
}