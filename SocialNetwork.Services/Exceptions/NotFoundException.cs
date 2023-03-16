namespace SocialNetwork.Services.Exceptions;

public class NotFoundException : Exception
{
    public NotFoundException(string entityName, string id) 
        : base($"Entity {entityName} with id = {id} not found")
    {
        
    }
    
    public NotFoundException(string entityName) 
        : base($"Entity {entityName} not found")
    {
        
    }
}