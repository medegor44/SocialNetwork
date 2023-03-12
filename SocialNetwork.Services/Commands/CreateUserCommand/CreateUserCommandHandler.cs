using SocialNetwork.Domain.Users;
using SocialNetwork.Domain.Users.Repositories;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.CommonServices;

namespace SocialNetwork.Services.Commands.CreateUserCommand;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHashingService _passwordHashingService;

    public CreateUserCommandHandler(IUserRepository userRepository, IPasswordHashingService passwordHashingService)
    {
        _userRepository = userRepository;
        _passwordHashingService = passwordHashingService;
    }

    public async Task<CreateUserCommandResponse> HandleAsync(CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var password = _passwordHashingService.Hash(request.Password);
        var user = new User(
            new(request.FirstName), 
            new(request.SecondName), 
            new(request.Age), 
            new(request.Biography), 
            Guid.Empty, 
            password);

        var id = await _userRepository.CreateAsync(user, cancellationToken);

        return new(id);
    }
}