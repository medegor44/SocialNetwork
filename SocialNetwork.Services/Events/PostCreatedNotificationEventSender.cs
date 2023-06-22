using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using SocialNetwork.Services.Events.Options;

namespace SocialNetwork.Services.Events;

public class PostCreatedNotificationEventSender : IPostCreatedNotificationSender
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private const string ExchangeName = "NotificationsExchange";
    private const string PostNotificationRoutingKey = nameof(PostNotificationRoutingKey);

    public PostCreatedNotificationEventSender(IOptions<ConnectionOptions> connectionOptions, IOptions<FeedNotificationsOptions> feedNotificationsOptions)
    {
        _connection = new ConnectionFactory()
        {
            HostName = connectionOptions.Value.HostName,
            UserName = connectionOptions.Value.UserName,
            Password = connectionOptions.Value.Password,
            Port = connectionOptions.Value.Port
        }.CreateConnection();
        
        _channel = _connection.CreateModel();
        
        _channel.ExchangeDeclare(ExchangeName, type: ExchangeType.Direct);
        
        var queueDeclareOk = _channel.QueueDeclare(
            queue: feedNotificationsOptions.Value.Name,
            durable: feedNotificationsOptions.Value.Durable,
            exclusive: feedNotificationsOptions.Value.Exclusive,
            autoDelete: feedNotificationsOptions.Value.AutoDelete
        );
        
        _channel.QueueBind(queueDeclareOk.QueueName, ExchangeName, PostNotificationRoutingKey);
    }
    
    public void Send(PostCreatedNotification notification, long recipientId)
    {
        var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(notification));
        _channel.BasicPublish(ExchangeName, body: bytes, routingKey: PostNotificationRoutingKey);
    }

    public void Dispose()
    {
        _connection.Dispose();
        _channel.Dispose();
    }
}