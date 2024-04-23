using Grpc.Core;
using Grpc.Core.Interceptors;
using HomeworkApp.Bll.Services.Interfaces;

namespace HomeworkApp.Interceptors;

public class RateLimiterInterceptor : Interceptor
{
    private readonly IRateLimiterService _rateLimiterService;
    public RateLimiterInterceptor(IRateLimiterService rateLimiterService)
    {
        _rateLimiterService = rateLimiterService;
    }
    
    public override async  Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var headers = context.RequestHeaders;
        var clientIp = headers
            .FirstOrDefault(x => x.Key.Equals("X-R256-USER-IP", StringComparison.CurrentCultureIgnoreCase))?
            .Value;
        
        if (!await _rateLimiterService.IsAllowed(clientIp, context.CancellationToken))
        {
            throw new RpcException(new Status(StatusCode.ResourceExhausted, "Rate limit exceeded"));
        }

        return await continuation(request, context);
    }
}