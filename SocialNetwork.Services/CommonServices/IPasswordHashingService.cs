using SocialNetwork.Domain.Users.ValueObjects;

namespace SocialNetwork.Services.CommonServices;

public interface IPasswordHashingService
{
    public Password Hash(string rawPassword);
    public bool Check(string rawPassword, Password hashedPassword);
}