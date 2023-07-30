using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.AspNetCore.Http;

namespace SocialNetwork.DataAccess.Grpc;

public class CorrelationIdInterceptor : Interceptor
{
    private readonly IHttpContextAccessor _accessor;

    public CorrelationIdInterceptor(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context,
        AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        if (!(_accessor.HttpContext?.Request.Headers.TryGetValue("X-Correlation-Id", out var correlationId) ?? false))
            correlationId = Guid.NewGuid().ToString();
        
        context.Options.Headers?.Add("X-Correlation-Id", correlationId);

        var call = continuation.Invoke(request, context);

        return new AsyncUnaryCall<TResponse>(
            call.ResponseAsync,
            HandleHeaders(call.ResponseHeadersAsync, correlationId),
            call.GetStatus,
            call.GetTrailers,
            call.Dispose
        );
    }

    private async Task<Metadata> HandleHeaders(Task<Metadata> headers, string correlationId)
    {
        var metadata = await headers;
        metadata.Add("X-Correlation-Id", correlationId);
        
        _accessor.HttpContext?.Response.Headers.TryAdd("X-Correlation-Id", correlationId);

        return metadata;
    }
}