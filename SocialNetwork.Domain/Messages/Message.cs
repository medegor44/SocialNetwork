using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Messages;

public sealed class Message : Entity<long>
{
    public Message(long from, long to, string text)
    {
        From = from;
        To = to;
        Text = text;
        CreateDate = DateTime.UtcNow;
    }

    public Message(long id, long from, long to, string text, DateTime createDate)
        : this(from, to, text)
    {
        Id = id;
        CreateDate = createDate;
    }

    public override long Id { get; protected set; }

    public long From { get; }
    public long To { get; }
    public DateTime CreateDate { get; }
    public string Text { get; }
}