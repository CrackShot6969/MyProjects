using System.Collections.Generic;

namespace VendingDataExtractor.Structures
{
    public class VendingDataResult : Result
    {
        public List<VendingDataObject> Items { get; set; }

        public VendingDataResult()
        {
            this.Items = new List<VendingDataObject>();
        }

        public void Add(VendingDataObject vendingObject)
        {
            this.Items.Add(vendingObject);
        }
    }
}