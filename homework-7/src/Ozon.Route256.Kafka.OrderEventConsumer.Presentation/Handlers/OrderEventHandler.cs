using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Services.Interfaces;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Kafka;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Extensions;
using OrderEvent = Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Contracts.OrderEvent;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Handlers;

public class OrderEventHandler : IHandler<Ignore, OrderEvent>
{
    private readonly ILogger<OrderEventHandler> _logger;
    
    private readonly IItemSaleService _itemSaleService;
    private readonly IPaymentService _paymentService;

    public OrderEventHandler(ILogger<OrderEventHandler> logger,
        IItemSaleService itemSaleService,
        IPaymentService paymentService)
    {
        _logger = logger;
        _itemSaleService = itemSaleService;
        _paymentService = paymentService;
    }

    public async Task Handle(IReadOnlyCollection<ConsumeResult<Ignore, OrderEvent>> messages, CancellationToken token)
    {
        _logger.LogInformation("Received {Count} messages", messages.Count);
        
        var orderEvents = messages
                .Select(m => m.Message.Value.ToDomainOrderEvent());
        foreach (var orderEvent in orderEvents)
        {
            await _paymentService.ProcessOrderEvent(orderEvent, token);
            await _itemSaleService.ProcessOrderEvent(orderEvent, token);
        }
        
        _logger.LogInformation("Handled {Count} messages", messages.Count);
    }
}
