using SocialNetwork.Domain.Dictionaries.Entities;

namespace SocialNetwork.Domain.Dictionaries;

public interface IDictionaryItemsRepository
{
    Task<DictionaryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<DictionaryItem>> GetByName(string name, CancellationToken cancellationToken);
}