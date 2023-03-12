using SocialNetwork.Domain.Users.Repositories;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.CommonServices;

namespace SocialNetwork.Services.Queries.AuthenticationQuery;

public class AuthenticateQueryHandler : IRequestHandler<AuthenticateQuery, AuthenticateQueryResponse>
{
    private readonly IUserRepository _repository;
    private readonly IPasswordHashingService _passwordHashingService;

    public AuthenticateQueryHandler(IUserRepository repository, IPasswordHashingService passwordHashingService)
    {
        _repository = repository;
        _passwordHashingService = passwordHashingService;
    }
    
    public async Task<AuthenticateQueryResponse> HandleAsync(AuthenticateQuery request, CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id, cancellationToken);

        return new(_passwordHashingService.Check(request.RawPassword, user.Password));
    }
}