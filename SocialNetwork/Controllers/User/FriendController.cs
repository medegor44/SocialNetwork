using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Controllers.Common;

namespace SocialNetwork.Controllers;

[ApiController]
[Route("[controller]")]
public class FriendController : Controller
{
    [HttpPut("set/{user_id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    public Task<IActionResult> SetFriend([FromRoute] Guid userId)
    {
        throw new NotImplementedException();
    }
    
    [HttpPut("delete/{user_id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    public Task<IActionResult> DeleteFriend([FromRoute] Guid userId)
    {
        throw new NotImplementedException();
    }
}