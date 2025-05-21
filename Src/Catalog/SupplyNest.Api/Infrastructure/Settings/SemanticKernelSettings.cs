namespace SupplyNest.Domain.Infrastructure.Settings;

public class SemanticKernelSettings
{
    public string ApiKey { get; set; } = string.Empty;
    public string EmbeddingModelId { get; set; } = string.Empty;
    public string QdrantEndpoint { get; set; } = string.Empty;
    public string CollectionName { get; set; } = string.Empty;
} 