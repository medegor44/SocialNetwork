using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SocialNetwork;

public static class AuthOptions
{
    public const string Issuer = "SocialNetworkService";
    public const string Audience = "SocialNetworkClient";
    public const string Key = "secret_keysecret_keysecret_keysecret_keysecret_keysecret_keysecret_keysecret_key";
    public static SymmetricSecurityKey GetSymmetricSecurityKey() => 
        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
}