using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Order;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Services.Interfaces;

public interface IItemSaleService
{
    Task ProcessOrderEvent(OrderEvent orderEvent, CancellationToken token);
}