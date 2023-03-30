namespace SocialNetwork.DataAccess.DbDto;

public class UserFilterDbDto
{
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public int Limit { get; set; }
    public int Offset { get; set; }
}