using System;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Providers;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Providers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset GetNow()
    {
        return DateTimeOffset.Now.UtcDateTime;
    }
}