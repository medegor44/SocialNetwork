using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.UpdatePostCommand;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand>
{
    public Task HandleAsync(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}