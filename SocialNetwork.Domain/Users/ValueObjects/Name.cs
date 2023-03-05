using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.Common.Exceptions;

namespace SocialNetwork.Domain.Users.ValueObjects;

public class Name : ValueObject
{
    public string Value { get; }

    public Name(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentValidationFailedException("Must contain at least one non whitespace character", nameof(name));
        Value = name.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}