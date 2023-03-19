namespace SocialNetwork.Services.Exceptions;

public class BadRequestException : Exception
{
    public BadRequestException(string message, Exception? e = null) : base(message, e)
    {
        
    }
}