# LoadCustomData Mod for Satellite Reign

*Version 2.1 - Complete Integration*

The LoadCustomData mod is a clean, service-based data import/export system for Satellite Reign that allows modders and players to customize items, translations, enemy data, and other game content through XML files.

## Overview

This mod provides a comprehensive framework for modifying Satellite Reign's game data through XML files. It features automatic loading on game startup with proper game state detection, comprehensive change tracking, and enhanced translation system integration.

## Features

### Core Systems (Refactored Architecture)
- **LoadCustomData**: Clean plugin coordinator with service-based architecture
- **ItemDataManager**: Enhanced item management with change detection and comprehensive field mapping
- **TranslationManager**: XML-based translation system with proper ITEM_[ID]_NAME pattern support
- **SpawnCardManager**: Fixed enemy spawn management (no more Unity coroutine issues)
- **Auto-Loading**: Intelligent loading with game state detection using `Manager.Get().GameInProgress`

### Supported Data Types
- **Item definitions**: Stats, costs, modifiers, abilities with comprehensive field mapping
- **Weapon data**: Complete weapon stats (damage, reload, range, ammo) via WeaponManager integration
- **Game translations**: XML format with automatic ITEM_[ID]_NAME pattern support  
- **Enemy spawn cards**: Enhanced enemy definitions and spawn management
- **Quest data**: Quest definitions and objectives
- **Item icons**: PNG format export/import
- **Complete game data**: Export for analysis and debugging

## Installation

1. Place the `LoadCustomData.dll` file in your Satellite Reign mods directory:
   ```
   c:\Modding\SatelliteReign\Mods\
   ```

2. Ensure the following directory structure exists:
   ```
   Mods/
   ├── LoadCustomData.dll
   ├── icons/                 (created automatically)
   ├── ToolsData/            (for SatelliteReignModdingTools compatibility)
   └── [data files]          (XML/JSON files)
   ```

3. Start Satellite Reign - the mod will initialize automatically

## Key Bindings

| Key | Action | Description |
|-----|--------|-------------|
| **F9** | Import All Data | Import items, weapons, translations, enemies, and quests from XML files |
| **F10** | Export All Data | Export items, weapons, translations, enemies, and quests to XML files |
| **F11** | Auto-Load | Auto-load all data (same as game startup) |
| **F4** | Diagnostics | Run system diagnostics (item count, weapon count, file status) |
| **INSERT** | Show Help | Display help information with current keybindings |

## File Formats

### items.xml (Primary Format)
Current XML format for item definitions with comprehensive field support.

### weapons.xml (New Format)
Complete weapon data system for modifying weapon stats:
- Damage (min/max)
- Reload time and speed
- Range and accuracy
- Ammo capacity
- Special weapon properties

### translations.xml (Primary Format) 
XML-based translation system with proper game integration:
```xml
<?xml version="1.0" encoding="utf-8"?>
<ArrayOfTranslationElementDTO>
  <TranslationElementDTO>
    <Key>ITEM_61_NAME</Key>
    <Element>
      <m_token>ITEM_61_NAME</m_token>
      <m_Translations>
        <string>Lincoln P97 Test</string>
        <string>Lincoln P97 Test</string>
        <string>Lincoln P97 Test</string>
        <string>Lincoln P97 Test</string>
        <string>Lincoln P97 Test</string>
        <string>Lincoln P97 Test</string>
        <string>Lincoln P97 Test</string>
        <string>Lincoln P97 Test</string>
      </m_Translations>
    </Element>
  </TranslationElementDTO>
</ArrayOfTranslationElementDTO>
```

### spawnCards.xml
Enemy spawn card definitions for customizing enemy encounters.

### quests.xml
Quest definitions and objectives for custom quest content.

## Auto-Loading Features

The mod automatically performs the following when a game is loaded (detected via `Manager.Get().GameInProgress`):

1. **Translation Loading**: Loads `translations.xml` with proper ITEM_[ID]_NAME pattern support
2. **Item Definition Loading**: Loads `items.xml` with comprehensive change detection  
3. **Weapon Data Loading**: Loads `weapons.xml` and updates WeaponManager.m_WeaponData array
4. **Enemy Data Loading**: Loads `spawnCards.xml` for custom enemy encounters
5. **Quest Data Loading**: Loads `quests.xml` for custom quest content
6. **Safe Updates**: Updates only data that has actually changed, preserving game state
7. **Change Tracking**: Logs detailed information about what changed (cost, damage, etc.)
8. **Smart Loading**: Prevents overwriting manual edits during auto-load

## Advanced Features

### Item Modification System
- **Change Detection**: `HasItemChanged()` method prevents unnecessary updates
- **Comprehensive Field Updates**: Updates m_Cost, m_ResearchCost, m_CurrentResearchCost, and more
- **Safe Updates**: Preserves runtime state (research progress, blueprints, etc.)
- **Individual Updates**: Updates items one by one rather than replacing entire lists
- **Detailed Logging**: Tracks exactly what changed (e.g., "Cost: 300 -> 333")

### Translation Management
- **XML Integration**: Direct access to TextManager.m_FastLanguageLookup via reflection
- **Game Pattern Support**: Uses proper ITEM_[ID]_NAME and ITEM_[ID]_DESCRIPTION patterns
- **Smart Loading**: Preserves manual edits, prevents overwrites during auto-load
- **Multi-language Support**: Supports all 8 game languages
- **UpdateGameTranslations**: New method for loading translations.xml files

## File Structure

| File | Description |
|------|-------------|
| `items.xml` | Item definitions with comprehensive field mapping |
| `weapons.xml` | Weapon data with damage, reload, range, and ammo stats |
| `translations.xml` | Translation data with ITEM_[ID]_NAME pattern support |
| `spawnCards.xml` | Enemy spawn card definitions |
| `quests.xml` | Quest definitions and objectives |
| `icons/*.png` | Item icon images |

## Current Status and Features

### ✅ Working Features
- **Auto-loading**: Uses SyndicateMod timing pattern for reliable game state detection
- **Translation system**: XML loading with proper ITEM_[ID]_NAME pattern support
- **Weapon data system**: Complete WeaponManager.m_WeaponData modification support with magazine size tracking
- **Spawn cards system**: Dual-format enemy and spawn card management (enemyentries.xml + spawnCards.xml)
- **Save game protection**: Excludes 19 game progress/runtime fields to preserve research progress and player state
- **Change detection**: Only updates items/weapons that actually changed
- **Item cost system**: All cost-related fields synchronized (m_Cost, m_BlueprintCost, etc.)
- **Magazine size capture**: Properly exports/imports weapon magazine capacity in m_max_ammo field
- **XML serialization fixes**: Resolved enum serialization issues and file format conflicts
- **Comprehensive refresh**: ItemManager events and agent notification system
- **Build system**: Works correctly with MSBuild via build.bat
- **Service architecture**: Clean separation of concerns with proper dependency injection

### 🛠️ Recent Major Fixes (v2.1)
- **Fixed auto-load timing**: Now uses `IsItemManagerReady()` check like working mods
- **Enhanced item cost updates**: Updates all cost-related fields for proper UI display
- **Save game integrity protection**: Excludes 19 game progress/runtime fields (research progress, inventory, etc.)
- **Weapon data integration**: Complete WeaponManager.m_WeaponData modification with change detection
- **WeaponType enum serialization fix**: Changed to int to avoid XML serialization errors with undefined enum values
- **Spawn cards XML loading fix**: Corrected file path conflict between enemyentries.xml and spawnCards.xml
- **Magazine size confirmation**: Verified m_max_ammo field properly captures weapon magazine capacity
- **XML serialization fixes**: Fixed Dictionary and null value issues in QuestDataManager and WeaponDataManager
- **Dual data system**: Proper integration of ItemData (costs/research) and WeaponData (weapon stats)
- **Robust error handling**: Enhanced null-safety and exception handling in weapon data constructors
- **Improved refresh system**: Multiple UI refresh methods and agent notification

### 🚧 Remaining Improvements  
- **Separate import/export buttons**: Currently combined F9/F10 operations
- **Enhanced validation**: More comprehensive data validation for modifiers
- **Individual file operations**: Import/export specific data types independently

## Item Properties Reference

### Basic Properties
- **ID**: Unique item identifier
- **Name**: Display name
- **Slot**: Equipment slot (Weapon, Armor, Augmentation, etc.)
- **Cost**: Purchase cost
- **ResearchCost**: Research time cost
- **BlueprintCost**: Blueprint acquisition cost
- **PrototypeCost**: Prototype acquisition cost

### Gameplay Properties
- **StealthVsCombat**: Balance between stealth and combat effectiveness (0.0-1.0)
- **PrereqID**: Prerequisite item ID
- **GearSubCategory**: Item subcategory
- **WeaponType**: Weapon classification
- **Modifiers**: Stat modifications and effects
- **Abilities**: Special ability IDs

### Availability Flags
- **AvailableToPlayer**: Can player access this item
- **PlayerCanResearchFromStart**: Available for research from game start
- **PlayerStartsWithBlueprints**: Player begins with blueprints
- **PlayerStartsWithPrototype**: Player begins with prototype

## Modifier Types
Common modifier types for item effects:
- `Health`, `Hacking`, `Stealth`, `Accuracy`
- `Damage`, `RateOfFire`, `ReloadTime`, `Range`
- `MovementSpeed`, `CoverBonus`, `SightRange`
- `EnergyRegenRate`, `HealthRegenRate`

## Troubleshooting

### Common Issues

**Items not appearing in-game:**
- Verify AvailableToPlayer is set to true
- Check that prerequisite items exist
- Ensure item validation passes
- Review logs for validation errors

**Translations not working:**
- Check translation keys follow pattern: `ITEM_[ID]_NAME`, `ITEM_[ID]_DESCRIPTION`
- Verify translations.xml format matches expected structure
- Ensure translation arrays have entries for all 8 languages
- Check that auto-load preserves your manual edits

**Price changes not showing:**
- m_Cost field may update successfully but UI doesn't refresh
- Currently under investigation
- Check logs for "Cost: [old] -> [new]" messages to confirm updates

**Build issues:**
- Use `build.bat` script instead of `dotnet build`
- Ensure proper .NET 3.5 compatibility and game assembly references

### Debugging and Logging

**Log Locations:**
- Unity Console (F1 in-game for debug view)
- `C:\Modding\SatelliteReign\SatelliteReignWindows_Data\output_log.txt`

**Change Detection Logging:**
The system now logs only actual changes with detailed information:
```
ItemDataManager: Item 61 changes detected: Cost: 300 -> 333, Progression: 0.5 -> 0.7
TranslationManager: Updated translation 'ITEM_61_NAME' -> 'Modified Item Name'
```

**Translation System Debugging:**
- Check ITEM_[ID]_NAME pattern usage
- Verify XML format matches game structure  
- Monitor auto-load preservation of manual edits

## Technical Architecture

### Service-Based Design
```
LoadCustomData.cs (400 lines)
├── AutoLoadService - Game state detection and auto-loading
├── CommandService - Hotkey command routing and diagnostics
├── UINotificationService - In-game notifications
└── Services/
    ├── ItemDataManager - Enhanced item operations with change detection
    ├── WeaponDataManager - Complete weapon data system
    ├── TranslationManager - XML-based translation integration
    ├── SpawnCardManager - Fixed enemy spawn management
    ├── QuestDataManager - Quest system integration
    └── FileManager - File I/O operations
```

### Key Technical Improvements
- **Plugin Context**: Fixed Unity coroutine issues (plugins can't use StartCoroutine)
- **Game Integration**: Proper game state detection with `Manager.Get().GameInProgress`
- **Reflection Access**: Safe access to private game fields like TextManager.m_FastLanguageLookup
- **Change Tracking**: Intelligent detection of data modifications
- **Error Handling**: Comprehensive try-catch with detailed logging

### Building the Mod
Use MSBuild via the provided script:
- `build.bat` - Builds with proper .NET 3.5 and game assembly references

## License and Credits

This mod enhances the Satellite Reign modding experience by providing robust data management capabilities. It's designed to work seamlessly with SatelliteReignModdingTools and other community modifications.

For support and updates, check the mod documentation and community resources.