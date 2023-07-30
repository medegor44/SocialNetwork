using System.Data.Common;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Grpc.Net.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using SocialNetwork.DataAccess.Grpc;
using SocialNetwork.DataAccess.Redis;
using SocialNetwork.DataAccess.Repositories;
using SocialNetwork.DataAccess.Repositories.Posts.Cache;
using SocialNetwork.DataAccess.Repositories.Posts.Redis;
using SocialNetwork.Domain.Dialogs;
using SocialNetwork.Domain.Dictionaries;
using SocialNetwork.Domain.Friends.Repositories;
using SocialNetwork.Domain.Messages;
using SocialNetwork.Domain.Posts.Repositories;
using SocialNetwork.Domain.Users.Repositories;

namespace SocialNetwork.DataAccess.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgres(this IServiceCollection services) =>
        services.AddScoped<NpgsqlDataSource>(provider =>
            new NpgsqlDataSourceBuilder(provider.GetRequiredService<DbConnectionStringBuilder>().ToString())
                .UseLoggerFactory(provider.GetRequiredService<ILoggerFactory>())
                .EnableParameterLogging()
                .Build());

    public static IServiceCollection AddCache(this IServiceCollection services, IConfiguration configuration)
    {
        return services.AddTransient<IRedisProvider, RedisProvider>(p =>
            new(configuration
                    .GetSection("RedisEndPoint")
                    .Value ??
                throw new InvalidOperationException("Endpoint should be specified")));
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        var host = configuration
                       .GetSection("MessagingServiceOptions")
                       .Value ??
                   throw new InvalidOperationException("Host of messaging service should be specified");

        services.AddGrpc();
        services.AddSingleton<CorrelationIdInterceptor>();
        services.AddSingleton<ChannelBase>(p =>
        {
            var grpcChannel = GrpcChannel.ForAddress(host);
            grpcChannel.Intercept(p.GetRequiredService<CorrelationIdInterceptor>());

            return grpcChannel;
        });
        
        services.AddScoped<MessagingService.Proto.MessagingService.MessagingServiceClient>();
        
        return services
            .AddScoped<IUserRepository, UserRepository>()
            .AddScoped<ICitiesRepository, CitiesRepository>()
            .AddScoped<IFriendsRepository, FriendsRepository>()
            .AddScoped<IPostsCacheInvalidator>(_ => null!)
            .AddScoped<IPostsRepository, PostsRepository>()
            .AddScoped<IMessageRepository, MessagesRepository>()
            .AddScoped<IDialogsRepository, DialogsRepository>();
    }
}