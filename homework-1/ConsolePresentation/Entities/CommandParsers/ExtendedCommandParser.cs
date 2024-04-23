using ConsolePresentation.Exceptions;

namespace ConsolePresentation.Entities.CommandParsers;

public class ExtendedCommandParser : MinimalCommandParser, IExtendedCommandParser
{
    public int ParseDaysAmount(string[] args)
    {
        if (args.Length < 3)
        {
            throw new InvalidCommandException("DaysAmount is required param");
        }

        return int.Parse(args[2]);
    }
}