using SupplyNest.Domain.Domain.Products;

namespace SupplyNest.Domain.Application.Services;

public interface IProductRepository
{
    Task<Product> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<Product> AddAsync(Product product);
    Task<bool> UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
    Task<bool> ExistsAsync(string name);
    
    // Simple full-text search
    Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm);
    
    // Boolean search
    Task<IEnumerable<Product>> BooleanSearchAsync(string searchTerm, string searchOperator);
    
    // Fuzzy search
    Task<IEnumerable<Product>> FuzzySearchByNameAsync(string searchTerm, int maxDistance = 2);
    
    // Wildcard search
    Task<IEnumerable<Product>> WildcardSearchAsync(string pattern);
    
    // Phrase search
    Task<IEnumerable<Product>> PhraseSearchAsync(string phrase);
    
    // Proximity search
    Task<IEnumerable<Product>> ProximitySearchAsync(string term1, string term2, int maxDistance);
    
    // Range search
    Task<IEnumerable<Product>> RangeSearchAsync(decimal minPrice, decimal maxPrice);
    
    // Faceted search
    Task<IEnumerable<Product>> FacetedSearchAsync(Dictionary<string, string> facets);
} 