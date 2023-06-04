using StackExchange.Redis;

namespace SocialNetwork.DataAccess.Redis;

public interface IRedisProvider
{
    Task<IConnectionMultiplexer> CreateConnectionAsync();
}