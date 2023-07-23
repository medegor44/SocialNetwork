namespace SocialNetwork.Controllers.Dialogs;

public class GetMessageDto
{
    public long From { get; set; }
    public long To { get; set; }
    public string Text { get; set; }
}