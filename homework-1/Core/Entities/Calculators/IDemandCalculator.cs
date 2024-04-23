namespace Core.Entities.Calculators;

public interface IDemandCalculator
{
    int Calculate(IReadOnlyCollection<SalesHistory> salesHistories, long productId, int daysAmount);
}