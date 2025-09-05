using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Serialization;

namespace SRMod.DTOs
{
    [Serializable]
    public class SerializableWeaponData
    {
        [Description("Weapon type index (0-30)")]
        public int m_WeaponType { get; set; }

        [Description("Weapon display name")]
        public string m_Name { get; set; }

        [Description("Maximum range of the weapon")]
        public float m_Range { get; set; }

        [Description("Can shoot while changing targets")]
        public bool m_ShootWhileChangeTarget { get; set; }

        [Description("Default ammo type index")]
        public int m_DefaultAmmo { get; set; }

        [Description("List of ammunition configurations")]
        public List<SerializableWeaponAttachmentAmmo> m_Ammo { get; set; }

        [Description("List of ability IDs")]
        public List<int> m_Abilities { get; set; }

        public SerializableWeaponData() 
        { 
            m_Ammo = new List<SerializableWeaponAttachmentAmmo>();
            m_Abilities = new List<int>();
            m_Name = "";
        }
    }

    [Serializable]
    public class SerializableWeaponAttachmentAmmo
    {
        [Description("Ammo type index")]
        public int m_Type { get; set; }

        [Description("Maximum damage per shot")]
        public float m_damage_max { get; set; }

        [Description("Minimum damage per shot")]
        public float m_damage_min { get; set; }

        [Description("Damage radius for explosives")]
        public float m_DamageRadius { get; set; }

        [Description("Knockback force amount")]
        public float m_knockback_amount { get; set; }

        [Description("Magazine size (rounds per reload)")]
        public int m_max_ammo { get; set; }

        [Description("Time to reload in seconds")]
        public float m_reload_time { get; set; }

        [Description("Reload speed multiplier")]
        public float m_ReloadSpeed { get; set; }

        [Description("Charge time before firing")]
        public float m_ChargeTime { get; set; }

        [Description("Must charge for every shot")]
        public bool m_ChargeEveryShot { get; set; }

        [Description("Damage dealt to shields")]
        public float m_shield_damage { get; set; }

        [Description("Critical hit chance (0.0-1.0)")]
        public float m_CritChance { get; set; }

        [Description("Critical damage multiplier")]
        public float m_CritDamageMultiplier { get; set; }

        [Description("Accuracy modification")]
        public float m_AccuracyDelta { get; set; }

        [Description("EMP damage")]
        public float m_Emp { get; set; }

        [Description("Maximum beam width for beam weapons")]
        public float m_MaxBeamWidth { get; set; }

        [Description("Number of projectiles per shot")]
        public int m_ProjectilesPerShot { get; set; }

        public SerializableWeaponAttachmentAmmo() 
        {
            m_CritDamageMultiplier = 1f;
            m_ProjectilesPerShot = 1;
            m_max_ammo = 1;
            m_damage_max = 1f;
            m_damage_min = 1f;
        }
    }
}