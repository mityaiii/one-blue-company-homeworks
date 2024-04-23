using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Kafka;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation;

public class KafkaBackgroundService<TKey, TValue> : BackgroundService
{
    private readonly KafkaAsyncConsumer<TKey, TValue> _consumer;
    private readonly ILogger<KafkaBackgroundService<TKey, TValue>> _logger;

    public KafkaBackgroundService(IServiceProvider serviceProvider,
        ILogger<KafkaBackgroundService<TKey, TValue>> logger)
    {
        _logger = logger;
        _consumer = serviceProvider.GetRequiredService<KafkaAsyncConsumer<TKey, TValue>>();
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Dispose();

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _consumer.Consume(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occured");
        }
    }
}
