namespace Microsoft.eShopOnContainers.Services.Ordering.Domain.Seedwork;

public abstract class Entity<TId> where TId : IEquatable<TId>
{
    int? _requestedHashCode;
    TId _id;        
    public virtual TId Id 
    {
        get => _id;
        protected set => _id = value;
    }

    public bool IsTransient()
    {
        return Id.Equals(default);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity<TId> item)
            return false;

        if (ReferenceEquals(this, item))
            return true;

        if (GetType() != item.GetType())
            return false;

        if (item.IsTransient() || IsTransient())
            return false;
        return item.Id.Equals(Id);
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            _requestedHashCode ??= Id.GetHashCode() ^ 31;

            return _requestedHashCode.Value;
        }

        return base.GetHashCode();

    }
    public static bool operator ==(Entity<TId> left, Entity<TId>? right)
    {
        if (Object.Equals(left, null))
            return (Object.Equals(right, null)) ? true : false;
        return left.Equals(right);
    }

    public static bool operator !=(Entity<TId> left, Entity<TId> right)
    {
        return !(left == right);
    }
}