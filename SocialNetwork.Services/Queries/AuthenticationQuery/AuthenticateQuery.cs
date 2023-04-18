using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Queries.AuthenticationQuery;

public record AuthenticateQuery(long Id, string RawPassword) : IRequest<AuthenticateQueryResponse>;