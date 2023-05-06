using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.Posts.ValueObjects;

namespace SocialNetwork.Domain.Posts;

public sealed class Post : Entity<long>, IAggregateRoot
{
    public Text Text { get; }
    public long UserId { get; }
    
    public Post(long id, Text text, long userId)
    {
        Id = id;
        Text = text;
        UserId = userId;
    }
}