using Microsoft.Extensions.Logging;
using Npgsql;

namespace SocialNetwork.Postgres;

public class ConnectionFactory : IConnectionFactory
{
    private readonly ILoggerFactory _factory;

    public ConnectionFactory(ILoggerFactory factory)
    {
        _factory = factory;

    }
    
    private NpgsqlDataSource GetDataSource(NpgsqlConnectionStringBuilder connectionStringBuilder) => new NpgsqlDataSourceBuilder(connectionStringBuilder.ToString())
        .UseLoggerFactory(_factory)
        .EnableParameterLogging()
        .Build();
    
    public NpgsqlConnection GetMaster()
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Database= "social-network",
            Username= "social-network-service",
            Password= "postgres",
            Host= "localhost",
            Port = 5432,
            CommandTimeout = 300

        };

        return GetDataSource(connectionStringBuilder).CreateConnection();
    }

    public NpgsqlConnection GetSync()
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Database= "social-network",
            Username= "social-network-service",
            Password= "postgres",
            Host= "localhost",
            Port = 15432,
            CommandTimeout = 300
        };

        return GetDataSource(connectionStringBuilder).CreateConnection();
    }

    public NpgsqlConnection GetAsync()
    {
        var connectionStringBuilder = new NpgsqlConnectionStringBuilder
        {
            Database= "social-network",
            Username= "social-network-service",
            Password= "postgres",
            Host= "localhost",
            Port = 25432,
            CommandTimeout = 300
        };

        return GetDataSource(connectionStringBuilder).CreateConnection();
    }
}