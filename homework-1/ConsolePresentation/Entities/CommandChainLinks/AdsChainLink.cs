using ConsolePresentation.Entities.CommandParsers;
using ConsolePresentation.Entities.Commands;
using ConsolePresentation.Exceptions;
using Core.Services;

namespace ConsolePresentation.Entities.CommandChainLinks;

public class AdsChainLink : CommandChainLinkBase
{
    private readonly IAdsService _adsService;
    private readonly IMinimalCommandParser _minimalCommandParser;

    public AdsChainLink(IAdsService adsService, IMinimalCommandParser minimalCommandParser)
    {
        _adsService = adsService ?? throw new ArgumentNullException(nameof(adsService));
        _minimalCommandParser = minimalCommandParser ?? throw new ArgumentNullException(nameof(minimalCommandParser));
    }

    public override ICommand Handle(string[] args)
    {
        string inputCommand = _minimalCommandParser.ParseCommand(args);
        long productId = _minimalCommandParser.ParseProductId(args);
        
        ArgumentException.ThrowIfNullOrEmpty(inputCommand);

        if (inputCommand.Equals("ads", StringComparison.CurrentCultureIgnoreCase))
        {
            return new AdsCommand(_adsService, productId);
        }
        
        if (Next is null)
        {
            throw new InvalidCommandException($"{inputCommand} not found");
        }
        
        return Next.Handle(args);
    }
}