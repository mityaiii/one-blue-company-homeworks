using System;
using Confluent.Kafka;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation;

public class NullTextJsonSerializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        return default!;
    }
}