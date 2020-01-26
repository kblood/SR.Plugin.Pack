using SyndicateMod.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using UnityEngine;

using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using SyndicateMod.CustomUI;
using UnityEngine.UI;

namespace SyndicateMod
{
    public class SyndicateMod : ISrPlugin
    {
        bool weaponsUpdated = false;
        int debug = 0;
        private bool runonce = false;

        bool active = false;

        /// <summary>
        /// Plugin initialization 
        /// </summary>
        public void Initialize()
        {
            Debug.Log("Initializing Satellite Reign Syndicate mod");
            //            _instance = this;

            if (!active)
                active = true;
            else
                active = false;
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            if (!active)
                return;

            //if (!weaponsUpdated)
            //{
            //    WeaponChanges();
            //    weaponsUpdated = true;
            //}

            if (Manager.Get().GameInProgress)
            {
                if (!runonce)
                {
                    //Test();
                    runonce = true;
                    
                }

                if (debug > 0)
                    DebugOptions.ms_DebugEnabled = true;
                if (debug == 1)
                    DebugOptions.ms_DebugEnabledParanoid = true;

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

                    AddWeaponToSelectedUnits(units, 0);

                    //var agents = AgentAI.GetAgents();
                    //var abils = agents.Where(a => a.GetClass() == AgentAI.AgentClass.Hacker).FirstOrDefault()?.m_Abilities.AllAbilities().Select(a => a.GetID()).ToArray();
                    //DMMap minimap -AddSimpleIcon to add minimap
                    /*
                    var ent = units.Where(e => e.m_IsControllable == false && e.IsVIP == false).FirstOrDefault();

                    if (ent != null)
                        Manager.GetUIManager().ShowMessagePopup("Found " + ent.m_Identity.GetFullName());

                    if (ent)
                    {
                        Manager.GetUIManager().ShowMessagePopup("Found " + ent.m_Identity.GetFullName() + ". Applying VIP status.");
                        ent.m_Identity.SetRandomIdentity();
                        MakeVIP(ent);
                        ent.name = "Peter Molyneux";
                        ent.m_Identity.name = "Peter Molyneux";
                        Manager.GetUIManager().ShowMessagePopup("Made " + ent.m_Identity.GetFullName() + " VIP.");
                    }

                    var names = IdentityManager.Get().m_NamesXML.text;
                    Manager.GetUIManager().ShowWarningPopup(names);

                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(names);
                    //xmlDocument.Save("TestFileDoc.xml");

                    IdentityManager.Get().GetName(1, out string first, out string last);
                    //Manager.GetUIManager().ShowWarningPopup(first + " " + last);

#pragma warning disable CS0618 // Type or member is obsolete
                    Manager.GetUIManager().GetEventPopUpUi().AddMessage("Name is now: " + first + " " + last, null, null, 50f, false);
#pragma warning restore CS0618 // Type or member is obsolete
*/
                    //bool flag = .Shift IsDown();
                    //bool flag2 = .Ctrl IsDown();
                    //bool flag3 = .Alt IsDown();

                    //var fileManager = new FileManager();
                    //byte[] bytes = Encoding.Default.GetBytes(xmlDocument.OuterXml);
                    //fileManager.SaveData("TestFileDoc2.xml", bytes);

                    //var icons = Manager.GetMiniMap().Icons;
                    //icons = Manager.GetMiniMapUi().MapCameraViewUi.DmMap.ic

                    //var Soldier = AgentAI.GetAgent(AgentAI.AgentClass.Soldier);
                    //var Hacker = AgentAI.GetAgent(AgentAI.AgentClass.Hacker);
                    //var Support = AgentAI.GetAgent(AgentAI.AgentClass.Support);
                    //var Assassin = AgentAI.GetAgent(AgentAI.AgentClass.Assassin);

                    //foreach (var selectedAgent in agents.Where(u => u.IsSelected()))
                    //{
                    //    if (selectedAgent.m_Class != AgentAI.AgentClass.NONE)
                    //        selectedAgent.m_Class = AgentAI.AgentClass.NONE;
                    //}

                    //AddAbilitiesToUnits(units, abils);

                    //ToggleWeaponOfUnits(units, WeaponType.B_minigun);

                    //Soldier.m_Class = AgentAI.AgentClass.Soldier;
                    //Hacker.m_Class = AgentAI.AgentClass.Hacker;
                    //Support.m_Class = AgentAI.AgentClass.Support;
                    //Assassin.m_Class = AgentAI.AgentClass.Assassin;
                }

                if (Input.GetKeyDown(KeyCode.ScrollLock) || Input.GetKeyDown(KeyCode.ScrollLock))
                {
                    if (debug < 2)
                        debug++;
                    else
                        debug = 0;
                    Manager.GetUIManager().ShowMessagePopup("Debug mode " + debug + " active\nDebug mode: description\n0: off\n1: debug\n2: paraniod debug");
                    //if (debug == 3)
                    //    Manager.GetUIManager().Show();
                }

                if (Input.GetKeyDown(KeyCode.Insert) || Input.GetKeyDown(KeyCode.Insert))
                {
                    //GenericsHelper.CheckedObjects.Clear();

                    List<string> output = new List<string>();
                    List<object> checkedObjects = new List<object>();

                    var infoObject = Manager.GetUIManager().InputBoxUi.transform;

                    var infos = GenericsHelper.GetNamesAndValues(infoObject.GetType(), infoObject, ref output, ref checkedObjects);
                    FileManager.SaveList(output, Manager.GetPluginManager().PluginPath + $@"\ObjectInfo_{infoObject.name}.txt");

                    infos.Select(i => "Name: " + i.Key + " Value: " + i.Value).ToList();
                    FileManager.SaveList(output, Manager.GetPluginManager().PluginPath + $@"\ObjectData_{infoObject.name}.txt");

                    var infoObject2 = Manager.GetUIManager().InputBoxUi;

                    infos = GenericsHelper.GetNamesAndValues(infoObject.GetType(), infoObject, ref output, ref checkedObjects);
                    FileManager.SaveList(output, Manager.GetPluginManager().PluginPath + $@"\ObjectInfo2_{infoObject2}.txt");

                    infos.Select(i => "Name: " + i.Key + " Value: " + i.Value).ToList();
                    FileManager.SaveList(output, Manager.GetPluginManager().PluginPath + $@"\ObjectData2_InputBoxUi.txt");

                    //var infos = GenericsHelper.GetNamesAndTypes(Manager.GetUIManager().InputBoxUi.transform);

                    return;
                    Test2();

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
                    Manager.GetUIManager().ShowSubtitle("Running Test 1. Change pistol text without replacing fastlanglookup.", 5);
                    try
                    {
                        SaveDataTest();
                        //Test();
                    }
                    catch (Exception e)
                    {
                        Manager.GetUIManager().DoModalMessageBox("Error thrown while running test", e.Message, InputBoxUi.InputBoxTypes.MbOk);
                    }
                    //SaveDataTest();

                    //AddAbilityToSelectedUnits(GetSelectUnits());
                }

                if (Input.GetKeyDown(KeyCode.PageUp) || Input.GetKeyDown(KeyCode.PageUp))
                {
                    Test();
                    //var handler = new ObjectiveEventHandler();
                    //handler.StartCoroutine(CoRoutineUITest("Test1", "Test2", InputBoxUi.InputBoxTypes.MbOkcancel));
                }

                // Hijack every non-controlled unit
                if (Input.GetKeyDown(KeyCode.Keypad5) || Input.GetKeyDown(KeyCode.Keypad5))
                {
                    List<AIEntity> units = new List<AIEntity>();
                    foreach (AIEntity ae in AIEntity.FindObjectsOfType(typeof(AIEntity)))
                    {
                        if ((ae.IsHuman() || ae.IsMech()) && !ae.m_IsControllable)
                        {
                            units.Add(ae);
                        }
                    }

                    ToggleControlOfUnits(units);
                }

                if (Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.Keypad6))
                {
                    var units = GetSelectUnits();

                    var FiveLives = units.Where(ae => ae.m_Identity.GetNameID() < 5)?.ToList();

                    if (FiveLives != null && FiveLives.Any())
                    {
                        foreach (var dev in FiveLives)
                            MakeVIP(dev);
                    }
                }
            }
        }

        //private static SyndicateMod _instance;

        //public static SyndicateMod Get()
        //{
        //    return _instance;
        //}

        public static void ShowMessage(string text)
        {
            Manager.GetUIManager().ShowMessagePopup(text, 5);
            //Manager.GetUIManager().man (text, 10);
            Manager.GetUIManager().ShowSubtitle(text, 10);
            //Get().setEntityInfo("Show Message", text);
        }

        static InputBoxUi_V2 inputBoxUi_V2;

        public InputBoxUi_V2 m_InputBoxUi { get { return inputBoxUi_V2; } }

        static InputBoxUi original;

        private Button testButton;

        public void Test()
        {
            //((MonoBehaviour)Manager.Get()).StartCoroutine
            Manager.Get().StartCoroutine(ModalMessageBoxRoutine("Test", "Test", InputBoxUi.InputBoxTypes.MbOkcancel, "Test2", "Cancel", 
                delegate (bool b)
                {
                    if(b)
                        SaveDataTest();
                    else
                    {
                        Manager.GetUIManager().ShowMessagePopup("It worked", 10);
                    }
                }
            ));
            //if (testButton = null)
            //{
            //    testButton = SREditor.CreateButton(SaveDataTest);
            //    testButton.transform.SetAsLastSibling();
                
            //}
            //else
            //{
            //    GameObject.Destroy(testButton);
            //    testButton = null;
            //}
        }

        private IEnumerator ModalMessageBoxRoutine(string titleText, string mesageText, InputBoxUi.InputBoxTypes messsageBoxType, string okText = null, string cancelText = null, Action<bool> ok = null, Func<string> messageTextFunc = null, string inputText = "")
        {
            if (Manager.GetUIManager().m_InputBoxUi.isActiveAndEnabled)
            {
                Manager.GetUIManager().m_InputBoxUi.Hide();
                yield return null;
            }
            bool inputControlEnabled = Manager.GetUIManager().InputControlUi.gameObject.activeSelf;
            Manager.GetUIManager().InputControlUi.gameObject.SetActive(false);
            Manager.ptr.DisableKeyCommands();
            Manager.GetUIManager().m_InputBoxUi.InputBoxType = messsageBoxType;
            Manager.GetUIManager().m_InputBoxUi.InputFieldLabelText = mesageText;
            Manager.GetUIManager().m_InputBoxUi.InputFieldLabelTextFunc = messageTextFunc;
            Manager.GetUIManager().m_InputBoxUi.TitleText = titleText;
            Manager.GetUIManager().m_InputBoxUi.InputText = inputText;
            Manager.GetUIManager().m_InputBoxUi.OkButtonText.text = (okText ?? TextManager.GetLoc("BUTTON_OK", true, false));
            Manager.GetUIManager().m_InputBoxUi.CancelButtonText.text = (cancelText ?? TextManager.GetLoc("BUTTON_CANCEL", true, false));

            var newButton = UnityEngine.Object.Instantiate(Manager.GetUIManager().m_InputBoxUi.m_CancelButtonContainer);
            Manager.GetUIManager().m_InputBoxUi.m_CancelButtonContainer = newButton;
            newButton.transform.SetParent(Manager.GetUIManager().m_InputBoxUi.CancelButton.transform.parent);
            GameObject.Destroy(Manager.GetUIManager().m_InputBoxUi.CancelButton);
            newButton.ButtonText.text = "Haxxors2";
            newButton.GetComponentInChildren<Text>().text = "Haxxors";

            Manager.GetUIManager().m_InputBoxUi.Show(Manager.GameActive);
            Manager.GetUIManager().m_InputBoxUi.transform.SetAsLastSibling();
            Manager.GetUIManager().ToggleEverything(true);

            yield return Manager.GetUIManager().WaitForActive(Manager.GetUIManager().m_InputBoxUi.gameObject, false);
            Utils.SafeInvoke<bool>(ok, Manager.GetUIManager().m_InputBoxUi.IsOk());
            Manager.GetUIManager().InputControlUi.gameObject.SetActive(inputControlEnabled);
            Manager.GetUIManager().ToggleEverything(true);
            Manager.ptr.EnableKeyCommands();
            yield break;
        }

        public IEnumerator CoRoutineUITest(string titleText, string messageText, InputBoxUi.InputBoxTypes messageBoxType, string okText = null, string cancelText = null, Action<bool> ok = null, Func<string> messageTextFunc = null, string inputText = "")
        {
            if (inputBoxUi_V2 == null)
            {
                //var inputBox = UnityEngine.Object.Instantiate(Manager.GetUIManager().m_InputBoxUi);
                original = Manager.GetUIManager().m_InputBoxUi;
                inputBoxUi_V2 = new InputBoxUi_V2(Manager.GetUIManager().m_InputBoxUi);
                Manager.GetUIManager().m_InputBoxUi = inputBoxUi_V2;
            }

            if (m_InputBoxUi.isActiveAndEnabled)
            {
                this.m_InputBoxUi.Hide();
                yield return null;
            }
            bool inputControlEnabled = Manager.GetUIManager().InputControlUi.gameObject.activeSelf;
            Manager.GetUIManager().InputControlUi.gameObject.SetActive(false);
            Manager.ptr.DisableKeyCommands();

            this.m_InputBoxUi.InputBoxType = messageBoxType;
            this.m_InputBoxUi.InputFieldLabelText = messageText;
            this.m_InputBoxUi.InputFieldLabelTextFunc = messageTextFunc;
            this.m_InputBoxUi.TitleText = titleText;
            this.m_InputBoxUi.InputText = inputText;
            this.m_InputBoxUi.OkButtonText.text = (okText ?? TextManager.GetLoc("BUTTON_OK", true, false));
            this.m_InputBoxUi.CancelButtonText.text = (cancelText ?? TextManager.GetLoc("BUTTON_CANCEL", true, false));
            this.m_InputBoxUi.Show(Manager.GameActive);
            this.m_InputBoxUi.transform.SetAsLastSibling();

            inputBoxUi_V2.AddButton
            ("Test", delegate ()
                {
                    SaveDataTest();
                }
            );

            Manager.GetUIManager().ToggleEverything(true);
            yield return Manager.GetUIManager().WaitForActive(this.m_InputBoxUi.gameObject, false);
            Utils.SafeInvoke<bool>(ok, this.m_InputBoxUi.IsOk());
            Manager.GetUIManager().InputControlUi.gameObject.SetActive(inputControlEnabled);
            Manager.GetUIManager().ToggleEverything(true);
            Manager.ptr.EnableKeyCommands();
            //yield 0;

            //inputBoxUi_V2.gameObject.SetActive(true);

            //inputBoxUi_V2.Show(Manager.GameActive);

            //inputBoxUi_V2.transform.SetAsLastSibling();
            yield break;

        }

        public void Test2()
        {
            if(Manager.GetPluginManager().LoadedPlugins.Any())
            {
                Manager.GetPluginManager().LoadedPlugins.Clear();
            }
            else
            {
                Manager.GetPluginManager().Start();
            }
        }

        public void SaveDataTest()
        {
            Transform transform = Manager.GetUIManager().m_InputBoxUi.transform;

            var output = SRInfoHelper.GetFullTransformHierchy(transform);
            output.AddRange(SRInfoHelper.GetTransformData(transform));

            FileManager.SaveList(output, Manager.GetPluginManager().PluginPath + $@"\{transform.name}.txt");

            SRInfoHelper.SaveLangData();

            output = SRInfoHelper.SaveAbilityData();

            FileManager.SaveList(output, Manager.GetPluginManager().PluginPath + $@"\Abilities.txt");

            transform = Manager.GetSkillManager().transform;
            output = SRInfoHelper.GetFullTransformHierchy(transform);
            output.AddRange(SRInfoHelper.GetTransformData(transform));
            FileManager.SaveList(output, Manager.GetPluginManager().PluginPath + $@"\{transform.name}.txt");

            transform = Manager.GetCloneManager().transform;
            output = SRInfoHelper.GetFullTransformHierchy(transform);
            output.AddRange(SRInfoHelper.GetTransformData(transform));
            FileManager.SaveList(output, Manager.GetPluginManager().PluginPath + $@"\{transform.name}.txt");

            transform = Manager.GetAbilityManager().transform;
            output = SRInfoHelper.GetFullTransformHierchy(transform);
            output.AddRange(SRInfoHelper.GetTransformData(transform));
            FileManager.SaveList(output, Manager.GetPluginManager().PluginPath + $@"\{transform.name}.txt");
        }

        public void SaveDataTest2()
        {
            //var abilities = Manager.GetAbilityManager().m_AbilityData; //.GetFieldValue<Dictionary<int, Ability.AbilityData>>("m_ALLAbilityData");
                                                                       //abilities.Any();
            Manager.GetAbilityManager().GetAbilityNamesAndIDs(out var ids, out var names);
            //abilities = Manager.GetAbilityManager().GetFieldValue<Dictionary<int, Ability.AbilityData>>("__ALLAbilityData");

            List<string> values = new List<string>();
            foreach (var id in ids)
            {
                string name = Manager.GetAbilityManager().GetName(id);
                values.Add($"Ability id: {id} Name: {name}");

                //if (id > 0)
                //    values.Add($"Ability id: {id} Name: {names[0]}");
                //else
                //    values.Add($"Ability id: {id} Name: {Manager.GetAbilityManager().GetName(id)}");

                if (id > 0 && !name.Contains("###"))
                {
                    var a = Manager.GetAbilityManager().GetData(id);

                    try
                    {
                        values.Add($@"m_Name:{a.m_Name} energyCostWhen: {a.m_EnergyCostWhen} m_UseAmmoWhen: {a.m_UseAmmoWhen} m_TargetRange:{a.m_TargetRange}");
                        values.Add($@"m_WeaponCheckMask:{a.m_WeaponCheckMask} m_TargetEffectTimeOuts: {string.Join(",", a.m_TargetEffectTimeOuts?.Select(s => s.ToString()).ToArray())} m_TargetEffects: {string.Join(",", a.m_TargetEffects?.Select(s => s.ToString()).ToArray())} m_RemoveModifiersWhen:{a.m_RemoveModifiersWhen}");
                        values.Add($@"m_Range:{a.m_Range} m_OnMove: {a.m_OnMove?.Select(om => om.ToString()).Aggregate((x, y) => x + "," + y)} m_RangeModifier: {a.m_RangeModifier}");
                        values.Add($@"m_RemoveModifiersWhen:{a.m_RemoveModifiersWhen} m_MyEffects: {a.m_MyEffects?.Select(om => om.ToString())?.Aggregate((x, y) => x + "," + y)} m_MyEffectTimeOuts:{a.m_MyEffectTimeOuts?.Select(om => om.ToString())?.Aggregate((x, y) => x + "," + y)}");
                        values.Add($@"m_Modifiers: {a.m_Modifiers?.Select(m => $@"m_Ammount:{m.m_Ammount} m_AmountModifier:{m.m_AmountModifier} m_TimeOut:{m.m_TimeOut} m_Type:{m.m_Type}").Aggregate((x, y) => x + "," + y)}");
                        values.Add($@"m_ModifiersTarget: {a.m_ModifiersTarget?.Select(m => $@"m_Ammount:{m.m_Ammount} m_AmountModifier:{m.m_AmountModifier} m_TimeOut:{m.m_TimeOut} m_Type:{m.m_Type}").Aggregate((x, y) => x + "," + y)}");
                        values.Add($@"m_RequiresWeaponCheck:{a.m_RequiresWeaponCheck} m_RequiresAgentSelected: {a.m_RequiresAgentSelected} ...");
                        values.Add($"End of Ability index: {a.m_ID} Name: { a.Name}");
                    }
                    catch
                    {

                    }
                    

                    //values.Add($@"KEY: {x.Key} TOKEN: {x.Value.m_token} m_Translation 0: {x.Value.m_Translations[0]}");
                    //values.Add(x.Value.m_Translations[i]);
                }

            }

            Manager.GetUIManager().ShowMessagePopup("Saving abilities, 5");

            FileManager.SaveList(values, Manager.GetPluginManager().PluginPath + $@"\Abilities2.txt");

            //List<string> values = new List<string>();
            //foreach (var a in abilities)
            //{
            //    values.Add($"Ability index: {a.Key} Name: {a.Value?.Name}");

            //    //values.Add($@"m_Name:{a.Value?.m_Name} energyCostWhen: {a.Value?.m_EnergyCostWhen.ToString()} m_UseAmmoWhen: {a.Value?.m_UseAmmoWhen} m_TargetRange:{a.Value?.m_TargetRange}");
            //    //values.Add($@"m_WeaponCheckMask:{a.Value.m_WeaponCheckMask} m_TargetEffectTimeOuts: {string.Join(",", a.Value?.m_TargetEffectTimeOuts?.Select(s => s.ToString()).ToArray())} 
            //    //m_TargetEffects: {string.Join(",", a.Value.m_TargetEffects?.Select(s => s.ToString()).ToArray())} m_RemoveModifiersWhen:{a.Value.m_RemoveModifiersWhen}");
            //    //values.Add($@"m_Range:{a.Value?.m_Range} m_OnMove: {a.Value?.m_OnMove.Select(om => om.ToString()).Aggregate((x, y) => x + "," + y)} m_RangeModifier: {a.Value?.m_RangeModifier}");
            //    //values.Add($@"m_RemoveModifiersWhen:{a.Value?.m_RemoveModifiersWhen} m_MyEffects: {a.Value?.m_MyEffects?.Select(om => om.ToString())?.Aggregate((x, y) => x + "," + y)} m_MyEffectTimeOuts:{a.Value?.m_MyEffectTimeOuts?.Select(om => om.ToString())?.Aggregate((x, y) => x + "," + y)}");
            //    //values.Add($@"m_Modifiers: {a.Value?.m_Modifiers?.Select(m => $@"m_Ammount:{m.m_Ammount} m_AmountModifier:{m.m_AmountModifier} m_TimeOut:{m.m_TimeOut} m_Type:{m.m_Type}").Aggregate((x, y) => x + "," + y)}");
            //    //values.Add($@"m_ModifiersTarget: {a.Value?.m_ModifiersTarget?.Select(m => $@"m_Ammount:{m.m_Ammount} m_AmountModifier:{m.m_AmountModifier} m_TimeOut:{m.m_TimeOut} m_Type:{m.m_Type}").Aggregate((x, y) => x + "," + y)}");
            //    //values.Add($@"m_RequiresWeaponCheck:{a.Value?.m_RequiresWeaponCheck} m_RequiresAgentSelected: {a.Value?.m_RequiresAgentSelected} ...");
            //    //values.Add($"End of Ability index: {a.Key} Name: { a.Value.Name}");

            //    //values.Add($@"KEY: {x.Key} TOKEN: {x.Value.m_token} m_Translation 0: {x.Value.m_Translations[0]}");
            //    //values.Add(x.Value.m_Translations[i]);
            //}

            //typeof(AbilityThrowProjectile).GetField("m_Range", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(ability, fogRange * 3);


            //FileManager.SaveObjectAsJson(langLookup, "localizationfile1");
            //langLookup.
        }

        public void LoadDataTest()
        {
            var langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");

            Manager.GetUIManager().ShowMessagePopup("Loading localization take 2", 5);
            var localization = TextManager.Get().m_LocalisationXML.text;

            Array values = Enum.GetValues(typeof(TextManager.Language));
            List<string> list = new List<string>();
            foreach (object obj in values)
            {
                list.Add(obj.ToString().ToLower());
            }
            langLookup.Clear();

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(Manager.GetPluginManager().PluginPath + @"\CustLang1.xml"); // TextManager.Get().m_LocalisationXML.text
            XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("row");
            foreach (object obj2 in elementsByTagName)
            {
                XmlNode xmlNode = (XmlNode)obj2;
                TextManager.LocElement locElement = new TextManager.LocElement();
                locElement.m_token = xmlNode.Attributes.GetNamedItem("name").Value;  //get_ItemOf("name").Value;
                foreach (object obj3 in xmlNode.ChildNodes)
                {
                    XmlNode xmlNode2 = (XmlNode)obj3;
                    //string text = xmlNode2.Attributes.get_ItemOf("name").Value.ToLower();
                    string text = xmlNode2.Attributes.GetNamedItem("name").Value.ToLower();
                    string innerText = xmlNode2.InnerText;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i].Equals(text))
                        {
                            locElement.m_Translations[i] = innerText;
                            break;
                        }
                    }
                }
                if (langLookup.ContainsKey(locElement.m_token))
                {
                    Debug.LogError(" duplicate key " + locElement.m_token + " found in localisation text");
                }
                else
                {
                    langLookup.Add(locElement.m_token, locElement);
                }
            }
            return;

            //Manager.GetUIManager().ShowWarningPopup(localization);

            Manager.GetUIManager().ShowMessagePopup("Loading localization", 5);

            //XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(localization);

            Manager.GetUIManager().ShowMessagePopup("Saving localization with xml", 5);
            xmlDocument.Save("TestFileDoc.xml");

            Manager.GetUIManager().ShowMessagePopup("Encoding localization xml to bytes", 5);
            var fileManager = new FileManager();
            byte[] bytes = Encoding.UTF8.GetBytes(xmlDocument.OuterXml);

            Manager.GetUIManager().ShowMessagePopup("Saving bytes", 5);

            FileManager.SaveData(bytes);
            FileManager.SaveData(bytes, "TestFileDoc3.xml");

            Manager.GetUIManager().ShowMessagePopup(@"Saving bytes with pluginPath " + Manager.GetPluginManager().PluginPath, 5);

            Manager.GetUIManager().ShowWarningPopup(Manager.GetPluginManager().PluginPath, 8);

            FileManager.SaveData(bytes, Manager.GetPluginManager().PluginPath + @"\TestFileDoc3.xml");


            //Manager.GetQuestManager().
            
            
            Manager.GetUIManager().ShowMessagePopup("Getting fast lang lookup dict from TextManager", 5);


            //FileManager.SaveObjectAsJson(langLookup, "localizationfile2");

            //FileManager.SaveObjectAsJson(Manager.GetAchievements(), "StatsAndArchievements");

            Manager.GetUIManager().ShowMessagePopup("Getting abilities dict", 5);

            List<string> datalist = new List<string>();

            var abilities = ReflectionExtensions.GetFieldValue<Dictionary<int, Ability.AbilityData>>(Manager.GetAbilityManager(), "m_ALLAbilityData");

            Manager.GetUIManager().ShowMessagePopup("test", 5);

            FileManager.SaveList(new List<string> { "test1", "test2" }, Manager.GetPluginManager().PluginPath + @"\TestList.txt");

            Manager.GetUIManager().ShowMessagePopup("Abilities names and values 1", 5);

            Manager.GetUIManager().ShowMessagePopup("Abilities names and values 2 : " + abilities == null ? "is null": abilities.Any().ToString(), 5);

            datalist = abilities?.Select(a => "ID: " + a.Key + " Names: " + a.Value?.Name).ToList();

            Manager.GetUIManager().ShowMessagePopup("Saving Abilities names and values", 5);

            FileManager.SaveList(datalist, @"C:\Temp\abilityList.txt");

            Manager.GetUIManager().ShowMessagePopup("Getting names and values as strings", 5);

            var data = GenericsHelper.GetNamesAndValuesAsString(typeof(Ability.AbilityData), abilities.First().Value);

            Manager.GetUIManager().ShowMessagePopup("Saving Abilities names and types", 5);


            datalist = data.Select(x => x.Key + " has value " + x.Value.ToString()).ToList();

            Manager.GetUIManager().ShowMessagePopup("Done", 5);


            //var namesAndTypesAbil = GenericsHelper.GetNamesAndTypes(typeof(Dictionary<int, Ability.AbilityData>));

            //Manager.GetUIManager().ShowMessagePopup("LangLookup names and types", 5);

            //var namesAndTypesLangLookup = GenericsHelper.GetNamesAndTypes(typeof(Dictionary<string, TextManager.LocElement>));

            //Manager.GetUIManager().ShowMessagePopup("Saving names and types of abilities", 5);

            //List<string> datalist = new List<string>();

            //datalist = namesAndTypesAbil.Select(x => x.Key + " is of type " + x.Value.ToString()).ToList();

            //FileManager.SaveList(datalist, @"C:\Temp\abilityTypeList.txt");

            //Manager.GetUIManager().ShowMessagePopup("Saving names and types of FastLangLookup", 5);

            //datalist = namesAndTypesLangLookup.Select(x => x.Key + " is of type " + x.Value.ToString()).ToList();

            //FileManager.SaveList(datalist, @"C:\Temp\FastLangLookupTypeList.txt");

            //Manager.GetUIManager().ShowMessagePopup("LangLookup names and values", 5);

            //var namesAndValuesLangLookup = GenericsHelper.GetNamesAndValues(typeof(Dictionary<string, TextManager.LocElement>), langLookup);

            //Manager.GetUIManager().ShowMessagePopup("abilities names and values", 5);

            //var namesAndValuesAbil = GenericsHelper.GetNamesAndValues(typeof(Dictionary<int, Ability.AbilityData>), abilities);

            //Manager.GetUIManager().ShowMessagePopup("Saving names and values of abilities", 5);

            //datalist = namesAndValuesAbil.Select(x => x.Key + " is of type " + x.Value.ToString()).ToList();

            //FileManager.SaveList(datalist, @"C:\Temp\abilityValueList.txt");

            //Manager.GetUIManager().ShowMessagePopup("Saving names and values of FastLangLookup", 5);

            //datalist = namesAndValuesLangLookup.Select(x => x.Key + " is of type " + x.Value.ToString()).ToList();

            //FileManager.SaveList(datalist, @"C:\Temp\FastLangLookupValueList.txt");
        }


        public void MakeVIP(AIEntity unit)
        {
            unit.gameObject.AddComponent<VIP>();
            VIP vip = unit.GetComponent<VIP>();
            vip.VIPType = VIPType.InfoTarget;
            unit.HasBeenScanned = false;
        }

        public List<AIEntity> GetSelectedUnits()
        {
            return Manager.GetInputControl().GetPlayerSelectedEntities();
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

        public void AddWeaponToSelectedUnits(List<AIEntity> units, int indexAt = 0)
        {
            //Manager.GetUIManager().ShowMessagePopup("Listing weapons");

            string info = "";

            var weapons = Enum.GetValues(typeof(WeaponType)).Cast<WeaponType>().ToList();

            int skip = 16;
            for (int i = indexAt; i < 29 && i < indexAt + skip; i++)
            {
                info += "\nID: " + i + " - " + (WeaponType)i;
            }

            string cancelButtonText = "Next";
            if (indexAt + skip > 29)
                cancelButtonText = "Cancel";

            bool userOk = false;
            string input = "";
            Manager.GetUIManager().DoModalMessageBox("Toggle weapon of selected units", "Pick weapon to add:" + info, InputBoxUi.InputBoxTypes.MbInput, null, cancelButtonText,
            delegate (bool b)
            {
                userOk = b;
                if (b)
                {
                    input = Manager.GetUIManager().InputValue();
                    int id = -1;
                    int.TryParse(input, out id);
                    if (id > 0 || id < 28)
                        ToggleWeaponOfUnits(units, (WeaponType)id);
                    Manager.ptr.EnableKeyCommands();
                    Manager.GetUIManager().InputBoxUi.Hide();
                    
                }
                else
                {
                    if(indexAt+ skip < 29)
                    {
                        AddWeaponToSelectedUnits(units, indexAt + skip);

                        //Manager.GetInputControl().ResetIdleTime();
                        
                    }
                    else
                    {
                        Manager.GetUIManager().InputBoxUi.Hide();
                        Manager.ptr.EnableKeyCommands();
                    }
                }
            },
            null, "put weapon ID here");

            //if (userOk)
            //{
            //    int id = -1;
            //    int.TryParse(input, out id);
            //    if (id > 0 || id < 28)
            //        ToggleWeaponOfUnits(units, (WeaponType)id);
            //    Manager.ptr.EnableGameCamera(true, true);
            //    Manager.ptr.EnableKeyCommands();
            //}
            //else
            //{
            //    if (indexAt + skip < 29)
            //    {
            //        AddWeaponToSelectedUnits(units, indexAt + skip);

            //        //Manager.GetInputControl().ResetIdleTime();

            //    }
            //    else
            //    {
            //        Manager.GetUIManager().InputBoxUi.Hide();
            //        Manager.ptr.EnableKeyCommands();
            //    }
            //}
        }

        

        SortedDictionary<int, string> _abilityDictionary;

        public IEnumerator AddAbilityToSelectedUnits(List<AIEntity> units, int indexAt = 1)
        {
            Manager.GetUIManager().ShowMessagePopup("Getting skills");

            string info = "ID and ability name:";
            string[] aNames = new string[0];
            int[] aIds = new int[0];

            if(_abilityDictionary == null || !_abilityDictionary.Any())
            {
                var abilMan = Manager.GetAbilityManager();
                Manager.GetUIManager().ShowMessagePopup("Creating dictionary");
                _abilityDictionary = new SortedDictionary<int, string>();
                Manager.GetUIManager().ShowMessagePopup("Getting ids and names");
                abilMan.GetAbilityNamesAndIDs(out aIds, out aNames);

                foreach (int id in aIds)
                {
                    if (id < 0)
                        continue;
                    string name = "";
                    Manager.GetUIManager().ShowMessagePopup("Get name of ability id " + id);
                    if (abilMan.GetName(id).Contains("###"))
                        continue;
                    
                    Manager.GetUIManager().ShowMessagePopup("Getting ability name of ability with id " + id);
                    name = abilMan.GetAbilityName(id);


                    Manager.GetUIManager().ShowMessagePopup("Adding ability id+name " + id +" "+ name + " to dictionary");
                    if (!name.ToLower().Contains("obsolete"))
                    {
                        //if(!_abilityDictionary.ContainsKey(id))
                            _abilityDictionary.Add(id, name);
                    }
                }
            }
            Manager.GetUIManager().ShowMessagePopup("Adding sorted keys to aids array");
            aIds = _abilityDictionary.Keys.ToArray();

            Manager.GetUIManager().ShowMessagePopup("About to show skills");

            int skip = 13;

            for (int i = indexAt; i < aIds.Length && i < (indexAt + skip); i++)
            {
                //Manager.GetUIManager().ShowWarningPopup("Adding skill no: " + i + ": " + aIds[i] + ": " + aNames[i], 5);
                
                info += "\n" + aIds[i] + " : " + _abilityDictionary[aIds[i]];
            }
            Manager.GetUIManager().ShowMessagePopup("info text is " + info.Length + " long.", 8);
            if(info.Length > 400)
                info = info.Substring(0, 400);

            string cancelButtonText = "Next";
            if (indexAt + skip > aIds.Length)
                cancelButtonText = "Cancel";

            bool boolOk = false;
            string input = "";

            yield return Manager.GetUIManager().DoModalMessageBox("Add ability to selected units", info, InputBoxUi.InputBoxTypes.MbInput, null, cancelButtonText,
            delegate (bool b)
            {
                //Manager.GetUIManager().ShowWarningPopup("Clicked message box " + b, 4);
                boolOk = b;
                if (b)
                {
                    input = Manager.GetUIManager().InputValue();
                }
            },
            null, "put skill ID here");

            if(boolOk)
            {
                int id = 0;
                int.TryParse(input, out id);

                if (aIds.Contains(id))
                {
                    foreach (var unit in units)
                        AddAbilityToUnit(unit, id);
                }

                Manager.ptr.EnableKeyCommands();
            }
            else
            {
                if (indexAt + skip < aIds.Length)
                {
                    var co = AddAbilityToSelectedUnits(units, indexAt + skip);
                    //StartCoroutine(co);
                }
                    
                else
                {
                    Manager.ptr.EnableKeyCommands();
                }
            }
            yield break;
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

        // DMMap minimap - AddSimpleIcon to add minimap icon that follows target AIentity


        public void ToggleWeaponOfUnits(List<AIEntity> units, WeaponType type)
        {
            //DebugOptions.ms_DebugEnabled = true;
            //DebugOptions.ms_DebugEnabledParanoid = true;

            //Manager.GetUIManager().ShowMessagePopup("Changing weapons of select units to " + type.ToString(), 6);
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
            //Manager.GetUIManager().ShowWarningPopup("Changed weapons of select units to " + type.ToString());
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

            var allGear = Manager.GetItemManager().GetAllItems().Where(g => g.m_Slot == ItemSlotTypes.Gear);
            var allAugs = Manager.GetItemManager().GetAllItems().Where(g => (int)g.m_Slot > 0 && (int)g.m_Slot < 5);
            var allArmor = Manager.GetItemManager().GetAllItems().Where(g => g.m_GearSubCategory == ItemSubCategories.ArmorBody || g.m_GearSubCategory == ItemSubCategories.ArmorHead);

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
            var item = Manager.GetItemManager().m_ItemDefinitions.First();
            var mods = item.m_Modifiers;


            Manager.GetUIManager().ShowMessagePopup("Pistol upgraded");

            List<int> abil = weaponManager.m_WeaponData[(int)WeaponType.B_pistol].m_Abilities.ToList();

            //abil.Add(Manager.GetAbilityManager().m_AbilityStealth[0].m_ID);
            //abil.Add(Manager.GetAbilityManager().m_AbilityHardwire[2].m_ID);
            //abil.Add(Manager.GetAbilityManager().m_AbilityHackTarget[2].m_ID);
            abil.Add(1);
            abil.Add(2);

            //m_FastLanguageLookup
            typeof(AbilityThrowProjectile).GetField("m_Range", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(abil, 3 * 3);



            //customItem.m_ID = Manager.GetItemManager().m_ItemDefinitions.Where(it => it.nam)

            ItemManager.ItemData customItem = new ItemManager.ItemData();
            customItem.m_ID = Manager.GetItemManager().m_ItemDefinitions.Count();
            customItem.m_Slot = ItemSlotTypes.AugmentationLegs;
            customItem.m_Modifiers = new ModifierData5L[] 
            {
                new ModifierData5L()
                {
                    m_Type = ModifierType.SpeedMultiplier,
                    m_TimeOut = 1,
                    m_Ammount = 2,
                    m_AmountModifier = ModifierType.SpeedMultiplier
                },
                new ModifierData5L()
                {
                    m_Type = ModifierType.SprintEnergyCostMultiplier,
                    m_TimeOut = 0,
                    m_Ammount = 0.2f,
                    m_AmountModifier = ModifierType.SprintEnergyCostMultiplier
                },
                new ModifierData5L()
                {
                    m_Type = ModifierType.SprintEnergyCostOffset,
                    m_TimeOut = 0,
                    m_Ammount = 1.0f,
                    m_AmountModifier = ModifierType.SprintEnergyCostOffset
                }
            };
            //Manager.GetItemManager().
            Manager.GetAbilityManager().GetAbilityNamesAndIDs(out int[] ids, out string[] names);
            //var test = AbilityManager.m_ALLAbilityData;
            //Manager.GetAbilityManager().
            customItem.m_AbilityIDs = new List<int>() { Manager.GetAbilityManager().m_AbilityHijack[0].m_ID, 2, 3};
            customItem.m_PlayerHasBlueprints = true;
            customItem.m_GearSubCategory = ItemSubCategories.Standard;
            customItem.m_Cost = 64;
            customItem.m_Count = 1;
            customItem.m_PrototypeCost = 640;
            customItem.m_WeaponType = WeaponType.None;


            customItem.m_FriendlyName = "CustomItem";
            string note = GoogleFu.LocalisationMaster.Instance.Rows[0]._NOTES;
            //GoogleFu.LocalisationMaster.Instance.Rows[9]._NOTES = "new Wyvern INCO";
            //GoogleFu.LocalisationMaster.Instance.Rows[9]._EN = "new Wyvern INCO";
            Manager.GetItemManager().m_ItemDefinitions.Add(customItem);
            var locations = Manager.GetLocationManager().m_Locations;
            //locations[0].

            var test = TextManager.Get().m_LocalisationXML;
            TextManager.LocElement locElement = new TextManager.LocElement();


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

        // Custom info shown in a entity popup
        public EntityInfoPanel setEntityInfo(string title, string info)
        {
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

            ai.m_Selectable.SetSelected(true);

            EntityInfoPanel ui = (EntityInfoPanel)EntityInfoPanel.FindObjectOfType(typeof(EntityInfoPanel));

            ui.m_DetailText.Text = info; // + "(GUI m_DetailText)";
            ui.m_DetailText.m_Text.text = info; // + "(GUI m_Text)";
            ui.name = title; // + "(GUI name)";
            ui.m_SummaryText.text = title; // + "(GUI m_SummaryText)";
                                           //ui.m_BuyButton.enabled = true;
            if (!ui.IsVisible())
                ui.Show();

            return ui;
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
