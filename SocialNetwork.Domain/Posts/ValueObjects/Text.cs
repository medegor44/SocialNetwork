using Microsoft.eShopOnContainers.Services.Ordering.Domain.SeedWork;

namespace SocialNetwork.Domain.Posts.ValueObjects;

public class Text : ValueObject
{
    public string Value { get; }
    
    public Text(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Must contain at least one non whitespace character", nameof(text));
        
        Value = text;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}