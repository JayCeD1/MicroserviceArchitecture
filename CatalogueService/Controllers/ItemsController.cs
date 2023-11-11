using AutoMapper;
using CatalogueService.Entities;
using Common;
using Microsoft.AspNetCore.Mvc;
using static CatalogueService.Dtos;

namespace CatalogueService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IRepo<Item> itemRepo;

        private static int requestCounter = 0;

        public ItemsController(IRepo<Item> itemRepo, IMapper mapper)
        {
            this.itemRepo = itemRepo;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ItemDto>>> GetAsync()
        {
            requestCounter++;

            Console.WriteLine($"Request {requestCounter}: Starting... ");

            if (requestCounter <= 2)
            {
                Console.WriteLine($"Request {requestCounter}: Delaying... ");
                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            if (requestCounter <= 4)
            {
                Console.WriteLine($"Request {requestCounter}: 500 (Internal Server Error) ");
                return StatusCode(500);
            }

            var items = await itemRepo.GetAllAsync();
            Console.WriteLine($"Request {requestCounter}: 200 (OK) ");
            return Ok(mapper.Map<IEnumerable<ItemDto>>(items));
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
