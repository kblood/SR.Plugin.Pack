using SRMod.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace SRMod.DTOs
{
    [Serializable]
    public class SerializableItemData
    {
        public int m_ID;
        public string m_FriendlyName;
        public int m_PrereqID;
        public ItemSlotTypes m_Slot;
        public ItemSubCategories m_GearSubCategory;
        public string m_UIIconName; // We'll store the name of the sprite instead of the Sprite itself

        // Serialize as string to handle LoadCustomData exports (e.g., "B_pistol")
        // Will be converted to WeaponType enum after deserialization
        [System.Xml.Serialization.XmlElement("m_WeaponType")]
        public string m_WeaponTypeString { get; set; }

        // Property that converts string to/from WeaponType enum
        [System.Xml.Serialization.XmlIgnore]
        public WeaponType m_WeaponType
        {
            get
            {
                if (string.IsNullOrEmpty(m_WeaponTypeString))
                    return (WeaponType)0;

                // Try to parse as enum name first (from LoadCustomData string exports)
                if (System.Enum.TryParse<WeaponType>(m_WeaponTypeString, out var enumValue))
                    return enumValue;

                // Try to parse as int (legacy format)
                if (int.TryParse(m_WeaponTypeString, out var intValue))
                    return (WeaponType)intValue;

                // Default fallback
                return (WeaponType)0;
            }
            set
            {
                m_WeaponTypeString = value.ToString();
            }
        }

        public int m_ValidWeaponAugmentationWeaponMask;
        public int m_PrototypeProgressionValue;
        public int m_BlueprintProgressionValue;
        public float m_StealthVsCombat;
        public List<SerializableModifierData> m_Modifiers;
        public List<int> m_AbilityIDs;
        public List<int> m_AbilityMasks;
        public bool m_AvailableToPlayer;
        public bool m_AvailableFor_ALPHA_BETA_EARLYACCESS;
        public bool m_PlayerStartsWithBlueprints;
        public bool m_PlayerStartsWithPrototype;
        public bool m_PlayerCanResearchFromStart;
        public bool m_ResearchStarted;
        public float m_BlueprintCost;
        public float m_PrototypeCost;
        public float m_FindBlueprintCost;
        public float m_FindPrototypeCost;
        public float m_Cost;
        public bool m_ItemHasBeenLocated;
        public int m_BlueprintRandomReleaseStage;
        public int m_PrototypeRandomReleaseStage;
        public float m_ResearchCost;
        public float m_TotalResearchTime;
        public float m_Progression;
        public int m_MinResearchersRequired;
        public int m_Count;
        public bool m_PlayerHasPrototype;
        public bool m_PlayerHasBlueprints;
        public float m_ResearchProgress;
        public float m_ResearchTimeToDate;
        public float m_ResearchCostToDate;
        public bool m_PrototypeIsInTheWorld;
        public int m_InHouseResearchersResearching;
        public int m_ExternalResearchersResearching;
        public float m_ResearchProgressionPerSecond;
        public float m_CurrentResearchCost;
        public bool m_Expanded;
        public int m_OverrideAmmo;

        public SerializableItemData()
        {
            // Initialize all collections to prevent null reference exceptions
            m_Modifiers = new List<SerializableModifierData>();
            m_AbilityIDs = new List<int>();
            m_AbilityMasks = new List<int>();
        }

        public SerializableItemData(ItemManager.ItemData itemData)
        {
            m_ID = itemData.m_ID;
            m_FriendlyName = itemData.m_FriendlyName;
            m_PrereqID = itemData.m_PrereqID;
            m_Slot = itemData.m_Slot;
            m_GearSubCategory = itemData.m_GearSubCategory;
            m_UIIconName = itemData.m_UIIcon != null ? itemData.m_UIIcon.texture.name : "";
            if(itemData.m_UIIcon != null && !string.IsNullOrEmpty(m_UIIconName))
            {
                //string path = Path.Combine(Manager.GetPluginManager().PluginPath, "icons");
                //path = Path.Combine(path, m_UIIconName + ".png");
                FileManager.SaveTextureToFile(itemData.m_UIIcon.texture);
                //SpriteSerializer.SaveSpriteToDisk(itemData.m_UIIcon, path);
            }

            m_WeaponType = itemData.m_WeaponType;
            m_ValidWeaponAugmentationWeaponMask = itemData.m_ValidWeaponAugmentationWeaponMask;
            m_PrototypeProgressionValue = itemData.m_PrototypeProgressionValue;
            m_BlueprintProgressionValue = itemData.m_BlueprintProgressionValue;
            m_StealthVsCombat = itemData.m_StealthVsCombat;
            m_Modifiers = itemData.m_Modifiers != null ?
                Array.ConvertAll(itemData.m_Modifiers, m => new SerializableModifierData(m)).ToList() :
                new List<SerializableModifierData>();
            m_AbilityIDs = new List<int>(itemData.m_AbilityIDs);
            m_AbilityMasks = new List<int>(itemData.m_AbilityMasks);
            m_AvailableToPlayer = itemData.m_AvailableToPlayer;
            m_AvailableFor_ALPHA_BETA_EARLYACCESS = itemData.m_AvailableFor_ALPHA_BETA_EARLYACCESS;
            m_PlayerStartsWithBlueprints = itemData.m_PlayerStartsWithBlueprints;
            m_PlayerStartsWithPrototype = itemData.m_PlayerStartsWithPrototype;
            m_PlayerCanResearchFromStart = itemData.m_PlayerCanResearchFromStart;
            m_ResearchStarted = itemData.m_ResearchStarted;
            m_BlueprintCost = itemData.m_BlueprintCost;
            m_PrototypeCost = itemData.m_PrototypeCost;
            m_FindBlueprintCost = itemData.m_FindBlueprintCost;
            m_FindPrototypeCost = itemData.m_FindPrototypeCost;
            m_Cost = itemData.m_Cost;
            m_ItemHasBeenLocated = itemData.m_ItemHasBeenLocated;
            m_BlueprintRandomReleaseStage = itemData.m_BlueprintRandomReleaseStage;
            m_PrototypeRandomReleaseStage = itemData.m_PrototypeRandomReleaseStage;
            m_ResearchCost = itemData.m_ResearchCost;
            //m_TotalResearchTime = itemData.m_TotalResearchTime;
            m_Progression = itemData.m_Progression;
            m_MinResearchersRequired = itemData.m_MinResearchersRequired;
            m_Count = itemData.m_Count;
            m_PlayerHasPrototype = itemData.m_PlayerHasPrototype;
            m_PlayerHasBlueprints = itemData.m_PlayerHasBlueprints;
            m_ResearchProgress = itemData.m_ResearchProgress;
            //m_ResearchTimeToDate = itemData.m_ResearchTimeToDate;
            //m_ResearchCostToDate = itemData.m_ResearchCostToDate;
            m_PrototypeIsInTheWorld = itemData.m_PrototypeIsInTheWorld;
            m_InHouseResearchersResearching = itemData.m_InHouseResearchersResearching;
            m_ExternalResearchersResearching = itemData.m_ExternalResearchersResearching;
            m_ResearchProgressionPerSecond = itemData.m_ResearchProgressionPerSecond;
            m_CurrentResearchCost = itemData.m_CurrentResearchCost;
            m_Expanded = itemData.m_Expanded;
            m_OverrideAmmo = itemData.m_OverrideAmmo;
        }

        public ItemManager.ItemData ToItemData()
        {
            ItemManager.ItemData itemData = new ItemManager.ItemData();
            itemData.m_ID = m_ID;
            itemData.m_FriendlyName = m_FriendlyName;
            itemData.m_PrereqID = m_PrereqID;
            itemData.m_Slot = m_Slot;
            itemData.m_GearSubCategory = m_GearSubCategory;
            //itemData.m_UIIcon = !string.IsNullOrEmpty(m_UIIconName) ? Resources.Load<Sprite>(m_UIIconName) : null;
            itemData.m_WeaponType = m_WeaponType;
            itemData.m_ValidWeaponAugmentationWeaponMask = m_ValidWeaponAugmentationWeaponMask;
            itemData.m_PrototypeProgressionValue = m_PrototypeProgressionValue;
            itemData.m_BlueprintProgressionValue = m_BlueprintProgressionValue;
            itemData.m_StealthVsCombat = m_StealthVsCombat;
            itemData.m_Modifiers = m_Modifiers != null ?
                Array.ConvertAll(m_Modifiers.ToArray(), m => m.ToModifierData()) :
                new ModifierData5L[0];
            itemData.m_AbilityIDs = new List<int>(m_AbilityIDs);
            itemData.m_AbilityMasks = new List<int>(m_AbilityMasks);
            itemData.m_AvailableToPlayer = m_AvailableToPlayer;
            itemData.m_AvailableFor_ALPHA_BETA_EARLYACCESS = m_AvailableFor_ALPHA_BETA_EARLYACCESS;
            itemData.m_PlayerStartsWithBlueprints = m_PlayerStartsWithBlueprints;
            itemData.m_PlayerStartsWithPrototype = m_PlayerStartsWithPrototype;
            itemData.m_PlayerCanResearchFromStart = m_PlayerCanResearchFromStart;
            itemData.m_ResearchStarted = m_ResearchStarted;
            itemData.m_BlueprintCost = m_BlueprintCost;
            itemData.m_PrototypeCost = m_PrototypeCost;
            itemData.m_FindBlueprintCost = m_FindBlueprintCost;
            itemData.m_FindPrototypeCost = m_FindPrototypeCost;
            itemData.m_Cost = m_Cost;
            itemData.m_ItemHasBeenLocated = m_ItemHasBeenLocated;
            itemData.m_BlueprintRandomReleaseStage = m_BlueprintRandomReleaseStage;
            itemData.m_PrototypeRandomReleaseStage = m_PrototypeRandomReleaseStage;
            itemData.m_ResearchCost = m_ResearchCost;
            //itemData.m_TotalResearchTime = m_TotalResearchTime;
            itemData.m_Progression = m_Progression;
            itemData.m_MinResearchersRequired = m_MinResearchersRequired;
            itemData.m_Count = m_Count;
            itemData.m_PlayerHasPrototype = m_PlayerHasPrototype;
            itemData.m_PlayerHasBlueprints = m_PlayerHasBlueprints;
            itemData.m_ResearchProgress = m_ResearchProgress;
            //itemData.m_ResearchTimeToDate = m_ResearchTimeToDate;
            //itemData.m_ResearchCostToDate = m_ResearchCostToDate;
            itemData.m_PrototypeIsInTheWorld = m_PrototypeIsInTheWorld;
            itemData.m_InHouseResearchersResearching = m_InHouseResearchersResearching;
            itemData.m_ExternalResearchersResearching = m_ExternalResearchersResearching;
            itemData.m_ResearchProgressionPerSecond = m_ResearchProgressionPerSecond;
            itemData.m_CurrentResearchCost = m_CurrentResearchCost;
            itemData.m_Expanded = m_Expanded;
            itemData.m_OverrideAmmo = m_OverrideAmmo;

            return itemData;
        }
    }

    [Serializable]
    public class SerializableModifierData
    {
        public ModifierType m_Type;
        public float m_Ammount;
        public ModifierType m_AmountModifier;
        public float m_TimeOut;

        public SerializableModifierData() { }

        public SerializableModifierData(ModifierData5L modifierData)
        {
            m_Type = modifierData.m_Type;
            m_Ammount = modifierData.m_Ammount;
            m_AmountModifier = modifierData.m_AmountModifier;
            m_TimeOut = modifierData.m_TimeOut;
        }

        public ModifierData5L ToModifierData()
        {
            return new ModifierData5L
            {
                m_Type = m_Type,
                m_Ammount = m_Ammount,
                m_AmountModifier = m_AmountModifier,
                m_TimeOut = m_TimeOut
            };
        }
    }
}
