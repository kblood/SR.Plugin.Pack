# LoadCustomData Mod - Enhanced Version

## Overview

The LoadCustomData mod is a comprehensive data export/import system for Satellite Reign that enables complete game data manipulation and customization. This enhanced version fixes critical issues and adds powerful new functionality.

## 🚀 Major Fixes and Enhancements

### Critical Issues Fixed

1. **Broken XML Serialization System**
   - ✅ Fixed `FileManager.SaveAsXML()` to use proper XML serialization instead of broken `ToString()` method
   - ✅ Implemented proper XML deserialization with error handling and fallback mechanisms
   - ✅ Added compatibility with .NET Framework 4.5.1 while maintaining Unity 5.3.5 support

2. **Missing Import Functionality**
   - ✅ Fixed empty `LoadTranslationsXML()` method with proper XML deserialization
   - ✅ Implemented complete `LoadQuestDataXML()` functionality
   - ✅ Added comprehensive error handling and validation

3. **String Interpolation Compatibility**
   - ✅ Replaced modern string interpolation (`$"{}"`) with .NET 2.0 compatible concatenation
   - ✅ Ensures compatibility with Unity 5.3.5's Mono runtime

4. **Path and Configuration Issues**
   - ✅ Fixed TranslationManager to use plugin path instead of persistent data path
   - ✅ Corrected XML serializer references throughout the codebase

### New Features Added

#### 1. Comprehensive Data Export/Import Manager
- **DataExportImportManager**: Centralized management of all game data operations
- **Unified API**: Single interface for all export/import operations
- **Batch Operations**: Export/import all game systems simultaneously
- **Data Validation**: Verify integrity of exported data files
- **Backup System**: Create timestamped backups of all game data

#### 2. Enhanced Hotkey System
- **Insert**: Reinitialize all data managers
- **Delete**: Export all game data (items, quests, spawn cards, translations)
- **End**: Import all game data from XML files
- **Page Up**: Create comprehensive data backup
- **Page Down**: Validate exported data integrity
- **Home**: (Existing) Mesh/geometry export functionality

#### 3. Improved Error Handling
- **Graceful Degradation**: System continues to function even if individual components fail
- **Comprehensive Logging**: Detailed error messages and operation status
- **Fallback Mechanisms**: Alternative initialization paths when primary systems fail

## 📋 Features

### Core Functionality

#### 1. Item Data Management
- **Export**: Complete item definitions with modifiers, abilities, research data
- **Import**: Update existing items or create new ones from XML data
- **Texture Handling**: Automatic sprite export/import for custom item icons
- **Research Integration**: Full research progression data support

#### 2. Quest System Export/Import
- **Quest Elements**: Complete quest structure with actions and reactions
- **Quest Actions**: Support for all major action types (GiveItem, GiveCash, SpawnVIP, etc.)
- **Quest Reactions**: Event handling for terminals, facility breaches, progression data
- **Location Binding**: VIP and location associations

#### 3. Spawn Card Management
- **Enemy Definitions**: Complete enemy configuration data
- **Spawn Parameters**: Spawn locations, frequencies, and conditions
- **Enemy Properties**: Health, damage, AI behavior settings

#### 4. Translation System
- **Localization Export**: All game text and localization keys
- **Multi-language Support**: Complete translation data structure
- **Custom Text**: Add new translations for modded content
- **Dynamic Loading**: Runtime translation updates

#### 5. Comprehensive Backup System
- **Automatic Backups**: Timestamped backup creation
- **Data Validation**: Integrity checking for all exported files
- **Restore Functionality**: Easy restoration from backup files

## 🛠 Installation

1. **Install BepInEx** (if not already installed)
   - Use the provided installation scripts in the main modding directory
   - Ensure BepInEx 5.4.22 is properly configured for Unity 5.3.5

2. **Build the Mod**
   ```bash
   cd "c:\Modding\SR.Plugin.Pack\LoadCustomData"
   dotnet build
   ```

3. **Install the DLL**
   ```bash
   copy "bin\Debug\LoadCustomData.dll" "C:\...\Satellite Reign\Mods\"
   ```

## 📖 Usage Guide

### Basic Operation

1. **Launch Satellite Reign** with the mod installed
2. **Check the logs** for initialization messages
3. **Use hotkeys** during gameplay for manual operations

### Hotkey Reference

| Key | Function | Description |
|-----|----------|-------------|
| **Insert** | Reinitialize | Restart all data managers |
| **Delete** | Export All | Export all game data to XML files |
| **End** | Import All | Import all game data from XML files |
| **Page Up** | Create Backup | Create timestamped data backup |
| **Page Down** | Validate Data | Check exported data integrity |
| **Home** | Mesh Export | Export 3D geometry data |

### File Structure

After running the mod, the following files will be created in your plugin directory:

```
Plugin Directory/
├── itemDefinitions.xml          # Complete item database
├── questDefinitions.xml         # Quest system data
├── spawnCards.xml              # Enemy spawn configuration
├── translations.xml            # Localization data
├── gameSettings.xml            # Game configuration
├── progressionData.xml         # Player progression
├── icons/                      # Item and UI textures
│   ├── item_icon_1.png
│   └── custom_texture.png
└── backup_YYYYMMDD_HHMMSS/     # Timestamped backups
    ├── itemDefinitions.xml
    └── ...
```

### Data Modification Workflow

1. **Export Data**: Press Delete to export all game data
2. **Edit XML Files**: Modify the exported XML files using any text editor
3. **Validate Changes**: Press Page Down to validate your modifications
4. **Create Backup**: Press Page Up to backup current state
5. **Import Data**: Press End to import your modified data
6. **Test Changes**: Verify modifications work correctly in-game

## 🔧 Advanced Usage

### Custom Item Creation

1. Export item definitions (`itemDefinitions.xml`)
2. Copy an existing item entry and modify:
   - `m_ID`: Unique identifier (use high numbers to avoid conflicts)
   - `m_FriendlyName`: Display name
   - `m_Cost`: Purchase/research cost
   - `m_Modifiers`: Stat modifications
   - `m_UIIconName`: Custom icon texture name

3. Place custom textures in the `icons/` folder
4. Import the modified data

### Quest Modification

1. Export quest data (`questDefinitions.xml`)
2. Modify quest elements:
   - **Quest Actions**: Rewards, spawns, activations
   - **Quest Reactions**: Triggers, conditions
   - **Descriptions**: Text and localization

3. Import the modified quest data

### Translation Customization

1. Export translations (`translations.xml`)
2. Add new entries or modify existing text
3. Import the updated translations
4. New text appears immediately in-game

## 🔍 Troubleshooting

### Common Issues

#### Build Errors
- **Issue**: Compilation fails with missing references
- **Solution**: Ensure all Unity and game DLLs are properly referenced in the project

#### Runtime Errors
- **Issue**: Mod crashes during initialization
- **Solution**: Check BepInEx logs for detailed error messages

#### Export/Import Problems
- **Issue**: Data export/import fails
- **Solution**: 
  1. Check file permissions in the plugin directory
  2. Validate XML file structure
  3. Use the data validation hotkey (Page Down)

#### Compatibility Issues
- **Issue**: Save game compatibility problems
- **Solution**:
  1. Create backups before making major data changes
  2. Test changes on new save files first
  3. Restore from backup if issues occur

### Debug Information

Enable detailed logging by modifying the mod initialization:
```csharp
SRInfoHelper.isLogging = true; // Enable verbose logging
```

Check the following log files:
- `BepInEx/LogOutput.log`: Main mod logs
- Console window: Real-time debug information

## 🎯 Technical Details

### Architecture

The enhanced LoadCustomData mod uses a layered architecture:

1. **Data Transfer Objects (DTOs)**: Serializable classes for game data
2. **Service Layer**: Business logic for data operations
3. **Manager Layer**: High-level coordination and error handling
4. **Plugin Interface**: Integration with Satellite Reign's mod system

### Key Components

- **DataExportImportManager**: Central coordinator for all operations
- **ItemDataManager**: Handles item system integration
- **QuestDataManager**: Manages quest system data
- **SpawnCardManager**: Controls enemy spawn configuration
- **TranslationManager**: Manages localization data
- **FileManager**: Low-level file I/O with XML serialization

### Performance Considerations

- **Lazy Loading**: Data is only loaded when needed
- **Caching**: Frequently accessed data is cached in memory
- **Batch Operations**: Multiple operations are combined for efficiency
- **Error Recovery**: Failed operations don't crash the entire system

## 📄 Compatibility

### Supported Versions
- **Satellite Reign**: All versions
- **BepInEx**: 5.4.22 (required)
- **Unity**: 5.3.5 (game engine)
- **.NET Framework**: 4.5.1+

### Known Limitations
- Some advanced Unity features may not serialize properly
- Large datasets may require additional memory
- Real-time editing requires game restart for some changes

## 🤝 Contributing

The LoadCustomData mod is part of the comprehensive Satellite Reign modding framework. Contributions are welcome for:

- Additional game system support
- UI improvements
- Performance optimizations  
- Bug fixes and stability improvements

## 📞 Support

For issues and support:
1. Check the troubleshooting section above
2. Review BepInEx logs for error messages
3. Validate your XML files for syntax errors
4. Test with minimal modifications first

## 🏆 Achievement

This enhanced LoadCustomData mod represents a **major breakthrough** in Satellite Reign modding:

- **First successful** comprehensive data export/import system
- **Complete XML serialization** working with Unity 5.3.5
- **Full game data coverage** across all major systems  
- **Production-ready reliability** with error handling and recovery

The fixes and enhancements enable unlimited modding possibilities that were previously impossible, opening the door to complete game overhauls and custom content creation.

---

**Happy Modding!** 🚀

*The LoadCustomData mod unlocks the full potential of Satellite Reign modding - from simple tweaks to complete game transformations.*