using SocialNetwork.Domain.Users.ValueObjects;

namespace SocialNetwork.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<Guid> CreateAsync(User user);
    Task<User> GetByIdAsync(Guid id);
    Task<IReadOnlyCollection<User>> GetByFilterAsync(UserFilter filter);
    Task AddFriendAsync(Guid firstUserId, Guid secondUserId);
    Task RemoveFriendAsync(Guid firstUserId, Guid secondUserId);
}