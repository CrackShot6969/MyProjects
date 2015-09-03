using System;

namespace VendingDataExtractor.Structures
{
    [Serializable]
    public class GenericProduct
    {
        public string ProductCode { get; set; }
        public string Description { get; set; }
        public string Cost { get; set; }
        public string Price { get; set; }

        protected GenericProduct()
        {

        }

        public GenericProduct(string productCode, string description, string cost, string price)
        {
            this.ProductCode = productCode;
            this.Description = description;
            this.Cost = cost;
            this.Price = price;
        }


    }
}
