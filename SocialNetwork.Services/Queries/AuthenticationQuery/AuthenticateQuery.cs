using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Queries.AuthenticationQuery;

public record AuthenticateQuery(Guid Id, string RawPassword) : IRequest<AuthenticateQueryResponse>;