using System.Text.Json;
using SocialNetwork.Controllers.Common;

namespace SocialNetwork.Middleware;

public class BadStatusCodesHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public BadStatusCodesHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var memoryStream = new MemoryStream();

        var responseBody = context.Response.Body;
        context.Response.Body = memoryStream;
        
        await _next.Invoke(context);
        
        memoryStream.Position = 0;

        if (context.Response.StatusCode >= StatusCodes.Status400BadRequest)
        {
            ActionFailedResponse response;
            using (var reader = new StreamReader(memoryStream, leaveOpen:true))
            {
                response = new ActionFailedResponse()
                {
                    Message = await reader.ReadToEndAsync(),
                    RequestId = context.TraceIdentifier,
                    Status = context.Response.StatusCode
                };
            }

            memoryStream.Position = 0;
            
            using (var writer = new StreamWriter(memoryStream, leaveOpen:true))
            {
                await writer.WriteAsync(JsonSerializer.Serialize(response));
            }

            memoryStream.Position = 0;
        }

        await memoryStream.CopyToAsync(responseBody);
        context.Response.Body = responseBody;
    }
}