using StackExchange.Redis;

namespace SocialNetwork.DataAccess.Redis;

public class RedisProvider : IRedisProvider
{
    private readonly string _endpoint;

    public RedisProvider(string endpoint) =>
        _endpoint = endpoint;
    
    public async Task<IConnectionMultiplexer> CreateConnectionAsync() =>
        await ConnectionMultiplexer.ConnectAsync(_endpoint);
}