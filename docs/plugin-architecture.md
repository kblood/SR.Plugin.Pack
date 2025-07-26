# Plugin Architecture

## Overview

Satellite Reign's modding system is built around the `ISrPlugin` interface, which provides a standardized way to integrate custom functionality into the game. This document covers the core architecture, lifecycle management, and advanced plugin patterns.

## ISrPlugin Interface

All mods must implement the `ISrPlugin` interface, which defines three required methods:

```csharp
public interface ISrPlugin
{
    void Initialize();
    void Update();
    string GetName();
}
```

### Method Details

#### Initialize()
Called once when the plugin is first loaded by the game.
- Use for one-time setup operations
- Register event handlers
- Initialize data structures
- Set up UI elements

```csharp
public void Initialize()
{
    Debug.Log("Initializing MyMod");
    
    // Initialize data
    myDataList = new List<CustomData>();
    
    // Set up timers
    nextUpdateTime = Time.time + updateInterval;
    
    // Register with game systems
    SetupEventHandlers();
}
```

#### Update()
Called every frame while the game is running.
- Check for input
- Update mod state
- Perform periodic operations
- **Important**: Always check `Manager.Get().GameInProgress` before game operations

```csharp
public void Update()
{
    // Only run during active gameplay
    if (!Manager.Get().GameInProgress)
        return;
        
    // Handle input
    HandleInput();
    
    // Periodic updates
    if (Time.time > nextUpdateTime)
    {
        PerformPeriodicUpdate();
        nextUpdateTime = Time.time + updateInterval;
    }
}
```

#### GetName()
Returns a string identifier for your mod.
- Used by the mod system for identification
- Should be unique and descriptive
- Keep it concise

```csharp
public string GetName()
{
    return "Advanced Weapon Mod";
}
```

## Plugin Lifecycle

### 1. Loading Phase
- Game discovers DLL files in the `Mods` folder
- Creates instances of classes implementing `ISrPlugin`
- Calls `Initialize()` on each plugin

### 2. Runtime Phase
- `Update()` called every frame during gameplay
- Plugins can interact with game systems
- Event handling and user input processing

### 3. Cleanup
- No explicit cleanup method in `ISrPlugin`
- Use static destructors or Unity's `OnDestroy` patterns if needed

## Advanced Plugin Patterns

### Singleton Pattern
For mods that need global access:

```csharp
public class MyMod : ISrPlugin
{
    private static MyMod instance;
    
    public static MyMod Instance => instance;
    
    public void Initialize()
    {
        instance = this;
        // Additional initialization
    }
    
    // Access from other classes:
    // MyMod.Instance.DoSomething();
}
```

### Event-Driven Architecture
Use events for loose coupling between components:

```csharp
public class MyMod : ISrPlugin
{
    public static event Action<AgentAI> OnAgentModified;
    public static event Action<string> OnStatusChanged;
    
    private void TriggerAgentModification(AgentAI agent)
    {
        OnAgentModified?.Invoke(agent);
    }
    
    public void Initialize()
    {
        // Subscribe to events from other systems
        SomeOtherSystem.OnSomethingHappened += HandleSystemEvent;
    }
}
```

### Service Locator Pattern
For accessing shared utilities:

```csharp
public static class ModServices
{
    private static Dictionary<Type, object> services = new Dictionary<Type, object>();
    
    public static void Register<T>(T service)
    {
        services[typeof(T)] = service;
    }
    
    public static T Get<T>()
    {
        return (T)services[typeof(T)];
    }
}

// In your plugin Initialize():
ModServices.Register<IDataManager>(new MyDataManager());

// Later, in any part of your mod:
var dataManager = ModServices.Get<IDataManager>();
```

## Performance Considerations

### Update() Optimization
The `Update()` method is called every frame, so optimize accordingly:

```csharp
private float nextCheck = 0f;
private const float checkInterval = 0.1f; // Check every 100ms instead of every frame

public void Update()
{
    if (!Manager.Get().GameInProgress)
        return;
        
    // Only perform expensive operations periodically
    if (Time.time > nextCheck)
    {
        ExpensiveOperation();
        nextCheck = Time.time + checkInterval;
    }
    
    // Handle input every frame (lightweight)
    HandleCriticalInput();
}
```

### Memory Management
Be mindful of memory allocations in Update():

```csharp
// Bad - creates garbage every frame
public void Update()
{
    var agents = new List<AgentAI>();
    foreach (var agent in AgentAI.GetAgents())
    {
        agents.Add(agent);
    }
}

// Good - reuse collections
private List<AgentAI> agentCache = new List<AgentAI>();

public void Update()
{
    agentCache.Clear();
    foreach (var agent in AgentAI.GetAgents())
    {
        agentCache.Add(agent);
    }
}
```

## Error Handling

### Graceful Degradation
Always handle potential errors gracefully:

```csharp
public void Initialize()
{
    try
    {
        // Risky initialization
        InitializeAdvancedFeatures();
    }
    catch (Exception e)
    {
        Debug.LogError($"MyMod: Advanced features failed to initialize: {e.Message}");
        // Fall back to basic functionality
        InitializeBasicFeatures();
    }
}
```

### Safe Update Pattern
Protect the Update loop from exceptions:

```csharp
public void Update()
{
    try
    {
        if (!Manager.Get().GameInProgress)
            return;
            
        UpdateInternal();
    }
    catch (Exception e)
    {
        Debug.LogError($"MyMod Update Error: {e.Message}");
        // Optionally disable problematic features
        DisableProblematicFeature();
    }
}
```

## Multi-Plugin Communication

### Static Events
Use static events for communication between plugins:

```csharp
// In Plugin A
public static class ModEvents
{
    public static event Action<string> OnGlobalMessage;
    
    public static void SendMessage(string message)
    {
        OnGlobalMessage?.Invoke(message);
    }
}

// In Plugin B
public void Initialize()
{
    ModEvents.OnGlobalMessage += HandleGlobalMessage;
}

private void HandleGlobalMessage(string message)
{
    Debug.Log($"Received global message: {message}");
}
```

### Shared Data Structures
Use static classes for shared data:

```csharp
public static class SharedModData
{
    public static Dictionary<string, object> GlobalSettings { get; } 
        = new Dictionary<string, object>();
    
    public static List<CustomItem> SharedItems { get; }
        = new List<CustomItem>();
}
```

## Configuration and Settings

### Simple Configuration
Use a configuration class for mod settings:

```csharp
public class ModConfig
{
    public bool EnableAdvancedFeatures { get; set; } = true;
    public float UpdateInterval { get; set; } = 1.0f;
    public KeyCode ActivationKey { get; set; } = KeyCode.F5;
}

public class MyMod : ISrPlugin
{
    private ModConfig config = new ModConfig();
    
    public void Initialize()
    {
        LoadConfiguration();
    }
    
    private void LoadConfiguration()
    {
        // Load from file or use defaults
        // Implementation depends on your persistence method
    }
}
```

## Best Practices

### 1. Null Checking
Always check for null references:

```csharp
public void DoSomethingWithAgent()
{
    var agent = AgentAI.FirstSelectedAgentAi();
    if (agent != null)
    {
        agent.DoSomething();
    }
}
```

### 2. Game State Validation
Verify game state before operations:

```csharp
public void ModifyAgent()
{
    if (!Manager.Get().GameInProgress)
        return;
        
    if (Manager.Get().IsLoading())
        return;
        
    // Safe to proceed with modifications
}
```

### 3. Resource Cleanup
Clean up resources when appropriate:

```csharp
public class MyMod : ISrPlugin
{
    private List<GameObject> createdObjects = new List<GameObject>();
    
    ~MyMod()
    {
        // Clean up created objects
        foreach (var obj in createdObjects)
        {
            if (obj != null)
                UnityEngine.Object.Destroy(obj);
        }
    }
}
```

## Example: Complete Plugin Structure

Here's a template for a well-structured plugin:

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedMod : ISrPlugin
{
    #region Fields
    private bool initialized = false;
    private float nextUpdate = 0f;
    private const float updateInterval = 0.5f;
    private ModConfig config = new ModConfig();
    #endregion
    
    #region ISrPlugin Implementation
    public void Initialize()
    {
        try
        {
            Debug.Log("AdvancedMod: Initializing...");
            
            LoadConfiguration();
            SetupEventHandlers();
            InitializeComponents();
            
            initialized = true;
            Debug.Log("AdvancedMod: Initialization complete");
        }
        catch (Exception e)
        {
            Debug.LogError($"AdvancedMod: Initialization failed: {e.Message}");
        }
    }
    
    public void Update()
    {
        if (!initialized || !Manager.Get().GameInProgress)
            return;
            
        try
        {
            HandleInput();
            
            if (Time.time > nextUpdate)
            {
                PeriodicUpdate();
                nextUpdate = Time.time + updateInterval;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"AdvancedMod: Update error: {e.Message}");
        }
    }
    
    public string GetName()
    {
        return "Advanced Mod v1.0";
    }
    #endregion
    
    #region Private Methods
    private void LoadConfiguration()
    {
        // Configuration loading logic
    }
    
    private void SetupEventHandlers()
    {
        // Event handler setup
    }
    
    private void InitializeComponents()
    {
        // Component initialization
    }
    
    private void HandleInput()
    {
        // Input handling logic
    }
    
    private void PeriodicUpdate()
    {
        // Periodic update logic
    }
    #endregion
}
```

This architecture provides a robust foundation for building complex Satellite Reign mods while maintaining good performance and error handling practices.