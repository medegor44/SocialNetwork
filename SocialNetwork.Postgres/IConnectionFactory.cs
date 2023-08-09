using Npgsql;

namespace SocialNetwork.Postgres;

public interface IConnectionFactory
{
    NpgsqlConnection GetMaster();
    NpgsqlConnection GetSync();
    NpgsqlConnection GetAsync();
}