using System;
using System.Xml.Serialization;

namespace SatelliteReignModdingTools.DTOs
{
    // Future-proof DTOs mapped from SkillManager/ProgressionManager exports.
    // These are stubs to allow UI scaffolding without breaking existing builds.

    [Serializable]
    public class SerializableSkillData
    {
        [XmlAttribute("Id")] public int Id { get; set; }
        [XmlAttribute("Key")] public string Key { get; set; } // translation key or internal name
        [XmlElement("DisplayName")] public string DisplayName { get; set; }
        [XmlElement("DescriptionKey")] public string DescriptionKey { get; set; }
        [XmlElement("Cost")] public int Cost { get; set; }
        [XmlArray("Prerequisites"), XmlArrayItem("SkillId")] public int[] Prerequisites { get; set; } = Array.Empty<int>();
        [XmlElement("EffectType")] public string EffectType { get; set; } // e.g., StatModifier, AbilityUnlock
        [XmlElement("EffectValue")] public float EffectValue { get; set; }
    }

    [Serializable]
    public class SerializableAgentClassData
    {
        [XmlAttribute("Id")] public int Id { get; set; }
        [XmlAttribute("Name")] public string Name { get; set; } // Soldier, Hacker, Support, Infiltrator, etc.
        [XmlArray("StartingSkills"), XmlArrayItem("SkillId")] public int[] StartingSkills { get; set; } = Array.Empty<int>();
        [XmlArray("RestrictedSkills"), XmlArrayItem("SkillId")] public int[] RestrictedSkills { get; set; } = Array.Empty<int>();
        [XmlElement("BaseHealth")] public int BaseHealth { get; set; }
        [XmlElement("BaseEnergy")] public int BaseEnergy { get; set; }
        [XmlElement("BaseArmor")] public int BaseArmor { get; set; }
    }

    [Serializable]
    public class SerializableSkillTree
    {
        [XmlArray("Skills"), XmlArrayItem("Skill")] public SerializableSkillData[] Skills { get; set; } = Array.Empty<SerializableSkillData>();
        [XmlArray("Classes"), XmlArrayItem("Class")] public SerializableAgentClassData[] Classes { get; set; } = Array.Empty<SerializableAgentClassData>();
    }

    [Serializable]
    public class SerializableProgressionCurve
    {
        // Simple piecewise points for XP curve; UI will render graph later.
        [XmlArray("Points"), XmlArrayItem("Point")] public ProgressionPoint[] Points { get; set; } = Array.Empty<ProgressionPoint>();
    }

    [Serializable]
    public class ProgressionPoint
    {
        [XmlAttribute("Level")] public int Level { get; set; }
        [XmlAttribute("XP")] public int XP { get; set; }
    }
}
