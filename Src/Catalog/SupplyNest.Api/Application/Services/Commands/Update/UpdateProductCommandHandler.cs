using DispatchR.Requests.Send;
using SupplyNest.Domain.Domain.Products;

namespace SupplyNest.Domain.Application.Services.Commands.Update;

public sealed class UpdateProductCommandHandler(IProductRepository productRepository)
    :IRequestHandler<UpdateProductCommand, ValueTask<bool>>
{
    public async ValueTask<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        Product? product = await productRepository.GetByIdAsync(request.Id);
        if (product is null)
            throw new InvalidOperationException("Product Does Not Exist In DataBase... ");
        
        product.UpdateDetails(request.Name, request.Description,request.Price,
            request.SKU, request.Category, request.Brand,
            request.Weight,request.Dimensions, request.IsActive);

        bool result =  await productRepository.UpdateAsync(product);

        return result;
    }
}