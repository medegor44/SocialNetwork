using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.CreateUserCommand;

public record CreateUserCommand(
    string FirstName, 
    string SecondName, 
    int Age, 
    string Biography, 
    string City, 
    string Password) : IRequest<CreateUserCommandResponse>;