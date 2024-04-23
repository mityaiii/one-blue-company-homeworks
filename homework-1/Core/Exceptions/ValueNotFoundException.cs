namespace Core.Exceptions;

public class ValueNotFoundException : ArgumentException
{
    public ValueNotFoundException(string message)
        : base(message)
    { }
}