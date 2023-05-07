using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.CreatePostCommand;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, CreatePostCommandResponse>
{
    public Task<CreatePostCommandResponse> HandleAsync(CreatePostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}