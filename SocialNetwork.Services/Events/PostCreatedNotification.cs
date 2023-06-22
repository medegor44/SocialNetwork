namespace SocialNetwork.Services.Events;

public record PostCreatedNotification(long PostId, string PostText, long AuthorUserId, long RecipientId);
