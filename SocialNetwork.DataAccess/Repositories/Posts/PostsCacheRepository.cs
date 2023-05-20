using System.Text.Json;
using SocialNetwork.DataAccess.Repositories.Dto;
using SocialNetwork.Domain.Friends.Entities;
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

    private static class CacheKeys
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
    
    public async Task<Post> CreateAsync(Post post, CancellationToken cancellationToken)
    {
        var createdPost = await _postsRepository.CreateAsync(post, cancellationToken);

        try
        {
            var user = await _friendsRepository.GetUserByIdAsync(post.UserId, cancellationToken);

            var cache = _connectionMultiplexer.GetDatabase();
            var cacheDto = new PostCacheDto()
            {
                UserId = createdPost.UserId,
                Text = createdPost.Text.Value,
                Id = createdPost.Id
            };

            var transaction = cache.CreateTransaction();

            foreach (var friend in user?.Friends ?? ArraySegment<Friend>.Empty)
            {
                var redisKey = CacheKeys.Feed(friend.Id);
                await transaction.ListRightPushAsync(redisKey, JsonSerializer.Serialize(cacheDto));
                if (await transaction.ListLengthAsync(redisKey) > Feed.MaxPosts)
                    await transaction.ListLeftPopAsync(redisKey);
            }

            await transaction.ExecuteAsync();
        }
        catch
        {
            // ignored
        }

        return createdPost;
    }

    public async Task UpdateAsync(Post updatedPost, CancellationToken cancellationToken)
    {
        await _postsRepository.UpdateAsync(updatedPost, cancellationToken);
        try
        {

            var user = await _friendsRepository.GetUserByIdAsync(updatedPost.UserId, cancellationToken);

            var cache = _connectionMultiplexer.GetDatabase();
            var cacheDto = new PostCacheDto()
            {
                Id = updatedPost.Id,
                Text = updatedPost.Text.Value,
                UserId = updatedPost.UserId
            };

            var transaction = cache.CreateTransaction();
        
            foreach (var friend in user?.Friends ?? ArraySegment<Friend>.Empty)
            {
                var redisKey = CacheKeys.Feed(friend.Id);
                var (_, position) = (await transaction.ListRangeAsync(redisKey))
                    .Where(serialized => serialized.HasValue)
                    .Select((serialized, idx) => (Post: JsonSerializer.Deserialize<PostCacheDto>(serialized!), Idx: idx))
                    .FirstOrDefault(pair => pair.Post!.Id == updatedPost.Id);
            
                await transaction.ListSetByIndexAsync(redisKey, position, JsonSerializer.Serialize(cacheDto));
            }

            await transaction.ExecuteAsync();
        }
        catch
        {
            // ignored
        }
    }

    public Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<Post>> GetByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken) => 
        _postsRepository.GetByIdsAsync(ids, cancellationToken);

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