namespace SocialNetwork.Services.Dto;

public record GetUserDto(
    Guid Id, 
    string FirstName, 
    string SecondName, 
    int Age, 
    string Biography, 
    string City);