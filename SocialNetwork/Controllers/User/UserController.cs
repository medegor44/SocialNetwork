using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Controllers.Common;
using SocialNetwork.Controllers.Requests;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Commands.CreateUserCommand;
using SocialNetwork.Services.Queries.GetUserByFilterQuery;
using SocialNetwork.Services.Queries.GetUserByIdQuery;

namespace SocialNetwork.Controllers.User;

[ApiController]
[Route("[controller]")]
public class UserController : Controller
{
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterSuccessfulResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IResult> Register(
        RegisterRequestDto requestDto, 
        [FromServices] IRequestHandler<CreateUserCommand, CreateUserCommandResponse> handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new(
            requestDto.FirstName, 
            requestDto.SecondName, 
            requestDto.Age, 
            requestDto.Biography, 
            requestDto.City,
            requestDto.Password), cancellationToken);

        return Results.Ok(new RegisterSuccessfulResponseDto(response.Id));
    }

    [HttpGet("get/{id:guid}")]
    [ProducesResponseType(typeof(GetUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IResult> GetById(
        [FromRoute] Guid id, 
        [FromServices] IRequestHandler<GetUserByIdQuery, GetUserByIdQueryResponse> handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new(id), cancellationToken);

        return Results.Ok(new GetUserResponseDto(
            response.UserDto.Id,
            response.UserDto.FirstName,
            response.UserDto.SecondName,
            response.UserDto.Age,
            response.UserDto.Biography,
            response.UserDto.City
        ));
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(List<GetUserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IResult> Search(
        [FromQuery] string firstName, 
        [FromQuery] string secondName, 
        [FromServices] IRequestHandler<GetUserByFilterQuery, GetUserByFilterQueryResponse> handler,
        CancellationToken cancellationToken)
    {
        var response = await handler.HandleAsync(new(firstName, secondName), cancellationToken);

        return Results.Ok(response.Users.Select(x => new GetUserResponseDto(
                x.Id,
                x.FirstName,
                x.SecondName,
                x.Age,
                x.Biography,
                x.City))
            .ToList());
    }
}