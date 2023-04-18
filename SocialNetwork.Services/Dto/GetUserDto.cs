namespace SocialNetwork.Services.Dto;

public record GetUserDto(
    long Id, 
    string FirstName, 
    string SecondName, 
    int Age, 
    string Biography, 
    string City);