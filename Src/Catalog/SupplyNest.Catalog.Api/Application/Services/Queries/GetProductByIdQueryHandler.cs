using DispatchR.Requests.Send;
using SupplyNest.Domain.Application.Services.Interfaces;
using SupplyNest.Domain.Domain.Products;

namespace SupplyNest.Domain.Application.Services.Queries;

public sealed class GetProductByIdQueryHandler(IProductRepository productRepository):IRequestHandler<GetProductByIdQuery, ValueTask<Product>>
{

    public async ValueTask<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(request.Id);
        return product;
    }
}