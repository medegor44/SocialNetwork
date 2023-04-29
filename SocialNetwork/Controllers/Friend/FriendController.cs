using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Controllers.Auth;
using SocialNetwork.Controllers.Common;

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
    public Task<IResult> AddFriend([FromRoute] long friendId, [FromServices] IClaimsStore store)
    {
        var userId = store.FromClaims(HttpContext.User.Claims).GetUserId();
        
        _logger.LogInformation($"{userId} is friend to {friendId}");
        
        return Task.FromResult(Results.Ok());
    }
    
    [HttpPut("delete/{friendId:long}")]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status400BadRequest)]
    [Authorize]
    public Task<IResult> RemoveFriend([FromRoute] long friendId, [FromServices] IClaimsStore store)
    {
        var userId = store.FromClaims(HttpContext.User.Claims).GetUserId();
        
        _logger.LogInformation($"{userId} now is not friend to {friendId}");
        
        return Task.FromResult(Results.Ok());
    }
}