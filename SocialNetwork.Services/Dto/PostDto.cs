namespace SocialNetwork.Services.Dto;

public record PostDto(
    long Id, 
    string Text, 
    long AuthorUserId);
