namespace SocialNetwork.Services.Events;

public interface IPostCreatedNotificationSender : IDisposable
{
    void Send(PostCreatedNotification notification, long recipientId);
}