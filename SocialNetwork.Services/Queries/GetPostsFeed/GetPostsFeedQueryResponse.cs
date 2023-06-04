using SocialNetwork.Services.Dto;

namespace SocialNetwork.Services.Queries.GetPostsFeed;

public record GetPostsFeedQueryResponse(List<PostDto> Posts);
