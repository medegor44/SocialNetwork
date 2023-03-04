using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Posts.ValueObjects;

public class PostFilter : ValueObject
{
    public IReadOnlyCollection<Guid> UserIds { get; }
    public Pagination Pagination { get; }

    public PostFilter(IReadOnlyCollection<Guid> userIds, Pagination pagination)
    {
        UserIds = userIds;
        Pagination = pagination;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return UserIds;
        yield return Pagination;
    }
}