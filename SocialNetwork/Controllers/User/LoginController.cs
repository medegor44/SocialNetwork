using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Controllers.Requests;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Queries.AuthenticationQuery;

namespace SocialNetwork.Controllers.User;

[ApiController]
[Route("[controller]")]
public class LoginController : Controller
{
    [HttpPost]
    public async Task<IResult> Post(
        LoginRequest request, 
        [FromServices] IRequestHandler<AuthenticateQuery, AuthenticateQueryResponse> handler, 
        CancellationToken cancellationToken)
    {
        var authResponse = await handler.HandleAsync(new(Guid.Parse(request.Id), request.Password), cancellationToken);
        if (!authResponse.Succeeded)
            return Results.Unauthorized();

        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, request.Id)
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