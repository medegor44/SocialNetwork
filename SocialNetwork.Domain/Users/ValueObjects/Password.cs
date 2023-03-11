using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Users.ValueObjects;

public class Password : ValueObject
{
    public string Value { get; }

    public Password(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Должен содержать хотя бы один непробельный символ", nameof(value));
        Value = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}