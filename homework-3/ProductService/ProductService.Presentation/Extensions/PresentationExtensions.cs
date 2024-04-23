using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Presentation.Services;

namespace Presentation.Extensions;

public static class PresentationExtensions
{
    public static IServiceCollection AddPresentationExtensions(this IServiceCollection collection)
    {
        return collection.AddScoped<IHostedService, ConsoleService>();
    }
}