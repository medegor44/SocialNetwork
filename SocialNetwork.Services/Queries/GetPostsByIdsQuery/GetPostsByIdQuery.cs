using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Queries;

public record GetPostsByIdQuery(long Id) : IRequest<GetPostsByIdQueryResponse>;