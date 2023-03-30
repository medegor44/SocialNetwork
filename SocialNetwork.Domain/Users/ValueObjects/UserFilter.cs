using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Users.ValueObjects;

public class UserFilter : ValueObject
{
    public UserFilter(string? firstName, string? secondName, Pagination pagination)
    {
        FirstName = firstName;
        SecondName = secondName;
        Pagination = pagination;
    }

    public string? FirstName { get; }
    public string? SecondName { get; }
    public Pagination Pagination { get; }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return FirstName;
        yield return SecondName;
        yield return Pagination;
    }
}