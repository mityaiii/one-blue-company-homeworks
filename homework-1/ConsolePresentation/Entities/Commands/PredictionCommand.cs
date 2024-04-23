using Core.Exceptions;
using Core.Services;
using Core.Services.PredictionServices;

namespace ConsolePresentation.Entities.Commands;

public class PredictionCommand : ICommand
{
    private readonly IPredictionService _predictionService;
    private readonly long _productId;
    private readonly int _daysAmount;

    public PredictionCommand(IPredictionService predictionService, long productId, int daysAmount)
    {
        _predictionService = predictionService ?? throw new ArgumentNullException($"{predictionService}");
        _productId = productId;
        _daysAmount = daysAmount;
    }

    public void Execute()
    {
        try
        {
            int predication = _predictionService.Predicate(_productId, _daysAmount);
            Console.WriteLine(predication);
        }
        catch (ValueNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }
    }
}