using DispatchR.Requests.Send;

namespace SupplyNest.Domain.Application.Services.Commands.Update;

public sealed record UpdateProductCommand:IRequest<UpdateProductCommand, ValueTask<bool>>
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string? SKU { get; set; }
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public decimal Weight { get; set; }
    public string? Dimensions { get; set; }
    public bool? IsActive { get; set; }
}