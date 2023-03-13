using Npgsql;
using SocialNetwork.DataAccess.DbDto;
using SocialNetwork.Domain.Dictionaries;
using SocialNetwork.Domain.Dictionaries.Entities;

namespace SocialNetwork.DataAccess.Repositories;

public class CitiesRepository : ICitiesRepository
{
    private readonly NpgsqlDataSource _dataSource;
    private const string TableName = "Cities";
    
    public CitiesRepository(NpgsqlDataSource dataSource)
    {
        _dataSource = dataSource;
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

        var connection = _dataSource.CreateConnection();
        await using var query = new NpgsqlCommand(sql, connection);

        query.Parameters.AddWithValue(nameof(DictionaryItemDbDto.Id), id);
        
        await connection.OpenAsync(cancellationToken);
        await using var reader = await query.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return null;

        var dto = new DictionaryItemDbDto()
        {
            Id = reader.GetGuid(0),
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

        var connection = _dataSource.CreateConnection();
        await using var query = new NpgsqlCommand(sql, connection);

        query.Parameters.AddWithValue(nameof(DictionaryItemDbDto.Id), name);
        
        await connection.OpenAsync(cancellationToken);
        await using var reader = await query.ExecuteReaderAsync(cancellationToken);
        var dtos = new List<DictionaryItemDbDto>();

        while (await reader.ReadAsync(cancellationToken))
            dtos.Add(new DictionaryItemDbDto()
            {
                Id = reader.GetGuid(0),
                Name = reader.GetString(1)
            });

        return dtos.Select(dto => new DictionaryItem(dto.Id, dto.Name)).ToList();
    }
}