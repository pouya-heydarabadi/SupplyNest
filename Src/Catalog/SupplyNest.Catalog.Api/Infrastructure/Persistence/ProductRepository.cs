using System.Text.RegularExpressions;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using SupplyNest.Domain.Application.Services.Interfaces;
using SupplyNest.Domain.Domain.Products;

namespace SupplyNest.Domain.Infrastructure.Persistence;

public class ProductRepository:IProductRepository
{
    private readonly IMongoCollection<Product> _collection;

    public ProductRepository(IMongoDatabase database)
    {
        _collection = database.GetCollection<Product>(nameof(Product));
        
        // Create text index on Name field if it doesn't exist
        var indexKeysDefinition = Builders<Product>.IndexKeys.Text(x => x.Name);
        _collection.Indexes.CreateOne(new CreateIndexModel<Product>(indexKeysDefinition));
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

    public async Task<IEnumerable<Product>> SearchByNameAsync(string searchTerm)
    {
        // Split search term into individual words
        var searchWords = searchTerm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        
        // Create a filter for each word
        var filters = searchWords.Select(word => 
            Builders<Product>.Filter.Regex(x => x.Name, 
                new MongoDB.Bson.BsonRegularExpression(word, "i"))).ToList();

        // Combine all filters with AND operator
        var combinedFilter = Builders<Product>.Filter.And(filters);

        // Execute the search
        var products = await _collection.Find(combinedFilter).ToListAsync();

        return products;
    }

    public async Task<IEnumerable<Product>> FuzzySearchByNameAsync(string searchTerm, int maxDistance = 2)
    {
        // Split search term into individual words
        var searchWords = searchTerm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        
        // Create a filter for each word using fuzzy matching
        var filters = searchWords.Select(word => 
            Builders<Product>.Filter.Regex(x => x.Name, 
                new MongoDB.Bson.BsonRegularExpression($"(?i){word}", "i"))).ToList();

        // Combine all filters with AND operator
        var combinedFilter = Builders<Product>.Filter.And(filters);

        // Execute the search with fuzzy matching
        var products = await _collection.Find(combinedFilter)
            .ToListAsync();

        // Apply fuzzy matching in memory for more accurate results
        var fuzzyResults = products.Where(product =>
        {
            var productWords = product.Name.ToLower().Split(' ');
            return searchWords.All(searchWord =>
                productWords.Any(productWord =>
                    LevenshteinDistance(searchWord.ToLower(), productWord) <= maxDistance));
        });

        return fuzzyResults;
    }

    private static int LevenshteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0) return m;
        if (m == 0) return n;

        for (int i = 0; i <= n; i++)
            d[i, 0] = i;

        for (int j = 0; j <= m; j++)
            d[0, j] = j;

        for (int j = 1; j <= m; j++)
        {
            for (int i = 1; i <= n; i++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }

        return d[n, m];
    }

    public async Task<IEnumerable<Product>> BooleanSearchAsync(string searchTerm, string searchOperator)
    {
        var searchWords = searchTerm.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        var filters = new List<FilterDefinition<Product>>();

        foreach (var word in searchWords)
        {
            var wordFilter = Builders<Product>.Filter.Regex(x => x.Name, 
                new MongoDB.Bson.BsonRegularExpression(word, "i"));
            filters.Add(wordFilter);
        }

        FilterDefinition<Product> combinedFilter;
        switch (searchOperator.ToLower())
        {
            case "and":
                combinedFilter = Builders<Product>.Filter.And(filters);
                break;
            case "or":
                combinedFilter = Builders<Product>.Filter.Or(filters);
                break;
            default:
                throw new ArgumentException("Invalid operator. Use 'AND' or 'OR'.");
        }

        return await _collection.Find(combinedFilter).ToListAsync();
    }

    public async Task<IEnumerable<Product>> WildcardSearchAsync(string pattern)
    {
        // Convert wildcard pattern to regex
        var regexPattern = pattern
            .Replace("*", ".*")
            .Replace("?", ".");
        
        var filter = Builders<Product>.Filter.Regex(x => x.Name, 
            new MongoDB.Bson.BsonRegularExpression($"^{regexPattern}$", "i"));
        
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Product>> PhraseSearchAsync(string phrase)
    {
        // Escape special characters in the phrase
        var escapedPhrase = Regex.Escape(phrase);
        var filter = Builders<Product>.Filter.Regex(x => x.Name, 
            new MongoDB.Bson.BsonRegularExpression(escapedPhrase, "i"));
        
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Product>> ProximitySearchAsync(string term1, string term2, int maxDistance)
    {
        var allProducts = await _collection.Find(_ => true).ToListAsync();
        
        return allProducts.Where(product =>
        {
            var words = product.Name.ToLower().Split(' ');
            var term1Index = Array.IndexOf(words, term1.ToLower());
            var term2Index = Array.IndexOf(words, term2.ToLower());
            
            if (term1Index == -1 || term2Index == -1) return false;
            
            return Math.Abs(term1Index - term2Index) <= maxDistance;
        });
    }

    public async Task<IEnumerable<Product>> RangeSearchAsync(decimal minPrice, decimal maxPrice)
    {
        var filter = Builders<Product>.Filter.And(
            Builders<Product>.Filter.Gte(x => x.Price, minPrice),
            Builders<Product>.Filter.Lte(x => x.Price, maxPrice)
        );
        
        return await _collection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Product>> FacetedSearchAsync(Dictionary<string, string> facets)
    {
        var filters = new List<FilterDefinition<Product>>();
        
        foreach (var facet in facets)
        {
            switch (facet.Key.ToLower())
            {
                case "name":
                    filters.Add(Builders<Product>.Filter.Regex(x => x.Name, 
                        new MongoDB.Bson.BsonRegularExpression(facet.Value, "i")));
                    break;
                case "category":
                    filters.Add(Builders<Product>.Filter.Eq(x => x.Category, facet.Value));
                    break;
                case "brand":
                    filters.Add(Builders<Product>.Filter.Eq(x => x.Brand, facet.Value));
                    break;
                case "minprice":
                    if (decimal.TryParse(facet.Value, out decimal minPrice))
                        filters.Add(Builders<Product>.Filter.Gte(x => x.Price, minPrice));
                    break;
                case "maxprice":
                    if (decimal.TryParse(facet.Value, out decimal maxPrice))
                        filters.Add(Builders<Product>.Filter.Lte(x => x.Price, maxPrice));
                    break;
                case "isactive":
                    if (bool.TryParse(facet.Value, out bool isActive))
                        filters.Add(Builders<Product>.Filter.Eq(x => x.IsActive, isActive));
                    break;
            }
        }
        
        var combinedFilter = Builders<Product>.Filter.And(filters);
        return await _collection.Find(combinedFilter).ToListAsync();
    }


}