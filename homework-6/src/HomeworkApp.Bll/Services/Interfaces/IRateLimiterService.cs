namespace HomeworkApp.Bll.Services.Interfaces;

public interface IRateLimiterService
{
    public Task<bool> IsAllowed(string clientId, CancellationToken token);
}