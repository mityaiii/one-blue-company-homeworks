namespace Core.Entities.Calculators;

public interface IAdsCalculator
{
    decimal Calculate(IReadOnlyCollection<SalesHistory> salesHistories);
}