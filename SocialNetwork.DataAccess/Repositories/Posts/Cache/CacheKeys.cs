namespace SocialNetwork.DataAccess.Repositories.Posts.Cache;

public static class CacheKeys
{
    private static string UserEntity => "user";
    public static string Feed(long userId) => $"{User(userId)}:feed";
    public static string FeedList(long userId) => $"{Feed(userId)}:list";
    public static string User(long userId) => $"{UserEntity}:{userId}";
    public static string PostsCounter(long userId) => $"{User(userId)}:count";
    public static string PostsStorage(long userId) => $"{User(userId)}:posts";
}