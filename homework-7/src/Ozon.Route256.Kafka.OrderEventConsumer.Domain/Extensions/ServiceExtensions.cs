using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Providers;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Services;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Services.Interfaces;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Providers;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddServiceExtensions(this IServiceCollection collection)
    {
        collection.AddDateTimeProvider();
        
        collection.AddSingleton<IItemSaleService, ItemSaleService>();
        collection.AddSingleton<IPaymentService, PaymentService>();

        return collection;
    }
    
    private static IServiceCollection AddDateTimeProvider(this IServiceCollection collection)
    {
        collection.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        return collection;
    }

}