using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.DeletePostCommand;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand>
{
    public Task HandleAsync(DeletePostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}