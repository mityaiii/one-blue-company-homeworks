using Microsoft.Extensions.DependencyInjection;
using ProductService.DataAccess.Repositories;
using ProductService.Domain.Entities.Repositories;

namespace ProductService.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddSingleton<IProductRepository, ProductRepository>();
    }
}