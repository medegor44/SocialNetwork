using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;
using SocialNetwork.Domain.Posts.ValueObjects;

namespace SocialNetwork.Domain.Posts;

public sealed class Post : Entity<Guid>, IAggregateRoot
{
    public Text Text { get; }
    public Guid UserId { get; }
    
    public Post(Guid id, Text text, Guid userId)
    {
        Id = id;
        Text = text;
        UserId = userId;
    }
}