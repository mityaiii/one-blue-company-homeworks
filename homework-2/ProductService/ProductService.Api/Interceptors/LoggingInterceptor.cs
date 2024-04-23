using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ProductService.Api.Interceptors;

public class LoggingInterceptor : Interceptor
{
    private ILogger<LoggingInterceptor> _logger;

    public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
    {
        _logger = logger;
    }
    
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        _logger.LogInformation($"Start grpc method with request:{request}");

        try
        {
            TResponse response = await continuation(request, context);
            _logger.LogInformation($"End grpc method with response:{response}");
            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error");
            throw;
        }
    }
}