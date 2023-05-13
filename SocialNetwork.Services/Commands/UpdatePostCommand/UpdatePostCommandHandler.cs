using SocialNetwork.Domain.Posts.Repositories;
using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.UpdatePostCommand;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand>
{
    private readonly IPostRepository _repository;

    public UpdatePostCommandHandler(IPostRepository repository)
    {
        _repository = repository;
    }

    public Task HandleAsync(UpdatePostCommand request, CancellationToken cancellationToken) =>
        _repository.UpdateAsync(new(new(request.Text), request.Id), cancellationToken);
}