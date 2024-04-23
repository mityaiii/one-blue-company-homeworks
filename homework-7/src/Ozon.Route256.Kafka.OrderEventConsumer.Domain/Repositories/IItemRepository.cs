using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.ValueObjects;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Repositories;

public interface IItemRepository
{
    Task Update(ItemSaleEntityV1[] saleEntities, CancellationToken token);
    Task<ItemSaleEntityV1[]> GetByIds(long[] itemIds, CancellationToken token);
}
