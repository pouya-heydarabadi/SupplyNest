namespace SupplyNest.Warehouse.Api.Application.Dtos;

public record UpdateInventoryRequestDto(Guid inventoryId, int quantity);