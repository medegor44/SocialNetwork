using System.Data.Common;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SocialNetwork.Migrations.Migrations;
using SocialNetwork.Postgres;

namespace SocialNetwork.Migrations;

public static class Migrator
{
    public static IServiceCollection AddMigration(this IServiceCollection collection, IConfiguration configuration)
    {
        return collection
            .AddFluentMigratorCore()
            .ConfigureRunner(builder => builder
                .WithGlobalConnectionString("Database=social-network;Username=social-network-service;Password=postgres;Host=localhost;Port=5432")
                .AddPostgres()
                .ScanIn(typeof(CreateUsers).Assembly).For.Migrations())
            .AddLogging(x => x.AddFluentMigratorConsole());
    }

    public static void UpdateDatabase(this IServiceProvider serviceProvider) =>
        serviceProvider
            .GetRequiredService<IMigrationRunner>()
            .MigrateUp();
}