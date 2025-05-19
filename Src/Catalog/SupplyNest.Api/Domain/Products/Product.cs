using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SupplyNest.Domain.Domain.Products;

public class Product
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public string SKU { get; private set; }
    public string Category { get; private set; }
    public string Brand { get; private set; }
    public decimal Weight { get; private set; }
    public string Dimensions { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    private Product() { }

    public static Product CreateProduct(string name, string description, string sku, string category, string brand, string dimensions,
        bool isActive, decimal price, decimal weight)
    {
        DateTime now = DateTime.Now;
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            SKU = sku,
            Category = category,
            Brand = brand,
            Dimensions = dimensions,
            CreatedAt = now,
            IsActive = isActive,
            Price = price,
            Weight = weight,
            UpdatedAt = now
        };
    }

    public void UpdateDetails(string? name, string? description, decimal? price, string? sku, string? category, string? brand, decimal? weight, string? dimensions, bool? isActive)
    {
        if (name is not null)
            Name = name;
            
        if (description is not null)
            Description = description;
            
        if (price.HasValue)
            Price = price.Value;
            
        if (sku is not null)
            SKU = sku;
            
        if (category is not null)
            Category = category;
            
        if (brand is not null)
            Brand = brand;
            
        if (weight.HasValue)
            Weight = weight.Value;
            
        if (dimensions is not null)
            Dimensions = dimensions;
            
        if (isActive.HasValue)
            IsActive = isActive.Value;
            
        UpdatedAt = DateTime.Now;
    }
}