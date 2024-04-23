using System;
using System.Linq;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Common;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Settings;
using Polly;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Kafka;

public sealed class KafkaAsyncConsumer<TKey, TValue> : IDisposable
{
    private readonly int _channelCapacity;
    private readonly TimeSpan _bufferDelay;
    private readonly int _maxPollyRetries;

    private readonly Channel<ConsumeResult<TKey, TValue>> _channel;
    private readonly IConsumer<TKey, TValue> _consumer;
    private readonly IHandler<TKey, TValue> _handler;

    private readonly ILogger<KafkaAsyncConsumer<TKey, TValue>> _logger;

    public KafkaAsyncConsumer(
        IHandler<TKey, TValue> handler,
        IOptions<KafkaBackgroundOptions> options,
        ILogger<KafkaAsyncConsumer<TKey, TValue>> logger,
        IDeserializer<TKey>? keyDeserializer = null,
        IDeserializer<TValue>? valueDeserializer = null)
    {
        _channelCapacity = options.Value.ChannelCapacity;
        _bufferDelay = TimeSpan.FromSeconds(options.Value.BufferDelay);
        _maxPollyRetries = options.Value.MaxPollyRetries;
        
        var builder = new ConsumerBuilder<TKey, TValue>(
            new ConsumerConfig
            {
                BootstrapServers = options.Value.BootstrapServers,
                GroupId = options.Value.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
                EnableAutoOffsetStore = false
            });

        if (keyDeserializer is not null)
        {
            builder.SetKeyDeserializer(keyDeserializer);
        }

        if (valueDeserializer is not null)
        {
            builder.SetValueDeserializer(valueDeserializer);
        }

        _handler = handler;
        _logger = logger;

        _channel = Channel.CreateBounded<ConsumeResult<TKey, TValue>>(
            new BoundedChannelOptions(_channelCapacity)
            {
                SingleWriter = true,
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait
            });

        _consumer = builder.Build();
        _consumer.Subscribe(options.Value.Topic);
    }

    public Task Consume(CancellationToken token)
    {
        var handle = HandleCore(token);
        var consume = ConsumeCore(token);

        return Task.WhenAll(handle, consume);
    }

    private async Task HandleCore(CancellationToken token)
    {
        await Task.Yield();

        await foreach (var consumeResults in _channel.Reader
                           .ReadAllAsync(token)
                           .Buffer(_channelCapacity, _bufferDelay)
                           .WithCancellation(token))
        {
            token.ThrowIfCancellationRequested();
            
            await Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    retryCount: _maxPollyRetries, 
                    sleepDurationProvider: (attemptCount) => TimeSpan.FromMilliseconds(attemptCount * 10),
                    onRetry: (exception, sleepDuration, attemptNumber, context) =>
                    {
                        _logger.LogError(
                            exception, 
                            $"Unhandled exception occured {exception}. Retrying in {sleepDuration}. " +
                            $"{attemptNumber} / {_maxPollyRetries}",
                            sleepDuration, 
                            attemptNumber, 
                            _maxPollyRetries);
                    })
                .ExecuteAsync(async () =>
                {
                    await _handler.Handle(consumeResults, token);
                });

            
            var partitionLastOffsets = consumeResults
                .GroupBy(
                    r => r.Partition.Value,
                    (_, f) => f.MaxBy(p => p.Offset.Value));

            foreach (var partitionLastOffset in partitionLastOffsets)
            {
                _consumer.StoreOffset(partitionLastOffset);
            }
        }
    }

    private async Task ConsumeCore(CancellationToken token)
    {
        await Task.Yield();

        while (_consumer.Consume(token) is { } result)
        {
            await _channel.Writer.WriteAsync(result, token);
            _logger.LogTrace(
                "{Partition}:{Offset}:WriteToChannel",
                result.Partition.Value,
                result.Offset.Value);
        }

        _channel.Writer.Complete();
    }

    public void Dispose() => _consumer.Close();
}
