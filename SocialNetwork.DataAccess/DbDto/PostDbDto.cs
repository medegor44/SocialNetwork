namespace SocialNetwork.DataAccess.DbDto;

public class PostDbDto
{
    public long UserId { get; set; }
    public long Id { get; set; }
    public string Text { get; set; }
    public DateTimeOffset CreateDate { get; set; }
}