namespace ConsolePresentation.Entities.CommandParsers;

public interface IMinimalCommandParser
{
    public string ParseCommand(string[] args);
    public long ParseProductId(string[] args);
}