using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.Common.Exceptions;

namespace SocialNetwork.Domain.Users.ValueObjects;

public class Age : ValueObject
{
    public int Value { get; }

    public Age(int value)
    {
        if (value <= 0)
            throw new ArgumentValidationFailedException("Mut be positive", nameof(value));
        Value = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}