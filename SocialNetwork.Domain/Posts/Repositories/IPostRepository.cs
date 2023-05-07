namespace SocialNetwork.Domain.Posts.Repositories;

public interface IPostRepository
{
    Task<Post> CreateAsync(Post post, CancellationToken cancellationToken);
    Task UpdateAsync(Post updatedPost, CancellationToken cancellationToken);
    Task DeleteAsync(long id, CancellationToken cancellationToken);
    Task<IReadOnlyCollection<Post>> GetByIdsAsync(IReadOnlyCollection<long> id, CancellationToken cancellationToken);
}