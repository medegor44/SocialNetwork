namespace SocialNetwork.Controllers.Requests;

public record GetUserResponseDto(Guid Id, string FirstName, string SecondName, int Age, string Biography, string City);