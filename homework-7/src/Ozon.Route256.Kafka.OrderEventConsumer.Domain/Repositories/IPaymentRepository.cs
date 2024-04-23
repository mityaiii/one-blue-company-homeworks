using System.Threading;
using System.Threading.Tasks;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Repositories;

public interface IPaymentRepository
{
    Task Add(PaymentEntityV1 paymentEntity, CancellationToken token);
    Task Update(PaymentEntityV1 paymentEntity, CancellationToken token);
    Task<PaymentEntityV1?> Get(long sellerId, CancellationToken token);
}