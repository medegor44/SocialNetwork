using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.Friends.Entities;

namespace SocialNetwork.Domain.Friends;

public class User : Entity<long>, IAggregateRoot
{
    public User(long id, IReadOnlyCollection<Friend> friends)
    {
        Id = id;
        Friends = friends;
    }

    public User AddFriends(IReadOnlyCollection<Friend> friends) => 
        new(Id, friends.Concat(Friends).ToHashSet());

    public User DeleteFriends(IReadOnlyCollection<Friend> friends) =>
        new(Id, Friends.Except(friends).ToHashSet());

    public IReadOnlyCollection<Friend> Friends { get; }
}