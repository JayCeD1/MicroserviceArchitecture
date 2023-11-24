using Common;
using Contract;
using InventoryService.Entities;
using MassTransit;

namespace InventoryService.Consumers;

public class CatalogueItemCreatedConsumer : IConsumer<CatalogueItemCreated>
{
    private readonly IRepo<CatalogueItem> _repo;

    public CatalogueItemCreatedConsumer(IRepo<CatalogueItem> repo)
    {
        _repo = repo;
    }
    public async Task Consume(ConsumeContext<CatalogueItemCreated> context)
    {
        var message = context.Message;

        var item = await _repo.GetAsync(message.ItemId);

        if (item != null)
        {
            Console.WriteLine("--> Always null inside here!");
            return;
        }

        item = new CatalogueItem
        {
            Id = message.ItemId,
            Name = message.Name,
            Description = message.Description
        };

        await _repo.CreateAsync(item);
    }
}