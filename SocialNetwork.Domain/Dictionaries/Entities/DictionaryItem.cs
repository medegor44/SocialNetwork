using SocialNetwork.Domain.Common;

namespace SocialNetwork.Domain.Dictionaries.Entities;

public sealed class DictionaryItem : Entity<Guid>
{
    public DictionaryItem(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
    
    public string Name { get; }
}