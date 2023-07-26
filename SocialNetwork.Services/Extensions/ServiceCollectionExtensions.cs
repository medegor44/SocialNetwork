using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Commands.CreateFriendsCommand;
using SocialNetwork.Services.Commands.CreatePostCommand;
using SocialNetwork.Services.Commands.CreateUserCommand;
using SocialNetwork.Services.Commands.DeletePostCommand;
using SocialNetwork.Services.Commands.RemoveFriendsCommand;
using SocialNetwork.Services.Commands.SendMessageCommand;
using SocialNetwork.Services.Commands.UpdatePostCommand;
using SocialNetwork.Services.CommonServices;
using SocialNetwork.Services.Events;
using SocialNetwork.Services.Queries;
using SocialNetwork.Services.Queries.AuthenticationQuery;
using SocialNetwork.Services.Queries.GetPostsFeed;
using SocialNetwork.Services.Queries.GetUserByFilterQuery;
using SocialNetwork.Services.Queries.GetUserByIdQuery;
using SocialNetwork.Services.Queries.ListDialogQuery;

namespace SocialNetwork.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        services.AddScoped<IPostCreatedNotificationSender>(_ => null!);
        
        services.AddScoped<IRequestHandler<CreateUserCommand, CreateUserCommandResponse>, CreateUserCommandHandler>();
        services.AddScoped<IRequestHandler<AuthenticateQuery, AuthenticateQueryResponse>, AuthenticateQueryHandler>();
        services.AddScoped<IRequestHandler<GetUserByIdQuery, GetUserByIdQueryResponse>, GetUserByIdQueryHandler>();
        services
            .AddScoped<IRequestHandler<GetUserByFilterQuery, GetUserByFilterQueryResponse>,
                GetUserByFilterQueryHandler>()
            .AddScoped<IRequestHandler<CreateFriendsCommand>, CreateFriendsCommandHandler>()
            .AddScoped<IRequestHandler<RemoveFriendsCommand>, RemoveFriendsCommandHandler>();
        
        services
            .AddScoped<IRequestHandler<CreatePostCommand, CreatePostCommandResponse>, CreatePostCommandHandler>()
            .AddScoped<IRequestHandler<UpdatePostCommand>, UpdatePostCommandHandler>()
            .AddScoped<IRequestHandler<DeletePostCommand>, DeletePostCommandHandler>()
            .AddScoped<IRequestHandler<GetPostsFeedQuery, GetPostsFeedQueryResponse>, GetPostsFeedQueryHandler>()
            .AddScoped<IRequestHandler<GetPostsByIdQuery, GetPostsByIdQueryResponse>, GetPostsByIdsQueryHandler>();

        services
            .AddScoped<IRequestHandler<SendMessageCommand>, SendMessageCommandHandler>()
            .AddScoped<IRequestHandler<ListDialogQuery, ListDialogQueryResponse>, ListDialogQueryHandler>();
        
        return services;
    }

    public static IServiceCollection AddPasswordHashingService(this IServiceCollection services) =>
        services.AddScoped<IPasswordHashingService, PasswordHashingService>();
}