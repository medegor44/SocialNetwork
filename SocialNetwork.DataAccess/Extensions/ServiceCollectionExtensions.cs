using System.Data.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using SocialNetwork.DataAccess.Repositories;
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

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        return services;
    }
}