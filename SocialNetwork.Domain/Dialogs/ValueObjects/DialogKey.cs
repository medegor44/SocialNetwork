using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Dialogs.ValueObjects;

public class DialogKey : ValueObject, IEquatable<DialogKey>
{
    public long From { get; }
    public long To { get; }

    public DialogKey(long from, long to)
    {
        From = from;
        To = to;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return new[] { From, To }.Order().Select(x => (object)x);
    }

    public bool Equals(DialogKey? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return base.Equals(other) && From == other.From && To == other.To;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((DialogKey)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(base.GetHashCode(), From, To);
    }
}