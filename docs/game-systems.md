# Game Systems and Manager Classes

## Overview

Satellite Reign uses a centralized Manager pattern to provide access to various game systems. This document covers the available managers, their functionality, and how to interact with different game systems through modding.

## Core Manager Pattern

### Manager.Get() - Central Access Point
The `Manager.Get()` method provides access to the main game manager and system state:

```csharp
// Check if game is in progress
if (Manager.Get().GameInProgress)
{
    // Safe to perform game operations
}

// Check if game is loading
if (Manager.Get().IsLoading())
{
    // Game is loading, avoid heavy operations
}
```

### System Manager Access
Each game system has its own manager accessible through static methods:

```csharp
UIManager uiManager = Manager.GetUIManager();
AudioManager audioManager = Manager.GetAudioManager();
InputManager inputManager = Manager.GetInputManager();
WeaponManager weaponManager = Manager.GetWeaponManager();
// ... and many more
```

## Available Game Systems

### 1. UI Manager (`Manager.GetUIManager()`)

Handles all user interface operations and feedback:

```csharp
UIManager uiManager = Manager.GetUIManager();

// Display messages
uiManager.ShowMessagePopup("Hello World!", 5); // 5 seconds duration
uiManager.ShowWarningPopup("Warning message", 3);
uiManager.ShowSubtitle("Subtitle text", 10);

// Banner messages
uiManager.ShowBannerMessage("Title", "Top Text", "Bottom Text", 5);

// Modal dialogs
uiManager.DoModalMessageBox("Title", "Message", 
    InputBoxUi.InputBoxTypes.MbOkcancel, "OK", "Cancel", callbackFunction);

// Screen effects
uiManager.ShowBlank("Loading..."); // Full screen with text
uiManager.ToggleTextWindow("Toggle this window");

// UI state management
uiManager.ToggleEverything(true); // Show/hide all UI
```

### 2. Audio Manager (`Manager.GetAudioManager()`)

Controls game audio and music:

```csharp
AudioManager audioManager = Manager.GetAudioManager();

// Music control
if (audioManager.IsLoginMusicPlaying())
{
    audioManager.StopAllMusic(true);
}

// Audio state queries
bool isMusicPlaying = audioManager.IsMusicPlaying();
bool isSoundEnabled = audioManager.IsSoundEnabled();
```

### 3. Input Manager (`Manager.GetInputManager()`)

Handles input and entity selection:

```csharp
InputManager inputManager = Manager.GetInputManager();

// Get selected entities
List<AgentAI> selectedAgents = inputManager.GetPlayerSelectedEntities();
List<AIEntity> selectedNPCs = inputManager.GetNonPlayerSelectedEntities();

// Selection management
inputManager.Select(entityUID, addToSelection: true);
inputManager.ClearSelection();

// Input control
InputControlUI inputControl = inputManager.GetInputControl();
Vector3 mouseWorldPos;
inputControl.GetGroundPointUnderMouse(out mouseWorldPos);
```

### 4. Money Manager (`Manager.GetMoneyManager()`)

Manages the game's economy:

```csharp
MoneyManager moneyManager = Manager.GetMoneyManager();

// Current funds
int currentFunds = moneyManager.GetCurrentFunds();
moneyManager.SetCurrentFunds(1000000);

// Transactions
moneyManager.OffsetFunds(5000, showFeedback: true, playSound: true);

// Bank systems
moneyManager.UpdateBankSiphonAmount(MoneyManager.Banks.CBDDistrict_1, 1000);
```

### 5. Weapon Manager (`Manager.GetWeaponManager()`)

Controls weapon data and behavior:

```csharp
WeaponManager weaponManager = Manager.GetWeaponManager();

// Access weapon data
WeaponData[] allWeapons = weaponManager.m_WeaponData;
WeaponData pistolData = weaponManager.m_WeaponData[(int)WeaponType.B_pistol];

// Get weapon prefabs
WeaponPrefab prefab = weaponManager.GetPrefab(WeaponType.P_Minigun);
WeaponAttachmentAmmo ammoData = weaponManager.GetAmmoData(WeaponType.B_pistol, ammoType);
```

### 6. Item Manager (`Manager.GetItemManager()`)

Manages game items and equipment:

```csharp
ItemManager itemManager = Manager.GetItemManager();

// Get all items
foreach (ItemManager.ItemData item in itemManager.GetAllItems())
{
    Debug.Log($"Item: {item.m_Name} - Cost: {item.m_Cost}");
}

// Find specific items
ItemManager.ItemData specificItem = itemManager.GetItemData(itemId);
```

### 7. Clone Manager (`CloneManager.Get()`)

Handles agent appearance and identity:

```csharp
CloneManager cloneManager = CloneManager.Get();

// Access clone data
CloneableData cloneData = cloneManager.GetCloneableData(agent.CurrentCloneableId);

// Modify appearance
cloneData.Sex = WardrobeManager.Sex.Female;
cloneData.m_RandomSeed = Wardrobe.GenerateRandomSeed();
cloneData.WardrobeType = WardrobeManager.WardrobeType.AgentSoldierBacker;

// Identity management
cloneData.IdentityId = IdentityManager.GetRandomNameID(cloneData.Sex);
```

### 8. Time Manager (`TimeManager`)

Controls game time and pause functionality:

```csharp
// Game time
float gameTime = TimeManager.GameTime;
float realTime = TimeManager.RealTime;

// Time scaling
TimeManager.TimeScaler timeScaler = TimeManager.AddTimeScaler();
timeScaler.TimeScale = 2f; // Double speed
timeScaler.Pause();        // Pause game
timeScaler.Reset();        // Resume normal speed
```

### 9. Camera Manager (`Manager.GetCamera()`)

Controls camera behavior:

```csharp
CameraManager cameraManager = Manager.GetCamera();

// Camera properties
cameraManager.m_ScrollSpeed = 15f; // Adjust scroll speed

// Camera state queries and controls
// (specific methods depend on available API)
```

### 10. Plugin Manager (`Manager.GetPluginManager()`)

Manages mod loading and plugin system:

```csharp
PluginManager pluginManager = Manager.GetPluginManager();

// Plugin paths
string pluginPath = pluginManager.PluginPath;

// Plugin management functionality
// (limited API access for security)
```

## Agent and AI Systems

### AgentAI - Player Character Control

```csharp
// Get all player agents
foreach (AgentAI agent in AgentAI.GetAgents())
{
    // Agent state
    bool isSelected = agent.IsSelected();
    bool isDead = agent.m_Dead;
    bool isDowned = agent.IsDowned;
    bool isInDanger = agent.IsInDanger;
    
    // Agent class
    AgentAI.AgentClass agentClass = agent.GetClass();
    string className = agent.AgentClassName();
    
    // Health and energy
    agent.SetHealthValue(100);
    agent.m_Health.SetHealthFull();
    agent.m_Energy.AddEnergy(500);
    agent.m_Energy.SetInfiniteEnergy(true);
    
    // Weapons and ammunition
    agent.AddAmmo(50);
    agent.SetWeapon(10);
    WeaponType currentWeapon = agent.GetWeaponType(0);
    
    // Skills and abilities
    agent.ServerAddAbility(abilityId);
    agent.SkillUpdated();
    
    // Position and movement
    agent.transform.position = newPosition;
    agent.RespawnAtCurrentLocation();
    agent.RespawnAt(position, rotation);
}

// Get specific agents
AgentAI firstAgent = AgentAI.FirstAgentAi();
AgentAI selectedAgent = AgentAI.FirstSelectedAgentAi();
AgentAI specificAgent = AgentAI.GetAgent(AgentAI.AgentClass.Soldier);
```

### AIEntity - General AI Control

```csharp
// Find all AI entities
foreach (AIEntity entity in AIEntity.FindObjectsOfType(typeof(AIEntity)))
{
    // Entity properties
    bool isHuman = entity.IsHuman();
    bool isMech = entity.IsMech();
    bool isSelected = entity.IsSelected();
    GroupID group = entity.m_Group;
    
    // Control and selection
    entity.m_IsControllable = true;
    entity.m_IsIgnoringInput = false;
    entity.SetSelected(true);
    
    // Health and status
    entity.SetHealthValue(100);
    entity.m_Health.SetAllFull();
    
    // AI behavior
    entity.m_AIAbilities = AIAbilities.LicensedToKill;
    bool hasAbility = entity.HasAIAbility(AIAbilities.Shoot);
}
```

### Vehicle Systems (CarAI)

```csharp
// Find and control vehicles
foreach (CarAI car in UnityEngine.Object.FindObjectsOfType(typeof(CarAI)))
{
    if (car.IsAddedToWorld)
    {
        // Vehicle control
        car.m_IsControllable = true;
        car.m_IgnoreInput = false;
        car.SetParked();
        
        // Agent interaction
        foreach (AgentAI agent in AgentAI.GetAgents())
        {
            if (agent.IsSelected())
            {
                // Enter or exit vehicle
                if (agent.m_InCar == car)
                {
                    agent.ExitCar(car.transform, doorIndex);
                }
                else
                {
                    agent.UseCar(car.transform, doorIndex);
                }
            }
        }
        
        // Vehicle state
        int doorsInUse = car.m_DoorsInUse;
        string carName = car.GetName();
    }
}
```

## World and Environment Systems

### Progression Manager (`ProgressionManager.Get()`)

```csharp
ProgressionManager progressionManager = ProgressionManager.Get();

// District information
District currentDistrict = progressionManager.CurrentDistrict;

// Events
progressionManager.OnDistrictChange += HandleDistrictChange;
```

### Identity Manager (`IdentityManager.Get()`)

```csharp
IdentityManager identityManager = IdentityManager.Get();

// Name generation
int randomNameId = IdentityManager.GetRandomNameID(WardrobeManager.Sex.Male);

// Name lookup
string firstName, lastName;
identityManager.GetName(nameId, out firstName, out lastName);
```

### AI World Manager (`Manager.GetAIWorld()`)

```csharp
AIWorld aiWorld = Manager.GetAIWorld();

// Civilian management
CivilianAI[] civTemplates = aiWorld.CivTemplates;
aiWorld.DebugSpawnRandomCivAtMouse();

// Prefab access
AgentAI assassinPrefab = aiWorld.m_AssassinPrefab;
```

## System Integration Patterns

### Manager Initialization Check
Always verify managers are available before use:

```csharp
public void SafeManagerAccess()
{
    try
    {
        if (Manager.Get() != null && Manager.Get().GameInProgress)
        {
            var uiManager = Manager.GetUIManager();
            if (uiManager != null)
            {
                uiManager.ShowMessagePopup("Safe access successful", 3);
            }
        }
    }
    catch (Exception e)
    {
        Debug.LogError($"Manager access failed: {e.Message}");
    }
}
```

### System State Monitoring
Monitor system states for proper timing:

```csharp
public class SystemMonitor : ISrPlugin
{
    private bool systemsReady = false;
    
    public void Update()
    {
        if (!systemsReady)
        {
            systemsReady = CheckSystemsReady();
            if (systemsReady)
            {
                OnSystemsReady();
            }
        }
    }
    
    private bool CheckSystemsReady()
    {
        return Manager.Get() != null &&
               Manager.Get().GameInProgress &&
               !Manager.Get().IsLoading() &&
               Manager.GetUIManager() != null &&
               AgentAI.GetAgents().Count > 0;
    }
    
    private void OnSystemsReady()
    {
        Debug.Log("All systems ready for mod operations");
        // Initialize mod functionality that requires all systems
    }
    
    public void Initialize() { }
    public string GetName() => "System Monitor";
}
```

### Cross-System Communication
Coordinate between different game systems:

```csharp
public class SystemCoordinator
{
    public void CoordinatedAction()
    {
        // Pause time
        var timeScaler = TimeManager.AddTimeScaler();
        timeScaler.Pause();
        
        // Show UI feedback
        Manager.GetUIManager().ShowMessagePopup("Time paused for coordinated action", 3);
        
        // Modify agents
        foreach (var agent in AgentAI.GetAgents())
        {
            if (agent.IsSelected())
            {
                // Health system
                agent.SetHealthValue(100);
                
                // Economy system
                Manager.GetMoneyManager().OffsetFunds(-1000, true, true);
                
                // Clone system
                var cloneData = CloneManager.Get().GetCloneableData(agent.CurrentCloneableId);
                // Modify clone as needed
            }
        }
        
        // Resume time
        timeScaler.Reset();
    }
}
```

## Error Handling and Safety

### Safe Manager Access Pattern
```csharp
public static class SafeManagerAccess
{
    public static T SafeGetManager<T>(Func<T> getManager, string managerName = "Unknown") where T : class
    {
        try
        {
            var manager = getManager();
            if (manager == null)
            {
                Debug.LogWarning($"{managerName} manager is null");
            }
            return manager;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to access {managerName} manager: {e.Message}");
            return null;
        }
    }
    
    // Usage:
    // var uiManager = SafeGetManager(() => Manager.GetUIManager(), "UI");
}
```

### System Availability Checks
```csharp
public static class SystemChecker
{
    public static bool IsUISystemReady()
    {
        return Manager.Get()?.GameInProgress == true && 
               Manager.GetUIManager() != null;
    }
    
    public static bool IsGameStateValid()
    {
        return Manager.Get() != null &&
               Manager.Get().GameInProgress &&
               !Manager.Get().IsLoading();
    }
    
    public static bool AreAgentsAvailable()
    {
        return AgentAI.GetAgents()?.Count > 0;
    }
}
```

This comprehensive overview of Satellite Reign's game systems provides the foundation for creating complex mods that interact with multiple game systems safely and effectively.