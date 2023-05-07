using SocialNetwork.Services.Abstractions;

namespace SocialNetwork.Services.Queries.GetPostsFeed;

public record GetPostsFeedQuery(
    long UserId, 
    int Offset,
    int Limit) : IRequest<GetPostsFeedQueryResponse>;