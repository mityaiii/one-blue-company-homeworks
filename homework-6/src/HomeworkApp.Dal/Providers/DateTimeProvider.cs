using HomeworkApp.Dal.Providers.Interfaces;

namespace HomeworkApp.Dal.Providers;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset Now()
    {
        return DateTimeOffset.Now.UtcDateTime;
    }
}