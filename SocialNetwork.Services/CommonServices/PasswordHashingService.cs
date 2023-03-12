using System.Security.Cryptography;
using System.Text;
using SocialNetwork.Domain.Users.ValueObjects;

namespace SocialNetwork.Services.CommonServices;

public class PasswordHashingService : IPasswordHashingService
{
    private const int SaltLength = 20;
    
    public Password Hash(string rawPassword)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltLength);
        return Hash(rawPassword, Encoding.UTF8.GetString(salt));
    }
    
    private Password Hash(string rawPassword, string salt)
    {
        var hash = SHA256.HashData(
            Encoding.ASCII.GetBytes(rawPassword)
                .Concat(Encoding.ASCII.GetBytes(salt))
                .ToArray());

        return new(Encoding.ASCII.GetString(hash), salt);
    }

    public bool Check(string rawPassword, Password hashedPassword) => 
        Hash(rawPassword, hashedPassword.Salt).Equals(hashedPassword);
}