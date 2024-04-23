using System;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;

public class PaymentEntityV1
{
    public long SellerId { get; init; }
    public decimal Rub { get; init; }
    public decimal Kzt { get; init; }
}