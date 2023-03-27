using System.Data.Common;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Migrations.Migrations;

namespace SocialNetwork.Migrations;

public static class Migrator
{
    public static IServiceCollection AddMigration(this IServiceCollection collection, IConfiguration configuration)
    {
        return collection
            .AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .WithGlobalCommandTimeout(TimeSpan.MaxValue)
                .WithGlobalConnectionString(provider => provider.GetRequiredService<DbConnectionStringBuilder>().ToString())
                .AddPostgres()
                .ScanIn(typeof(CreateUsers).Assembly).For.Migrations())
            .AddLogging(x => x.AddFluentMigratorConsole());
    }

    public static void UpdateDatabase(this IServiceProvider serviceProvider) =>
        serviceProvider
            .GetRequiredService<IMigrationRunner>()
            .MigrateUp();
}