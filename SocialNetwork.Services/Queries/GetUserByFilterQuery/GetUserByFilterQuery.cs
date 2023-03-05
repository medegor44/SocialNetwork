using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Queries.GetUserByFilterQuery;

public record GetUserByFilterQuery(string FirstName, string SecondName) : IRequest<GetUserByFilterQueryResponse>;
