using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Friends.Entities;

public class Friend : Entity<long>
{
    public Friend(long id, IReadOnlyCollection<long> friends)
    {
        Id = id;
        Friends = friends;
    }

    public Friend AddFriend(long friendId) =>
        new(Id, Friends.Concat(new[] {friendId}).ToHashSet());

    public IReadOnlyCollection<long> Friends { get; }
}