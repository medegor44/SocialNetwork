namespace SocialNetwork.DataAccess.DbDto;

public class UserDbDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public int Age { get; set; }
    public string Biography { get; set; }
    public Guid CityId { get; set; }
    public string Password { get; set; }
    public string Salt { get; set; }
}