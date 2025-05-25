using Carter;
using DispatchR.Requests;
using SupplyNest.Inventory.Api.Application.Services.Command.Commands;

namespace SupplyNest.Inventory.Api.Presentations.Api.Inventories.Endpoints;

public sealed class CreateInventoryEndpoint:ICarterModule
{

    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("Api/Inventory", 
            async (IMediator mediator, CancellationToken cancellationToken ,CreateInventoryCommand request ) =>
            {
                var result = await mediator.Send(request, cancellationToken);

                return Results.Ok(result);
            });
    }
}