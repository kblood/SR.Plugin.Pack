# SatelliteReignModdingTools - Build System Improvements

## Overview

The build system has been completely revamped to create a lightweight, self-contained distribution that reuses DLLs from the Satellite Reign game installation - just like the LoadCustomData mod does.

## Changes Made

### Build Files Consolidated
- ✅ Deleted redundant `build_fixed.bat`
- ✅ Kept single `build.bat` with all improvements
- ✅ Added new `LaunchTools.bat` for smart game detection

### Build Process Improvements

**Before:**
- Manual Roslyn compiler invocation
- No automatic dependency management
- Large distribution with game DLLs (~13 MB)
- No launcher script

**After:**
- Uses MSBuild (more reliable)
- Automatic game DLL detection at runtime
- Lightweight distribution (~5-6 MB, no game DLLs)
- Smart launcher finds game installation automatically
- Cleans old artifacts before building

## How It Works

### Distribution Model

```
SatelliteReignModdingTools/
├── SatelliteReignModdingTools.exe (350 KB - your app)
├── LaunchTools.bat (smart launcher - finds game DLLs)
├── LaunchToolsDebug.bat (debug launcher - shows errors)
├── SatelliteReignModdingTools.exe.config (app config)
├── itemDefinitions.xml (game data)
├── translations.xml (game text)
├── questDefinitions.xml (game quests)
├── enemyentries.xml (enemy data)
├── spawnCards.xml (spawn data)
├── weapons.xml (weapon stats)
└── icons/ (PNG icon files)

Does NOT include:
✗ Assembly-CSharp.dll (4.8 MB) - loaded from game
✗ Assembly-CSharp-firstpass.dll (729 KB) - loaded from game
✗ UnityEngine.dll (950 KB) - loaded from game
✗ UnityEngine.UI.dll (214 KB) - loaded from game
✗ UnityEngine.Networking.dll (226 KB) - loaded from game
✗ System.dll, System.Core.dll - loaded from Windows
```

### Runtime DLL Loading

When you run the application:

1. **LaunchTools.bat:**
   - Searches for Satellite Reign installation
   - Sets `SATELLITE_REIGN_DLL_PATH` environment variable pointing to game managed DLLs folder
   - Launches SatelliteReignModdingTools.exe

2. **SatelliteReignModdingTools.exe startup (Program.cs):**
   - Registers a custom AssemblyResolve handler before loading any other assemblies
   - This handler intercepts all assembly load requests

3. **Custom Assembly Resolver:**
   - Searches for required DLLs in this priority order:
     1. Current directory (where exe/launcher are located)
     2. `SATELLITE_REIGN_DLL_PATH` environment variable (set by LaunchTools.bat)
     3. Hardcoded game paths:
        - `D:\SteamLibrary\steamapps\common\SatelliteReign\SatelliteReignWindows_Data\Managed`
        - `c:\Modding\SatelliteReign\SatelliteReignWindows_Data\Managed`
     4. Steam registry path (reads from Windows registry)
     5. Multiple drive letters (C, D, E, F, G, H)
   - Returns the loaded assembly once found
   - Graceful fallback if DLL not found anywhere

4. **Global Exception Handlers:**
   - Catches unhandled exceptions in all threads
   - Displays error messages so you know what went wrong
   - Supports both regular and UI thread exceptions

## Building

### Quick Build

```batch
cd C:\Modding\SR\SR.Plugin.Pack\SatelliteReignModdingTools
build.bat
```

### Build Process

The `build.bat` script:

1. **Cleans old artifacts**
   - Removes previous .exe
   - Removes all game DLLs from output
   - Keeps data files

2. **Compiles code**
   - Uses MSBuild with Visual Studio 2022
   - Creates lightweight executable

3. **Deploys data**
   - Copies XML data files
   - Copies icons folder
   - Copies launcher script

4. **Verifies output**
   - Checks all files are in place
   - Reports success/failure

### Output

```
✓ Build successful!
✓ Output: bin\Debug\
✓ Size: ~5-6 MB (no game DLLs)
✓ Ready to distribute
```

## Running the Tools

### Option 1: Smart Launcher (Recommended)

```batch
LaunchTools.bat
```

The launcher:
- Finds your Satellite Reign installation automatically
- Sets up environment variables
- Launches the modding tools
- Works with any game location (Steam, custom, etc.)

### Option 2: Direct Execution

```batch
SatelliteReignModdingTools.exe
```

Requires game to be in standard location, but works immediately.

## Distribution

### Package Contents

Copy the entire `bin\Debug\` folder containing:
- SatelliteReignModdingTools.exe
- LaunchTools.bat
- All XML data files
- icons/ folder

### Size Comparison

| Component | Before | After | Savings |
|-----------|--------|-------|---------|
| Application | 350 KB | 350 KB | - |
| Game DLLs | 8.5 MB | 0 MB | 8.5 MB saved |
| Data Files | 4.5 MB | 4.5 MB | - |
| **Total** | **13 MB** | **5 MB** | **61% smaller** |

### Installation for Users

1. Extract `bin\Debug\` contents to any folder
2. Double-click `LaunchTools.bat`
3. Application starts automatically (no manual path configuration)

## Technical Details

### Environment Variables

The launcher sets:
- `SATELLITE_REIGN_PATH` - Root game directory
- `SATELLITE_REIGN_DLL_PATH` - Managed DLLs folder

### Game Location Detection

Searches in this order:
1. D:\SteamLibrary\steamapps\common\SatelliteReign (common custom install)
2. c:\Modding\SatelliteReign (development setup)
3. Steam registry location (official Steam installations)
4. Multiple drives C-H (distributed Steam libraries)

### DLL Loading at Runtime

The .exe uses standard .NET assembly loading:
1. Probes local directory (bin\Debug\)
2. Uses custom paths from environment variables
3. Falls back to system directories
4. Raises exception if not found

## Troubleshooting

### Error: "Could not find Satellite Reign"

**Solution:** LaunchTools.bat couldn't find the game.

```batch
REM Run from game directory:
cd "D:\SteamLibrary\steamapps\common\SatelliteReign"
"path\to\LaunchTools.bat"
```

Or edit `LaunchTools.bat` to add your game path.

### Error: "Assembly not found"

**Solution:** Game DLLs not found at runtime.

```batch
REM Ensure LaunchTools.bat is used (not direct .exe execution)
REM Or verify game installation is complete
```

### Missing Icons

The `icons/` folder is optional. Application works without it.

## Advantages Over Old System

| Feature | Old | New |
|---------|-----|-----|
| Size | 13 MB | 5 MB |
| DLL Management | Manual copying | Automatic |
| Game Path Detection | None | Automatic |
| Distribution | Complex | Simple |
| Development | Easy | Easy |
| User Experience | Manual setup | One-click run |

## Future Improvements

Possible enhancements:
1. Create installer (.msi) - bundles everything
2. Single executable using IL Merge (more complex)
3. .NET Core version (requires major refactoring)
4. Web UI version (advanced)

## Summary

The new build system provides:
- ✅ 61% smaller distribution size
- ✅ Automatic game detection
- ✅ One-click launcher
- ✅ Cleaner code organization
- ✅ Easy distribution
- ✅ Professional user experience
- ✅ Development-friendly build process
