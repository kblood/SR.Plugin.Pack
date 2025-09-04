using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SRMod.DTOs
{
    [Serializable]
    public class SerializableWeaponData
    {
        public WeaponType m_WeaponType;
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
            m_WeaponType = weaponType;
            m_Name = weaponData.m_Name;
            m_Range = weaponData.m_Range;
            m_ShootWhileChangeTarget = weaponData.m_ShootWhileChangeTarget;
            m_DefaultAmmo = (int)weaponData.m_DefaultAmmo;
            
            m_Ammo = new List<SerializableWeaponAttachmentAmmo>();
            if (weaponData.m_Ammo != null)
            {
                foreach (var ammo in weaponData.m_Ammo)
                {
                    m_Ammo.Add(new SerializableWeaponAttachmentAmmo(ammo));
                }
            }

            m_Abilities = new List<int>();
            if (weaponData.m_Abilities != null)
            {
                m_Abilities.AddRange(weaponData.m_Abilities);
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