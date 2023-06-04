using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Friends.Entities;

public class Friend : Entity<long>
{
    public Friend(long id)
    {
        Id = id;
    }
}