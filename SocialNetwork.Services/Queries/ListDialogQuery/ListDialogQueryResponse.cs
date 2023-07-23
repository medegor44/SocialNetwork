namespace SocialNetwork.Services.Queries.ListDialogQuery;

public record MessageDto(long From, long To, string Text);

public record ListDialogQueryResponse(List<MessageDto> Mmessages);