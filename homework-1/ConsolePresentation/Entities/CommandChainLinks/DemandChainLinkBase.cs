using ConsolePresentation.Entities.CommandParsers;
using ConsolePresentation.Entities.Commands;
using ConsolePresentation.Exceptions;
using Core.Services;
using Core.Services.DemandServices;

namespace ConsolePresentation.Entities.CommandChainLinks;

public class DemandChainLink : CommandChainLinkBase
{
    private readonly IDemandService _demandService;
    private readonly IExtendedCommandParser _extendedCommandParser;

    public DemandChainLink(IDemandService demandService, IExtendedCommandParser extendedCommandParser)
    {
        _demandService = demandService ?? throw new ArgumentNullException(nameof(demandService));
        _extendedCommandParser = extendedCommandParser ?? throw new ArgumentNullException(nameof(extendedCommandParser));
    }

    public override ICommand Handle(string[] args)
    {
        string inputCommand = _extendedCommandParser.ParseCommand(args);
        long productId = _extendedCommandParser.ParseProductId(args);
        int daysAmount = _extendedCommandParser.ParseDaysAmount(args); 
        
        ArgumentException.ThrowIfNullOrEmpty(inputCommand);
        
        if (inputCommand.Equals("demand", StringComparison.CurrentCultureIgnoreCase))
        {
            return new DemandCommand(_demandService, productId, daysAmount);
        }
        
        if (Next is null)
        {
            throw new InvalidCommandException($"{inputCommand} not found");
        }
        
        return Next.Handle(args);
    }
}