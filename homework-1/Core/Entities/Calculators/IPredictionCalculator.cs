namespace Core.Entities.Calculators;

public interface IPredictionCalculator
{
    public int Calculate(IReadOnlyCollection<SalesSeasonality> salesSeasonalities, long productId, int daysAmount);
}