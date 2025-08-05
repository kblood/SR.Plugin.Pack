using SRMod.DTOs;
using SRMod.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
// System.Xml.Serialization now available in .NET Framework 4.5.1
using UnityEngine;

public class ItemDataManager : MonoBehaviour
{
    private static ItemDataManager _instance;
    private List<SerializableItemData> _itemDefinitions;
    private const string SAVE_FILE_NAME = "itemDefinitions.xml";

    public static ItemDataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ItemDataManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ItemDataManager");
                    _instance = go.AddComponent<ItemDataManager>();
                }
            }
            return _instance;
        }
    }

    public void Initialize()
    {
        _itemDefinitions = new List<SerializableItemData>();
        if (!LoadItemDefinitionsFromFileAndUpdateItemManager())
        {
            PopulateFromItemManager();
        }
    }

    private void PopulateFromItemManager()
    {
        _itemDefinitions = Manager.GetItemManager().GetAllItems()
            .Select(item => new SerializableItemData(item))
            .ToList();
    }

    public bool LoadItemDefinitionsFromFileAndUpdateItemManager()
    {
        string path = Path.Combine(Manager.GetPluginManager().PluginPath, SAVE_FILE_NAME);
        if (File.Exists(path))
        {
            try
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<SerializableItemData>));
                using (var streamReader = new StreamReader(path))
                {
                    _itemDefinitions = (List<SerializableItemData>)serializer.Deserialize(streamReader);
                }

                // Update ItemManager with loaded definitions
                UpdateItemManager();

                SRInfoHelper.Log("Loaded and updated ItemManager with " + _itemDefinitions.Count + " item definitions from file");
                return true;
            }
            catch (System.Exception e)
            {
                SRInfoHelper.Log("Error loading item definitions: " + e.Message);
            }
        }
        SRInfoHelper.Log("No item definitions found in file.");
        return false;
    }

    private void UpdateItemManager()
    {
        var itemManager = Manager.GetItemManager();
        var existingItems = itemManager.GetAllItems();

        foreach (var serializedItem in _itemDefinitions)
        {
            var existingItem = existingItems.FirstOrDefault(i => i.m_ID == serializedItem.m_ID);
            if (existingItem != null)
            {
                // Update existing item
                UpdateExistingItem(existingItem, serializedItem);
            }
            else
            {
                // Create new item based on an existing one
                var newItem = CreateNewItem(serializedItem, existingItems);
                itemManager.m_ItemDefinitions.Add(newItem);
            }
        }
    }

    private void UpdateExistingItem(ItemManager.ItemData existingItem, SerializableItemData serializedItem)
    {
        // Basic properties
        existingItem.m_ID = serializedItem.m_ID;
        existingItem.m_FriendlyName = serializedItem.m_FriendlyName;
        existingItem.m_PrereqID = serializedItem.m_PrereqID;
        existingItem.m_Slot = serializedItem.m_Slot;
        existingItem.m_GearSubCategory = serializedItem.m_GearSubCategory;
        //existingItem.m_UIIcon = !string.IsNullOrEmpty(serializedItem.m_UIIconName) ? Resources.Load<Sprite>(serializedItem.m_UIIconName) : existingItem.m_UIIcon;

        //string path = Path.Combine(Manager.GetPluginManager().PluginPath, "icons");
        //path = Path.Combine(path, serializedItem.m_UIIconName + ".png");
        var loadedTexture = FileManager.LoadTextureFromFile(serializedItem.m_UIIconName);
        if (loadedTexture != null)
        {
            var loadedSprite = SpriteSerializer.UpdateSprite(loadedTexture, existingItem.m_UIIcon);
            if (loadedSprite != null)
            {
                existingItem.m_UIIcon = loadedSprite;
                //SRInfoHelper.Log($"Loaded sprite {serializedItem.m_UIIconName} for {serializedItem.m_FriendlyName}");
            }
        }
        existingItem.m_WeaponType = serializedItem.m_WeaponType;
        existingItem.m_ValidWeaponAugmentationWeaponMask = serializedItem.m_ValidWeaponAugmentationWeaponMask;

        // Progression values
        //existingItem.m_PrototypeProgressionValue = serializedItem.m_PrototypeProgressionValue;
        //existingItem.m_BlueprintProgressionValue = serializedItem.m_BlueprintProgressionValue;
        existingItem.m_StealthVsCombat = serializedItem.m_StealthVsCombat;
        //existingItem.m_Progression = serializedItem.m_Progression;

        // Modifiers and Abilities
        existingItem.m_Modifiers = serializedItem.m_Modifiers?.Select(m => m.ToModifierData()).ToArray() ?? new ModifierData5L[0];
        existingItem.m_AbilityIDs = new List<int>(serializedItem.m_AbilityIDs);
        existingItem.m_AbilityMasks = new List<int>(serializedItem.m_AbilityMasks);

        // Availability flags
        //existingItem.m_AvailableToPlayer = serializedItem.m_AvailableToPlayer;
        existingItem.m_AvailableFor_ALPHA_BETA_EARLYACCESS = serializedItem.m_AvailableFor_ALPHA_BETA_EARLYACCESS;
        existingItem.m_PlayerStartsWithBlueprints = serializedItem.m_PlayerStartsWithBlueprints;
        existingItem.m_PlayerStartsWithPrototype = serializedItem.m_PlayerStartsWithPrototype;
        existingItem.m_PlayerCanResearchFromStart = serializedItem.m_PlayerCanResearchFromStart;

        // Research-related properties
        //existingItem.m_ResearchStarted = serializedItem.m_ResearchStarted;
        existingItem.m_ResearchCost = serializedItem.m_ResearchCost;
        //existingItem.m_ResearchProgress = serializedItem.m_ResearchProgress;
        //existingItem.m_ResearchTimeToDate = serializedItem.m_ResearchTimeToDate;
        //existingItem.m_ResearchCostToDate = serializedItem.m_ResearchCostToDate;
        existingItem.m_ResearchProgressionPerSecond = serializedItem.m_ResearchProgressionPerSecond;
        existingItem.m_CurrentResearchCost = serializedItem.m_CurrentResearchCost;
        existingItem.m_MinResearchersRequired = serializedItem.m_MinResearchersRequired;
        existingItem.m_InHouseResearchersResearching = serializedItem.m_InHouseResearchersResearching;
        existingItem.m_ExternalResearchersResearching = serializedItem.m_ExternalResearchersResearching;

        // Cost-related properties
        existingItem.m_BlueprintCost = serializedItem.m_BlueprintCost;
        existingItem.m_PrototypeCost = serializedItem.m_PrototypeCost;
        existingItem.m_FindBlueprintCost = serializedItem.m_FindBlueprintCost;
        existingItem.m_FindPrototypeCost = serializedItem.m_FindPrototypeCost;
        existingItem.m_Cost = serializedItem.m_Cost;

        // State flags
        //existingItem.m_ItemHasBeenLocated = serializedItem.m_ItemHasBeenLocated;
        //existingItem.m_PlayerHasPrototype = serializedItem.m_PlayerHasPrototype;
        //existingItem.m_PlayerHasBlueprints = serializedItem.m_PlayerHasBlueprints;
        //existingItem.m_PrototypeIsInTheWorld = serializedItem.m_PrototypeIsInTheWorld;

        // Miscellaneous
        //existingItem.m_BlueprintRandomReleaseStage = serializedItem.m_BlueprintRandomReleaseStage;
        //existingItem.m_PrototypeRandomReleaseStage = serializedItem.m_PrototypeRandomReleaseStage;
        //existingItem.m_Count = serializedItem.m_Count;
        //existingItem.m_Expanded = serializedItem.m_Expanded;
        //existingItem.m_OverrideAmmo = serializedItem.m_OverrideAmmo;

        // If there's a TotalResearchTime property (it was in the serialized data but not in the original class)
        //if (existingItem.GetType().GetProperty("m_TotalResearchTime") != null)
        //{
        //    existingItem.GetType().GetProperty("m_TotalResearchTime").SetValue(existingItem, serializedItem.m_TotalResearchTime);
        //}

        // Update any event handlers or calculated properties
        if (existingItem.ItemDataChanged != null)
        {
            existingItem.ItemDataChanged.Invoke(existingItem);
        }
    }

    private ItemManager.ItemData CreateNewItem(SerializableItemData serializedItem, List<ItemManager.ItemData> existingItems)
    {
        // Find a similar existing item to copy from
        var similarItem = existingItems.FirstOrDefault(i => i.m_Slot == serializedItem.m_Slot);
        if (similarItem == null)
        {
            similarItem = existingItems.First(); // Fallback to the first item if no similar item found
        }

        // Create a new item by copying the similar item
        var newItem = new ItemManager.ItemData();
        CopyItemProperties(similarItem, newItem);

        // Update the new item with serialized data
        UpdateExistingItem(newItem, serializedItem);

        // Ensure the new item has the correct ID
        newItem.m_ID = serializedItem.m_ID;

        return newItem;
    }

    private void CopyItemProperties(ItemManager.ItemData source, ItemManager.ItemData destination)
    {
        // Basic properties
        destination.m_ID = source.m_ID;
        destination.m_FriendlyName = source.m_FriendlyName;
        destination.m_PrereqID = source.m_PrereqID;
        destination.m_Slot = source.m_Slot;
        destination.m_GearSubCategory = source.m_GearSubCategory;

        //destination.m_UIIcon = source.m_UIIcon != null ? source.m_UIIcon : destination.m_UIIcon;
        //destination.m_UIIcon = source.m_UIIcon; // Note: Sprites are Unity objects and can't be deep copied easily
        destination.m_WeaponType = source.m_WeaponType;
        destination.m_ValidWeaponAugmentationWeaponMask = source.m_ValidWeaponAugmentationWeaponMask;

        // Progression values
        destination.m_PrototypeProgressionValue = source.m_PrototypeProgressionValue;
        destination.m_BlueprintProgressionValue = source.m_BlueprintProgressionValue;
        destination.m_StealthVsCombat = source.m_StealthVsCombat;
        destination.m_Progression = source.m_Progression;

        // Modifiers and Abilities
        destination.m_Modifiers = source.m_Modifiers?.Select(m => new ModifierData5L
        {
            m_Type = m.m_Type,
            m_Ammount = m.m_Ammount,
            m_AmountModifier = m.m_AmountModifier,
            m_TimeOut = m.m_TimeOut
        }).ToArray() ?? new ModifierData5L[0];
        destination.m_AbilityIDs = new List<int>(source.m_AbilityIDs);
        destination.m_AbilityMasks = new List<int>(source.m_AbilityMasks);

        // Availability flags
        destination.m_AvailableToPlayer = source.m_AvailableToPlayer;
        destination.m_AvailableFor_ALPHA_BETA_EARLYACCESS = source.m_AvailableFor_ALPHA_BETA_EARLYACCESS;
        destination.m_PlayerStartsWithBlueprints = source.m_PlayerStartsWithBlueprints;
        destination.m_PlayerStartsWithPrototype = source.m_PlayerStartsWithPrototype;
        destination.m_PlayerCanResearchFromStart = source.m_PlayerCanResearchFromStart;

        // Research-related properties
        destination.m_ResearchStarted = source.m_ResearchStarted;
        destination.m_ResearchCost = source.m_ResearchCost;
        destination.m_ResearchProgress = source.m_ResearchProgress;
        //destination.m_ResearchTimeToDate = source.m_ResearchTimeToDate;
        //destination.m_ResearchCostToDate = source.m_ResearchCostToDate;
        destination.m_ResearchProgressionPerSecond = source.m_ResearchProgressionPerSecond;
        destination.m_CurrentResearchCost = source.m_CurrentResearchCost;
        destination.m_MinResearchersRequired = source.m_MinResearchersRequired;
        destination.m_InHouseResearchersResearching = source.m_InHouseResearchersResearching;
        destination.m_ExternalResearchersResearching = source.m_ExternalResearchersResearching;

        // Cost-related properties
        destination.m_BlueprintCost = source.m_BlueprintCost;
        destination.m_PrototypeCost = source.m_PrototypeCost;
        destination.m_FindBlueprintCost = source.m_FindBlueprintCost;
        destination.m_FindPrototypeCost = source.m_FindPrototypeCost;
        destination.m_Cost = source.m_Cost;

        // State flags
        destination.m_ItemHasBeenLocated = source.m_ItemHasBeenLocated;
        destination.m_PlayerHasPrototype = source.m_PlayerHasPrototype;
        destination.m_PlayerHasBlueprints = source.m_PlayerHasBlueprints;
        destination.m_PrototypeIsInTheWorld = source.m_PrototypeIsInTheWorld;

        // Miscellaneous
        destination.m_BlueprintRandomReleaseStage = source.m_BlueprintRandomReleaseStage;
        destination.m_PrototypeRandomReleaseStage = source.m_PrototypeRandomReleaseStage;
        destination.m_Count = source.m_Count;
        destination.m_Expanded = source.m_Expanded;
        destination.m_OverrideAmmo = source.m_OverrideAmmo;

        // If there's a TotalResearchTime property
        //if (source.GetType().GetProperty("m_TotalResearchTime") != null && destination.GetType().GetProperty("m_TotalResearchTime") != null)
        //{
        //    var sourceTotalResearchTime = source.GetType().GetProperty("m_TotalResearchTime").GetValue(source);
        //    destination.GetType().GetProperty("m_TotalResearchTime").SetValue(destination, sourceTotalResearchTime);
        //}

        //// Copy ResearchDataPoints if it exists
        //if (source.GetType().GetProperty("m_ResearchDataPoints") != null && destination.GetType().GetProperty("m_ResearchDataPoints") != null)
        //{
        //    var sourceResearchDataPoints = source.GetType().GetProperty("m_ResearchDataPoints").GetValue(source) as List<ResearchDataPoint>;
        //    if (sourceResearchDataPoints != null)
        //    {
        //        var copiedResearchDataPoints = sourceResearchDataPoints.Select(rdp => new ResearchDataPoint
        //        {
        //            Time = rdp.Time,
        //            ResearchProgress = rdp.ResearchProgress,
        //            ResearchProgressPct = rdp.ResearchProgressPct,
        //            ResearchCost = rdp.ResearchCost,
        //            ResearchProgressPerSecond = rdp.ResearchProgressPerSecond
        //        }).ToList();
        //        destination.GetType().GetProperty("m_ResearchDataPoints").SetValue(destination, copiedResearchDataPoints);
        //    }
        //}

        // Copy the event handler if it exists
        if (source.ItemDataChanged != null)
        {
            destination.ItemDataChanged = (Action<ItemManager.ItemData>)source.ItemDataChanged.Clone();
        }
        if (destination.ItemDataChanged != null)
        {
            destination.ItemDataChanged.Invoke(destination);
        }
    }

    public void SaveItemDefinitionsToFile()
    {
        try
        {
            _itemDefinitions = Manager.GetItemManager().GetAllItems()
                .Select(item => new SerializableItemData(item))
                .ToList();

            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<SerializableItemData>));
            string path = Path.Combine(Manager.GetPluginManager().PluginPath, SAVE_FILE_NAME);
            using (var streamWriter = new StreamWriter(path))
            {
                serializer.Serialize(streamWriter, _itemDefinitions);
            }

            SRInfoHelper.Log("Saved " + _itemDefinitions.Count + " item definitions to " + path);
        }
        catch (System.Exception e)
        {
            SRInfoHelper.Log("Error saving item definitions: " + e.Message);
        }
    }
}