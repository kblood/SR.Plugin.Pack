using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace SyndicateMod
{
    public class SyndicateMod : ISrPlugin
    {

        bool weaponsUpdated = false;

        /// <summary>
        /// Plugin initialization 
        /// </summary>
        public void Initialize()
        {
            Debug.Log("Initializing Satellite Reign Syndicate mod");
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            if (Manager.Get().GameInProgress)
            {
                if (!weaponsUpdated)
                {
                    WeaponChanges();
                    weaponsUpdated = true;
                }

                if (Input.GetKeyDown(KeyCode.Delete) || Input.GetKeyDown(KeyCode.Delete))
                {
                    UnlockAll();
                    //Manager.GetItemManager().UnlockAllPrototypes();
                    //Manager.GetUIManager().ShowMessagePopup("Unlocked all?");
                }
                if (Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.PageDown))
                {
                    var units = GetSelectUnits();
                    ToggleControlOfUnits(units);
                }

                if (Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.Home))
                {
                    var units = GetSelectUnits();

                    var agents = AgentAI.GetAgents();

                    var abils = agents.Where(a => a.GetClass() == AgentAI.AgentClass.Hacker).FirstOrDefault()?.m_Abilities.AllAbilities().Select(a => a.GetID()).ToArray();

                    var Soldier = AgentAI.GetAgent(AgentAI.AgentClass.Soldier);
                    var Hacker = AgentAI.GetAgent(AgentAI.AgentClass.Hacker);
                    var Support = AgentAI.GetAgent(AgentAI.AgentClass.Support);
                    var Assassin = AgentAI.GetAgent(AgentAI.AgentClass.Assassin);

                    foreach (var selectedAgent in agents.Where(u => u.IsSelected()))
                    {
                        if (selectedAgent.m_Class != AgentAI.AgentClass.NONE)
                            selectedAgent.m_Class = AgentAI.AgentClass.NONE;
                    }

                    AddAbilitiesToUnits(units, abils);

                    ToggleWeaponOfUnits(units, WeaponType.B_minigun);

                    Soldier.m_Class = AgentAI.AgentClass.Soldier;
                    Hacker.m_Class = AgentAI.AgentClass.Hacker;
                    Support.m_Class = AgentAI.AgentClass.Support;
                    Assassin.m_Class = AgentAI.AgentClass.Assassin;
                }
                if (Input.GetKeyDown(KeyCode.Insert) || Input.GetKeyDown(KeyCode.Insert))
                {
                    var units = GetSelectUnits();
                    var selectedUnit = units.FirstOrDefault();
                    if (selectedUnit == null)
                        return;

                    string[] aNames = new string[0];
                    int[] aIds = new int[0];
                    selectedUnit.GetAbilities().m_AbilityManager.GetAbilityNamesAndIDs(out aIds, out aNames);

                    string info = "";
                    int enabled = 0; int instant = 0; int passive = 0;
                    int ready = 0; int allowed = 0; int remote = 0;

                    foreach (Ability abil in selectedUnit.GetAbilities().AllAbilities())
                    {
                        info += "\nAbility: " + abil.m_Name + " id: " + abil.GetID() + " uid: " + abil.GetUID();
                    }

                    for (int i = 0; i < aIds.Length; i++)
                    {
                        Ability abil = selectedUnit.GetAbilities().GetAbility(aIds[i]);
                        if (abil != null)
                        {
                            if (abil.IsEnabled)
                                enabled++;
                            if (abil.isInstant)
                                instant++;
                            if (abil.isPassive)
                                passive++;
                            if (abil.IsReady())
                                ready++;
                            if (abil.IsAllowed())
                                allowed++;
                            if (abil.IsRemoteAbility())
                                remote++;
                            //if (abil.IsActive())
                        }

                    }
                    info += "\n Total: \n" + enabled + " enabled. " + instant + " instant" + passive + " passive" + ready + " ready" + allowed + " allowed" + remote + " remote";
                    Manager.GetUIManager().DoModalMessageBox("Abilities of selected unit", info, InputBoxUi.InputBoxTypes.MbInput);
                }
                if (Input.GetKeyDown(KeyCode.End) || Input.GetKeyDown(KeyCode.End))
                {
                    AddAbilityToSelectedUnits();
                }

                if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.PageUp))
                {
                    Manager.GetUIManager().ShowMessagePopup("Listing weapons");
                    var units = GetSelectUnits();

                    string info = "\n";
                    string[] aNames = new string[0];
                    int[] aIds = new int[0];
                    Manager.GetAbilityManager().GetAbilityNamesAndIDs(out aIds, out aNames);

                    var weapons = Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().ToList();

                    for (int i = 0; i < 29; i++)
                    {
                        info += "(ID_"+ i + ":" + (WeaponType)i + ")";
                    }
                    //info = info.Substring(0, 230);
                    bool userOk;

                    Coroutine corot = null;
                    Manager.GetUIManager().DoModalMessageBox("Add ability of selected units", "Pick weapon to add:" + info, InputBoxUi.InputBoxTypes.MbInput, null, null,
                    delegate (bool b)
                    {
                        userOk = b;
                        if (b)
                        {
                            var input = Manager.GetUIManager().InputValue();
                            int id = 0;
                            int.TryParse(input, out id);
                            if(id > 0 || id < 28)
                                ToggleWeaponOfUnits(units, (WeaponType)id);
                        }
                    },
                    null, "put skill ID here");
                }

            }
        }

        public List<AIEntity> GetSelectUnits()
        {
            List<AIEntity> selected = new List<AIEntity>();

            foreach (AIEntity ae in AIEntity.FindObjectsOfType(typeof(AIEntity)))
            {
                if ((ae.IsHuman() || ae.IsMech()) && ae.IsSelected()) //&& ae.m_Group != GroupID.Resistance)
                {
                    selected.Add(ae);
                }
            }
            return selected;
        }

        public void AddAbilityToSelectedUnits(int indexAt = 1)
        {
            Manager.GetUIManager().ShowMessagePopup("Getting skills");
            var units = GetSelectUnits();

            string info = "ID and ability name:";
            string[] aNames = new string[0];
            int[] aIds = new int[0];
            Manager.GetAbilityManager().GetAbilityNamesAndIDs(out aIds, out aNames);

            Manager.GetUIManager().ShowMessagePopup("About to show skills");

            for (int i = indexAt; i < aIds.Length && i < (indexAt +10); i++)
            {
                //Manager.GetUIManager().ShowWarningPopup("Adding skill no: " + i + ": " + aIds[i] + ": " + aNames[i], 5);
                if (aNames[i].ToLower().Contains("obsolete"))
                {
                    indexAt++;
                    continue;
                }
                info += "\n" + aIds[i] + " : " + aNames[i];
            }
            Manager.GetUIManager().ShowMessagePopup("info text is " + info.Length + " long.", 8);
            if(info.Length > 400)
                info = info.Substring(0, 400);

            bool userOk;
            Manager.GetUIManager().ShowWarningPopup("ModalMessageBox being created", 2);
            //Coroutine corot = null;
            Manager.GetUIManager().DoModalMessageBox("Add ability to selected units", info, InputBoxUi.InputBoxTypes.MbInput, null, "Next",
            delegate (bool b)
            {
                userOk = b;
                //Manager.GetUIManager().ShowWarningPopup("Clicked message box " + b, 4);
                if (b)
                {
                    var input = Manager.GetUIManager().InputValue();
                    int id = 0;
                    int.TryParse(input, out id);

                    if(aIds.Contains(id))
                    {
                        foreach (var unit in units)
                            AddAbilityToUnit(unit, id);

                        //int[] ids = new int[1];
                        //ids[0] = id;
                        //AddAbilitiesToUnits(units, ids);
                    }
                }
                else
                {
                    AddAbilityToSelectedUnits(indexAt+10);
                }
            },
            null, "put skill ID here");
        }

        public void ToggleControlOfUnits(List<AIEntity> units)
        {
            foreach (AIEntity ae in units)
            {
                if (ae.m_Group != GroupID.Resistance)
                {
                    ae.m_IsIgnoringInput = false;
                    ae.m_IsControllable = !ae.m_IsControllable;
                    
                    //ae.m_HijackLevelRequired = 1;
                    //ae.SetHealthValue(0);
                    //ae.m_Health.HealthValue = 0;
                    //ae.m_Health.DisableShields();
                    if (ae.m_IsControllable)
                    {
                        Manager.GetUIManager().ShowMessagePopup("Now controlling " + ae.name + " " + ae.GetDescription(), 6);
                        ae.Hijack(AgentAI.GetAgent(AgentAI.AgentClass.Soldier), true);
                        ae.m_Wardrobe.SetXrayColor(Color.magenta);
                        ae.UpdateXrayColor();
                    }
                    else
                    {
                        Manager.GetUIManager().ShowMessagePopup("Released control of " + ae.name + " " + ae.GetDescription(), 6);
                        ae.Unhijack();
                        ae.m_Wardrobe.SetXrayColor(Color.blue);
                        ae.UpdateXrayColor();
                    }
                    //ae.name = "Caldor was here!";

                    //ae.ServerAddAbility(1);
                }
            }
        }

        public void ToggleWeaponOfUnits(List<AIEntity> units, WeaponType type)
        {
            //DebugOptions.ms_DebugEnabled = true;
            //DebugOptions.ms_DebugEnabledParanoid = true;

            Manager.GetUIManager().ShowMessagePopup("Changing weapons of select units to " + type.ToString(), 6);
            foreach (AIEntity ae in units)
            {
                //ae.m_Weapons.NextWeaponCheat(1);
                ae.m_Weapons.AddWeapon(type);
                //ae.m_Weapons.SetWeapon((int)type);
                //ae.m_Weapons.NotifyWeaponChanged();
                //ae.SetWeaponToToggleTo(type);
                //ae.m_Weapons.TogglePrimaryWeapon();
                //ae.m_Weapons.NotifyWeaponChanged();
            }
            Manager.GetUIManager().ShowWarningPopup("Changed weapons of select units to " + type.ToString());
        }

        public void AddAbilityToUnit(AIEntity unit, int abilityId)
        {
            var skillManager = Manager.GetSkillManager();

            //unit.m_Modifiers.AddModifier(ModifierType.HackerLevel, 5, 5);
            //unit.m_Modifiers.AddModifier(ModifierType.HardwireLevel, 5, 5);
            //unit.m_Modifiers.AddModifier(ModifierType.HijackerLevel, 5, 5);

            var abil =  unit.m_Abilities.AddAbility(abilityId);

            var mod = new ModifierData5L();
            mod.m_Type = ModifierType.HackerLevel;
            mod.m_Ammount = 5;
            mod.m_AmountModifier = ModifierType.HackerLevel;
            unit.m_Modifiers.AddModifier(mod);

            mod = new ModifierData5L();
            mod.m_Type = ModifierType.HijackerLevel;
            mod.m_Ammount = 5;
            mod.m_AmountModifier = ModifierType.HijackerLevel;
            unit.m_Modifiers.AddModifier(mod);

            mod = new ModifierData5L();
            mod.m_Type = ModifierType.HardwireLevel;
            mod.m_Ammount = 5;
            mod.m_AmountModifier = ModifierType.HardwireLevel;
            unit.m_Modifiers.AddModifier(mod);

            //skillManager.AddLevel(abil.GetID());
            
            abil.State = Ability.AbilityState.ready;
            abil.SetState(Ability.AbilityState.ready);
            abil.ToSave();
            //abil.ShouldAIPerformAbility();
            abil.m_AvailableWhenAlive = true;
            //abil.UpdateAvailable();
            //abil.UpdateAndNotify();


        }

        public void AddAbilitiesToUnits(List<AIEntity> units, int[] abils)
        {
            foreach (AIEntity ae in units)
            {
                ae.m_Abilities.AddAbilities(abils);
                ae.m_Abilities.UpdateAbilitiesToAllow();

                ae.m_Abilities.AddAbilities(abils);
                var skillManager = Manager.GetSkillManager();
                foreach (var abi in ae.m_Abilities.AllAbilities())
                {
                    
                    var data = Manager.GetSkillManager().GetSkillData(abi.GetID());

                    int cap = Manager.GetSkillManager().GetLevelCap(abi.GetID());
                    int curLvl = Manager.GetSkillManager().GetCurrentSkillLevel(data);
                    //if(manager.CanAddLevel(data))
                    if (curLvl < cap)
                    {
                        skillManager.AddLevel(abi.GetID());
                    }
                }

                //Manager.GetAbilityManager().CreateAbility(Manager.GetAbilityManager().m_AbilityHardwire[2].m_ID, ae.transform);
                //Manager.GetAbilityManager().CreateAbility(Manager.GetAbilityManager().m_AbilityHackTarget[1].m_ID, ae.transform);
                //Manager.GetAbilityManager().CreateAbility(Manager.GetAbilityManager().m_AbilityHijack[1].m_ID, ae.transform);
            }
        }

        public void UnlockAll()
        {
            //Unlock all prototypes, blueprints, districts

            Manager.GetUIManager().ShowMessagePopup("Unlocking all prototypes, blueprints and relays");

            Manager.GetItemManager().UnlockAllPrototypes();
            Manager.GetItemManager().UnlockAllBlueprints();

            for (int i = 1; i < 8; i++)
                RelayBeaconManager.Get().ActivateAllForDistrict((District)i);

            Manager.GetUIManager().ShowMessagePopup("Unlocking all prototypes, blueprints, relays and skills");

            //Manager.GetSkillManager().AddAllSkills();

            var skillman = Manager.GetSkillManager();
            foreach(var aClass in AgentAI.AgentClasses)
            {
                //SkillManager.AgentData agentData = skillman.AgentDataList[(int)aClass];
                //agentData.m_XP += 300000;
                //agentData.m_CurrentLevel += 20;
                //agentData.m_AvailableSkillPoints += 15;
                //agentData.m_SkillPointsAcknowledged = true;
                skillman.AddXP(aClass, 500000);
            }

            Manager.GetUIManager().ShowMessagePopup("Unlocking all prototypes, blueprints, relays and skills, hacking all ATMs");
            Manager.GetMoneyManager().AllAtmsHacked();
            Manager.GetUIManager().ShowMessagePopup("Unlocked all prototypes, blueprints, relays and skills. Hacked all ATMs");
            Manager.GetMoneyManager().SetCurrentFunds(Manager.GetMoneyManager().GetCurrentFunds() + 50000);
            Manager.GetUIManager().ShowWarningPopup("Current funds " + Manager.GetMoneyManager().GetCurrentFunds(), 5);

        }

        //WeaponChanges
        // ************************ WeaponChanges ********************************************
        //WeaponChanges
        public void WeaponChanges()
        {
            weaponsUpdated = true;
            Manager.GetUIManager().ShowMessagePopup("Weapon changes starting");

            //Weapon weapon = Manager.GetWeaponManager().GetPrefab(WeaponType.B_UZI).m_Weapon;
            
            ////Weapon weapon = Manager.GetWeaponManager().GetPrefab(WeaponType.P_Minigun).m_Weapon;
            //if (weapon != null)
            //{
            //    Manager.GetUIManager().ShowMessagePopup("Weapon change minigun 1");
            //    weapon.m_Delayed = false;
            //    Manager.GetUIManager().ShowMessagePopup("Weapon change minigun 2");
            //    weapon.m_ChargeTime = 0;
            //    Manager.GetUIManager().ShowMessagePopup("Weapon change minigun 3");
            //    weapon.m_ChargeEveryShot = false;
            //    //weapon.
            //}
            //else
            //    Manager.GetUIManager().ShowMessagePopup("No prefab found");

            //weapon.ammo

            WeaponAttachmentAmmo weaponAttachmentAmmo = null;
            //int loop = 0;

            GameObject GO = Manager.GetWeaponManager().gameObject;
            WeaponManager weaponManager = GO.GetComponent<WeaponManager>();

            foreach (ItemManager.ItemData data in Manager.GetItemManager().GetAllItems())
            {
                //data.
            }

            Manager.GetUIManager().ShowMessagePopup("Setting all weapons to have range 60, infinite ammo, 100% accuracy");

            //foreach (WeaponData data in weaponManager.m_WeaponData)
            //{
            //    data.m_Range = 60;
            //    data.m_MinAccuracy = 1;
            //    data.m_MaxAccuracy = 1;
            //    data.m_InfiniteAmmo = true;
            //    data.m_Name = "Test_" + data.m_Name;
            //    data.m_ShootWhileChangeTarget = true;
                
            //    foreach (WeaponAttachmentAmmo ammo in data.m_Ammo)
            //    {
            //        ammo.m_ChargeTime = 0;
            //        ammo.m_damage_max = 20;
            //        ammo.m_damage_min = 15;
            //        ammo.m_DamageRadius = 10;
            //        ammo.m_knockback_amount = 10;
            //        ammo.m_max_ammo = 65;
            //        ammo.m_reload_time = 0;
            //        ammo.m_ChargeEveryShot = false;
            //        ammo.m_shield_damage = 20;
            //        ammo.m_CritChance = 1;
            //        ammo.m_AccuracyDelta = 1;
            //    }
            //}

            Manager.GetUIManager().ShowMessagePopup("Setting pistol range");
            weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Range = 200;
            Manager.GetUIManager().ShowMessagePopup("Setting pistol charge time. Ammo on pistol: " + weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Ammo.Length);
            weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Ammo[0].m_ChargeTime = 0;
            Manager.GetUIManager().ShowMessagePopup("Setting pistol charge every shot. Ammo on pistol: " + weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Ammo.Length + ". Ammo: " + weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Ammo[0].m_Type);
            weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Ammo[0].m_ChargeEveryShot = false;
            Manager.GetUIManager().ShowMessagePopup("Setting pistol reload speed. Ammo on pistol: " + weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Ammo.Length + ". Ammo: " + weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Ammo[0].m_Type);
            weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Ammo[(int)weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_DefaultAmmo].m_ReloadSpeed = 0;
            Manager.GetUIManager().ShowMessagePopup("Setting pistol reload time");
            weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Ammo[(int)weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_DefaultAmmo].m_reload_time = 0;

            //weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_MaxAccuracy = 1;
            //weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_MinAccuracy = 1;
            //weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Name = "BFG9000";
            
            weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Ammo[(int)weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_DefaultAmmo].m_Emp = 1;
            weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Ammo[(int)weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_DefaultAmmo].m_MaxBeamWidth = 1;
            weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Ammo[(int)weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_DefaultAmmo].m_knockback_amount = 20;
            weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Ammo[(int)weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_DefaultAmmo].m_ProjectilesPerShot = 20;

            Manager.GetUIManager().ShowMessagePopup("Pistol upgraded");

            List<int> abil = weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Abilities.ToList();

            //abil.Add(Manager.GetAbilityManager().m_AbilityStealth[0].m_ID);
            //abil.Add(Manager.GetAbilityManager().m_AbilityHardwire[2].m_ID);
            //abil.Add(Manager.GetAbilityManager().m_AbilityHackTarget[2].m_ID);
            abil.Add(1);
            abil.Add(2);

            //weaponManager.m_WeaponData[(int)WeaponType.].m_Abilities.ToList();
            

            weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Abilities = abil.ToArray();
            Manager.GetUIManager().ShowMessagePopup("Abilities added to pistol");

            //Manager.GetUIManager().ShowMessagePopup("Setting minigun range");
            //weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Range = 100;
            //Manager.GetUIManager().ShowMessagePopup("Setting minigun charge time. Ammo on minigun: " + weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo.Length);
            //weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo[0].m_ChargeTime = 0;
            //Manager.GetUIManager().ShowMessagePopup("Setting minigun charge every shot. Ammo on minigun: " + weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo.Length + ". Ammo: " + weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo[0].m_Type);
            //weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo[0].m_ChargeEveryShot = false;
            //Manager.GetUIManager().ShowMessagePopup("Setting minigun reload speed. Ammo on minigun: " + weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo.Length + ". Ammo: " + weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo[0].m_Type);
            //weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo[(int)weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_DefaultAmmo].m_ReloadSpeed = 0;
            //Manager.GetUIManager().ShowMessagePopup("Setting minigun reload time");
            //weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo[(int)weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_DefaultAmmo].m_reload_time = 0;
            ////Manager.GetUIManager().ShowMessagePopup("Setting minigun charge time");

            Manager.GetUIManager().ShowMessagePopup("Weapon changes done");


            //    foreach (WeaponData weaponData in weaponManager.m_WeaponData)
            //    {
            //        loop++;
            //        Manager.GetUIManager().ShowMessagePopup("Weapon changes in progress 1 : Loop " + loop);
            //        for (int i = 0; i < weaponData.m_Ammo.Length; i++)
            //        {
            //            if (weaponData.m_Ammo[i].m_Type == weaponData.m_DefaultAmmo)
            //            {
            //                weaponAttachmentAmmo = weaponData.m_Ammo[i];
            //            }
            //        }


            //        if (Manager.GetWeaponManager().m_WeaponData[(int)WeaponType.P_Minigun].m_Name == weaponData.m_Name)
            //        {
            //            //Manager.GetWeaponManager().GetPrefab(WeaponType.P_Minigun).m_Weapon
            //            //Manager.GetWeaponManager().GetAmmoData().

            //            Manager.GetUIManager().ShowMessagePopup("Weapon changes in progress 4 : Loop " + loop);
            //            var attachmentAmmo = Manager.GetWeaponManager().GetAmmoData(WeaponType.P_Minigun, weaponAttachmentAmmo.m_Type);
            //            Manager.GetUIManager().ShowMessagePopup("Weapon changes in progress 5 : Loop " + loop);
            //            //attachmentAmmo.m_reload_time = 0.1f;
            //            Manager.GetUIManager().ShowMessagePopup("Weapon changes in progress 6 : Loop " + loop);
            //            //attachmentAmmo.m_ChargeTime = 0.1f;
            //            Manager.GetUIManager().ShowMessagePopup("Weapon changes in progress 7 : Loop " + loop);
            //        }
            //        Manager.GetUIManager().ShowMessagePopup("Weapon changes in progress end of loop " + loop);
            //    }
            //    Manager.GetUIManager().ShowMessagePopup("Weapon changes done");
        }

        /// <summary>
        /// Sets name for this mod
        /// </summary>
        public string GetName()
        {
            return "Syndicate Mod";
        }
    }
}
