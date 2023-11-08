using CatalogueService.Entities;

namespace CatalogueService.Repo
{
    public interface IItemRepo
    {
        Task CreateAsync(Item entity);
        Task<IReadOnlyCollection<Item>> GetAllAsync();
        Task<Item> GetAsync(Guid id);
        Task RemoveAsync(Guid id);
        Task UpdateAsync(Item entity);
    }
}