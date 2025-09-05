using System;
using System.Collections.Generic;
using System.Linq;
using SRMod.DTOs;
using SRMod.Services;

public class WeaponDataManager
{
    public static void ExportWeaponDataToXML(string filename = "weapons.xml")
    {
        try
        {
            SRInfoHelper.Log("WeaponDataManager: Starting weapon data export");
            
            var weaponManager = Manager.GetWeaponManager();
            if (weaponManager?.m_WeaponData == null)
            {
                SRInfoHelper.Log("WeaponDataManager: WeaponManager or weapon data is null");
                return;
            }

            var serializableWeapons = new List<SerializableWeaponData>();
            
            for (int i = 0; i < weaponManager.m_WeaponData.Length; i++)
            {
                try
                {
                    var weaponData = weaponManager.m_WeaponData[i];
                    if (weaponData != null)
                    {
                        var weaponType = (WeaponType)i;
                        var serializable = new SerializableWeaponData(weaponData, weaponType);
                        serializableWeapons.Add(serializable);
                        SRInfoHelper.Log($"WeaponDataManager: Successfully serialized {weaponType}");
                    }
                    else
                    {
                        SRInfoHelper.Log($"WeaponDataManager: Weapon at index {i} is null");
                    }
                }
                catch (System.Exception ex)
                {
                    SRInfoHelper.Log($"WeaponDataManager: Error serializing weapon at index {i}: {ex.Message}");
                }
            }

            SRInfoHelper.Log($"WeaponDataManager: Attempting to serialize {serializableWeapons.Count} weapons to XML");
            
            string path = Manager.GetPluginManager().PluginPath;
            try
            {
                FileManager.SaveAsXML(serializableWeapons, filename, path);
                SRInfoHelper.Log($"WeaponDataManager: Successfully exported {serializableWeapons.Count} weapon definitions to {filename}");
            }
            catch (System.Exception ex)
            {
                SRInfoHelper.Log($"WeaponDataManager: XML serialization failed: {ex.Message}");
                SRInfoHelper.Log($"WeaponDataManager: Full exception: {ex}");
                throw; // Re-throw to preserve stack trace
            }
        }
        catch (System.Exception ex)
        {
            SRInfoHelper.Log($"WeaponDataManager: Error exporting weapon data: {ex.Message}");
        }
    }

    public static void ImportWeaponDataFromXML(string filename = "weapons.xml")
    {
        try
        {
            SRInfoHelper.Log("WeaponDataManager: Starting weapon data import");
            
            string path = Manager.GetPluginManager().PluginPath;
            var weaponList = FileManager.LoadFromXML<List<SerializableWeaponData>>(filename, path);
            
            if (weaponList == null || weaponList.Count == 0)
            {
                SRInfoHelper.Log("WeaponDataManager: No weapon data found or failed to load - this is normal if no weapons.xml exists");
                return;
            }

            var weaponManager = Manager.GetWeaponManager();
            if (weaponManager?.m_WeaponData == null)
            {
                SRInfoHelper.Log("WeaponDataManager: WeaponManager or weapon data is null - game may not be ready");
                return;
            }
            
            SRInfoHelper.Log($"WeaponDataManager: Found {weaponManager.m_WeaponData.Length} weapons in WeaponManager");

            int updatedCount = 0;
            
            foreach (var serializableWeapon in weaponList)
            {
                try
                {
                    int weaponIndex = serializableWeapon.m_WeaponType;
                    
                    if (weaponIndex >= 0 && weaponIndex < weaponManager.m_WeaponData.Length)
                    {
                        var existingWeapon = weaponManager.m_WeaponData[weaponIndex];
                        if (existingWeapon != null && HasWeaponChanged(existingWeapon, serializableWeapon))
                        {
                            ApplyWeaponChanges(existingWeapon, serializableWeapon);
                            updatedCount++;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    SRInfoHelper.Log($"WeaponDataManager: Error updating weapon {serializableWeapon.m_WeaponType}: {ex.Message}");
                }
            }
            
            SRInfoHelper.Log($"WeaponDataManager: Import complete - updated {updatedCount} weapons from {filename}");
            
            // Force agents to refresh their weapon data if any weapons were updated
            if (updatedCount > 0)
            {
                try
                {
                    var agents = AgentAI.GetAgents();
                    if (agents != null)
                    {
                        foreach (var agent in agents)
                        {
                            try
                            {
                                // Force weapon refresh by toggling weapons if possible
                                if (agent.m_Weapons != null && agent.GetWeapon() != null)
                                {
                                    // Simple approach: just log that we would refresh the weapon
                                    // The weapon changes should take effect automatically
                                    SRInfoHelper.Log($"WeaponDataManager: Weapon data updated for agent {agent.GetName()}");
                                }
                            }
                            catch (System.Exception agentEx)
                            {
                                SRInfoHelper.Log($"WeaponDataManager: Could not refresh weapon for agent {agent?.GetName()}: {agentEx.Message}");
                            }
                        }
                    }
                }
                catch (System.Exception refreshEx)
                {
                    SRInfoHelper.Log($"WeaponDataManager: Could not refresh agent weapons: {refreshEx.Message}");
                }
            }
        }
        catch (System.Exception ex)
        {
            SRInfoHelper.Log($"WeaponDataManager: Error importing weapon data: {ex.Message}");
        }
    }

    private static bool HasWeaponChanged(WeaponData existingWeapon, SerializableWeaponData importedWeapon)
    {
        bool hasChanged = false;
        List<string> changes = new List<string>();
        
        if (Math.Abs(existingWeapon.m_Range - importedWeapon.m_Range) > 0.001f)
        {
            changes.Add(string.Format("Range: {0} -> {1}", existingWeapon.m_Range, importedWeapon.m_Range));
            hasChanged = true;
        }
        
        if (existingWeapon.m_ShootWhileChangeTarget != importedWeapon.m_ShootWhileChangeTarget)
        {
            changes.Add(string.Format("ShootWhileChangeTarget: {0} -> {1}", existingWeapon.m_ShootWhileChangeTarget, importedWeapon.m_ShootWhileChangeTarget));
            hasChanged = true;
        }
        
        if ((int)existingWeapon.m_DefaultAmmo != importedWeapon.m_DefaultAmmo)
        {
            changes.Add(string.Format("DefaultAmmo: {0} -> {1}", existingWeapon.m_DefaultAmmo, importedWeapon.m_DefaultAmmo));
            hasChanged = true;
        }

        // Check ammo changes
        if (existingWeapon.m_Ammo != null && importedWeapon.m_Ammo != null)
        {
            for (int i = 0; i < Math.Min(existingWeapon.m_Ammo.Length, importedWeapon.m_Ammo.Count); i++)
            {
                if (HasAmmoChanged(existingWeapon.m_Ammo[i], importedWeapon.m_Ammo[i]))
                {
                    changes.Add(string.Format("Ammo[{0}] stats changed", i));
                    hasChanged = true;
                }
            }
        }
        
        if (hasChanged)
        {
            SRInfoHelper.Log(string.Format("WeaponDataManager: Weapon {0} changes detected: {1}", 
                importedWeapon.m_WeaponType, string.Join(", ", changes.ToArray())));
        }
        
        return hasChanged;
    }

    private static bool HasAmmoChanged(WeaponAttachmentAmmo existingAmmo, SerializableWeaponAttachmentAmmo importedAmmo)
    {
        return Math.Abs(existingAmmo.m_damage_max - importedAmmo.m_damage_max) > 0.001f ||
               Math.Abs(existingAmmo.m_damage_min - importedAmmo.m_damage_min) > 0.001f ||
               Math.Abs(existingAmmo.m_reload_time - importedAmmo.m_reload_time) > 0.001f ||
               Math.Abs(existingAmmo.m_ReloadSpeed - importedAmmo.m_ReloadSpeed) > 0.001f ||
               Math.Abs(existingAmmo.m_ChargeTime - importedAmmo.m_ChargeTime) > 0.001f ||
               existingAmmo.m_ChargeEveryShot != importedAmmo.m_ChargeEveryShot ||
               Math.Abs(existingAmmo.m_CritChance - importedAmmo.m_CritChance) > 0.001f ||
               existingAmmo.m_max_ammo != importedAmmo.m_max_ammo;
    }

    private static void ApplyWeaponChanges(WeaponData existingWeapon, SerializableWeaponData importedWeapon)
    {
        // Update basic weapon properties
        existingWeapon.m_Range = importedWeapon.m_Range;
        existingWeapon.m_ShootWhileChangeTarget = importedWeapon.m_ShootWhileChangeTarget;
        existingWeapon.m_DefaultAmmo = (WeaponAmmoType)importedWeapon.m_DefaultAmmo;
        
        // Update ammo properties
        if (existingWeapon.m_Ammo != null && importedWeapon.m_Ammo != null)
        {
            for (int i = 0; i < Math.Min(existingWeapon.m_Ammo.Length, importedWeapon.m_Ammo.Count); i++)
            {
                importedWeapon.m_Ammo[i].ApplyToAmmo(existingWeapon.m_Ammo[i]);
            }
        }
        
        // Update abilities if available
        if (importedWeapon.m_Abilities != null && importedWeapon.m_Abilities.Count > 0)
        {
            existingWeapon.m_Abilities = importedWeapon.m_Abilities.ToArray();
        }
        
        SRInfoHelper.Log(string.Format("WeaponDataManager: Updated weapon {0} ({1})", 
            importedWeapon.m_WeaponType, importedWeapon.m_Name));
    }
}