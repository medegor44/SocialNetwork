using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.RemoveFriendsCommand;

public record RemoveFriendsCommand(
    long UserId, 
    List<long> FriendsIds) : IRequest;
