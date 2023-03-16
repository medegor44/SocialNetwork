using SocialNetwork.Domain.Dictionaries;
using SocialNetwork.Domain.Users;
using SocialNetwork.Domain.Users.Entities;
using SocialNetwork.Domain.Users.Repositories;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.CommonServices;
using SocialNetwork.Services.Exceptions;

namespace SocialNetwork.Services.Commands.CreateUserCommand;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, CreateUserCommandResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ICitiesRepository _citiesRepository;
    private readonly IPasswordHashingService _passwordHashingService;

    public CreateUserCommandHandler(
        IUserRepository userRepository, 
        ICitiesRepository citiesRepository,
        IPasswordHashingService passwordHashingService)
    {
        _userRepository = userRepository;
        _citiesRepository = citiesRepository;
        _passwordHashingService = passwordHashingService;
    }

    public async Task<CreateUserCommandResponse> HandleAsync(CreateUserCommand request,
        CancellationToken cancellationToken)
    {
        var cities = await _citiesRepository.GetByName(request.City, cancellationToken);
        if (!cities.Any())
            throw new BadRequestException($"City with name {request.City} does not exists");
        
        var password = _passwordHashingService.Hash(request.Password);

        var city = cities.First();
        var user = new User(
            new(request.FirstName), 
            new(request.SecondName), 
            new(request.Age), 
            new(request.Biography), 
            new(city.Id, city.Name), 
            password);

        var id = await _userRepository.CreateAsync(user, cancellationToken);

        return new(id);
    }
}