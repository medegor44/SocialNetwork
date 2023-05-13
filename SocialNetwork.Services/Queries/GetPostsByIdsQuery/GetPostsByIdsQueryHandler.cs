using SocialNetwork.Domain.Posts;
using SocialNetwork.Domain.Posts.Repositories;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Exceptions;

namespace SocialNetwork.Services.Queries;

public class GetPostsByIdsQueryHandler : IRequestHandler<GetPostsByIdQuery, GetPostsByIdQueryResponse>
{
    private readonly IPostRepository _repository;

    public GetPostsByIdsQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<GetPostsByIdQueryResponse> HandleAsync(GetPostsByIdQuery request, CancellationToken cancellationToken)
    {
        var post = (await _repository.GetByIdsAsync(new[] {request.Id}, cancellationToken))
            .FirstOrDefault();

        if (post is null)
            throw new NotFoundException(nameof(Post), request.Id.ToString());

        return new(new(post.Id, post.Text.Value, post.UserId));
    }
}