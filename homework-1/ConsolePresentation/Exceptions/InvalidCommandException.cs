namespace ConsolePresentation.Exceptions;

public class InvalidCommandException : ArgumentException
{
    public InvalidCommandException(string message)
        : base(message)
    { }
}