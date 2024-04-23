using Core.Repositories;
using DataAccess.Parsers;
using Microsoft.Extensions.DependencyInjection;

namespace DataAccess.Repositories;

public static class DataAccessExtensions
{
    public static IServiceCollection AddDataAccessExtensions(this IServiceCollection collection, 
        string salesHistoryDbPath, 
        string salesSeasonalityDbPath)
    {
        collection.AddScoped<ISalesHistoryParser, SalesHistoryParser>();
        collection.AddScoped<ISalesSeasonalityParser, SalesSeasonalityParser>();
        
        collection.AddScoped<ISalesHistoryRepository>(provider =>
        {
            var salesHistoryParser = provider.GetRequiredService<ISalesHistoryParser>();
            return new SalesHistoryRepository(salesHistoryParser, salesHistoryDbPath);
        });
        
        collection.AddScoped<ISalesSeasonalityRepository>(provider =>
        {
            var salesSeasonalityParser = provider.GetRequiredService<ISalesSeasonalityParser>();
            return new SalesSeasonalityRepository(salesSeasonalityParser, salesSeasonalityDbPath);
        });
        
        return collection;
    }
}