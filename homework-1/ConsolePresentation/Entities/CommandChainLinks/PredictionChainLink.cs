using ConsolePresentation.Entities.CommandParsers;
using ConsolePresentation.Entities.Commands;
using ConsolePresentation.Exceptions;
using Core.Services;
using Core.Services.PredictionServices;

namespace ConsolePresentation.Entities.CommandChainLinks;

public class PredictionChainLink : CommandChainLinkBase
{
    private readonly IPredictionService _predictionService;
    private readonly IExtendedCommandParser _extendedCommandParser;
    public PredictionChainLink(IPredictionService predictionService, IExtendedCommandParser extendedCommandParser)
    {
        _predictionService = predictionService ?? throw new ArgumentNullException(nameof(predictionService));
        _extendedCommandParser = extendedCommandParser ?? throw new ArgumentNullException(nameof(extendedCommandParser));
    }
    
    public override ICommand Handle(string[] args)
    {
        string inputCommand = _extendedCommandParser.ParseCommand(args);
        long productId = _extendedCommandParser.ParseProductId(args);
        int daysAmount = _extendedCommandParser.ParseDaysAmount(args);
        
        ArgumentException.ThrowIfNullOrEmpty(inputCommand);

        if (inputCommand.Equals("prediction", StringComparison.CurrentCultureIgnoreCase))
        {
            return new PredictionCommand(_predictionService, productId, daysAmount);
        }
        
        if (Next is null)
        {
            throw new InvalidCommandException($"{inputCommand} not found");
        }
        
        return Next.Handle(args);
    }
}