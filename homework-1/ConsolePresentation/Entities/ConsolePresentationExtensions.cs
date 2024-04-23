using ConsolePresentation.Entities.CommandParsers;
using ConsolePresentation.Entities.Commands;
using Core.Services.PredictionServices;
using Microsoft.Extensions.DependencyInjection;

namespace ConsolePresentation.Entities;

public static class ConsolePresentationExtensions
{
    public static IServiceCollection AddConsolePresentationExtensions(this IServiceCollection collection)
    {
        collection.AddScoped<IMinimalCommandParser, MinimalCommandParser>();
        collection.AddScoped<IExtendedCommandParser, ExtendedCommandParser>();

        collection.AddScoped<IMinimalCommandParser, MinimalCommandParser>();
        collection.AddScoped<IExtendedCommandParser, ExtendedCommandParser>();

        return collection;
    }
}