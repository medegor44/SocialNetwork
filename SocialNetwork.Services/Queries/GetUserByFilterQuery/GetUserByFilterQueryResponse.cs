using SocialNetwork.Services.Dto;

namespace SocialNetwork.Services.Queries.GetUserByFilterQuery;

public record GetUserByFilterQueryResponse(List<GetUserDto> Users);