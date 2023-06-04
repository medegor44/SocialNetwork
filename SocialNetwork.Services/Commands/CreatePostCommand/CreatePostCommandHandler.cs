using SocialNetwork.Domain.Posts.Repositories;
using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Commands.CreatePostCommand;

public class CreatePostCommandHandler : IRequestHandler<CreatePostCommand, CreatePostCommandResponse>
{
    private readonly IPostsRepository _repository;

    public CreatePostCommandHandler(IPostsRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<CreatePostCommandResponse> HandleAsync(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _repository.CreateAsync(new(new(request.Text), request.UserId), cancellationToken);

        return new CreatePostCommandResponse(post.Id);
    }
}