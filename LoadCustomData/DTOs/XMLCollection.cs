﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace LoadCustomDataMod.DTOs
{
    public class XMLCollection<T>
    {
        [XmlArray("ItemList")]
        public List<T> Items { get; set; }
    }
}
