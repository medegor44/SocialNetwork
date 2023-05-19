using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using SocialNetwork.DataAccess.Repositories.Dto;
using SocialNetwork.Domain.Friends.Repositories;
using SocialNetwork.Domain.Posts;
using SocialNetwork.Domain.Posts.Repositories;
using SocialNetwork.Domain.Posts.ValueObjects;
using StackExchange.Redis;

namespace SocialNetwork.DataAccess.Repositories;

public class PostsCacheRepository : IPostsRepository
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IPostsRepository _postsRepository;
    private readonly IFriendsRepository _friendsRepository;

    private class CacheKeys
    {
        public static string Feed(long userId) => $"user:{userId}:feed";
    }

    public PostsCacheRepository(
        IConnectionMultiplexer connectionMultiplexer, 
        IPostsRepository postsRepository,
        IFriendsRepository friendsRepository)
    {
        _connectionMultiplexer = connectionMultiplexer;
        _postsRepository = postsRepository;
        _friendsRepository = friendsRepository;
    }
    
    public Task<Post> CreateAsync(Post post, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Post updatedPost, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<Post>> GetByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<Feed> GetFeedAsync(FeedOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var cache = _connectionMultiplexer.GetDatabase();
            if (await cache.KeyTouchAsync(CacheKeys.Feed(options.FeedRecipientUserId)))
            {
                var feed = (await cache
                        .ListRangeAsync(CacheKeys.Feed(options.FeedRecipientUserId)))
                    .Where(x => x.HasValue)
                    .Select(serializedPost => JsonSerializer.Deserialize<PostCacheDto>(serializedPost!))
                    .Select(x => new Post(x.Id, new(x.Text), x.UserId))
                    .ToList();

                return new(feed
                        .Skip(options.Offset)
                        .Take(options.Limit)
                        .ToList(),
                    feed.Count);
            }
        }
        catch
        { 
            // ignored
        }

        return await _postsRepository.GetFeedAsync(options, cancellationToken);
    }
}