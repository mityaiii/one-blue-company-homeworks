using Core.Entities.Calculators;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Services.AdsServices;

public static class AdsServiceExtensions
{
    public static IServiceCollection AddAdsServiceExtensions(this IServiceCollection collection)
    {
        collection.AddScoped<IAdsService, AdsService>();
        collection.AddTransient<IAdsCalculator, AdsCalculator>();

        return collection;
    }
}