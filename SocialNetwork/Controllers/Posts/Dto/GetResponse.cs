namespace SocialNetwork.Controllers.Dto;

public record GetResponse(
    long Id, 
    string Text, 
    long AuthorUserId);