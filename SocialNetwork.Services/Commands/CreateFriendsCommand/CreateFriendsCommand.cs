using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.CreateFriendsCommand;

public record CreateFriendsCommand(
    long UserId, 
    List<long> FriendsIds) : IRequest;