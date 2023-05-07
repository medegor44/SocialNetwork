using SocialNetwork.Domain.Users.ValueObjects;

namespace SocialNetwork.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<long> CreateAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<User>> GetByFilterAsync(UserFilter filter, CancellationToken cancellationToken);
}