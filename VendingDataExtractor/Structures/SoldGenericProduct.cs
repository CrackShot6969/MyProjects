using System;

namespace VendingDataExtractor.Structures
{
    [Serializable]
    public class SoldGenericProduct : GenericProduct
    {
        public string QuantitySold { get; set; }
        public string DateSold { get; set; }
        
        public SoldGenericProduct()
        {
        }

        public SoldGenericProduct(string productCode, string description, string cost, string price, string quantitySold, string dateSold) : base(productCode, description, cost, price)
        {
            QuantitySold = quantitySold;
            DateSold = dateSold;
        }

        public bool Equals(SoldGenericProduct obj)
        {
            return (obj.Cost.Trim() == this.Cost.Trim())
                   && (obj.DateSold.Trim() == this.DateSold.Trim())
                   && (obj.Description.Trim() == this.Description.Trim())
                   && (obj.Price.Trim() == this.Price.Trim())
                   && (obj.ProductCode.Trim() == this.ProductCode.Trim())
                   && (obj.QuantitySold.Trim() == this.QuantitySold.Trim());
        }

    }
}