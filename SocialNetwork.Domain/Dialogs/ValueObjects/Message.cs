using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Dialogs.ValueObjects;

public class Message : ValueObject
{
    public Message(long from, long to, string text, DateTime createDate)
    {
        From = from;
        To = to;
        Text = text;
        CreateDate = createDate;
    }

    public long From { get; }
    public long To { get; }
    public string Text { get; }
    public DateTime CreateDate { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return From;
        yield return To;
        yield return Text;
        yield return CreateDate;
    }
}