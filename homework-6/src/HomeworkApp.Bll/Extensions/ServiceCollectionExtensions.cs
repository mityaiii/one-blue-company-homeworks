using HomeworkApp.Bll.Providers;
using HomeworkApp.Bll.Providers.Interfaces;
using HomeworkApp.Bll.Services;
using HomeworkApp.Bll.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HomeworkApp.Bll.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBllServices(
        this IServiceCollection services)
    {
        services.AddScoped<IDistributedCacheProvider, DistributedCacheProvider>();
        services.AddScoped<IRateLimiterService, RateLimiterService>();
        services.AddScoped<ITaskService, TaskService>();
        services.AddScoped<ITaskCommentsService, TaskCommentsService>();
        
        return services;
    }
}