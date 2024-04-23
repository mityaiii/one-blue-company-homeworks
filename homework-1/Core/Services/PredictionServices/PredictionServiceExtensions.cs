using Core.Entities.Calculators;
using Core.Repositories;
using Core.Services.AdsServices;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services.PredictionServices;

public static class PredictionServiceExtensions
{
    public static IServiceCollection AddPredictionServiceExtensions(this IServiceCollection collection)
    {
        collection.AddScoped<IPredictionService, PredictionService>();
        collection.AddTransient<IPredictionCalculator, PredictionCalculator>();

        return collection;
    }
}