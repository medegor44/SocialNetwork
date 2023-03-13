using Npgsql;
using SocialNetwork.Domain.Users;
using SocialNetwork.Domain.Users.Repositories;
using SocialNetwork.DataAccess.DbDto;

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
    
    public async Task<Guid> CreateAsync(User user, CancellationToken cancellationToken)
    {
        var dto = new UserDbDto
        {
            Id = Guid.NewGuid(),
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
INSERT INTO "{UsersTableName}" ("Id", "FirstName", "SecondName", "Age", "Biography", "CityId", "Password", "Salt") 
VALUES (
        @{nameof(UserDbDto.Id)},
        @{nameof(UserDbDto.FirstName)}, 
        @{nameof(UserDbDto.SecondName)}, 
        @{nameof(UserDbDto.Age)}, 
        @{nameof(UserDbDto.Biography)}, 
        @{nameof(UserDbDto.CityId)}, 
        @{nameof(UserDbDto.Password)},
        @{nameof(UserDbDto.Salt)})
RETURNING "Id"
""";
        var connection = _source.CreateConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue(nameof(dto.Id), dto.Id);
        command.Parameters.AddWithValue(nameof(dto.FirstName), dto.FirstName);
        command.Parameters.AddWithValue(nameof(dto.SecondName), dto.SecondName);
        command.Parameters.AddWithValue(nameof(dto.Age), dto.Age);
        command.Parameters.AddWithValue(nameof(dto.Biography), dto.Biography);
        command.Parameters.AddWithValue(nameof(dto.CityId), dto.CityId);
        command.Parameters.AddWithValue(nameof(dto.Password), dto.Password);
        command.Parameters.AddWithValue(nameof(dto.Salt), dto.Salt);

        await connection.OpenAsync(cancellationToken);

        if (await command.ExecuteScalarAsync(cancellationToken) is not Guid id)
            throw new Exception("Не удалось создать пользователя");
        
        return id;
    }

    public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
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
    "{UsersTableName}" u JOIN "{CitiesTableName}" c ON u."CityId" = c."Id"
WHERE
    u."Id" = @{nameof(UserDbDto.Id)}
""";
        var connection = _source.CreateConnection();
        await using var query = new NpgsqlCommand(sql, connection);
        query.Parameters.AddWithValue(nameof(UserDbDto.Id), id);

        await connection.OpenAsync(cancellationToken);
        await using var reader = await query.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
            return null;

        var dto = new UserDbDto
        {
            Id = reader.GetGuid(0),
            FirstName = reader.GetString(1),
            SecondName = reader.GetString(2),
            Age = reader.GetInt32(3),
            Biography = reader.GetString(4),
            CityId = reader.GetGuid(5),
            Password = reader.GetString(6),
            Salt = reader.GetString(7),
            CityName = reader.GetString(8)
        };

        return new User(
            dto.Id, 
            new(dto.FirstName), 
            new(dto.SecondName), 
            new(dto.Age), 
            new(dto.Biography), 
            new(dto.CityId, dto.CityName),
            new(dto.Password, dto.Salt));
    }
}