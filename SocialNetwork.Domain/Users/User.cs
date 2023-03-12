using SocialNetwork.Domain.Common;
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
    public Password Password { get; }

    public User(Name firstName, Name lastName, Age age, Biography biography, Guid cityId, Password password)
    {
        CityId = cityId;
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        Biography = biography;
        Password = password;
    }
    
    public User(Guid id, Name firstName, Name lastName, Age age, Biography biography, Guid city, Password password) 
        : this(firstName, lastName, age, biography, city, password) 
    {
        Id = id;
    }

    public User(
        Guid id, 
        Name firstName, 
        Name lastName, 
        Age age, 
        Biography biography, 
        Guid city, Password password,
        IReadOnlyCollection<Guid> friendIds)
        : this(id, firstName, lastName, age, biography, city, password)
    {
        FriendIds = friendIds;
    }

    public User(
        Guid id, 
        Name firstName, 
        Name lastName, 
        Age age, 
        Biography biography, 
        Guid city, Password password,
        IReadOnlyCollection<Guid> friendIds, 
        IReadOnlyCollection<Guid> postsIdsIds)
        : this(id, firstName, lastName, age, biography, city, password, friendIds)
    {
        PostsIds = postsIdsIds;
    }
}