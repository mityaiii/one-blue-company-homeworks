namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Settings;

public record KafkaBackgroundOptions
{
    public required string BootstrapServers { get; init; }
    public required string Topic { get; init; }
    public required string GroupId { get; init; }
    public required int ChannelCapacity { get; init; }
    public required int BufferDelay { get; init; }
    public required int MaxPollyRetries { get; init; }
}