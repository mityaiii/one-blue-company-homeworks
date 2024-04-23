using HomeworkApp.Bll.Providers.Interfaces;
using HomeworkApp.Bll.Services;
using HomeworkApp.Bll.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Moq;

namespace HomeworkApp.UnitTests;

public class RateLimiterTests
{
    private readonly IRateLimiterService _rateLimiterService;
    private readonly Mock<IDistributedCacheProvider> _distributedCache = new();
    public RateLimiterTests()
    {
        _rateLimiterService = new RateLimiterService(_distributedCache.Object);
    }
    
    [Fact]
    public async Task SendRequest_ToRateLimiterServiceLessThanLimit_Success()
    {
        // Arrange
        const string clientId = "client"; 
        
        _distributedCache
            .Setup(f => f.GetStringAsync(It.IsAny<string>(), default))
            .ReturnsAsync("99");

        _distributedCache
            .Setup(f => f.SetStringAsync(clientId, "100", default));

        // Act
        bool isAllowed = await _rateLimiterService.IsAllowed(clientId, default);
        
        // Assert
        Assert.True(isAllowed);
    }
    
    [Fact]
    public async Task SendRequest_ToRateLimiterServiceGreaterThanLimit_ShouldThrowRateLimitExceededException()
    {
        // Arrange
        const string clientId = "client"; 
        
        _distributedCache
            .Setup(f => f.GetStringAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("100");

        // Act
        bool isAllowed = await _rateLimiterService.IsAllowed(clientId, default);
        
        // Assert
        Assert.False(isAllowed);
    }
}