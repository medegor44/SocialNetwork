using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Friends.ValueObjects;

public class FriendsId : ValueObject, IEquatable<FriendsId>
{
    public long UserId { get; }
    public long FriendId { get; }

    public FriendsId(long userId, long friendId)
    {
        UserId = userId;
        FriendId = friendId;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return UserId;
        yield return FriendId;
    }

    public bool Equals(FriendsId? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((FriendsId) obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), UserId, FriendId);
    }
}