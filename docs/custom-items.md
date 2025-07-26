# Custom Items and Item System

## Overview

Satellite Reign's item system manages gear, augmentations, weapons, and other equipment through the `ItemManager` and related data structures. This guide covers how to create custom items, modify existing items, and work with the item serialization system.

## Item System Architecture

### ItemSlotTypes Enum
Items are categorized by slot types:

```csharp
public enum ItemSlotTypes
{
    Gear = 0,
    AugmentationHead = 1,
    AugmentationBody = 2,
    AugmentationArms = 3,
    AugmentationLegs = 4,
    WeaponPistol = 5,
    Weapon = 6,
    WeaponAugmentation = 7,
    Max = 8
}
```

### ItemSubCategories Enum
Items have subcategories for more specific classification:

```csharp
public enum ItemSubCategories
{
    Standard = 0,
    Drone = 1,
    WeaponAmmo = 2,
    ArmorBody = 3,
    ArmorHead = 4,
    Shields = 5,
    StealthGenerators = 6
}
```

### Item Data Structure
Items are represented by `ItemData` objects containing:

- **Basic Properties**: ID, Name, Description, Cost
- **Slot Information**: Type, SubCategory, StackSize
- **Modifiers**: Effects and bonuses the item provides
- **Visual Data**: Icons and sprites

## Accessing the Item System

### Getting the ItemManager
```csharp
ItemManager itemManager = Manager.GetItemManager();
```

### Accessing Item Data
```csharp
// Get all items
foreach (ItemManager.ItemData itemData in itemManager.GetAllItems())
{
    Debug.Log($"Item: {itemData.m_Name} (ID: {itemData.m_ID})");
}

// Find specific item by ID
ItemManager.ItemData specificItem = itemManager.GetItemData(itemId);

// Find items by type
var gearItems = itemManager.GetAllItems()
    .Where(item => item.m_ItemSlotType == ItemSlotTypes.Gear);
```

## Item Data Serialization

### SerializableItemData Class
For custom item creation and modification, use the serializable data transfer object:

```csharp
[System.Serializable]
public class SerializableItemData
{
    public int m_ID { get; set; }
    public string m_Name { get; set; }
    public string m_Description { get; set; }
    public int m_Cost { get; set; }
    public ItemSlotTypes m_ItemSlotType { get; set; }
    public ItemSubCategories m_ItemSubCategory { get; set; }
    public int m_StackSize { get; set; }
    public List<SerializableModifierData> m_Modifiers { get; set; }
    public string m_IconPath { get; set; }
    
    public SerializableItemData()
    {
        m_Modifiers = new List<SerializableModifierData>();
    }
}
```

### SerializableModifierData Class
Modifiers define the effects items have on agents:

```csharp
[System.Serializable]
public class SerializableModifierData
{
    public ModifierType m_Type { get; set; }
    public float m_Amount { get; set; }
    public ModifierType m_AmountModifier { get; set; }
    public bool m_IsPercentage { get; set; }
}
```

## Creating Custom Items

### Basic Custom Item Creation
```csharp
public class CustomItemCreator
{
    public SerializableItemData CreateSuperShield()
    {
        var superShield = new SerializableItemData
        {
            m_ID = 1000, // Use high ID to avoid conflicts
            m_Name = "Super Shield Generator",
            m_Description = "An advanced shield generator with enhanced capacity and regeneration.",
            m_Cost = 5000,
            m_ItemSlotType = ItemSlotTypes.Gear,
            m_ItemSubCategory = ItemSubCategories.Shields,
            m_StackSize = 1,
            m_IconPath = "custom_super_shield.png"
        };
        
        // Add modifiers
        superShield.m_Modifiers.Add(new SerializableModifierData
        {
            m_Type = ModifierType.ShieldCapacity,
            m_Amount = 200f,
            m_IsPercentage = false
        });
        
        superShield.m_Modifiers.Add(new SerializableModifierData
        {
            m_Type = ModifierType.ShieldRegenRate,
            m_Amount = 50f,
            m_IsPercentage = false
        });
        
        return superShield;
    }
    
    public SerializableItemData CreateAdvancedArmor()
    {
        var advancedArmor = new SerializableItemData
        {
            m_ID = 1001,
            m_Name = "Titanium Combat Armor",
            m_Description = "Military-grade titanium armor providing superior protection.",
            m_Cost = 7500,
            m_ItemSlotType = ItemSlotTypes.AugmentationBody,
            m_ItemSubCategory = ItemSubCategories.ArmorBody,
            m_StackSize = 1
        };
        
        // Add multiple modifiers
        advancedArmor.m_Modifiers.AddRange(new[]
        {
            new SerializableModifierData
            {
                m_Type = ModifierType.HealthOffset,
                m_Amount = 100f,
                m_IsPercentage = false
            },
            new SerializableModifierData
            {
                m_Type = ModifierType.SpeedMultiplier,
                m_Amount = -0.1f, // 10% speed reduction
                m_IsPercentage = true
            },
            new SerializableModifierData
            {
                m_Type = ModifierType.DamageResistance,
                m_Amount = 0.25f, // 25% damage resistance
                m_IsPercentage = true
            }
        });
        
        return advancedArmor;
    }
}
```

### Item Collection Management
```csharp
public class CustomItemCollection
{
    private List<SerializableItemData> customItems = new List<SerializableItemData>();
    
    public void CreateItemCollection()
    {
        var creator = new CustomItemCreator();
        
        // Create various custom items
        customItems.Add(creator.CreateSuperShield());
        customItems.Add(creator.CreateAdvancedArmor());
        customItems.Add(CreateHealingStim());
        customItems.Add(CreateStealthCloak());
        customItems.Add(CreateEnergyBooster());
    }
    
    private SerializableItemData CreateHealingStim()
    {
        return new SerializableItemData
        {
            m_ID = 1002,
            m_Name = "Advanced Medical Stim",
            m_Description = "Rapidly restores health and provides temporary regeneration.",
            m_Cost = 500,
            m_ItemSlotType = ItemSlotTypes.Gear,
            m_ItemSubCategory = ItemSubCategories.Standard,
            m_StackSize = 5,
            m_Modifiers = new List<SerializableModifierData>
            {
                new SerializableModifierData
                {
                    m_Type = ModifierType.HealthRegenRate,
                    m_Amount = 25f,
                    m_IsPercentage = false
                }
            }
        };
    }
    
    private SerializableItemData CreateStealthCloak()
    {
        return new SerializableItemData
        {
            m_ID = 1003,
            m_Name = "Prototype Stealth Cloak",
            m_Description = "Experimental cloaking device with extended duration.",
            m_Cost = 12000,
            m_ItemSlotType = ItemSlotTypes.Gear,
            m_ItemSubCategory = ItemSubCategories.StealthGenerators,
            m_StackSize = 1,
            m_Modifiers = new List<SerializableModifierData>
            {
                new SerializableModifierData
                {
                    m_Type = ModifierType.StealthDuration,
                    m_Amount = 200f, // 200% longer stealth
                    m_IsPercentage = true
                },
                new SerializableModifierData
                {
                    m_Type = ModifierType.EnergyMaxOffset,
                    m_Amount = -50f, // Energy cost
                    m_IsPercentage = false
                }
            }
        };
    }
    
    private SerializableItemData CreateEnergyBooster()
    {
        return new SerializableItemData
        {
            m_ID = 1004,
            m_Name = "Neural Energy Amplifier",
            m_Description = "Increases maximum energy capacity and regeneration rate.",
            m_Cost = 3000,
            m_ItemSlotType = ItemSlotTypes.AugmentationHead,
            m_ItemSubCategory = ItemSubCategories.Standard,
            m_StackSize = 1,
            m_Modifiers = new List<SerializableModifierData>
            {
                new SerializableModifierData
                {
                    m_Type = ModifierType.EnergyMaxOffset,
                    m_Amount = 150f,
                    m_IsPercentage = false
                },
                new SerializableModifierData
                {
                    m_Type = ModifierType.EnergyRegenRate,
                    m_Amount = 3f,
                    m_IsPercentage = false
                }
            }
        };
    }
    
    public List<SerializableItemData> GetAllCustomItems()
    {
        return new List<SerializableItemData>(customItems);
    }
}
```

## Item Data Persistence

### Saving and Loading Custom Items
```csharp
public class ItemDataManager
{
    private const string CUSTOM_ITEMS_FILE = "customItems.xml";
    
    public void SaveCustomItems(List<SerializableItemData> items)
    {
        try
        {
            FileManager.SaveAsXML(items, CUSTOM_ITEMS_FILE);
            Debug.Log($"Saved {items.Count} custom items to {CUSTOM_ITEMS_FILE}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save custom items: {e.Message}");
        }
    }
    
    public List<SerializableItemData> LoadCustomItems()
    {
        try
        {
            // Note: This assumes a generic LoadXML method exists
            var items = FileManager.LoadItemDataXML(CUSTOM_ITEMS_FILE, FileManager.ExecPath);
            Debug.Log($"Loaded {items?.Count ?? 0} custom items from {CUSTOM_ITEMS_FILE}");
            return items ?? new List<SerializableItemData>();
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load custom items: {e.Message}");
            return new List<SerializableItemData>();
        }
    }
    
    public void ExportItemTemplate(SerializableItemData item, string fileName)
    {
        try
        {
            FileManager.SaveAsXML(item, $"template_{fileName}.xml");
            Debug.Log($"Exported item template: {fileName}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to export item template: {e.Message}");
        }
    }
}
```

### XML Serialization Example
Custom items are saved in XML format:

```xml
<?xml version="1.0" encoding="utf-8"?>
<SerializableItemData>
  <m_ID>1000</m_ID>
  <m_Name>Super Shield Generator</m_Name>
  <m_Description>An advanced shield generator with enhanced capacity and regeneration.</m_Description>
  <m_Cost>5000</m_Cost>
  <m_ItemSlotType>Gear</m_ItemSlotType>
  <m_ItemSubCategory>Shields</m_ItemSubCategory>
  <m_StackSize>1</m_StackSize>
  <m_IconPath>custom_super_shield.png</m_IconPath>
  <m_Modifiers>
    <SerializableModifierData>
      <m_Type>ShieldCapacity</m_Type>
      <m_Amount>200</m_Amount>
      <m_IsPercentage>false</m_IsPercentage>
    </SerializableModifierData>
    <SerializableModifierData>
      <m_Type>ShieldRegenRate</m_Type>
      <m_Amount>50</m_Amount>
      <m_IsPercentage>false</m_IsPercentage>
    </SerializableModifierData>
  </m_Modifiers>
</SerializableItemData>
```

## Dynamic Item System

### Runtime Item Creation and Management
```csharp
public class DynamicItemSystem : ISrPlugin
{
    private ItemDataManager itemManager;
    private List<SerializableItemData> runtimeItems;
    private bool itemsInitialized = false;
    
    public void Initialize()
    {
        itemManager = new ItemDataManager();
        runtimeItems = new List<SerializableItemData>();
        LoadCustomItems();
    }
    
    public void Update()
    {
        if (!Manager.Get().GameInProgress)
            return;
            
        if (!itemsInitialized)
        {
            InitializeRuntimeItems();
            itemsInitialized = true;
        }
        
        HandleItemCommands();
    }
    
    private void LoadCustomItems()
    {
        try
        {
            runtimeItems = itemManager.LoadCustomItems();
            
            if (runtimeItems.Count == 0)
            {
                CreateDefaultItems();
                itemManager.SaveCustomItems(runtimeItems);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load custom items: {e.Message}");
            CreateDefaultItems();
        }
    }
    
    private void CreateDefaultItems()
    {
        var collection = new CustomItemCollection();
        collection.CreateItemCollection();
        runtimeItems = collection.GetAllCustomItems();
    }
    
    private void InitializeRuntimeItems()
    {
        foreach (var item in runtimeItems)
        {
            RegisterCustomItem(item);
        }
        
        Manager.GetUIManager().ShowMessagePopup(
            $"Loaded {runtimeItems.Count} custom items", 3);
    }
    
    private void RegisterCustomItem(SerializableItemData item)
    {
        try
        {
            // Note: Actual item registration would require deeper game integration
            // This is a conceptual example of the process
            Debug.Log($"Registered custom item: {item.m_Name} (ID: {item.m_ID})");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to register item {item.m_Name}: {e.Message}");
        }
    }
    
    private void HandleItemCommands()
    {
        if (Input.GetKeyDown(KeyCode.I) && Input.GetKey(KeyCode.LeftControl))
        {
            ShowItemCreationMenu();
        }
        
        if (Input.GetKeyDown(KeyCode.L) && Input.GetKey(KeyCode.LeftControl))
        {
            ListCustomItems();
        }
    }
    
    private void ShowItemCreationMenu()
    {
        // This would typically show a custom UI for item creation
        Manager.GetUIManager().ShowMessagePopup("Item creation menu (not implemented in this example)", 3);
    }
    
    private void ListCustomItems()
    {
        string itemList = "Custom Items:\n";
        foreach (var item in runtimeItems)
        {
            itemList += $"- {item.m_Name} (${item.m_Cost})\n";
        }
        
        Manager.GetUIManager().ShowMessagePopup(itemList, 8);
    }
    
    public string GetName() => "Dynamic Item System";
}
```

## Item Modification and Enhancement

### Item Upgrading System
```csharp
public class ItemUpgradeSystem
{
    private Dictionary<int, int> itemUpgradeLevels = new Dictionary<int, int>();
    
    public SerializableItemData UpgradeItem(SerializableItemData baseItem, int upgradeLevel)
    {
        var upgradedItem = CloneItem(baseItem);
        upgradedItem.m_Name = $"{baseItem.m_Name} +{upgradeLevel}";
        upgradedItem.m_Cost = (int)(baseItem.m_Cost * (1 + upgradeLevel * 0.5f));
        
        // Enhance modifiers based on upgrade level
        foreach (var modifier in upgradedItem.m_Modifiers)
        {
            float enhancement = 1f + (upgradeLevel * 0.2f);
            modifier.m_Amount *= enhancement;
        }
        
        return upgradedItem;
    }
    
    private SerializableItemData CloneItem(SerializableItemData original)
    {
        var clone = new SerializableItemData
        {
            m_ID = original.m_ID + 10000, // Offset for upgraded items
            m_Name = original.m_Name,
            m_Description = original.m_Description,
            m_Cost = original.m_Cost,
            m_ItemSlotType = original.m_ItemSlotType,
            m_ItemSubCategory = original.m_ItemSubCategory,
            m_StackSize = original.m_StackSize,
            m_IconPath = original.m_IconPath
        };
        
        // Deep copy modifiers
        foreach (var modifier in original.m_Modifiers)
        {
            clone.m_Modifiers.Add(new SerializableModifierData
            {
                m_Type = modifier.m_Type,
                m_Amount = modifier.m_Amount,
                m_AmountModifier = modifier.m_AmountModifier,
                m_IsPercentage = modifier.m_IsPercentage
            });
        }
        
        return clone;
    }
}
```

### Item Set System
Create item sets with bonus effects:

```csharp
public class ItemSetSystem
{
    [System.Serializable]
    public class ItemSet
    {
        public string SetName { get; set; }
        public List<int> ItemIDs { get; set; }
        public List<SerializableModifierData> SetBonuses { get; set; }
        public string SetDescription { get; set; }
        
        public ItemSet()
        {
            ItemIDs = new List<int>();
            SetBonuses = new List<SerializableModifierData>();
        }
    }
    
    public ItemSet CreateCyberNinjaSet()
    {
        return new ItemSet
        {
            SetName = "Cyber Ninja",
            SetDescription = "A complete stealth and agility enhancement set",
            ItemIDs = new List<int> { 1003, 1005, 1006, 1007 }, // Custom item IDs
            SetBonuses = new List<SerializableModifierData>
            {
                new SerializableModifierData
                {
                    m_Type = ModifierType.SpeedMultiplier,
                    m_Amount = 0.5f, // 50% speed bonus
                    m_IsPercentage = true
                },
                new SerializableModifierData
                {
                    m_Type = ModifierType.StealthDuration,
                    m_Amount = 100f, // Double stealth duration
                    m_IsPercentage = true
                }
            }
        };
    }
    
    public bool CheckSetCompletion(List<int> equippedItems, ItemSet itemSet)
    {
        return itemSet.ItemIDs.All(id => equippedItems.Contains(id));
    }
    
    public void ApplySetBonuses(AgentAI agent, ItemSet itemSet)
    {
        foreach (var bonus in itemSet.SetBonuses)
        {
            var modifierData = new ModifierData5L
            {
                m_Type = bonus.m_Type,
                m_Ammount = bonus.m_Amount,
                m_AmountModifier = bonus.m_AmountModifier
            };
            
            agent.m_Modifiers.AddModifier(modifierData);
        }
        
        Manager.GetUIManager().ShowMessagePopup(
            $"{agent.GetName()} equipped {itemSet.SetName} set!", 3);
    }
}
```

## Item Browser Integration

### Custom Item Browser
Extend the existing modding tools with custom item support:

```csharp
public class CustomItemBrowser
{
    private List<SerializableItemData> allItems;
    private List<SerializableItemData> filteredItems;
    
    public void LoadItems()
    {
        allItems = new List<SerializableItemData>();
        
        // Load custom items
        var itemManager = new ItemDataManager();
        allItems.AddRange(itemManager.LoadCustomItems());
        
        // Load vanilla items (converted to serializable format)
        LoadVanillaItems();
        
        filteredItems = new List<SerializableItemData>(allItems);
    }
    
    private void LoadVanillaItems()
    {
        var gameItemManager = Manager.GetItemManager();
        foreach (var gameItem in gameItemManager.GetAllItems())
        {
            var serializableItem = ConvertToSerializable(gameItem);
            allItems.Add(serializableItem);
        }
    }
    
    private SerializableItemData ConvertToSerializable(ItemManager.ItemData gameItem)
    {
        var serializable = new SerializableItemData
        {
            m_ID = gameItem.m_ID,
            m_Name = gameItem.m_Name,
            m_Description = gameItem.m_Description,
            m_Cost = gameItem.m_Cost,
            m_ItemSlotType = gameItem.m_ItemSlotType,
            m_ItemSubCategory = gameItem.m_ItemSubCategory,
            m_StackSize = gameItem.m_StackSize
        };
        
        // Convert modifiers (implementation depends on game structure)
        ConvertModifiers(gameItem, serializable);
        
        return serializable;
    }
    
    private void ConvertModifiers(ItemManager.ItemData gameItem, SerializableItemData serializable)
    {
        // This would require access to the game's modifier system
        // Implementation depends on available APIs
    }
    
    public List<SerializableItemData> FilterBySlotType(ItemSlotTypes slotType)
    {
        return allItems.Where(item => item.m_ItemSlotType == slotType).ToList();
    }
    
    public List<SerializableItemData> SearchByName(string searchTerm)
    {
        return allItems.Where(item => 
            item.m_Name.ToLower().Contains(searchTerm.ToLower())).ToList();
    }
    
    public SerializableItemData GetItemById(int id)
    {
        return allItems.FirstOrDefault(item => item.m_ID == id);
    }
}
```

## Best Practices and Considerations

### ID Management
- Use high ID ranges (1000+) for custom items to avoid conflicts
- Maintain an ID registry to prevent duplicates
- Consider using GUID-based IDs for guaranteed uniqueness

### Balance Considerations
- Test items thoroughly for game balance
- Provide appropriate costs for powerful items
- Consider negative modifiers for very powerful positive effects

### Performance
- Cache item data rather than recreating it frequently
- Use efficient data structures for item lookups
- Minimize file I/O operations during gameplay

### Compatibility
- Design items to work with existing game systems
- Test with other mods for compatibility
- Provide fallback behavior for missing dependencies

### User Experience
- Provide clear item descriptions
- Use consistent naming conventions
- Include visual feedback for item effects

This comprehensive guide covers the creation, management, and integration of custom items in Satellite Reign's modding system, providing the foundation for rich equipment and progression systems.