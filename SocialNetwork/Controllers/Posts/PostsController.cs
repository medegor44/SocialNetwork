using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Controllers.Auth;
using SocialNetwork.Controllers.Common;
using SocialNetwork.Controllers.Dto;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Commands.CreatePostCommand;
using SocialNetwork.Services.Commands.DeletePostCommand;
using SocialNetwork.Services.Commands.UpdatePostCommand;
using SocialNetwork.Services.Queries;
using SocialNetwork.Services.Queries.GetPostsFeed;

namespace SocialNetwork.Controllers.Posts;

[ApiController]
[Route("[controller]")]
public class PostsController : Controller
{
    [Authorize]
    [HttpPost("create")]
    [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IResult> CreateAsync(
        [FromBody] CreateRequest request,
        [FromServices] IRequestHandler<CreatePostCommand, CreatePostCommandResponse> handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new(request.Text), cancellationToken);
        
        return Results.Ok(response.Id);
    }

    [Authorize]
    [HttpPut("update")]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IResult> UpdateAsync(
        [FromBody] UpdateRequest request, 
        [FromServices] IRequestHandler<UpdatePostCommand> handler,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new(request.Id, request.Text), cancellationToken);
        
        return Results.Ok();
    }

    [HttpPut("delete")]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IResult> DeleteAsync(
        [FromBody] DeleteRequest request, 
        [FromServices] IRequestHandler<DeletePostCommand> handler,
        CancellationToken cancellationToken)
    {
        await handler.HandleAsync(new(request.Id), cancellationToken);
        
        return Results.Ok();
    }

    [Authorize]
    [HttpGet("get/{id:long}")]
    [ProducesResponseType(typeof(GetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IResult> GetAsync(
        [FromRoute] long id,
        [FromServices] IRequestHandler<GetPostsByIdQuery, GetPostsByIdQueryResponse> handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new(id), cancellationToken);

        return Results.Ok(new GetResponse(
            response.Post.Id,
            response.Post.Text,
            response.Post.AuthorUserId
        ));
    }

    [Authorize]
    [HttpGet("feed")]
    [ProducesResponseType(typeof(List<GetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status401Unauthorized)]
    public async Task<IResult> Feed(
        [FromQuery] int offset, 
        [FromQuery] int limit,
        [FromServices] IRequestHandler<GetPostsFeedQuery, GetPostsFeedQueryResponse> handler,
        [FromServices] IClaimsStore store,
        CancellationToken cancellationToken)
    {
        var userId = store.FromClaims(HttpContext.User.Claims).GetUserId();

        var response = await handler.HandleAsync(new(userId, offset, limit), cancellationToken);
        
        return Results.Ok(response
            .Posts
            .Select(x => 
                new GetResponse(x.Id, x.Text, x.AuthorUserId))
            .ToList());
    }
}