﻿using SocialNetwork.Domain.Users.ValueObjects;

namespace SocialNetwork.Domain.Users.Repositories;

public interface IUserRepository
{
    Task<long> CreateAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetByIdAsync(long id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<User>> GetByIdsAsync(IReadOnlyCollection<long> ids, CancellationToken cancellationToken);

    Task<IReadOnlyCollection<User>> GetByFilterAsync(UserFilter filter, CancellationToken cancellationToken);
    Task UpdateAsync(User oldUser, User updatedUser, CancellationToken cancellationToken);
}