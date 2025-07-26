# Mission Editor Usage Guide

This guide explains how to use the new mission editing functionality added to the Satellite Reign modding tools.

## Overview

The mission editing system consists of two main components:

1. **LoadCustomData Mod** - Exports and loads mission data from the game
2. **SatelliteReignModdingTools Mission Browser** - Visual editor for creating and modifying missions

## Getting Started

### Step 1: Export Mission Data from Game

1. Load the LoadCustomData mod in Satellite Reign
2. Start or load a game to initialize the quest system
3. The mod will automatically export quest data to `questDefinitions.xml` on first run
4. To manually export quest data, press the **Delete** key in-game

### Step 2: Edit Missions in Mission Browser

1. Open the SatelliteReignModdingTools application
2. Click the **"Mission Browser"** button
3. The Mission Browser will automatically load existing quest data

## Mission Browser Interface

### Main Sections

**Left Panel - Quest List:**
- Shows all available quests/missions
- Click on a quest to edit its properties

**Center Panel - Quest Properties:**
- **ID**: Unique quest identifier (read-only)
- **Title**: Localization key for quest title
- **State**: Current quest state (NotStarted, Active, Completed, etc.)
- **Hidden**: Whether quest appears in mission control
- **Show Debrief**: Whether to show completion debrief

**Right Panel - Quest Components:**
- **Actions**: What happens when quest triggers
- **Reactions**: Conditions that trigger quest actions
- **Descriptions**: Quest text and objectives

### Creating a New Mission

1. Click **"New Quest"** button
2. Fill in the quest properties:
   - Set a meaningful title (localization key)
   - Choose appropriate state
   - Configure visibility options
3. Add quest actions and reactions (see below)
4. Click **"Save Quest"** to update the quest
5. Click **"Save All"** to write changes to XML file

### Adding Quest Actions

Quest actions define what happens when a quest is triggered:

1. Select an action type from the **Action Type** dropdown:
   - **QAGiveItem**: Award items to player
   - **QAGiveCash**: Give money reward
   - **QASelectLocation**: Choose mission location
   - **QASpawnVIP**: Spawn VIP characters
   - **QABroadcastMessage**: Display messages to player
   - **QAActivateQuest**: Trigger other quests

2. Click **"Add Action"** to create the action
3. The action will appear in the Actions list

### Adding Quest Reactions

Quest reactions define what conditions trigger the quest:

1. Select a reaction type from the **Reaction Type** dropdown:
   - **QRWait**: Wait for time period
   - **QRDataTerminalAccessed**: Data terminal interaction
   - **QRFacilityBreached**: Facility security breach
   - **QRProgressionData**: Game progression triggers

2. Click **"Add Reaction"** to create the reaction
3. The reaction will appear in the Reactions list

### Loading Mission Data Back to Game

1. Save your changes in the Mission Browser
2. Copy the `questDefinitions.xml` file to your plugin directory
3. The LoadCustomData mod will automatically load the modified quest data
4. Start or reload your game to see the changes

## Mission Design Patterns

### Simple Reward Mission
```
Reaction: QRDataTerminalAccessed (any terminal)
Action: QAGiveCash (1000 credits)
Action: QABroadcastMessage ("Data retrieved successfully!")
```

### Location-Based Mission
```
Action: QASelectLocation (choose target facility)
Action: QASpawnVIP (place target character)
Reaction: QRFacilityBreached (target facility)
Action: QAGiveCash (reward)
Action: QACompleteCurrentQuest
```

### Chain Mission
```
Reaction: QRDataTerminalAccessed
Action: QABroadcastMessage ("Phase 1 complete")
Action: QAActivateQuest (next mission ID)
```

## Best Practices

### Quest Design
- Use descriptive titles with localization keys
- Start missions in "NotStarted" state
- Set "Show Debrief" for important missions
- Use "Hidden" for background/tutorial quests

### Action Ordering
- Place condition checks before rewards
- Use broadcast messages for player feedback
- Complete quests explicitly with QACompleteCurrentQuest

### Testing
- Test mission flow in-game after each change
- Use Debug mode to verify quest triggers
- Check that rewards are properly awarded

## Advanced Features

### Custom Quest Parameters

The system supports custom parameters for actions and reactions through the parameter dictionary. This allows for:

- Custom item rewards with specific IDs
- Variable cash amounts
- Specific location targeting
- Custom message text

### Quest Hierarchies

Missions can be organized in hierarchical structures:
- Parent quests containing multiple objectives
- Sequential mission chains
- Branching storylines based on player choices

### Integration with Game Systems

Missions integrate with all major game systems:
- **Location Manager**: Dynamic location selection
- **VIP System**: Character spawning and interaction
- **Progression System**: Game state tracking
- **UI System**: Player feedback and notifications

## Troubleshooting

### Common Issues

**Mission Browser won't load:**
- Check that questDefinitions.xml exists in the correct directory
- Verify XML file is not corrupted
- Try clicking "Load Data" to refresh

**Missions don't appear in game:**
- Ensure LoadCustomData mod is active
- Check that quest data was exported properly
- Verify quest state is set correctly

**Actions not triggering:**
- Check that reactions are properly configured
- Verify quest is in "Active" state
- Test trigger conditions in game

### File Locations

- **Export Location**: `<PluginPath>/questDefinitions.xml`
- **Debug Output**: `<PluginPath>/questDefinitions.txt`
- **Backup Files**: Created automatically before saves

## Limitations

### Current Limitations
- Complex quest logic requires manual scripting
- Visual quest graph not yet implemented
- Limited action parameter editing in UI
- Save game compatibility requires testing

### Future Enhancements
- Visual quest flow editor
- Advanced action parameter editors
- Quest validation and error checking
- Template system for common mission types

This mission editing system provides a powerful foundation for creating custom content while maintaining compatibility with Satellite Reign's existing quest system.