namespace SocialNetwork.Middleware;

public class CorrelationIdGeneratorMiddleware
{
    private readonly RequestDelegate _next;

    public CorrelationIdGeneratorMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());
        await _next.Invoke(context);
    }
}