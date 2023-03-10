using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Commands.CreateUserCommand;

namespace SocialNetwork.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<CreateUserCommand, CreateUserCommandResponse>, CreateUserCommandHandler>();
        
        return services;
    }
}