namespace SocialNetwork.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            _logger.LogError(e.StackTrace);
            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.Headers.RetryAfter = 120.ToString();

            context.Response.Body.Position = 0;
            await using var writer = new StreamWriter(context.Response.Body, leaveOpen:true);
            await writer.WriteAsync("Internal error");
        }
    }
}