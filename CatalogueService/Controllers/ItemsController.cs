using Microsoft.AspNetCore.Mvc;
using static CatalogueService.Dtos;

namespace CatalogueService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private static readonly List<ItemDto> items = new()
        {
            new ItemDto(Guid.NewGuid(), "Portion", "Restores", 5 , DateTimeOffset.UtcNow),
            new ItemDto(Guid.NewGuid(), "Portion 2", "Restores all", 20 , DateTimeOffset.UtcNow),
        };

        [HttpGet]
        public IEnumerable<ItemDto> Get()
        {
            return items;
        }

        [HttpGet("{id}",Name = "GetById")]
        public ItemDto GetById(Guid id)
        {
            var item = items.Where(item => item.Id == id).SingleOrDefault();
            return item;
        }

        [HttpPost]
        public ActionResult<ItemDto> Create(CreateItemDto createItemDto)
        {
            var item = new ItemDto(Guid.NewGuid(), createItemDto.Name, createItemDto.Description, createItemDto.Price, DateTimeOffset.UtcNow);
            items.Add(item);

            return CreatedAtAction(nameof(GetById), new { item.Id}, item);
        }

        [HttpPut("{id}")]
        public ActionResult Update(Guid id, UpdateItemDto updateItemDto)
        {
            var item = items.Where(x => x.Id == id).SingleOrDefault();

            var updatedItem = item with
            {
                Name = updateItemDto.Name,
                Description = updateItemDto.Description,
                Price = updateItemDto.Price
            };

            var index = items.FindIndex(x => x.Id == id);
            items[index] = updatedItem;

            return NoContent();
        }
    }
}
