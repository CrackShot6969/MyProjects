using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace VendingDataExtractor.Structures
{
   

    [Serializable]
    public class VendingDataObject
    {
        [XmlElement("AssetID")]
        public string AssetID {get; set;}
        [XmlElement("Date")]
        public string Date {get; set;}
        [XmlElement("Coil")]
        public string Coil {get; set;}
        [XmlElement("Item")]
        public string Item  {get; set;}
        [XmlElement("Cost")]
        public string Cost  {get; set;}
        [XmlElement("Price")]
        public string Price  {get; set;}
        [XmlElement("Vends")]
        public string Vends  {get; set;}
        [XmlElement("Value")]
        public string Value { get; set; }
    }

    [Serializable()]
    [XmlRoot("VendingDataObjects")]
    public class VendingDataObjects
    {
        [XmlArrayItem("VendingData", typeof(VendingDataObject))]
        List<VendingDataObject> VendingData { get; set; }

    }
}