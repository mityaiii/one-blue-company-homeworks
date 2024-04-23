using Core.Entities;
using Core.Entities.Calculators;
using Core.Exceptions;
using Core.Repositories;

namespace Core.Services.PredictionServices;

public class PredictionService : IPredictionService
{
    private readonly ISalesSeasonalityRepository _salesSeasonalityRepository;
    private readonly IPredictionCalculator _predictionCalculator;

    public PredictionService(ISalesSeasonalityRepository salesSeasonalityRepository, 
        IPredictionCalculator predictionCalculator)
    {
        _salesSeasonalityRepository = salesSeasonalityRepository ??
                                      throw new ArgumentNullException(nameof(salesSeasonalityRepository));
        _predictionCalculator = predictionCalculator ?? 
                                 throw new ArgumentNullException(nameof(predictionCalculator));
    }
    
    public int Predicate(long productId, int daysAmount)
    {
        if (daysAmount < 0)
        {
            throw new NegativeValueException($"{nameof(daysAmount)} should be positive {daysAmount}");
        }

        if (productId < 0)
        {
            throw new NegativeValueException($"{nameof(productId)} should be positive {productId}");
        }
        
        IReadOnlyCollection<SalesSeasonality> salesSeasonalities = _salesSeasonalityRepository.Get(productId);

        return _predictionCalculator.Calculate( salesSeasonalities, productId, daysAmount);
    }
}