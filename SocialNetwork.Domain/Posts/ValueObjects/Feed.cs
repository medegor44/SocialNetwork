using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.Common.Exceptions;

namespace SocialNetwork.Domain.Posts.ValueObjects;

public class Feed : ValueObject
{
    public static int MaxPosts => 1000;
    public IReadOnlyCollection<Post> PostsOnPage { get; }
    public int TotalCount { get; }

    public Feed(IReadOnlyCollection<Post> postsOnPage, int totalCount)
    {
        if (postsOnPage.Count > totalCount)
            throw new ArgumentValidationFailedException("Should not be greater than totalCount", nameof(postsOnPage));
        if (totalCount > MaxPosts)
            throw new ArgumentValidationFailedException(
                $"Total count of posts in feed should not be greater than {MaxPosts}", nameof(totalCount));
        
        PostsOnPage = postsOnPage;
        TotalCount = totalCount;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return PostsOnPage;
        yield return TotalCount;
    }
}