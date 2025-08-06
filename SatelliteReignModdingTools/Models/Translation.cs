using System.Collections.Generic;
using System.Xml.Serialization;

namespace SatelliteReignModdingTools.Models
{
    [XmlRoot("Translation")]
    public class Translation
    {
        [XmlElement("Key")]
        public string Key { get; set; }

        [XmlElement("Element")]
        public TranslationElement Element { get; set; }

        public Translation()
        {
            Element = new TranslationElement();
        }
    }

    public class TranslationElement
    {
        [XmlElement("m_token")]
        public string Token { get; set; }

        [XmlArray("m_Translations")]
        [XmlArrayItem("string")]
        public List<string> Translations { get; set; }

        public TranslationElement()
        {
            Translations = new List<string>();
        }

        // Helper property for primary language (first entry)
        [XmlIgnore]
        public string PrimaryTranslation
        {
            get { return Translations.Count > 0 ? Translations[0] : string.Empty; }
            set 
            { 
                if (Translations.Count == 0)
                    Translations.Add(value);
                else
                    Translations[0] = value;
            }
        }
    }

    [XmlRoot("Translations")]
    public class TranslationCollection
    {
        [XmlElement("Translation")]
        public List<Translation> Translations { get; set; }

        public TranslationCollection()
        {
            Translations = new List<Translation>();
        }
    }

    // Helper class for translation categories
    public static class TranslationCategories
    {
        public static readonly Dictionary<string, string> Categories = new Dictionary<string, string>
        {
            { "Q_GEN_", "General Quests" },
            { "Q_RL_", "Red Light District" },
            { "Q_IND_", "Industrial District" },
            { "Q_GRID_", "Grid District" },
            { "Q_CBD_", "CBD District" },
            { "STORY_", "Story Quests" },
            { "MENU_", "Menu & UI" },
            { "GEN_", "General Game Text" },
            { "CUSTOM_", "Custom Mod Text" }
        };

        public static string GetCategory(string key)
        {
            if (string.IsNullOrEmpty(key))
                return "Uncategorized";

            foreach (var category in Categories)
            {
                if (key.StartsWith(category.Key))
                    return category.Value;
            }

            return "Uncategorized";
        }

        public static string GetCategoryPrefix(string categoryName)
        {
            foreach (var category in Categories)
            {
                if (category.Value == categoryName)
                    return category.Key;
            }

            return "CUSTOM_";
        }
    }
}