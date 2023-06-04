using Npgsql;
using SocialNetwork.Domain.Users;
using SocialNetwork.Domain.Users.Repositories;
using SocialNetwork.DataAccess.DbDto;
using SocialNetwork.DataAccess.Exceptions;
using SocialNetwork.Domain.Users.ValueObjects;

namespace SocialNetwork.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly NpgsqlDataSource _source;
    private const string UsersTableName = "Users";
    private const string CitiesTableName = "Cities";

    public UserRepository(NpgsqlDataSource source)
    {
        _source = source;
    }
    
    public async Task<long> CreateAsync(User user, CancellationToken cancellationToken)
    {
        var dto = new UserDbDto
        {
            Age = user.Age.Value,
            Biography = user.Biography.Value,
            CityId = user.City.Id,
            CityName = user.City.Name,
            FirstName = user.FirstName.Value,
            SecondName = user.LastName.Value,
            Password = user.Password.HashedValue,
            Salt = user.Password.Salt
        };
        
        var sql = $"""
INSERT INTO "{UsersTableName}" ("FirstName", "SecondName", "Age", "Biography", "CityId", "Password", "Salt") 
VALUES (
        @{nameof(UserDbDto.FirstName)}, 
        @{nameof(UserDbDto.SecondName)}, 
        @{nameof(UserDbDto.Age)}, 
        @{nameof(UserDbDto.Biography)}, 
        @{nameof(UserDbDto.CityId)}, 
        @{nameof(UserDbDto.Password)},
        @{nameof(UserDbDto.Salt)})
RETURNING "Id"
""";
        await using var connection = _source.CreateConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue(nameof(dto.FirstName), dto.FirstName);
        command.Parameters.AddWithValue(nameof(dto.SecondName), dto.SecondName);
        command.Parameters.AddWithValue(nameof(dto.Age), dto.Age);
        command.Parameters.AddWithValue(nameof(dto.Biography), dto.Biography);
        command.Parameters.AddWithValue(nameof(dto.CityId), dto.CityId);
        command.Parameters.AddWithValue(nameof(dto.Password), dto.Password);
        command.Parameters.AddWithValue(nameof(dto.Salt), dto.Salt);

        await connection.OpenAsync(cancellationToken);

        if (await command.ExecuteScalarAsync(cancellationToken) is not long id)
            throw new InfrastructureException("Couldn't create user");
        
        return id;
    }

    public async Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken)
    {
        var dto = await GetUserDtoByIdAsync(id, cancellationToken);

        if (dto is null)
            return null;

        return new User(
            dto.Id, 
            new(dto.FirstName ?? string.Empty), 
            new(dto.SecondName ?? string.Empty), 
            new(dto.Age), 
            new(dto.Biography ?? string.Empty), 
            new(dto.CityId, dto.CityName ?? string.Empty),
            new(dto.Password ?? string.Empty, dto.Salt ?? string.Empty));
    }

    private async Task<UserDbDto?> GetUserDtoByIdAsync(long id, CancellationToken cancellationToken)
    {
        var sql = $"""
SELECT 
    u."Id", 
    u."FirstName", 
    u."SecondName", 
    u."Age", 
    u."Biography", 
    u."CityId", 
    u."Password", 
    u."Salt",
    c."Name" 
FROM
    "{UsersTableName}" u 
    JOIN "{CitiesTableName}" c ON u."CityId" = c."Id"
WHERE
    u."Id" = @{nameof(UserDbDto.Id)}
""";
        await using var connection = _source.CreateConnection();
        await using var query = new NpgsqlCommand(sql, connection);
        query.Parameters.AddWithValue(nameof(UserDbDto.Id), id);

        await connection.OpenAsync(cancellationToken);
        await using var reader = await query.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return null;

        var dto = new UserDbDto
        {
            Id = reader.GetInt64(0),
            FirstName = reader.GetString(1),
            SecondName = reader.GetString(2),
            Age = reader.GetInt32(3),
            Biography = reader.GetString(4),
            CityId = reader.GetInt64(5),
            Password = reader.GetString(6),
            Salt = reader.GetString(7),
            CityName = reader.GetString(8)
        };
        return dto;
    }

    public async Task<IReadOnlyCollection<User>> GetByFilterAsync(UserFilter filter, CancellationToken cancellationToken)
    {
        var filterDbDto = new UserFilterDbDto()
        {
            SecondName = $"{filter.SecondName?.ToLower() ?? ""}%",
            FirstName = $"{filter.FirstName?.ToLower() ?? ""}%",
            Limit = filter.Pagination.Limit,
            Offset = filter.Pagination.Offset
        };

        var conditions = new List<string> {"TRUE"};
        if (filter.FirstName is not null)
            conditions.Add($"""LOWER(u."FirstName") LIKE @{nameof(filterDbDto.FirstName)}""");
        if (filter.SecondName is not null)
            conditions.Add($"""LOWER(u."SecondName") LIKE @{nameof(filterDbDto.SecondName)}""");
        conditions.Add($"""u."Id" >= @{nameof(filterDbDto.Offset)} """);
        
        var queryCondition = string.Join(" AND ", conditions);

        var sql = $"""
SELECT
    u."Id", 
    u."FirstName", 
    u."SecondName", 
    u."Age", 
    u."Biography", 
    c."Name",
    c."Id"
FROM
    "{UsersTableName}" u JOIN "{CitiesTableName}" c ON u."CityId" = c."Id"
WHERE
    {queryCondition}
ORDER BY 
    u."Id" ASC 
LIMIT @{nameof(UserFilterDbDto.Limit)}
""";

        await using var connection = _source.CreateConnection();
        var query = new NpgsqlCommand(sql, connection);
        query.Parameters.AddWithValue(nameof(filterDbDto.FirstName), filterDbDto.FirstName);
        query.Parameters.AddWithValue(nameof(filterDbDto.SecondName), filterDbDto.SecondName);
        query.Parameters.AddWithValue(nameof(filterDbDto.Limit), filterDbDto.Limit);
        query.Parameters.AddWithValue(nameof(filterDbDto.Offset), filterDbDto.Offset);
        
        await connection.OpenAsync(cancellationToken);
        await using var reader = await query.ExecuteReaderAsync(cancellationToken);

        var users = new List<UserDbDto>(filterDbDto.Limit);
        while (await reader.ReadAsync(cancellationToken))
            users.Add(new()
            {
                Id = reader.GetInt64(0),
                FirstName = reader.GetString(1),
                SecondName = reader.GetString(2),
                Age = reader.GetInt32(3),
                Biography = reader.GetString(4),
                CityName = reader.GetString(5),
                CityId = reader.GetInt64(6)
            });

        return users.Select(x => new User(
                x.Id,
                new(x.FirstName),
                new(x.SecondName),
                new(x.Age),
                new(x.Biography),
                new(x.CityId, x.CityName)))
            .ToList();
    }
}