using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Commands.CreateUserCommand;
using SocialNetwork.Services.CommonServices;
using SocialNetwork.Services.Queries.AuthenticationQuery;

namespace SocialNetwork.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<CreateUserCommand, CreateUserCommandResponse>, CreateUserCommandHandler>();
        services.AddScoped<IRequestHandler<AuthenticateQuery, AuthenticateQueryResponse>, AuthenticateQueryHandler>();
        return services;
    }

    public static IServiceCollection AddPasswordHashingService(this IServiceCollection services) =>
        services.AddScoped<IPasswordHashingService, PasswordHashingService>();
}