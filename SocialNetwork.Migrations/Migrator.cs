using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using SocialNetwork.Migrations.Migrations;

namespace SocialNetwork.Migrations;

public static class Migrator
{
    public static IServiceCollection AddMigration(this IServiceCollection collection, IConfiguration configuration)
    {
        return collection
            .AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .WithGlobalConnectionString(provider =>
                {
                    var options = provider.GetRequiredService<IConfiguration>()
                        .GetSection(nameof(PostgresConnectionOptions))
                        .Get<PostgresConnectionOptions>();
                    
                    return new NpgsqlConnectionStringBuilder()
                    {
                        Host = options.Host,
                        Port = options.Port,
                        Database = options.Database,
                        Username = options.Username,
                        Password = options.Password
                    }.ToString();
                })
                .AddPostgres()
                .ScanIn(typeof(CreateUsers).Assembly).For.Migrations())
            .AddLogging(x => x.AddFluentMigratorConsole());
    }

    public static void UpdateDatabase(this IServiceProvider serviceProvider) =>
        serviceProvider
            .GetRequiredService<IMigrationRunner>()
            .MigrateUp();
}