using System.Text.Json;
using SocialNetwork.DataAccess.Repositories.Posts.Cache;
using StackExchange.Redis;

namespace SocialNetwork.DataAccess.Repositories.Posts.Redis;

public static class RedisDatabaseExtensions
{
    public static async Task<RedisResult> FunctionCall(this IDatabase database, string functionName, params object[] args) => 
        await database.ExecuteAsync("FCALL", functionName, args);

    public static async Task<long> GetPostId(this IDatabase database)
    {
        var id = await database.ExecuteAsync("FCALL", "get_post_id", 0);

        if (id.Type != ResultType.Integer || id.ToString() is null)
            throw new Exception("Invalid redis type");

        return long.Parse(id.ToString()!);
    }

    public static async Task SavePost(this IDatabase database, PostCacheDto dto)
    {
        await database.ExecuteAsync("FCALL", "save_post", 3, JsonSerializer.Serialize(dto), dto.UserId.ToString(),
            dto.Id.ToString());
    }

    public static async Task DeletePost(this IDatabase database, long postId, long userId)
    {
        await database.ExecuteAsync("FCALL", "delete_post", 2, userId, postId);
    }

    public static async Task<PostCacheDto?> GetById(this IDatabase database, long postId)
    {
        var result = await database.ExecuteAsync("FCALL", "get_post_by_id", 1, postId);

        if (result.IsNull)
            return null;
        
        if (result.Type is not (ResultType.SimpleString or ResultType.BulkString))
            throw new Exception("Invalid redis type");

        return JsonSerializer.Deserialize<PostCacheDto>(result.ToString()!);
    }

    public static async Task<IReadOnlyCollection<long>> GetUserPosts(this IDatabase database, long userId, int start,
        int stop)
    {
        var result = await database.ExecuteAsync("FCALL", "get_user_posts", 3, userId, start, stop);
        
        if (result.IsNull)
            return ArraySegment<long>.Empty;
        
        if (result.Type is not (ResultType.BulkString or ResultType.SimpleString))
            throw new Exception("Invalid redis type");

        return JsonSerializer.Deserialize<List<long>>(result.ToString()!)!;
    }
}