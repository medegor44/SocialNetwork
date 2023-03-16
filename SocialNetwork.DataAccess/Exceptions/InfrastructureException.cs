namespace SocialNetwork.DataAccess.Exceptions;

public class InfrastructureException : Exception
{
    public InfrastructureException(string message, Exception? e = null) : base(message, e)
    {
        
    }
}