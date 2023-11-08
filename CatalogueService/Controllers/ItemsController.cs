using AutoMapper;
using CatalogueService.Entities;
using CatalogueService.Repo;
using Microsoft.AspNetCore.Mvc;
using static CatalogueService.Dtos;

namespace CatalogueService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IItemRepo itemRepo;

        public ItemsController(IItemRepo itemRepo, IMapper mapper)
        {
            this.itemRepo = itemRepo;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ItemDto>> GetAsync()
        {
            var items = await itemRepo.GetAllAsync();
            return mapper.Map<IEnumerable<ItemDto>>(items);
        }

        [HttpGet("{id}",Name = "GetByIdAsync")]
        public async Task<ActionResult<ItemDto>> GetByIdAsync(Guid id)
        {
            var item = await itemRepo.GetAsync(id);

            if(item != null)
            {
                return mapper.Map<ItemDto>(item);
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

            await itemRepo.CreateAsync(item);

            return CreatedAtAction(nameof(GetByIdAsync), new { item.Id }, item);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, UpdateItemDto updateItemDto)
        {
            var item = await itemRepo.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            item.Name = updateItemDto.Name;
            item.Description = updateItemDto.Description;
            item.Price = updateItemDto.Price;


            await itemRepo.UpdateAsync(item);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var item = await itemRepo.GetAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            await itemRepo.RemoveAsync(item.Id);

            return NoContent();
        }
    }
}
