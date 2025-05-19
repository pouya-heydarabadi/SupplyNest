using DispatchR.Requests.Send;
using SupplyNest.Domain.Domain.Products;

namespace SupplyNest.Domain.Application.Services.Commands;

public class CreateProductCommandHandler
    (IProductRepository productRepository):IRequestHandler<CreateProductCommand, ValueTask<bool>>
{
    public async ValueTask<bool> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var existProduct = await productRepository.ExistsAsync(request.Name);
        if (existProduct)
            throw new InvalidOperationException("Product Is Created...");
        
        Product product = Product.CreateProduct(request.Name, request.Description,
            request.SKU, request.Category, request.Brand, request.Dimensions,
            request.IsActive, request.Price,request.Weight);

        var result = await productRepository.AddAsync(product);

        return true;
    }
}