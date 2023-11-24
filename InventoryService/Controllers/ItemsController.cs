using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Common;
using InventoryService.Entities;
using Microsoft.AspNetCore.Mvc;
using static InventoryService.Dtos;

namespace InventoryService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IRepo<InventoryItem> _inventoryRepo;
        private readonly IRepo<CatalogueItem> _catalogueRepo;
        private readonly IMapper _mapper;

        public ItemsController(IRepo<InventoryItem> inventoryRepo, IMapper mapper, IRepo<CatalogueItem> catalogueRepo)
        {
            _inventoryRepo = inventoryRepo;
            _mapper = mapper;
            _catalogueRepo = catalogueRepo;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetAsync([Required] Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return BadRequest();
            }

            var inventoryItems = await _inventoryRepo.GetAllAsync(item => item.UserId == userId);
            var catalogIds = inventoryItems.Select(inventoryItem => inventoryItem.CatalogItemId);
            var catalogItems = await _catalogueRepo.GetAllAsync(catalogueItem => catalogIds.Contains(catalogueItem.Id));

            var inventoryItemDto = inventoryItems.Select(inventoryItem =>
            {
                var catalogItem = catalogItems.Single(catalog => catalog.Id == inventoryItem.CatalogItemId);

                return new InventoryItemDto (catalogItem.Id, catalogItem.Name, catalogItem.Description, inventoryItem.Quantity, inventoryItem.AcquiredDate);
            });

            return Ok(inventoryItemDto);
        }

        [HttpPost]
        public async Task<ActionResult> CreateAsync(GrantItemsDto grantItemsDto)
        {

            var inventoryItem = await _inventoryRepo.GetAsync(item => item.UserId == grantItemsDto.UserId && item.CatalogItemId == grantItemsDto.CatalogueItemId);

            if (inventoryItem == null)
            {
                inventoryItem = new InventoryItem
                {
                    CatalogItemId = grantItemsDto.CatalogueItemId,
                    UserId = grantItemsDto.UserId,
                    Quantity = grantItemsDto.Quantity,
                    AcquiredDate = DateTimeOffset.UtcNow
                };

                await _inventoryRepo.CreateAsync(inventoryItem);

            }
            else
            {
                inventoryItem.Quantity += grantItemsDto.Quantity;
                await _inventoryRepo.UpdateAsync(inventoryItem);
            }

            return Ok();
        }
    }
}
