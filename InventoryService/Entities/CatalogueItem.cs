using Common;

namespace InventoryService.Entities;

public class CatalogueItem : IEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }
}