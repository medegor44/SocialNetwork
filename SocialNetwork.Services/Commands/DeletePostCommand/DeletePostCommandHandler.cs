using SocialNetwork.Domain.Posts.Repositories;
using SocialNetwork.Domain.Posts.ValueObjects;
using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.DeletePostCommand;

public class DeletePostCommandHandler : IRequestHandler<DeletePostCommand>
{
    private readonly IPostsRepository _repository;

    public DeletePostCommandHandler(IPostsRepository repository)
    {
        _repository = repository;
    }
    
    public Task HandleAsync(DeletePostCommand request, CancellationToken cancellationToken) =>
        _repository.DeleteAsync(new(request.Id, new Text("empty"), request.UserId, DateTimeOffset.UtcNow), cancellationToken);
}