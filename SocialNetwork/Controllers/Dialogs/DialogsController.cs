using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Controllers.Auth;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Commands.SendMessageCommand;
using SocialNetwork.Services.Queries.ListDialogQuery;

namespace SocialNetwork.Controllers.Dialogs;

[ApiController]
public class DialogsController : Controller
{
    [HttpPost("{userId:long}/send")]
    public async Task<IResult> SendAsync(
        long userId, 
        [FromBody] SendMessageDto message, [FromServices] IRequestHandler<SendMessageCommand> handler,
        CancellationToken cancellationToken)
    {
        var sender = new ClaimsStore().FromClaims(HttpContext.User.Claims).GetUserId();
        
        await handler.HandleAsync(new(sender, userId, message.Text), cancellationToken);

        return Results.Ok();
    }

    [HttpGet("{userId:long}/list")]
    public async Task<List<GetMessageDto>> ListAsync(
        long userId,
        [FromServices] IRequestHandler<ListDialogQuery, ListDialogQueryResponse> handler,
        CancellationToken cancellationToken)
    {
        var sender = new ClaimsStore().FromClaims(HttpContext.User.Claims).GetUserId();
        var response = await handler.HandleAsync(new(sender, userId), cancellationToken);

        return response.Mmessages.Select(m => new GetMessageDto()
        {
            From = m.From,
            To = m.To,
            Text = m.Text
        }).ToList();
    }
}