using System.Text.Json;
using SocialNetwork.DataAccess.Redis;
using SocialNetwork.Domain.Friends.Repositories;
using SocialNetwork.Domain.Posts;
using SocialNetwork.Domain.Posts.Repositories;
using SocialNetwork.Domain.Posts.ValueObjects;
using StackExchange.Redis;

namespace SocialNetwork.DataAccess.Repositories.Posts.Cache;

public class PostsCacheRepository : IPostsRepository
{
    private const int DatabaseId = 1;
    private readonly IRedisProvider _provider;
    private readonly IPostsRepository _postsRepository;
    private readonly IFriendsRepository _friendsRepository;

    public PostsCacheRepository(
        IRedisProvider provider, 
        IPostsRepository postsRepository,
        IFriendsRepository friendsRepository)
    {
        _provider = provider;
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

        await using var connection = await _provider.CreateConnectionAsync();
        var cache = connection.GetDatabase(DatabaseId);
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
            var userCache = CacheKeys.User(friend);
            
            if (!await cache.KeyExistsAsync(userCache))
                continue;
            
            _ = transaction.HashSetAsync(
                hashKey, 
                cacheDto.Id.ToString(), 
                JsonSerializer.Serialize(cacheDto));
            
            _ = transaction.ListRightPushAsync(
                feedList, 
                cacheDto.Id);

            if (await cache.ListLengthAsync(feedList) == Feed.MaxPosts)
            {
                var extraPostId = await transaction.ListLeftPopAsync(feedList);
                _ = transaction.HashDeleteAsync(hashKey, extraPostId);
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

        await using var connection = await _provider.CreateConnectionAsync();
        var cache = connection.GetDatabase(DatabaseId);
        var cacheDto = new PostCacheDto()
        {
            Id = updatedPost.Id,
            Text = updatedPost.Text.Value,
            UserId = updatedPost.UserId,
            CreateDate = updatedPost.CreateDate
        };

        foreach (var friend in feedRecipientsIds)
        {
            var hashKey = CacheKeys.Feed(friend);
            var userCache = CacheKeys.User(friend);
            
            if (!await cache.KeyExistsAsync(userCache))
                continue;

            await cache.HashSetAsync(
                hashKey, 
                cacheDto.Id.ToString(), 
                JsonSerializer.Serialize(cacheDto));
        }
    }

    public async Task DeleteAsync(Post post, CancellationToken cancellationToken)
    {
        await _postsRepository.DeleteAsync(post, cancellationToken);

        try
        {
            await DeleteFromFriendsFeed(post, cancellationToken);
        }
        catch
        {
            // ignored
        }
    }

    private async Task DeleteFromFriendsFeed(Post post, CancellationToken cancellationToken)
    {
        await using var connection = await _provider.CreateConnectionAsync();
        var cache = connection.GetDatabase(DatabaseId);

        var user = await _friendsRepository.GetUserByIdAsync(post.UserId, cancellationToken);
        var feedRecipientsIds = user?
            .Friends
            .Select(x => x.Id)
            .Concat(new[] {user.Id}) ?? ArraySegment<long>.Empty;

        var transaction = cache.CreateTransaction();

        foreach (var friend in feedRecipientsIds)
        {
            var hashKey = CacheKeys.Feed(friend);
            var feedList = CacheKeys.FeedList(friend);
            var userCache = CacheKeys.User(friend);
            
            if (!await cache.KeyExistsAsync(userCache))
                continue;

            _ = transaction.HashDeleteAsync(
                hashKey, 
                post.Id.ToString());

            _ = transaction.ListRemoveAsync(feedList, post.Id.ToString());
        }

        await transaction.ExecuteAsync();
    }

    public Task<IReadOnlyCollection<Post>> GetByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken) => 
        _postsRepository.GetByIdsAsync(ids, cancellationToken);

    public async Task<Feed> GetFeedAsync(FeedOptions options, CancellationToken cancellationToken)
    {
        try
        {
            var posts = await GetPostsFromCacheOrDefaultAsync(options.FeedRecipientUserId);
            if (posts != null)
                return new(posts
                        .Skip(options.Offset)
                        .Take(options.Limit)
                        .ToList(),
                    posts.Count);
        }
        catch
        { 
            // ignored
        }

        var feed = await _postsRepository.GetFeedAsync(new(options.FeedRecipientUserId, 0, Feed.MaxPosts), cancellationToken);

        try
        {
            await SaveToCache(feed.PostsOnPage, options.FeedRecipientUserId);
        }
        catch
        {
            // ignored
        }

        return new(feed.PostsOnPage
                .Skip(options.Offset)
                .Take(options.Limit)
                .ToList(),
            feed.TotalCount);
    }

    private async Task SaveToCache(IReadOnlyCollection<Post> posts, long userId)
    {
        await using var connection = await _provider.CreateConnectionAsync();
        var cache = connection.GetDatabase(DatabaseId);

        var hashEntries = posts
            .Select(p => new PostCacheDto()
            {
                Id = p.Id,
                Text = p.Text.Value,
                UserId = p.UserId,
                CreateDate = p.CreateDate
            })
            .Select(p => new HashEntry(p.Id.ToString(), JsonSerializer.Serialize(p)))
            .ToArray();

        var asyncState = new object();
        var transaction = cache.CreateTransaction(asyncState);
        _ = transaction.StringSetAsync(CacheKeys.User(userId), userId);
        _ = transaction.HashSetAsync(CacheKeys.Feed(userId), hashEntries);
        foreach (var post in posts)
            _ = transaction.ListRightPushAsync(CacheKeys.FeedList(userId), post.Id);

        await transaction.ExecuteAsync();
    }

    private async Task<List<Post>?> GetPostsFromCacheOrDefaultAsync(long userId)
    {
        await using var connection = await _provider.CreateConnectionAsync();
        var cache = connection.GetDatabase(DatabaseId);

        if (!await cache.KeyExistsAsync(CacheKeys.User(userId))) 
            return null;
        
        var feedFromCache = (await cache
                .HashGetAllAsync(CacheKeys.Feed(userId)))
            .Where(x => x.Value.HasValue)
            .Select(serializedPost => JsonSerializer.Deserialize<PostCacheDto>(serializedPost.Value!))
            .OrderBy(x => x.CreateDate)
            .Select(x => new Post(x!.Id, new(x.Text!), x.UserId, x.CreateDate))
            .ToList();

        return feedFromCache;
    }
}