using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Satellite Reign cheats mod.
/// </summary>
public class Cheats : ISrPlugin
{
    protected TimeManager.TimeScaler m_TacticalPauseTimeScaler;
    private bool cheatMode = false;
    private int numberOfWeaponTypes = System.Enum.GetValues(typeof(WeaponType)).Length;
    private float timer;

    // Achievement checker
    private float aTimer;
    private District activeDistrict;
    Dictionary<CctvCamera, District> cctvs;

    public static event Action<bool> clickedOk;
    private int count;
    private bool displayedModInfo = false;
    CarAI car = null;
    string[] aNames; int[] aIds;
    private GameObject[] objects;
    string objectMode;
    int shaderNo = 0;
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
    }

    public ProgressionManager.OnDistrictChangeDelegate RescanCameras()
    {
        Manager.GetUIManager().ShowMessagePopup("Changed district to: " + Manager.GetProgressionManager().CurrentDistrict + ". Rescanning cameras", 60);
        ProgressionManager.Get().OnDistrictChange += RescanCameras();
        return null;
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
        if (Manager.Get().GameInProgress)
        {
            if (!displayedModInfo && TimeManager.GameTime > timer)
            {
                displayedModInfo = true;
                timer = Time.time;
                ShowModsInfo();
                //Manager.GetUIManager().ShowMessagePopup("PopUpMessage", 7);
            }
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

            // stuff to do in an interval
            if(aTimer < Time.time)
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

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                cheatMode = !cheatMode;
                if (!cheatMode)
                {
                    foreach (AgentAI a in AgentAI.GetAgents())
                    {
                        a.m_Energy.SetInfiniteEnergy(false);
                        a.m_Health.Invulnerable = false;
                        Manager.GetUIManager().ShowMessagePopup("Cameras in this district " + ProgressionManager.Get().GetCameraDistrictCount() + " Devs cloned:" + ProgressionManager.Get().m_DevsCloned + " Devs killed:" + ProgressionManager.Get().m_DevsKilled, 20);
                        ProgressionManager.Get().GetCameraDistrictCount();
                        
                    }

                    var ui = setEntityInfo("Cheat mode off ", "Your agents can now be killed and lose energy again.");
                }
            }

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                m_TacticalPauseTimeScaler.TimeScale += 0.5f;
            }
            if (Input.GetKeyDown(KeyCode.KeypadMinus) && m_TacticalPauseTimeScaler.TimeScale >= 0.5f)
            {
                m_TacticalPauseTimeScaler.TimeScale -= 0.5f;
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

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                //AgentAI.FirstSelectedAgentAi().SetHealthValue(Object.FindObjectsOfType(typeof(AIEntity)).Length);

                foreach (AIEntity ae in AIEntity.FindObjectsOfType(typeof(AIEntity)))
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

                //Manager.GetUIManager().ShowMessagePopup("PopUpMessage", 3);
                //Manager.GetUIManager().ShowBlank("BlankText"); // Screen becomes blank showing only the text
                //Manager.GetUIManager().ToggleTextWindow("Toggle Text Window");
            }

            if (Input.GetKeyDown(KeyCode.Keypad7))
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
                foreach (CarAI ca in CarAI.FindObjectsOfType(typeof(CarAI)))
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

            if (Input.GetKeyDown(KeyCode.Insert))
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

            //Show mod info
            /*
            if (Input.GetKeyDown(KeyCode.RightWindows) || Input.GetKeyDown(KeyCode.RightCommand) || Input.GetKeyDown(KeyCode.RightAlt) || Input.GetKeyDown(KeyCode.Pause))
                ShowModsInfo();
            */

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


                setEntityInfo("Weapon "+ weapon.Name(), info);
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
                /* Did not work. Tried to change shader and scene.
                setEntityInfo("Creating scene","Will it work?");
                Scene myScene = new Scene();
                string info = "";

                if (!SceneManager.GetSceneByName("LightScene").IsValid())
                {
                    myScene = SceneManager.GetSceneByPath("Assets/Scenes/LightScene.unity");
                    setEntityInfo("Found valid scene", "LightScene found? " + myScene.name + " maybe?");
                    //myScene = SceneManager.GetSceneByName("LightScene");
                    Manager.GetUIManager().ShowMessagePopup("LightScene is " + myScene.name + " and " + myScene.path, 20);
                    //SceneManager.LoadSceneAsync("LightScene", LoadSceneMode.Additive);

                    myScene = SceneManager.GetSceneByPath("LightScene.unity");
                    setEntityInfo("Found valid scene in root", "LightScene found and added?" + myScene.name);
                    //SceneManager.SetActiveScene(SceneManager.GetSceneByName("LightScene"));
                    Manager.GetUIManager().ShowMessagePopup("LightScene is still not found");
                }

                if (!SceneManager.GetSceneByName("SomeTest").IsValid())
                {
                    setEntityInfo("Creating scene SomeTest", "Will it work?");
                    myScene = SceneManager.CreateScene("SomeTest");
                }
                else if (!SceneManager.GetSceneByName("SomeTest").isLoaded)
                {
                    setEntityInfo("Loading scene", "Loading SomeTest additive");
                    SceneManager.LoadSceneAsync("SomeTest", LoadSceneMode.Additive);
                    setEntityInfo("Loaded scene", "Loaded SomeTest additive?");
                    myScene = SceneManager.CreateScene("SomeTest");
                    SceneManager.LoadScene("SomeTest", LoadSceneMode.Additive);
                    setEntityInfo("Loaded scene?", "Loaded SomeTest additive??");
                    //SceneManager.SetActiveScene(SceneManager.GetSceneByName("SomeTest"));
                }
                else if (SceneManager.GetActiveScene().name == "GameLoader")
                {
                    myScene = SceneManager.GetSceneByName("SomeTest");
                    string text = "Is valid? " + myScene.IsValid();
                    setEntityInfo("SomeTest " + text, "Making the SomeTest.scene the active scene");
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName("SomeTest"));
                    
                }
                else if (SceneManager.GetActiveScene().name == "SomeTest")
                {
                    myScene = SceneManager.GetActiveScene();
                    setEntityInfo("Active", "Active scene is " + myScene.name + " path " + myScene.path +". Also... unloading city scene...");
                    SceneManager.SetActiveScene(SceneManager.GetSceneByName("SRWorldStreamFull"));
                    myScene = SceneManager.GetActiveScene();
                    Manager.GetUIManager().ShowMessagePopup("WorldStream has " + myScene.GetRootGameObjects().Length + " gameobjects and its path is " + myScene.path, 20);

                    count = 0;
                    clickedOk = NextObjectInfo;
                    
                    objects = myScene.GetRootGameObjects();
                    //foreach(var obj in objects)
                    //{
                    //    info += " :: " + obj.name;
                    //}
                    
                    Manager.GetUIManager().DoModalMessageBox("SRWorldStreamFull objects", info, InputBoxUi.InputBoxTypes.MbOkcancel, "OK", "Cancel", clickedOk);
                    //SceneManager.UnloadScene(1);
                }
                else if (SceneManager.GetActiveScene().name == "SRWorldStreamFull")
                {
                    writtenText = InputText;

                    myScene = SceneManager.GetActiveScene();
                    Manager.GetUIManager().ShowMessagePopup("WorldStream has " + myScene.GetRootGameObjects().Length + " gameobjects and its path is " + myScene.path, 20);

                    count = 0;
                    clickedOk = NextObjectInfo;

                    objects = myScene.GetRootGameObjects();

                    Manager.GetUIManager().DoModalMessageBox("SRWorldStreamFull objects", info, InputBoxUi.InputBoxTypes.MbInputOk, "OK", "Cancel", clickedOk, writtenText);
                }
                */
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
        }
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
            if(abil != null)
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
        setEntityInfo("Ability info of " +a.AgentClassName(), info);
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
        foreach (AIEntity a in AIEntity.FindObjectsOfType(typeof(AIEntity)))
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
        
        EntityInfoPanel ui = (EntityInfoPanel)EntityInfoPanel.FindObjectOfType(typeof(EntityInfoPanel));
        //ui.SetAiEntity(ai);
        
        ui.m_DetailText.Text = info; // + "(GUI m_DetailText)";
        ui.m_DetailText.m_Text.text = info; // + "(GUI m_Text)";
        ui.name = title; // + "(GUI name)";
        ui.m_SummaryText.text = title; // + "(GUI m_SummaryText)";
        //ui.m_BuyButton.enabled = true;
        if(!ui.IsVisible())
            ui.Show();
        //AIEntityInfoUi aiui = (AIEntityInfoUi)AIEntityInfoUi.FindObjectOfType(typeof(AIEntityInfoUi));

        //ui.m_SummaryText.color = Color.red;

        //ai.m_Selectable.SetSelected(false);
        //ai.SetSelected(false);
        return ui;
    }

    event Func<string> writtenText;
    string inputText;

    public void DoStuff(bool b)
    {
        writtenText = InputText;
        clickedOk = NextObjectInfo;
        if (b)
            setEntityInfo("You clicked okay", "Your are now better at modding.");
        //else

        Manager.GetUIManager().DoModalMessageBox("Title", inputText, InputBoxUi.InputBoxTypes.MbInputOk, "Yup", "Nope", clickedOk, writtenText);
    }

    //WeaponChanges
    // ************************ WeaponChanges ********************************************
    //WeaponChanges
    public void WeaponChanges()
    {
        Manager.GetUIManager().ShowMessagePopup("Weapon changes starting");

        

        Weapon weapon = Manager.GetWeaponManager().GetPrefab(WeaponType.P_Minigun).m_Weapon;
        if(weapon != null)
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

        foreach(ItemManager.ItemData data in Manager.GetItemManager().GetAllItems())
        {
            //data.
        }

        foreach(WeaponData data in weaponManager.m_WeaponData)
        {
            data.m_Range = 60;
            data.m_ShootWhileChangeTarget = true;
            foreach(WeaponAttachmentAmmo ammo in data.m_Ammo)
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

    public void ShowAbilityInfo()
    {
        string info = "";
        string titel = "";
        if (aIds != null && aIds.Length > 0 && count < aIds.Length)
        {
            int toShow = aIds.Length - count;
            if (toShow > 7)
                toShow = 7;
            titel = "Ability# " + count + " to " + (count + toShow) + " info.";
            for (int i = count; i < count + toShow; i++)
            {
                info += "\nId:" + aIds[i] + " Ability: " + aNames[i];
            }
            count = count + toShow;
        }
        else
        {
            count = 0;
            return;
        }
        info += "\nAbilities total: " + aIds.Length;
        clickedOk = NextAbilityInfo;
        Manager.GetUIManager().DoModalMessageBox(titel, info, InputBoxUi.InputBoxTypes.MbOkcancel, "Ok", "Cancel", clickedOk);
    }

    public void NextAbilityInfo(bool b)
    {
        string info = "";
        string titel = "";
        if (aIds != null && aIds.Length > 0 && count < aIds.Length && b)
        {
            int toShow = aIds.Length - count;
            if (toShow > 7)
                toShow = 7;
            titel = "Ability# " + count + " to " + (count + toShow) + " info.";
            for(int i = count; i < count+toShow; i++)
            {
                info += "\nId:" + aIds[i] + " Ability: " + aNames[i];
            }
            count = count + toShow;
        }
        else
        {
            count = 0;
            return;
        }
        
        Manager.GetUIManager().DoModalMessageBox(titel, info, InputBoxUi.InputBoxTypes.MbOkcancel, "Ok", "Cancel", clickedOk);
    }

    public void ChangeRenderPath()
    {
        if(Camera.main.renderingPath == RenderingPath.DeferredLighting)
        {
            Camera.main.renderingPath = RenderingPath.DeferredShading;
        }
        else if (Camera.main.renderingPath == RenderingPath.DeferredShading)
        {
            Camera.main.renderingPath = RenderingPath.Forward;
        }
        else if (Camera.main.renderingPath == RenderingPath.Forward)
        {
            Camera.main.renderingPath = RenderingPath.UsePlayerSettings;
        }
        else if (Camera.main.renderingPath == RenderingPath.UsePlayerSettings)
        {
            Camera.main.renderingPath = RenderingPath.VertexLit;
        }
        else if (Camera.main.renderingPath == RenderingPath.VertexLit)
        {
            Camera.main.renderingPath = RenderingPath.DeferredLighting;
        }
        setEntityInfo("Command Interface", "Changed RenderPath to " + Camera.main.renderingPath.ToString());
    }

    public void ChangeRenderPath(string rendermode)
    {
        if (Camera.main.renderingPath == RenderingPath.DeferredLighting)
        {
            Camera.main.renderingPath = RenderingPath.DeferredShading;
        }
        else if (Camera.main.renderingPath == RenderingPath.DeferredShading)
        {
            Camera.main.renderingPath = RenderingPath.Forward;
        }
        else if (Camera.main.renderingPath == RenderingPath.Forward)
        {
            Camera.main.renderingPath = RenderingPath.UsePlayerSettings;
        }
        else if (Camera.main.renderingPath == RenderingPath.UsePlayerSettings)
        {
            Camera.main.renderingPath = RenderingPath.VertexLit;
        }
        else if (Camera.main.renderingPath == RenderingPath.VertexLit)
        {
            Camera.main.renderingPath = RenderingPath.DeferredLighting;
        }
        setEntityInfo("Command Interface", "Changed RenderPath to " + Camera.main.renderingPath.ToString());
    }

    public void ChangeShader()
    {
        string info = "";
        setEntityInfo("Changing SSRR shader", "Shader number " + shaderNo + ": " + Resources.FindObjectsOfTypeAll<Shader>()[shaderNo]);
        info += "Last shader: " + shaderNo + ": " + Resources.FindObjectsOfTypeAll<Shader>()[shaderNo].name;
        shaderNo++;
        if (shaderNo > Resources.FindObjectsOfTypeAll<Shader>().Length)
            shaderNo = 0;

        Camera.main.GetComponent<SSRR>().sSRRShader = Resources.FindObjectsOfTypeAll<Shader>()[shaderNo];
        info += " New shader: " + count + ": " + Resources.FindObjectsOfTypeAll<Shader>()[shaderNo].name;
        info += " isSupported: " + Camera.main.GetComponent<SSRR>().sSRRShader.isSupported.ToString();
        setEntityInfo("Shaders:", info);
    }

    public void CommandInterface(bool b)
    {
        string commands = "Your wish, master? Commands: exit, destroy (all, cameras, enemies, cars, turrets, doors, drones), activesc 0, activesc 1, listobjects,\n listobject #, repath, listcamera, ssrcamera";
        string titel = "Type Command"; string info = "Commands: exit, activescene 0, activescene 1, listobjects, listobject #, renderpath";
        writtenText = InputText;
        inputText = inputText.ToLower();
        Scene myScene = SceneManager.GetActiveScene();

        if (!b)
        {
            Manager.GetUIManager().DoModalMessageBox(titel, info, InputBoxUi.InputBoxTypes.MbInputOk, "Go", "Cancel", clickedOk, writtenText);
            setEntityInfo("Command Interface", commands);
        }
        else if (inputText.Contains("listobjects"))
        {
            count = 0;
            Manager.GetUIManager().ShowMessagePopup(myScene.name + " has " + myScene.GetRootGameObjects().Length + " gameobjects and its path is " + myScene.path, 20);

            clickedOk = NextObjectInfo;

            objects = myScene.GetRootGameObjects();
            info = "GameObjects in root of this scene " + objects.Length;
            Manager.GetUIManager().DoModalMessageBox(myScene.name + " objects", info, InputBoxUi.InputBoxTypes.MbInputOk, "OK", "Cancel", clickedOk, writtenText);
        }
        else if (inputText.Contains("repath"))
        {
            ChangeRenderPath();
        }
        else if (inputText.Contains("listcamera"))
        {
            List<Component> components = new List<Component>();
            info = "";
            components.AddRange(Camera.main.GetComponents(typeof(Component)));
            foreach (Component comp in components)
                //if(!comp.name.Contains("Clone"))
                    info += comp.ToString().Remove(0,14) + " ";
            //Manager.GetUIManager().DoModalMessageBox
            setEntityInfo("Components on camera are:", info);

        }
        else if (inputText.Contains("ssrcamera"))
        {
            info = "";
            if (inputText.Contains("off"))
            {
                setEntityInfo("SSRR info:", "Loading SSR off");
                Camera.main.GetComponent<CandelaSSRR>().enabled = false;
                Camera.main.GetComponent<CandelaSSRR>().HDRreflections = false;
                Camera.main.GetComponent<CandelaSSRR>().enableSkyReflections = false;
                Camera.main.GetComponent<CandelaSSRR>().BlurQualityHigh = false;
                Camera.main.GetComponent<SSRR>().enabled = false;
                setEntityInfo("SSRR info:", "all off");
                //Camera.main.GetComponent<SSRR>().sSRRMode = SSRR.SSRRMode.SSRR;
            }
            else if (inputText.Contains("1"))
            {
                setEntityInfo("SSRR info:", "Loading Candela SSSR off");
                Camera.main.GetComponent<CandelaSSRR>().enabled = false;
                Camera.main.GetComponent<CandelaSSRR>().HDRreflections = false;
                Camera.main.GetComponent<CandelaSSRR>().enableSkyReflections = false;
                Camera.main.GetComponent<CandelaSSRR>().BlurQualityHigh = false;
                setEntityInfo("SSRR info:", "Candela SSSR off");
                //Camera.main.GetComponent<SSRR>().sSRRMode = SSRR.SSRRMode.SSRR;
            }
            else if (inputText.Contains("2"))
            {
                setEntityInfo("SSRR info:", "Loading SSRR off");
                Camera.main.GetComponent<SSRR>().enabled = false;
                setEntityInfo("SSRR info:", "SSRR off");
                //Camera.main.GetComponent<SSRR>().sSRRMode = SSRR.SSRRMode.SSRR;
            }
            else if (inputText.Contains("3"))
            {
                setEntityInfo("SSRR info:", "Loading Candela SSSR on");
                Camera.main.GetComponent<CandelaSSRR>().enabled = true;
                Camera.main.GetComponent<CandelaSSRR>().HDRreflections = true;
                Camera.main.GetComponent<CandelaSSRR>().enableSkyReflections = true;
                Camera.main.GetComponent<CandelaSSRR>().BlurQualityHigh = true;
                setEntityInfo("SSRR info:", "Candela SSSR on");
            }
            else if (inputText.Contains("4"))
            {
                setEntityInfo("SSRR info:", "Loading SSSR on");
                Camera.main.GetComponent<SSRR>().enabled = true;
                Camera.main.GetComponent<SSRR>().sSRRMode = SSRR.SSRRMode.SSRR;
                setEntityInfo("SSRR info:", "SSSR on");
            }
            else if (inputText.Contains("5"))
            {
                setEntityInfo("SSRR info:", "Adding SSRR");
                if (Camera.main.GetComponent<SSRR>() == null)
                    Camera.main.gameObject.AddComponent<SSRR>();
                //Camera.main.gameObject.AddComponent(typeof(SSRR)) as SSRR;
                Camera.main.GetComponent<SSRR>().enabled = true;
                Camera.main.GetComponent<SSRR>().sSRRMode = SSRR.SSRRMode.SSRR;
                setEntityInfo("SSRR info:", "SSSR added and enabled.");
            }
            else if (inputText.Contains("6"))
            {
                setEntityInfo("SSRR info:", "Loading SSSR on");
                Camera.main.GetComponent<SSRR>().enabled = true;
                Camera.main.GetComponent<SSRR>().sSRRMode = SSRR.SSRRMode.SSRR;
                Camera.main.GetComponent<SSRR>().sSRRShader = Shader.Find("CandelaSSRRv2");
                setEntityInfo("SSRR info:", "SSSR on");
            }
            else if (inputText.Contains("7"))
            {
                foreach(Shader shader in Resources.FindObjectsOfTypeAll<Shader>())
                {
                    info += " Shadername: " + shader.name;
                }
                setEntityInfo("Shaders:", info);
            }
            else if (inputText.Contains("8"))
            {
                setEntityInfo("Changing SSRR shader", "Shader number " + shaderNo + ": " + Resources.FindObjectsOfTypeAll<Shader>()[shaderNo]);
                info += "Last shader: " + shaderNo + ": " + Resources.FindObjectsOfTypeAll<Shader>()[shaderNo].name;
                shaderNo++;
                if (shaderNo > Resources.FindObjectsOfTypeAll<Shader>().Length)
                    shaderNo = 0;

                Camera.main.GetComponent<SSRR>().sSRRShader = Resources.FindObjectsOfTypeAll<Shader>()[shaderNo];
                info += " New shader: " + count + ": " + Resources.FindObjectsOfTypeAll<Shader>()[shaderNo].name;
                info += " isSupported: " + Camera.main.GetComponent<SSRR>().sSRRShader.isSupported.ToString();
                setEntityInfo("Shaders:", info);
            }
            else if (inputText.Contains("info") || inputText.Contains("status"))
            {
                setEntityInfo("SSRR info:", "Loading SSR status");
                info += "CandelaSSRR: " + Camera.main.GetComponent<CandelaSSRR>().enabled.ToString();
                info += " HDRreflections: " + Camera.main.GetComponent<CandelaSSRR>().HDRreflections.ToString();
                info += " Sky: " + Camera.main.GetComponent<CandelaSSRR>().enableSkyReflections.ToString();
                info += " Blur: " + Camera.main.GetComponent<CandelaSSRR>().BlurQualityHigh.ToString();
                if(Camera.main.GetComponent<SSRR>() != null)
                {
                    info += " SSRR: " + Camera.main.GetComponent<SSRR>().enabled.ToString();
                    info += " SSRRmode: " + Camera.main.GetComponent<SSRR>().sSRRMode;
                    if(Camera.main.GetComponent<SSRR>().sSRRShader != null)
                        info += " SSRRshader: " + Camera.main.GetComponent<SSRR>().sSRRShader.name;
                    else
                    {
                        info += " SSRRshader: none";
                    }
                }
                setEntityInfo("SSRR info:", info);
            }
            else
            {
                setEntityInfo("SSRR info:", "Loading Candela SSRR on");
                Camera.main.GetComponent<CandelaSSRR>().enabled = true;
                Camera.main.GetComponent<CandelaSSRR>().HDRreflections = true;
                Camera.main.GetComponent<CandelaSSRR>().enableSkyReflections = true;
                Camera.main.GetComponent<CandelaSSRR>().BlurQualityHigh = true;
                setEntityInfo("SSRR info:", "Candela SSRR on");
            }
            //Camera.main.GetComponent<CandelaSSRR>().SSRRcomposeMode = 5;

            

        }
        else if (inputText.Contains("listobject"))
        {
            string resultString = Regex.Match(inputText, @"\d+").Value;
            clickedOk = NextObjectInfo;
            count = Int32.Parse(resultString);
            objects = myScene.GetRootGameObjects();
            NextObjectInfo(true);
            //Manager.GetUIManager().DoModalMessageBox("SRWorldStreamFull objects", info, InputBoxUi.InputBoxTypes.MbInputOk, "OK", "Cancel", clickedOk, writtenText);
        }
        else if (inputText.Contains("activesc 0"))
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(0));
            Manager.GetUIManager().ShowMessagePopup("Scene 0 made active", 10);
        }
        else if (inputText.Contains("activesc 1"))
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
            Manager.GetUIManager().ShowMessagePopup("Scene 1 made active", 10);
        }
        else if (inputText.Contains("exit"))
        {
            inputText = "";
            count = 0;
            return;
        }
        else if (inputText.Contains("destroy"))
        {
            AgentAI soldier = AgentAI.GetAgent(AgentAI.AgentClass.Soldier);
            Damage_data dmg = new Damage_data(WeaponAmmoType.explosive, 50000, 0.5f, soldier.transform, 0, 0, Vector3.zero, soldier.m_UID, soldier.m_Weapons.GetBestWeapon(), 10, true, true);
            info = "";
            if(inputText.Contains("all"))
            {
                setEntityInfo("Destroying:", "Everything");
                foreach(AIEntity ai in AIEntity.FindObjectsOfType(typeof(AIEntity)))
                {
                    Health.TryDoDamage(ai.transform, dmg, true);
                }
            }
            else if (inputText.Contains("cameras"))
            {
                setEntityInfo("Destroying:", "All cameras");
                foreach (CctvCamera ca in CctvCamera.FindObjectsOfType(typeof(CctvCamera)))
                {
                    Health.TryDoDamage(ca.transform, dmg, true);
                }
            }
            else if (inputText.Contains("civ"))
            {
                setEntityInfo("Destroying:", "All civilians");
                foreach (CivilianAI ca in CivilianAI.FindObjectsOfType(typeof(CivilianAI)))
                {
                    Health.TryDoDamage(ca.transform, dmg, true);
                    
                }
            }
            else if (inputText.Contains("enemies"))
            {
                setEntityInfo("Destroying:", "All enemies");
                foreach (AIEntity ai in AIEntity.FindObjectsOfType(typeof(AIEntity)))
                {
                    if (ai.IsEnemy(soldier.transform))
                        Health.TryDoDamage(ai.transform, dmg, true);
                }
            }
            else if (inputText.Contains("cars"))
            {
                setEntityInfo("Destroying:", "All cars");
                foreach (CarAI ca in CarAI.FindObjectsOfType(typeof(CarAI)))
                {
                    Health.TryDoDamage(ca.transform, dmg, true);
                }
            }
            else if (inputText.Contains("doors"))
            {
                setEntityInfo("Destroying:", "All doors");
                foreach (Door ca in Door.FindObjectsOfType(typeof(Door)))
                {
                    Health.TryDoDamage(ca.transform, dmg, true);
                }
            }
            else if (inputText.Contains("robots"))
            {
                setEntityInfo("Destroying:", "All drones");
                foreach (FlyingDroneAI ca in FlyingDroneAI.FindObjectsOfType(typeof(FlyingDroneAI)))
                {
                    Health.TryDoDamage(ca.transform, dmg, true);
                }
                foreach (BipedMechAI ca in BipedMechAI.FindObjectsOfType(typeof(BipedMechAI)))
                {
                    Health.TryDoDamage(ca.transform, dmg, true);
                }
                foreach (GiantMechAI ca in GiantMechAI.FindObjectsOfType(typeof(GiantMechAI)))
                {
                    Health.TryDoDamage(ca.transform, dmg, true);
                }
            }
            else if (inputText.Contains("drones"))
            {
                setEntityInfo("Destroying:", "All drones");
                foreach (FlyingDroneAI ca in FlyingDroneAI.FindObjectsOfType(typeof(FlyingDroneAI)))
                {
                    Health.TryDoDamage(ca.transform, dmg, true);
                }
            }
            else if (inputText.Contains("mechs"))
            {
                setEntityInfo("Destroying:", "All mechs");
                foreach (BipedMechAI ca in BipedMechAI.FindObjectsOfType(typeof(BipedMechAI)))
                {
                    Health.TryDoDamage(ca.transform, dmg, true);
                }
                foreach (GiantMechAI ca in GiantMechAI.FindObjectsOfType(typeof(GiantMechAI)))
                {
                    Health.TryDoDamage(ca.transform, dmg, true);
                }
            }
            else if (inputText.Contains("turrets"))
            {
                setEntityInfo("Destroying:", "All turrets");
                foreach (TurretAI ca in TurretAI.FindObjectsOfType(typeof(TurretAI)))
                {
                    Health.TryDoDamage(ca.transform, dmg, true);
                }
            }
            else if (inputText.Contains("power"))
            {
                setEntityInfo("Destroying:", "All power stations");
                foreach (PowerNode pn in PowerNode.FindObjectsOfType(typeof(PowerNode)))
                {
                    Health.TryDoDamage(pn.transform, dmg, true);
                }
            }
            else if (inputText.Contains("jam"))
            {
                setEntityInfo("Destroying:", "All jammers");
                foreach (Jammer jam in Jammer.FindObjectsOfType(typeof(Jammer)))
                {
                    Health.TryDoDamage(jam.transform, dmg, true);
                }
            }
            //SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
            Manager.GetUIManager().ShowMessagePopup("Destroying stuff (Possible things to destroy (all, robots, drones, mechs, power stations, doors, cameras, enemies, civilians, turrets and jammers)", 30);
        }
        else if (inputText.Contains("cheat"))
        {
            
            if (inputText.Contains("on"))
            {
                cheatMode = true;
            }
            else if (inputText.Contains("off"))
            {
                cheatMode = false;
            }
            else if (inputText.Contains("big"))
            {
                

                return;
            }
            else
            {
                cheatMode = !cheatMode;
            }
            
            if (!cheatMode)
            {
                foreach (AgentAI a in AgentAI.GetAgents())
                {
                    a.m_Energy.SetInfiniteEnergy(false);
                    a.m_Health.Invulnerable = false;
                    Manager.GetUIManager().ShowMessagePopup("Cameras in this district " + ProgressionManager.Get().GetCameraDistrictCount() + " Devs cloned:" + ProgressionManager.Get().m_DevsCloned + " Devs killed:" + ProgressionManager.Get().m_DevsKilled, 20);
                    ProgressionManager.Get().GetCameraDistrictCount();

                }

                var ui = setEntityInfo("Cheat mode off ", "Your agents can now be killed and lose energy again.");
            }
        }
        else if (inputText.Contains("command"))
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
            Manager.GetUIManager().ShowMessagePopup("Scene 1 made active", 10);
        }
        else if (inputText.Contains("command"))
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(1));
            Manager.GetUIManager().ShowMessagePopup("Scene 1 made active", 10);
        }
        else
        {
            Manager.GetUIManager().ShowMessagePopup("Command " + inputText + " not understood", 30);
            Manager.GetUIManager().DoModalMessageBox(titel, info, InputBoxUi.InputBoxTypes.MbInputOk, "Go", "Cancel", clickedOk, writtenText);
            setEntityInfo("Command Interface", commands);
        }
        
    }

    

    public void NextObjectInfo(bool b)
    {
        string inputText = InputText();
        if (inputText != null && inputText != "")
        {
            setEntityInfo("You typed the text", inputText);
        } 

        //SceneManager.LoadSceneAsync("scene", LoadSceneMode.Additive);
        //SceneManager.GetActiveScene().name;
        string path = "not found";

        string info = "";
        string titel = "";

        if(objectMode == "both")
        {
            if (b)
            {
                MakeChildrenIntoObjects();

                count = 0;
                clickedOk = NextObjectInfo;
                GameObject obj = objects[count];
                Manager.GetUIManager().ShowMessagePopup(obj.name + " has " + obj.transform.childCount + " children and its path is " + obj.transform.GetPath(), 20);
                return;
            }
            else
            {
                Manager.GetUIManager().ShowMessagePopup("Not impletemented yet", 20);
                return;
            }
        }
        else if (objectMode != "")
        {
            if (!b)
            {
                return;
            }
            else if (objectMode == "child")
            {
                MakeChildrenIntoObjects();

                count = 0;
                clickedOk = NextObjectInfo;
                GameObject obj = objects[count];
                Manager.GetUIManager().ShowMessagePopup(obj.name + " has " + obj.transform.childCount + " children and its path is " + obj.transform.GetPath(), 20);
                return;
            }
            else if (objectMode == "component")
            {
                Manager.GetUIManager().ShowMessagePopup("Not impletemented yet", 20);
                return;
            }
        }
        else if (b && count < objects.Length)
        {
            // Cycling through object array
            GameObject obj = objects[count];
            titel = count + " : " + obj.name;
            info += obj.transform.childCount + " children. ";
            info += obj.GetComponents(obj.GetType().BaseType).Length + " Components.";
            //foreach (Transform child in obj.transform)
            //    info += " :: " + child.name;
            count++;
        }
        else
        {
            // More options
            count--;
            GameObject obj = objects[count];
            titel = count + " : " + obj.name;
            info += "Object path: "+ obj.transform.GetPath() + " ";

            int childCount = obj.transform.childCount; int componentCount = obj.GetComponents(obj.GetType().BaseType).Length;

            info += childCount + " children. ";
            info += componentCount + " Components.";

            string option1 = "Back";
            string option2 = "Cancel";
            objectMode = "";

            if(childCount > 0)
            {
                option1 = "Children";
                if (componentCount > 0)
                {
                    objectMode = "both";
                    option2 = "Components";
                }
                else
                {
                    objectMode = "child";
                }
            }
            else if(componentCount > 0)
            {
                objectMode = "component";
                option1 = "Components";
            }

            //clickedOk = DoStuff;
            Manager.GetUIManager().DoModalMessageBox(titel, info, InputBoxUi.InputBoxTypes.MbOkcancel, option1, option2, clickedOk);
            return;
        }

        objectMode = "";

        Manager.GetUIManager().DoModalMessageBox(titel, info, InputBoxUi.InputBoxTypes.MbOkcancel, "Next", "More", clickedOk);
    }

    public void MakeChildrenIntoObjects()
    {
        GameObject obj = objects[count];
        objects = new GameObject[obj.transform.childCount];

        int i = 0;
        foreach (Transform t in obj.transform)
        {
            objects[i] = t.gameObject;
            i++;
        }
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
        if(b)
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
            info += @" Scene 3 name: " + SceneManager.GetSceneByName("LightScene").name + " and script name " + this.GetName() + " path " + path +
                    " full type name " + this.GetType().FullName + " base type name " + this.GetType().BaseType.Name;


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
}