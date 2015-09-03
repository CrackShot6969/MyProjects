using System;
using System.Collections.Generic;
using System.Linq;
using VendingDataExtractor.Misc;
using VendingDataExtractor.Structures;

namespace VendingDataExtractor.Interfaces
{
    public class VendingProductMapper : IProductMapper
    {
        private string _mapFile { get; set; }

        public VendingProductMapper(string mapFile)
        {
            this._mapFile = mapFile;
        }

        public object GetProductMappings(object productsToMap, ILogger<string> logger)
        {
            var result = new ProductsSoldResult();
            
            try
            {
                var map = productsToMap as List<VendingDataObject>;
                if (map != null)
                {
                    map.ForEach(thisVendProduct => result.Add(new SoldGenericProduct(this.GetProdCodeByMapping(thisVendProduct, logger), thisVendProduct.Item, thisVendProduct.Cost, thisVendProduct.Price, thisVendProduct.Vends, thisVendProduct.Date)));
                }
                result.SetStatus(new Status("", true));
            }
            catch (Exception e)
            {
                result.SetStatus(new Status("An exception occurred mapping Products : " + e, false));
                throw;
            }
            return result;
        }

        private ProductMappings GetProductMappings(ILogger<string> logger)
        {
            return (ProductMappings)Utilities.DeserializeObject(typeof(ProductMappings), this._mapFile, logger);
        }

        private string GetProdCodeByMapping(object mapObj, ILogger<string> logger)
        {
            var result = String.Empty;
            try
            {
                var o = mapObj as VendingDataObject;
                if (o != null)
                {
                    VendingDataObject mapObjects = o;
                    var productMappingsSite = this.GetProductMappings(logger).Items.ToList().FirstOrDefault(i => i.AssetID == mapObjects.AssetID);
                    if (productMappingsSite != null)
                    {
                        var productMappingsSiteProduct = productMappingsSite.Product.FirstOrDefault(j => j.CircumtecCoil.Trim() == mapObjects.Coil.Trim());
                        if (productMappingsSiteProduct != null)
                            result = productMappingsSiteProduct.UnleashedCode;
                    }
                }
            }
            catch (Exception e)
            {
                logger.LogEntry("Error in GetGuidByMapping(): " + e);
            }

            return result;
        }
    }
}