namespace ProductService.DataAccess.Exceptions;

public class NoValueWasReceived : ArgumentException
{
    public NoValueWasReceived()
    { }
    public NoValueWasReceived(string message)
        : base(message)
    { }
}