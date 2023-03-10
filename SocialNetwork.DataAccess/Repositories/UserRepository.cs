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
    
    public async Task<Guid> CreateAsync(User user, CancellationToken cancellationToken)
    {
        var dto = new CreateUserDbDto
        {
            Id = Guid.NewGuid(),
            Age = 18,
            Biography = "xxx",
            CityId = Guid.Empty,
            FirstName = "Abc",
            SecondName = "Def",
            Password = "1234",
            Salt = "1"
        };
        
        var sql = $"""
INSERT INTO "{TableName}" ("Id", "FirstName", "SecondName", "Age", "Biography", "CityId", "Password", "Salt") 
VALUES (
        @{nameof(CreateUserDbDto.Id)},
        @{nameof(CreateUserDbDto.FirstName)}, 
        @{nameof(CreateUserDbDto.SecondName)}, 
        @{nameof(CreateUserDbDto.Age)}, 
        @{nameof(CreateUserDbDto.Biography)}, 
        @{nameof(CreateUserDbDto.CityId)}, 
        @{nameof(CreateUserDbDto.Password)}, 
        @{nameof(CreateUserDbDto.Salt)})
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

        if (!(await command.ExecuteScalarAsync(cancellationToken) is Guid id))
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