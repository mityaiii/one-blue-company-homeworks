using Microsoft.Extensions.Hosting;
using ProductService.Core.Services;

namespace Presentation.Services;

public class ConsoleService : IHostedService
{
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly IDemandService _demandService;
    private readonly CancellationToken _cancellationToken;
    private readonly CancellationTokenSource _cancellationTokenSource;
    
    public ConsoleService(IHostApplicationLifetime applicationLifetime, IDemandService demandService)
    {
        _applicationLifetime = applicationLifetime;
        _demandService = demandService;
        _cancellationTokenSource = new CancellationTokenSource();
        
        _cancellationToken = _cancellationTokenSource.Token;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Task demandCalculationTask = _demandService.StartCalculateAsync(_cancellationToken);

        try
        {
            await Task.WhenAny(demandCalculationTask, WaitEscapeKeyPressing());
        }
        catch (TaskCanceledException e)
        {
            await _demandService.StopCalculate();
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _demandService.StopCalculate();

        return Task.CompletedTask;
    }
    
    private async Task WaitEscapeKeyPressing()
    {
        while (true)
        {
             var keyInfo = await Task.Run(() => Console.ReadKey(intercept: true), _cancellationToken);
             if (keyInfo.Key != ConsoleKey.Q) 
                 continue;

             await _cancellationTokenSource.CancelAsync();
             _applicationLifetime.StopApplication();
             break;
        }
    }
}