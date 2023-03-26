using SocialNetwork.Domain.Users.ValueObjects;

namespace SocialNetwork.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<Guid> CreateAsync(User user, CancellationToken cancellationToken);
    Task<User> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<User>> GetByFilterAsync(UserFilter filter, CancellationToken cancellationToken);
}