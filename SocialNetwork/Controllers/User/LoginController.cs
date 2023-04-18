using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Controllers.Common;
using SocialNetwork.Controllers.Requests;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Exceptions;
using SocialNetwork.Services.Queries.AuthenticationQuery;

namespace SocialNetwork.Controllers.User;

[ApiController]
[Route("[controller]")]
public class LoginController : Controller
{
    [HttpPost]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ActionFailedResponse), StatusCodes.Status500InternalServerError)]
    public async Task<IResult> Post(
        LoginRequest request, 
        [FromServices] IRequestHandler<AuthenticateQuery, AuthenticateQueryResponse> handler, 
        CancellationToken cancellationToken)
    {
        AuthenticateQueryResponse authResponse;
        try
        {
            authResponse = await handler.HandleAsync(
                new(request.Id, request.Password),
                cancellationToken);
        }
        catch (NotFoundException e)
        {
            return Results.NotFound(e.Message);
        }
        catch (FormatException e)
        {
            return Results.BadRequest(e.Message);
        }

        if (!authResponse.Succeeded)
            return Results.BadRequest("Invalid id or password");

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, request.Id.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: AuthOptions.Issuer,
            audience: AuthOptions.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256));

        return Results.Ok(new LoginResponse(new JwtSecurityTokenHandler().WriteToken(token)));
    }
}