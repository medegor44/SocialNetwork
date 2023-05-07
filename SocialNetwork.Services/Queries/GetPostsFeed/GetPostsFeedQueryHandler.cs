using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Queries.GetPostsFeed;

public class GetPostsFeedQueryHandler : IRequestHandler<GetPostsFeedQuery, GetPostsFeedQueryResponse>
{
    public Task<GetPostsFeedQueryResponse> HandleAsync(GetPostsFeedQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}