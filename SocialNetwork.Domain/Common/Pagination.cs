namespace SocialNetwork.Domain.Common;

public class Pagination : ValueObject
{
    public int Limit { get; }
    public int Offset { get; }

    public Pagination(int limit, int offset)
    {
        if (limit <= 0)
            throw new ArgumentException("Must be positive", nameof(limit));
        if (offset < 0)
            throw new ArgumentException("Must be non negative", nameof(offset));
        
        Limit = limit;
        Offset = offset;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Limit;
        yield return Offset;
    }
}