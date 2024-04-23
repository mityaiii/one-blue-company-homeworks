namespace ConsolePresentation.Entities.CommandParsers;

public interface IExtendedCommandParser : IMinimalCommandParser
{
    public int ParseDaysAmount(string[] args);
}