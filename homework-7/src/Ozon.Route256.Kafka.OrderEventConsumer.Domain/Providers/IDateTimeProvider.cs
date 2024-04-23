using System;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Providers;

public interface IDateTimeProvider
{
    public DateTimeOffset GetNow();
}