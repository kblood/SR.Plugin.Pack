# Custom Weapons and Weapon Modification

## Overview

Satellite Reign's weapon system is built around the `WeaponManager` and `WeaponData` classes. This guide covers how to modify existing weapons, create custom weapons, and understand the weapon data structure.

## Weapon System Architecture

### WeaponType Enum
The game uses a `WeaponType` enum to identify different weapon types:

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
    P_sniper = 10,
    // ... additional weapon types
}
```

### WeaponData Structure
Each weapon is defined by a `WeaponData` object containing:

- **Basic Properties**: Name, Range, Damage
- **Ammo Data**: Different ammunition types and their properties
- **Behavior Flags**: Shooting behavior, targeting options
- **Audio/Visual**: Effects and sounds (limited mod access)

## Accessing the Weapon System

### Getting the WeaponManager
```csharp
WeaponManager weaponManager = Manager.GetWeaponManager();
```

### Accessing Weapon Data
```csharp
// Get specific weapon data
WeaponData pistolData = weaponManager.m_WeaponData[(int)WeaponType.B_pistol];

// Iterate through all weapons
foreach (WeaponData weaponData in weaponManager.m_WeaponData)
{
    Debug.Log($"Weapon: {weaponData.m_Name}");
}
```

## Modifying Existing Weapons

### Basic Weapon Properties
```csharp
public void ModifyWeaponProperties()
{
    WeaponManager weaponManager = Manager.GetWeaponManager();
    
    // Modify pistol range
    weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Range = 200f;
    
    // Enable shooting while changing targets
    weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_ShootWhileChangeTarget = true;
    
    // Modify all weapons at once
    foreach (WeaponData data in weaponManager.m_WeaponData)
    {
        data.m_Range = 60f;
        data.m_ShootWhileChangeTarget = true;
    }
}
```

### Ammunition Modification
Each weapon has multiple ammunition types with different properties:

```csharp
public void ModifyAmmunition()
{
    WeaponManager weaponManager = Manager.GetWeaponManager();
    WeaponData minigunData = weaponManager.m_WeaponData[(int)WeaponType.P_Minigun];
    
    // Modify primary ammo properties
    WeaponAttachmentAmmo primaryAmmo = minigunData.m_Ammo[0];
    primaryAmmo.m_ChargeTime = 0f;           // No charge time
    primaryAmmo.m_damage_max = 25f;          // Maximum damage
    primaryAmmo.m_damage_min = 20f;          // Minimum damage
    primaryAmmo.m_DamageRadius = 15f;        // Explosion radius
    primaryAmmo.m_knockback_amount = 20f;    // Knockback force
    primaryAmmo.m_max_ammo = 100;            // Magazine size
    primaryAmmo.m_reload_time = 0.5f;        // Reload time
    primaryAmmo.m_ChargeEveryShot = false;   // No per-shot charging
    primaryAmmo.m_shield_damage = 30f;       // Shield damage
    primaryAmmo.m_CritChance = 0.2f;         // 20% crit chance
    primaryAmmo.m_AccuracyDelta = 0.95f;     // High accuracy
    
    // Modify all ammo types for a weapon
    foreach (WeaponAttachmentAmmo ammo in minigunData.m_Ammo)
    {
        ammo.m_reload_time = 0.1f;
        ammo.m_ChargeTime = 0f;
    }
}
```

### Complete Weapon Enhancement Example
```csharp
public class WeaponEnhancer : ISrPlugin
{
    private bool weaponsModified = false;
    
    public void Initialize()
    {
        Debug.Log("Weapon Enhancer: Initialized");
    }
    
    public void Update()
    {
        if (!Manager.Get().GameInProgress || weaponsModified)
            return;
            
        try
        {
            EnhanceAllWeapons();
            weaponsModified = true;
            Manager.GetUIManager().ShowMessagePopup("All weapons enhanced!", 5);
        }
        catch (Exception e)
        {
            Debug.LogError($"Weapon enhancement failed: {e.Message}");
        }
    }
    
    private void EnhanceAllWeapons()
    {
        WeaponManager weaponManager = Manager.GetWeaponManager();
        
        foreach (WeaponData weaponData in weaponManager.m_WeaponData)
        {
            // Enhance basic properties
            weaponData.m_Range = Mathf.Max(weaponData.m_Range, 80f);
            weaponData.m_ShootWhileChangeTarget = true;
            
            // Enhance all ammunition types
            foreach (WeaponAttachmentAmmo ammo in weaponData.m_Ammo)
            {
                ammo.m_damage_max = Mathf.Max(ammo.m_damage_max, 25f);
                ammo.m_damage_min = Mathf.Max(ammo.m_damage_min, 20f);
                ammo.m_reload_time = Mathf.Max(0.1f, ammo.m_reload_time * 0.5f);
                ammo.m_ChargeTime = 0f;
                ammo.m_ChargeEveryShot = false;
                ammo.m_CritChance = Mathf.Min(1f, ammo.m_CritChance + 0.1f);
            }
        }
    }
    
    public string GetName()
    {
        return "Weapon Enhancer";
    }
}
```

## Custom Weapon Creation

### Approach 1: Weapon Data Cloning
Create new weapons by cloning and modifying existing weapon data:

```csharp
public class CustomWeaponCreator
{
    public void CreateSuperPistol()
    {
        WeaponManager weaponManager = Manager.GetWeaponManager();
        
        // Clone existing pistol data
        WeaponData basePistol = weaponManager.m_WeaponData[(int)WeaponType.B_pistol];
        WeaponData superPistol = CloneWeaponData(basePistol);
        
        // Customize the new weapon
        superPistol.m_Name = "Super Pistol";
        superPistol.m_Range = 150f;
        
        // Enhance ammo
        foreach (WeaponAttachmentAmmo ammo in superPistol.m_Ammo)
        {
            ammo.m_damage_max = 50f;
            ammo.m_damage_min = 45f;
            ammo.m_max_ammo = 20;
            ammo.m_reload_time = 0.5f;
        }
        
        // Note: Adding to the weapon array requires more advanced techniques
        // This example shows the data structure setup
    }
    
    private WeaponData CloneWeaponData(WeaponData original)
    {
        // Create a deep copy of the weapon data
        // Implementation depends on available serialization methods
        // This is a simplified example
        WeaponData clone = new WeaponData();
        clone.m_Name = original.m_Name + " (Custom)";
        clone.m_Range = original.m_Range;
        clone.m_ShootWhileChangeTarget = original.m_ShootWhileChangeTarget;
        
        // Clone ammunition data
        clone.m_Ammo = new WeaponAttachmentAmmo[original.m_Ammo.Length];
        for (int i = 0; i < original.m_Ammo.Length; i++)
        {
            clone.m_Ammo[i] = CloneAmmoData(original.m_Ammo[i]);
        }
        
        return clone;
    }
    
    private WeaponAttachmentAmmo CloneAmmoData(WeaponAttachmentAmmo original)
    {
        WeaponAttachmentAmmo clone = new WeaponAttachmentAmmo();
        clone.m_damage_max = original.m_damage_max;
        clone.m_damage_min = original.m_damage_min;
        clone.m_reload_time = original.m_reload_time;
        clone.m_ChargeTime = original.m_ChargeTime;
        clone.m_max_ammo = original.m_max_ammo;
        // Copy other properties as needed
        return clone;
    }
}
```

### Approach 2: Runtime Weapon Modification
Modify weapons as agents equip them:

```csharp
public class DynamicWeaponModifier : ISrPlugin
{
    private Dictionary<AgentAI, WeaponType> lastWeaponTypes = new Dictionary<AgentAI, WeaponType>();
    
    public void Update()
    {
        if (!Manager.Get().GameInProgress)
            return;
            
        foreach (AgentAI agent in AgentAI.GetAgents())
        {
            CheckAndModifyAgentWeapon(agent);
        }
    }
    
    private void CheckAndModifyAgentWeapon(AgentAI agent)
    {
        if (agent.GetWeapon() == null)
            return;
            
        WeaponType currentWeapon = agent.GetWeaponType(0);
        
        // Check if weapon changed
        if (!lastWeaponTypes.ContainsKey(agent) || 
            lastWeaponTypes[agent] != currentWeapon)
        {
            ModifyAgentWeapon(agent, currentWeapon);
            lastWeaponTypes[agent] = currentWeapon;
        }
    }
    
    private void ModifyAgentWeapon(AgentAI agent, WeaponType weaponType)
    {
        Weapon weapon = agent.GetWeapon();
        
        // Modify weapon properties
        weapon.m_ChargeTime = 0f;
        weapon.m_ChargeEveryShot = false;
        weapon.m_Delayed = false;
        weapon.m_ShootWhileChangeTarget = true;
        
        Debug.Log($"Modified {agent.GetName()}'s {weaponType}");
    }
    
    public void Initialize() { }
    public string GetName() => "Dynamic Weapon Modifier";
}
```

## Weapon Data Serialization

### Saving Custom Weapon Configurations
Use the FileManager service to save weapon configurations:

```csharp
[System.Serializable]
public class CustomWeaponConfig
{
    public string WeaponName { get; set; }
    public float Range { get; set; }
    public float Damage { get; set; }
    public float ReloadTime { get; set; }
    public int MagazineSize { get; set; }
}

public class WeaponConfigManager
{
    public void SaveWeaponConfig(WeaponType weaponType, CustomWeaponConfig config)
    {
        try
        {
            FileManager.SaveAsXML(config, $"weapon_{weaponType}.xml");
            Debug.Log($"Saved config for {weaponType}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save weapon config: {e.Message}");
        }
    }
    
    public CustomWeaponConfig LoadWeaponConfig(WeaponType weaponType)
    {
        try
        {
            // Note: LoadXML method would need to be implemented
            // This is a conceptual example
            return FileManager.LoadXML<CustomWeaponConfig>($"weapon_{weaponType}.xml");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load weapon config: {e.Message}");
            return CreateDefaultConfig(weaponType);
        }
    }
    
    private CustomWeaponConfig CreateDefaultConfig(WeaponType weaponType)
    {
        return new CustomWeaponConfig
        {
            WeaponName = weaponType.ToString(),
            Range = 50f,
            Damage = 20f,
            ReloadTime = 2f,
            MagazineSize = 15
        };
    }
}
```

## Advanced Weapon Techniques

### Weapon Upgrade System
Create a system for upgrading weapons during gameplay:

```csharp
public class WeaponUpgradeSystem : ISrPlugin
{
    private Dictionary<WeaponType, int> upgradelevels = new Dictionary<WeaponType, int>();
    
    public void Initialize()
    {
        // Initialize upgrade levels
        foreach (WeaponType weaponType in Enum.GetValues(typeof(WeaponType)))
        {
            upgradelevels[weaponType] = 0;
        }
    }
    
    public void Update()
    {
        if (!Manager.Get().GameInProgress)
            return;
            
        // Check for upgrade input
        if (Input.GetKeyDown(KeyCode.U))
        {
            UpgradeSelectedAgentWeapons();
        }
    }
    
    private void UpgradeSelectedAgentWeapons()
    {
        foreach (AgentAI agent in AgentAI.GetAgents())
        {
            if (agent.IsSelected())
            {
                WeaponType weaponType = agent.GetWeaponType(0);
                UpgradeWeapon(weaponType);
                
                Manager.GetUIManager().ShowMessagePopup(
                    $"Upgraded {agent.GetName()}'s {weaponType} to level {upgradelevels[weaponType]}", 3);
            }
        }
    }
    
    private void UpgradeWeapon(WeaponType weaponType)
    {
        WeaponManager weaponManager = Manager.GetWeaponManager();
        WeaponData weaponData = weaponManager.m_WeaponData[(int)weaponType];
        
        upgradelevels[weaponType]++;
        int level = upgradelevels[weaponType];
        
        // Apply upgrades based on level
        float damageMultiplier = 1f + (level * 0.2f);
        float rangeMultiplier = 1f + (level * 0.1f);
        float reloadMultiplier = 1f - (level * 0.05f);
        
        weaponData.m_Range *= rangeMultiplier;
        
        foreach (WeaponAttachmentAmmo ammo in weaponData.m_Ammo)
        {
            ammo.m_damage_max *= damageMultiplier;
            ammo.m_damage_min *= damageMultiplier;
            ammo.m_reload_time *= Math.Max(0.1f, reloadMultiplier);
        }
    }
    
    public string GetName() => "Weapon Upgrade System";
}
```

### Weapon Switching Enhancement
Improve weapon switching mechanics:

```csharp
public class WeaponSwitchEnhancer : ISrPlugin
{
    public void Update()
    {
        if (!Manager.Get().GameInProgress)
            return;
            
        HandleWeaponSwitching();
    }
    
    private void HandleWeaponSwitching()
    {
        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            SwitchToNextWeapon(1);
        }
        else if (Input.GetKeyDown(KeyCode.PageDown))
        {
            SwitchToNextWeapon(-1);
        }
    }
    
    private void SwitchToNextWeapon(int direction)
    {
        foreach (AgentAI agent in AgentAI.GetAgents())
        {
            if (agent.IsSelected())
            {
                bool hadWeaponOut = agent.HasWeaponOut();
                
                // Switch weapon
                agent.m_Weapons.NextWeaponCheat(direction);
                agent.SetWeapon(10); // Apply weapon change
                
                // Maintain weapon state
                if (!hadWeaponOut)
                {
                    agent.PutWeaponAway(true);
                }
                
                // Show feedback
                WeaponType newWeapon = agent.GetWeaponType(0);
                Manager.GetUIManager().ShowMessagePopup(
                    $"{agent.GetName()} switched to {newWeapon}", 2);
            }
        }
    }
    
    public void Initialize() { }
    public string GetName() => "Weapon Switch Enhancer";
}
```

## Limitations and Considerations

### Current Limitations
1. **No New Weapon Types**: Adding completely new weapon types to the enum is complex
2. **Limited Visual/Audio**: Weapon effects and sounds are harder to modify
3. **Save Game Compatibility**: Weapon modifications may affect save game compatibility

### Best Practices
1. **Backup Original Data**: Always store original weapon values before modification
2. **Gradual Modification**: Apply changes gradually to avoid breaking game balance
3. **User Feedback**: Always inform users when weapons are modified
4. **Compatibility**: Test with other mods to ensure compatibility

### Future Enhancements
- Integration with custom item system for weapon attachments
- Dynamic weapon generation based on player progress
- Weapon crafting and customization systems
- Integration with save game data for persistent upgrades

This concludes the custom weapons documentation. The weapon system provides extensive modification capabilities while maintaining game stability and performance.