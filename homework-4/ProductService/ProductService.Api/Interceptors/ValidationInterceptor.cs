using FluentValidation;
using Grpc.Core;
using Grpc.Core.Interceptors;

namespace ProductService.Api.Interceptors;

public class ValidationInterceptor : Interceptor
{
    private readonly IServiceProvider _serviceProvider;

    public ValidationInterceptor(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public override Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request, ServerCallContext context,
        UnaryServerMethod<TRequest, TResponse> continuation)
    {
        var validator = _serviceProvider.GetService<IValidator<TRequest>>();
        var validationResult = validator?.Validate(request);
        if (validationResult is not null && !validationResult.IsValid)
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    $"{string.Join(';', validationResult.Errors.Select(e => e.ErrorMessage))}"
                ));
        }

        return continuation(request, context);
    }
}