using AutoMapper;
using static InventoryService.Dtos;

namespace InventoryService.Profiles
{
    public class InventoryItemProfile : Profile
    {
        //Source -> Target

        public InventoryItemProfile()
        {
            CreateMap<InventoryItemProfile, InventoryItemDto>();
        }
    }
}
