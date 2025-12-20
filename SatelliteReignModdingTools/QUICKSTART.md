# SatelliteReignModdingTools - Quick Start

## Running the Tools

### Method 1: Smart Launcher (Recommended)

```batch
Double-click: LaunchTools.bat
```

**What happens:**
1. Launcher finds your Satellite Reign installation automatically
2. Sets up environment variables for DLL loading
3. Launches the modding tools executable
4. The executable's custom assembly resolver finds game DLLs from:
   - Current directory
   - Environment variables set by launcher
   - Known Steam locations
   - Steam registry
   - Multiple drives (C-G)
5. Works regardless of game installation location

### Method 2: Debug Launcher (If something goes wrong)

```batch
Double-click: LaunchToolsDebug.bat
```

**Features:**
- Keeps console window open so you can see what's happening
- Shows file checks (itemDefinitions.xml, translations.xml, icons folder)
- Displays any error messages before closing
- Helpful for troubleshooting

### Method 3: Direct Execution (If launcher doesn't work)

```batch
Double-click: SatelliteReignModdingTools.exe
```

**Note:** The executable's custom assembly resolver will still search for game DLLs in standard locations, so this often works too!

## What's Included

```
bin\Debug\
├── SatelliteReignModdingTools.exe      (350 KB - the app)
├── LaunchTools.bat                      (2.8 KB - smart launcher)
├── SatelliteReignModdingTools.exe.config
│
├── itemDefinitions.xml                  (282 KB - item data)
├── translations.xml                     (4 MB - game text)
├── questDefinitions.xml                 (86 KB - quest data)
├── enemyentries.xml                     (44 KB - enemy data)
├── spawnCards.xml                       (135 KB - spawn data)
├── weapons.xml                          (33 KB - weapon stats)
│
└── icons/                               (PNG icon files)
```

## File Sizes

| Component | Size |
|-----------|------|
| Application | 350 KB |
| Data Files | ~4.5 MB |
| Icons | Variable |
| **Total** | **~5-6 MB** |

No game DLLs included - loaded from game installation at runtime.

## Troubleshooting

### "Could not find Satellite Reign installation"

**Solution:** Edit `LaunchTools.bat` and add your game path:

```batch
REM Add this line after "set GAME_DLL_PATH="
if exist "X:\YourPath\SatelliteReign\SatelliteReignWindows_Data\Managed" (
    set "GAME_PATH=X:\YourPath\SatelliteReign"
    set "GAME_DLL_PATH=X:\YourPath\SatelliteReign\SatelliteReignWindows_Data\Managed"
    goto :found
)
```

### "ItemBrowser: Type initializer threw an exception"

**Cause:** Missing data files or game DLLs

**Solutions:**
1. Use `LaunchTools.bat` instead of direct .exe
2. Verify all XML files are in the same folder as .exe
3. Verify game is properly installed

### "Missing icons" warning

**Info:** Optional - app works fine without icons folder

## Distribution

To share with others:

1. **Minimal package:**
   - Copy entire `bin\Debug\` folder
   - User just runs `LaunchTools.bat`

2. **With installer:**
   - Create `.msi` or `.zip` with the `bin\Debug\` contents
   - Include `LaunchTools.bat` for easiest setup

3. **Single folder:**
   - No additional setup required
   - Works immediately after extraction

## Rebuilding

To rebuild the application:

```batch
build.bat
```

The build script will:
1. Clean old artifacts
2. Compile the source code
3. Copy all data files automatically
4. Remove game DLLs from output
5. Report success

Build output goes to `bin\Debug\`

## Features

The modding tools provide access to:
- **Item Browser** - Edit weapons, equipment, items
- **Quest Editor** - Create and modify quests
- **Enemy Browser** - Configure enemy spawns
- **Economy Browser** - Adjust weapon statistics
- **Translation Editor** - Manage game text
- **Skills Browser** - Edit character abilities
- **Faction Browser** - Modify faction relationships

See `README.md` for detailed documentation.

## Getting Help

1. Check `BUILD_IMPROVEMENTS.md` for build system details
2. Check `README.md` for feature documentation
3. Review game data XML files for structure
4. Check game console for error messages

## Summary

- ✅ Run with `LaunchTools.bat`
- ✅ 5-6 MB lightweight distribution
- ✅ Automatic game detection
- ✅ No manual DLL management
- ✅ Professional, user-friendly experience
