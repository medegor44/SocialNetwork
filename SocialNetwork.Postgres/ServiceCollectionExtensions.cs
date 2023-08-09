using System.Data.Common;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace SocialNetwork.Postgres;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConnectionStringBuilder(this IServiceCollection services) =>
        services
            .AddScoped<IConnectionFactory, ConnectionFactory>();
}