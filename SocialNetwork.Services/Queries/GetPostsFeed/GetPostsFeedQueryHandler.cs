using SocialNetwork.Domain.Posts.Repositories;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Dto;

namespace SocialNetwork.Services.Queries.GetPostsFeed;

public class GetPostsFeedQueryHandler : IRequestHandler<GetPostsFeedQuery, GetPostsFeedQueryResponse>
{
    private readonly IPostRepository _repository;

    public GetPostsFeedQueryHandler(IPostRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<GetPostsFeedQueryResponse> HandleAsync(GetPostsFeedQuery request, CancellationToken cancellationToken)
    {
        var feed = await _repository.GetFeedAsync(new(request.UserId, request.Offset, request.Limit), cancellationToken);

        return new(feed.PostsOnPage
            .Select(p => new PostDto(
                p.Id, 
                p.Text.Value, 
                p.UserId))
            .ToList());
    }
}