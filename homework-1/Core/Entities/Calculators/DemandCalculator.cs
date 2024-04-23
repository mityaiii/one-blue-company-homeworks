using Core.Services;
using Core.Services.PredictionServices;

namespace Core.Entities.Calculators;

public class DemandCalculator : IDemandCalculator
{
    private readonly IPredictionService _predictionService;
    public DemandCalculator(IPredictionService predictionService) {
        _predictionService = predictionService ?? throw new ArgumentNullException(nameof(predictionService));
    }

    public int Calculate(IReadOnlyCollection<SalesHistory> salesHistories, long productId, int daysAmount)
    {
        int predicationProductAmount = _predictionService.Predicate(productId, daysAmount);

        SalesHistory tmpHistory = salesHistories.First();
        
        foreach (var salesHistory in salesHistories)
        {
            if (salesHistory.Date > tmpHistory.Date)
            {
                tmpHistory = salesHistory;
            }
        }
        
        int totalProductsAmount = tmpHistory.Stock;
        return predicationProductAmount - totalProductsAmount;
    }
}