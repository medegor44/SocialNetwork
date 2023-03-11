using SocialNetwork.Domain.Users;
using SocialNetwork.Domain.Users.Repositories;
using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.CreateUserCommand;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResponse>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<CreateUserCommandResponse> HandleAsync(CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user = new User(
            new(request.FirstName), 
            new(request.SecondName), 
            new(request.Age), 
            new(request.Biography),
            Guid.Empty);

        var id = await _userRepository.CreateAsync(user, new(request.Password), cancellationToken);

        return new(id);
    }
}