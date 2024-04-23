using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Repositories;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories;

public class PaymentRepository: PgRepository, IPaymentRepository
{
    public PaymentRepository(string connectionString) 
        : base(connectionString)
    { }

    public async Task<PaymentEntityV1[]> GetByIds(long[] sellerIds, CancellationToken token)
    {
        const string sqlQuery = @"
select seller_id
     , rub
     , kzt
  from payments
 where id in @SellerIds;
";
        
        await using var connection = await GetConnection();
        var taskEntities = await connection.QueryAsync<PaymentEntityV1>(
            new CommandDefinition(sqlQuery,
                new
                {
                    SellerIds = sellerIds
                }, cancellationToken: token));
        
        return taskEntities.ToArray();
    }

    public async Task Update(long[] sellerIds, CancellationToken token)
    {
        const string sqlQuery = @"
INSERT INTO payments (seller_id, rub, kzt)
SELECT seller_id, rub, kzt
  FROM UNNEST(@SellerIds)
    ON CONFLICT (seller_id) DO UPDATE SET
       rub = EXCLUDED.rub
     , kzt = EXCLUDED.kzt
";
        await using var connection = await GetConnection();

        await connection.ExecuteAsync(
            new CommandDefinition(
                sqlQuery,
                new
                {
                    SellerIds = sellerIds
                }, cancellationToken: token));
    }
    
    public async Task Add(PaymentEntityV1 paymentEntity, CancellationToken token)
    {
        const string sqlQuery = @"
insert into payments (seller_id, rub, kzt)
values (@SellerId, @Rub, @Kzt);
";
        
        await using var connection = await GetConnection();

        await connection.ExecuteAsync(new CommandDefinition(
            sqlQuery,
            new
            {
                @SellerId = paymentEntity.SellerId,
                @Rub = paymentEntity.Rub,
                @Kzt = paymentEntity.Kzt
            }, cancellationToken: token));
    }

    public async Task Update(PaymentEntityV1 paymentEntity, CancellationToken token)
    {
        const string sqlQuery = @"
update payments
   set rub = @Rub
     , kzt = @Kzt
 where seller_id = @SellerId;
";
        
        await using var connection = await GetConnection();
        
        await connection.ExecuteAsync(
            new CommandDefinition(sqlQuery,
                new 
                {
                    @SellerId = paymentEntity.SellerId,
                    @Kzt = paymentEntity.Kzt,
                    @Rub = paymentEntity.Rub
                }, cancellationToken: token));
    }

    public async Task<PaymentEntityV1?> Get(long sellerId, CancellationToken token)
    {
        const string sqlQuery = @"
select seller_id, rub, kzt
  from payments
 where seller_id = @SellerId;
";
        await using var connection = await GetConnection();
        return await connection.QuerySingleOrDefaultAsync<PaymentEntityV1>(
            new CommandDefinition(sqlQuery,
                new 
                {
                    SellerId = sellerId
                }, cancellationToken: token));
    }
}