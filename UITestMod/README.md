# UITestMod - Satellite Reign UI Testing Tool

## Overview
This mod tests different UI approaches to understand what works in Satellite Reign's modding environment. The ItemEditorMod UI isn't working, so we need to systematically test what UI methods are available.

## Usage
1. Copy `UITestMod.dll` to your BepInEx plugins folder
2. Launch Satellite Reign
3. Press **F9** to cycle through different UI tests (0-4)
4. Press **F10** to close the current test UI

## Tests Included

### Test 0: Game Message Popup ✅
**Purpose**: Verify the game's built-in message system works  
**Method**: Uses `Manager.GetUIManager().ShowMessagePopup()`  
**Expected**: Should show a yellow text message at top of screen  
**What we learn**: Baseline - this should always work

### Test 1: Clone InputBoxUi
**Purpose**: Test if we can clone existing game UI elements  
**Method**: `Instantiate(uiManager.m_InputBoxUi.gameObject)`  
**Expected**: Should show a cloned input dialog box  
**What we learn**: 
- Can we reuse game UI elements?
- Do cloned UIs inherit proper canvas/hierarchy?
- Are event handlers/scripts properly copied?

### Test 2: Raw Canvas Creation
**Purpose**: Test creating a completely new Canvas from scratch  
**Method**: Create GameObject → Add Canvas → Add UI elements  
**Expected**: Should show a centered dark panel with text  
**What we learn**:
- Can we create standalone Canvas?
- Does Canvas render properly?
- Is input (GraphicRaycaster) working?
- Render mode and sorting order requirements

### Test 3: Find Existing Canvases
**Purpose**: Discover what UI canvases already exist in the game  
**Method**: `FindObjectsOfType<Canvas>()`  
**Expected**: Lists all canvases to console with details  
**What we learn**:
- How many canvases does the game have?
- What render modes are used?
- What sorting orders are in use?
- Which canvases are active/visible?

### Test 4: Simple Window (InputBox Parent)
**Purpose**: Test creating UI as child of existing game UI  
**Method**: Create panel attached to `InputBoxUi.transform.parent`  
**Expected**: Should show a window using existing canvas hierarchy  
**What we learn**:
- Is it better to attach to existing hierarchy?
- Does this avoid canvas rendering issues?
- Do we get proper layering automatically?

## Expected Results

### If Test 1 works:
✅ **Best approach**: Clone and modify existing UI elements  
✅ Guarantees proper canvas setup, fonts, styles  
✅ Event system is already wired up  

### If Test 2 works:
✅ **Most flexible**: Full control over UI structure  
✅ Can create custom layouts without game dependencies  
⚠️ Need to handle canvas setup, fonts, input manually  

### If Test 4 works:
✅ **Hybrid approach**: New UI on existing canvas  
✅ Simplified setup - no canvas configuration needed  
✅ Proper layering and input handling inherited  

### If none work:
❌ UI system may be locked down  
❌ May need to use game's built-in dialogs only  
❌ Consider alternative approaches (console commands, file-based editing)  

## Debugging

Check the game's log file (usually in `BepInEx/LogOutput.log`) for detailed output:
- `Test X: SUCCESS` - Test passed
- `Test X: FAIL` - Test failed with reason
- `Test X: EXCEPTION` - Error occurred with stack trace

Each test logs additional details about:
- GameObject hierarchy
- Canvas settings (renderMode, sortingOrder)
- Active/enabled states
- Transform positions

## Next Steps

Based on test results:
1. **If cloning works**: Modify ItemEditorMod to use cloned InputBoxUi properly
2. **If raw canvas works**: Simplify ItemEditorMod canvas creation
3. **If parent attachment works**: Restructure ItemEditorMod to attach to game UI
4. **If nothing works**: Consider simpler UI approaches (just use ShowMessagePopup, or external tools)

## Technical Notes

- Uses Unity 5.x UI system (Canvas, RectTransform, etc.)
- Targets .NET 3.5 (Satellite Reign's Unity version)
- Minimal dependencies: Only UnityEngine, UnityEngine.UI, Assembly-CSharp
- No BepInEx-specific features - pure ISrPlugin implementation

## Files
- `UITestMod.cs` - Main plugin code with all 5 tests
- `UITestMod.csproj` - Build configuration
- `bin/Debug/UITestMod.dll` - Compiled plugin (copy this to BepInEx/plugins)
