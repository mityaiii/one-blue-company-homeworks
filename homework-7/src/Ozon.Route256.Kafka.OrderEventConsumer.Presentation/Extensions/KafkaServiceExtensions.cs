using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Kafka;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Extensions;

public static class KafkaHandlerExtensions
{
    public static IServiceCollection AddKafkaHandler<TKey, TValue, THandler>(this IServiceCollection services,
        IDeserializer<TKey> keySerializer,
        IDeserializer<TValue> valueSerializer)
        where THandler : class, IHandler<TKey, TValue>
    {
        services.AddSingleton<IDeserializer<TKey>>(p 
            => keySerializer);
        services.AddSingleton<IDeserializer<TValue>>(p 
            => valueSerializer);
        services.AddSingleton<IHandler<TKey, TValue>, THandler>();
        
        services.AddSingleton<KafkaAsyncConsumer<TKey, TValue>>();
        
        services.AddHostedService<KafkaBackgroundService<TKey, TValue>>();
        
        return services;
    }
}