using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Order;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Repositories;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Services.Interfaces;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly Dictionary<long, ItemSaleEntityV1> _paymentDict = new();
    private const long GrpcNanosMultiplier = 1_000_000;

    public PaymentService(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository;
    }

    public async Task ProcessOrderEvent(OrderEvent orderEvent, CancellationToken token)
    {
        // foreach (var orderEventPosition in orderEvent.Positions)
        // {
        //     var sellerId = orderEventPosition.ItemId.Value / GrpcNanosMultiplier;
        //
        //     using var transaction = CreateTransactionScope();
        //
        //     var paymentEntity = await _paymentRepository.Get(sellerId, token);
        //     var payment = orderEventPosition.Quantity * orderEventPosition.Price.Value;
        //     var currency = orderEventPosition.Price.Currency;
        //
        //     if (paymentEntity is null)
        //     {
        //         await AddPayment(sellerId, payment, currency, token);
        //     
        //         return;
        //     }
        //     
        //     await UpdatePayment(paymentEntity, payment, currency, token);
        // }
        foreach (var orderEventPosition in orderEvent.Positions)
        {
            var itemId = orderEventPosition.ItemId.Value;
            var quantity = orderEventPosition.Quantity;

            if (_paymentDict.TryGetValue(itemId, out var value))
            {
                var updatedItemSale = UpdatePayment(value, quantity);
                _itemSaleDict[itemId] = updatedItemSale;
            }
            else
            {
                _itemSaleDict.Add(itemId, new ItemSaleEntityV1
                {
                    ItemId = new ItemId(itemId),
                    Reserved = 0,
                    Canceled = 0,
                    Sold = 0,
                    ModifiedAt = _dateTimeProvider.GetNow()
                });
            }

            using var transaction = CreateTransactionScope();
            var itemSalesInRepository = 
                await _itemRepository.GetByIds(_itemSaleDict.Keys.ToArray(), token);

            foreach (var itemSale in itemSalesInRepository)
            {
                var itemSaleId = itemSale.ItemId.Value;
                var upgradableItemSale = _itemSaleDict[itemSaleId];
                    
                upgradableItemSale = upgradableItemSale with
                {
                    Reserved = itemSale.Reserved + upgradableItemSale.Reserved,
                    Canceled = itemSale.Canceled + upgradableItemSale.Canceled,
                    Sold = itemSale.Sold + upgradableItemSale.Sold,
                };

                _itemSaleDict[itemSaleId] = upgradableItemSale;
            }

            await _itemRepository.Update(_itemSaleDict.Values.ToArray(), token);
        }
        
    _itemSaleDict.Clear();
    }
    
    private async Task AddPayment(long sellerId, decimal payment, string currency, CancellationToken token)
    {
        var paymentEntity = new PaymentEntityV1()
        {
            SellerId = sellerId,
            Kzt = 0,
            Rub = 0,
        };
        
        var updatedPayment = UpdatePayment(paymentEntity, currency, payment);
        await _paymentRepository.Add(updatedPayment, token);
    }
    
    private async Task UpdatePayment(PaymentEntityV1 paymentEntity, decimal payment, string currency, CancellationToken token)
    {
        var updatedPayment = UpdatePayment(paymentEntity, currency, payment);
        await _paymentRepository.Update(updatedPayment, token);
    }
    
    private TransactionScope CreateTransactionScope(
        IsolationLevel level = IsolationLevel.RepeatableRead)
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions 
            { 
                IsolationLevel = level, 
                Timeout = TimeSpan.FromSeconds(5) 
            },
            TransactionScopeAsyncFlowOption.Enabled);
    }
    
    private PaymentEntityV1 UpdatePayment(PaymentEntityV1 paymentEntity, string currency, decimal payment)
    {
        return currency switch
        {
            "RUB" => new PaymentEntityV1() { SellerId = paymentEntity.SellerId, Rub = paymentEntity.Rub + payment },
            "KZT" => new PaymentEntityV1() { SellerId = paymentEntity.SellerId, Kzt = paymentEntity.Kzt + payment },
            _ => throw new ArgumentOutOfRangeException(nameof(currency), currency, null)
        };
    }
}