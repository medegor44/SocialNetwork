using SocialNetwork.Domain.Posts.ValueObjects;

namespace SocialNetwork.Domain.Posts.Repositories;

public interface IPostRepository
{
    Task<Post> CreateAsync(Post post);
    Task UpdateAsync(Post updatedPost);
    Task DeleteAsync(Guid id);
    Task<Post> GetById(Guid id);
    Task<IReadOnlyCollection<Post>> GetByFilter(PostFilter filter);
}