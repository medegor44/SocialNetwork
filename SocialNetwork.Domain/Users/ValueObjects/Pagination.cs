namespace SocialNetwork.Domain.Users.ValueObjects;

public class Pagination
{
    public Pagination(int limit, int offset)
    {
        if (limit < 0 || offset < 0)
            throw new ArgumentException("Limits must be non negative");
        Limit = limit;
        Offset = offset;
    }
    
    public int Limit { get; }
    public int Offset { get; }
}