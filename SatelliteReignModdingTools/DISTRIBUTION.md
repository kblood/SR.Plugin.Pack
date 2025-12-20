# SatelliteReignModdingTools - Distribution & Setup

## Quick Start

1. **Copy files to any location:**
   - Copy entire `bin\Debug\` folder contents
   - Or copy to game Mods folder: `SatelliteReign\Mods\SatelliteReignModdingTools\`

2. **Run the launcher:**
   ```batch
   Double-click: LaunchTools.bat
   ```

3. **What happens:**
   - Automatically finds your Satellite Reign installation
   - Loads game DLLs from the game folder
   - Launches the modding tools UI
   - Works from any location

## Directory Structure

After copying, you should have:

```
YourFolder\SatelliteReignModdingTools\
├── LaunchTools.bat                      (Smart launcher - recommended)
├── LaunchToolsDebug.bat                 (Debug launcher - shows errors)
├── SatelliteReignModdingTools.exe       (The app)
├── SatelliteReignModdingTools.exe.config
│
├── itemDefinitions.xml
├── translations.xml
├── questDefinitions.xml
├── enemyentries.xml
├── spawnCards.xml
├── weapons.xml
│
└── icons/
    ├── icon1.png
    ├── icon2.png
    └── ...
```

## Important: File Locations

**The launcher script and executable MUST be in the same folder.**

✅ **Correct:**
```
D:\Mods\SatelliteReignModdingTools\
├── LaunchTools.bat
├── SatelliteReignModdingTools.exe
└── itemDefinitions.xml
```

❌ **Incorrect:**
```
D:\Mods\
├── LaunchTools.bat
└── Tools\
    └── SatelliteReignModdingTools.exe
```

## Running from Different Locations

### Option 1: From bin\Debug Folder (Development)
```batch
cd C:\Modding\SR\SR.Plugin.Pack\SatelliteReignModdingTools\bin\Debug
LaunchTools.bat
```

### Option 2: From Game Mods Folder (Recommended)
```batch
cd D:\SteamLibrary\steamapps\common\SatelliteReign\Mods\SatelliteReignModdingTools
LaunchTools.bat
```

### Option 3: From Custom Location
```batch
cd X:\MyModdingTools
LaunchTools.bat
```

All locations work the same way - launcher finds game DLLs automatically.

## How to Copy Files

### Using Command Prompt

```batch
REM Copy entire directory
xcopy "C:\Modding\SR\SR.Plugin.Pack\SatelliteReignModdingTools\bin\Debug\*" "D:\SteamLibrary\steamapps\common\SatelliteReign\Mods\SatelliteReignModdingTools\" /E /I /Y

REM Or copy to custom location
xcopy "C:\Modding\SR\SR.Plugin.Pack\SatelliteReignModdingTools\bin\Debug\*" "X:\MyTools\SatelliteReignModdingTools\" /E /I /Y
```

### Using File Explorer

1. Open: `C:\Modding\SR\SR.Plugin.Pack\SatelliteReignModdingTools\bin\Debug\`
2. Select all files and folders (Ctrl+A)
3. Copy (Ctrl+C)
4. Navigate to destination (Mods folder or custom location)
5. Paste (Ctrl+V)

### Using PowerShell

```powershell
Copy-Item -Path "C:\Modding\SR\SR.Plugin.Pack\SatelliteReignModdingTools\bin\Debug\*" `
          -Destination "D:\SteamLibrary\steamapps\common\SatelliteReign\Mods\SatelliteReignModdingTools\" `
          -Recurse -Force
```

## File Requirements

**Minimum files needed to run:**
- ✅ LaunchTools.bat (launcher)
- ✅ SatelliteReignModdingTools.exe (application)
- ✅ SatelliteReignModdingTools.exe.config (configuration)
- ✅ itemDefinitions.xml (item data)
- ✅ translations.xml (text data)

**Optional but recommended:**
- ✅ questDefinitions.xml (quest data)
- ✅ enemyentries.xml (enemy data)
- ✅ spawnCards.xml (spawn data)
- ✅ weapons.xml (weapon data)
- ✅ icons/ (icon files)

**Not needed:**
- ❌ Assembly-CSharp.dll (in game folder)
- ❌ UnityEngine.dll (in game folder)
- ❌ System.dll (Windows system)
- ❌ Any other .pdb files (debug symbols, optional)

## Troubleshooting

### "Cannot find SatelliteReignModdingTools.exe"

**Cause:** LaunchTools.bat and .exe are in different folders.

**Solution:** Make sure both are in the same directory.

### "Could not find Satellite Reign installation"

**Cause:** Game not in standard location.

**Solution:** Edit LaunchTools.bat and add your game path:

```batch
REM Add this before the :found label
if exist "X:\YourGamePath\SatelliteReign\SatelliteReignWindows_Data\Managed" (
    set "GAME_PATH=X:\YourGamePath\SatelliteReign"
    set "GAME_DLL_PATH=X:\YourGamePath\SatelliteReign\SatelliteReignWindows_Data\Managed"
    goto :found
)
```

### "itemDefinitions.xml not found"

**Cause:** Data files not in same folder as executable.

**Solution:** Verify all XML files are copied alongside the .exe.

### UI doesn't open / blank window

**Cause:** Missing or incorrect data files.

**Solution:**
1. Verify itemDefinitions.xml and translations.xml exist
2. Ensure game DLLs are being found (check console message)
3. Run from Mods folder or update LaunchTools.bat with your game path

## File Sizes

| Item | Size |
|------|------|
| LaunchTools.bat | 3 KB |
| SatelliteReignModdingTools.exe | 350 KB |
| itemDefinitions.xml | 282 KB |
| translations.xml | 4 MB |
| questDefinitions.xml | 86 KB |
| enemyentries.xml | 44 KB |
| spawnCards.xml | 135 KB |
| weapons.xml | 33 KB |
| icons/ | Variable |
| **Total (without icons)** | **~5 MB** |

## Next Steps After Setup

1. **Launch the tools:**
   ```batch
   LaunchTools.bat
   ```

2. **Select a browser:**
   - Item Browser (edit weapons, equipment, items)
   - Quest Editor (create/modify quests)
   - Translation Editor (manage game text)
   - Economy Browser (adjust weapon stats)
   - Enemy Browser (configure enemy spawns)
   - And more!

3. **Load game data:**
   - File → Load (or use in-game export first)

4. **Edit content:**
   - Make your changes in the UI

5. **Save changes:**
   - File → Save

## How Assembly Resolution Works

The modding tools use a custom assembly resolver to find game DLLs without including them in the distribution:

### Startup Sequence

1. **Application starts**
   - Before any other code runs, a custom AssemblyResolve event handler is registered
   - This intercepts all .NET assembly load requests

2. **Assembly resolution process** (for each required DLL)
   - Checks current directory first (where exe/launcher are)
   - Checks `SATELLITE_REIGN_DLL_PATH` environment variable
   - Checks hardcoded paths (D:\SteamLibrary, c:\Modding)
   - Reads Steam registry to find actual Steam installation
   - Checks multiple common drive letters (C-G)
   - Returns the first match found

3. **Assembly loading**
   - Once a DLL is located, it's loaded using `Assembly.LoadFrom()`
   - Application continues normally with loaded assemblies

### Why This Works

- **Reduces distribution size** - No need to include 8+ MB of game DLLs
- **Works anywhere** - Finds game DLLs regardless of installation path
- **Single executable** - All game DLL loading is built into the main .exe
- **Registry aware** - Automatically detects custom Steam library locations
- **Graceful fallback** - Works with standard locations if registry detection fails

## Summary

- ✅ Copy `bin\Debug\` contents to any location
- ✅ Run `LaunchTools.bat` or `LaunchToolsDebug.bat`
- ✅ No manual setup required
- ✅ Automatic game detection and DLL loading
- ✅ Works from Mods folder or anywhere else
- ✅ All data files included
- ✅ ~5-6 MB total size (no game DLLs)
- ✅ Works with any Steam installation path
- ✅ Error messages show what went wrong if anything fails
