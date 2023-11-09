namespace InventoryService
{
    public class Dtos
    {
        public record GrantItemsDto(Guid UserId, Guid CatalogueItemId, int Quantity);

        public record InventoryItemDto(Guid CatalogueItemId, int Quantity, DateTimeOffset AcquiredDate);
    }
}
