namespace HomeworkApp.Exceptions;

public class RateLimitExceededException(string message) : OverflowException(message);