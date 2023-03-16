namespace SocialNetwork.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next.Invoke(context);
        }
        catch (Exception)
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.Headers.RetryAfter = 120.ToString();

            context.Response.Body.Position = 0;
            await using var writer = new StreamWriter(context.Response.Body, leaveOpen:true);
            await writer.WriteAsync("Internal error");
        }
    }
}