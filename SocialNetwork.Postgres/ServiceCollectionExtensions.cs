using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace SocialNetwork.Postgres;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConnectionStringBuilder(this IServiceCollection services) =>
        services.AddSingleton<DbConnectionStringBuilder, NpgsqlConnectionStringBuilder>(provider =>
        {
            var options = provider.GetRequiredService<IConfiguration>()
                .GetSection(nameof(PostgresConnectionOptions))
                .Get<PostgresConnectionOptions>();
            return new NpgsqlConnectionStringBuilder
            {
                Host = options.Host,
                Port = options.Port,
                Username = options.Username,
                Password = options.Password,
                Database = options.Database
            };
        });
}