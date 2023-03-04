using Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;
using SocialNetwork.Domain.Users.ValueObjects;

namespace SocialNetwork.Domain.Users;

public sealed class User : Entity<Guid>, IAggregateRoot
{
    public Guid CityId { get; }
    public Name FirstName { get; }
    public Name LastName { get; }
    public Age Age { get; }
    public Biography Biography { get; }
    public IReadOnlyCollection<Guid> FriendIds { get; } = new List<Guid>();
    public IReadOnlyCollection<Guid> PostsIds { get; } = new List<Guid>();

    public User(Guid id, Name firstName, Name lastName, Age age, Biography biography, Guid cityId)
    {
        CityId = cityId;
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        Biography = biography;
        Id = id;
    }

    public User(
        Guid id, 
        Name firstName, 
        Name lastName, 
        Age age, 
        Biography biography, 
        Guid cityId,
        IReadOnlyCollection<Guid> friendIds)
        : this(id, firstName, lastName, age, biography, cityId)
    {
        FriendIds = friendIds;
    }

    public User(
        Guid id, 
        Name firstName, 
        Name lastName, 
        Age age, 
        Biography biography, 
        Guid cityId,
        IReadOnlyCollection<Guid> friendIds, 
        IReadOnlyCollection<Guid> postsIdsIds)
        : this(id, firstName, lastName, age, biography, cityId, friendIds)
    {
        PostsIds = postsIdsIds;
    }
}