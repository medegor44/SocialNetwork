using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Users.ValueObjects;

public class UserFilter : ValueObject
{
    public UserFilter(Guid? id, string? firstName, string? secondName)
    {
        Id = id;
        FirstName = firstName;
        SecondName = secondName;
    }

    public Guid? Id { get; }
    public string? FirstName { get; }
    public string? SecondName { get; }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Id;
        yield return FirstName;
        yield return SecondName;
    }
}