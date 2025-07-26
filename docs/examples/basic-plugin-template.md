# Basic Plugin Template

This template provides a starting point for creating Satellite Reign mods with proper structure, error handling, and best practices.

## Template Code

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Template for Satellite Reign mods with proper structure and error handling
/// Replace "TemplatePlugin" with your mod's name
/// </summary>
public class TemplatePlugin : ISrPlugin
{
    #region Configuration
    private const string MOD_NAME = "Template Plugin";
    private const string MOD_VERSION = "1.0.0";
    private const float UPDATE_INTERVAL = 1.0f; // Update every second
    #endregion
    
    #region Private Fields
    private bool initialized = false;
    private float nextUpdate = 0f;
    private Dictionary<string, object> settings;
    #endregion
    
    #region ISrPlugin Implementation
    public void Initialize()
    {
        try
        {
            Debug.Log($"{MOD_NAME} v{MOD_VERSION}: Starting initialization...");
            
            // Initialize settings
            InitializeSettings();
            
            // Setup event handlers if needed
            SetupEventHandlers();
            
            // Initialize components
            InitializeComponents();
            
            initialized = true;
            Debug.Log($"{MOD_NAME}: Initialization complete!");
            
            // Show welcome message
            ShowMessage($"{MOD_NAME} v{MOD_VERSION} loaded successfully!", 3);
        }
        catch (Exception e)
        {
            Debug.LogError($"{MOD_NAME}: Initialization failed - {e.Message}");
            initialized = false;
        }
    }
    
    public void Update()
    {
        if (!initialized || !IsGameReady())
            return;
            
        try
        {
            // Handle input every frame
            HandleInput();
            
            // Periodic updates
            if (Time.time >= nextUpdate)
            {
                PeriodicUpdate();
                nextUpdate = Time.time + UPDATE_INTERVAL;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{MOD_NAME} Update Error: {e.Message}");
        }
    }
    
    public string GetName()
    {
        return $"{MOD_NAME} v{MOD_VERSION}";
    }
    #endregion
    
    #region Initialization Methods
    private void InitializeSettings()
    {
        settings = new Dictionary<string, object>
        {
            ["EnableFeature1"] = true,
            ["EnableFeature2"] = false,
            ["UpdateInterval"] = UPDATE_INTERVAL,
            ["DebugMode"] = false
        };
        
        // Load settings from file if available
        LoadSettings();
    }
    
    private void SetupEventHandlers()
    {
        // Subscribe to events here if needed
        // Example: SomeEvent += HandleSomeEvent;
    }
    
    private void InitializeComponents()
    {
        // Initialize any required components
        // Example: Setup UI elements, data structures, etc.
    }
    #endregion
    
    #region Update Methods
    private void HandleInput()
    {
        // Handle key presses and input
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ShowModMenu();
        }
        
        if (Input.GetKeyDown(KeyCode.F2))
        {
            ToggleFeature("EnableFeature1");
        }
        
        // Add more input handling as needed
    }
    
    private void PeriodicUpdate()
    {
        // Perform periodic operations here
        
        if (GetSetting<bool>("DebugMode"))
        {
            Debug.Log($"{MOD_NAME}: Periodic update at {Time.time}");
        }
        
        // Example: Update agent states, check conditions, etc.
        UpdateAgentStates();
    }
    #endregion
    
    #region Core Functionality
    private void UpdateAgentStates()
    {
        try
        {
            foreach (var agent in AgentAI.GetAgents())
            {
                if (agent != null && agent.IsSelected())
                {
                    // Perform operations on selected agents
                    ProcessAgent(agent);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{MOD_NAME}: Error updating agent states - {e.Message}");
        }
    }
    
    private void ProcessAgent(AgentAI agent)
    {
        // Example agent processing
        if (GetSetting<bool>("EnableFeature1"))
        {
            // Do something with the agent
            // Example: Heal agent, modify stats, etc.
        }
    }
    
    private void ShowModMenu()
    {
        try
        {
            ShowMessage("Mod menu would appear here", 3);
            // Implement your mod's menu system here
        }
        catch (Exception e)
        {
            Debug.LogError($"{MOD_NAME}: Error showing mod menu - {e.Message}");
        }
    }
    
    private void ToggleFeature(string featureName)
    {
        try
        {
            if (settings.ContainsKey(featureName))
            {
                bool currentValue = GetSetting<bool>(featureName);
                settings[featureName] = !currentValue;
                
                string status = !currentValue ? "enabled" : "disabled";
                ShowMessage($"{featureName} {status}", 2);
                
                // Save settings after change
                SaveSettings();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{MOD_NAME}: Error toggling feature {featureName} - {e.Message}");
        }
    }
    #endregion
    
    #region Settings Management
    private T GetSetting<T>(string key)
    {
        if (settings.ContainsKey(key))
        {
            try
            {
                return (T)settings[key];
            }
            catch (Exception e)
            {
                Debug.LogError($"{MOD_NAME}: Error getting setting {key} - {e.Message}");
            }
        }
        
        return default(T);
    }
    
    private void SetSetting<T>(string key, T value)
    {
        settings[key] = value;
    }
    
    private void LoadSettings()
    {
        try
        {
            // Implement settings loading from file
            // This is a placeholder - implement based on your needs
            Debug.Log($"{MOD_NAME}: Settings loaded (placeholder)");
        }
        catch (Exception e)
        {
            Debug.LogError($"{MOD_NAME}: Error loading settings - {e.Message}");
        }
    }
    
    private void SaveSettings()
    {
        try
        {
            // Implement settings saving to file
            // This is a placeholder - implement based on your needs
            Debug.Log($"{MOD_NAME}: Settings saved (placeholder)");
        }
        catch (Exception e)
        {
            Debug.LogError($"{MOD_NAME}: Error saving settings - {e.Message}");
        }
    }
    #endregion
    
    #region Utility Methods
    private bool IsGameReady()
    {
        return Manager.Get() != null && 
               Manager.Get().GameInProgress && 
               !Manager.Get().IsLoading();
    }
    
    private void ShowMessage(string message, int duration = 3)
    {
        try
        {
            var uiManager = Manager.GetUIManager();
            if (uiManager != null)
            {
                uiManager.ShowMessagePopup(message, duration);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{MOD_NAME}: Error showing message - {e.Message}");
        }
    }
    
    private void ShowWarning(string message, int duration = 3)
    {
        try
        {
            var uiManager = Manager.GetUIManager();
            if (uiManager != null)
            {
                uiManager.ShowWarningPopup(message, duration);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"{MOD_NAME}: Error showing warning - {e.Message}");
        }
    }
    #endregion
    
    #region Cleanup (if needed)
    ~TemplatePlugin()
    {
        // Cleanup code if needed
        // Note: Destructors in Unity can be problematic, use with caution
    }
    #endregion
}
```

## Usage Instructions

1. **Copy the template** and rename `TemplatePlugin` to your mod's name
2. **Update constants** like `MOD_NAME` and `MOD_VERSION`
3. **Implement your features** in the designated sections
4. **Add your input handling** in the `HandleInput()` method
5. **Add periodic operations** in the `PeriodicUpdate()` method
6. **Implement settings persistence** in `LoadSettings()` and `SaveSettings()`

## Key Features

- **Error handling** throughout all methods
- **Settings management** system
- **Periodic update** pattern with configurable interval
- **Input handling** structure
- **Safe game state checking**
- **Logging and debugging** support
- **Message display utilities**

## Customization Points

### Adding New Features

```csharp
private void YourCustomFeature()
{
    try
    {
        // Your feature implementation
        foreach (var agent in AgentAI.GetAgents())
        {
            if (agent.IsSelected())
            {
                // Do something with selected agents
            }
        }
        
        ShowMessage("Custom feature executed!", 3);
    }
    catch (Exception e)
    {
        Debug.LogError($"{MOD_NAME}: Custom feature error - {e.Message}");
    }
}
```

### Adding New Settings

```csharp
private void InitializeSettings()
{
    settings = new Dictionary<string, object>
    {
        // Existing settings...
        ["YourNewSetting"] = "DefaultValue",
        ["NumericSetting"] = 42,
        ["BooleanSetting"] = true
    };
}
```

### Adding New Input Commands

```csharp
private void HandleInput()
{
    // Existing input handling...
    
    if (Input.GetKeyDown(KeyCode.F3))
    {
        YourCustomFeature();
    }
    
    if (Input.GetKeyDown(KeyCode.F4) && Input.GetKey(KeyCode.LeftControl))
    {
        // Ctrl+F4 combination
        AnotherCustomFeature();
    }
}
```

This template provides a solid foundation for creating robust, maintainable Satellite Reign mods with proper error handling and extensibility.