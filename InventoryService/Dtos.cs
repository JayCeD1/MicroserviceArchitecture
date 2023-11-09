namespace InventoryService
{
    public class Dtos
    {
        public record GrantItemsDto(Guid UserId, Guid CatalogueItemId, int Quantity);

        public record InventoryItemDto(Guid CatalogueItemId, string Name, string Description, int Quantity, DateTimeOffset AcquiredDate);

        public record CatalogItemDto(Guid Id, string Name, string Description);
    }
}
