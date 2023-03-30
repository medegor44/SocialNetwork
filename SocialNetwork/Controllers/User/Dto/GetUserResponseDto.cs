namespace SocialNetwork.Controllers.Requests;

public record GetUserResponseDto(
    long Id, 
    string FirstName, 
    string SecondName, 
    int Age, 
    string Biography, 
    string City);