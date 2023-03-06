﻿using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.Users.ValueObjects;

namespace SocialNetwork.Domain.Users;

public sealed class User : Entity<Guid>, IAggregateRoot
{
    public string City { get; }
    public Name FirstName { get; }
    public Name LastName { get; }
    public Age Age { get; }
    public Biography Biography { get; }
    public IReadOnlyCollection<Guid> FriendIds { get; } = new List<Guid>();
    public IReadOnlyCollection<Guid> PostsIds { get; } = new List<Guid>();

    public User(Name firstName, Name lastName, Age age, Biography biography, string city)
    {
        City = city;
        FirstName = firstName;
        LastName = lastName;
        Age = age;
        Biography = biography;
    }
    
    public User(Guid id, Name firstName, Name lastName, Age age, Biography biography, string city) 
        : this(firstName, lastName, age, biography, city) 
    {
        Id = id;
    }

    public User(
        Guid id, 
        Name firstName, 
        Name lastName, 
        Age age, 
        Biography biography, 
        string city,
        IReadOnlyCollection<Guid> friendIds)
        : this(id, firstName, lastName, age, biography, city)
    {
        FriendIds = friendIds;
    }

    public User(
        Guid id, 
        Name firstName, 
        Name lastName, 
        Age age, 
        Biography biography, 
        string city,
        IReadOnlyCollection<Guid> friendIds, 
        IReadOnlyCollection<Guid> postsIdsIds)
        : this(id, firstName, lastName, age, biography, city, friendIds)
    {
        PostsIds = postsIdsIds;
    }
}