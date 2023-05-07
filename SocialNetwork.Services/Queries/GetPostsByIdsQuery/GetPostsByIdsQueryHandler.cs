using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Queries;

public class GetPostsByIdsQueryHandler : IRequestHandler<GetPostsByIdQuery, GetPostsByIdQueryResponse>
{
    public Task<GetPostsByIdQueryResponse> HandleAsync(GetPostsByIdQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}