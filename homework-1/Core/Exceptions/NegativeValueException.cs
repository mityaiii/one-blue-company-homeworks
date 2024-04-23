namespace Core.Exceptions;

public class NegativeValueException : ArgumentException
{
    public NegativeValueException(string message)
        : base(message)
    {
    }
}