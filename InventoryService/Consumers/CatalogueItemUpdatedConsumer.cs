using Common;
using Contract;
using InventoryService.Entities;
using MassTransit;

namespace InventoryService.Consumers;

public class CatalogueItemUpdatedConsumer : IConsumer<CatalogueItemUpdated>
{
    private readonly IRepo<CatalogueItem> _repo;

    public CatalogueItemUpdatedConsumer(IRepo<CatalogueItem> repo)
    {
        _repo = repo;
    }
    public async Task Consume(ConsumeContext<CatalogueItemUpdated> context)
    {
        var message = context.Message;

        var item = await _repo.GetAsync(message.ItemId);

        if (item == null)
        {
            Console.WriteLine("--> Always writing inside here!");
            
            item = new CatalogueItem
            {
                Id = message.ItemId,
                Name = message.Name,
                Description = message.Description
            };

            await _repo.CreateAsync(item);
        }
        else
        {
            item.Name = message.Name;
            item.Description = message.Description;

            await _repo.UpdateAsync(item);
        }

       
    }
}