using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.Common.Exceptions;

namespace SocialNetwork.Domain.Users.ValueObjects;

public class Biography : ValueObject
{
    public Biography(string biography)
    {
        Value = biography ?? throw new ArgumentValidationFailedException("Must be not null", nameof(biography));
    }

    public string Value { get; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}