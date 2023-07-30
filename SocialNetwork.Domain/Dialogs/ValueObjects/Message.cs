using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Dialogs.ValueObjects;

public class Message : ValueObject
{
    public Message(long from, long to, string text)
    {
        From = from;
        To = to;
        Text = text;
    }

    public long From { get; }
    public long To { get; }
    public string Text { get; }
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return From;
        yield return To;
        yield return Text;
    }
}