namespace SupplyNest.Domain.Domain.Products;

public class Product
{
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

    public static Product CreateProduct(string name, string description, string sku, string category, string brand, string dimensions)
    {
        return new Product
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = description,
            SKU = sku,
            Category = category,
            Brand = brand,
            Dimensions = dimensions,
            CreatedAt = DateTime.Now
        };
    }

    public void UpdateDetails(string name, string description, decimal price, string sku, string category, string brand, decimal weight, string dimensions, bool isActive)
    {
        Name = name;
        Description = description;
        Price = price;
        SKU = sku;
        Category = category;
        Brand = brand;
        Weight = weight;
        Dimensions = dimensions;
        IsActive = isActive;
        UpdatedAt = DateTime.Now;
    }
}