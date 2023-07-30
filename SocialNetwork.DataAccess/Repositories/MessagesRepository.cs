using MessagingService.Proto;
using SocialNetwork.Domain.Messages;
using Message = SocialNetwork.Domain.Messages.Message;

namespace SocialNetwork.DataAccess.Repositories;

public class MessagesRepository : IMessageRepository
{
    private readonly MessagingService.Proto.MessagingService.MessagingServiceClient _client;

    public MessagesRepository(MessagingService.Proto.MessagingService.MessagingServiceClient client)
    {
        _client = client;
    }
    
    public async Task CreateAsync(Message message, CancellationToken cancellationToken)
    {
        await _client.SendAsync(new SendRequest()
        {
            From = message.From,
            To = message.To,
            Text = message.Text
        }, cancellationToken: cancellationToken);
    }
}