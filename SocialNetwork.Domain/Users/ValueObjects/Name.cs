using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

namespace SocialNetwork.Domain.Users.ValueObjects;

public class Name : ValueObject
{
    public string Value { get; }

    public Name(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Must contain at least one non whitespace character", nameof(name));
        Value = name.Trim();
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}