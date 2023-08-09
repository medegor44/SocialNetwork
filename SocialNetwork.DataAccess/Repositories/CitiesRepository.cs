using Npgsql;
using SocialNetwork.DataAccess.DbDto;
using SocialNetwork.Domain.Dictionaries;
using SocialNetwork.Domain.Dictionaries.Entities;
using SocialNetwork.Postgres;

namespace SocialNetwork.DataAccess.Repositories;

public class CitiesRepository : ICitiesRepository
{
    private readonly IConnectionFactory _connectionFactory;
    private const string TableName = "Cities";
    
    public CitiesRepository(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }
    
    public async Task<DictionaryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var sql = $"""
SELECT
    "Id",
    "Name"
FROM
    "{TableName}"
WHERE
    "Id" = @{nameof(DictionaryItemDbDto.Id)}
""";

        await using var connection = _connectionFactory.GetSync();
        await using var query = new NpgsqlCommand(sql, connection);

        query.Parameters.AddWithValue(nameof(DictionaryItemDbDto.Id), id);
        
        await connection.OpenAsync(cancellationToken);
        await using var reader = await query.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return null;

        var dto = new DictionaryItemDbDto()
        {
            Id = reader.GetInt64(0),
            Name = reader.GetString(1)
        };

        return new(dto.Id, dto.Name);
    }

    public async Task<List<DictionaryItem>> GetByName(string name, CancellationToken cancellationToken)
    {
        var sql = $"""
SELECT
    "Id",
    "Name"
FROM
    "{TableName}"
WHERE
    "Name" ILIKE @{nameof(DictionaryItemDbDto.Name)}
""";

        await using var connection = _connectionFactory.GetSync();
        await using var query = new NpgsqlCommand(sql, connection);

        query.Parameters.AddWithValue(nameof(DictionaryItemDbDto.Name), name);
        
        await connection.OpenAsync(cancellationToken);
        await using var reader = await query.ExecuteReaderAsync(cancellationToken);
        var dtos = new List<DictionaryItemDbDto>();

        while (await reader.ReadAsync(cancellationToken))
            dtos.Add(new DictionaryItemDbDto()
            {
                Id = reader.GetInt64(0),
                Name = reader.GetString(1)
            });

        return dtos.Select(dto => new DictionaryItem(dto.Id, dto.Name)).ToList();
    }
}