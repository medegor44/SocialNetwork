using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Posts.ValueObjects;

public class FeedOptions : ValueObject
{
    public long FeedRecipientUserId { get; }
    public int Offset { get; }
    public int Limit { get; }

    public FeedOptions(long feedRecipientUserId, int offset, int limit)
    {
        FeedRecipientUserId = feedRecipientUserId;
        Offset = offset;
        Limit = limit;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FeedRecipientUserId;
        yield return Offset;
        yield return Limit;
    }
}