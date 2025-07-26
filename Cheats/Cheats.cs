using Cheats.CustomUI;
using Cheats.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cheats
{
    /// <summary>
    /// Satellite Reign cheats mod.
    /// </summary>
    public class Cheats : ISrPlugin
    {
        protected TimeManager.TimeScaler m_TacticalPauseTimeScaler;
        private bool cheatMode = false;
        private int numberOfWeaponTypes = Enum.GetValues(typeof(WeaponType)).Length;
        private float timer;

        // Achievement checker
        private float aTimer;

        public static List<CustomUIActions> UIActions;
        CarAI car = null;

        private static Cheats instance;

        public static Cheats GetInstance() 
        {
            if(instance == null)
            {
                instance = new Cheats();
                instance.Initialize();
            }
            return instance;
        }

        //public static event Action<bool> clickedOk;


        //public static event Func<string> writtenText;

        // AgentAI state debugging

        /// <summary>
        /// Plugin initialization 
        /// </summary>
        public void Initialize()
        {
            Debug.Log("Initializing Satellite Reign cheats mod");
            m_TacticalPauseTimeScaler = TimeManager.AddTimeScaler();
            cheatMode = false;
            timer = TimeManager.GameTime + 10;
            aTimer = TimeManager.GameTime + 30;
            //cctvs = new Dictionary<CctvCamera, District>();
            //ProgressionManager.Get().OnDistrictChange += RescanCameras();
            //ProgressionManager.OnDistrictChangeDelegate() += RescanCameras();

            //WeaponChanges();

            UIActions = new List<CustomUIActions>();

            UIActions.Add
            (
                new CustomUIActions
                (
                    "Add Weapons", 
                    delegate { WeaponChanges(); return true; }
                )
            );
            UIActions.Add
            (
                new CustomUIActions
                (
                    "Enter nearest vehicle",
                    delegate { EnterNearestCar(); return false; }
                )
            );
            UIActions.Add
            (
                new CustomUIActions
                (
                    "Control selected",
                    delegate { SwitchControlOnSelected(); return false; }
                )
            );
            UIActions.Add
            (
                new CustomUIActions
                (
                    "Cheatmode",
                    delegate 
                    { 
                        cheatMode = !cheatMode;
                        if(cheatMode == false)
                            foreach (AgentAI a in AgentAI.GetAgents())
                            {
                                a.m_Energy.SetInfiniteEnergy(false);
                                a.m_Health.Invulnerable = false;
                            }
                        var ui = setEntityInfo("Cheat mode off ", "Your agents can now be killed and lose energy again."); 
                        return cheatMode; 
                    }
                )
            );

            instance = this;
        }

        public static void ShowModUI()
        {
            List<SRModButtonElement> buttons = new List<SRModButtonElement>();

            var instance = GetInstance();

            buttons.AddRange(UIActions.Select
                (
                    a =>
                    {
                        string buttonText = "Activate " + a.Name;
                        string buttonDescription = a.Name;
                        if (!a.Active)
                            buttonText = "Deactivate " +a.Name;
                        var button = new SRModButtonElement(buttonText,
                            delegate
                            {
                                var thisButton = UIHelper.VerticalButtonsUi.Buttons.Where(b => b.Description == a.Name).First();

                                a.Action();

                                if (a.Active)
                                {
                                    thisButton.Text.text = "Activate " + a.Name;
                                    UIHelper.ShowMessage("Disabled " + a.Name, 10, 2);
                                }
                                else
                                {
                                    thisButton.Text.text = "Deactivate " + a.Name;
                                    UIHelper.ShowMessage("Enabled " + a.Name, 10, 2);
                                }
                                //thisButton.Text.SetAllDirty();
                                Canvas.ForceUpdateCanvases();
                                //ShowModUI();
                            }
                        , buttonDescription);
                        return button;
                    }
                ).ToList());
            Manager.Get().StartCoroutine(UIHelper.ModalVerticalButtonsRoutine("Cheat mod UI", buttons));
        }



        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            if (Manager.Get().GameInProgress)
            {
                //if (!displayedModInfo && TimeManager.GameTime > timer)
                //{
                //    displayedModInfo = true;
                //    timer = Time.time;
                //    ShowModsInfo();
                //    //Manager.GetUIManager().ShowMessagePopup("PopUpMessage", 7);
                //}

                if (cheatMode && timer < Time.time)
                {
                    timer = Time.time + 0.1f;
                    foreach (AgentAI a in AgentAI.GetAgents())
                    {
                        a.AddAmmo(1);
                        a.AddAmmo(5);
                        a.AddAmmo(10);
                        a.AddAmmo(20);
                        a.AddAmmo(50);
                        a.AddAmmo(100);
                        a.WantedTotal = 0;
                        a.NotifyWantedChanged();
                        //a.SetMoveSpeed(1f);
                        a.SetHealthValue(100);
                        a.m_Energy.AddEnergy(500);
                        a.m_Energy.SetInfiniteEnergy(true);
                        //a.m_Health.m_ShieldRegenRate = 100;
                        a.m_Health.SetHealthFull();
                        a.m_Health.SetAllFull();
                        a.m_Health.Init();
                        a.m_Health.Invulnerable = true;
                    }
                }

                /*
                // stuff to do in an interval
                if (aTimer < Time.time)
                {
                    //WeaponChanges();
                    aTimer = Time.time + 5;

                    string info = "";

                    SquadMemberState state = new SquadMemberState(AgentAI.GetAgent(AgentAI.AgentClass.Soldier).transform);

                    info += "ExitInfo: " + state.GetExitInfo().ToString();

                    info += ". Extra Debug State Info: " + state.ExtraDebugInfo();

                    info += " State name: " + state.GetName();

                    Manager.GetUIManager().ShowMessagePopup(info, 6);
                }
                */
                
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    ShowModUI();
                }

                if (Input.GetKeyDown(KeyCode.KeypadPlus))
                {
                    m_TacticalPauseTimeScaler.TimeScale += 0.5f;
                }
                if (Input.GetKeyDown(KeyCode.KeypadMinus) && m_TacticalPauseTimeScaler.TimeScale >= 0.5f)
                {
                    m_TacticalPauseTimeScaler.TimeScale -= 0.5f;
                }

                

                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    SwitchControlOnSelected();
                }

                if (Input.GetKeyDown(KeyCode.Keypad7))
                {
                    EnterNearestCar();
                }

                if (Input.GetKeyDown(KeyCode.Insert))
                {
                    MiscCheats();
                }
            }
        }

        private void MiscCheats()
        {
            string info = "";
            //Manager.GetMoneyManager().UpdateBankSiphonAmount(MoneyManager.Banks.CBDDistrict_1, 1);
            //Manager.GetMoneyManager().UpdateBankSiphonAmount(MoneyManager.Banks.CBDDistrict_2, 2);
            //Manager.GetMoneyManager().UpdateBankSiphonAmount(MoneyManager.Banks.CBDDistrict_3, 3);
            //Manager.GetMoneyManager().UpdateBankSiphonAmount(MoneyManager.Banks.RedLightDistrict_1, 4);
            //Manager.GetMoneyManager().UpdateBankSiphonAmount(MoneyManager.Banks.IndustrialDistrict_1, 5);
            //Manager.GetMoneyManager().UpdateBankSiphonAmount(MoneyManager.Banks.AfluentDistrict_1, 6);

            Manager.GetUIManager().ShowMessagePopup("Current funds " + Manager.GetMoneyManager().GetCurrentFunds(), 5);
            Manager.GetMoneyManager().SetCurrentFunds(1000000);
            //Manager.GetMoneyManager().SetCurrentFunds(640000);
            //Manager.GetMoneyManager().OffsetFunds(6400, true, true);
            //Manager.GetMoneyManager().SetCurrentFunds(666);

            //MoneyManager.UpdateSiphonAmount();
            foreach (AIEntity ae in Manager.GetInputManager().GetNonPlayerSelectedEntities())
            {
                info += " Entity: " + ae.GetName() + " selected.";

                int enabled = 0; int instant = 0; int passive = 0;
                int ready = 0; int allowed = 0; int remote = 0;
                foreach (ModifierData5L mData in ae.m_Modifiers.m_DefaultModifiers)
                {

                    info += "\nModifierType:" + mData.m_Type + " Ammount: " + mData.m_Ammount + " AmountModifier: " + mData.m_AmountModifier;
                }
                foreach (ModifierType mType in ae.m_Modifiers.UniqueModifiers())
                {
                    info += "\nModifierType:" + mType;
                }
                foreach (Ability abil in ae.GetAbilities().AllAbilities())
                {
                    info += "\nAbility: " + abil.m_Name + " id: " + abil.GetID() + " uid: " + abil.GetUID();
                }

                string[] aNames; int[] aIds;
                ae.GetAbilities().m_AbilityManager.GetAbilityNamesAndIDs(out aIds, out aNames);
                for (int i = 0; i < aIds.Length; i++)
                {
                    Ability abil = ae.GetAbilities().GetAbility(aIds[i]);
                    if (ae.GetAbilities().HasAbility(aIds[i]) || ae.m_Abilities.HasAbility(aIds[i]))
                        info += "\nId:" + aIds[i] + " Ability: " + aNames[i];
                    ae.ServerAddAbility(aIds[i]);
                }

                info += "\n" + enabled + " enabled. " + instant + " instant" + passive + " passive" + ready + " ready" + allowed + " allowed" + remote + " remote";
                setEntityInfo("Ability info of " + ae.GetName(), info);
                Manager.GetUIManager().ShowSubtitle(info, 9);
            }

            foreach (AgentAI a in AgentAI.GetAgents())
            {

                if (a.IsSelected())
                {
                    AddSkills(a);

                    a.m_Weapons.AddWeapon(WeaponType.P_Smg);
                    a.SetWeapon(10);
                    //a.m_Abilities.AllAbilities();
                    a.SkillUpdated();
                    //Manager.GetCamera().m_ScrollSpeed = 10;
                }
            }

            foreach (AgentAI a in AgentAI.GetAgents())
            {
                //List<Modifiers> modifiers = new List<Modifiers>();
                //modifiers.Add(new Modifiers().)
                ModifierData5L health = new ModifierData5L(); health.m_AmountModifier = ModifierType.HealthOffset;
                ModifierData5L energy = new ModifierData5L(); energy.m_AmountModifier = ModifierType.EnergyMaxOffset;
                ModifierData5L healthReg = new ModifierData5L(); healthReg.m_AmountModifier = ModifierType.HealthRegenRate;
                ModifierData5L energyReg = new ModifierData5L(); energyReg.m_AmountModifier = ModifierType.HealthOffset;
                ModifierData5L accuracy = new ModifierData5L(); accuracy.m_AmountModifier = ModifierType.AccuracyModifier;
                ModifierData5L accuracyAmount = new ModifierData5L(); accuracyAmount.m_AmountModifier = ModifierType.AccuracyOffset;
                ModifierData5L speed = new ModifierData5L(); speed.m_AmountModifier = ModifierType.SpeedOffset;
                ModifierData5L speedMult = new ModifierData5L(); speedMult.m_AmountModifier = ModifierType.SpeedMultiplier;

                if (a.IsSelected())
                {
                    //a.ServerAddAbility(5);
                    if (CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifiers().Count < 4)
                    {
                        CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Clear();
                        CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(health);
                        CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(energy);
                        CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(healthReg);
                        CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(energyReg);
                        CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(accuracy);
                        CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(accuracyAmount);
                        CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(speed);
                        CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(speedMult);
                    }

                    foreach (CloneableModifier cmod in CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifiers())
                    {
                        cmod.GetModifierData().m_Ammount = 100f;
                    }
                }
                //CloneManager.Get().GetCloneableData(a.CurrentCloneableId).AddModifiers(new Modifiers().m_DefaultModifiers.);
                //a.RespawnAtCurrentLocation();
            }

        }

        public void SwitchControlOnSelected()
        {
            foreach (AIEntity ae in UnityEngine.Object.FindObjectsOfType(typeof(AIEntity)))
            {
                if ((ae.IsHuman() || ae.IsMech()) && ae.IsSelected() && ae.m_Group != GroupID.Resistance)
                {
                    ae.m_IsIgnoringInput = false;
                    ae.m_IsControllable = !ae.m_IsControllable;
                    ae.m_HijackLevelRequired = 1;
                    //ae.SetHealthValue(0);
                    //ae.m_Health.HealthValue = 0;
                    //ae.m_Health.DisableShields();
                    if (ae.m_IsControllable)
                        Manager.GetUIManager().ShowMessagePopup("Now controlling " + ae.name + " " + ae.GetDescription(), 6);
                    else
                        Manager.GetUIManager().ShowMessagePopup("Gave up controlling " + ae.name + " " + ae.GetDescription(), 6);
                    //ae.name = "Caldor was here!";
                }
            }
        }

        public void EnterNearestCar()
        {
            Vector3 loc;
            Vector3 mLoc;
            Manager.GetInputControl().GetGroundPointUnderMouse(out mLoc);
            loc = mLoc;// AgentAI.FirstAgentAi().transform.position;
            float distToMouse = 5000f;

            if (loc == null || loc == Vector3.zero)
            {
                Manager.GetUIManager().ShowMessagePopup("Mouse needs to be be above a place in the map, to find a car near it.", 5);
                return;
            }
            else
                Manager.GetUIManager().ShowMessagePopup("Mouse location: " + mLoc + ". Soldier agent location: " + loc, 5);

            //Find cars closest to mouse.
            foreach (CarAI ca in UnityEngine.Object.FindObjectsOfType(typeof(CarAI)))
            {
                if (ca.IsAddedToWorld)
                {

                    ca.m_Selectable.m_IsIgnoringInput = false;
                    ca.m_IsControllable = true;
                    ca.m_IgnoreInput = false;


                    Manager.GetUIManager().ShowSubtitle("Checking distance to " + ca.transform.position, 5);
                    if ((ca.transform.position - loc).magnitude < distToMouse)
                    {
                        Manager.GetUIManager().ShowSubtitle("Found closer car, updating car.", 5);
                        car = ca;
                        distToMouse = Vector3.Distance(ca.transform.position, loc);
                        Manager.GetUIManager().ShowSubtitle("Distance updated", 5);
                    }
                }
            }

            if (car != null)
            {
                Manager.GetUIManager().ShowMessagePopup("Car has been found", 5);
                car.m_Selectable.SetSelected(false);
                car.m_Selectable.m_IsIgnoringInput = false;
                car.m_IgnoreInput = false;
                car.CurrentlySelected();
                car.m_Selectable.SetSelected(true);
                Manager.GetUIManager().ShowMessagePopup("The car " + car.GetName() + " is now selected", 5);

                int door = 0;
                foreach (AgentAI a in AgentAI.GetAgents())
                {
                    if (a.IsSelected())
                    {
                        if (a.m_InCar != null && a.m_InCar == car)
                            a.ExitCar(a.m_InCar.transform, door);
                        else if (door < 3) //ca.m_DoorsInUse
                        {
                            car.SetParked();
                            a.UseCar(car.transform, door);
                            Manager.GetUIManager().ShowMessagePopup(a.GetName() + " uses the car " + car.GetName(), 5);
                        }
                        else
                            Manager.GetUIManager().ShowWarningPopup(car.GetName() + " car is full. Doors in use: " + car.m_DoorsInUse, 5);
                        door++;
                    }
                }
                if (door == 0)
                {
                    Manager.GetUIManager().ShowMessagePopup("Car parking", 5);
                    //car.m_ParkInStreet = true;
                    car.SetParked();
                }
                car.m_Selectable.SetSelected(false);
                car.m_Selectable.m_IsIgnoringInput = false;
            }
            else
                Manager.GetUIManager().ShowMessagePopup("No car found", 5);
            //ShowModsInfo();
        }

        public void AddSkills(AgentAI a)
        {
            ModifierData5L skill = new ModifierData5L(); skill.m_Type = ModifierType.HackerLevel;
            skill.m_Ammount = 5;
            //if(a.m_Modifiers.GetModifier(ModifierType.HackerLevel) < 1)
            a.m_Modifiers.AddModifier(skill);
            skill = new ModifierData5L(); skill.m_Type = ModifierType.HardwireLevel;
            skill.m_Ammount = 4;
            //if (a.m_Modifiers.GetModifier(ModifierType.HardwireLevel) < 1)
            a.m_Modifiers.AddModifier(skill);
            skill = new ModifierData5L(); skill.m_Type = ModifierType.ThiefMoneyStealAmount;
            skill.m_Ammount = 5;
            //if (a.m_Modifiers.GetModifier(ModifierType.ThiefMoneyStealAmount) < 1)
            a.m_Modifiers.AddModifier(skill);
            skill = new ModifierData5L(); skill.m_Type = ModifierType.UseHighVent;
            skill.m_Ammount = 1;
            //if (a.m_Modifiers.GetModifier(ModifierType.UseHighVent) < 1)
            a.m_Modifiers.AddModifier(skill);
            skill = new ModifierData5L(); skill.m_Type = ModifierType.CanUseZipline;
            skill.m_Ammount = 1;
            //if (a.m_Modifiers.GetModifier(ModifierType.CanUseZipline) < 1)
            a.m_Modifiers.AddModifier(skill);
            skill = new ModifierData5L(); skill.m_Type = ModifierType.PoisonValveStrength;
            skill.m_Ammount = 1;
            //if (a.m_Modifiers.GetModifier(ModifierType.PoisonValveStrength) < 1)
            a.m_Modifiers.AddModifier(skill);
            skill = new ModifierData5L(); skill.m_Type = ModifierType.GearSlotIncrease;
            skill.m_Ammount = 1;
            //if (a.m_Modifiers.GetModifier(ModifierType.GearSlotIncrease) < 1)
            a.m_Modifiers.AddModifier(skill);
            skill = new ModifierData5L(); skill.m_Type = ModifierType.WeaponCountIncrease;
            skill.m_Ammount = 1;
            //if (a.m_Modifiers.GetModifier(ModifierType.WeaponCountIncrease) < 1)
            a.m_Modifiers.AddModifier(skill);
            skill = new ModifierData5L(); skill.m_Type = ModifierType.CanUseZipline;
            skill.m_Ammount = 1;
            //if (a.m_Modifiers.GetModifier(ModifierType.CanUseZipline) < 1)
            a.m_Modifiers.AddModifier(skill);
            skill = new ModifierData5L(); skill.m_Type = ModifierType.WeaponRangeMultiplier;
            skill.m_Ammount = 5;
            //if (a.m_Modifiers.GetModifier(ModifierType.WeaponRangeMultiplier) < 1)
            a.m_Modifiers.AddModifier(skill);
            skill = new ModifierData5L(); skill.m_Type = ModifierType.PerfectShot;
            skill.m_Ammount = 5;
            //if (a.m_Modifiers.GetModifier(ModifierType.PerfectShot) < 1)
            a.m_Modifiers.AddModifier(skill);
            skill = new ModifierData5L(); skill.m_Type = ModifierType.AbilityCooldownSpeedMultiplier;
            skill.m_Ammount = 5;
            //if (a.m_Modifiers.GetModifier(ModifierType.AbilityCooldownSpeedMultiplier) < 1)
            a.m_Modifiers.AddModifier(skill);
            skill = new ModifierData5L(); skill.m_Type = ModifierType.HijackerLevel;
            skill.m_Ammount = 5;
            //if (a.m_Modifiers.GetModifier(ModifierType.HijackerLevel) < 1)
            a.m_Modifiers.AddModifier(skill);
            //a.GetAbilities().m_AbilityManager.m_AbilityData[0].
            string[] aNames; int[] aIds;
            a.GetAbilities().m_AbilityManager.GetAbilityNamesAndIDs(out aIds, out aNames);

            string info = "";
            int enabled = 0; int instant = 0; int passive = 0;
            int ready = 0; int allowed = 0; int remote = 0;

            foreach (Ability abil in a.GetAbilities().AllAbilities())
            {
                info += "\nAbility: " + abil.m_Name + " id: " + abil.GetID() + " uid: " + abil.GetUID();
            }

            foreach (ModifierData5L mData in a.m_Modifiers.m_DefaultModifiers)
            {
                info += "\nModifierType:" + mData.m_Type + " Ammount: " + mData.m_Ammount + " AmountModifier: " + mData.m_AmountModifier;
            }
            for (int i = 0; i < aIds.Length; i++)
            {
                Ability abil = a.GetAbilities().GetAbility(aIds[i]);
                if (abil != null)
                {
                    if (abil.IsEnabled)
                        enabled++;
                    if (abil.isInstant)
                        instant++;
                    if (abil.IsEnabled)
                        passive++;
                    if (abil.IsReady())
                        ready++;
                    if (abil.IsAllowed())
                        allowed++;
                    if (abil.IsRemoteAbility())
                        remote++;
                    //if (abil.IsActive())
                }

                // && abil.IsAllowed())
                //if (a.GetAbilities().HasAbility(aIds[i]) || a.m_Abilities.HasAbility(aIds[i]))
                //    info += "\nId:" + aIds[i] + " Ability: " + aNames[i];
                a.ServerAddAbility(aIds[i]);
            }

            info += "\n" + enabled + " enabled. " + instant + " instant" + passive + " passive" + ready + " ready" + allowed + " allowed" + remote + " remote";
            setEntityInfo("Ability info of " + a.AgentClassName(), info);
            Manager.GetUIManager().ShowSubtitle(info, 9);
        }

        // Custom info shown in a entity popup
        public EntityInfoPanel setEntityInfo(string title, string info)
        {
            //UIEntityInterface uiei = ((UIEntityInterface)UIEntityInterface.FindObjectOfType(typeof(UIEntityInterface)));
            //uiei.m_selected = true;
            //uiei.m_AttachTracker = true;
            //uiei.m_ShowAgentUI = true;
            //uiei.m_SelectedText = info;
            //uiei.TargetVisible = true;
            //uiei.enabled = true;
            //uiei.SetSelected(true);

            AIEntity ai = null;
            foreach (AIEntity a in UnityEngine.Object.FindObjectsOfType(typeof(AIEntity)))
            {
                if (a.IsSelected() && !a.m_IsControllable)
                {
                    ai = a;
                }
                if (ai == null && a.IsAddedToWorld && !a.m_IsControllable)
                {
                    ai = a;
                }
            }
            ai.m_Selectable.SetSelected(false);
            ai.CurrentlySelected();
            //ai.SetSelected(true);
            ai.m_Selectable.SetSelected(true);

            EntityInfoPanel ui = (EntityInfoPanel)UnityEngine.Object.FindObjectOfType(typeof(EntityInfoPanel));
            //ui.SetAiEntity(ai);

            ui.m_DetailText.Text = info; // + "(GUI m_DetailText)";
            ui.m_DetailText.m_Text.text = info; // + "(GUI m_Text)";
            ui.name = title; // + "(GUI name)";
            ui.m_SummaryText.text = title; // + "(GUI m_SummaryText)";
                                           //ui.m_BuyButton.enabled = true;
            if (!ui.IsVisible())
                ui.Show();
            //AIEntityInfoUi aiui = (AIEntityInfoUi)AIEntityInfoUi.FindObjectOfType(typeof(AIEntityInfoUi));

            //ui.m_SummaryText.color = Color.red;

            //ai.m_Selectable.SetSelected(false);
            //ai.SetSelected(false);
            return ui;
        }

        //WeaponChanges
        // ************************ WeaponChanges ********************************************
        //WeaponChanges
        public void WeaponChanges()
        {
            Manager.GetUIManager().ShowMessagePopup("Weapon changes starting");

            Weapon weapon = Manager.GetWeaponManager().GetPrefab(WeaponType.P_Minigun).m_Weapon;
            if (weapon != null)
            {
                Manager.GetUIManager().ShowMessagePopup("Weapon change minigun 1");
                weapon.m_Delayed = false;
                Manager.GetUIManager().ShowMessagePopup("Weapon change minigun 2");
                weapon.m_ChargeTime = 0;
                Manager.GetUIManager().ShowMessagePopup("Weapon change minigun 3");
                weapon.m_ChargeEveryShot = false;
            }
            else
                Manager.GetUIManager().ShowMessagePopup("No prefab found");

            //weapon.ammo

            WeaponAttachmentAmmo weaponAttachmentAmmo = null;
            //int loop = 0;

            GameObject GO = Manager.GetWeaponManager().gameObject;
            WeaponManager weaponManager = GO.GetComponent<WeaponManager>();

            foreach (ItemManager.ItemData data in Manager.GetItemManager().GetAllItems())
            {
                //data.
            }

            foreach (WeaponData data in weaponManager.m_WeaponData)
            {
                data.m_Range = 60;
                data.m_ShootWhileChangeTarget = true;
                foreach (WeaponAttachmentAmmo ammo in data.m_Ammo)
                {
                    ammo.m_ChargeTime = 0;
                    ammo.m_damage_max = 20;
                    ammo.m_damage_min = 15;
                    ammo.m_DamageRadius = 10;
                    ammo.m_knockback_amount = 10;
                    ammo.m_max_ammo = 65;
                    ammo.m_reload_time = 0;
                    ammo.m_ChargeEveryShot = false;
                    ammo.m_shield_damage = 20;
                    ammo.m_CritChance = 1;
                    ammo.m_AccuracyDelta = 1;
                }
            }

            Manager.GetUIManager().ShowMessagePopup("Setting minigun range");
            weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Range = 100;
            Manager.GetUIManager().ShowMessagePopup("Setting minigun charge time. Ammo on minigun: " + weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo.Length);
            weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo[0].m_ChargeTime = 0;
            Manager.GetUIManager().ShowMessagePopup("Setting minigun charge every shot. Ammo on minigun: " + weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo.Length + ". Ammo: " + weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo[0].m_Type);
            weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo[0].m_ChargeEveryShot = false;
            Manager.GetUIManager().ShowMessagePopup("Setting minigun reload speed. Ammo on minigun: " + weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo.Length + ". Ammo: " + weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo[0].m_Type);
            weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo[(int)weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_DefaultAmmo].m_ReloadSpeed = 0;
            Manager.GetUIManager().ShowMessagePopup("Setting minigun reload time");
            weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_Ammo[(int)weaponManager.m_WeaponData[(int)WeaponType.P_Minigun].m_DefaultAmmo].m_reload_time = 0;
            //Manager.GetUIManager().ShowMessagePopup("Setting minigun charge time");

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

        public string GetName()
        {
            return "Cheats";
        }

        //Manager.GetUIManager().ShowWarningPopup("WarningPopup", 4);
        //Manager.GetUIManager().ShowSubtitle("These are subtitles, you need to learn how to use subtitles");
        //Manager.GetUIManager().HideSubtitle();
        //Manager.GetUIManager().ShowBannerMessage("Heading", "HeadingTop", "HeadingBottom", 5);
        //Manager.GetUIManager().DoModalMessageBox("Title", "Message", InputBoxUi.InputBoxTypes.MbOkcancel, "Yup","Nope", clickedOk);
        //Manager.GetUIManager().ShowMessagePopup("PopUpMessage", 3);
        //Manager.GetUIManager().ShowBlank("BlankText"); // Screen becomes blank showing only the text
        //Manager.GetUIManager().ToggleTextWindow("Toggle Text Window");

        /* OLD STUFF */


        /*

        private District activeDistrict;
        Dictionary<CctvCamera, District> cctvs;
        private int count;
        private bool displayedModInfo = false;
        string[] aNames; int[] aIds;
        private GameObject[] objects;
        string objectMode;
        int shaderNo = 0;

        public ProgressionManager.OnDistrictChangeDelegate RescanCameras()
        {
            Manager.GetUIManager().ShowMessagePopup("Changed district to: " + Manager.GetProgressionManager().CurrentDistrict + ". Rescanning cameras", 60);
            ProgressionManager.Get().OnDistrictChange += RescanCameras();
            return null;
        }

        public void NextModInfo(bool b)
        {
            //SceneManager.LoadSceneAsync("scene", LoadSceneMode.Additive);
            //SceneManager.GetActiveScene().name;
            string path = "not found";
            //foreach (var res in ResourceDB.GetAllAssets("Cheats"))
            //{
            //    if(res.ResourcesPath != null && res.ResourcesPath != "")
            //        path = res.Path + " and " + res.ResourcesPath;
            //}
            //string path = AssetDatabase.GetAssetPath(this);
            string info = "";
            string titel = "";
            if (b)
                if (count == 0)
                {
                    titel = "UnStuck Mod info";
                    info += @"**Key: F11 or minus ** action: Helps your agents get unstuck.";
                    info += @" Current active scene name: " + SceneManager.GetActiveScene().name;
                    info += @" Current active scene path: " + SceneManager.GetActiveScene().path;
                    info += @" Scene count: " + SceneManager.sceneCount + " and scene count in build set " + SceneManager.sceneCountInBuildSettings;
                    info += @" Scene 1 name: " + SceneManager.GetSceneAt(0).name + " and scene 2 name " + SceneManager.GetSceneAt(1).name;
                }
                else if (count == 1)
                {
                    titel = "Change Looks Mod info";
                    info += @" Scene 3 name: " + SceneManager.GetSceneByName("LightScene").name + " and script name " + GetName() + " path " + path +
                            " full type name " + GetType().FullName + " base type name " + GetType().BaseType.Name;


                    info += @"**Key: M** action: Changes the sex of any selected agent.
**Key: End** action: All selected agents become female and increase the randomseed by 10.
**Key: Keypad 8 * *action: Selected agents gets a random sex and body.
**Key: Keypad 9 * *action: Shows some information about selected agents.
**Key: Keypad 1 - 3 * *action: Saves the seed / body of the selected agent or civilian.
**Key: Keypad 4 - 6 * *action: Applies settings saved by keypad 1 - 3 respectively.
**Key: Keypad 0 or home **action: home saves the seeds of every agent, and 0 reverts to those seeds.";
                }
                else if (count == 2)
                {
                    titel = "Cheats Mod info";
                    info += @"**Key: Backspace** action: Takes or loses control of any human or mech you have selected.
**Key: Insert** action: Gives any selected agent a skill boost and a plasma rifle.
**Key: N** action: Boosts the current modifiers of selected agents clones.
**Key: Page Up and Page Down** action: Switch selects agents weapon id up or down.
**Key: Del** action: Enables cheat mode.All agents will be unable to die or lose energy.
**Key: keypad plus and minus **action: Pauses increases or decreases the speed of the game by 0.5;
**Key: Keypad 7 * *action: An attempt to select a car and a second press commands agents to use it.";
                }
                else
                {
                    count = 0;
                    return;
                }
            else
            {
                count = 0;
                return;
            }
            count++;
            Manager.GetUIManager().DoModalMessageBox(titel, info, InputBoxUi.InputBoxTypes.MbOkcancel, "Ok", "Cancel", clickedOk);
        }

        public string InputText()
        {
            //clickedOk = NextObjectInfo;
            //if (method == "Hej")
            {
                inputText = Manager.GetUIManager().InputBoxUi.InputText;
                Manager.GetUIManager().ShowMessagePopup(inputText);
            }
            return inputText;
        }

        public void ShowModsInfo()
        {
            string info = "";
            info += @"**Key: Pause ** action: Pauses game tactically.";

            //Manager.GetUIManager().ShowWarningPopup(info, 15);
            count = 0;
            clickedOk = NextModInfo;
            Manager.GetUIManager().DoModalMessageBox("Tactical Pause Mod info", info, InputBoxUi.InputBoxTypes.MbOkcancel, "OK", "Cancel", clickedOk);
        } 
         
        public void OldStuff()
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                foreach (AgentAI a in AgentAI.GetAgents())
                {
                    if (a.IsSelected())
                    {
                        bool weaponOut = a.HasWeaponOut();
                        a.m_Weapons.NextWeaponCheat(1);
                        a.SetWeapon(10);
                        if (!weaponOut)
                            a.PutWeaponAway(true);
                    }
                }
                //ShowModsInfo();
                //clickedOk += DoStuff;
                //Manager.GetUIManager().DoModalMessageBox("Title", "Message", InputBoxUi.InputBoxTypes.MbOkcancel, "Yup","Nope", clickedOk);
            }

            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                //Manager.GetAIWorld().m_AssassinPrefab.m_Wardrobe.m_WardrobeType = WardrobeManager.WardrobeType.AgentAssassinBacker;
                //Manager.GetUIManager().ShowWarningPopup("WarningPopup", 4);

                string info = "";
                //foreach (CivilianAI cAI in Manager.GetAIWorld().CivTemplates)
                //    info += cAI.name + "\n" + cAI.GetName() + "\n";
                //info += Manager.GetAIWorld().CivTemplates.Length + " Civ Templates\n";
                //Manager.GetAIWorld().DebugSpawnRandomCivAtMouse();

                //ProgressionManager.Get().UpdateLocationText();

                //IdentityManager.Get().GetName(1, out first, out last);
                //info += "Name with ID 1: " + first + " " + last + "\n";

                //foreach (CivilianAI cAI in Manager.GetAIWorld().CivTemplates)
                //{

                //}
                foreach (AgentAI a in AgentAI.GetAgents())
                {
                    bool weaponOut = a.HasWeaponOut();

                    if (a.IsSelected())
                    {
                        a.m_Weapons.NextWeaponCheat(-1);
                        a.SetWeapon(10);
                        if (!weaponOut)
                            a.PutWeaponAway(true);
                        info += "\nChanging to " + a.GetWeaponType(0).ToString();
                        //Manager.GetUIManager().ShowMessagePopup("Weapon changed", 3);
                    }

                    //Manager.GetInputControl().AddPlayerEntity(a.Selectable);
                }
                Manager.GetUIManager().ShowWarningPopup(info, 10);

                setEntityInfo("Info about game", info);
                //Manager.GetUIManager().ToggleTextWindow(info);

                //Manager.GetUIManager().ShowSubtitle("These are subtitles, you need to learn how to use subtitles");
                //Manager.GetUIManager().HideSubtitle();
                //Manager.Get().useGUILayout = true;
                //Manager.Get().PostInfo("Some condition", "Some stacktrace");
                //Manager.GetDebugText().text = "Some debug text";
            }

            //if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.Asterisk) || Input.GetKeyDown(KeyCode.Home))
            //    ChangeShader();

            //Get current weapon info
            if (Input.GetKeyDown(KeyCode.Period))// || Input.GetKeyDown(KeyCode.N))
            {
                string info = "";

                AgentAI currentAgent = AgentAI.FirstSelectedAgentAi();
                Weapon weapon = currentAgent.GetWeapon();
                info += "Reload speed " + weapon.GetReloadSpeed() + ". Reload Time " + weapon.GetReloadTime();
                info += " Charge time " + weapon.m_ChargeTime + " Range " + weapon.GetRange();
                info += ". Is Delayed " + weapon.m_Delayed + ". Shoot while change target " + weapon.m_ShootWhileChangeTarget;
                //info += ". Weapon: "+ weapon.Name();
                //info += "";
                //WeaponAttachmentAmmo weaponAttachmentAmmo = null;
                //foreach (WeaponData weaponData in Manager.GetWeaponManager().m_WeaponData)
                //{
                //    for (int i = 0; i < weaponData.m_Ammo.Length; i++)
                //    {
                //        if (weaponData.m_Ammo[i].m_Type == weaponData.m_DefaultAmmo)
                //        {
                //            weaponAttachmentAmmo = weaponData.m_Ammo[i];
                //        }
                //    }
                //    if (Manager.GetWeaponManager().m_WeaponData[(int)WeaponType.P_Minigun].m_Name == weaponData.m_Name)
                //    {
                //        var attachmentAmmo = Manager.GetWeaponManager().GetAmmoData(WeaponType.P_Minigun, weaponAttachmentAmmo.m_Type);
                //        attachmentAmmo.m_ReloadSpeed *= 0.6f;
                //        attachmentAmmo.m_ChargeTime *= 0.2f;
                //    }
                //}


                setEntityInfo("Weapon " + weapon.Name(), info);
                WeaponChanges();


                weapon.m_Delayed = false;

                weapon.m_ChargeTime = 0;
                //Manager.GetUIManager().ShowMessagePopup("Weapon change minigun 3");
                weapon.m_ChargeEveryShot = false;

                //if (weapon.m_Physical == null)
                //    Manager.GetUIManager().ShowMessagePopup("No prefab on weapon");
                //else if (weapon.m_Physical.m_Weapon == null)
                //    Manager.GetUIManager().ShowMessagePopup("No weapon on prefab");
                //else
                //{
                //    weapon = weapon.m_Physical.m_Weapon;

                //    weapon.m_Delayed = false;
                //    weapon.m_ChargeTime = 0;
                //    weapon.m_ChargeEveryShot = false;
                //    weapon.m_ShootWhileChangeTarget = true;


                //    Manager.GetUIManager().ShowMessagePopup("Weapon prefab changed");
                //}



                // game scene name: SRWorldStreamFull  loader name: GameLoader
                // Did not work. Tried to change shader and scene.
                //setEntityInfo("Creating scene","Will it work?");
                //Scene myScene = new Scene();
                //string info = "";

                //if (!SceneManager.GetSceneByName("LightScene").IsValid())
                //{
                //    myScene = SceneManager.GetSceneByPath("Assets/Scenes/LightScene.unity");
                //    setEntityInfo("Found valid scene", "LightScene found? " + myScene.name + " maybe?");
                //    //myScene = SceneManager.GetSceneByName("LightScene");
                //    Manager.GetUIManager().ShowMessagePopup("LightScene is " + myScene.name + " and " + myScene.path, 20);
                //    //SceneManager.LoadSceneAsync("LightScene", LoadSceneMode.Additive);

                //    myScene = SceneManager.GetSceneByPath("LightScene.unity");
                //    setEntityInfo("Found valid scene in root", "LightScene found and added?" + myScene.name);
                //    //SceneManager.SetActiveScene(SceneManager.GetSceneByName("LightScene"));
                //    Manager.GetUIManager().ShowMessagePopup("LightScene is still not found");
                //}

                //if (!SceneManager.GetSceneByName("SomeTest").IsValid())
                //{
                //    setEntityInfo("Creating scene SomeTest", "Will it work?");
                //    myScene = SceneManager.CreateScene("SomeTest");
                //}
                //else if (!SceneManager.GetSceneByName("SomeTest").isLoaded)
                //{
                //    setEntityInfo("Loading scene", "Loading SomeTest additive");
                //    SceneManager.LoadSceneAsync("SomeTest", LoadSceneMode.Additive);
                //    setEntityInfo("Loaded scene", "Loaded SomeTest additive?");
                //    myScene = SceneManager.CreateScene("SomeTest");
                //    SceneManager.LoadScene("SomeTest", LoadSceneMode.Additive);
                //    setEntityInfo("Loaded scene?", "Loaded SomeTest additive??");
                //    //SceneManager.SetActiveScene(SceneManager.GetSceneByName("SomeTest"));
                //}
                //else if (SceneManager.GetActiveScene().name == "GameLoader")
                //{
                //    myScene = SceneManager.GetSceneByName("SomeTest");
                //    string text = "Is valid? " + myScene.IsValid();
                //    setEntityInfo("SomeTest " + text, "Making the SomeTest.scene the active scene");
                //    SceneManager.SetActiveScene(SceneManager.GetSceneByName("SomeTest"));

                //}
                //else if (SceneManager.GetActiveScene().name == "SomeTest")
                //{
                //    myScene = SceneManager.GetActiveScene();
                //    setEntityInfo("Active", "Active scene is " + myScene.name + " path " + myScene.path +". Also... unloading city scene...");
                //    SceneManager.SetActiveScene(SceneManager.GetSceneByName("SRWorldStreamFull"));
                //    myScene = SceneManager.GetActiveScene();
                //    Manager.GetUIManager().ShowMessagePopup("WorldStream has " + myScene.GetRootGameObjects().Length + " gameobjects and its path is " + myScene.path, 20);

                //    count = 0;
                //    clickedOk = NextObjectInfo;

                //    objects = myScene.GetRootGameObjects();
                //    //foreach(var obj in objects)
                //    //{
                //    //    info += " :: " + obj.name;
                //    //}

                //    Manager.GetUIManager().DoModalMessageBox("SRWorldStreamFull objects", info, InputBoxUi.InputBoxTypes.MbOkcancel, "OK", "Cancel", clickedOk);
                //    //SceneManager.UnloadScene(1);
                //}
                //else if (SceneManager.GetActiveScene().name == "SRWorldStreamFull")
                //{
                //    writtenText = InputText;

                //    myScene = SceneManager.GetActiveScene();
                //    Manager.GetUIManager().ShowMessagePopup("WorldStream has " + myScene.GetRootGameObjects().Length + " gameobjects and its path is " + myScene.path, 20);

                //    count = 0;
                //    clickedOk = NextObjectInfo;

                //    objects = myScene.GetRootGameObjects();

                //    Manager.GetUIManager().DoModalMessageBox("SRWorldStreamFull objects", info, InputBoxUi.InputBoxTypes.MbInputOk, "OK", "Cancel", clickedOk, writtenText);
                //}
                
                //setEntityInfo("End of method", "Last scene in method " + myScene.name + " path " + myScene.path);

                //if(SceneManager.GetSceneByPath("Assets/Scenes/LightScene.unity").IsValid())
                //{
                //    setEntityInfo("Valid", "Last scene in method " + myScene.name + " path " + myScene.path);
                //    myScene = SceneManager.GetSceneByPath("Assets/Scenes/LightScene.unity");
                //    setEntityInfo("Valid", "Last scene in method " + myScene.name + " path " + myScene.path);
                //}
            }
    
            if (Input.GetKeyDown(KeyCode.Dollar) || Input.GetKeyDown(KeyCode.Comma))
            {
                string commands = "Your wish, master? Commands: exit, activesc 0, activesc 1, listobjects,\n listobject #, repath, listcamera, ssrcamera";
                objectMode = "";
                // game scene name: SRWorldStreamFull  loader name: GameLoader
                setEntityInfo("Command Interface", commands);
                inputText = "";

                clickedOk = CommandInterface;
                CommandInterface(false);
            }

            //if (Input.GetKeyDown(KeyCode.N))
            //{
            //    foreach (AgentAI a in AgentAI.GetAgents())
            //    {
            //        //List<Modifiers> modifiers = new List<Modifiers>();
            //        //modifiers.Add(new Modifiers().)
            //        ModifierData5L health = new ModifierData5L(); health.m_AmountModifier = ModifierType.HealthOffset;
            //        ModifierData5L energy = new ModifierData5L(); energy.m_AmountModifier = ModifierType.EnergyMaxOffset;
            //        ModifierData5L healthReg = new ModifierData5L(); healthReg.m_AmountModifier = ModifierType.HealthRegenRate;
            //        ModifierData5L energyReg = new ModifierData5L(); energyReg.m_AmountModifier = ModifierType.HealthOffset;
            //        ModifierData5L accuracy = new ModifierData5L(); accuracy.m_AmountModifier = ModifierType.AccuracyModifier;
            //        ModifierData5L accuracyAmount = new ModifierData5L(); accuracyAmount.m_AmountModifier = ModifierType.AccuracyOffset;
            //        ModifierData5L speed = new ModifierData5L(); speed.m_AmountModifier = ModifierType.SpeedOffset;
            //        ModifierData5L speedMult = new ModifierData5L(); speedMult.m_AmountModifier = ModifierType.SpeedMultiplier;

            //        if (a.IsSelected())
            //        {
            //            //a.ServerAddAbility(5);
            //            if (CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifiers().Count < 4)
            //            {
            //                CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Clear();
            //                CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(health);
            //                CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(energy);
            //                CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(healthReg);
            //                CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(energyReg);
            //                CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(accuracy);
            //                CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(accuracyAmount);
            //                CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(speed);
            //                CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifierDatas().Add(speedMult);
            //            }

            //            foreach (CloneableModifier cmod in CloneManager.Get().GetCloneableData(a.CurrentCloneableId).GetModifiers())
            //            {
            //                cmod.GetModifierData().m_Ammount = 100f;
            //            }
            //        }
            //        //CloneManager.Get().GetCloneableData(a.CurrentCloneableId).AddModifiers(new Modifiers().m_DefaultModifiers.);
            //        //a.RespawnAtCurrentLocation();
            //    }
            //    string info = "";
            //    AgentAI.FirstAgentAi().GetAbilities().m_AbilityManager.GetAbilityNamesAndIDs(out aIds, out aNames);
            //    for (int i = 0; i < aIds.Length; i++)
            //    {
            //        info += "\nId:" + aIds[i] + " Ability: " + aNames[i];
            //    }
            //    count = 0;
            //    ShowAbilityInfo();
            //}


            //string info = "";
            //AgentAI.FirstAgentAi().GetAbilities().m_AbilityManager.GetAbilityNamesAndIDs(out aIds, out aNames);
            //for (int i = 0; i < aIds.Length; i++)
            //{
            //    info += "\nId:" + aIds[i] + " Ability: " + aNames[i];
            //}
            //count = 0;
            //ShowAbilityInfo();
            //if(info != "")
            //    Manager.GetUIManager().DoModalMessageBox("Ability info", info, InputBoxUi.InputBoxTypes.MbOk, "Ok", "Cancel");

            //Manager.GetUIManager().ShowSubtitle("Model Preview", 4);
            //Manager.GetUIManager().m_ModelPreviewManager.enabled = true;
            //int cloneId = AgentAI.FirstAgentAi().CurrentCloneableId;
            //Manager.GetUIManager().ShowSubtitle("Getting clonedata", 4);
            //CloneableData cData = CloneManager.Get().GetCloneableData(cloneId);
            //Manager.GetUIManager().ShowSubtitle("Preview clone", 4);
            //Manager.GetUIManager().m_ModelPreviewManager.PreviewCloneable(cData);
            ////Manager.GetUIManager().m_ModelPreviewManager.
            //if (Manager.GetUIManager().m_ModelPreviewManager.ReadyToRender())
            //    Manager.GetUIManager().ShowSubtitle("Preview ready to render", 4);
            //else
            //    Manager.GetUIManager().ShowSubtitle("Preview not ready to render", 4);

            //Manager.GetUIManager().ShowWarningPopup("WarningPopup, is preview showing?", 4);
            //Manager.GetUIManager().m_ModelPreviewManager.
            //Manager.GetUIManager().ShowWarningPopup("WarningPopup", 4);
            //Manager.GetUIManager().ShowBannerMessage("Heading", "HeadingTop", "HeadingBottom", 5);
        }
        */
    }
}