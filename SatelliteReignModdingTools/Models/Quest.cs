using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SatelliteReignModdingTools.Models
{
    /// <summary>
    /// Simple Quest model to match the exported questDefinitions.xml format
    /// </summary>
    public class Quest
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public bool Hidden { get; set; }
        public bool ShowDebrief { get; set; }
        public bool State { get; set; }  // True = completed, False = not completed
        public string TitleKey { get; set; }
        public string District { get; set; }
        public int WakeOnLocation { get; set; } = -1;
        
        public QuestLocation Location { get; set; }
        public QuestVIP VIP { get; set; }
        public List<QuestWakeOnLocation> WakeOnLocationList { get; set; }
        [XmlArray("Descriptions")]
        [XmlArrayItem("Description")]
        public List<QuestDescription> Descriptions { get; set; }
        [XmlArray("SubQuests")]
        [XmlArrayItem("SubQuestID")]
        public List<int> SubQuests { get; set; }
        
        [XmlArray("Rewards")]
        [XmlArrayItem("Reward")]
        public List<QuestReward> Rewards { get; set; }

        // Computed properties for UI display
        public string DisplayName => $"{ID}: {Title}";
        public string StatusText => State ? "✅ Completed" : "⏳ Active";
        public string DistrictText => District == "NONE" ? "No District" : District;
        public string LocationText => Location?.LocationID > 0 ? $"Location {Location.LocationID}" : "No Location";
        public string DescriptionText => Descriptions?.Count > 0 ? Descriptions[0].Translation : "No description";
        public string SubQuestText => SubQuests?.Count > 0 ? $"{SubQuests.Count} sub-quests" : "No sub-quests";

        public Quest()
        {
            WakeOnLocationList = new List<QuestWakeOnLocation>();
            Descriptions = new List<QuestDescription>();
            SubQuests = new List<int>();
            Rewards = new List<QuestReward>();
        }
    }

    public class QuestLocation
    {
        public int LocationID { get; set; }
    }

    public class QuestVIP
    {
        public bool HasVIP { get; set; }
    }

    public class QuestWakeOnLocation
    {
        public int Location { get; set; }
    }

    public class QuestDescription
    {
        public string LocTitle { get; set; }
        public string Translation { get; set; }
        public bool IsNew { get; set; }
        public bool HasBeenSeen { get; set; }
    }
    
    public enum QuestRewardType
    {
        Item,
        Money,
        Experience,
        Blueprint,
        Prototype,
        Ability,
        DistrictPass
    }
    
    public class QuestReward
    {
        public QuestRewardType Type { get; set; }
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string PrototypeInfo { get; set; }
        public string DistrictName { get; set; }
        public int Quantity { get; set; }
        public float DropChance { get; set; }
        public bool IsGuaranteed { get; set; }
        public string Description { get; set; }
        
        public QuestReward()
        {
            Type = QuestRewardType.Item;
            Quantity = 1;
            DropChance = 100.0f;
            IsGuaranteed = true;
        }
        
        public string DisplayText => GetDisplayText();
        
        private string GetDisplayText()
        {
            switch (Type)
            {
                case QuestRewardType.Prototype:
                    return $"Prototype: {PrototypeInfo ?? ItemName ?? $"ID {ItemId}"} (x{Quantity}) - {DropChance:F0}%";
                case QuestRewardType.DistrictPass:
                    return $"District Pass: {DistrictName ?? "Unknown District"} - {DropChance:F0}%";
                case QuestRewardType.Money:
                    return $"Credits: {Quantity} - {DropChance:F0}%";
                case QuestRewardType.Experience:
                    return $"Experience: {Quantity} XP - {DropChance:F0}%";
                default:
                    return $"{Type}: {ItemName ?? $"ID {ItemId}"} (x{Quantity}) - {DropChance:F0}%";
            }
        }
    }

    /// <summary>
    /// Root container for quest definitions XML
    /// </summary>
    [XmlRoot("QuestDefinitions")]
    public class QuestDefinitions
    {
        [XmlElement("Quest")]
        public List<Quest> Quests { get; set; }

        public QuestDefinitions()
        {
            Quests = new List<Quest>();
        }
    }
}