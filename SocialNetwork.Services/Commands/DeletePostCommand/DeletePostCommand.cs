using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.DeletePostCommand;

public record DeletePostCommand(long Id, long UserId) : IRequest;