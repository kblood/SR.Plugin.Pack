# Satellite Reign Quest Editor & Translation System Design

## Overview
The SatelliteReignModdingTools will be redesigned to focus on **Quest Template Editing** and **Translation Management** as the primary modding capabilities. This system allows for complete quest creation, modification, and localization support.

## Main Menu Redesign

### Current (To Be Removed)
- Mission Browser (non-editable, read-only)
- Weapon Browser (non-editable, read-only)

### New Design
1. **Quest Browser** - Edit and create quest templates
2. **Translations Browser** - Manage game text and localization  
3. **Item Browser** - Keep existing item functionality
4. **Settings** - Configuration and export/import

## Quest Browser Redesign

### Purpose
Transform from read-only browser to full **Quest Template Editor** supporting:
- Editing existing quest templates
- Creating new quest templates  
- Translation key integration
- Item rewards linking
- Field validation with dropdowns

### Quest Data Structure Analysis

Based on the exported quest data, here are the field types and their editing requirements:

#### Core Quest Fields

| Field | Type | Editable | Input Type | Notes |
|-------|------|----------|------------|-------|
| ID | int | ❌ | Read-only | Auto-generated for new quests |
| Title | string | ✅ | Text + Translation Key | Display text (auto-populated from translation) |
| TitleKey | string | ✅ | Translation Selector | Links to translations.xml |
| Hidden | bool | ✅ | Checkbox | Quest visibility |
| ShowDebrief | bool | ✅ | Checkbox | Shows completion debrief |
| State | bool | ❌ | Read-only | Runtime completion state |

#### Location & District Fields

| Field | Type | Editable | Input Type | Notes |
|-------|------|----------|------------|-------|
| District | enum | ✅ | Dropdown | NONE, RedLight, Industrial, Grid, CBD, BossFight |
| WakeOnLocation | int | ✅ | Number/Dropdown | Location ID (-1 = disabled) |
| WakeOnLocationList | int[] | ✅ | Multi-select | Multiple location IDs |

#### Advanced Fields

| Field | Type | Editable | Input Type | Notes |
|-------|------|----------|------------|-------|
| VIP | bool | ✅ | Checkbox | Quest involves VIP extraction |
| Descriptions | Description[] | ✅ | Custom Editor | Quest descriptions with translation keys |
| SubQuests | int[] | ✅ | Multi-select | Links to other quest IDs |
| Rewards | ItemReward[] | ✅ | Item Selector | **NEW**: Link to item definitions |

#### Description Structure
```xml
<Description>
  <LocTitle>QUEST_DESCRIPTION_KEY_01</LocTitle>
  <Translation>Translated description text</Translation>
  <IsNew>true/false</IsNew>
  <HasBeenSeen>false</HasBeenSeen>
</Description>
```

### Quest Editor UI Components

#### 1. Quest List Panel (Left Side)
- **Filter by District**: Dropdown filter
- **Search**: Text search by title/ID  
- **New Quest**: Button to create new quest template
- **Template vs Runtime**: Toggle between template and runtime data sources

#### 2. Quest Details Panel (Right Side)

##### Basic Information Tab
- **ID**: Read-only field (auto-generated for new)
- **Title Key**: Dropdown selector from translations
- **Title**: Auto-populated from selected translation, read-only
- **District**: Dropdown (NONE, RedLight, Industrial, Grid, CBD, BossFight)
- **Hidden**: Checkbox
- **Show Debrief**: Checkbox  
- **Has VIP**: Checkbox

##### Location Tab  
- **Wake on Location**: Number input with location ID
- **Wake on Location List**: Multi-select dropdown for multiple locations
- **Location Validation**: Warn if location ID doesn't exist

##### Descriptions Tab
- **Add Description**: Button to add new description
- **Description List**: 
  - Translation Key dropdown
  - Preview of translated text
  - Delete button
  - Reorder buttons

##### Sub-Quests Tab
- **Add Sub-Quest**: Multi-select from existing quests
- **Sub-Quest List**: Show linked quests with titles
- **Dependency Validation**: Warn about circular dependencies

##### Rewards Tab (**NEW**)
- **Add Item Reward**: Select from item definitions
- **Reward List**: 
  - Item name and icon
  - Quantity selector
  - Delete button

### Field Validation & Dropdowns

#### District Dropdown Values
```csharp
public enum QuestDistrict
{
    NONE,
    RedLight,
    Industrial, 
    Grid,
    CBD,
    BossFight
}
```

#### Translation Key Integration
- **Translation Selector**: Dropdown populated from translations.xml
- **Live Preview**: Show translated text when key is selected
- **Missing Translation Warning**: Highlight keys not found in translations
- **Quick Add**: Button to create new translation entry

#### Location ID Validation
- **Valid Range**: 1-200+ (based on game data)
- **Location Lookup**: Show location names if available
- **Invalid ID Warning**: Red highlight for non-existent locations

## Translations Browser (**NEW**)

### Purpose
Complete translation management system for:
- Adding new translation keys
- Editing existing translations
- Managing multi-language support
- Integration with Quest Browser

### Translation Structure
Based on translations.xml:
```xml
<Translation>
  <Key>Q_GEN_TITLE_001</Key>
  <Element>
    <m_token>Q_GEN_TITLE_001</m_token>
    <m_Translations>
      <string><![CDATA[Mission Control]]></string>
      <!-- Additional language entries -->
    </m_Translations>
  </Element>
</Translation>
```

### Translation Browser UI

#### 1. Translation List Panel
- **Search**: Filter by key or text content
- **Category Filter**: Group by prefix (Q_GEN_, Q_RL_, STORY_, etc.)
- **New Translation**: Create new translation entry
- **Import/Export**: Translation file management

#### 2. Translation Editor Panel  
- **Translation Key**: Unique identifier (validated)
- **Primary Language**: Default game text (English)
- **Additional Languages**: Multi-language support tabs
- **Preview**: Show how text appears in-game
- **Used By**: List quests/items using this translation

### Translation Categories
Based on existing keys:
- **Q_GEN_**: General quest titles and descriptions
- **Q_RL_**: Red Light district specific
- **Q_IND_**: Industrial district specific  
- **Q_GRID_**: Grid district specific
- **Q_CBD_**: CBD district specific
- **STORY_**: Main story quest content
- **MENU_**: UI and menu text

## Integration Between Systems

### Quest → Translation Flow
1. User selects "Title Key" in Quest Browser
2. Dropdown shows available translation keys
3. Title field auto-populates with translated text  
4. "Edit Translation" button opens Translation Browser
5. Changes reflect immediately in Quest Browser

### Translation → Usage Flow  
1. User edits translation in Translation Browser
2. "Used By" panel shows affected quests
3. Quest Browser updates reflect changes immediately
4. Validation warnings if translation is deleted

### Item Rewards Integration
1. Quest Rewards tab links to Item Browser
2. Select items by name/icon from item definitions
3. Reward quantities and conditions
4. Export includes item reward data

## Technical Implementation

### Data Sources
- **questTemplates.xml**: Raw quest template definitions
- **questRuntimeState.xml**: Current game progress (read-only)  
- **translations.xml**: All game text translations
- **itemDefinitions.xml**: Item data for rewards

### Export/Import System
- **Export Quest Mod**: Generate quest template modifications
- **Export Translations**: Generate translation modifications  
- **Import Validation**: Verify quest dependencies and translations
- **Merge System**: Combine multiple mod files

### File Structure
```
QuestMods/
├── questTemplates_modified.xml
├── translations_modified.xml  
├── new_quests.xml
├── quest_rewards.xml
└── mod_manifest.xml
```

## User Workflow Examples

### Creating a New Quest
1. Open Quest Browser
2. Click "New Quest" 
3. Fill basic information (auto-assign ID)
4. Select district and locations
5. Add translation keys (create new if needed)
6. Add descriptions with translation keys
7. Link to sub-quests if applicable  
8. Add item rewards
9. Save and export

### Adding Custom Text
1. Open Translations Browser
2. Click "New Translation"
3. Enter unique key (e.g., "CUSTOM_QUEST_TITLE_01")
4. Add translated text
5. Save translation
6. Use in Quest Browser via dropdown

### Editing Existing Quest  
1. Filter quests by district
2. Select quest from list
3. Modify fields (preserves ID)
4. Add new descriptions/rewards
5. Export modified template

## Benefits

### For Modders
- **Complete Quest Control**: Create entirely new storylines
- **Professional Tools**: Validation, auto-completion, warnings
- **Translation Support**: Multi-language mods possible
- **Item Integration**: Reward system integration  
- **No Code Required**: Pure data-driven modding

### For Players
- **Rich Quest Content**: Complex branching storylines
- **Localized Mods**: Mods in multiple languages
- **Balanced Rewards**: Proper item reward integration
- **Professional Quality**: Editor ensures valid quest data

## Current Implementation Status (Updated)

### ✅ Completed Features
1. **Main Menu Redesigned**: Removed mission/weapon browsers, added translations browser
2. **Translation Browser**: Complete translation management system implemented
3. **Basic Quest Editing**: District, TitleKey, WakeOnLocation dropdowns/controls
4. **Field Validation**: District enum validation, translation key integration
5. **Project Integration**: All components build successfully and integrate properly

### 🔄 Currently Missing Quest Editor Features

Based on the Quest model analysis, here are the **uneditable fields** and implementation plans:

#### Completely Uneditable Fields

| Field | Current Status | Implementation Plan | Priority |
|-------|----------------|-------------------|----------|
| **Title** | Read-only text | Make editable text field | High |
| **State** | Read-only boolean | Add editable checkbox for "Completed" | Medium |
| **Location.LocationID** | Display-only | Add numeric control with validation | High |
| **VIP.HasVIP** | Not shown | Add checkbox control | Medium |
| **WakeOnLocationList** | Display-only | Add multi-select location picker | High |
| **Descriptions** | Display-only list | Add full CRUD interface | High |
| **SubQuests** | Display-only | Add multi-select quest picker | High |

#### Missing Advanced Features

| Feature | Current Status | Implementation Plan | Priority |
|---------|----------------|-------------------|----------|
| **Quest Descriptions Editor** | Read-only list display | Complete CRUD interface with translation keys | High |
| **Sub-Quest Management** | Display-only text | Multi-select dropdown with dependency validation | High |
| **Location Management** | Single location only | Multi-location picker with validation | Medium |
| **VIP Quest Support** | Not implemented | Checkbox and VIP-specific options | Medium |
| **Quest State Management** | Not implemented | Completion status and state controls | Low |
| **Quest Rewards System** | Not implemented | Item reward selection and management | Medium |

## Detailed Implementation Plans

### 1. Quest Descriptions Editor (HIGH PRIORITY)

#### Current Issue
- Descriptions are displayed in a read-only list
- Cannot add, edit, or remove quest descriptions
- No translation key management for descriptions

#### Implementation Plan
```csharp
// New UI Controls Needed
private DataGridView dgvDescriptions;
private Button btnAddDescription;
private Button btnDeleteDescription;
private ComboBox cmbDescriptionKey;
private TextBox txtDescriptionText;
private CheckBox chkDescriptionIsNew;
private CheckBox chkDescriptionSeen;

// New Methods Needed
private void LoadQuestDescriptions()
private void AddDescription()
private void EditDescription()
private void DeleteDescription()
private void ValidateDescriptionKeys()
```

#### UI Design
```
[Descriptions Tab]
┌─────────────────────────────────────────────────┐
│ Translation Key    │ Text Preview    │ Actions │
├────────────────────┼─────────────────┼─────────┤
│ Q_GEN_DESC_001 ▼  │ "Infiltrate..." │ Edit Del│
│ Q_GEN_DESC_002 ▼  │ "Extract the..." │ Edit Del│
└────────────────────┴─────────────────┴─────────┘
[Add Description] [Import from Translation]

Description Details:
┌─────────────────────────────────────────────────┐
│ Translation Key: [Q_GEN_DESC_003    ▼] [New...] │
│ Text Preview:   [Auto-populated from key      ] │
│ □ Is New   □ Has Been Seen                      │
└─────────────────────────────────────────────────┘
```

### 2. Sub-Quest Management (HIGH PRIORITY)

#### Current Issue
- Sub-quests are display-only text
- Cannot add, remove, or manage quest dependencies
- No validation for circular dependencies

#### Implementation Plan
```csharp
// New UI Controls Needed
private ListBox lstSubQuests;
private ComboBox cmbAvailableQuests;
private Button btnAddSubQuest;
private Button btnRemoveSubQuest;
private Label lblDependencyWarning;

// New Methods Needed
private void LoadAvailableQuests()
private void AddSubQuest()
private void RemoveSubQuest()
private void ValidateQuestDependencies()
private bool HasCircularDependency(int questId, List<int> subQuests)
```

#### UI Design
```
[Sub-Quests Tab]
┌─────────────────────────────────────────────────┐
│ Current Sub-Quests:                             │
│ ┌─────────────────────────────────────────────┐ │
│ │ ☑ [002] Secure the Perimeter               │ │
│ │ ☑ [005] Eliminate Security Chief           │ │
│ │ ☑ [012] Extract Corporate Data             │ │
│ └─────────────────────────────────────────────┘ │
│                                                 │
│ Add Sub-Quest:                                  │
│ [Select Quest...                         ▼] [+] │
│                                                 │
│ ⚠ Dependency Warning: Creating circular dep... │
└─────────────────────────────────────────────────┘
```

### 3. Multi-Location Management (MEDIUM PRIORITY)

#### Current Issue
- Only single WakeOnLocation supported
- WakeOnLocationList is not editable
- No location name validation

#### Implementation Plan
```csharp
// New UI Controls Needed
private CheckedListBox clbWakeLocations;
private NumericUpDown numNewLocation;
private Button btnAddLocation;
private Button btnRemoveLocation;
private Label lblLocationValidation;

// New Methods Needed
private void LoadWakeOnLocations()
private void AddWakeOnLocation()
private void RemoveWakeOnLocation()
private void ValidateLocationExists(int locationId)
private void UpdateLocationNames()
```

#### UI Design
```
[Locations Tab]
┌─────────────────────────────────────────────────┐
│ Wake On Location: [12 ▼] Industrial District    │
│                                                 │
│ Additional Wake Locations:                      │
│ ┌─────────────────────────────────────────────┐ │
│ │ ☑ [034] Red Light Back Alley               │ │
│ │ ☑ [078] Grid Corporate Plaza               │ │
│ │ ☑ [156] CBD Residential Block              │ │
│ └─────────────────────────────────────────────┘ │
│                                                 │
│ Add Location: [Location ID: 089] [Add]          │
│ ✓ Location 089: "Downtown Market" exists        │
└─────────────────────────────────────────────────┘
```

### 4. Quest Rewards System (MEDIUM PRIORITY)

#### Current Issue
- No reward system implementation
- Cannot link quests to item rewards
- Missing integration with existing ItemBrowser

#### Implementation Plan
```csharp
// New Model Extensions
public class QuestReward
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public float DropChance { get; set; }
    public bool IsGuaranteed { get; set; }
}

// New UI Controls Needed
private DataGridView dgvRewards;
private Button btnAddReward;
private Button btnRemoveReward;
private ComboBox cmbItemSelector;
private NumericUpDown numRewardQuantity;
private NumericUpDown numDropChance;

// New Methods Needed
private void LoadAvailableItems()
private void AddQuestReward()
private void RemoveQuestReward()
private void ValidateItemExists(int itemId)
```

#### UI Design
```
[Rewards Tab]
┌─────────────────────────────────────────────────┐
│ Quest Rewards:                                  │
│ ┌─────────────────────────────────────────────┐ │
│ │ Item             │Qty│Drop%│ Actions        │ │
│ ├─────────────────┼───┼─────┼───────────────┤ │
│ │🔫 Combat Rifle  │ 1 │100% │ Edit    Delete│ │
│ │💰 Credits       │500│ 75% │ Edit    Delete│ │
│ │🔧 Tech Component│ 3 │ 50% │ Edit    Delete│ │
│ └─────────────────┴───┴─────┴───────────────┘ │
│                                                 │
│ Add Reward:                                     │
│ Item: [Select Item...              ▼] [Browse] │
│ Quantity: [1  ] Drop Chance: [100%] □Guaranteed│
│                                      [Add]      │
└─────────────────────────────────────────────────┘
```

### 5. Enhanced Basic Field Editing (HIGH PRIORITY)

#### Fields Needing Implementation

##### Quest Title Direct Editing
```csharp
// Currently: Read-only text populated from translation
// Needed: Editable text field that can override translation

private void txtQuestTitle_TextChanged(object sender, EventArgs e)
{
    if (isEditing && activeQuest != null)
    {
        activeQuest.Title = txtQuestTitle.Text;
        OnQuestPropertyChanged();
    }
}
```

##### Quest State Management
```csharp
// Currently: Display-only "✅ Completed" or "⏳ Active"
// Needed: Editable checkbox for quest completion state

private CheckBox chkQuestCompleted;

private void chkQuestCompleted_CheckedChanged(object sender, EventArgs e)
{
    if (isEditing && activeQuest != null)
    {
        activeQuest.State = chkQuestCompleted.Checked;
        OnQuestPropertyChanged();
    }
}
```

##### VIP Quest Support
```csharp
// Currently: Not implemented
// Needed: Checkbox for VIP extraction quests

private CheckBox chkHasVIP;

private void chkHasVIP_CheckedChanged(object sender, EventArgs e)
{
    if (isEditing && activeQuest != null)
    {
        if (activeQuest.VIP == null)
            activeQuest.VIP = new QuestVIP();
        activeQuest.VIP.HasVIP = chkHasVIP.Checked;
        OnQuestPropertyChanged();
    }
}
```

##### Location ID Direct Editing
```csharp
// Currently: Display-only text
// Needed: Editable numeric control with validation

private NumericUpDown numLocationID;

private void numLocationID_ValueChanged(object sender, EventArgs e)
{
    if (isEditing && activeQuest != null)
    {
        if (activeQuest.Location == null)
            activeQuest.Location = new QuestLocation();
        activeQuest.Location.LocationID = (int)numLocationID.Value;
        OnQuestPropertyChanged();
    }
}
```

## Implementation Priority Plan

### Phase 1: Critical Missing Fields (HIGH PRIORITY)
1. **Quest Title Direct Editing** - Make title field editable
2. **Quest Descriptions CRUD** - Complete description management interface
3. **Sub-Quest Multi-Select** - Add/remove sub-quest dependencies
4. **Location ID Editing** - Make location editable with validation

### Phase 2: Advanced Features (MEDIUM PRIORITY) 
1. **Multi-Location Support** - WakeOnLocationList management
2. **VIP Quest Support** - VIP checkbox and related features
3. **Quest State Management** - Completion status controls
4. **Quest Rewards System** - Item reward selection and management

### Phase 3: Polish & Integration (LOW PRIORITY)
1. **Enhanced Validation** - Circular dependency detection
2. **Location Name Resolution** - Show location names instead of IDs
3. **Advanced Export Options** - Partial quest exports
4. **Import Validation** - Comprehensive mod file validation

## Technical Architecture Changes Needed

### Database/Model Updates
```csharp
// Extend Quest model with missing properties
public class Quest
{
    // Add missing reward system
    public List<QuestReward> Rewards { get; set; }
    
    // Ensure all nested objects are properly initialized
    public QuestLocation Location { get; set; } = new QuestLocation();
    public QuestVIP VIP { get; set; } = new QuestVIP();
}

// New reward model
public class QuestReward
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public int Quantity { get; set; }
    public float DropChance { get; set; }
    public bool IsGuaranteed { get; set; }
}
```

### UI Architecture Changes
```csharp
// Convert to tabbed interface for better organization
private TabControl tcQuestDetails;
private TabPage tpBasicInfo;
private TabPage tpLocations; 
private TabPage tpDescriptions;
private TabPage tpSubQuests;
private TabPage tpRewards;

// Enhanced editing state management
private Dictionary<string, bool> fieldEditStates;
private void SetFieldEditState(string fieldName, bool editable);
private void ValidateAllFields();
```

This comprehensive analysis shows that while the basic quest editing framework is in place, significant functionality is still missing for a complete quest editor. The phased implementation plan prioritizes the most critical missing features first, ensuring users can create fully functional custom quests.