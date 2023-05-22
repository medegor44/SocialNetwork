namespace SocialNetwork.DataAccess.Repositories.Posts.Cache;

public interface IPostsCacheInvalidator
{
    Task InvalidateAsync(IReadOnlyCollection<long> userIds);
}