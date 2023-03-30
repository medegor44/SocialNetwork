using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Queries.GetUserByIdQuery;

public record GetUserByIdQuery(long Id) : IRequest<GetUserByIdQueryResponse>;