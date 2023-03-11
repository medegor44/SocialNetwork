using Npgsql;
using SocialNetwork.Domain.Users;
using SocialNetwork.Domain.Users.Repositories;
using SocialNetwork.Domain.Users.ValueObjects;
using SocialNetwork.DataAccess.DbDto;

namespace SocialNetwork.DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly NpgsqlDataSource _source;
    private const string TableName = "Users";

    public UserRepository(NpgsqlDataSource source)
    {
        _source = source;
    }
    
    public async Task<Guid> CreateAsync(User user, Password password, CancellationToken cancellationToken)
    {
        var dto = new CreateUserDbDto
        {
            Id = Guid.NewGuid(),
            Age = user.Age.Value,
            Biography = user.Biography.Value,
            CityId = user.CityId,
            FirstName = user.FirstName.Value,
            SecondName = user.LastName.Value,
            RawPassword = password.Value,
        };
        
        var sql = $"""
INSERT INTO "{TableName}" ("Id", "FirstName", "SecondName", "Age", "Biography", "CityId", "Password") 
VALUES (
        @{nameof(CreateUserDbDto.Id)},
        @{nameof(CreateUserDbDto.FirstName)}, 
        @{nameof(CreateUserDbDto.SecondName)}, 
        @{nameof(CreateUserDbDto.Age)}, 
        @{nameof(CreateUserDbDto.Biography)}, 
        @{nameof(CreateUserDbDto.CityId)}, 
        CRYPT(@{nameof(CreateUserDbDto.RawPassword)}, GEN_SALT('md5'))) 
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
        command.Parameters.AddWithValue(nameof(dto.RawPassword), dto.RawPassword);

        await connection.OpenAsync(cancellationToken);

        if (await command.ExecuteScalarAsync(cancellationToken) is not Guid id)
            throw new Exception("Не удалось создать пользователя");
        
        return id;
    }

    public Task<User> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IReadOnlyCollection<User>> GetByFilterAsync(UserFilter filter)
    {
        throw new NotImplementedException();
    }

    public Task AddFriendAsync(Guid firstUserId, Guid secondUserId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveFriendAsync(Guid firstUserId, Guid secondUserId)
    {
        throw new NotImplementedException();
    }
}