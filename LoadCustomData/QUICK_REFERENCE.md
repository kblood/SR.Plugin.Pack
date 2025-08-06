# LoadCustomData Mod - Quick Reference

## 🎮 In-Game Hotkeys

| Key | Function | Output File |
|-----|----------|-------------|
| **Delete** | Export all data (comprehensive) | translations.xml, itemDefinitions.xml, questDefinitions.xml, icons/ |
| **Insert** | Resume + comprehensive export | Same as Delete (for maps/levels) |
| **F2** | Export translations only | translations.xml |
| **F3** | Export sprites only | icons/ folder with PNG files |
| **F4** | Export quests only | questDefinitions.xml |

## 📁 Output Files

### questDefinitions.xml
- **Contains**: All mission and quest data
- **Format**: XML with lowercase boolean values (`true`/`false`)
- **Data**: Quest IDs, titles, status, descriptions, sub-quests, districts, locations
- **Size**: Typically 40-50KB for full game data

### translations.xml  
- **Contains**: All in-game text and localization
- **Format**: XML with CDATA sections for text content
- **Data**: UI text, dialogue, item names, quest descriptions

### itemDefinitions.xml
- **Contains**: Weapons, equipment, and item specifications
- **Format**: XML serialization of item data structures
- **Data**: Item stats, modifiers, categories, descriptions

### icons/ folder
- **Contains**: Extracted sprite textures
- **Format**: PNG files
- **Naming**: Uses original texture names from game assets

## 🛠 Build Process

### Correct Project File
```bash
cd "c:\Modding\SR.Plugin.Pack\LoadCustomData"
msbuild LoadCustomDataClean.csproj
```

**Important**: Use `LoadCustomDataClean.csproj`, not `LoadCustomData.csproj`
- **Clean project**: Only includes working plugin class and dependencies
- **Avoids conflicts**: No multiple ISrPlugin implementations
- **Result**: 32KB DLL (vs 136KB wrong build)

### Installation
```bash
copy "bin\Debug\LoadCustomData.dll" "C:\...\Satellite Reign\Mods\"
```

## 🖥️ SatelliteReignModdingTools

### Quest Browser
1. **Export quests**: Press F4 in-game
2. **Open tools**: Run SatelliteReignModdingTools.exe  
3. **Browse data**: Click "Quest Browser" button
4. **Features**: Search, filter, detailed quest information

### Troubleshooting
- **"Der er en fejl i XML-dokumentet (7, 6)"**: Re-export with updated LoadCustomData mod
- **Empty quest list**: Export after loading a save game
- **Mod not working**: Ensure using LoadCustomDataClean.csproj build

## 🔧 Technical Details

### .NET Framework Compatibility
- **Game Runtime**: .NET Framework 4.5.1 (Unity 5.3.5)
- **Tools Runtime**: .NET Framework 4.8
- **Compatibility**: Uses `ReferenceEquals()` instead of `!=` for reflection

### XML Format Requirements
- **Boolean values**: Lowercase (`true`/`false`)
- **Text content**: CDATA sections for special characters
- **Encoding**: UTF-8
- **Structure**: Compatible with System.Xml.Serialization

### Recent Fixes Applied
- ✅ Boolean serialization compatibility
- ✅ Automatic quest tree initialization  
- ✅ Reflection compatibility for .NET 4.5.1
- ✅ Project structure cleanup
- ✅ Quest Browser XML parsing

## 📊 File Sizes (Reference)
- **LoadCustomData.dll**: ~32KB (correct build)
- **questDefinitions.xml**: ~40-50KB (full game)
- **translations.xml**: ~15-25KB
- **itemDefinitions.xml**: ~10-20KB
- **Total export**: ~100-150KB + sprite files

## 🚨 Common Issues

### Quest Export Fails
- **Cause**: Quest tree not initialized
- **Solution**: Automatic initialization implemented (no user action needed)

### XML Parsing Error  
- **Cause**: Boolean format mismatch (True vs true)
- **Solution**: Use updated LoadCustomData mod with boolean fix

### Mod Hotkeys Not Working
- **Cause**: Wrong project build with multiple plugin classes
- **Solution**: Build with LoadCustomDataClean.csproj

### Quest Browser Shows Empty List
- **Cause**: No quest data or wrong file location
- **Solution**: Export quests (F4) after loading save game