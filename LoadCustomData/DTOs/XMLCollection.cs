using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
// System.Xml.Serialization not available in .NET 3.5

namespace SRMod.DTOs
{
    public class XMLCollection<T>
    {
        [XmlArray("ItemList")]
        public List<T> Items { get; set; }
    }
}
