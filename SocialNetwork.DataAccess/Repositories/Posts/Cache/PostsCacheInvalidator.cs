using SocialNetwork.DataAccess.Redis;
using StackExchange.Redis;

namespace SocialNetwork.DataAccess.Repositories.Posts.Cache;

public class PostsCacheInvalidator : IPostsCacheInvalidator
{
    private readonly IRedisProvider _provider;

    public PostsCacheInvalidator(IRedisProvider provider)
    {
        _provider = provider;
    }
    
    public async Task InvalidateAsync(IReadOnlyCollection<long> userIds)
    {
        var userKeys = userIds.Select(CacheKeys.User);
        var hashKeys = userIds.Select(CacheKeys.Feed);
        var listKeys = userIds.Select(CacheKeys.FeedList);

        await using var connection = await _provider.CreateConnectionAsync();
        var cache = connection.GetDatabase();

        await cache.KeyDeleteAsync(
            hashKeys
                .Concat(listKeys)
                .Concat(userKeys)
                .Select(x => new RedisKey(x))
                .ToArray()
            );
    }
}