using System.Text.Json;
using SocialNetwork.Domain.Friends.Repositories;
using SocialNetwork.Domain.Posts;
using SocialNetwork.Domain.Posts.Repositories;
using SocialNetwork.Domain.Posts.ValueObjects;
using StackExchange.Redis;

namespace SocialNetwork.DataAccess.Repositories.Posts.Cache;

public class PostsCacheRepository : IPostsRepository
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly IPostsRepository _postsRepository;
    private readonly IFriendsRepository _friendsRepository;

    private static class CacheKeys
    {
        public static string Feed(long userId) => $"user:{userId}:feed";
        public static string FeedList(long userId) => $"user:{userId}:feed:list";
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
            await PushPostToFriendsFeed(post, cancellationToken, createdPost);
        }
        catch
        {
            // ignored
        }

        return createdPost;
    }

    private async Task PushPostToFriendsFeed(Post post, CancellationToken cancellationToken, Post createdPost)
    {
        var user = await _friendsRepository.GetUserByIdAsync(post.UserId, cancellationToken);

        var feedRecipientsIds = user?
            .Friends
            .Select(x => x.Id)
            .Concat(new[] {user.Id}) ?? ArraySegment<long>.Empty;

        var cache = _connectionMultiplexer.GetDatabase();
        var cacheDto = new PostCacheDto()
        {
            UserId = createdPost.UserId,
            Text = createdPost.Text.Value,
            Id = createdPost.Id,
            CreateDate = DateTimeOffset.UtcNow
        };

        var transaction = cache.CreateTransaction();

        foreach (var friend in feedRecipientsIds)
        {
            var hashKey = CacheKeys.Feed(friend);
            var feedList = CacheKeys.FeedList(friend);

            await transaction.HashSetAsync(
                hashKey, 
                cacheDto.Id.ToString(), 
                JsonSerializer.Serialize(cacheDto), 
                When.Exists);
            
            await transaction.ListRightPushAsync(
                feedList, 
                cacheDto.Id, 
                When.Exists);

            if (await transaction.ListLengthAsync(feedList) > Feed.MaxPosts)
            {
                var extraPostId = await transaction.ListLeftPopAsync(feedList);
                await transaction.HashDeleteAsync(hashKey, extraPostId);
            }
        }

        await transaction.ExecuteAsync();
    }

    public async Task UpdateAsync(Post updatedPost, CancellationToken cancellationToken)
    {
        await _postsRepository.UpdateAsync(updatedPost, cancellationToken);
        try
        {
            await UpdatePostInFriendsFeed(updatedPost, cancellationToken);
        }
        catch
        {
            // ignored
        }
    }

    private async Task UpdatePostInFriendsFeed(Post updatedPost, CancellationToken cancellationToken)
    {
        var user = await _friendsRepository.GetUserByIdAsync(updatedPost.UserId, cancellationToken);
        var feedRecipientsIds = user?
            .Friends
            .Select(x => x.Id)
            .Concat(new[] {user.Id}) ?? ArraySegment<long>.Empty;

        var cache = _connectionMultiplexer.GetDatabase();
        var cacheDto = new PostCacheDto()
        {
            Id = updatedPost.Id,
            Text = updatedPost.Text.Value,
            UserId = updatedPost.UserId
        };

        var transaction = cache.CreateTransaction();

        foreach (var friend in feedRecipientsIds)
        {
            var hashKey = CacheKeys.Feed(friend);
            
            await cache.HashSetAsync(
                hashKey, 
                cacheDto.Id.ToString(), 
                JsonSerializer.Serialize(cacheDto),
                When.Exists);
        }

        await transaction.ExecuteAsync();
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        await _postsRepository.DeleteAsync(id, cancellationToken);

        try
        {
            await DeleteFromFriendsFeed(id, cancellationToken);
        }
        catch
        {
            // ignored
        }
    }

    private async Task DeleteFromFriendsFeed(long id, CancellationToken cancellationToken)
    {
        var cache = _connectionMultiplexer.GetDatabase();

        var user = await _friendsRepository.GetUserByIdAsync(id, cancellationToken);
        var feedRecipientsIds = user?
            .Friends
            .Select(x => x.Id)
            .Concat(new[] {user.Id}) ?? ArraySegment<long>.Empty;

        var transaction = cache.CreateTransaction();

        foreach (var friend in feedRecipientsIds)
        {
            var hashKey = CacheKeys.Feed(friend);
            var feedList = CacheKeys.FeedList(friend);
            await cache.HashDeleteAsync(
                hashKey, 
                id.ToString());

            await cache.ListRemoveAsync(feedList, id.ToString());
        }

        await transaction.ExecuteAsync();
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
                        .HashGetAllAsync(CacheKeys.Feed(options.FeedRecipientUserId)))
                    .Where(x => x.Value.HasValue)
                    .Select(serializedPost => JsonSerializer.Deserialize<PostCacheDto>(serializedPost.Value!))
                    .OrderBy(x => x.CreateDate)
                    .Select(x => new Post(x!.Id, new(x.Text!), x.UserId))
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