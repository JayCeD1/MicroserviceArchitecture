using Common;
using Contract;
using InventoryService.Entities;
using MassTransit;

namespace InventoryService.Consumers;

public class CatalogueItemDeletedConsumer : IConsumer<CatalogueItemDeleted>
{
    private readonly IRepo<CatalogueItem> _repo;

    public CatalogueItemDeletedConsumer(IRepo<CatalogueItem> repo)
    {
        _repo = repo;
    }
    public async Task Consume(ConsumeContext<CatalogueItemDeleted> context)
    {
        var message = context.Message;

        var item = await _repo.GetAsync(message.ItemId);

        if (item == null)
        {
            return;
        }

        await _repo.RemoveAsync(item.Id);
    }
}