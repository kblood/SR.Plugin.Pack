# API Reference

## Overview

This document provides a comprehensive reference for the Satellite Reign modding API, including all available classes, methods, and properties accessible to mod developers.

## Core Interfaces

### ISrPlugin Interface

The fundamental interface that all mods must implement.

```csharp
public interface ISrPlugin
{
    void Initialize();
    void Update();
    string GetName();
}
```

**Methods:**
- `Initialize()` - Called once when the plugin loads
- `Update()` - Called every frame during gameplay
- `GetName()` - Returns the plugin's display name

## Manager Classes

### Manager (Static Class)

Central access point for all game systems.

```csharp
public static class Manager
{
    public static Manager Get();
    public static UIManager GetUIManager();
    public static AudioManager GetAudioManager();
    public static InputManager GetInputManager();
    public static MoneyManager GetMoneyManager();
    public static WeaponManager GetWeaponManager();
    public static ItemManager GetItemManager();
    public static CameraManager GetCamera();
    public static PluginManager GetPluginManager();
    public static AIWorld GetAIWorld();
}
```

**Properties:**
- `Manager.Get().GameInProgress` - Returns `bool` indicating if game is active
- `Manager.Get().IsLoading()` - Returns `bool` indicating if game is loading

### UIManager

Handles all user interface operations.

```csharp
public class UIManager
{
    // Message Display
    public void ShowMessagePopup(string message, int duration);
    public void ShowWarningPopup(string message, int duration);
    public void ShowSubtitle(string text, int duration);
    public void HideSubtitle();
    
    // Advanced UI
    public void ShowBannerMessage(string title, string topText, string bottomText, int duration);
    public void DoModalMessageBox(string title, string message, InputBoxUi.InputBoxTypes type, 
        string okText, string cancelText, System.Action<bool> callback);
    
    // Screen Control
    public void ShowBlank(string text);
    public void ToggleTextWindow(string text);
    public void ToggleEverything(bool visible);
    
    // UI Components
    public InputBoxUi InputBoxUi { get; }
    public InputControlUI InputControlUi { get; }
    
    // Coroutine Support
    public IEnumerator WaitForActive(GameObject obj, bool active);
}
```

### AudioManager

Controls game audio and music.

```csharp
public class AudioManager
{
    public bool IsLoginMusicPlaying();
    public bool IsMusicPlaying();
    public bool IsSoundEnabled();
    public void StopAllMusic(bool immediate);
}
```

### InputManager

Manages input and entity selection.

```csharp
public class InputManager
{
    public List<AgentAI> GetPlayerSelectedEntities();
    public List<AIEntity> GetNonPlayerSelectedEntities();
    public void Select(int entityUID, bool addToSelection);
    public void ClearSelection();
    public InputControl GetInputControl();
}
```

### MoneyManager

Handles the game's economy system.

```csharp
public class MoneyManager
{
    public int GetCurrentFunds();
    public void SetCurrentFunds(int amount);
    public void OffsetFunds(int amount, bool showFeedback, bool playSound);
    public void UpdateBankSiphonAmount(Banks bank, int amount);
    
    public enum Banks
    {
        CBDDistrict_1,
        CBDDistrict_2,
        CBDDistrict_3,
        RedLightDistrict_1,
        IndustrialDistrict_1,
        AfluentDistrict_1
    }
}
```

### WeaponManager

Controls weapon data and behavior.

```csharp
public class WeaponManager
{
    public WeaponData[] m_WeaponData;
    public WeaponPrefab GetPrefab(WeaponType weaponType);
    public WeaponAttachmentAmmo GetAmmoData(WeaponType weaponType, AmmoType ammoType);
}
```

### ItemManager

Manages game items and equipment.

```csharp
public class ItemManager
{
    public ItemData[] GetAllItems();
    public ItemData GetItemData(int itemId);
    
    public class ItemData
    {
        public int m_ID;
        public string m_Name;
        public string m_Description;
        public int m_Cost;
        public ItemSlotTypes m_ItemSlotType;
        public ItemSubCategories m_ItemSubCategory;
        public int m_StackSize;
    }
}
```

## Agent and AI Classes

### AgentAI

Represents player-controlled agents.

```csharp
public class AgentAI : AIEntity
{
    // Static Methods
    public static AgentAI[] GetAgents();
    public static AgentAI FirstAgentAi();
    public static AgentAI FirstSelectedAgentAi();
    public static AgentAI GetAgent(AgentClass agentClass);
    
    // Properties
    public bool m_Dead;
    public Health m_Health;
    public Energy m_Energy;
    public Weapons m_Weapons;
    public Modifiers m_Modifiers;
    public Abilities m_Abilities;
    public Identity m_Identity;
    public Wardrobe m_Wardrobe;
    public CarAI m_InCar;
    public int CurrentCloneableId;
    
    // Methods
    public bool IsSelected();
    public bool IsDowned();
    public bool IsInDanger();
    public AgentClass GetClass();
    public string AgentClassName();
    public string GetName();
    
    // Health and Energy
    public void SetHealthValue(float health);
    public float GetHealthValue();
    
    // Weapons
    public void AddAmmo(int ammoType);
    public void SetWeapon(int weaponSlot);
    public WeaponType GetWeaponType(int slot);
    public Weapon GetWeapon();
    public bool HasWeaponOut();
    public void PutWeaponAway(bool immediate);
    
    // Abilities
    public void ServerAddAbility(int abilityId);
    public void SkillUpdated();
    
    // Movement and Position
    public void RespawnAtCurrentLocation();
    public void RespawnAt(Vector3 position, Quaternion rotation);
    public void Teleport(Transform target);
    
    // Vehicle Interaction
    public void UseCar(Transform car, int doorIndex);
    public void ExitCar(Transform car, int doorIndex);
    
    public enum AgentClass
    {
        Soldier,
        Assassin,
        Hacker,
        Support
    }
}
```

### AIEntity

Base class for all AI entities.

```csharp
public class AIEntity : MonoBehaviour
{
    // Properties
    public bool m_IsControllable;
    public bool m_IsIgnoringInput;
    public AIAbilities m_AIAbilities;
    public GroupID m_Group;
    public Health m_Health;
    public Selectable m_Selectable;
    public Identity m_Identity;
    public Wardrobe m_Wardrobe;
    
    // Methods
    public bool IsSelected();
    public bool IsHuman();
    public bool IsMech();
    public bool IsAddedToWorld();
    public string GetName();
    public string GetDescription();
    public void SetSelected(bool selected);
    public void CurrentlySelected();
    public void SetHealthValue(float health);
    public bool HasAIAbility(AIAbilities ability);
    
    // Static Methods
    public static AIEntity[] FindObjectsOfType(System.Type type);
}
```

### CarAI

Represents vehicle entities.

```csharp
public class CarAI : AIEntity
{
    public bool m_IgnoreInput;
    public int m_DoorsInUse;
    
    public void SetParked();
    
    // Static Methods
    public static CarAI[] FindObjectsOfType(System.Type type);
}
```

## Data Structures

### WeaponData

Defines weapon properties.

```csharp
public class WeaponData
{
    public string m_Name;
    public float m_Range;
    public bool m_ShootWhileChangeTarget;
    public WeaponAttachmentAmmo[] m_Ammo;
    public AmmoType m_DefaultAmmo;
}
```

### WeaponAttachmentAmmo

Defines ammunition properties.

```csharp
public class WeaponAttachmentAmmo
{
    public AmmoType m_Type;
    public float m_damage_max;
    public float m_damage_min;
    public float m_DamageRadius;
    public float m_knockback_amount;
    public int m_max_ammo;
    public float m_reload_time;
    public float m_ReloadSpeed;
    public float m_ChargeTime;
    public bool m_ChargeEveryShot;
    public float m_shield_damage;
    public float m_CritChance;
    public float m_AccuracyDelta;
}
```

### ModifierData5L

Represents character modifiers.

```csharp
public class ModifierData5L
{
    public ModifierType m_Type;
    public float m_Ammount;
    public ModifierType m_AmountModifier;
}
```

### CloneableData

Defines agent appearance and identity.

```csharp
public class CloneableData
{
    public WardrobeManager.Sex Sex;
    public WardrobeManager.Sex m_Sex;
    public int RandomSeed;
    public int m_RandomSeed;
    public int IdentityId;
    public int m_IdentityID;
    public WardrobeManager.WardrobeType WardrobeType;
    public WardrobeManager.WardrobeType m_WardrobeType;
    public WardrobeConfigurationData WardrobeConfigurationData;
    public List<CloneableModifier> GetModifiers();
    public List<ModifierData5L> GetModifierDatas();
}
```

## Enumerations

### WeaponType

```csharp
public enum WeaponType
{
    B_pistol = 0,
    P_pistol = 1,
    B_smg = 2,
    P_Smg = 3,
    B_rifle = 4,
    P_rifle = 5,
    B_shotgun = 6,
    P_shotgun = 7,
    P_Minigun = 8,
    B_sniper = 9,
    P_sniper = 10
    // Additional weapon types...
}
```

### ItemSlotTypes

```csharp
public enum ItemSlotTypes
{
    Gear = 0,
    AugmentationHead = 1,
    AugmentationBody = 2,
    AugmentationArms = 3,
    AugmentationLegs = 4,
    WeaponPistol = 5,
    Weapon = 6,
    WeaponAugmentation = 7,
    Max = 8
}
```

### ItemSubCategories

```csharp
public enum ItemSubCategories
{
    Standard = 0,
    Drone = 1,
    WeaponAmmo = 2,
    ArmorBody = 3,
    ArmorHead = 4,
    Shields = 5,
    StealthGenerators = 6
}
```

### ModifierType

```csharp
public enum ModifierType
{
    HealthOffset,
    HealthRegenRate,
    EnergyMaxOffset,
    EnergyRegenRate,
    SpeedOffset,
    SpeedMultiplier,
    AccuracyModifier,
    AccuracyOffset,
    WeaponRangeMultiplier,
    DamageResistance,
    ShieldCapacity,
    ShieldRegenRate,
    StealthDuration,
    HackerLevel,
    HardwireLevel,
    ThiefMoneyStealAmount,
    UseHighVent,
    CanUseZipline,
    PoisonValveStrength,
    GearSlotIncrease,
    WeaponCountIncrease,
    PerfectShot,
    AbilityCooldownSpeedMultiplier,
    HijackerLevel
    // Additional modifier types...
}
```

### AIAbilities

```csharp
[System.Flags]
public enum AIAbilities
{
    None = 0,
    CanMelee = 1,
    ScanEnemy = 2,
    Shoot = 4,
    LeaveCover = 8,
    GetWeaponOut = 16,
    FindBetterCover = 32,
    OnlyNearCover = 64,
    HasSpeedAdjust = 128,
    IgnoreBeingShot = 256,
    LicensedToKill = 512
    // Additional abilities...
}
```

### GroupID

```csharp
public enum GroupID
{
    Resistance,
    Police,
    Corporate,
    Gang,
    Civilian
    // Additional group types...
}
```

## Unity Integration Classes

### TimeManager

```csharp
public static class TimeManager
{
    public static float GameTime { get; }
    public static float RealTime { get; }
    
    public static TimeScaler AddTimeScaler();
    
    public class TimeScaler
    {
        public float TimeScale { get; set; }
        public bool Paused { get; }
        
        public void Pause();
        public void Reset();
    }
}
```

### Input (Unity)

```csharp
public static class Input
{
    public static bool GetKeyDown(KeyCode key);
    public static bool GetKey(KeyCode key);
    public static bool GetKeyUp(KeyCode key);
}
```

### KeyCode (Unity)

```csharp
public enum KeyCode
{
    // Alphabet
    A, B, C, D, E, F, G, H, I, J, K, L, M,
    N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
    
    // Numbers
    Alpha0, Alpha1, Alpha2, Alpha3, Alpha4,
    Alpha5, Alpha6, Alpha7, Alpha8, Alpha9,
    
    // Function Keys
    F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
    
    // Keypad
    Keypad0, Keypad1, Keypad2, Keypad3, Keypad4,
    Keypad5, Keypad6, Keypad7, Keypad8, Keypad9,
    KeypadPlus, KeypadMinus, KeypadMultiply, KeypadDivide,
    
    // Special Keys
    Space, Enter, Escape, Tab, Backspace, Delete,
    Insert, Home, End, PageUp, PageDown,
    Pause, LeftControl, RightControl, LeftShift, RightShift,
    LeftAlt, RightAlt,
    
    // Arrow Keys
    UpArrow, DownArrow, LeftArrow, RightArrow,
    
    // Punctuation
    Comma, Period, Slash, Semicolon, Quote,
    LeftBracket, RightBracket, Backslash,
    Minus, Equals
}
```

## Utility Classes

### FileManager

```csharp
public static class FileManager
{
    public static string ExecPath { get; }
    
    public static bool SaveData(byte[] data, string fileName);
    public static void SaveAsXML<T>(T data, string fileName);
    public static void SaveAsXML(List<TranslationElementDTO> data, string fileName);
    public static List<TranslationElementDTO> LoadTranslationsXML(string fileName);
    
    public static string SaveTextureToFile(Texture2D texture);
    public static Texture2D LoadTextureFromFile(string textureName);
    
    public static void SaveText(string text, string fileName);
    public static string SaveList(List<string> strings, string fileName);
    public static List<string> LoadList(string fileName);
    
    public static string FilePathCheck(string fileName);
}
```

### UIHelper

```csharp
public static class UIHelper
{
    public static SRModVerticalButtonsUI VerticalButtonsUi { get; set; }
    
    public static void ShowMessage(string text, int seconds = 30, int messageTypes = 0);
    public static IEnumerator ModalVerticalButtonsRoutine(string title, List<SRModButtonElement> buttons);
}
```

### SRModButtonElement

```csharp
public class SRModButtonElement
{
    public string ButtonText;
    public string Description;
    public UnityAction Action;
    public Button Button;
    public Text Text;
    public Text DescriptionText;
    public Transform Container;
    
    public SRModButtonElement(string buttonText, UnityAction action, string description);
}
```

## Manager Singletons

### CloneManager

```csharp
public class CloneManager
{
    public static CloneManager Get();
    
    public CloneableData GetCloneableData(int cloneId);
    public CloneableData NewCloneableFromPrefab(AIEntity prefab);
}
```

### IdentityManager

```csharp
public class IdentityManager
{
    public static IdentityManager Get();
    public static int GetRandomNameID(WardrobeManager.Sex sex);
    
    public void GetName(int nameId, out string firstName, out string lastName);
}
```

### ProgressionManager

```csharp
public class ProgressionManager
{
    public static ProgressionManager Get();
    
    public District CurrentDistrict { get; }
    public event OnDistrictChangeDelegate OnDistrictChange;
    
    public void UpdateLocationText();
    
    public delegate void OnDistrictChangeDelegate();
}
```

## Exception Handling

### Common Exception Patterns

```csharp
// Safe method call pattern
try
{
    Manager.GetUIManager().ShowMessagePopup("Test message", 5);
}
catch (NullReferenceException e)
{
    Debug.LogError($"Manager not available: {e.Message}");
}
catch (Exception e)
{
    Debug.LogError($"Unexpected error: {e.Message}");
}

// Safe property access
var gameManager = Manager.Get();
if (gameManager != null && gameManager.GameInProgress)
{
    // Safe to proceed with game operations
}

// Safe collection iteration
var agents = AgentAI.GetAgents();
if (agents != null)
{
    foreach (var agent in agents)
    {
        if (agent != null)
        {
            // Process agent
        }
    }
}
```

## Version Compatibility

### API Availability

- **Core ISrPlugin Interface**: Available in all versions
- **Manager Classes**: Available in all versions, some methods may vary
- **UI System**: Stable across versions
- **Agent/AI Classes**: Core functionality stable, extended features may vary
- **File I/O**: Stable XML serialization, binary operations may vary
- **Custom UI Framework**: Based on community extensions, may require specific mods

### Deprecation Notices

Always check for null references and handle exceptions gracefully, as API availability may change between game updates.

## Complete Example

```csharp
using System;
using System.Collections.Generic;
using UnityEngine;

public class APIReferenceExample : ISrPlugin
{
    public void Initialize()
    {
        Debug.Log("API Reference Example Plugin Initialized");
    }
    
    public void Update()
    {
        if (!Manager.Get().GameInProgress)
            return;
            
        if (Input.GetKeyDown(KeyCode.F1))
        {
            DemonstrateAPIs();
        }
    }
    
    private void DemonstrateAPIs()
    {
        // UI Manager
        Manager.GetUIManager().ShowMessagePopup("API Demo Started", 3);
        
        // Agent Management
        foreach (var agent in AgentAI.GetAgents())
        {
            if (agent.IsSelected())
            {
                agent.SetHealthValue(100);
                agent.m_Energy.AddEnergy(500);
            }
        }
        
        // Money Management
        Manager.GetMoneyManager().OffsetFunds(1000, true, true);
        
        // Time Management
        var timeScaler = TimeManager.AddTimeScaler();
        timeScaler.TimeScale = 2f;
    }
    
    public string GetName()
    {
        return "API Reference Example";
    }
}
```

This API reference provides comprehensive documentation for all available classes, methods, and properties in the Satellite Reign modding framework.