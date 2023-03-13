using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Users.Entities;

public class City : Entity<Guid>
{
    public City(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public string Name { get; }
}