using System.Text.Json;
using SocialNetwork.DataAccess.Redis;
using SocialNetwork.DataAccess.Repositories.Posts.Cache;
using SocialNetwork.Domain.Friends.Repositories;
using SocialNetwork.Domain.Posts;
using SocialNetwork.Domain.Posts.Repositories;
using SocialNetwork.Domain.Posts.ValueObjects;
using StackExchange.Redis;

namespace SocialNetwork.DataAccess.Repositories.Posts.Redis;

public class PostsRedisRepository : IPostsRepository
{
    private readonly IRedisProvider _provider;
    private readonly IFriendsRepository _friendsRepository;

    public PostsRedisRepository(
        IRedisProvider provider, 
        IFriendsRepository friendsRepository)
    {
        _provider = provider;
        _friendsRepository = friendsRepository;
    }
    
    public async Task<Post> CreateAsync(Post post, CancellationToken cancellationToken)
    {
        await using var connection = await _provider.CreateConnectionAsync();
        var database = connection.GetDatabase();

        var id = await database.GetPostId();

        var dto = new PostCacheDto()
        {
            Id = id,
            CreateDate = DateTimeOffset.UtcNow,
            Text = post.Text.Value,
            UserId = post.UserId
        };

        await database.SavePost(dto);

        var createdPost = new Post(dto.Id, post.Text, post.UserId, post.CreateDate);

        return createdPost;
    }

    public async Task UpdateAsync(Post updatedPost, CancellationToken cancellationToken)
    {
        await using var connection = await _provider.CreateConnectionAsync();
        var database = connection.GetDatabase();

        var dto = new PostCacheDto()
        {
            Id = updatedPost.Id,
            CreateDate = updatedPost.CreateDate,
            Text = updatedPost.Text.Value,
            UserId = updatedPost.UserId
        };
        
        await database.SavePost(dto);
    }

    public async Task DeleteAsync(Post post, CancellationToken cancellationToken)
    {
        await using var connection = await _provider.CreateConnectionAsync();
        var database = connection.GetDatabase();
        
        await database.DeletePost(post.Id, post.UserId);
    }

    public async Task<IReadOnlyCollection<Post>> GetByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken)
    {
        await using var connection = await _provider.CreateConnectionAsync();
        var database = connection.GetDatabase();

        var dtos = ids.Select(x => database.GetById(x));

        return (await Task.WhenAll(dtos))
            .Where(dto => dto != null)
            .Select(dto => new Post(dto!.Id, new(dto.Text!), dto.UserId, dto.CreateDate))
            .ToList();
    }

    public Task<Feed> GetFeedAsync(FeedOptions options, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}