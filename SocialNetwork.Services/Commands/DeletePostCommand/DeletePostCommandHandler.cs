using SocialNetwork.Domain.Posts.Repositories;
using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.DeletePostCommand;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand>
{
    private readonly IPostRepository _repository;

    public DeletePostCommandHandler(IPostRepository repository)
    {
        _repository = repository;
    }
    
    public Task HandleAsync(DeletePostCommand request, CancellationToken cancellationToken) =>
        _repository.DeleteAsync(request.Id, cancellationToken);
}