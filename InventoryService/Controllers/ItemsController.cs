using AutoMapper;
using Common;
using InventoryService.Clients;
using InventoryService.Entities;
using Microsoft.AspNetCore.Mvc;
using static InventoryService.Dtos;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepo<InventoryItem> itemsRepo;
        private readonly IMapper mapper;
        private readonly CatalogClient catalogClient;

        public ItemsController(IRepo<InventoryItem> itemsRepo, IMapper mapper, CatalogClient catalogClient)
        {
            this.itemsRepo = itemsRepo;
            this.mapper = mapper;
            this.catalogClient = catalogClient;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var catalogItems = await catalogClient.GetCatalogItemsAsync();
            var items = await itemsRepo.GetAllAsync(item => item.UserId == userId);

            var inventoryItemDto = items.Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(catalog => catalog.Id == inventoryItem.CatalogItemId);

                return new InventoryItemDto (catalogItem.Id, catalogItem.Name, catalogItem.Description, inventoryItem.Quantity, inventoryItem.AcquiredDate);
            });

            return Ok(inventoryItemDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(GrantItemsDto grantItemsDto)
        {

            var inventoryItem = await itemsRepo.GetAsync(item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogueItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogueItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await itemsRepo.CreateAsync(inventoryItem);

            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await itemsRepo.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}
