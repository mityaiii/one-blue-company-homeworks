using Core.Entities;
using Core.Entities.Calculators;
using Core.Exceptions;
using Core.Repositories;

namespace Core.Services.AdsServices;

public class AdsService : IAdsService
{
    private readonly ISalesHistoryRepository _salesHistoryRepository;
    private readonly IAdsCalculator _adsCalculator;

    public AdsService(ISalesHistoryRepository salesHistoryRepository, IAdsCalculator adsCalculator)
    {
        _salesHistoryRepository = salesHistoryRepository ?? 
                                  throw new ArgumentNullException(nameof(salesHistoryRepository));
        _adsCalculator = adsCalculator ??
                         throw new ArgumentNullException(nameof(adsCalculator));
    }
    
    public decimal CalculateAds(long productId)
    {
        if (productId < 0)
        {
            throw new NegativeValueException($"{nameof(productId)} should be positive {productId}");
        }
        
        IReadOnlyCollection<SalesHistory> salesHistories = _salesHistoryRepository.Get(productId);
        if (salesHistories.Count == 0)
        {
            throw new ValueNotFoundException($"Product with id {productId} not found");
        }
        
        IReadOnlyCollection<SalesHistory> orderedSalesHistories = salesHistories
            .OrderBy(s => s.Date)
            .ToList();
        
        return _adsCalculator.Calculate(orderedSalesHistories);
    }
}