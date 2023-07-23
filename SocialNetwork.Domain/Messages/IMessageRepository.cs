namespace SocialNetwork.Domain.Messages;

public interface IMessageRepository
{
    Task CreateAsync(Message message, CancellationToken cancellationToken);
}