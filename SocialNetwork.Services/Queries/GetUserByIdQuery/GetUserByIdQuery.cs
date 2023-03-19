using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Queries.GetUserByIdQuery;

public record GetUserByIdQuery(Guid Id) : IRequest<GetUserByIdQueryResponse>;