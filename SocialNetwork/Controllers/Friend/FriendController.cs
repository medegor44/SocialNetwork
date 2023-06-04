using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Controllers.Auth;
using SocialNetwork.Controllers.Common;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Commands.CreateFriendsCommand;
using SocialNetwork.Services.Commands.RemoveFriendsCommand;

namespace SocialNetwork.Controllers.Friend;

[ApiController]
public class FriendController : Controller
{
    private readonly ILogger<FriendController> _logger;

    public FriendController(ILogger<FriendController> logger)
    {
        _logger = logger;
    }
    
    [HttpPut("set/{friendId:long}")]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status400BadRequest)]
    [Authorize]
    public async Task<IResult> AddFriendAsync(
        [FromRoute] long friendId, 
        [FromServices] IClaimsStore store,
        [FromServices] IRequestHandler<CreateFriendsCommand> handler,
        CancellationToken cancellationToken)
    {
        var userId = store.FromClaims(HttpContext.User.Claims).GetUserId();

        await handler.HandleAsync(new(userId, new List<long>()
        {
            friendId
        }), cancellationToken);

        return Results.Ok();
    }
    
    [HttpPut("delete/{friendId:long}")]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status400BadRequest)]
    [Authorize]
    public async Task<IResult> RemoveFriend(
        [FromRoute] long friendId, 
        [FromServices] IClaimsStore store,
        [FromServices] IRequestHandler<RemoveFriendsCommand> handler,
        CancellationToken cancellationToken)
    {
        var userId = store.FromClaims(HttpContext.User.Claims).GetUserId();
        
        await handler.HandleAsync(new(userId, new List<long>()
        {
            friendId
        }), cancellationToken);
        
        return Results.Ok();
    }
}