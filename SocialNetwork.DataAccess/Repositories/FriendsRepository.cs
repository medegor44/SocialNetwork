using Npgsql;
using SocialNetwork.DataAccess.DbDto;
using SocialNetwork.DataAccess.Repositories.Posts.Cache;
using SocialNetwork.Domain.Friends;
using SocialNetwork.Domain.Friends.Entities;
using SocialNetwork.Domain.Friends.Repositories;

namespace SocialNetwork.DataAccess.Repositories;

public class FriendsRepository : IFriendsRepository
{
    private readonly NpgsqlDataSource _source;
    private readonly IPostsCacheInvalidator _invalidator;
    private const string FriendsTableName = "Friends";
    private const string UsersTableName = "Users";

    public FriendsRepository(NpgsqlDataSource source, IPostsCacheInvalidator invalidator)
    {
        _source = source;
        _invalidator = invalidator;
    }

    public async Task<User?> GetUserByIdAsync(long id, CancellationToken cancellationToken) =>
        (await GetUsersByIdsAsync(new[] {id}, cancellationToken)).FirstOrDefault();

    public async Task<IReadOnlyCollection<User>> GetUsersByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken)
    {
        var sql = $"""
SELECT
    u."Id",
    f."FriendId"
FROM
    "{FriendsTableName}" f RIGHT JOIN "{UsersTableName}" u ON f."UserId" = u."Id"
WHERE
    u."Id" = ANY(ARRAY[@Ids]::INT8[])
""";

        await using var connection = _source.CreateConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("Ids", ids);

        await connection.OpenAsync(cancellationToken);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var dtos = new List<FriendDbDto>();
        while (await reader.ReadAsync(cancellationToken))
        {
            var friendId = await reader.GetFieldValueAsync<long>(0, cancellationToken);
            var friendOdFriendId = await reader.GetFieldValueAsync<long?>(1, cancellationToken);
            
            dtos.Add(new()
            {
                UserId = friendId,
                FriendId = friendOdFriendId
            });
        }

        return dtos
            .GroupBy(x => x.UserId)
            .Select(x => new User(x.Key, x
                .Where(friend => friend.FriendId.HasValue)
                .Select(friend => new Friend(friend.FriendId!.Value))
                .ToList()))
            .ToList();
    }

    public async Task<IReadOnlyCollection<Friend>> GetFriendsByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken)
    {
        var sql = $"""
SELECT
    u."Id"
FROM
    "{UsersTableName}" u
WHERE
    u."Id" = ANY(ARRAY[@Ids]::INT8[])
""";

        await using var connection = _source.CreateConnection();
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("Ids", ids);

        await connection.OpenAsync(cancellationToken);
        
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var selectedIds = new List<long>();
        while (await reader.ReadAsync(cancellationToken))
            selectedIds.Add(await reader.GetFieldValueAsync<long>(0, cancellationToken));

        return selectedIds
            .Select(x => new Friend(x))
            .ToList();
    }

    private async Task CreateFriendAsync(IReadOnlyCollection<Friend> newFriends, long userId, NpgsqlConnection connection,
        CancellationToken cancellationToken)
    {
        if (newFriends is {Count: 0} or null)
            return;
        
        var values = string.Join(",", newFriends
            .Select(x => $"({userId}, {x.Id}), ({x.Id}, {userId})"));

        var sql = $"""
INSERT INTO "{FriendsTableName}"("UserId", "FriendId") 
VALUES {values}
""";

        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync(cancellationToken);
        
        await _invalidator.InvalidateAsync(newFriends.Select(x => x.Id).Concat(new[]{userId}).ToList());
    }

    private async Task RemoveFriendsAsync(IReadOnlyCollection<Friend> removedFriends, long userId,
        NpgsqlConnection connection, CancellationToken cancellationToken)
    {
        if (removedFriends is {Count:0} or null)
            return;
        
        var sql = $"""
DELETE FROM "{FriendsTableName}"
WHERE 
    "UserId" = @UserId AND "FriendId" = ANY(ARRAY[@FriendIds]::INT8[]) OR 
    "UserId" = ANY(ARRAY[@FriendIds]::INT8[]) AND "FriendId" = @UserId
""";

        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddWithValue("UserId", userId);
        command.Parameters.AddWithValue("FriendIds", removedFriends.Select(x => x.Id).ToList());

        await command.ExecuteNonQueryAsync(cancellationToken);
        
        await _invalidator.InvalidateAsync(removedFriends.Select(x => x.Id).Concat(new[]{userId}).ToList());
    }

    public async Task UpdateUserAsync(User oldUser, User updatedUser, CancellationToken cancellationToken)
    {
        var recentlyAddedFriends = updatedUser.Friends.Except(oldUser.Friends).ToList();
        var recentlyRemovedFriends = oldUser.Friends.Except(updatedUser.Friends).ToList();

        await using var connection = _source.CreateConnection();
        await connection.OpenAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        try
        {
            await CreateFriendAsync(recentlyAddedFriends, updatedUser.Id, connection, cancellationToken);
            await RemoveFriendsAsync(recentlyRemovedFriends, updatedUser.Id, connection, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}