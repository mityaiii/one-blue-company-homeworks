using ConsolePresentation.Entities;
using ConsolePresentation.Entities.CommandChainLinks;
using ConsolePresentation.Entities.CommandParsers;
using ConsolePresentation.Entities.Commands;
using Core.Services;
using Core.Services.AdsServices;
using Core.Services.DemandServices;
using Core.Services.PredictionServices;
using DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace ConsolePresentation;

public class Program
{
    public static void Main(string[] args)
    {
        string? salesHistoryFilePath = Environment.GetEnvironmentVariable("SALES_HISTIRY_FILE_PATH");
        string? salesSeasonsFilePath = Environment.GetEnvironmentVariable("SALES_SEASONS_FILE_PATH");
        
        var collection = new ServiceCollection()
            .AddAdsServiceExtensions()
            .AddPredictionServiceExtensions()
            .AddDemandServiceExtensions()
            .AddDataAccessExtensions(salesHistoryFilePath, salesSeasonsFilePath)
            .AddConsolePresentationExtensions();
        
        ServiceProvider provider = collection.BuildServiceProvider();
        using IServiceScope scope = provider.CreateScope();
        
        var adsService = scope.ServiceProvider.GetRequiredService<IAdsService>();
        var predictionService = scope.ServiceProvider.GetRequiredService<IPredictionService>();
        var demandService = scope.ServiceProvider.GetRequiredService<IDemandService>();
        
        var minimalCommandParser = scope.ServiceProvider.GetRequiredService<IMinimalCommandParser>();
        var extendedCommandParser = scope.ServiceProvider.GetRequiredService<IExtendedCommandParser>();
        
        AdsChainLink adsChainLink = new AdsChainLink(adsService, minimalCommandParser);
        DemandChainLink demandChainLinkBase = new DemandChainLink(demandService, extendedCommandParser);
        PredictionChainLink predictionChainLink = new PredictionChainLink(predictionService, extendedCommandParser);
        
        adsChainLink.AddNext(demandChainLinkBase);
        adsChainLink.AddNext(predictionChainLink);
        
        ICommand command = adsChainLink.Handle(args);
        command.Execute();
    }
}