namespace VendingDataExtractor.Interfaces
{
    public interface IProductMapper
    {
        object GetProductMappings(object productsToMap, ILogger<string> logger );
    }
}
