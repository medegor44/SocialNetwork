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

    public async Task<CreateUserCommandResponse> HandleAsync(CreateUserCommand request)
    {
        var user = new User(
            new(request.FirstName), 
            new(request.SecondName), 
            new(request.Age), 
            new(request.Biography),
            request.City);

        var id = await _userRepository.CreateAsync(user);

        return new(id);
    }
}