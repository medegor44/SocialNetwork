namespace SocialNetwork.DataAccess.Repositories.Posts.Cache;

public static class CacheKeys
{
    private static string UserEntity => "user";
    public static string Feed(long userId) => $"{User(userId)}:feed";
    public static string FeedList(long userId) => $"{Feed(userId)}:list";
    public static string User(long userId) => $"{UserEntity}:{userId}";
}