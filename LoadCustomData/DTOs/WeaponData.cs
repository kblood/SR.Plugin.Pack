using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using SRMod.Services;

namespace SRMod.DTOs
{
    [Serializable]
    public class SerializableWeaponData
    {
        public int m_WeaponType; // Using int instead of WeaponType enum for XML serialization compatibility
        public string m_Name;
        public float m_Range;
        public bool m_ShootWhileChangeTarget;
        public int m_DefaultAmmo; // Using int instead of AmmoType enum for simplicity
        public List<SerializableWeaponAttachmentAmmo> m_Ammo;
        public List<int> m_Abilities;

        public SerializableWeaponData() 
        { 
            m_Ammo = new List<SerializableWeaponAttachmentAmmo>();
            m_Abilities = new List<int>();
        }

        public SerializableWeaponData(WeaponData weaponData, WeaponType weaponType)
        {
            try
            {
                m_WeaponType = (int)weaponType;
                m_Name = weaponData?.m_Name ?? string.Empty; // Handle null names
                m_Range = weaponData?.m_Range ?? 0f;
                m_ShootWhileChangeTarget = weaponData?.m_ShootWhileChangeTarget ?? false;
                
                // Handle DefaultAmmo conversion
                try
                {
                    m_DefaultAmmo = weaponData != null ? (int)weaponData.m_DefaultAmmo : 0;
                }
                catch
                {
                    m_DefaultAmmo = 0; // Default value if conversion fails
                }
                
                // Handle Ammo array
                m_Ammo = new List<SerializableWeaponAttachmentAmmo>();
                if (weaponData?.m_Ammo != null)
                {
                    foreach (var ammo in weaponData.m_Ammo)
                    {
                        if (ammo != null) // Add null check for individual ammo entries
                        {
                            try
                            {
                                m_Ammo.Add(new SerializableWeaponAttachmentAmmo(ammo));
                            }
                            catch (System.Exception ex)
                            {
                                // Skip problematic ammo entries but continue processing
                                SRInfoHelper.Log($"WeaponData: Skipping problematic ammo for {weaponType}: {ex.Message}");
                            }
                        }
                    }
                }

                // Handle Abilities array
                m_Abilities = new List<int>();
                if (weaponData?.m_Abilities != null)
                {
                    try
                    {
                        m_Abilities.AddRange(weaponData.m_Abilities);
                    }
                    catch (System.Exception ex)
                    {
                        // Skip problematic abilities but continue processing
                        SRInfoHelper.Log($"WeaponData: Skipping problematic abilities for {weaponType}: {ex.Message}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                // If constructor fails completely, create minimal valid object
                SRInfoHelper.Log($"WeaponData: Constructor failed for {weaponType}, creating minimal object: {ex.Message}");
                m_WeaponType = (int)weaponType;
                m_Name = string.Empty;
                m_Range = 0f;
                m_ShootWhileChangeTarget = false;
                m_DefaultAmmo = 0;
                m_Ammo = new List<SerializableWeaponAttachmentAmmo>();
                m_Abilities = new List<int>();
            }
        }
    }

    [Serializable]
    public class SerializableWeaponAttachmentAmmo
    {
        public int m_Type; // Using int instead of AmmoType enum
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
        public float m_CritDamageMultiplier;
        public float m_AccuracyDelta;
        public float m_Emp;
        public float m_MaxBeamWidth;
        public int m_ProjectilesPerShot;

        public SerializableWeaponAttachmentAmmo() { }

        public SerializableWeaponAttachmentAmmo(WeaponAttachmentAmmo ammo)
        {
            if (ammo == null)
            {
                // Set default values for null ammo
                m_Type = 0;
                m_damage_max = 0f;
                m_damage_min = 0f;
                m_DamageRadius = 0f;
                m_knockback_amount = 0f;
                m_max_ammo = 0;
                m_reload_time = 0f;
                m_ReloadSpeed = 0f;
                m_ChargeTime = 0f;
                m_ChargeEveryShot = false;
                m_shield_damage = 0f;
                m_CritChance = 0f;
                m_CritDamageMultiplier = 1f;
                m_AccuracyDelta = 0f;
                m_Emp = 0f;
                m_MaxBeamWidth = 0f;
                m_ProjectilesPerShot = 1;
                return;
            }

            try
            {
                m_Type = (int)ammo.m_Type;
                m_damage_max = ammo.m_damage_max;
                m_damage_min = ammo.m_damage_min;
                m_DamageRadius = ammo.m_DamageRadius;
                m_knockback_amount = ammo.m_knockback_amount;
                m_max_ammo = ammo.m_max_ammo;
                m_reload_time = ammo.m_reload_time;
                m_ReloadSpeed = ammo.m_ReloadSpeed;
                m_ChargeTime = ammo.m_ChargeTime;
                m_ChargeEveryShot = ammo.m_ChargeEveryShot;
                m_shield_damage = ammo.m_shield_damage;
                m_CritChance = ammo.m_CritChance;
                m_CritDamageMultiplier = ammo.m_CritDamageMultiplier;
                m_AccuracyDelta = ammo.m_AccuracyDelta;
                m_Emp = ammo.m_Emp;
                m_MaxBeamWidth = ammo.m_MaxBeamWidth;
                m_ProjectilesPerShot = ammo.m_ProjectilesPerShot;
            }
            catch (System.Exception ex)
            {
                SRInfoHelper.Log($"SerializableWeaponAttachmentAmmo: Error copying ammo data: {ex.Message}");
                // Set safe defaults on error
                m_Type = 0;
                m_damage_max = 1f;
                m_damage_min = 1f;
                m_DamageRadius = 0f;
                m_knockback_amount = 0f;
                m_max_ammo = 1;
                m_reload_time = 1f;
                m_ReloadSpeed = 1f;
                m_ChargeTime = 0f;
                m_ChargeEveryShot = false;
                m_shield_damage = 0f;
                m_CritChance = 0f;
                m_CritDamageMultiplier = 1f;
                m_AccuracyDelta = 0f;
                m_Emp = 0f;
                m_MaxBeamWidth = 0f;
                m_ProjectilesPerShot = 1;
            }
        }

        public void ApplyToAmmo(WeaponAttachmentAmmo ammo)
        {
            ammo.m_Type = (WeaponAmmoType)m_Type;
            ammo.m_damage_max = m_damage_max;
            ammo.m_damage_min = m_damage_min;
            ammo.m_DamageRadius = m_DamageRadius;
            ammo.m_knockback_amount = m_knockback_amount;
            ammo.m_max_ammo = m_max_ammo;
            ammo.m_reload_time = m_reload_time;
            ammo.m_ReloadSpeed = m_ReloadSpeed;
            ammo.m_ChargeTime = m_ChargeTime;
            ammo.m_ChargeEveryShot = m_ChargeEveryShot;
            ammo.m_shield_damage = m_shield_damage;
            ammo.m_CritChance = m_CritChance;
            ammo.m_CritDamageMultiplier = m_CritDamageMultiplier;
            ammo.m_AccuracyDelta = m_AccuracyDelta;
            ammo.m_Emp = m_Emp;
            ammo.m_MaxBeamWidth = m_MaxBeamWidth;
            ammo.m_ProjectilesPerShot = m_ProjectilesPerShot;
        }
    }
}