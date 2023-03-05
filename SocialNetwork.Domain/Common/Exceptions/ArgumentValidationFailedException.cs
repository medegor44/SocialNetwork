namespace SocialNetwork.Domain.Common.Exceptions;

public class ArgumentValidationFailedException : ValidationException
{
    public ArgumentValidationFailedException(string message, string paramName)
        : this($"Message: {message}. Param: {paramName}")
    {
    }
    
    protected ArgumentValidationFailedException(string message, Exception? innerException = null) : base(message, innerException)
    {
    }
}