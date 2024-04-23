using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Repositories;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories;

public sealed class ItemRepository : PgRepository, IItemRepository
{
    public ItemRepository(string connectionString) 
        : base(connectionString)
    { }

    public async Task Update(ItemSaleEntityV1[] saleEntities, CancellationToken token)
    {
        const string sqlQuery = @"
INSERT INTO item_sales (item_id, reserved, sold, canceled, modified_at)
SELECT item_id, reserved, sold, canceled, modified_at
  FROM UNNEST(@Items)
    ON CONFLICT (item_id) DO UPDATE SET
       created = EXCLUDED.created
     , reserved = EXCLUDED.reserved
     , sold = EXCLUDED.sold
     , modified_at = EXCLUDED.modified_at
";
        await using var connection = await GetConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    Items = saleEntities
                }, cancellationToken: token));
    }
    
    public async Task<ItemSaleEntityV1[]> GetByIds(long[] itemIds, CancellationToken token)
    {
        const string sqlQuery = @"
select item_id
     , reserved
     , sold
     , canceled
     , modified_at
  from YourTable
 where id IN @Ids";
        
        await using var connection = await GetConnection();
        var taskEntities = await connection.QueryAsync<ItemSaleEntityV1>(
            new CommandDefinition(sqlQuery,
                new
                {
                    ItemId = itemIds
                }, cancellationToken: token));
        
        return taskEntities.ToArray();
    }
}
