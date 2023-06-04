using System.Text.Json.Serialization;

namespace SocialNetwork.DataAccess.Repositories.Posts.Cache;

public class PostCacheDto
{
    [JsonPropertyOrder(1)]
    public long Id { get; set; }
    [JsonPropertyOrder(2)]
    public long UserId { get; set; }
    [JsonPropertyOrder(3)]
    public string? Text { get; set; }
    [JsonPropertyOrder(4)]
    public DateTimeOffset CreateDate { get; set; }
}