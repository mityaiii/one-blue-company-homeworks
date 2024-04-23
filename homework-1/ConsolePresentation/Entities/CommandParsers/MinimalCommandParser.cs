using ConsolePresentation.Exceptions;

namespace ConsolePresentation.Entities.CommandParsers;

public class MinimalCommandParser : IMinimalCommandParser
{
    public string ParseCommand(string[] args)
    {
        if (args.Length == 0)
        {
            throw new InvalidCommandException("Enter command");
        }

        return args[0];
    }

    public long ParseProductId(string[] args)
    {
        if (args.Length == 1)
        {
            throw new InvalidCommandException("ProductId is required param");
        }

        return long.Parse(args[1]);
    }
}