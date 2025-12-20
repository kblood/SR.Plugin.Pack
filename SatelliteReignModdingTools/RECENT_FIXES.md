# SatelliteReignModdingTools - Recent Fixes & Updates

## Fix 1: WeaponType Enum Serialization (LoadCustomData Compatibility)

**Problem:** ItemBrowser crashed when loading itemDefinitions.xml exported by LoadCustomData because `m_WeaponType` was exported as a string (e.g., `"B_pistol"`) but the DTO expected a `WeaponType` enum.

**Solution:** Modified `SerializableItemData.cs` to:
- Serialize as `m_WeaponTypeString` (string field in XML)
- Provide `m_WeaponType` property that converts string → enum
- Supports both string enum names (LoadCustomData format) and int values (legacy)
- Graceful fallback to (WeaponType)0 if conversion fails

**Result:** ✅ ItemBrowser now loads LoadCustomData exports without errors

---

## Fix 2: Missing Assembly Resolution (DLL Loading)

**Problem:** Application crashed with `FileNotFoundException: Assembly-CSharp not found` because game DLLs weren't being found at runtime.

**Solution:** Added custom `AssemblyResolve` event handler in `Program.cs` that:
1. Intercepts all .NET assembly load requests
2. Searches for DLLs in this order:
   - Current directory
   - `SATELLITE_REIGN_DLL_PATH` environment variable (set by launcher)
   - Hardcoded Steam paths (D:\SteamLibrary, c:\Modding)
   - Steam registry (automatic detection of custom Steam locations)
   - Multiple drives (C-G)
3. Uses `Assembly.LoadFrom()` to load found DLLs
4. Gracefully returns null if DLL not found

**Result:** ✅ Application can now load from Mods folder or any other location without copying game DLLs

---

## Fix 3: Silent Crashes (Error Visibility)

**Problem:** Application would start and immediately close with no error message, making debugging impossible.

**Solution:** Added comprehensive exception handling:

**In Program.cs:**
- Global `AppDomain.UnhandledException` handler - catches all unhandled exceptions
- `Application.ThreadException` handler - catches UI thread exceptions
- Both display MessageBox with exception details and stack trace

**In ItemBrowser.cs:**
- Wrapped constructor in try-catch
- Displays initialization errors before rethrowing

**LaunchToolsDebug.bat:**
- Runs application without "start" command (keeps console open)
- Shows file existence checks
- Displays any errors before closing

**Result:** ✅ Now you get helpful error messages instead of silent crashes

---

## Fix 4: Build System Enhancements

**Problem:** Manual DLL copying was error-prone; old build scripts were duplicated.

**Solution:** Updated `build.bat` to:
1. Clean old artifacts (removes DLLs, keeps data files)
2. Compile fresh with MSBuild
3. Automatically copy data files (XML)
4. Automatically copy icons folder
5. Automatically copy launcher scripts
6. No manual DLL copying needed

**Result:** ✅ Clean, automated build process that works every time

---

## How Everything Works Together

```
User double-clicks: LaunchTools.bat
    ↓
Batch script finds Satellite Reign installation
    ↓
Sets SATELLITE_REIGN_DLL_PATH environment variable
    ↓
Launches: SatelliteReignModdingTools.exe
    ↓
Program.cs Main() runs FIRST, before any other code
    ↓
Custom AssemblyResolve handler registered
    ↓
Application tries to load Assembly-CSharp.dll
    ↓
Custom resolver intercepts the request
    ↓
Searches for Assembly-CSharp.dll using multiple strategies
    ↓
Finds it in game folder (from environment variable or registry)
    ↓
Loads it with Assembly.LoadFrom()
    ↓
All other assemblies loaded the same way
    ↓
ItemBrowser initializes with proper string→enum conversion
    ↓
UI opens successfully!
```

---

## Testing Checklist

- ✅ App loads from `bin\Debug` folder (development)
- ✅ App loads from game `Mods` folder
- ✅ App loads from custom location
- ✅ ItemBrowser displays items from LoadCustomData exports
- ✅ WeaponType values convert correctly
- ✅ Error messages appear if something goes wrong
- ✅ Debug launcher shows file checks
- ✅ Works with custom Steam installation paths

---

## Files Modified

1. **ItemData.cs** - Added WeaponType string-to-enum conversion
2. **ItemBrowser.cs** - Added try-catch in constructor
3. **Program.cs** - Added AssemblyResolve handler and global exception handlers
4. **LaunchTools.bat** - Works from any location
5. **LaunchToolsDebug.bat** - New debug launcher with error visibility
6. **build.bat** - Automated build process with DLL cleanup
7. **BUILD_IMPROVEMENTS.md** - Updated with assembly resolver details
8. **QUICKSTART.md** - Updated with debug launcher info
9. **DISTRIBUTION.md** - Added assembly resolver explanation

---

## Key Improvements

| Aspect | Before | After |
|--------|--------|-------|
| DLL Distribution | Included (13 MB) | Not included (5 MB) |
| Error Messages | Silent crashes | Clear error dialogs |
| Game Path Detection | Manual | Automatic (registry + env) |
| LoadCustomData Support | Broken | ✅ Works |
| Build Process | Manual | Automated |
| Debug Capability | Impossible | Via debug launcher |
| Works from Mods folder | ❌ No | ✅ Yes |
| Works from any location | ❌ No | ✅ Yes |

---

## Fix 5: New Item Creation - Missing m_FriendlyName Field

**Problem:** When creating a new item in the modding tools (e.g., duplicating an existing item to create ID 147), the game would crash with `NullReferenceException: Object reference not set to an instance of an object` in `ItemManager.SaveItemData` whenever the game tried to save.

**Root Cause:** The `CopyItemProperties` function in `SREditor.cs` was copying all item properties EXCEPT `m_FriendlyName`. This caused new items to be created with a null friendly name, which breaks the game's save logic that expects all items to have a valid name.

**Solution:** Modified `SREditor.cs` line 484 to:
```csharp
target.m_FriendlyName = source.m_FriendlyName ?? "New Item";
```

This ensures:
- The friendly name is copied from the source item
- If source is null, defaults to "New Item"
- All new items have a valid name when created

**Additional Fix:** The initial fix copied `m_FriendlyName` but still had issues because `m_PlayerHasPrototype`, `m_PlayerHasBlueprints`, and other player state fields were being overridden with hardcoded default values instead of being copied from the source item. This caused new items to have incompatible player state that crashed the game's save system.

**Enhanced Solution:** Modified `CopyItemProperties` to copy **ALL** fields from the source item instead of overriding them:
- `m_PlayerHasPrototype`
- `m_PlayerHasBlueprints`
- `m_Count`
- `m_AvailableToPlayer`
- `m_AvailableFor_ALPHA_BETA_EARLYACCESS`
- `m_PlayerStartsWithBlueprints`
- `m_PlayerStartsWithPrototype`
- `m_PlayerCanResearchFromStart`
- `m_ResearchStarted`
- `m_ItemHasBeenLocated`
- `m_ResearchProgress`
- `m_ResearchTimeToDate`
- `m_ResearchCostToDate`
- `m_TotalResearchTime`
- `m_InHouseResearchersResearching`
- `m_ExternalResearchersResearching`

**Result:** ✅ New items now inherit all player state and game state from source item, allowing the game to save without crashes

---

## Fix 6: Item Deletion Not Actually Removing Items from Collections

**Problem:** When deleting items in the modding tools and saving, the deleted items would still appear in the saved XML file. The file size would increase and item counts wouldn't match. Investigation showed that deleted items weren't actually being removed from the collections.

**Root Cause:** The delete function had two issues:
1. Used `itemDTOs.Remove(activeItemData)` which depends on reference equality - if the reference didn't match exactly, the item wouldn't be found
2. Only removed from `itemDTOs` collection but NOT from `_allItems` filtered collection, leaving a stale copy

**Solution:** Modified `DeleteItemButton_Click` to:
1. Find the item by ID instead of reference: `itemDTOs.FirstOrDefault(i => i.m_ID == itemID)`
2. Remove from BOTH collections (`itemDTOs` and `_allItems`) to keep them in sync
3. Added null checks for translations before removing
4. Added logging to confirm removal

**Technical Details:**
- Changed from: `itemDTOs.Remove(activeItemData)` (reference-based removal)
- Changed to: `var itemToRemove = itemDTOs.FirstOrDefault(i => i.m_ID == itemID); if (itemToRemove != null) itemDTOs.Remove(itemToRemove);` (ID-based removal)
- Applied same logic to `_allItems` collection to keep UI and save data consistent

**Result:** ✅ Deleted items are now properly removed from both collections and don't appear in saved files

---

## Next Steps for Users

1. **Copy files** from `bin\Debug` to desired location
2. **Run `LaunchTools.bat`** - that's it!
3. **If issues occur** - run `LaunchToolsDebug.bat` to see error messages
4. **No additional setup** needed - everything is automatic

---

## Technical Notes

- Assembly resolver handles both compile-time references and runtime loads
- Uses `Assembly.LoadFrom()` which respects .NET version requirements
- Environment variable override allows testing with different game paths
- Registry detection works with Steam's "SteamPath" HKCU key
- Multiple drives support allows distributed Steam libraries
- Error handlers prevent exceptions from closing the app unexpectedly
