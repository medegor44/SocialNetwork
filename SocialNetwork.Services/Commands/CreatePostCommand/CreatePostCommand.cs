using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.CreatePostCommand;

public record CreatePostCommand(string Text, long UserId) : IRequest<CreatePostCommandResponse>;