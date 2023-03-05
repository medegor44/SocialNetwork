namespace SocialNetwork.Controllers.Requests;

public record RegisterRequestDto(
    string FirstName, 
    string SecondName, 
    int Age, 
    string Biography, 
    string City, 
    string Password);