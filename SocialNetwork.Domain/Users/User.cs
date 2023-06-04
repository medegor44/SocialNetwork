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
}