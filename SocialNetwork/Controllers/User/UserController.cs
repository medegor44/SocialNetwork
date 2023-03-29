using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Controllers.Common;
using SocialNetwork.Controllers.Requests;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Commands.CreateUserCommand;
using SocialNetwork.Services.Exceptions;
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
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status400BadRequest)]
    public async Task<IResult> Register(
        RegisterRequestDto requestDto, 
        [FromServices] IRequestHandler<CreateUserCommand, CreateUserCommandResponse> handler,
        CancellationToken cancellationToken)
    {
        CreateUserCommandResponse response;
        try
        {
            response = await handler.HandleAsync(new(
                requestDto.FirstName, 
                requestDto.SecondName, 
                requestDto.Age, 
                requestDto.Biography, 
                requestDto.City,
                requestDto.Password), cancellationToken);

        }
        catch (BadRequestException e)
        {
            return Results.BadRequest(e.Message);
        }
        
        return Results.Ok(new RegisterSuccessfulResponseDto(response.Id));
    }

    [HttpGet("get/{id:guid}")]
    [ProducesResponseType(typeof(GetUserResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IResult> GetById(
        [FromRoute] Guid id, 
        [FromServices] IRequestHandler<GetUserByIdQuery, GetUserByIdQueryResponse> handler,
        CancellationToken cancellationToken)
    {
        GetUserByIdQueryResponse response;
        try
        {
            response = await handler.HandleAsync(new(id), cancellationToken);
        }
        catch (NotFoundException e)
        {
            return Results.NotFound(e.Message);
        }

        return Results.Ok(new GetUserResponseDto(
            response.UserDto.Id,
            response.UserDto.FirstName,
            response.UserDto.SecondName,
            response.UserDto.Age,
            response.UserDto.Biography,
            response.UserDto.City
        ));
    }

    [HttpGet("get")]
    [ProducesResponseType(typeof(List<GetUserResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<IResult> GetByNames(
        [FromQuery] string? firstName, 
        [FromQuery] string? secondName, 
        CancellationToken cancellationToken,
        [FromServices] IRequestHandler<GetUserByFilterQuery, GetUserByFilterQueryResponse> handler)
    {
        var response = await handler.HandleAsync(new(firstName, secondName), cancellationToken);

        return Results.Ok(response.Users.Select(user => new GetUserResponseDto(
            user.Id,
            user.FirstName,
            user.SecondName,
            user.Age,
            user.Biography,
            user.City
        )).ToList());
    }
}