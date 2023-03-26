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
    
    public async Task<GetUserByFilterQueryResponse> HandleAsync(GetUserByFilterQuery request,
        CancellationToken cancellationToken)
    {
        var users = await _repository.GetByFilterAsync(new(request.FirstName, request.SecondName), cancellationToken);

        return new GetUserByFilterQueryResponse(users.Select(user => new GetUserDto(
                user.Id,
                user.FirstName.Value,
                user.LastName.Value,
                user.Age.Value,
                user.Biography.Value,
                user.City.Name))
            .ToList());
    }
}