using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
// System.Xml.Serialization not available in .NET 3.5

namespace SRMod.DTOs
{
    public class TranslationsDTO
    {
        [XmlArray("TranslationsList")]
        public List<TranslationElementDTO> Translations { get; set; }
    }

    public class TranslationElementDTO
    {
        public string Key { get; set; }
        public TextManager.LocElement Element { get; set; }
    }
}
