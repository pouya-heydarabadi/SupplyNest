using System.Collections.ObjectModel;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SupplyNest.Domain.Domain.Products;

namespace SupplyNest.Domain.Application.Services;

public class ProductRepository:IProductRepository
{
    private readonly IMongoCollection<Product> _collection;
    public ProductRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Product>(nameof(Product));
    }

    public async Task<Product> GetByIdAsync(Guid id)
    {
        Product? product = await _collection.AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == id);
        return product;
    }
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        IEnumerable<Product> products = await _collection.AsQueryable()
            .ToListAsync();

        return products;
    }
    public async Task<Product> AddAsync(Product product)
    {
        await _collection.InsertOneAsync(product);
        return product;
    }
    public async Task<bool> UpdateAsync(Product product)
    {
        Product beforeProduct = await this.GetByIdAsync(product.Id);
        if (beforeProduct is null)
            throw new InvalidOperationException("Product Was Not Found In DataBase ... ");
        
        
        var filter = Builders<Product>.Filter.Eq(x => x.Id , product.Id);
        var updateQuery = Builders<Product>.Update;
        var listUpdateProperties = new List<UpdateDefinition<Product>>();

        foreach (var property in typeof(Product).GetProperties())
        {
            if (property.Name != nameof(Product.Id))
            {
                var propertyValue = property.GetValue(product);
                var beforeValue = property.GetValue(beforeProduct);
                
                if(propertyValue is not null  && (beforeValue is null || !beforeValue.Equals(propertyValue)))
                    listUpdateProperties.Add(updateQuery.Set(property.Name, propertyValue));
                
            }
        }

        if (!listUpdateProperties.Any())
            return false;

        var combineUpdate = updateQuery.Combine(listUpdateProperties);
        var result = await _collection.UpdateOneAsync(filter,
            combineUpdate);

        return result.IsAcknowledged;

    }
    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
    public Task<bool> ExistsAsync(Guid id)
    {
        throw new NotImplementedException();
    }
    public async Task<bool> ExistsAsync(string name)
    {
        var findProduct = await _collection.AsQueryable()
            .FirstOrDefaultAsync(x => x.Name == name);
        return findProduct == null ? false : true;
    }
}