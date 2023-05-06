using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.Users.Entities;
using SocialNetwork.Domain.Users.ValueObjects;

namespace SocialNetwork.Domain.Users;

public sealed class User : Entity<long>, IAggregateRoot
{
    public City City { get; }
    public Name FirstName { get; }
    public Name LastName { get; }
    public Age Age { get; }
    public Biography Biography { get; }
    public IReadOnlyCollection<long> FriendIds { get; } = new List<long>();
    public IReadOnlyCollection<long> PostsIds { get; } = new List<long>();
    public Password? Password { get; }

    public User(Name firstName, Name lastName, Age age, Biography biography, City city, Password? password)
    {
        City = city;
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        Biography = biography;
        Password = password;
    }
    
    public User(long id, Name firstName, Name lastName, Age age, Biography biography, City city, Password? password = null) 
        : this(firstName, lastName, age, biography, city, password) 
    {
        Id = id;
    }

    public User(
        long id, 
        Name firstName, 
        Name lastName, 
        Age age, 
        Biography biography, 
        City city, 
        Password? password,
        IReadOnlyCollection<long> friendIds)
        : this(id, firstName, lastName, age, biography, city, password)
    {
        FriendIds = friendIds;
    }

    public User(
        long id, 
        Name firstName, 
        Name lastName, 
        Age age, 
        Biography biography, 
        City city, 
        Password? password,
        IReadOnlyCollection<long> friendIds, 
        IReadOnlyCollection<long> postsIdsIds)
        : this(id, firstName, lastName, age, biography, city, password, friendIds)
    {
        PostsIds = postsIdsIds;
    }
}