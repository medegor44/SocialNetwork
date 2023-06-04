using SocialNetwork.Domain.Friends.Entities;

namespace SocialNetwork.Domain.Friends.Repositories;

public interface IFriendsRepository
{
    public Task<User?> GetUserByIdAsync(long id, CancellationToken cancellationToken);
    public Task<IReadOnlyCollection<User>> GetUsersByIdsAsync(IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken);

    public Task<IReadOnlyCollection<Friend>> GetFriendsByIdsAsync(IReadOnlyCollection<long> ids,
        CancellationToken cancellationToken);

    public Task UpdateUserAsync(User oldUser, User updatedUser, CancellationToken cancellationToken);
}