# Mission Modding in Satellite Reign

This guide covers how to create and modify missions in Satellite Reign using the SR.Plugin.Pack modding framework.

## Overview

Satellite Reign uses a sophisticated quest/mission system built on modular components. Missions are constructed from Quest Elements that can be combined to create complex objectives and storylines.

## Mission System Architecture

### Core Components

1. **Quest Manager** (`QuestManager.cs`) - Central coordinator for all missions
2. **Quest Elements** - Modular mission components 
3. **Quest Actions & Reactions** - Event-driven mission logic
4. **Quest UI System** - Mission display and interaction

### Mission Building Blocks

**Base Quest Operations:**
- `QBase.cs` - Foundation for all quest operations
- `QContainer.cs` - Groups multiple quest elements together
- `QSequence.cs` - Executes quest elements in sequence
- `QSequenceTrigger.cs` - Triggers sequence execution

**Timing and Conditions:**
- `QWait.cs` - Generic wait operation
- `QWaitForProgression.cs` - Wait for game progression
- `QWaitForAgentSelected.cs` - Wait for player input
- `QWaitForAgentWithinDistance.cs` - Proximity triggers
- `QWaitForAllAgentsOutOfDanger.cs` - Safety conditions

## Mission Actions (QA Prefix)

### Mission Flow Control
```csharp
// Starting missions
QAActivateQuest.cs         // Activate a specific quest
QACancelCurrentQuest.cs    // Cancel the current mission
QACompleteCurrentQuest.cs  // Complete the current mission

// Quest management
QAQuestList.cs            // Manage quest lists
QARandomQuestList.cs      // Handle random quest selection
```

### Location and Entity Management
```csharp
QASelectLocation.cs       // Choose mission location
QASelectFacility.cs       // Select target facility
QASelectCompound.cs       // Choose compound for mission
QASpawnVIP.cs            // Spawn VIP targets
QAReleaseVIP.cs          // Release VIP characters
```

### Rewards and Progression
```csharp
QAGiveCash.cs            // Award money
QAGiveItem.cs            // Give items to player
QAGiveSecurityKey.cs     // Provide access keys
QAAssignItems.cs         // Assign specific items
QADistrictBonus.cs       // District-wide bonuses
QACompoundBonus.cs       // Compound-specific bonuses
QASetProgressionData.cs  // Update game progression
```

### Communication and UI
```csharp
QABroadcastMessage.cs    // Send messages to player
QADescriptionChange.cs   // Update quest descriptions
QAFlagAsNew.cs          // Mark content as new
QAChangePing.cs         // Update map pings
```

## Mission Reactions (QR Prefix)

Reactions respond to player actions and game events:

```csharp
QRFacilityBreached.cs        // Triggered when facility is infiltrated
QRDataTerminalAccessed.cs    // Responds to terminal hacking
QROutOfDanger.cs            // Triggered when agents are safe
QRProgressionData.cs        // Responds to progression changes
QRPurchaseInfo.cs           // Handles purchase events
QRWait.cs                   // Timing-based reactions
QRAIEntityEvent.cs          // AI entity state changes
```

## Direct Quest Operations (Q Prefix)

### Mission Management
```csharp
QCreateQuest.cs          // Create new quest instances
QCompleteQuest.cs        // Complete specific quests
QCancelQuest.cs          // Cancel quest operations
```

### Entity Operations
```csharp
QSpawnVIP.cs             // Spawn VIP characters
QReleaseVIP.cs           // Release VIP targets
QHijackVIP.cs            // Hijack VIP functionality
QVIPMods.cs              // VIP modifications

QFacilityBreached.cs     // Handle facility breach
QDataTerminalAccessed.cs // Terminal access events
QPickupItemAquired.cs    // Item pickup events
```

### Tutorial Operations
```csharp
QDoCloneTutorial.cs      // Clone system tutorial
QDoFastTravelTutorial.cs // Fast travel tutorial
QDoSkillsTutorial.cs     // Skills system tutorial
QShowTutorialPointer.cs  // Tutorial UI pointers
```

## Creating Custom Missions

### Basic Mission Plugin Structure

```csharp
using System;
using UnityEngine;

public class CustomMissionMod : ISrPlugin
{
    private QuestManager questManager;
    private bool missionActive = false;
    
    public void Initialize()
    {
        Debug.Log("Custom Mission Mod initialized");
        questManager = Manager.GetQuestManager();
    }
    
    public void Update()
    {
        if (Manager.Get().GameInProgress && !missionActive)
        {
            // Check conditions to start custom mission
            if (ShouldStartCustomMission())
            {
                StartCustomMission();
            }
        }
        
        // Handle mission input
        if (Input.GetKeyDown(KeyCode.F9))
        {
            TriggerCustomMissionEvent();
        }
    }
    
    private bool ShouldStartCustomMission()
    {
        // Add your mission trigger conditions here
        return AgentAI.GetAgents().Count > 0 && 
               Manager.GetMoneyManager().GetCurrentFunds() > 1000;
    }
    
    private void StartCustomMission()
    {
        missionActive = true;
        
        // Display mission briefing
        Manager.GetUIManager().ShowBannerMessage(
            "CUSTOM MISSION", 
            "Infiltrate the Corporate Facility", 
            "Retrieve the data without being detected", 
            8
        );
        
        // Set mission objectives using quest system
        CreateMissionObjectives();
    }
    
    private void CreateMissionObjectives()
    {
        // Example: Create a facility breach objective
        // This would require accessing quest creation functionality
        
        Manager.GetUIManager().ShowMessagePopup(
            "Mission Objective: Breach the target facility", 5
        );
    }
    
    private void TriggerCustomMissionEvent()
    {
        if (missionActive)
        {
            // Custom mission event logic
            Manager.GetUIManager().ShowMessagePopup("Mission event triggered!", 3);
        }
    }
    
    public string GetName()
    {
        return "Custom Mission Mod";
    }
}
```

### Mission Scripting Patterns

**1. Sequential Mission Steps:**
```csharp
// Use QSequence to chain mission elements
// 1. Wait for agent selection
// 2. Move to location
// 3. Complete objective
// 4. Provide reward
```

**2. Branching Missions:**
```csharp
// Use QContainer with conditional logic
// Different paths based on player choices or actions
```

**3. Timed Missions:**
```csharp
// Use QWait variants for time limits
// QWaitForProgressionData for state-based timing
```

## Accessing Quest System Components

### Quest Manager Access
```csharp
var questManager = Manager.GetQuestManager();

// Check active quests
if (questManager != null)
{
    // Quest manager operations
    // Note: Specific methods depend on available API
}
```

### Location and Entity Integration
```csharp
// Work with game locations
var locationManager = Manager.GetLocationManager();
var locations = locationManager.m_Locations;

// Spawn and manage VIPs
foreach (var location in locations)
{
    // Check if location is suitable for mission
    // Spawn VIPs or set up mission elements
}
```

### Progression Integration
```csharp
var progressionManager = ProgressionManager.Get();
var currentDistrict = progressionManager.CurrentDistrict;

// Tailor missions based on game progression
// Lock/unlock content based on district
```

## Advanced Mission Features

### Custom Mission UI

```csharp
// Create custom mission UI elements
var uiManager = Manager.GetUIManager();

// Show custom mission briefings
uiManager.ShowBannerMessage("Mission Title", "Objective", "Details", duration);

// Custom objective tracking
uiManager.ShowMessagePopup("Objective Complete", 5);
```

### VIP and Target Management

```csharp
// Create custom VIP targets
public void CreateCustomVIP(AIEntity entity)
{
    entity.gameObject.AddComponent<VIP>();
    var vip = entity.GetComponent<VIP>();
    vip.VIPType = VIPType.InfoTarget;
    entity.HasBeenScanned = false;
}
```

### Mission Rewards

```csharp
// Implement custom reward systems
public void GiveCustomReward()
{
    // Money rewards
    Manager.GetMoneyManager().OffsetFunds(5000, true, true);
    
    // Item rewards - requires item system integration
    // Skill rewards - requires skill system integration
}
```

## Integration with Game Systems

### Working with Facilities
```csharp
// Monitor facility breach events
// Integrate with security systems
// Handle alarm states and responses
```

### District and Compound Integration
```csharp
// Mission availability based on district control
// Compound-specific objectives
// Territory-based mission chains
```

### Agent and Skill Integration
```csharp
// Class-specific missions (Hacker, Soldier, etc.)
// Skill requirement gating
// Ability-based objectives
```

## Mission Save/Load System

Missions must integrate with the save system:

```csharp
// Implement ISavableQuest interface for persistence
// Save mission state and progress
// Restore mission state on load
```

## Debugging and Testing

### Debug Tools
```csharp
// Enable debug output
SRInfoHelper.isLogging = true;
SRInfoHelper.Log("Mission debug message");

// Use UI feedback for testing
Manager.GetUIManager().ShowMessagePopup("Debug: Mission state", 3);
```

### Testing Patterns
```csharp
// Test mission triggers
// Validate quest completion conditions
// Verify reward distribution
// Test save/load functionality
```

## Best Practices

1. **Modular Design**: Use quest containers and sequences for complex missions
2. **State Management**: Properly track mission state and handle edge cases
3. **Error Handling**: Gracefully handle missing components or failed operations
4. **Performance**: Avoid heavy operations in Update() loops
5. **Compatibility**: Test with other mods and ensure clean integration
6. **Save Integration**: Ensure missions work correctly with save/load system

## Limitations and Considerations

- Quest creation API may be limited in modding context
- Direct manipulation of quest system requires careful integration
- Mission UI customization is constrained by existing UI framework
- Complex mission logic may require extensive game system knowledge
- Save/load compatibility must be maintained

## Example Mission Types

### 1. Facility Infiltration Mission
- Use `QFacilityBreached` for objective completion
- Integrate with stealth mechanics
- Custom VIP targets with data retrieval

### 2. District Liberation Mission
- Progressive objectives across multiple compounds
- District-wide progression tracking
- Resource management elements

### 3. Character Rescue Mission
- VIP spawning and protection
- Escort mechanics
- Time-limited objectives

### 4. Corporate Espionage Mission
- Data terminal access requirements
- Multi-stage information gathering
- Corporate hierarchy navigation

This mission modding system provides a powerful foundation for creating custom content while integrating seamlessly with Satellite Reign's existing game systems.