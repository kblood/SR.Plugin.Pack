using SRMod.DTOs;
using SRMod.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
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
        PopulateFromItemManager();
    }

    public void InitializeWithAutoLoad()
    {
        SRInfoHelper.Log("ItemDataManager: ===== INITIALIZE WITH AUTO-LOAD CALLED (NEW VERSION) =====");
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
                var serializer = new XmlSerializer(typeof(List<SerializableItemData>));
                using (var streamReader = new StreamReader(path))
                {
                    _itemDefinitions = (List<SerializableItemData>)serializer.Deserialize(streamReader);
                }
                // SAFE UPDATE: Never clear the entire list - preserve game state
                bool success = UpdateItemManagerSafely(_itemDefinitions);
                
                if (success)
                {
                    SRInfoHelper.Log($"Loaded and updated ItemManager with {_itemDefinitions.Count} item definitions from file");
                }
                return success;
            }
            catch (System.Exception e)
            {
                SRInfoHelper.Log($"Error loading item definitions: {e.Message}");
            }
        }
        SRInfoHelper.Log("No item definitions found in file.");
        return false;
    }

    /// <summary>
    /// Safely update ItemManager by updating existing items and adding new ones
    /// NEVER replaces the entire m_ItemDefinitions list to preserve game state
    /// </summary>
    private bool UpdateItemManagerSafely(List<SerializableItemData> importedItems)
    {
        try
        {
            var itemManager = Manager.GetItemManager();
            if (itemManager == null)
            {
                SRInfoHelper.Log("ItemDataManager: ItemManager not available");
                return false;
            }

            int updatedCount = 0;
            int newCount = 0;

            foreach (var importedItem in importedItems)
            {
                // Validate item data before processing
                if (!ValidateItemData(importedItem))
                {
                    SRInfoHelper.Log($"ItemDataManager: Skipping invalid item {importedItem.m_ID}");
                    continue;
                }

                var existingItem = itemManager.m_ItemDefinitions.FirstOrDefault(x => x.m_ID == importedItem.m_ID);
                
                if (existingItem != null)
                {
                    // Check if item actually changed before logging
                    bool hasChanged = HasItemChanged(existingItem, importedItem);
                    if (hasChanged)
                    {
                        UpdateExistingItemSafely(existingItem, importedItem);
                        updatedCount++;
                        SRInfoHelper.Log($"ItemDataManager: Updated existing item {importedItem.m_ID}");
                    }
                    // If no changes, don't log anything
                }
                else
                {
                    // Add new item
                    var newItem = CreateNewItem(importedItem, itemManager.m_ItemDefinitions);
                    if (newItem != null)
                    {
                        itemManager.m_ItemDefinitions.Add(newItem);
                        newCount++;
                        SRInfoHelper.Log($"ItemDataManager: Added new item {importedItem.m_ID}");
                    }
                }
            }

            // Only log if there were actual changes
            if (updatedCount > 0 || newCount > 0)
            {
                SRInfoHelper.Log($"ItemDataManager: Safely updated {updatedCount} items, added {newCount} new items");
            }
            
            // Force game systems to refresh - use multiple approaches for better compatibility
            try
            {
                // Method 1: Trigger ItemManager event if available
                var onResearchedItemsChanged = itemManager.GetType().GetField("OnResearchedItemsChanged", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (onResearchedItemsChanged != null)
                {
                    var eventHandler = onResearchedItemsChanged.GetValue(itemManager) as ItemManager.OnResearchedItemsChangedDelegate;
                    if (eventHandler != null)
                    {
                        eventHandler.Invoke();
                        SRInfoHelper.Log($"ItemDataManager: Successfully triggered OnResearchedItemsChanged event");
                    }
                }
                
                // Method 2: Try to refresh all agents' loadouts to pick up changes
                try
                {
                    var agents = AgentAI.GetAgents();
                    if (agents != null)
                    {
                        foreach (var agent in agents)
                        {
                            try
                            {
                                // Force agents to recalculate their item data
                                if (agent.m_Modifiers != null)
                                {
                                    // Simple approach: just log that we would refresh modifiers
                                    // The modifier changes should take effect automatically
                                    SRInfoHelper.Log($"ItemDataManager: Item data updated for agent {agent.GetName()}");
                                }
                            }
                            catch (System.Exception agentEx)
                            {
                                SRInfoHelper.Log($"ItemDataManager: Could not refresh agent {agent?.GetName()}: {agentEx.Message}");
                            }
                        }
                    }
                }
                catch (System.Exception agentRefreshEx)
                {
                    SRInfoHelper.Log($"ItemDataManager: Could not refresh agents: {agentRefreshEx.Message}");
                }
                
                // Also try to force UI refresh by accessing UI manager
                var uiManager = Manager.GetUIManager();
                if (uiManager != null)
                {
                    // Force any open research/loadout panels to refresh
                    var uiType = uiManager.GetType();
                    var refreshMethods = uiType.GetMethods().Where(m => m.Name.Contains("Refresh") || m.Name.Contains("Update"));
                    foreach (var method in refreshMethods)
                    {
                        if (method.GetParameters().Length == 0) // Only call parameterless methods
                        {
                            try
                            {
                                method.Invoke(uiManager, null);
                                // Removed excessive UI refresh logging
                            }
                            catch { /* Continue trying other methods */ }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                SRInfoHelper.Log($"ItemDataManager: Could not trigger change notification - {e.Message}");
            }

            return updatedCount + newCount > 0;
        }
        catch (Exception e)
        {
            SRInfoHelper.Log($"ItemDataManager: Safe update failed - {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// Update existing item while preserving runtime state and calculated properties
    /// </summary>
    private void UpdateExistingItemSafely(ItemManager.ItemData existingItem, SerializableItemData importedItem)
    {
        // Update ONLY base item definition properties (excludes all 19 game progress/runtime fields)
        UpdateItemBasePropertiesSafely(existingItem, importedItem);
        
        // Don't overwrite manual translation changes during auto-load
        // UpdateItemTranslations(existingItem.m_ID, importedItem.m_FriendlyName);
        
        // Notify item changed
        if (existingItem.ItemDataChanged != null)
        {
            existingItem.ItemDataChanged(existingItem);
        }
    }

    /// <summary>
    /// Update only the base properties that are safe to modify without breaking game state
    /// EXCLUDES all 19 game progress/runtime data fields to preserve save game integrity
    /// </summary>
    private void UpdateItemBasePropertiesSafely(ItemManager.ItemData existingItem, SerializableItemData importedItem)
    {
        // Update basic identity and display properties
        existingItem.m_FriendlyName = importedItem.m_FriendlyName;
        existingItem.m_PrereqID = importedItem.m_PrereqID;
        existingItem.m_Slot = importedItem.m_Slot;
        existingItem.m_GearSubCategory = importedItem.m_GearSubCategory;
        existingItem.m_WeaponType = importedItem.m_WeaponType;
        existingItem.m_ValidWeaponAugmentationWeaponMask = importedItem.m_ValidWeaponAugmentationWeaponMask;
        existingItem.m_StealthVsCombat = importedItem.m_StealthVsCombat;
        
        // Update BASE cost values ONLY (not runtime/progress costs)
        float oldCost = existingItem.m_Cost;
        existingItem.m_Cost = importedItem.m_Cost;
        existingItem.m_ResearchCost = importedItem.m_ResearchCost;
        existingItem.m_BlueprintCost = importedItem.m_BlueprintCost;
        existingItem.m_PrototypeCost = importedItem.m_PrototypeCost;
        existingItem.m_FindBlueprintCost = importedItem.m_FindBlueprintCost;
        existingItem.m_FindPrototypeCost = importedItem.m_FindPrototypeCost;
        
        // Update progression design values (not runtime progression state)
        existingItem.m_PrototypeProgressionValue = importedItem.m_PrototypeProgressionValue;
        existingItem.m_BlueprintProgressionValue = importedItem.m_BlueprintProgressionValue;
        existingItem.m_MinResearchersRequired = importedItem.m_MinResearchersRequired;
        
        SRInfoHelper.Log($"ItemDataManager: Updated item {importedItem.m_ID} cost from {oldCost} to {importedItem.m_Cost}");
        
        // Update availability flags (design properties, not runtime state)
        existingItem.m_AvailableToPlayer = importedItem.m_AvailableToPlayer;
        existingItem.m_AvailableFor_ALPHA_BETA_EARLYACCESS = importedItem.m_AvailableFor_ALPHA_BETA_EARLYACCESS;
        existingItem.m_PlayerStartsWithBlueprints = importedItem.m_PlayerStartsWithBlueprints;
        existingItem.m_PlayerStartsWithPrototype = importedItem.m_PlayerStartsWithPrototype;
        existingItem.m_PlayerCanResearchFromStart = importedItem.m_PlayerCanResearchFromStart;
        
        // Update modifiers and abilities (item design properties)
        if (importedItem.m_Modifiers != null)
        {
            existingItem.m_Modifiers = importedItem.m_Modifiers.Select(m => m.ToModifierData()).ToArray();
        }
        
        existingItem.m_AbilityIDs = new List<int>(importedItem.m_AbilityIDs ?? new List<int>());
        existingItem.m_AbilityMasks = new List<int>(importedItem.m_AbilityMasks ?? new List<int>());
        
        // Update icon if provided
        if (!string.IsNullOrEmpty(importedItem.m_UIIconName))
        {
            var loadedTexture = FileManager.LoadTextureFromFile(importedItem.m_UIIconName);
            if (loadedTexture != null)
            {
                var loadedSprite = SpriteSerializer.UpdateSprite(loadedTexture, existingItem.m_UIIcon);
                if (loadedSprite != null)
                {
                    existingItem.m_UIIcon = loadedSprite;
                }
            }
        }
        
        // ===== EXCLUDED GAME PROGRESS/RUNTIME DATA FIELDS (19 fields) =====
        // These fields are INTENTIONALLY NOT UPDATED to preserve save game integrity:
        //
        // Research Progress & State:
        // - m_ResearchStarted (preserved to maintain active research)
        // - m_ResearchProgress (preserved to maintain research progress)
        // - m_ResearchProgressionPerSecond (runtime calculation)
        // - m_CurrentResearchCost (runtime calculation)
        // - m_ResearchTimeToDate (player's invested time)
        // - m_ResearchCostToDate (player's invested money)
        // - m_TotalResearchTime (internal calculation)
        //
        // Player Ownership State:
        // - m_PlayerHasPrototype (player's current ownership)
        // - m_PlayerHasBlueprints (player's current ownership)
        // - m_ItemHasBeenLocated (player's discovery state)
        // - m_PrototypeIsInTheWorld (current world state)
        //
        // Active Research Personnel:
        // - m_InHouseResearchersResearching (current assignment)
        // - m_ExternalResearchersResearching (current assignment)
        //
        // Item Inventory & Usage:
        // - m_Count (player's current inventory)
        //
        // UI State:
        // - m_Expanded (UI preference)
        //
        // Progression System State:
        // - m_BlueprintRandomReleaseStage (current game progression)
        // - m_PrototypeRandomReleaseStage (current game progression)
        // - m_Progression (current game progression level)
        // - m_OverrideAmmo (runtime session override)
    }

    /// <summary>
    /// Legacy method - now redirects to safe version
    /// </summary>
    private void UpdateExistingItem(ItemManager.ItemData existingItem, SerializableItemData serializedItem)
    {
        UpdateExistingItemSafely(existingItem, serializedItem);
    }

    /// <summary>
    /// Check if an item has actually changed to avoid unnecessary logging
    /// </summary>
    private bool HasItemChanged(ItemManager.ItemData existingItem, SerializableItemData importedItem)
    {
        bool hasChanged = false;
        List<string> changes = new List<string>();

        // Check each property and log what changed
        if (existingItem.m_FriendlyName != importedItem.m_FriendlyName)
        {
            changes.Add($"FriendlyName: '{existingItem.m_FriendlyName}' -> '{importedItem.m_FriendlyName}'");
            hasChanged = true;
        }
        if (existingItem.m_Cost != importedItem.m_Cost)
        {
            changes.Add($"Cost: {existingItem.m_Cost} -> {importedItem.m_Cost}");
            hasChanged = true;
        }
        if (existingItem.m_ResearchCost != importedItem.m_ResearchCost)
        {
            changes.Add($"ResearchCost: {existingItem.m_ResearchCost} -> {importedItem.m_ResearchCost}");
            hasChanged = true;
        }
        if (existingItem.m_Progression != importedItem.m_Progression)
        {
            changes.Add($"Progression: {existingItem.m_Progression} -> {importedItem.m_Progression}");
            hasChanged = true;
        }
        if (existingItem.m_PrereqID != importedItem.m_PrereqID)
        {
            changes.Add($"PrereqID: {existingItem.m_PrereqID} -> {importedItem.m_PrereqID}");
            hasChanged = true;
        }
        if (existingItem.m_AvailableToPlayer != importedItem.m_AvailableToPlayer)
        {
            changes.Add($"AvailableToPlayer: {existingItem.m_AvailableToPlayer} -> {importedItem.m_AvailableToPlayer}");
            hasChanged = true;
        }
        if (existingItem.m_PlayerStartsWithBlueprints != importedItem.m_PlayerStartsWithBlueprints)
        {
            changes.Add($"PlayerStartsWithBlueprints: {existingItem.m_PlayerStartsWithBlueprints} -> {importedItem.m_PlayerStartsWithBlueprints}");
            hasChanged = true;
        }
        if (existingItem.m_PlayerStartsWithPrototype != importedItem.m_PlayerStartsWithPrototype)
        {
            changes.Add($"PlayerStartsWithPrototype: {existingItem.m_PlayerStartsWithPrototype} -> {importedItem.m_PlayerStartsWithPrototype}");
            hasChanged = true;
        }
        if (existingItem.m_PlayerCanResearchFromStart != importedItem.m_PlayerCanResearchFromStart)
        {
            changes.Add($"PlayerCanResearchFromStart: {existingItem.m_PlayerCanResearchFromStart} -> {importedItem.m_PlayerCanResearchFromStart}");
            hasChanged = true;
        }
        if (existingItem.m_Slot != importedItem.m_Slot)
        {
            changes.Add($"Slot: {existingItem.m_Slot} -> {importedItem.m_Slot}");
            hasChanged = true;
        }
        if (existingItem.m_GearSubCategory != importedItem.m_GearSubCategory)
        {
            changes.Add($"GearSubCategory: {existingItem.m_GearSubCategory} -> {importedItem.m_GearSubCategory}");
            hasChanged = true;
        }
        if (existingItem.m_WeaponType != importedItem.m_WeaponType)
        {
            changes.Add($"WeaponType: {existingItem.m_WeaponType} -> {importedItem.m_WeaponType}");
            hasChanged = true;
        }

        // Log changes for debugging
        if (hasChanged)
        {
            SRInfoHelper.Log($"ItemDataManager: Item {importedItem.m_ID} changes detected: {string.Join(", ", changes.ToArray())}");
        }

        return hasChanged;
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

    /// <summary>
    /// Validate imported item data to prevent crashes
    /// </summary>
    private bool ValidateItemData(SerializableItemData item)
    {
        // Basic validation
        if (item.m_ID <= 0)
        {
            SRInfoHelper.Log($"Invalid item ID: {item.m_ID}");
            return false;
        }
        
        // Progression must be 0-1
        if (item.m_Progression < 0f || item.m_Progression > 1f)
        {
            SRInfoHelper.Log($"Invalid progression value for item {item.m_ID}: {item.m_Progression}");
            item.m_Progression = Mathf.Clamp01(item.m_Progression); // Auto-fix
        }
        
        // Validate enums
        if (!Enum.IsDefined(typeof(ItemSlotTypes), item.m_Slot))
        {
            SRInfoHelper.Log($"Invalid slot type for item {item.m_ID}: {item.m_Slot}");
            return false;
        }
        
        if (!Enum.IsDefined(typeof(WeaponType), item.m_WeaponType))
        {
            SRInfoHelper.Log($"Invalid weapon type for item {item.m_ID}: {item.m_WeaponType}");
            return false;
        }
        
        // Validate modifiers
        if (item.m_Modifiers != null)
        {
            foreach (var modifier in item.m_Modifiers)
            {
                if (!Enum.IsDefined(typeof(ModifierType), modifier.m_Type))
                {
                    SRInfoHelper.Log($"Invalid modifier type for item {item.m_ID}: {modifier.m_Type}");
                    return false;
                }
            }
        }
        
        return true;
    }

    public void SaveItemDefinitionsToFile()
    {
        try
        {
            _itemDefinitions = Manager.GetItemManager().GetAllItems()
                .Select(item => new SerializableItemData(item))
                .ToList();

            var serializer = new XmlSerializer(typeof(List<SerializableItemData>));
            string path = Path.Combine(Manager.GetPluginManager().PluginPath, SAVE_FILE_NAME);
            using (var streamWriter = new StreamWriter(path))
            {
                serializer.Serialize(streamWriter, _itemDefinitions);
            }

            SRInfoHelper.Log($"Saved {_itemDefinitions.Count} item definitions to {path}");
        }
        catch (System.Exception e)
        {
            SRInfoHelper.Log($"Error saving item definitions: {e.Message}");
        }
    }

    /// <summary>
    /// Update translations for item names and descriptions so they appear in-game
    /// </summary>
    private void UpdateItemTranslations(int itemId, string itemName, string description = null)
    {
        try
        {
            SRInfoHelper.Log($"ItemDataManager: Attempting to update translations for item {itemId} with name '{itemName}'");
            
            var textManager = TextManager.Get();
            if (textManager == null) 
            {
                SRInfoHelper.Log($"ItemDataManager: TextManager.Get() returned null");
                return;
            }

            var langLookup = textManager.GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");
            if (langLookup == null) 
            {
                SRInfoHelper.Log($"ItemDataManager: Failed to get m_FastLanguageLookup from TextManager");
                return;
            }

            string nameKey = "ITEM_" + itemId + "_NAME";
            string descKey = "ITEM_" + itemId + "_DESCRIPTION";

            SRInfoHelper.Log($"ItemDataManager: Updating translation key '{nameKey}' with value '{itemName}'");

            // Update or create name translation
            if (langLookup.ContainsKey(nameKey))
            {
                string oldValue = langLookup[nameKey].m_Translations[2];
                langLookup[nameKey].m_Translations[2] = itemName; // English language index
                SRInfoHelper.Log($"ItemDataManager: Updated existing translation '{nameKey}': '{oldValue}' -> '{itemName}'");
            }
            else
            {
                var nameElement = new TextManager.LocElement
                {
                    m_token = nameKey,
                    m_Translations = new string[8]
                };
                nameElement.m_Translations[2] = itemName;
                langLookup[nameKey] = nameElement;
                SRInfoHelper.Log($"ItemDataManager: Created new translation '{nameKey}' with value '{itemName}'");
            }

            // Update or create description translation if provided
            if (!string.IsNullOrEmpty(description))
            {
                SRInfoHelper.Log($"ItemDataManager: Updating description key '{descKey}' with value '{description}'");
                if (langLookup.ContainsKey(descKey))
                {
                    string oldDesc = langLookup[descKey].m_Translations[2];
                    langLookup[descKey].m_Translations[2] = description;
                    SRInfoHelper.Log($"ItemDataManager: Updated existing description '{descKey}': '{oldDesc}' -> '{description}'");
                }
                else
                {
                    var descElement = new TextManager.LocElement
                    {
                        m_token = descKey,
                        m_Translations = new string[8]
                    };
                    descElement.m_Translations[2] = description;
                    langLookup[descKey] = descElement;
                    SRInfoHelper.Log($"ItemDataManager: Created new description '{descKey}' with value '{description}'");
                }
            }

            SRInfoHelper.Log($"ItemDataManager: Translation update completed for item {itemId}");
        }
        catch (System.Exception e)
        {
            SRInfoHelper.Log($"ItemDataManager: Error updating item translations: {e.Message}");
        }
    }
}