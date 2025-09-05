# LoadCustomData v2.1 - Technical Summary

## Recent Issues Resolved and Source Code References

### 1. WeaponType Enum Serialization Issue

**Problem:** XML serialization failed with error `'30' is not a valid value for WeaponType`

**Root Cause:** Satellite Reign has 31 weapons (indices 0-30), but not all indices have corresponding enum values defined.

**Solution Implemented:**
- Changed `SerializableWeaponData.m_WeaponType` from `WeaponType` to `int` (Source: `DTOs\WeaponData.cs:11`)
- Updated constructor: `m_WeaponType = (int)weaponType;` (Source: `DTOs\WeaponData.cs:29`)
- Maintained compatibility with import logic (Source: `WeaponDataManager.cs:98`)

**Files Modified:**
- `DTOs\WeaponData.cs` - Changed enum to int for XML compatibility
- `Services\WeaponDataManager.cs` - Import logic already used int conversion

### 2. Spawn Cards XML Loading Error

**Problem:** Error message "There is an error in XML document" when loading spawn cards

**Root Cause:** `LoadSpawnCardsFromFileAndUpdateSpawnManager()` was loading `enemyentries.xml` (wrong format) instead of `spawnCards.xml`

**Solution Implemented:**
```csharp
// BEFORE (Services\SpawnCardManager.cs:195)
string path = Path.Combine(Manager.GetPluginManager().PluginPath, SAVE_FILE_NAME); // Wrong - loads enemyentries.xml

// AFTER (Services\SpawnCardManager.cs:195) 
string path = Path.Combine(Manager.GetPluginManager().PluginPath, "spawnCards.xml"); // Correct
```

**Files Modified:**
- `Services\SpawnCardManager.cs:195` - Fixed file path for spawn cards loading

### 3. Magazine Size Investigation

**Finding:** Magazine sizes **are** properly captured and exported in weapon data.

**Source Code Evidence:**
- Field definition: `SerializableWeaponAttachmentAmmo.m_max_ammo` (Source: `DTOs\WeaponData.cs:103`)
- Constructor mapping: `m_max_ammo = ammo.m_max_ammo;` (Source: `DTOs\WeaponData.cs:150`)
- Import application: `ammo.m_max_ammo = m_max_ammo;` (Source: `DTOs\WeaponData.cs:189`)

**Live Data Examples from weapons.xml:**
- MiniGun: `<m_max_ammo>750</m_max_ammo>`
- SMG: `<m_max_ammo>466</m_max_ammo>` 
- Heavy Plasma: `<m_max_ammo>1000</m_max_ammo>`
- Pistols: `<m_max_ammo>1</m_max_ammo>`

### 4. Enhanced Error Handling

**Improvements Made:**

**WeaponData Constructor (Source: `DTOs\WeaponData.cs:27-92`):**
- Added null-safe property access: `weaponData?.m_Name ?? string.Empty`
- Wrapped constructor in comprehensive try-catch
- Fallback to minimal valid object on complete failure

**WeaponAttachmentAmmo Constructor (Source: `DTOs\WeaponData.cs:118-185`):**
- Added null check with safe defaults
- Individual property try-catch for robustness
- Detailed error logging with weapon type context

**WeaponDataManager Export (Source: `Services\WeaponDataManager.cs:47-60`):**
- Separate try-catch around XML serialization
- Enhanced logging: "Attempting to serialize X weapons to XML"
- Detailed exception information in logs

### 5. Dual Data System Integration

**Architecture Confirmed:**
1. **ItemManager.ItemData** - Research cost, availability, progression, pricing (Source: `Services\ItemDataManager.cs`)
2. **WeaponManager.WeaponData** - Actual weapon stats (damage, reload, range, ammo) (Source: `Services\WeaponDataManager.cs`)

**Integration Point:**
- `ItemData.m_WeaponType` links to `WeaponManager.m_WeaponData[(int)weaponType]`
- Both systems work together for complete weapon functionality

## Current System Capabilities

### Weapon Data Export/Import
- **31 weapon types** successfully serialized (indices 0-30)
- **Complete ammunition data** including magazine sizes, damage, reload times
- **Change detection** to avoid unnecessary updates
- **XML format compatibility** using int instead of enum for weapon types

### Spawn Cards Management  
- **Dual file format support**: enemyentries.xml + spawnCards.xml
- **Fallback behavior**: Auto-populates from SpawnManager if XML loading fails
- **Comprehensive enemy data**: Weapons, items, abilities, modifiers

### File Structure Generated
```
Mods/
├── weapons.xml           (31 weapons with complete ammo data)
├── spawnCards.xml        (Grouped enemy spawn configurations) 
├── enemyentries.xml      (Individual enemy definitions)
├── items.xml             (Item definitions with costs/research)
├── translations.xml      (ITEM_[ID]_NAME pattern translations)
└── quests.xml            (Quest definitions)
```

## Integration Points

**LoadCustomData Main Workflow (Source: `LoadCustomData.cs`):**
- **F9**: Import all data types
- **F10**: Export all data types  
- **F11**: Auto-load (same as game startup)
- **F4**: Diagnostics (weapon count, file status)
- **Insert**: Help display

**Auto-Load Timing (Source: `LoadCustomData.cs:160-166`):**
Uses proper game state detection:
```csharp
if (IsGameLoaded() && IsItemManagerReady())
{
    PerformAutoLoad(); // Load all data types
}
```

## Testing Verification

**Successful Export Log Pattern:**
```
WeaponDataManager: Successfully serialized [WeaponType]
WeaponDataManager: Attempting to serialize 31 weapons to XML  
WeaponDataManager: Successfully exported 31 weapon definitions to weapons.xml
```

**Successful Import Log Pattern:**
```
WeaponDataManager: Starting weapon data import
WeaponDataManager: Found 31 weapons in WeaponManager
WeaponDataManager: Weapon [Type] changes detected: [specific changes]
WeaponDataManager: Successfully updated [count] weapons
```

### 6. Game Progress Field Exclusion

**Enhancement:** Improved save game integrity by excluding runtime/progress data during item loading.

**Implementation (Source: `Services\ItemDataManager.cs:263-353`):**
```csharp
private void UpdateItemBasePropertiesSafely(ItemManager.ItemData existingItem, SerializableItemData importedItem)
{
    // Update ONLY base item definition properties (27 fields)
    // EXCLUDES all 19 game progress/runtime data fields
    
    // ===== EXCLUDED FIELDS =====
    // Research Progress: m_ResearchStarted, m_ResearchProgress, etc.
    // Player Ownership: m_PlayerHasPrototype, m_PlayerHasBlueprints, etc.
    // Inventory: m_Count
    // UI State: m_Expanded
    // Progression State: m_Progression, m_BlueprintRandomReleaseStage, etc.
}
```

**Benefits:**
- **Preserves active research** - Won't reset research in progress
- **Maintains player inventory** - Won't change item counts
- **Protects progression state** - Won't affect discovered items
- **Safe for multiplayer** - Won't desync player states

**Fields Loaded vs Excluded:**
- **Total SerializableItemData fields**: 46
- **Base item definition fields loaded**: 27 (59%)
- **Game progress fields excluded**: 19 (41%)

This technical summary documents the complete resolution of weapon data serialization, spawn cards loading issues, and save game integrity protection in LoadCustomData v2.1.