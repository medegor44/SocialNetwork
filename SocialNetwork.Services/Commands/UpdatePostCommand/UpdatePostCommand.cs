using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.UpdatePostCommand;

public record UpdatePostCommand(long Id, string Text) : IRequest;
