using System;
using System.Collections.Generic;

namespace VendingDataExtractor.Structures
{
    [Serializable]
    public class ProductsSoldResult : Result
    {
        public List<SoldGenericProduct> Items { get; set; }

        public ProductsSoldResult()
        {
            this.Items = new List<SoldGenericProduct>();
        }

        public void Add(SoldGenericProduct product)
        {
            this.Items.Add(product);
        }
    }
}