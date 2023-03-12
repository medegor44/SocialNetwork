using SocialNetwork.Domain.Users.Repositories;
using SocialNetwork.Services.Abstractions;
using SocialNetwork.Services.Dto;

namespace SocialNetwork.Services.Queries.GetUserByFilterQuery;

public class GetUserByFilterQueryHandler : IRequestHandler<GetUserByFilterQuery, GetUserByFilterQueryResponse>
{
    private readonly IUserRepository _repository;

    public GetUserByFilterQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public Task<GetUserByFilterQueryResponse> HandleAsync(GetUserByFilterQuery request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}