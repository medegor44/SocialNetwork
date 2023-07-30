using SocialNetwork.Domain.Dialogs;
using SocialNetwork.Domain.Dialogs.ValueObjects;

namespace SocialNetwork.DataAccess.Repositories;

public class DialogsRepository : IDialogsRepository
{
    private readonly MessagingService.Proto.MessagingService.MessagingServiceClient _client;

    public DialogsRepository(MessagingService.Proto.MessagingService.MessagingServiceClient client)
    {
        _client = client;
    }
    
    public async Task<Dialog> GetAsync(DialogKey key, CancellationToken cancellationToken)
    {
        var response = await _client.ListDialogAsync(new()
        {
            From = key.From,
            To = key.To
        }, cancellationToken: cancellationToken);

        return new(
            key.From, 
            key.To, 
            response
                .Messages
                .Select(x => new Message(x.From, x.To, x.Text)).ToList()
            );
    }
}