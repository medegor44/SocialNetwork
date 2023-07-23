using Npgsql;
using SocialNetwork.DataAccess.DbDto;
using SocialNetwork.Domain.Dialogs;
using SocialNetwork.Domain.Dialogs.ValueObjects;

namespace SocialNetwork.DataAccess.Repositories;

public class DialogsRepository : IDialogsRepository
{
    private readonly NpgsqlDataSource _source;

    public DialogsRepository(NpgsqlDataSource source)
    {
        _source = source;
    }
    
    private async Task<IReadOnlyCollection<Message>> GetMessagesAsync(long from, long to, CancellationToken cancellationToken)
    {
        var param = new 
        {
            From = from,
            To = to
        };

        var sql = $"""
WITH  
first AS (
    SELECT
        "From",
        "To",
        "CreateDate",
        "Text"
    FROM
        "Dialogs"
    WHERE
        "From" = @{nameof(param.From)} AND
        "To" = @{nameof(param.To)}
),
second AS (
    SELECT
        "From",
        "To",
        "CreateDate",
        "Text"
    FROM
        "Dialogs"
    WHERE
        "From" = @{nameof(param.To)} AND
        "To" = @{nameof(param.From)}
)
SELECT
    m."From",
    m."To",
    m."CreateDate",
    m."Text"
FROM
    (SELECT * FROM first UNION SELECT * FROM second) AS m
ORDER BY
    m."CreateDate"
""";

        await using var connection = _source.CreateConnection();
        await using var command = new NpgsqlCommand(sql, connection)
        {
            Parameters =
            {
                new(nameof(param.From), param.From),
                new(nameof(param.To), param.To)
            }
        };

        await connection.OpenAsync(cancellationToken);
        var reader = await command.ExecuteReaderAsync(cancellationToken);
        var messages = new List<MessageDbDto>();
        
        while (await reader.ReadAsync(cancellationToken))
        {
            var dto = new MessageDbDto()
            {
                From = await reader.GetFieldValueAsync<long>(0, cancellationToken),
                To = await reader.GetFieldValueAsync<long>(1, cancellationToken),
                CreateDate = await reader.GetFieldValueAsync<DateTime>(2, cancellationToken),
                Text = await reader.GetFieldValueAsync<string>(3, cancellationToken)
            };
            
            messages.Add(dto);
        }

        return messages.Select(x => new Message(x.From, x.To, x.Text, x.CreateDate)).ToList();
    }
    
    public async Task<Dialog> GetAsync(DialogKey key, CancellationToken cancellationToken)
    {
        var messages = await GetMessagesAsync(key.From, key.To, cancellationToken);

        return new(key.From, key.To, messages);
    }
}