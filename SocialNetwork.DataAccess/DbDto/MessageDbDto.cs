namespace SocialNetwork.DataAccess.DbDto;

public class MessageDbDto
{
    public long From { get; set; }
    public long To { get; set; }
    public DateTime CreateDate { get; set; }
    public long Id { get; set; }
    public string Text { get; set; }
}