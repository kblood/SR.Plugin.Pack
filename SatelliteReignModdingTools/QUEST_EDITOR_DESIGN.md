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

## Post-Implementation Enhancement Goals (HIGH PRIORITY)

### 1. Dropdown Selection State Management (HIGH PRIORITY)

#### Current Issue
When entering edit mode, dropdowns reset to default values instead of showing the currently selected values from the quest data.

#### Implementation Plan
```csharp
// Fix dropdown initialization in UpdateQuestDetails()
private void UpdateQuestDetails()
{
    if (activeQuest == null) return;
    
    // District dropdown - ensure correct selection
    if (isEditing && !string.IsNullOrEmpty(activeQuest.District))
    {
        for (int i = 0; i < cmbDistrict.Items.Count; i++)
        {
            var item = (dynamic)cmbDistrict.Items[i];
            if (item.Value.ToString() == activeQuest.District)
            {
                cmbDistrict.SelectedIndex = i;
                break;
            }
        }
    }
    
    // Title key dropdown - ensure correct selection  
    if (isEditing && !string.IsNullOrEmpty(activeQuest.TitleKey))
    {
        int index = cmbTitleKey.Items.Cast<string>()
            .ToList().IndexOf(activeQuest.TitleKey);
        if (index >= 0)
            cmbTitleKey.SelectedIndex = index;
    }
    
    // Available quests dropdown should exclude current quest and existing sub-quests
    PopulateAvailableQuestsDropdown();
}
```

### 2. Quest Reward Items System (HIGH PRIORITY)

#### Current Issue
Sub-quest system exists but quest rewards (items, money, experience) are not implemented. This is a critical feature for complete quest functionality.

#### Implementation Plan
```csharp
// New reward model extensions
public class QuestReward
{
    public QuestRewardType Type { get; set; } // Item, Money, Experience, etc.
    public int ItemId { get; set; }           // For item rewards
    public string ItemName { get; set; }      // Display name
    public int Quantity { get; set; }         // Amount to reward
    public float DropChance { get; set; }     // Probability (0-100%)
    public bool IsGuaranteed { get; set; }    // Always awarded
    public string Description { get; set; }   // Reward description
}

public enum QuestRewardType
{
    Item,
    Money,
    Experience,
    Blueprint,
    Prototype,
    Ability
}

// New UI Controls Needed
private TabPage tpRewards;
private DataGridView dgvRewards;
private ComboBox cmbRewardType;
private ComboBox cmbRewardItem;
private NumericUpDown numRewardQuantity;
private NumericUpDown numDropChance;
private CheckBox chkGuaranteed;
private Button btnAddReward;
private Button btnEditReward;
private Button btnDeleteReward;
private Button btnBrowseItems; // Links to existing ItemBrowser
```

#### UI Design for Rewards Tab
```
[Rewards Tab]
┌─────────────────────────────────────────────────┐
│ Quest Rewards:                                  │
│ ┌─────────────────────────────────────────────┐ │
│ │Type     │Item/Amount     │Qty│Drop%│Guar.  │ │
│ ├─────────┼───────────────┼───┼─────┼───────┤ │
│ │Item     │Combat Rifle   │ 1 │100% │  ✓    │ │
│ │Money    │Credits        │500│ 75% │  ✗    │ │
│ │Item     │Tech Component │ 3 │ 50% │  ✗    │ │
│ │Blueprint│Stealth Augment│ 1 │ 25% │  ✗    │ │
│ └─────────┴───────────────┴───┴─────┴───────┘ │
│                                                 │
│ Add Reward:                                     │
│ Type: [Item        ▼] Item: [Browse...] [📁]    │
│ Quantity: [1  ] Drop Chance: [100%] □Guaranteed│
│                                      [Add]      │
│                                                 │
│ 💡 Tip: Use Browse button to select from       │
│    existing game items via Item Browser        │
└─────────────────────────────────────────────────┘
```

### 3. Translation Integration for Descriptions (HIGH PRIORITY)

#### Current Issue
Quest descriptions can be added/deleted but cannot be edited with translation key integration. Users need seamless translation workflow.

#### Implementation Plan
```csharp
// Enhanced description editing with translation integration
private void btnEditDescription_Click(object sender, EventArgs e)
{
    if (!isEditing || activeQuest == null || lstDescriptions.SelectedIndex < 0)
        return;

    var selectedDescription = activeQuest.Descriptions[lstDescriptions.SelectedIndex];
    
    // Show translation edit dialog
    var translationEditForm = new TranslationEditDialog(selectedDescription.LocTitle, translations);
    if (translationEditForm.ShowDialog() == DialogResult.OK)
    {
        // Update description with new/modified translation
        selectedDescription.LocTitle = translationEditForm.TranslationKey;
        selectedDescription.Translation = translationEditForm.TranslationText;
        
        // Add to translations if new key
        if (translationEditForm.IsNewTranslation)
        {
            var newTranslation = new Translation 
            {
                Key = translationEditForm.TranslationKey,
                Element = new TranslationElement 
                {
                    Token = translationEditForm.TranslationKey,
                    Translations = new List<string> { translationEditForm.TranslationText }
                }
            };
            translations.Add(newTranslation);
        }
        
        OnQuestPropertyChanged();
        UpdateQuestDetails();
    }
}

// New Translation Edit Dialog
public partial class TranslationEditDialog : Form
{
    public string TranslationKey { get; set; }
    public string TranslationText { get; set; }
    public bool IsNewTranslation { get; set; }
    
    // UI: Key textbox, text area, existing keys dropdown, OK/Cancel
}
```

### 4. Quest-Translation Editor Integration (MEDIUM PRIORITY)

#### Current Issue
No direct links between Quest Editor and Translations Browser for seamless workflow.

#### Implementation Plan
```csharp
// Add translation helper buttons throughout Quest Editor
private Button btnEditTitleTranslation;
private Button btnEditDescriptionTranslation;
private Button btnOpenTranslationsEditor;

// Quick translation editing methods
private void btnEditTitleTranslation_Click(object sender, EventArgs e)
{
    if (activeQuest?.TitleKey != null)
    {
        // Open Translations Browser focused on this key
        var translationsBrowser = new TranslationsBrowser(activeQuest.TitleKey);
        translationsBrowser.ShowDialog();
        
        // Refresh translation display
        PopulateTitleKeyDropdown();
        UpdateQuestDetails();
    }
}

// Enhanced TranslationsBrowser constructor
public TranslationsBrowser(string focusTranslationKey = null)
{
    InitializeComponent();
    InitializeEditor();
    LoadAllData();
    
    // Focus on specific translation if provided
    if (!string.IsNullOrEmpty(focusTranslationKey))
    {
        FocusOnTranslation(focusTranslationKey);
    }
    
    UpdateTranslationList();
    SetupFormTitle();
}
```

#### UI Integration Points
```
[Quest Editor - Enhanced with Translation Links]
┌─────────────────────────────────────────────────┐
│ Title Key: [Q_GEN_TITLE_001    ▼] [Edit Trans.] │
│ Title:     [Mission Control              ] [🔗]  │
│                                                 │
│ Descriptions:                    [Add] [Edit]   │
│ ┌─────────────────────────────────────────────┐ │
│ │ Q_GEN_DESC_001: "Infiltrate..."    [Edit🔗]│ │
│ │ Q_GEN_DESC_002: "Extract the..."    [Edit🔗]│ │
│ └─────────────────────────────────────────────┘ │
│                                                 │
│ [🌐 Open Translations Editor]                   │
└─────────────────────────────────────────────────┘
```

### 5. Enhanced Quest Validation System

#### Current Issue
Missing comprehensive validation for quest completeness and game compatibility.

#### Implementation Plan
```csharp
// Quest validation system
public class QuestValidator
{
    public static QuestValidationResult ValidateQuest(Quest quest, List<Translation> translations, List<Quest> allQuests)
    {
        var result = new QuestValidationResult();
        
        // Check required fields
        if (string.IsNullOrEmpty(quest.Title))
            result.Errors.Add("Quest must have a title");
            
        // Check translation keys exist
        if (!string.IsNullOrEmpty(quest.TitleKey) && !translations.Any(t => t.Key == quest.TitleKey))
            result.Warnings.Add($"Translation key '{quest.TitleKey}' not found in translations");
            
        // Validate sub-quest dependencies
        foreach (int subQuestId in quest.SubQuests ?? new List<int>())
        {
            if (!allQuests.Any(q => q.ID == subQuestId))
                result.Errors.Add($"Sub-quest {subQuestId} does not exist");
        }
        
        // Validate location IDs
        if (quest.Location?.LocationID > 0 && !QuestValidation.LocationExists(quest.Location.LocationID))
            result.Warnings.Add($"Location ID {quest.Location.LocationID} may not exist in game");
            
        return result;
    }
}

public class QuestValidationResult
{
    public List<string> Errors { get; set; } = new List<string>();
    public List<string> Warnings { get; set; } = new List<string>();
    public bool IsValid => Errors.Count == 0;
}
```

## Implementation Priority (Updated)

### Phase 1: Critical UX Issues (HIGH PRIORITY)
1. **Fix Dropdown Selections** - Dropdowns show current values in edit mode
2. **Quest Rewards System** - Complete item reward functionality  
3. **Description Translation Integration** - Seamless description editing with translations

### Phase 2: Workflow Enhancement (MEDIUM PRIORITY)  
1. **Quest-Translation Editor Links** - Direct navigation between editors
2. **Title Translation Integration** - Enhanced title editing with translation support
3. **Comprehensive Quest Validation** - Pre-save validation system

### Phase 3: Advanced Features (LOW PRIORITY)
1. **Quest Templates System** - Save/load quest templates
2. **Bulk Translation Operations** - Mass translation key operations
3. **Quest Dependency Visualization** - Visual quest chain mapping
4. **Advanced Export Options** - Selective quest export with dependencies

## Technical Implementation Notes

### Data Model Extensions Required
```csharp
// Extend Quest model  
public class Quest
{
    // Add reward system
    public List<QuestReward> Rewards { get; set; } = new List<QuestReward>();
    
    // Enhanced validation
    public QuestValidationResult ValidationResult { get; set; }
    
    // Translation integration
    public DateTime LastTranslationUpdate { get; set; }
    public List<string> RequiredTranslationKeys { get; set; } = new List<string>();
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
private TabPage tpRewards;      // NEW
private TabPage tpTranslations; // NEW

// Enhanced integration
private Button btnQuickTranslationEdit;
private Button btnValidateQuest;
private Label lblValidationStatus;
```

This enhanced plan addresses all the critical usability issues identified and provides a roadmap for creating a truly professional quest development environment with seamless translation workflow and complete reward system integration.