using System;
using System.Xml.Serialization;

namespace ConsumeServices
{
    public class Product
    {
        public string id { get; set; }
        public string name { get; set; }
        public decimal price { get; set; }
    }
}