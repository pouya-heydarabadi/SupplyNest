using DispatchR.Requests.Send;
using SupplyNest.Domain.Domain.Products;

namespace SupplyNest.Domain.Application.Services.Queries;

public sealed record GetProductByIdQuery : IRequest<GetProductByIdQuery, ValueTask<Product>>
{
    public Guid Id { get; set; }
}