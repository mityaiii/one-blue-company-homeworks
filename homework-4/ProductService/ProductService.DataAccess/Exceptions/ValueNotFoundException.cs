namespace ProductService.DataAccess.Exceptions;

public class ValueNotFoundException(string message) : ArgumentException(message);