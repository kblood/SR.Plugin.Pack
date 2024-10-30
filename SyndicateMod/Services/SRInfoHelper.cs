using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using SyndicateMod.Services;
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Globalization;
using System.Text.RegularExpressions;

namespace SyndicateMod.Services 
{
    public static class SRInfoHelper
    {
        public static bool isLogging = true;

        public static List<string> GetItemInfo()
        {
            List<string> output = new List<string>();

            var allItems = Manager.GetItemManager().GetAllItems();

            foreach(var item in allItems.OrderBy(i => i.m_ID))
            {
                try
                {
                    output.Add($"***** ID: {item.m_ID} m_FriendlyName: {item.m_FriendlyName} locName: {item.m_LocName} slot: {item.m_Slot} weapontype: {item.m_WeaponType}");
                    output.Add($"m_ResearchCost: {item.m_ResearchCost} m_Cost: {item.m_Cost} m_BlueprintCost: {item.m_BlueprintCost} m_PrototypeCost: {item.m_PrototypeCost}");
                    output.Add($"m_Description: {item.m_Description}");
                    try { output.Add($"Ability IDs: " + item.m_AbilityIDs.Select(a => a + ":" + Manager.GetAbilityManager().GetAbilityName(a)).Aggregate((x, y) => x + "," + y)); } catch { }
                    try { output.Add($"AbilityMasks: " + item.m_AbilityMasks.Select(a => a.ToString()).Aggregate((x, y) => x + "," + y)); } catch { }
                    try { output.Add($@"Modifiers:
" + item.m_Modifiers.Select(m => $"Type:{m.m_Type} Timeout:{m.m_TimeOut} AmountType:{m.m_AmountModifier} Amount: {m.m_Ammount}").Aggregate((x, y) => x + @",
" + y)); } catch { }
                    output.Add("");

                }
                catch (Exception e)
                {
                    output.Add("Exception caught. Message: " + e.Message);
                }
            }

            return output;
        }

        public static List<string> GetWeaponInfo()
        {
            List<string> output = new List<string>();

            var allItems = Manager.GetWeaponManager().m_WeaponData;

            foreach (var item in allItems.OrderBy(i => i.m_Name))
            {
                try
                {
                    output.Add($"***** m_Name: {item.m_Name} m_Range: {item.m_Range} m_ShootWhileChangeTarget: {item.m_ShootWhileChangeTarget} m_BulletType: {item.m_BulletType} m_Class: {item.m_Class}");
                    output.Add($"m_DefaultAmmo: {item.m_DefaultAmmo} m_InfiniteAmmo: {item.m_InfiniteAmmo} m_MaxAccuracy: {item.m_MaxAccuracy} m_MinAccuracy:{item.m_MinAccuracy} ");
                    try { output.Add($"m_Template: {item.m_Template?.name} m_Weapon: {item.m_Template.m_Weapon.Name()}"); } catch { }

                    try { output.Add($"m_MechTemplate: {item.m_MechTemplate?.name} m_HudeSprite: {item.m_HudeSprite?.name}"); } catch { }
                    try { output.Add($"Ability IDs: " + item.m_Abilities.Select(a => a + ":" + Manager.GetAbilityManager().GetAbilityName(a)).Aggregate((x, y) => x + "," + y)); } catch { }
                    try { output.Add($"AbilityMasks: " + item.m_AbilityMasks.Select(a => a.ToString()).Aggregate((x, y) => x + "," + y)); } catch { }
                    try { output.Add($@"Modifiers:
" + item.m_Modifiers.Select(m => $"Type:{m.m_Type} Timeout:{m.m_TimeOut} AmountType:{m.m_AmountModifier} Amount: {m.m_Ammount}").Aggregate((x, y) => x + @",
" + y)); } catch { }
                    var weapon = item.m_Template.m_Weapon;
                    try { output.Add($"m_ChanceToGibOnDie: {weapon.m_ChanceToGibOnDie} m_ChanceToGibOnHurt: {weapon.m_ChanceToGibOnHurt} m_ChargeEveryShot: {weapon.m_ChargeEveryShot}"); } catch { }
                    try { output.Add($"m_ChargeTime: {weapon.m_ChargeTime} m_ChargeTimer: {weapon.m_ChargeTimer} m_Delayed: {weapon.m_Delayed}"); } catch { }
                    try { output.Add($"m_FireMagicPlaceboSuppressionBullets: {weapon.m_FireMagicPlaceboSuppressionBullets} m_ShootWhileChangeTarget: {weapon.m_ShootWhileChangeTarget}"); } catch { }

                    var ammoAttachments = item.m_Ammo;
                    try { output.Add($"ammoAttachments: " + ammoAttachments.Select(a => $"Ammoattachment:{a} m_ammo_from_pack:{a.m_ammo_from_pack} m_armor_pierce:{a.m_armor_pierce} m_AutoReloadTime: {a.m_AutoReloadTime} " +
                    $"m_burstMax: {a.m_burstMax} m_burstMin: {a.m_burstMin} m_ChanceToCallReinforcements: {a.m_ChanceToCallReinforcements} m_ChargeEveryShot: {a.m_ChargeEveryShot}" +
                    $"m_ChargeTime: {a.m_ChargeTime} m_CritChance: {a.m_CritChance} m_CritDamageMultiplier: {a.m_CritDamageMultiplier} m_DamageRadius: {a.m_DamageRadius}").Aggregate((x, y) => x + @",
" + y)); } catch { }

                    output.Add("");
                }
                catch (Exception e)
                {
                    output.Add("Exception caught. Message: " + e.Message);
                }
            }

            return output;
        }

        public static List<string> GetQuestInfo()
        {
            List<string> output = new List<string>();
            var districtQuests = Manager.GetQuestManager().GetFieldValue<List<List<QuestElement>>>("m_ElementsWaitingToBeEnabled");

            foreach(var district in districtQuests)
            foreach(var quest in district)
            {
                    try { 
                        output.Add($"***** Quest_Name: {quest.name} m_Title: {quest.m_Title} m_MandatoryQuests: {quest.m_MandatoryQuests.Count()} ParentQuest_Title: {quest.ParentQuest?.m_Title} m_VIP: {(quest.m_VIP == null ? "no VIP": "Has VIP")}");
                    } catch { output.Add($"Error for {quest.m_ID}{quest.m_Title}"); }
                    try
                    {
                        output.Add($"m_Hidden: {quest.m_Hidden} m_DescriptionObjects: {quest.m_DescriptionObjects.Count()} m_CompleteElements: {quest.m_CompleteElements.Count()}");
                    }
                    catch { output.Add($"Error for {quest.m_ID}{quest.m_Title}"); }
                    try {
                        output.Add($"** Quest Location: m_ID: {quest.m_Location.m_ID} m_LocationID: {quest.m_Location.m_LocationID} m_DistricFilterType: {quest.m_Location.m_DistricFilterType} " +
                        $"m_Address: {quest.m_Location.m_Address.Select(a => a.ToString()).Aggregate((x, y) => x + "" + y)} tag: {quest.m_Location.tag} m_PickupPointCount: {quest.m_Location.m_PickupPointCount}");
                    } catch { output.Add($"Error for {quest.m_ID}{quest.m_Title}"); }
            }

            return output;
        }

        public static void Log(string log)
        {
            if(isLogging)
            {
                var loglist = FileManager.LoadList(Manager.GetPluginManager().PluginPath + @"\logfile.log");
                loglist.Add($"{DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")} : {log}");
                FileManager.SaveList(loglist, Manager.GetPluginManager().PluginPath + @"\logfile.log");
            }
        }

        public static List<string> GetAbilityEnum()
        {
            List<string> output = new List<string>();

            Manager.GetAbilityManager().GetAbilityNamesAndIDs(out int[] ids, out string[] names);

            output.Add(@"public enum AbilityEnum");
            output.Add(@"{");
            foreach (var id in ids.OrderBy(i => i))
            {
                try
                {
                    string name = Manager.GetAbilityManager().GetName(id);
                    var a = Manager.GetAbilityManager().GetData(id);

                    if (name.Contains("###"))
                    {
                        output.Add($"{a.m_Name.Replace(" ", "_")} = {id},");
                    }
                    else
                    {
                        output.Add($"{name.Replace(" ", "_")} = {id},");
                    }
                }
                catch (Exception e){ output.Add("Exception caught: " + e.Message); }
            }
            output.Add(@"}");
            return output;
        }

        public static List<string> GetAbilityInfo()
        {
            List<string> output = new List<string>();

            Manager.GetAbilityManager().GetAbilityNamesAndIDs(out int[] ids, out string[] names);

            foreach (var id in ids.OrderBy(i => i))
            {

                string name = Manager.GetAbilityManager().GetName(id);
                output.Add($"Ability id: {id} Name: {name}");

                //if (id > 0)
                //    values.Add($"Ability id: {id} Name: {names[0]}");
                //else
                //    values.Add($"Ability id: {id} Name: {Manager.GetAbilityManager().GetName(id)}");

                if (id > 0)// && !name.Contains("###"))
                {
                    var a = Manager.GetAbilityManager().GetData(id);

                    try
                    {
                        output.Add($@"m_ID{a.m_ID} m_Name:{a.m_Name} energyCostWhen: {a.m_EnergyCostWhen} m_UseAmmoWhen: {a.m_UseAmmoWhen} m_TargetRange:{a.m_TargetRange}");
                        try
                        {
                            output.Add($@"m_WeaponCheckMask:{a.m_WeaponCheckMask} m_TargetEffectTimeOuts: {string.Join(",", a.m_TargetEffectTimeOuts?.Select(s => s.ToString()).ToArray())} m_TargetEffects: {string.Join(",", a.m_TargetEffects?.Select(s => s.ToString()).ToArray())} m_RemoveModifiersWhen:{a.m_RemoveModifiersWhen}");
                        } catch (Exception e){ output.Add("Exception caught: "+ e.Message); }
                        //try
                        //{
                        //    output.Add($@"m_WeaponCheckMask:{a.m_WeaponCheckMask} m_TargetEffectTimeOuts: {string.Join(",", a.m_TargetEffectTimeOuts?.Select(s => s.ToString()).ToArray())} m_TargetEffects: {string.Join(",", a.m_TargetEffects?.Select(s => s.ToString()).ToArray())} m_RemoveModifiersWhen:{a.m_RemoveModifiersWhen}");
                        //} catch { }
                        try
                        {
                            output.Add($@"m_Range:{a.m_Range} m_OnMove: {a.m_OnMove?.Select(om => om.ToString()).Aggregate((x, y) => x + "," + y)} m_RangeModifier: {a.m_RangeModifier}");
                        }
                        catch (Exception e) { output.Add("Exception caught: " + e.Message); }

                        try
                        {
                            output.Add($@"m_Modifiers: {a.m_Modifiers?.Select(m => $@"m_Type:{m.m_Type} m_Ammount:{m.m_Ammount} m_AmountModifier:{m.m_AmountModifier} m_TimeOut:{m.m_TimeOut}").Aggregate((x, y) => x + @",
" + y)}");
                        }
                        catch (Exception e) { output.Add("Exception caught: " + e.Message); }
                        try
                        {
                            output.Add($@"m_ModifiersTarget: {a.m_ModifiersTarget?.Select(m => $@"m_Type:{m.m_Type} m_Ammount:{m.m_Ammount} m_AmountModifier:{m.m_AmountModifier} m_TimeOut:{m.m_TimeOut} ").Aggregate((x, y) => x + @",
" + y)}");
                        }
                        catch (Exception e) { output.Add("Exception caught: " + e.Message); }
                        try
                        {
                            output.Add($@"m_RemoveModifiersWhen:{a.m_RemoveModifiersWhen}");
                        }
                        catch (Exception e) { output.Add("Exception caught: " + e.Message); }
                        try
                        {
                            output.Add($@"m_MyEffects: {a.m_MyEffects?.Select(om => om.ToString())?.Aggregate((x, y) => x + "," + y)} m_MyEffectTimeOuts:{a.m_MyEffectTimeOuts?.Select(om => om.ToString())?.Aggregate((x, y) => x + "," + y)}");
                        } catch (Exception e) { output.Add("Exception caught: " + e.Message); }

                        try
                        {
                            output.Add($@"m_RequiresWeaponCheck:{a.m_RequiresWeaponCheck} m_RequiresAgentSelected: {a.m_RequiresAgentSelected} ...");
                        }
                        catch (Exception e) { output.Add("Exception caught: " + e.Message); }
                        output.Add($"End of Ability index: {a.m_ID} Name: { a.Name}");
                        output.Add($"");

                    }
                    catch
                    {

                    }

                    //values.Add($@"KEY: {x.Key} TOKEN: {x.Value.m_token} m_Translation 0: {x.Value.m_Translations[0]}");
                    //values.Add(x.Value.m_Translations[i]);
                }

            }

            return output;
        }

        public static List<string> GetFullTransformHierchy(Transform transform)
        {
            List<string> output = new List<string>();

            try
            {
                var transforms = GetAllTransforms(transform);

                foreach (Transform t in transforms)
                {
                    try
                    {
                        output.Add(GetSingleTransforHierarchy(t));
                    }
                    catch (Exception e)
                    {
                        output.Add("*** Exception cast while trying to get data from child transform " + t.name);
                    }
                }
            }
            catch (Exception e)
            {
                output.Add("*** Exception cast " + e.Message + ". Current transform name is " + transform.name);
            }

            return output;
        }

        public static List<Transform> GetAllTransforms(Transform transform)
        {
            List<Transform> transforms = new List<Transform>();

            foreach(Transform t in transform)
            {
                transforms.Add(t);
                transforms.AddRange(GetAllTransforms(t));
            }

            return transforms;
        }

        public static string GetSingleTransforHierarchy(Transform transform)
        {
            string transformName = transform.name;
            if(transform.parent != null)
            {
                transformName = GetSingleTransforHierarchy(transform.parent)+" => "+ transformName;
            }
            return transformName;
        }

        public static List<string> GetTransformData(Transform transform, bool componentInfo = false, bool isRoot = true)
        {
            List<string> output = new List<string>();

            if(isRoot)
            {
                output.AddRange(GetFullTransformHierchy(transform));
            }

            try
            {
                output.Add("Transform information about transform named: " + transform.name + ". It has " + transform.childCount + " child transforms.");

                if (transform.parent != null)
                {
                    output.Add("Parent transform named: " + transform.parent.name);
                }
                else
                {
                    output[output.Count - 1] += " No parent.";
                }

                List<Component> components = new List<Component>();
                components.AddRange(transform.GetComponents(typeof(Component)));

                output.Add(components.Count + " components on " + transform.name + ":");
                //string info = "";
                // Get all components and list them
                if(componentInfo)
                {
                    foreach (var c in components)
                    {
                        output.Add(transform.name + " component " + components.IndexOf(c) + " name: " + c.ToString());
                        try
                        {
                            output.AddRange(GetComponentInfo(c));
                        }
                        catch(Exception e)
                        {
                            output.Add("Exception thrown: " + e.Message);
                        }
                    }
                }
                else
                    output.AddRange(components.Select(c => "Component " + components.IndexOf(c) + " name: " + c.ToString()));

                //components.Select(c => c.ToString().Remove(0, 14)).Aggregate((x,y) => x +"," +y);
                foreach (Transform t in transform)
                {
                    try
                    {
                        output.AddRange(GetTransformData(t, componentInfo, false));
                    }
                    catch (Exception e)
                    {
                        output.Add("*** Exception cast while trying to get data from child transform " + t.name);
                    }
                }
            }
            catch (Exception e)
            {
                output.Add("*** Exception cast " + e.Message + ". Current transform name is " + transform.name);
            }

            return output;
        }

        public static List<string> GetComponentInfo(Component component)
        {
            List<string> output = new List<string>();

            if (component.GetType() == typeof(UnityEngine.RectTransform))
            {
                var c = (RectTransform)component;
                output.Add("RectTransform component name:" + c.name);
                output.Add("rect:" + c.rect);
                output.Add("anchoredPosition:" + c.anchoredPosition);
                output.Add("anchoredPosition3D:" + c.anchoredPosition3D);
                output.Add("anchorMax:" + c.anchorMax);
                output.Add("anchorMin:" + c.anchorMin);
                output.Add("pivot:" + c.pivot);
                output.Add("sizeDelta:" + c.sizeDelta);
                output.Add("offsetMax:" + c.offsetMax);
                output.Add("offsetMin:" + c.offsetMin);

                output.Add("eulerAngles:" + c.eulerAngles);
                output.Add("position:" + c.position);
                output.Add("rotation:" + c.rotation);

                output.Add("lossyScale:" + c.lossyScale);
                output.Add("localEulerAngles:" + c.localEulerAngles);
                output.Add("localPosition:" + c.localPosition);
                output.Add("localRotation:" + c.localRotation);
                output.Add("localScale:" + c.localScale);
                output.Add("localToWorldMatrix:" + c.localToWorldMatrix);
            }
            else if (component.GetType() == typeof(UnityEngine.Canvas))
            {
                var c = (Canvas)component;
                output.Add("Canvas component name:" + c.name);
                output.Add("isRootCanvas:" + c.isRootCanvas);
                output.Add("pixelPerfect:" + c.pixelPerfect);
                output.Add("sortingOrder:" + c.sortingOrder);
                output.Add("sortingLayerName:" + c.sortingLayerName);
                output.Add("sortingLayerID:" + c.sortingLayerID);

                output.Add("overridePixelPerfect:" + c.overridePixelPerfect);
                output.Add("overrideSorting:" + c.overrideSorting);

                output.Add("tag:" + c.tag);
                output.Add("scaleFactor:" + c.scaleFactor);
                output.Add("renderMode:" + c.renderMode);
                output.Add("renderOrder:" + c.renderOrder);
                output.Add("referencePixelsPerUnit:" + c.referencePixelsPerUnit);
                output.Add("planeDistance:" + c.planeDistance);
                output.Add("sortingGridNormalizedSize:" + c.sortingGridNormalizedSize);
            }
            else if (component.GetType() == typeof(UnityEngine.UI.HorizontalLayoutGroup))
            {
                var c = (UnityEngine.UI.HorizontalLayoutGroup)component;
                output.Add("Canvas component name:" + c.name);
                output.Add("childAlignment:" + c.childAlignment);
                output.Add("childForceExpandHeight:" + c.childForceExpandHeight);
                output.Add("childForceExpandWidth:" + c.childForceExpandWidth);
                output.Add("flexibleHeight:" + c.flexibleHeight);
                output.Add("flexibleWidth:" + c.flexibleWidth);

                output.Add("layoutPriority:" + c.layoutPriority);
                output.Add("minHeight:" + c.minHeight);
                output.Add("minWidth:" + c.minWidth);

                output.Add("padding:" + c.padding);
                output.Add("preferredHeight:" + c.preferredHeight);
                output.Add("preferredWidth:" + c.preferredWidth);
                output.Add("useGUILayout:" + c.useGUILayout);
            }
            else if (component.GetType() == typeof(UnityEngine.UI.VerticalLayoutGroup))
            {
                var c = (UnityEngine.UI.VerticalLayoutGroup)component;
                output.Add("Canvas component name:" + c.name);
                output.Add("childAlignment:" + c.childAlignment);
                output.Add("childForceExpandHeight:" + c.childForceExpandHeight);
                output.Add("childForceExpandWidth:" + c.childForceExpandWidth);
                output.Add("flexibleHeight:" + c.flexibleHeight);
                output.Add("flexibleWidth:" + c.flexibleWidth);

                output.Add("layoutPriority:" + c.layoutPriority);
                output.Add("minHeight:" + c.minHeight);
                output.Add("minWidth:" + c.minWidth);

                output.Add("padding:" + c.padding);
                output.Add("preferredHeight:" + c.preferredHeight);
                output.Add("preferredWidth:" + c.preferredWidth);
                output.Add("useGUILayout:" + c.useGUILayout);
            }
            else if (component.GetType() == typeof(UnityEngine.UI.LayoutElement))
            {
                var c = (UnityEngine.UI.LayoutElement)component;
                output.Add("Canvas component name:" + c.name);
                output.Add("flexibleHeight:" + c.flexibleHeight);
                output.Add("flexibleWidth:" + c.flexibleWidth);

                output.Add("layoutPriority:" + c.layoutPriority);
                output.Add("minHeight:" + c.minHeight);
                output.Add("minWidth:" + c.minWidth);

                output.Add("preferredHeight:" + c.preferredHeight);
                output.Add("preferredWidth:" + c.preferredWidth);
                output.Add("useGUILayout:" + c.useGUILayout);
            }
            else if (component.GetType() == typeof(UnityEngine.UI.ContentSizeFitter))
            {
                var c = (UnityEngine.UI.ContentSizeFitter)component;
                output.Add("Canvas component name:" + c.name);
                output.Add("verticalFit:" + c.verticalFit);
                output.Add("horizontalFit:" + c.horizontalFit);
                output.Add("useGUILayout:" + c.useGUILayout);
            }
            else if (component.GetType() == typeof(UnityEngine.UI.Image))
            {
                var c = (UnityEngine.UI.Image)component;
                output.Add("Canvas component name:" + c.name);
                output.Add("color:" + c.color);
                output.Add(GetMaterialInfo(c.material));
                output.Add("depth:" + c.depth);
                output.Add("flexibleHeight:" + c.flexibleHeight);
                output.Add("flexibleWidth:" + c.flexibleWidth);
                output.Add("layoutPriority:" + c.layoutPriority);
                output.Add("minHeight:" + c.minHeight);
                output.Add("minWidth:" + c.minWidth);
                output.Add("preferredHeight:" + c.preferredHeight);
                output.Add("preferredWidth:" + c.preferredWidth);
                output.Add("useGUILayout:" + c.useGUILayout);
            }
            else if (component.GetType() == typeof(UnityEngine.CanvasRenderer))
            {
                var c = (CanvasRenderer)component;
                output.Add("Canvas component name:" + c.name);
                output.Add("materialCount:" + c.materialCount);
                output.Add("popMaterialCount:" + c.popMaterialCount);
                output.Add("relativeDepth:" + c.relativeDepth);
                output.Add("absoluteDepth:" + c.absoluteDepth);
                output.Add("cull:" + c.cull);

                output.Add("hasPopInstruction:" + c.hasPopInstruction);
                output.Add("hasRectClipping:" + c.hasRectClipping);

                output.Add("tag:" + c.tag);
            }
            else if (component.GetType() == typeof(UnityEngine.UI.Text))
            {
                var c = (UnityEngine.UI.Text)component;
                output.Add("Canvas component name:" + c.name);
                output.Add(GetMaterialInfo(c.material));
                output.Add(GetMaterialInfo(c.materialForRendering));
                output.Add("flexibleHeight:" + c.flexibleHeight);
                output.Add("flexibleWidth:" + c.flexibleWidth);

                output.Add("layoutPriority:" + c.layoutPriority);
                output.Add("minHeight:" + c.minHeight);
                output.Add("minWidth:" + c.minWidth);

                output.Add("padding:" + c.pixelsPerUnit);
                output.Add("preferredHeight:" + c.preferredHeight);
                output.Add("preferredWidth:" + c.preferredWidth);
                output.Add("useGUILayout:" + c.useGUILayout);
                output.Add("tag:" + c.tag);
            }
            return output;
        }

        public static string GetMaterialInfo(Material material)
        {
            string info = "";
            info += "Material name: " + material.name;
            if(material.mainTexture != null)
            {
                info += " Texture name: " + material.mainTexture.name;
                info += " mainTextureOffset: " + material.mainTextureOffset;
                info += " mainTextureScale: " + material.mainTextureScale;
            }

            if (material.shader != null)
            {
                info += " Shader name: " + material.shader.name;
            }

            return info;
        }

        public static string GetVector3Info(Vector3 vector3)
        {
            string info = "";

            info = $"x:{vector3.x}, y:{vector3.y}, z:{vector3.z}";

            return info;
        }

        public static string GetVector2Info(Vector2 vector2)
        {
            string info = "";

            return info;
        }

        public static List<Transform> GetAllChildren(Transform transform)
        {
            List<string> output = new List<string>();
            List<Transform> transforms = new List<Transform>();

            try
            {
                output.Add("Transform information about transform named: " + transform.name + ". It has " + transform.childCount + " child transforms.");

                if (transform.parent != null)
                {
                    output.Add("Parent transform named: " + transform.parent.name);
                }
                else
                {
                    output[output.Count - 1] += " No parent.";
                }

                List<Component> components = new List<Component>();
                components.AddRange(transform.GetComponents(typeof(Component)));

                output.Add(components.Count + " components on " + transform.name + ":");
                //string info = "";
                // Get all components and list them
                output.AddRange(components.Select(c => "Component " + components.IndexOf(c) + " name: " + c.ToString()));
                //components.Select(c => c.ToString().Remove(0, 14)).Aggregate((x,y) => x +"," +y);
                foreach (Transform t in transform)
                {
                    try
                    {
                        transforms.Add(t);
                        transforms.AddRange(GetAllChildren(t));
                        output.AddRange(GetTransformData(t));
                    }
                    catch (Exception e)
                    {
                        output.Add("*** Exception cast while trying to get data from child transform " + t.name);
                    }
                }
            }
            catch (Exception e)
            {
                output.Add("*** Exception cast " + e.Message + ". Current transform name is " + transform.name);
            }
            FileManager.SaveList(output, Manager.GetPluginManager().PluginPath+ @"\debugtext.txt");
            return transforms;
        }

        public static void SaveLangData()
        {
            var langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");

            var langs = Enum.GetValues(typeof(TextManager.Language)).Cast<TextManager.Language>().ToList();

            for (int i = 1; i < langs.Count() - 1; i++)
            {
                List<string> values = new List<string>();
                foreach (var x in langLookup)
                {
                    values.Add($@"KEY: {x.Key} TOKEN: {x.Value.m_token} m_Translation 0: {x.Value.m_Translations[0]}");
                    values.Add(x.Value.m_Translations[i]);
                }

                FileManager.SaveList(values, Manager.GetPluginManager().PluginPath + $@"\LangLoolupList_{langs[i]}.txt");
            }


            //typeof(AbilityThrowProjectile).GetField("m_Range", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(ability, fogRange * 3);
            //Manager.GetUIManager().ShowMessagePopup("Getting fast lang dict", 5);

            //langLookup = (Dictionary<string, TextManager.LocElement>)TextManager.Get().GetMemberValue("m_FastLanguageLookup"); // .GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");

            //if (langLookup == null || !langLookup.Any())
            //    langLookup = (Dictionary<string, TextManager.LocElement>)ReflectionExtensions.GetMemberValue(TextManager.Get(), "m_FastLanguageLookup");

            //if (langLookup == null || !langLookup.Any())
            //    Manager.GetUIManager().ShowMessagePopup("No lang data");

            //FileManager.SaveObjectAsJson(langLookup, "localizationfile1");
            //langLookup.
        }

        public static List<string> SaveAbilityData()
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
            return values;
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
    }
}
