namespace SocialNetwork.Domain.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException(string message, Exception? innerException = null)
        : base(message, innerException)
    {

    }
}