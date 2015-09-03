namespace VendingDataExtractor.Interfaces
{
    public class ProductMappingManager
    {
        public IProductMapper _mapper { get; set; }
        
        public ProductMappingManager(IProductMapper mapper)
        {
            this._mapper = mapper;
        }

        public object GetProductMappings(object productsToMap, ILogger<string> logger)
        {
            return this._mapper.GetProductMappings(productsToMap, logger);
        }
    }
}