using CatalogueService.Entities;
using static CatalogueService.Dtos;

namespace CatalogueService.Profiles
{
    public class ItemProfile : AutoMapper.Profile
    {
        public ItemProfile()
        {
            //Source -> Target
            CreateMap<Item, ItemDto>();
        }
    }
}
