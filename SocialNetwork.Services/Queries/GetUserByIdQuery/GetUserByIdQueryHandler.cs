using SocialNetwork.Domain.Users.Repositories;
using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Queries.GetUserByIdQuery;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, GetUserByIdQueryResponse>
{
    private readonly IUserRepository _repository;

    public GetUserByIdQueryHandler(IUserRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<GetUserByIdQueryResponse> HandleAsync(GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _repository.GetByIdAsync(request.Id);

        return new(new(
            user.Id, 
            user.FirstName.Value, 
            user.LastName.Value, 
            user.Age.Value, 
            user.Biography.Value,
            ""));
    }
}