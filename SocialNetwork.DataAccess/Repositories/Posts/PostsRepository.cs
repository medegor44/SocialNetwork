using Npgsql;
using SocialNetwork.DataAccess.DbDto;
using SocialNetwork.DataAccess.Exceptions;
using SocialNetwork.Domain.Posts;
using SocialNetwork.Domain.Posts.Repositories;
using SocialNetwork.Domain.Posts.ValueObjects;

namespace SocialNetwork.DataAccess.Repositories;

public class PostsRepository : IPostsRepository
{
    private readonly NpgsqlDataSource _source;
    private const string PostsTableName = "Posts";
    private const string FriendsTableName = "Friends";
    
    public PostsRepository(NpgsqlDataSource source)
    {
        _source = source;
    }
    
    public async Task<Post> CreateAsync(Post post, CancellationToken cancellationToken)
    {
        var sql = $"""
INSERT INTO "{PostsTableName}"("UserId", "Text", "CreateDate")
VALUES (@{nameof(PostDbDto.UserId)}, @{nameof(PostDbDto.Text)}, @{nameof(PostDbDto.CreateDate)})
RETURNING "Id"
""";

        await using var connection = _source.CreateConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        var createDate = DateTimeOffset.UtcNow;
        
        var dto = new PostDbDto()
        {
            UserId = post.UserId,
            Text = post.Text.Value,
            CreateDate = createDate
        };

        command.Parameters.AddWithValue(nameof(PostDbDto.UserId), dto.UserId);
        command.Parameters.AddWithValue(nameof(PostDbDto.Text), dto.Text);
        command.Parameters.AddWithValue(nameof(PostDbDto.CreateDate), dto.CreateDate);
        
        await connection.OpenAsync(cancellationToken);

        if (await command.ExecuteScalarAsync(cancellationToken) is not long id)
            throw new InfrastructureException("Couldn't create post");

        return new(id, post.Text, post.UserId, createDate);
    }

    public async Task UpdateAsync(Post updatedPost, CancellationToken cancellationToken)
    {
        var sql = $"""
UPDATE "{PostsTableName}"
SET "Text" = @{nameof(PostDbDto.Text)}
WHERE "Id" = @{nameof(PostDbDto.Id)}
""";

        await using var connection = _source.CreateConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        
        var dto = new PostDbDto
        {
            Text = updatedPost.Text.Value,
            Id = updatedPost.Id
        };

        command.Parameters.AddWithValue(nameof(PostDbDto.Text), dto.Text);
        command.Parameters.AddWithValue(nameof(PostDbDto.Id), dto.Id);
        
        await connection.OpenAsync(cancellationToken);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task DeleteAsync(long id, CancellationToken cancellationToken)
    {
        var sql = $"""
DELETE FROM "{PostsTableName}"
WHERE "Id" = @{nameof(PostDbDto.Id)}
""";

        await using var connection = _source.CreateConnection();
        await using var command = new NpgsqlCommand(sql, connection);

        var dto = new PostDbDto
        {
            Id = id
        };

        command.Parameters.AddWithValue(nameof(PostDbDto.Id), dto.Id);
        
        await connection.OpenAsync(cancellationToken);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Post>> GetByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken)
    {
        var sql = $"""
SELECT
    "Id",
    "Text",
    "UserId",
    "CreateDate"
FROM
    "{PostsTableName}"
WHERE
    "Id" = ANY(ARRAY[@ids]::INT8[])
""";

        await using var connection = _source.CreateConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("Ids", ids.ToList());

        await connection.OpenAsync(cancellationToken);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        var dtos = new List<PostDbDto>();
        while (await reader.ReadAsync(cancellationToken))
        {
            var dto = new PostDbDto
            {
                Id = await reader.GetFieldValueAsync<long>(0, cancellationToken),
                Text = await reader.GetFieldValueAsync<string>(1, cancellationToken),
                UserId = await reader.GetFieldValueAsync<long>(2, cancellationToken),
                CreateDate = await reader.GetFieldValueAsync<DateTimeOffset>(3, cancellationToken)
            };
            
            dtos.Add(dto);
        }

        return dtos
            .Select(x => new Post(x.Id, new(x.Text), x.UserId, x.CreateDate))
            .ToList();
    }

    public async Task<Feed> GetFeedAsync(FeedOptions options, CancellationToken cancellationToken)
    {
        var sql = $"""
WITH 
"Posts" AS (
    (SELECT
        p."Id",
        p."UserId",
        p."Text",
        p."CreateDate"
    FROM 
        "{PostsTableName}" p 
            JOIN "{FriendsTableName}" f ON p."UserId" = f."FriendId"
    WHERE
        f."UserId" = @UserId
    ORDER BY
        p."CreateDate" DESC
    LIMIT @TotalPosts)

    UNION

    (SELECT
        p."Id",
        p."UserId",
        p."Text",
        p."CreateDate"
    FROM 
        "{PostsTableName}" p 
    WHERE
        p."UserId" = @UserId
    ORDER BY
        p."CreateDate" DESC
    LIMIT @TotalPosts)
)
SELECT 
    t."Id",
    t."UserId",
    t."Text",
    t."CreateDate"
FROM 
    "Posts" AS t
ORDER BY 
    t."CreateDate" DESC
LIMIT @Limit
OFFSET @Offset
""";

        await using var connection = _source.CreateConnection();
        await using var command = new NpgsqlCommand(sql, connection);

        command.Parameters.AddWithValue("UserId", options.FeedRecipientUserId);
        command.Parameters.AddWithValue("TotalPosts", Feed.MaxPosts);
        command.Parameters.AddWithValue("Limit", options.Limit);
        command.Parameters.AddWithValue("Offset", options.Offset);
        
        await connection.OpenAsync(cancellationToken);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var postDbDtos = new List<PostDbDto>();
        while (await reader.ReadAsync(cancellationToken))
            postDbDtos.Add(new()
            { 
                Id = await reader.GetFieldValueAsync<long>(0, cancellationToken),
                UserId = await reader.GetFieldValueAsync<long>(1, cancellationToken),
                Text = await reader.GetFieldValueAsync<string>(2, cancellationToken),
                CreateDate = await reader.GetFieldValueAsync<DateTimeOffset>(3, cancellationToken)
            });

        return new(postDbDtos
                .Select(x => new Post(
                    x.Id,
                    new(x.Text),
                    x.UserId, 
                    x.CreateDate)).ToList(),
            Feed.MaxPosts);
    }
}