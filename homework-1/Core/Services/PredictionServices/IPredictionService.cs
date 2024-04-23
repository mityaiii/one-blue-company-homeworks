namespace Core.Services.PredictionServices;

public interface IPredictionService
{
    int Predicate(long productId, int daysAmount);
}