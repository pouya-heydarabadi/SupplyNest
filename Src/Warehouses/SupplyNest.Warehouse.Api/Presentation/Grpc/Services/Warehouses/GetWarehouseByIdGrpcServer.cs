using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using SupplyNest.Warehouse.Api.Infrastructure.SqlServerConfigs.DbContexts;
using SupplyNest.Warehouse.Api.Presentations.Grpc.Structure.Server;

namespace SupplyNest.Warehouse.Api.Presentation.Grpc.Services.Warehouses;

public class GetWarehouseByIdGrpcServer(WarehouseDbContext _dbContext):GetWarehouseById.GetWarehouseByIdBase
{
    public override async Task<GetWarehouseByIdResponse> GetWarehouseById(GetWarehouseByIdRequest request, ServerCallContext context)
    {
        bool warehouseId = Guid.TryParse(request.WarehouseId, out Guid warehosueId);
        
        bool response = await _dbContext.Warehouses
            .AnyAsync(x => x.Id == warehosueId);
        return new GetWarehouseByIdResponse()
        {
            Result = response
        };    
    }
}