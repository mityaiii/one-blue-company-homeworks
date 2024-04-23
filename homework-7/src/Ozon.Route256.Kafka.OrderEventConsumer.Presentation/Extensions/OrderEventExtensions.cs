using System;
using System.Linq;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Order;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.ValueObjects;
using OrderEvent = Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Contracts.OrderEvent;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Extensions;

public static class OrderEventExtensions
{
    public static Status ToDomainOrderEventStatus(this OrderEvent.OrderStatus orderStatus)
    {
        return orderStatus switch
        {
            OrderEvent.OrderStatus.Created => Status.Created,
            OrderEvent.OrderStatus.Canceled => Status.Canceled,
            OrderEvent.OrderStatus.Delivered => Status.Delivered,
            _ => throw new ArgumentOutOfRangeException(nameof(orderStatus))
        };
    }
    
    public static OrderEventPosition ToDomainOrderEventPosition(this OrderEvent.OrderEventPosition orderEventPosition)
    {
        const decimal grpcNanosMultiplier = 1_000_000_000m;

        return new OrderEventPosition(new ItemId(orderEventPosition.ItemId),
            orderEventPosition.Quantity,
            new Money()
            {
                Value = orderEventPosition.Price.Units + orderEventPosition.Price.Nanos / grpcNanosMultiplier,
                Currency = orderEventPosition.Price.Currency
            });
    }

    public static Domain.Order.OrderEvent ToDomainOrderEvent(this OrderEvent orderEvent)
    {
        var orderEventPositions = orderEvent.Positions
            .Select(o => o.ToDomainOrderEventPosition())
            .ToArray();
        
        return new Domain.Order.OrderEvent(new OrderId(orderEvent.OrderId),
            new UserId(orderEvent.UserId),
            new WarehouseId(orderEvent.WarehouseId),
            orderEvent.Status.ToDomainOrderEventStatus(),
            orderEvent.Moment,
            orderEventPositions);
    }
}