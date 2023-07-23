using SocialNetwork.Domain.Common;
using SocialNetwork.Domain.Dialogs.ValueObjects;

namespace SocialNetwork.Domain.Dialogs;

public class Dialog : Entity<DialogKey>
{
    public IReadOnlyCollection<Message> Messages { get; }

    public Dialog(long from, long to, IReadOnlyCollection<Message> messages)
    {
        Messages = messages;
        Id = new(from, to);
    }
}