using SocialNetwork.Domain.Dialogs.ValueObjects;

namespace SocialNetwork.Domain.Dialogs;

public interface IDialogsRepository
{
    Task<Dialog> GetAsync(DialogKey key, CancellationToken cancellationToken);
}