using Core.Entities;
using Core.Entities.Calculators;
using Core.Exceptions;
using Core.Repositories;

namespace Core.Services.DemandServices;

public class DemandService : IDemandService
{
    private readonly ISalesHistoryRepository _salesHistoryRepository;
    private readonly IDemandCalculator _demandCalculator;

    public DemandService(ISalesHistoryRepository salesHistoryRepository, IDemandCalculator demandCalculator)
    {
        _salesHistoryRepository = salesHistoryRepository ??
                                  throw new ArgumentNullException(nameof(salesHistoryRepository));
        _demandCalculator = demandCalculator ?? 
                            throw new ArgumentNullException(nameof(demandCalculator));
    }

    public int Demand(long productId, int daysAmount)
    {
        if (daysAmount < 0)
        {
            throw new NegativeValueException($"{nameof(daysAmount)} should be positive {daysAmount}");
        }

        if (productId < 0)
        {
            throw new NegativeValueException($"{nameof(productId)} should be positive {productId}");
        }
        
        IReadOnlyCollection<SalesHistory> salesHistories = _salesHistoryRepository.Get(productId);
        return _demandCalculator.Calculate(salesHistories, productId, daysAmount);
    }
}