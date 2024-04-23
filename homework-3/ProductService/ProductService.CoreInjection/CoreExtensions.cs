using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ProductService.Core.Entities;
using ProductService.Core.Services;

namespace ProductService.CoreInjection;

public static class CoreExtensions
{
    public static IServiceCollection AddCoreExtensions(this IServiceCollection collection, 
        HostBuilderContext configuration, ICsvParser csvParser, ICsvFileWriter csvFileWriter)
    {
        collection.Configure<AppSettings>(configuration.Configuration.GetSection(nameof(AppSettings)));
        return collection.AddScoped<IDemandService, DemandService>(provider =>
        {
            IOptionsMonitor<AppSettings> optionsMonitor = provider.GetRequiredService<IOptionsMonitor<AppSettings>>();
            var demandCalculator = new DemandCalculator();
                    
            return new DemandService(demandCalculator, csvParser, csvFileWriter, optionsMonitor);
        });
    }
}