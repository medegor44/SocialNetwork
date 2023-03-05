using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.Common.Exceptions;

namespace SocialNetwork.Domain.Posts.ValueObjects;

public class Text : ValueObject
{
    public string Value { get; }
    
    public Text(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentValidationFailedException("Must contain at least one non whitespace character", nameof(text));
        
        Value = text;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}