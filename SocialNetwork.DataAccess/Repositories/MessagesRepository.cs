using Npgsql;
using SocialNetwork.DataAccess.DbDto;
using SocialNetwork.Domain.Messages;

namespace SocialNetwork.DataAccess.Repositories;

public class MessagesRepository : IMessageRepository
{
    private readonly NpgsqlDataSource _source;

    public MessagesRepository(NpgsqlDataSource source)
    {
        _source = source;
    }
    
    public async Task CreateAsync(Message message, CancellationToken cancellationToken)
    {
        var sql = $"""
INSERT INTO "Dialogs"("From", "To", "CreateDate", "Text")
VALUES (
    @{nameof(MessageDbDto.From)},
    @{nameof(MessageDbDto.To)},
    @{nameof(MessageDbDto.CreateDate)},
    @{nameof(MessageDbDto.Text)}
)
""";

        var dto = new MessageDbDto()
        {
            From = message.From,
            To = message.To,
            CreateDate = message.CreateDate,
            Text = message.Text
        };

        await using var connection = _source.CreateConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        
        command.Parameters.AddWithValue(nameof(dto.From), dto.From);
        command.Parameters.AddWithValue(nameof(dto.To), dto.To);
        command.Parameters.AddWithValue(nameof(dto.CreateDate), dto.CreateDate);
        command.Parameters.AddWithValue(nameof(dto.Text), dto.Text);
        
        await connection.OpenAsync(cancellationToken);

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}