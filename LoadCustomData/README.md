# LoadCustomData Mod for Satellite Reign

*Version 2.0 - Refactored Architecture*

The LoadCustomData mod is a clean, service-based data import/export system for Satellite Reign that allows modders and players to customize items, translations, enemy data, and other game content through XML files.

## Overview

This mod provides a comprehensive framework for modifying Satellite Reign's game data through XML files. It features automatic loading on game startup with proper game state detection, comprehensive change tracking, and enhanced translation system integration.

## Features

### Core Systems (Refactored Architecture)
- **LoadCustomDataRefactor**: Clean plugin coordinator with service-based architecture
- **ItemDataManager**: Enhanced item management with change detection and comprehensive field mapping
- **TranslationManager**: XML-based translation system with proper ITEM_[ID]_NAME pattern support
- **SpawnCardManager**: Fixed enemy spawn management (no more Unity coroutine issues)
- **Auto-Loading**: Intelligent loading with game state detection using `Manager.Get().GameInProgress`

### Supported Data Types
- **Item definitions**: Stats, costs, modifiers, abilities with comprehensive field mapping
- **Game translations**: XML format with automatic ITEM_[ID]_NAME pattern support  
- **Enemy spawn cards**: Enhanced enemy definitions and spawn management
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
| **F9** | Import Data | Import items and translations from XML files |
| **F10** | Export Data | Export items and translations to XML files |
| **F11** | Auto-Load | Auto-load items and translations (same as game start) |
| **F1** | Export Icons | Export all item icons to PNG files |
| **DELETE** | Export Basic Data | Export basic game data to text file |
| **HOME** | Export Advanced Data | Export advanced game information |
| **F5** | Debug Translations | Debug translation access (check logs) |
| **INSERT** | Show Help | Display help information and save help file |

## File Formats

### items.xml (Primary Format)
Current XML format for item definitions with comprehensive field support.

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

## Auto-Loading Features

The mod automatically performs the following when a game is loaded (detected via `Manager.Get().GameInProgress`):

1. **Translation Loading**: Loads `translations.xml` with proper ITEM_[ID]_NAME pattern support
2. **Item Definition Loading**: Loads `items.xml` with comprehensive change detection  
3. **Safe Updates**: Updates only items that have actually changed, preserving game state
4. **Change Tracking**: Logs detailed information about what changed (cost, progression, etc.)
5. **Smart Loading**: Prevents overwriting manual translation edits during auto-load

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
| `translations.xml` | Translation data with ITEM_[ID]_NAME pattern support |
| `spawnCards.xml` | Enemy spawn card definitions |
| `icons/*.png` | Item icon images |
| `basicGameData.txt` | Basic game information export |
| `advancedGameData.txt` | Advanced game information export |
| `LoadCustomData_Help.txt` | Generated help file |

## Current Status and Known Issues

### ✅ Working Features
- **Auto-loading**: Properly detects game state and loads data
- **Translation system**: XML loading with proper game integration
- **Change detection**: Only updates items that actually changed
- **Build system**: Works correctly with MSBuild via build.bat
- **File organization**: Clean service-based architecture

### ❓ Under Investigation
- **Item pricing**: m_Cost field updates successfully but changes don't appear in game UI
- **UI refresh**: May need to trigger ItemManager refresh events

### 🚧 Planned Improvements  
- **Separate import/export**: Currently both operations run together
- **Enhanced validation**: More comprehensive data validation
- **Command pattern**: Better hotkey organization

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
LoadCustomDataRefactor.cs (300 lines)
├── AutoLoadService - Game state detection and auto-loading
├── CommandService - Hotkey command routing
└── Services/
    ├── ItemDataManager - Enhanced item operations with change detection
    ├── TranslationManager - XML-based translation integration
    ├── SpawnCardManager - Fixed enemy spawn management
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