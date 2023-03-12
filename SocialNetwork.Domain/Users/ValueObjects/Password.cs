using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Users.ValueObjects;

public class Password : ValueObject
{
    public string HashedValue { get; }
    public string Salt { get; }

    public Password(string hashedValue, string salt)
    {
        HashedValue = hashedValue;
        Salt = salt;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return HashedValue;
        yield return Salt;
    }
}