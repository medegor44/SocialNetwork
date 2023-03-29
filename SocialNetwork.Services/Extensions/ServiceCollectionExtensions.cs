using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Commands.CreateUserCommand;
using SocialNetwork.Services.CommonServices;
using SocialNetwork.Services.Queries.AuthenticationQuery;
using SocialNetwork.Services.Queries.GetUserByFilterQuery;
using SocialNetwork.Services.Queries.GetUserByIdQuery;

namespace SocialNetwork.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<CreateUserCommand, CreateUserCommandResponse>, CreateUserCommandHandler>();
        services.AddScoped<IRequestHandler<AuthenticateQuery, AuthenticateQueryResponse>, AuthenticateQueryHandler>();
        services.AddScoped<IRequestHandler<GetUserByIdQuery, GetUserByIdQueryResponse>, GetUserByIdQueryHandler>();
        services
            .AddScoped<IRequestHandler<GetUserByFilterQuery, GetUserByFilterQueryResponse>,
                GetUserByFilterQueryHandler>();
        return services;
    }

    public static IServiceCollection AddPasswordHashingService(this IServiceCollection services) =>
        services.AddScoped<IPasswordHashingService, PasswordHashingService>();
}