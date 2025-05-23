using DispatchR.Requests.Send;

namespace SupplyNest.Domain.Application.Services.Commands.Create;

public sealed record CreateProductCommand:IRequest<CreateProductCommand, ValueTask<bool>>
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string SKU { get; set; }
    public string Category { get; set; }
    public string Brand { get; set; }
    public decimal Weight { get; set; }
    public string Dimensions { get; set; }
    public bool IsActive { get; set; }
}