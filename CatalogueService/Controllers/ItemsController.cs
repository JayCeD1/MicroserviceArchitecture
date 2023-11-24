using AutoMapper;
using CatalogueService.Entities;
using Common;
using Contract;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using static CatalogueService.Dtos;

namespace CatalogueService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IRepo<Item> _itemRepo;
        private readonly IPublishEndpoint _publishEndpoint;


        public ItemsController(IRepo<Item> itemRepo, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _itemRepo = itemRepo;
            _mapper = mapper;
            _publishEndpoint = publishEndpoint;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            var items = await _itemRepo.GetAllAsync();
            return Ok(_mapper.Map<IEnumerable<ItemDto>>(items));
        }

        [HttpGet("{id}",Name = "GetByIdAsync")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await _itemRepo.GetAsync(id);

            if(item != null)
            {
                return _mapper.Map<ItemDto>(item);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<ItemDto>> CreateAsync(CreateItemDto createItemDto)
        {
            var item = new Item
            {
                Name = createItemDto.Name,
                Description = createItemDto.Description,
                Price = createItemDto.Price,
                CreatedDate = DateTimeOffset.UtcNow
            };

            await _itemRepo.CreateAsync(item);

            await _publishEndpoint.Publish(new CatalogueItemCreated(item.Id, item.Name, item.Description));

            return CreatedAtAction(nameof(GetByIdAsync), new { item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var item = await _itemRepo.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            item.Name = updateItemDto.Name;
            item.Description = updateItemDto.Description;
            item.Price = updateItemDto.Price;


            await _itemRepo.UpdateAsync(item);
            
            await _publishEndpoint.Publish(new CatalogueItemUpdated(item.Id, item.Name, item.Description));

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await _itemRepo.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await _itemRepo.RemoveAsync(item.Id);
            
            await _publishEndpoint.Publish(new CatalogueItemDeleted(item.Id));

            return NoContent();
        }
    }
}
