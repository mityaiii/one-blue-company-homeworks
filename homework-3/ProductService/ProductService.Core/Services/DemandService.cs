using System.Threading.Channels;
using Microsoft.Extensions.Options;
using ProductService.Core.Entities;
using ProductService.Core.Models;

namespace ProductService.Core.Services;

public class DemandService : IDemandService
{
    private readonly IDemandCalculator _demandCalculator;
    private readonly ICsvParser _parser;
    private readonly ICsvFileWriter _csvWriter;
    private readonly IOptionsMonitor<AppSettings> _optionsMonitor;
    private readonly IDisposable? _updateSubscriber;
    private readonly ManualResetEventSlim _manualResetEvent;
    
    private readonly Channel<ProductDemand> _productDemands;
    private Channel<ProductSaleInfo> _channel;
    
    private int _readProductsAmount;
    private int _proceedProductAmount;
    private int _handledProductAmount;
    
    public DemandService(IDemandCalculator demandCalculator, 
        ICsvParser parser, 
        ICsvFileWriter csvWriter, 
        IOptionsMonitor<AppSettings> optionsMonitor)
    {
        _demandCalculator = demandCalculator;
        _parser = parser;
        
        _optionsMonitor = optionsMonitor;
        _csvWriter = csvWriter;
        _manualResetEvent = new ManualResetEventSlim(true);

        _updateSubscriber = _optionsMonitor.OnChange( settings =>
        {
            Task.FromResult(UpdateThreadsAmount(settings.ThreadsAmount));
        });

        _productDemands = Channel.CreateUnbounded<ProductDemand>();
        _channel = Channel.CreateBounded<ProductSaleInfo>(_optionsMonitor.CurrentValue.ThreadsAmount);
    }
    
    public async Task StartCalculateAsync(CancellationToken cancellationToken)
    {
        var taskProducer = WriteAsync(cancellationToken);
        var taskConsumer = ReadAsync(cancellationToken);
        var taskWriteInFile = WriteInFileAsync(cancellationToken);
        
        await Task.WhenAll(taskProducer, taskConsumer, taskWriteInFile);
    }

    public Task StopCalculate()
    {
        _parser.Close();
        _csvWriter.Close();
        _updateSubscriber?.Dispose();
        
        return Task.CompletedTask;
    }

    private async Task WriteAsync(CancellationToken cancellationToken)
    {
        while (_parser.GetNext() is { } productSaleInfo)
        {
            _manualResetEvent.Wait(cancellationToken);
            await _channel.Writer.WriteAsync(productSaleInfo, cancellationToken);
            Interlocked.Increment(ref _readProductsAmount);
        }
        
        _channel.Writer.Complete();
    }

    private async Task ReadAsync(CancellationToken cancellationToken)
    {
        while (!_channel.Reader.Completion.IsCompleted)
        {
            var productInfoSale = await _channel.Reader.ReadAsync(cancellationToken);
            Interlocked.Increment(ref _proceedProductAmount);
            int demand = await _demandCalculator.Calculate(productInfoSale);
            Interlocked.Increment(ref _handledProductAmount);
            
            await _productDemands.Writer.WriteAsync(new ProductDemand(productInfoSale.Id, demand), cancellationToken);

            if (_readProductsAmount == _handledProductAmount)
            {
                _manualResetEvent.Wait(cancellationToken);
            }
        }

        _productDemands.Writer.Complete();
    }

    private async Task WriteInFileAsync(CancellationToken cancellationToken)
    {
        await foreach (var productDemand in _productDemands.Reader.ReadAllAsync(cancellationToken))
        {
            _csvWriter.Write(productDemand);
            PrintStatistic();   
        }
    }

    private async Task WaitAllProceedTasks()
    {
        while (_handledProductAmount < _readProductsAmount)
        {
            await Task.Delay(100);
        }
    }
    
    private async Task UpdateThreadsAmount(int threadsAmount)
    {
        _manualResetEvent.Reset();
        await WaitAllProceedTasks();
        _channel = Channel.CreateBounded<ProductSaleInfo>(threadsAmount);
        _manualResetEvent.Set();
    }
    private void PrintStatistic()
    {
        Console.WriteLine($"ReadProductsAmount: {_readProductsAmount} ProceedProductsAmount: {_proceedProductAmount} HandledProductsAmount: {_handledProductAmount}");
    }
}