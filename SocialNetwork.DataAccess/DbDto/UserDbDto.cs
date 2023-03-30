namespace SocialNetwork.DataAccess.DbDto;

public class UserDbDto
{
    public long Id { get; set; }
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public int Age { get; set; }
    public string? Biography { get; set; }
    public long CityId { get; set; }
    public string? CityName { get; set; }
    public string? Password { get; set; }
    public string? Salt { get; set; }
}