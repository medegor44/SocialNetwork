using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Users.Entities;

public class City : Entity<long>
{
    public City(long id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public string Name { get; }
}