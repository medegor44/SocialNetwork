using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.Posts.ValueObjects;

namespace SocialNetwork.Domain.Posts;

public sealed class Post : Entity<long>, IAggregateRoot
{
    public Text Text { get; }
    public long UserId { get; }
    public DateTimeOffset CreateDate { get; }
    
    public Post(long id, Text text, long userId, DateTimeOffset createDate)
    {
        Id = id;
        Text = text;
        UserId = userId;
        CreateDate = createDate;
    }
    
    public Post(Text text, long userId)
    {
        Id = default;
        Text = text;
        UserId = userId;
    }
}