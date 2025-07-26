# Advanced Modding Techniques

## Overview

This guide covers advanced techniques for Satellite Reign modding, including complex system interactions, performance optimization, multi-plugin communication, and sophisticated modding patterns.

## Advanced Plugin Architecture Patterns

### Plugin Service System

Create a service-oriented architecture for complex mods:

```csharp
public interface IModService
{
    string ServiceName { get; }
    bool IsEnabled { get; }
    void Initialize();
    void Shutdown();
}

public class ServiceRegistry
{
    private static Dictionary<Type, IModService> services = new Dictionary<Type, IModService>();
    
    public static void RegisterService<T>(T service) where T : IModService
    {
        services[typeof(T)] = service;
        service.Initialize();
    }
    
    public static T GetService<T>() where T : class, IModService
    {
        return services.ContainsKey(typeof(T)) ? services[typeof(T)] as T : null;
    }
    
    public static void ShutdownAllServices()
    {
        foreach (var service in services.Values)
        {
            service.Shutdown();
        }
        services.Clear();
    }
}

// Example service implementation
public class WeaponEnhancementService : IModService
{
    public string ServiceName => "Weapon Enhancement Service";
    public bool IsEnabled { get; private set; }
    
    private Dictionary<WeaponType, WeaponEnhancement> enhancements = new Dictionary<WeaponType, WeaponEnhancement>();
    
    public void Initialize()
    {
        LoadEnhancements();
        IsEnabled = true;
        Debug.Log($"{ServiceName} initialized");
    }
    
    public void Shutdown()
    {
        SaveEnhancements();
        IsEnabled = false;
        Debug.Log($"{ServiceName} shutdown");
    }
    
    public void EnhanceWeapon(WeaponType weaponType, WeaponEnhancement enhancement)
    {
        if (!IsEnabled) return;
        
        enhancements[weaponType] = enhancement;
        ApplyEnhancement(weaponType, enhancement);
    }
    
    private void LoadEnhancements()
    {
        // Load enhancement data from file
    }
    
    private void SaveEnhancements()
    {
        // Save enhancement data to file
    }
    
    private void ApplyEnhancement(WeaponType weaponType, WeaponEnhancement enhancement)
    {
        // Apply the enhancement to the weapon
    }
}

public class WeaponEnhancement
{
    public float DamageMultiplier { get; set; } = 1.0f;
    public float RangeMultiplier { get; set; } = 1.0f;
    public float ReloadSpeedMultiplier { get; set; } = 1.0f;
    public bool UnlimitedAmmo { get; set; } = false;
}
```

### Advanced State Management

Implement sophisticated state management for complex mods:

```csharp
public enum ModState
{
    Uninitialized,
    Initializing,
    Ready,
    Active,
    Paused,
    Error,
    Shutting_Down
}

public class StateMachine<T> where T : struct, IConvertible
{
    private T currentState;
    private Dictionary<T, List<T>> validTransitions = new Dictionary<T, List<T>>();
    private Dictionary<T, Action> stateEnterActions = new Dictionary<T, Action>();
    private Dictionary<T, Action> stateExitActions = new Dictionary<T, Action>();
    
    public T CurrentState => currentState;
    public event Action<T, T> StateChanged;
    
    public StateMachine(T initialState)
    {
        currentState = initialState;
    }
    
    public void AddTransition(T from, T to)
    {
        if (!validTransitions.ContainsKey(from))
            validTransitions[from] = new List<T>();
        
        validTransitions[from].Add(to);
    }
    
    public void SetStateEnterAction(T state, Action action)
    {
        stateEnterActions[state] = action;
    }
    
    public void SetStateExitAction(T state, Action action)
    {
        stateExitActions[state] = action;
    }
    
    public bool TryTransition(T newState)
    {
        if (!CanTransition(newState))
            return false;
            
        T oldState = currentState;
        
        // Exit current state
        if (stateExitActions.ContainsKey(currentState))
            stateExitActions[currentState]?.Invoke();
        
        // Change state
        currentState = newState;
        
        // Enter new state
        if (stateEnterActions.ContainsKey(newState))
            stateEnterActions[newState]?.Invoke();
        
        StateChanged?.Invoke(oldState, newState);
        return true;
    }
    
    private bool CanTransition(T newState)
    {
        return validTransitions.ContainsKey(currentState) && 
               validTransitions[currentState].Contains(newState);
    }
}

// Usage in a plugin
public class AdvancedStatePlugin : ISrPlugin
{
    private StateMachine<ModState> stateMachine;
    
    public void Initialize()
    {
        SetupStateMachine();
        stateMachine.TryTransition(ModState.Initializing);
        
        // Initialize components
        InitializeComponents();
        
        stateMachine.TryTransition(ModState.Ready);
    }
    
    private void SetupStateMachine()
    {
        stateMachine = new StateMachine<ModState>(ModState.Uninitialized);
        
        // Define valid transitions
        stateMachine.AddTransition(ModState.Uninitialized, ModState.Initializing);
        stateMachine.AddTransition(ModState.Initializing, ModState.Ready);
        stateMachine.AddTransition(ModState.Initializing, ModState.Error);
        stateMachine.AddTransition(ModState.Ready, ModState.Active);
        stateMachine.AddTransition(ModState.Active, ModState.Paused);
        stateMachine.AddTransition(ModState.Paused, ModState.Active);
        stateMachine.AddTransition(ModState.Active, ModState.Ready);
        
        // Define state actions
        stateMachine.SetStateEnterAction(ModState.Active, OnEnterActiveState);
        stateMachine.SetStateExitAction(ModState.Active, OnExitActiveState);
        stateMachine.SetStateEnterAction(ModState.Error, OnEnterErrorState);
        
        // Subscribe to state changes
        stateMachine.StateChanged += OnStateChanged;
    }
    
    private void OnEnterActiveState()
    {
        Debug.Log("Plugin entered active state");
        Manager.GetUIManager().ShowMessagePopup("Advanced plugin activated", 2);
    }
    
    private void OnExitActiveState()
    {
        Debug.Log("Plugin exited active state");
    }
    
    private void OnEnterErrorState()
    {
        Debug.LogError("Plugin entered error state");
        Manager.GetUIManager().ShowWarningPopup("Plugin error occurred", 5);
    }
    
    private void OnStateChanged(ModState oldState, ModState newState)
    {
        Debug.Log($"State transition: {oldState} -> {newState}");
    }
    
    public void Update()
    {
        switch (stateMachine.CurrentState)
        {
            case ModState.Ready:
                // Check conditions to become active
                if (ShouldActivate())
                    stateMachine.TryTransition(ModState.Active);
                break;
                
            case ModState.Active:
                // Perform active state operations
                UpdateActiveState();
                break;
                
            case ModState.Paused:
                // Handle paused state
                break;
                
            case ModState.Error:
                // Handle error recovery
                if (CanRecover())
                    stateMachine.TryTransition(ModState.Ready);
                break;
        }
    }
    
    private bool ShouldActivate() => Manager.Get()?.GameInProgress == true;
    private bool CanRecover() => true; // Implement recovery logic
    private void UpdateActiveState() { /* Active state logic */ }
    private void InitializeComponents() { /* Initialization logic */ }
    
    public string GetName() => "Advanced State Plugin";
}
```

## Complex System Integration

### Multi-System Coordinator

Coordinate operations across multiple game systems:

```csharp
public class SystemCoordinator
{
    private List<ISystemInterface> systems = new List<ISystemInterface>();
    private Dictionary<string, object> sharedData = new Dictionary<string, object>();
    
    public void RegisterSystem(ISystemInterface system)
    {
        systems.Add(system);
        system.SetCoordinator(this);
    }
    
    public void SetSharedData(string key, object value)
    {
        sharedData[key] = value;
        NotifySystemsOfDataChange(key, value);
    }
    
    public T GetSharedData<T>(string key)
    {
        return sharedData.ContainsKey(key) ? (T)sharedData[key] : default(T);
    }
    
    public void ExecuteCoordinatedAction(string actionName, params object[] parameters)
    {
        foreach (var system in systems)
        {
            system.ExecuteAction(actionName, parameters);
        }
    }
    
    private void NotifySystemsOfDataChange(string key, object value)
    {
        foreach (var system in systems)
        {
            system.OnSharedDataChanged(key, value);
        }
    }
}

public interface ISystemInterface
{
    string SystemName { get; }
    void SetCoordinator(SystemCoordinator coordinator);
    void ExecuteAction(string actionName, params object[] parameters);
    void OnSharedDataChanged(string key, object value);
}

public class AgentManagementSystem : ISystemInterface
{
    public string SystemName => "Agent Management";
    private SystemCoordinator coordinator;
    
    public void SetCoordinator(SystemCoordinator coordinator)
    {
        this.coordinator = coordinator;
    }
    
    public void ExecuteAction(string actionName, params object[] parameters)
    {
        switch (actionName)
        {
            case "HealAllAgents":
                HealAllAgents();
                break;
            case "EnhanceSelectedAgents":
                EnhanceSelectedAgents((float)parameters[0]);
                break;
        }
    }
    
    public void OnSharedDataChanged(string key, object value)
    {
        if (key == "GlobalHealthMultiplier")
        {
            ApplyGlobalHealthMultiplier((float)value);
        }
    }
    
    private void HealAllAgents()
    {
        foreach (var agent in AgentAI.GetAgents())
        {
            agent.SetHealthValue(100);
            agent.m_Energy.AddEnergy(500);
        }
        
        coordinator.SetSharedData("LastHealTime", Time.time);
    }
    
    private void EnhanceSelectedAgents(float enhancementFactor)
    {
        foreach (var agent in AgentAI.GetAgents())
        {
            if (agent.IsSelected())
            {
                EnhanceAgent(agent, enhancementFactor);
            }
        }
    }
    
    private void EnhanceAgent(AgentAI agent, float factor)
    {
        // Apply enhancements based on factor
        var healthModifier = new ModifierData5L
        {
            m_Type = ModifierType.HealthOffset,
            m_Ammount = 50f * factor
        };
        agent.m_Modifiers.AddModifier(healthModifier);
    }
    
    private void ApplyGlobalHealthMultiplier(float multiplier)
    {
        // Apply global health changes
    }
}
```

### Event-Driven Architecture

Implement a sophisticated event system:

```csharp
public static class ModEventSystem
{
    private static Dictionary<Type, List<Delegate>> eventHandlers = new Dictionary<Type, List<Delegate>>();
    
    public static void Subscribe<T>(Action<T> handler) where T : IModEvent
    {
        Type eventType = typeof(T);
        if (!eventHandlers.ContainsKey(eventType))
            eventHandlers[eventType] = new List<Delegate>();
        
        eventHandlers[eventType].Add(handler);
    }
    
    public static void Unsubscribe<T>(Action<T> handler) where T : IModEvent
    {
        Type eventType = typeof(T);
        if (eventHandlers.ContainsKey(eventType))
        {
            eventHandlers[eventType].Remove(handler);
        }
    }
    
    public static void Publish<T>(T eventData) where T : IModEvent
    {
        Type eventType = typeof(T);
        if (eventHandlers.ContainsKey(eventType))
        {
            foreach (Action<T> handler in eventHandlers[eventType])
            {
                try
                {
                    handler?.Invoke(eventData);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Event handler error: {e.Message}");
                }
            }
        }
    }
}

public interface IModEvent
{
    DateTime Timestamp { get; }
    string Source { get; }
}

public class AgentHealthChangedEvent : IModEvent
{
    public DateTime Timestamp { get; }
    public string Source { get; }
    public AgentAI Agent { get; }
    public float OldHealth { get; }
    public float NewHealth { get; }
    
    public AgentHealthChangedEvent(AgentAI agent, float oldHealth, float newHealth, string source = "Unknown")
    {
        Timestamp = DateTime.Now;
        Source = source;
        Agent = agent;
        OldHealth = oldHealth;
        NewHealth = newHealth;
    }
}

public class WeaponEnhancedEvent : IModEvent
{
    public DateTime Timestamp { get; }
    public string Source { get; }
    public WeaponType WeaponType { get; }
    public Dictionary<string, float> Enhancements { get; }
    
    public WeaponEnhancedEvent(WeaponType weaponType, Dictionary<string, float> enhancements, string source = "Unknown")
    {
        Timestamp = DateTime.Now;
        Source = source;
        WeaponType = weaponType;
        Enhancements = new Dictionary<string, float>(enhancements);
    }
}

// Usage in plugins
public class EventDrivenPlugin : ISrPlugin
{
    public void Initialize()
    {
        // Subscribe to events
        ModEventSystem.Subscribe<AgentHealthChangedEvent>(OnAgentHealthChanged);
        ModEventSystem.Subscribe<WeaponEnhancedEvent>(OnWeaponEnhanced);
    }
    
    public void Update()
    {
        // Monitor agent health and publish events
        MonitorAgentHealth();
    }
    
    private void MonitorAgentHealth()
    {
        foreach (var agent in AgentAI.GetAgents())
        {
            float currentHealth = agent.GetHealthValue();
            float lastKnownHealth = GetLastKnownHealth(agent);
            
            if (Math.Abs(currentHealth - lastKnownHealth) > 0.1f)
            {
                ModEventSystem.Publish(new AgentHealthChangedEvent(
                    agent, lastKnownHealth, currentHealth, GetName()));
                
                UpdateLastKnownHealth(agent, currentHealth);
            }
        }
    }
    
    private void OnAgentHealthChanged(AgentHealthChangedEvent eventData)
    {
        Debug.Log($"Agent {eventData.Agent.GetName()} health changed: {eventData.OldHealth} -> {eventData.NewHealth}");
        
        // React to health changes
        if (eventData.NewHealth < 20f && eventData.OldHealth >= 20f)
        {
            // Agent is in critical condition
            HandleCriticalHealth(eventData.Agent);
        }
    }
    
    private void OnWeaponEnhanced(WeaponEnhancedEvent eventData)
    {
        Debug.Log($"Weapon {eventData.WeaponType} was enhanced by {eventData.Source}");
        
        // React to weapon enhancements
        Manager.GetUIManager().ShowMessagePopup(
            $"{eventData.WeaponType} has been enhanced!", 3);
    }
    
    private void HandleCriticalHealth(AgentAI agent)
    {
        Manager.GetUIManager().ShowWarningPopup(
            $"{agent.GetName()} is in critical condition!", 5);
    }
    
    private float GetLastKnownHealth(AgentAI agent) { /* Implementation */ return 0f; }
    private void UpdateLastKnownHealth(AgentAI agent, float health) { /* Implementation */ }
    
    public string GetName() => "Event Driven Plugin";
}
```

## Performance Optimization Techniques

### Object Pooling

Implement object pooling for frequently created/destroyed objects:

```csharp
public class ObjectPool<T> where T : class, new()
{
    private readonly Queue<T> pool = new Queue<T>();
    private readonly Func<T> createFunc;
    private readonly Action<T> resetAction;
    private readonly int maxSize;
    
    public ObjectPool(Func<T> createFunc = null, Action<T> resetAction = null, int maxSize = 100)
    {
        this.createFunc = createFunc ?? (() => new T());
        this.resetAction = resetAction;
        this.maxSize = maxSize;
    }
    
    public T Get()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        
        return createFunc();
    }
    
    public void Return(T item)
    {
        if (pool.Count < maxSize)
        {
            resetAction?.Invoke(item);
            pool.Enqueue(item);
        }
    }
    
    public void Clear()
    {
        pool.Clear();
    }
}

// Usage example
public class PerformantPlugin : ISrPlugin
{
    private ObjectPool<StringBuilder> stringBuilderPool;
    private ObjectPool<List<AgentAI>> agentListPool;
    
    public void Initialize()
    {
        stringBuilderPool = new ObjectPool<StringBuilder>(
            () => new StringBuilder(),
            sb => sb.Clear(),
            20);
            
        agentListPool = new ObjectPool<List<AgentAI>>(
            () => new List<AgentAI>(),
            list => list.Clear(),
            10);
    }
    
    public void Update()
    {
        if (Time.frameCount % 60 == 0) // Every 60 frames
        {
            GenerateReport();
        }
    }
    
    private void GenerateReport()
    {
        var sb = stringBuilderPool.Get();
        var agentList = agentListPool.Get();
        
        try
        {
            // Build report without creating garbage
            agentList.AddRange(AgentAI.GetAgents());
            
            sb.AppendLine("Agent Report:");
            foreach (var agent in agentList)
            {
                sb.AppendFormat("- {0}: Health {1:F1}\n", 
                    agent.GetName(), agent.GetHealthValue());
            }
            
            string report = sb.ToString();
            // Use the report...
        }
        finally
        {
            // Return objects to pools
            stringBuilderPool.Return(sb);
            agentListPool.Return(agentList);
        }
    }
    
    public string GetName() => "Performant Plugin";
}
```

### Cached Calculations

Cache expensive calculations to improve performance:

```csharp
public class CalculationCache<TKey, TValue>
{
    private readonly Dictionary<TKey, CacheEntry<TValue>> cache = new Dictionary<TKey, CacheEntry<TValue>>();
    private readonly float cacheTimeout;
    
    public CalculationCache(float timeoutSeconds = 5f)
    {
        cacheTimeout = timeoutSeconds;
    }
    
    public TValue GetOrCalculate(TKey key, Func<TKey, TValue> calculator)
    {
        if (cache.ContainsKey(key))
        {
            var entry = cache[key];
            if (Time.time - entry.Timestamp < cacheTimeout)
            {
                return entry.Value;
            }
        }
        
        TValue value = calculator(key);
        cache[key] = new CacheEntry<TValue>(value, Time.time);
        return value;
    }
    
    public void Clear()
    {
        cache.Clear();
    }
    
    public void ClearExpired()
    {
        var expiredKeys = cache
            .Where(kvp => Time.time - kvp.Value.Timestamp >= cacheTimeout)
            .Select(kvp => kvp.Key)
            .ToList();
            
        foreach (var key in expiredKeys)
        {
            cache.Remove(key);
        }
    }
    
    private class CacheEntry<T>
    {
        public T Value { get; }
        public float Timestamp { get; }
        
        public CacheEntry(T value, float timestamp)
        {
            Value = value;
            Timestamp = timestamp;
        }
    }
}

// Usage in a plugin
public class CachedCalculationsPlugin : ISrPlugin
{
    private CalculationCache<AgentAI, float> threatLevelCache;
    private CalculationCache<Vector3, List<AgentAI>> nearbyAgentsCache;
    
    public void Initialize()
    {
        threatLevelCache = new CalculationCache<AgentAI, float>(2f);
        nearbyAgentsCache = new CalculationCache<Vector3, List<AgentAI>>(1f);
    }
    
    public void Update()
    {
        if (Time.frameCount % 300 == 0) // Every 5 seconds at 60 FPS
        {
            threatLevelCache.ClearExpired();
            nearbyAgentsCache.ClearExpired();
        }
        
        // Use cached calculations
        foreach (var agent in AgentAI.GetAgents())
        {
            if (agent.IsSelected())
            {
                float threatLevel = GetThreatLevel(agent);
                var nearbyAgents = GetNearbyAgents(agent.transform.position);
                
                // Use the calculated values...
            }
        }
    }
    
    private float GetThreatLevel(AgentAI agent)
    {
        return threatLevelCache.GetOrCalculate(agent, CalculateThreatLevel);
    }
    
    private List<AgentAI> GetNearbyAgents(Vector3 position)
    {
        return nearbyAgentsCache.GetOrCalculate(position, CalculateNearbyAgents);
    }
    
    private float CalculateThreatLevel(AgentAI agent)
    {
        // Expensive calculation
        float threatLevel = 0f;
        
        // Factor in health
        threatLevel += (100f - agent.GetHealthValue()) * 0.01f;
        
        // Factor in nearby enemies
        var nearbyEntities = FindNearbyEnemies(agent.transform.position);
        threatLevel += nearbyEntities.Count * 0.1f;
        
        return Mathf.Clamp01(threatLevel);
    }
    
    private List<AgentAI> CalculateNearbyAgents(Vector3 position)
    {
        return AgentAI.GetAgents()
            .Where(agent => Vector3.Distance(agent.transform.position, position) < 10f)
            .ToList();
    }
    
    private List<AIEntity> FindNearbyEnemies(Vector3 position)
    {
        return AIEntity.FindObjectsOfType(typeof(AIEntity))
            .Cast<AIEntity>()
            .Where(entity => entity.m_Group != GroupID.Resistance && 
                            Vector3.Distance(entity.transform.position, position) < 15f)
            .ToList();
    }
    
    public string GetName() => "Cached Calculations Plugin";
}
```

## Advanced Data Persistence

### Versioned Data System

Implement a versioned data system for backward compatibility:

```csharp
public interface IVersionedData
{
    int Version { get; set; }
    void UpgradeFrom(int oldVersion, object oldData);
}

public class VersionedDataManager<T> where T : class, IVersionedData, new()
{
    private const int CURRENT_VERSION = 1;
    private readonly string fileName;
    
    public VersionedDataManager(string fileName)
    {
        this.fileName = fileName;
    }
    
    public T Load()
    {
        try
        {
            string filePath = FileManager.FilePathCheck(fileName);
            if (!System.IO.File.Exists(filePath))
                return CreateDefault();
            
            // Load raw data
            var rawData = LoadRawData(filePath);
            if (rawData == null)
                return CreateDefault();
            
            // Check version and upgrade if necessary
            if (rawData.Version < CURRENT_VERSION)
            {
                var upgradedData = UpgradeData(rawData);
                Save(upgradedData); // Save upgraded version
                return upgradedData;
            }
            
            return rawData;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load versioned data: {e.Message}");
            return CreateDefault();
        }
    }
    
    public void Save(T data)
    {
        try
        {
            data.Version = CURRENT_VERSION;
            FileManager.SaveAsXML(data, fileName);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save versioned data: {e.Message}");
        }
    }
    
    private T LoadRawData(string filePath)
    {
        var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
        using (var reader = new System.IO.StreamReader(filePath))
        {
            return (T)serializer.Deserialize(reader);
        }
    }
    
    private T UpgradeData(T oldData)
    {
        var newData = new T();
        newData.UpgradeFrom(oldData.Version, oldData);
        return newData;
    }
    
    private T CreateDefault()
    {
        var defaultData = new T();
        defaultData.Version = CURRENT_VERSION;
        return defaultData;
    }
}

// Example versioned data class
[System.Serializable]
public class ModSettings : IVersionedData
{
    public int Version { get; set; } = 1;
    public string ModName { get; set; } = "Default";
    public Dictionary<string, object> Settings { get; set; } = new Dictionary<string, object>();
    
    // Version 1 additions
    public bool EnableAdvancedFeatures { get; set; } = false;
    public List<string> EnabledPlugins { get; set; } = new List<string>();
    
    public void UpgradeFrom(int oldVersion, object oldData)
    {
        if (oldData is ModSettings oldSettings)
        {
            // Copy existing data
            ModName = oldSettings.ModName;
            Settings = new Dictionary<string, object>(oldSettings.Settings);
            
            // Apply version-specific upgrades
            if (oldVersion < 1)
            {
                // Upgrade from version 0 to 1
                EnableAdvancedFeatures = false;
                EnabledPlugins = new List<string>();
            }
        }
    }
}
```

## Cross-Plugin Communication

### Plugin Message Bus

Create a message bus for inter-plugin communication:

```csharp
public static class PluginMessageBus
{
    private static Dictionary<string, List<Action<PluginMessage>>> subscribers = 
        new Dictionary<string, List<Action<PluginMessage>>>();
    private static Queue<PluginMessage> messageQueue = new Queue<PluginMessage>();
    
    public static void Subscribe(string messageType, Action<PluginMessage> handler)
    {
        if (!subscribers.ContainsKey(messageType))
            subscribers[messageType] = new List<Action<PluginMessage>>();
        
        subscribers[messageType].Add(handler);
    }
    
    public static void Unsubscribe(string messageType, Action<PluginMessage> handler)
    {
        if (subscribers.ContainsKey(messageType))
        {
            subscribers[messageType].Remove(handler);
        }
    }
    
    public static void SendMessage(PluginMessage message)
    {
        messageQueue.Enqueue(message);
    }
    
    public static void ProcessMessages()
    {
        while (messageQueue.Count > 0)
        {
            var message = messageQueue.Dequeue();
            DeliverMessage(message);
        }
    }
    
    private static void DeliverMessage(PluginMessage message)
    {
        if (subscribers.ContainsKey(message.Type))
        {
            foreach (var handler in subscribers[message.Type])
            {
                try
                {
                    handler(message);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Message handler error: {e.Message}");
                }
            }
        }
    }
}

[System.Serializable]
public class PluginMessage
{
    public string Type { get; set; }
    public string Sender { get; set; }
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Data { get; set; }
    
    public PluginMessage(string type, string sender)
    {
        Type = type;
        Sender = sender;
        Timestamp = DateTime.Now;
        Data = new Dictionary<string, object>();
    }
    
    public T GetData<T>(string key)
    {
        return Data.ContainsKey(key) ? (T)Data[key] : default(T);
    }
    
    public void SetData<T>(string key, T value)
    {
        Data[key] = value;
    }
}

// Message bus coordinator plugin
public class MessageBusCoordinator : ISrPlugin
{
    public void Initialize()
    {
        Debug.Log("Message Bus Coordinator initialized");
    }
    
    public void Update()
    {
        // Process messages every frame
        PluginMessageBus.ProcessMessages();
    }
    
    public string GetName() => "Message Bus Coordinator";
}

// Example plugins using the message bus
public class WeaponPlugin : ISrPlugin
{
    public void Initialize()
    {
        // Subscribe to weapon enhancement requests
        PluginMessageBus.Subscribe("WeaponEnhanceRequest", HandleWeaponEnhanceRequest);
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            // Send weapon enhancement notification
            var message = new PluginMessage("WeaponEnhanced", GetName());
            message.SetData("WeaponType", WeaponType.B_pistol);
            message.SetData("Enhancement", "Damage +50%");
            
            PluginMessageBus.SendMessage(message);
        }
    }
    
    private void HandleWeaponEnhanceRequest(PluginMessage message)
    {
        WeaponType weaponType = message.GetData<WeaponType>("WeaponType");
        string enhancement = message.GetData<string>("Enhancement");
        
        Debug.Log($"Enhancing {weaponType} with {enhancement}");
        // Perform enhancement...
    }
    
    public string GetName() => "Weapon Plugin";
}

public class UIPlugin : ISrPlugin
{
    public void Initialize()
    {
        // Subscribe to weapon enhancement notifications
        PluginMessageBus.Subscribe("WeaponEnhanced", HandleWeaponEnhanced);
    }
    
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            // Request weapon enhancement
            var message = new PluginMessage("WeaponEnhanceRequest", GetName());
            message.SetData("WeaponType", WeaponType.P_rifle);
            message.SetData("Enhancement", "Range +25%");
            
            PluginMessageBus.SendMessage(message);
        }
    }
    
    private void HandleWeaponEnhanced(PluginMessage message)
    {
        WeaponType weaponType = message.GetData<WeaponType>("WeaponType");
        string enhancement = message.GetData<string>("Enhancement");
        
        Manager.GetUIManager().ShowMessagePopup(
            $"{weaponType} enhanced: {enhancement}", 3);
    }
    
    public string GetName() => "UI Plugin";
}
```

## Best Practices Summary

### Advanced Modding Guidelines

1. **Use service-oriented architecture** for complex mods
2. **Implement proper state management** for reliable operation
3. **Use event-driven patterns** for loose coupling between components
4. **Optimize performance** with caching, pooling, and efficient algorithms
5. **Design for extensibility** with versioned data and plugin communication
6. **Handle errors gracefully** with comprehensive exception handling
7. **Document your APIs** for other modders to use your systems
8. **Test thoroughly** with multiple plugins and game scenarios

These advanced techniques provide the foundation for creating sophisticated, performant, and maintainable Satellite Reign mods that can interact seamlessly with the game and other mods.