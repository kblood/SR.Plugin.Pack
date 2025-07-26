# Data Management and Persistence

## Overview

Satellite Reign modding supports various data management techniques for persisting custom data, configurations, and game state modifications. This guide covers file I/O, XML serialization, data transfer objects (DTOs), and best practices for data persistence.

## FileManager Service

### Core FileManager Functionality

The `FileManager` class provides comprehensive file operations for mod data:

```csharp
using SRMod.Services;

public class DataPersistenceExample
{
    public void SaveAndLoadData()
    {
        // Get plugin execution path
        string pluginPath = FileManager.ExecPath;
        
        // Save simple text
        FileManager.SaveText("Hello World!", "greeting.txt");
        
        // Save complex data as XML
        var customData = new CustomDataStructure { Name = "Test", Value = 42 };
        FileManager.SaveAsXML(customData, "data.xml");
        
        // Load data (requires implementation of LoadXML method)
        // var loadedData = FileManager.LoadXML<CustomDataStructure>("data.xml");
    }
}
```

### Path Management

```csharp
public class PathManager
{
    public void DemonstratePathHandling()
    {
        // FileManager automatically handles path construction
        string execPath = FileManager.ExecPath; // Plugin directory
        
        // Relative path automatically converted to absolute
        string configFile = "config.xml";
        string fullPath = FileManager.FilePathCheck(configFile);
        // Result: [PluginPath]\config.xml
        
        // Absolute paths left unchanged
        string absolutePath = @"C:\temp\data.xml";
        string checkedPath = FileManager.FilePathCheck(absolutePath);
        // Result: C:\temp\data.xml (unchanged)
    }
}
```

## XML Serialization System

### Basic XML Serialization

```csharp
[System.Serializable]
public class ModConfiguration
{
    public string ModName { get; set; }
    public bool EnableAdvancedFeatures { get; set; }
    public float UpdateInterval { get; set; }
    public List<string> EnabledFeatures { get; set; }
    
    public ModConfiguration()
    {
        EnabledFeatures = new List<string>();
        UpdateInterval = 1.0f;
        EnableAdvancedFeatures = true;
    }
}

public class ConfigurationManager
{
    private const string CONFIG_FILE = "mod_config.xml";
    
    public void SaveConfiguration(ModConfiguration config)
    {
        try
        {
            FileManager.SaveAsXML(config, CONFIG_FILE);
            Debug.Log("Configuration saved successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save configuration: {e.Message}");
        }
    }
    
    public ModConfiguration LoadConfiguration()
    {
        try
        {
            // Note: Generic LoadXML method would need implementation
            // This is conceptual - actual implementation may vary
            return LoadXMLData<ModConfiguration>(CONFIG_FILE);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load configuration: {e.Message}");
            return CreateDefaultConfiguration();
        }
    }
    
    private ModConfiguration CreateDefaultConfiguration()
    {
        return new ModConfiguration
        {
            ModName = "Default Mod",
            EnableAdvancedFeatures = true,
            UpdateInterval = 1.0f,
            EnabledFeatures = new List<string> { "HealthBoost", "SpeedEnhance" }
        };
    }
    
    private T LoadXMLData<T>(string fileName) where T : new()
    {
        // Implementation would use System.Xml.Serialization.XmlSerializer
        // Similar to FileManager.SaveAsXML but for loading
        string filePath = FileManager.FilePathCheck(fileName);
        
        if (!System.IO.File.Exists(filePath))
            return new T();
            
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        using (var reader = new System.IO.StreamReader(filePath))
        {
            return (T)serializer.Deserialize(reader);
        }
    }
}
```

### Advanced XML Serialization with Collections

```csharp
[System.Serializable]
public class CustomItem
{
    public int ID { get; set; }
    public string Name { get; set; }
    public float Value { get; set; }
    public List<string> Properties { get; set; }
    
    public CustomItem()
    {
        Properties = new List<string>();
    }
}

[System.Serializable]
public class ItemDatabase
{
    public List<CustomItem> Items { get; set; }
    public DateTime LastModified { get; set; }
    public int Version { get; set; }
    
    public ItemDatabase()
    {
        Items = new List<CustomItem>();
        LastModified = DateTime.Now;
        Version = 1;
    }
}

public class ItemDatabaseManager
{
    private const string DATABASE_FILE = "item_database.xml";
    private ItemDatabase database;
    
    public void InitializeDatabase()
    {
        database = LoadDatabase();
        if (database.Items.Count == 0)
        {
            CreateSampleData();
            SaveDatabase();
        }
    }
    
    private void CreateSampleData()
    {
        database.Items.AddRange(new[]
        {
            new CustomItem
            {
                ID = 1,
                Name = "Super Health Pack",
                Value = 100f,
                Properties = new List<string> { "Consumable", "Rare", "Healing" }
            },
            new CustomItem
            {
                ID = 2,
                Name = "Energy Booster",
                Value = 50f,
                Properties = new List<string> { "Consumable", "Common", "Energy" }
            }
        });
    }
    
    public void AddItem(CustomItem item)
    {
        item.ID = GetNextItemID();
        database.Items.Add(item);
        database.LastModified = DateTime.Now;
        SaveDatabase();
    }
    
    public CustomItem GetItem(int id)
    {
        return database.Items.FirstOrDefault(item => item.ID == id);
    }
    
    public List<CustomItem> GetItemsByProperty(string property)
    {
        return database.Items
            .Where(item => item.Properties.Contains(property))
            .ToList();
    }
    
    private int GetNextItemID()
    {
        return database.Items.Count > 0 ? database.Items.Max(i => i.ID) + 1 : 1;
    }
    
    private ItemDatabase LoadDatabase()
    {
        try
        {
            string filePath = FileManager.FilePathCheck(DATABASE_FILE);
            if (System.IO.File.Exists(filePath))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ItemDatabase));
                using (var reader = new System.IO.StreamReader(filePath))
                {
                    return (ItemDatabase)serializer.Deserialize(reader);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load item database: {e.Message}");
        }
        
        return new ItemDatabase();
    }
    
    private void SaveDatabase()
    {
        try
        {
            FileManager.SaveAsXML(database, DATABASE_FILE);
            Debug.Log($"Item database saved with {database.Items.Count} items");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save item database: {e.Message}");
        }
    }
}
```

## Translation and Localization Data

### Translation Management System

```csharp
[System.Serializable]
public class TranslationElementDTO
{
    public string Key { get; set; }
    public LocElement Element { get; set; }
    
    // Multiple language support
    public Dictionary<string, string> Translations { get; set; }
    
    public TranslationElementDTO()
    {
        Translations = new Dictionary<string, string>();
    }
}

[System.Serializable]
public class TranslationsDTO
{
    public List<TranslationElementDTO> Translations { get; set; }
    public string Language { get; set; }
    public int Version { get; set; }
    
    public TranslationsDTO()
    {
        Translations = new List<TranslationElementDTO>();
        Language = "English";
        Version = 1;
    }
}

public class TranslationManager
{
    private const string TRANSLATIONS_FILE = "translations.xml";
    private TranslationsDTO translations;
    
    public void LoadTranslations()
    {
        translations = FileManager.LoadTranslationsXML(TRANSLATIONS_FILE) != null
            ? new TranslationsDTO { Translations = FileManager.LoadTranslationsXML(TRANSLATIONS_FILE) }
            : CreateDefaultTranslations();
    }
    
    public void SaveTranslations()
    {
        FileManager.SaveAsXML(translations.Translations, TRANSLATIONS_FILE);
    }
    
    public string GetTranslation(string key, string defaultValue = "")
    {
        var translation = translations.Translations
            .FirstOrDefault(t => t.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            
        return translation?.Element?.English ?? defaultValue;
    }
    
    public void AddTranslation(string key, string english, string description = "")
    {
        var existing = translations.Translations
            .FirstOrDefault(t => t.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            
        if (existing != null)
        {
            existing.Element.English = english;
        }
        else
        {
            translations.Translations.Add(new TranslationElementDTO
            {
                Key = key,
                Element = new LocElement
                {
                    English = english,
                    Description = description
                }
            });
        }
        
        SaveTranslations();
    }
    
    private TranslationsDTO CreateDefaultTranslations()
    {
        var defaultTranslations = new TranslationsDTO();
        
        defaultTranslations.Translations.AddRange(new[]
        {
            new TranslationElementDTO
            {
                Key = "MOD_WELCOME",
                Element = new LocElement { English = "Welcome to the custom mod!" }
            },
            new TranslationElementDTO
            {
                Key = "MOD_HEALTH_RESTORED",
                Element = new LocElement { English = "Health has been restored to all agents" }
            },
            new TranslationElementDTO
            {
                Key = "MOD_WEAPONS_ENHANCED",
                Element = new LocElement { English = "All weapons have been enhanced" }
            }
        });
        
        return defaultTranslations;
    }
}
```

## Texture and Image Management

### Texture Persistence

```csharp
public class TextureManager
{
    public void SaveAndLoadTextures()
    {
        // Save texture to file
        Texture2D customTexture = CreateCustomTexture();
        string savedPath = FileManager.SaveTextureToFile(customTexture);
        Debug.Log($"Texture saved to: {savedPath}");
        
        // Load texture from file
        Texture2D loadedTexture = FileManager.LoadTextureFromFile("custom_texture");
        if (loadedTexture != null)
        {
            Debug.Log($"Loaded texture: {loadedTexture.name} ({loadedTexture.width}x{loadedTexture.height})");
        }
    }
    
    private Texture2D CreateCustomTexture()
    {
        // Create a simple 64x64 texture
        Texture2D texture = new Texture2D(64, 64);
        
        // Fill with a pattern
        Color[] colors = new Color[64 * 64];
        for (int y = 0; y < 64; y++)
        {
            for (int x = 0; x < 64; x++)
            {
                float r = (float)x / 64f;
                float g = (float)y / 64f;
                float b = 0.5f;
                colors[y * 64 + x] = new Color(r, g, b, 1f);
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        texture.name = "custom_generated_texture";
        
        return texture;
    }
    
    public void ManageTextureCache()
    {
        // Create texture cache directory
        string iconPath = FileManager.FilePathCheck("icons");
        if (!System.IO.Directory.Exists(iconPath))
        {
            System.IO.Directory.CreateDirectory(iconPath);
        }
        
        // Cache commonly used textures
        CacheTexture("health_icon", CreateHealthIconTexture());
        CacheTexture("energy_icon", CreateEnergyIconTexture());
        CacheTexture("weapon_icon", CreateWeaponIconTexture());
    }
    
    private void CacheTexture(string name, Texture2D texture)
    {
        texture.name = name;
        FileManager.SaveTextureToFile(texture);
    }
    
    private Texture2D CreateHealthIconTexture()
    {
        // Create a red cross texture for health
        Texture2D texture = new Texture2D(32, 32);
        Color[] colors = new Color[32 * 32];
        
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                // Create a red cross pattern
                bool isVerticalBar = x >= 12 && x <= 19;
                bool isHorizontalBar = y >= 12 && y <= 19;
                
                if (isVerticalBar || isHorizontalBar)
                {
                    colors[y * 32 + x] = Color.red;
                }
                else
                {
                    colors[y * 32 + x] = Color.clear;
                }
            }
        }
        
        texture.SetPixels(colors);
        texture.Apply();
        return texture;
    }
    
    private Texture2D CreateEnergyIconTexture()
    {
        // Create a blue lightning bolt texture
        Texture2D texture = new Texture2D(32, 32);
        // Implementation would create lightning bolt pattern
        return texture;
    }
    
    private Texture2D CreateWeaponIconTexture()
    {
        // Create a weapon-like texture
        Texture2D texture = new Texture2D(32, 32);
        // Implementation would create weapon pattern
        return texture;
    }
}
```

## Binary Data Management

### Binary File Operations

```csharp
public class BinaryDataManager
{
    public void SaveBinaryData(byte[] data, string fileName)
    {
        try
        {
            bool success = FileManager.SaveData(data, fileName);
            if (success)
            {
                Debug.Log($"Binary data saved to {fileName}");
            }
            else
            {
                Debug.LogError($"Failed to save binary data to {fileName}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Exception saving binary data: {e.Message}");
        }
    }
    
    public byte[] LoadBinaryData(string fileName)
    {
        try
        {
            if (System.IO.File.Exists(fileName))
            {
                return System.IO.File.ReadAllBytes(fileName);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load binary data: {e.Message}");
        }
        
        return new byte[0];
    }
    
    public void SaveCompressedData<T>(T data, string fileName) where T : class
    {
        try
        {
            // Serialize to JSON or XML first
            string jsonData = JsonUtility.ToJson(data);
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonData);
            
            // Compress the data (conceptual - would need compression library)
            byte[] compressedData = CompressData(jsonBytes);
            
            SaveBinaryData(compressedData, fileName);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save compressed data: {e.Message}");
        }
    }
    
    private byte[] CompressData(byte[] data)
    {
        // Placeholder for compression implementation
        // Would use System.IO.Compression or similar
        return data;
    }
}
```

## Data Validation and Integrity

### Data Validation System

```csharp
public class DataValidator
{
    public static bool ValidateModConfiguration(ModConfiguration config)
    {
        if (config == null)
        {
            Debug.LogError("Configuration is null");
            return false;
        }
        
        if (string.IsNullOrEmpty(config.ModName))
        {
            Debug.LogError("Mod name cannot be empty");
            return false;
        }
        
        if (config.UpdateInterval <= 0)
        {
            Debug.LogError("Update interval must be positive");
            return false;
        }
        
        if (config.EnabledFeatures == null)
        {
            Debug.LogWarning("Enabled features list is null, initializing empty list");
            config.EnabledFeatures = new List<string>();
        }
        
        return true;
    }
    
    public static bool ValidateItemData(SerializableItemData item)
    {
        if (item == null) return false;
        
        var validationResults = new List<string>();
        
        if (item.m_ID <= 0)
            validationResults.Add("ID must be positive");
            
        if (string.IsNullOrEmpty(item.m_Name))
            validationResults.Add("Name cannot be empty");
            
        if (item.m_Cost < 0)
            validationResults.Add("Cost cannot be negative");
            
        if (item.m_StackSize <= 0)
            validationResults.Add("Stack size must be positive");
        
        if (validationResults.Count > 0)
        {
            Debug.LogError($"Item validation failed: {string.Join(", ", validationResults)}");
            return false;
        }
        
        return true;
    }
    
    public static void ValidateAndRepairDatabase(ItemDatabase database)
    {
        if (database == null)
        {
            Debug.LogError("Database is null, cannot validate");
            return;
        }
        
        // Remove invalid items
        var validItems = database.Items.Where(ValidateItemData).ToList();
        
        if (validItems.Count != database.Items.Count)
        {
            Debug.LogWarning($"Removed {database.Items.Count - validItems.Count} invalid items");
            database.Items = validItems;
        }
        
        // Check for duplicate IDs
        var duplicateIds = database.Items
            .GroupBy(i => i.ID)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);
            
        foreach (var duplicateId in duplicateIds)
        {
            Debug.LogWarning($"Found duplicate ID: {duplicateId}");
            RepairDuplicateIds(database.Items, duplicateId);
        }
        
        // Update version and timestamp
        database.Version++;
        database.LastModified = DateTime.Now;
    }
    
    private static void RepairDuplicateIds(List<CustomItem> items, int duplicateId)
    {
        var duplicates = items.Where(i => i.ID == duplicateId).Skip(1).ToList();
        int nextId = items.Max(i => i.ID) + 1;
        
        foreach (var duplicate in duplicates)
        {
            duplicate.ID = nextId++;
            Debug.Log($"Reassigned duplicate item ID to: {duplicate.ID}");
        }
    }
}
```

## Performance Optimization

### Efficient Data Loading

```csharp
public class PerformantDataManager
{
    private Dictionary<string, object> dataCache = new Dictionary<string, object>();
    private Dictionary<string, DateTime> cacheTimestamps = new Dictionary<string, DateTime>();
    private const int CACHE_EXPIRY_MINUTES = 30;
    
    public T GetCachedData<T>(string key, Func<T> loadFunction) where T : class
    {
        // Check if data is cached and not expired
        if (dataCache.ContainsKey(key) && 
            cacheTimestamps.ContainsKey(key) &&
            DateTime.Now - cacheTimestamps[key] < TimeSpan.FromMinutes(CACHE_EXPIRY_MINUTES))
        {
            return dataCache[key] as T;
        }
        
        // Load data and cache it
        T data = loadFunction();
        if (data != null)
        {
            dataCache[key] = data;
            cacheTimestamps[key] = DateTime.Now;
        }
        
        return data;
    }
    
    public void PreloadCriticalData()
    {
        // Preload frequently accessed data
        GetCachedData("mod_config", () => LoadConfiguration());
        GetCachedData("translations", () => LoadTranslations());
        GetCachedData("item_database", () => LoadItemDatabase());
    }
    
    public void ClearExpiredCache()
    {
        var expiredKeys = cacheTimestamps
            .Where(kvp => DateTime.Now - kvp.Value > TimeSpan.FromMinutes(CACHE_EXPIRY_MINUTES))
            .Select(kvp => kvp.Key)
            .ToList();
            
        foreach (var key in expiredKeys)
        {
            dataCache.Remove(key);
            cacheTimestamps.Remove(key);
        }
        
        if (expiredKeys.Count > 0)
        {
            Debug.Log($"Cleared {expiredKeys.Count} expired cache entries");
        }
    }
    
    private ModConfiguration LoadConfiguration()
    {
        // Implementation for loading configuration
        return new ModConfiguration();
    }
    
    private TranslationsDTO LoadTranslations()
    {
        // Implementation for loading translations
        return new TranslationsDTO();
    }
    
    private ItemDatabase LoadItemDatabase()
    {
        // Implementation for loading item database
        return new ItemDatabase();
    }
}
```

## Best Practices Summary

### Data Management Guidelines

1. **Always validate data** before processing or saving
2. **Use caching** for frequently accessed data
3. **Handle exceptions gracefully** with fallback mechanisms
4. **Version your data structures** for future compatibility
5. **Use compression** for large data sets
6. **Implement data repair** mechanisms for corrupted files
7. **Cache file paths** to avoid repeated path construction
8. **Use asynchronous operations** for large file operations when possible

### Example Implementation

```csharp
public class ComprehensiveDataManager : ISrPlugin
{
    private ConfigurationManager configManager;
    private ItemDatabaseManager itemManager;
    private TranslationManager translationManager;
    private PerformantDataManager cacheManager;
    
    public void Initialize()
    {
        configManager = new ConfigurationManager();
        itemManager = new ItemDatabaseManager();
        translationManager = new TranslationManager();
        cacheManager = new PerformantDataManager();
        
        InitializeAllData();
    }
    
    private void InitializeAllData()
    {
        try
        {
            // Load all data with error handling
            var config = configManager.LoadConfiguration();
            if (!DataValidator.ValidateModConfiguration(config))
            {
                config = new ModConfiguration(); // Use defaults
            }
            
            itemManager.InitializeDatabase();
            translationManager.LoadTranslations();
            cacheManager.PreloadCriticalData();
            
            Debug.Log("Data management system initialized successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to initialize data management: {e.Message}");
        }
    }
    
    public void Update()
    {
        // Periodic cache cleanup
        if (Time.time % 300f < 0.1f) // Every 5 minutes
        {
            cacheManager.ClearExpiredCache();
        }
    }
    
    public string GetName() => "Comprehensive Data Manager";
}
```

This comprehensive data management system provides robust, performant, and reliable data persistence for Satellite Reign mods.