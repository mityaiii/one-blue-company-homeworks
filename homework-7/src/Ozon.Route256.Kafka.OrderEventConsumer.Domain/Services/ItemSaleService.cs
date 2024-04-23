using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Entities;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Order;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Repositories;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Services.Interfaces;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.ValueObjects;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Providers;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Domain.Services;

public class ItemSaleService :  IItemSaleService
{
    private readonly IItemRepository _itemRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly Dictionary<long, ItemSaleEntityV1> _itemSaleDict = new();

    public ItemSaleService(IItemRepository itemRepository, IDateTimeProvider dateTimeProvider)
    {
        _itemRepository = itemRepository;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task ProcessOrderEvent(OrderEvent orderEvent, CancellationToken token)
    {
        var orderEventPositions =  orderEvent.Positions
            .GroupBy(o => o.ItemId)
            .ToArray();

        var itemIds = orderEventPositions
            .Select(group => group.Key.Value)
            .ToArray();

        var itemSales = await _itemRepository.GetByIds(itemIds, token);
        
        foreach (var orderEventGroupPosition in orderEventPositions)
        {
            long itemId = orderEventGroupPosition.First().ItemId.Value;
            int quantity = 0;
            
            foreach (var orderEventPosition in orderEventGroupPosition)
            {
                quantity += orderEventPosition.Quantity;
            }
            
            UpdateItemSale(, orderEvent.Status, quantity);
        }
        

        // foreach (var orderEventPosition in orderEvent.Positions)
        // {
        //     var itemId = orderEventPosition.ItemId.Value;
        //     var quantity = orderEventPosition.Quantity;
        //
        //     if (_itemSaleDict.TryGetValue(itemId, out var value))
        //     {
        //         var updatedItemSale = UpdateItemSale(value, orderEvent.Status, quantity);
        //         _itemSaleDict[itemId] = updatedItemSale;
        //     }
        //     else
        //     {
        //         _itemSaleDict.Add(itemId, new ItemSaleEntityV1
        //         {
        //             ItemId = new ItemId(itemId),
        //             Reserved = 0,
        //             Canceled = 0,
        //             Sold = 0,
        //             ModifiedAt = _dateTimeProvider.GetNow()
        //         });
        //     }
        //
        //     using var transaction = CreateTransactionScope();
        //     var itemSalesInRepository = 
        //         await _itemRepository.GetByIds(_itemSaleDict.Keys.ToArray(), token);
        //
        //     foreach (var itemSale in itemSalesInRepository)
        //     {
        //         var itemSaleId = itemSale.ItemId.Value;
        //         var upgradableItemSale = _itemSaleDict[itemSaleId];
        //         
        //         upgradableItemSale = upgradableItemSale with
        //         {
        //             Reserved = itemSale.Reserved + upgradableItemSale.Reserved,
        //             Canceled = itemSale.Canceled + upgradableItemSale.Canceled,
        //             Sold = itemSale.Sold + upgradableItemSale.Sold,
        //         };
        //
        //         _itemSaleDict[itemSaleId] = upgradableItemSale;
        //     }
        //
        //     await _itemRepository.Update(_itemSaleDict.Values.ToArray(), token);
        // }
        //
        // _itemSaleDict.Clear();
    }

    private ItemSaleEntityV1 UpdateItemSale(ItemSaleEntityV1 itemSale, Status status, int quantity)
    {
        var reserved = 0;
        var sold = 0;
        var canceled = 0;
        switch (status)
        {
            case Status.Created:
                reserved += quantity;
                break;
            case Status.Canceled:
                sold += quantity;
                reserved -= quantity;
                break;
            case Status.Delivered:
                canceled += quantity;
                reserved -= quantity;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
            
        return itemSale with
        {
            Reserved = reserved,
            Sold = sold,
            Canceled = canceled,
        };
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
}