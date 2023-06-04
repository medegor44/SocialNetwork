using SocialNetwork.Domain.Posts;
using SocialNetwork.Domain.Posts.Repositories;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Exceptions;

namespace SocialNetwork.Services.Commands.UpdatePostCommand;

public class UpdatePostCommandHandler : IRequestHandler<UpdatePostCommand>
{
    private readonly IPostsRepository _repository;

    public UpdatePostCommandHandler(IPostsRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(UpdatePostCommand request, CancellationToken cancellationToken)
    {
        var post = (await _repository.GetByIdsAsync(new[]{request.Id}, cancellationToken)).FirstOrDefault();

        if (post is null)
            throw new NotFoundException(nameof(Post), request.Id.ToString());
        
        post.UpdateText(new(request.Text));
        
        await _repository.UpdateAsync(post, cancellationToken);
    }
}