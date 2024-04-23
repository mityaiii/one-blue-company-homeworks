using Core.Entities.Calculators;
using Core.Services.PredictionServices;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services.DemandServices;

public static class DemandServiceExtensions
{
    public static IServiceCollection AddDemandServiceExtensions(this IServiceCollection collection)
    {
        collection.AddScoped<IDemandService, DemandService>();
        collection.AddTransient<IDemandCalculator, DemandCalculator>();
        
        return collection;
    }
}