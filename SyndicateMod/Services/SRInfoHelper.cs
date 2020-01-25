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

        public static List<string> GetTransformData(Transform transform)
        {
            List<string> output = new List<string>();

            try
            {
                output.Add("Transform information about transform named: " + transform.name + ". It has " + transform.childCount + " child transforms.");

                if (transform.parent != null)
                {
                    output.Add("Parent transform named: " + transform.parent.name);
                }
                else
                {
                    output[output.Count-1] += " No parent.";
                }

                List<Component> components = new List<Component>();
                components.AddRange(transform.GetComponents(typeof(Component)));

                output.Add(components.Count + " components on " + transform.name + ":");
                //string info = "";
                // Get all components and list them
                output.AddRange(components.Select(c => "Component "+components.IndexOf(c)+" name: " + c.ToString()));
                //components.Select(c => c.ToString().Remove(0, 14)).Aggregate((x,y) => x +"," +y);
                foreach (Transform t in transform)
                {
                    try
                    {
                        output.AddRange(GetTransformData(t));
                    }
                    catch (Exception e)
                    {
                        output.Add("*** Exception cast while trying to get data from child transform " + t.name);
                    }
                }
            }
            catch(Exception e)
            {
                output.Add("*** Exception cast " + e.Message + ". Current transform name is " + transform.name);
            }

            return output;
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
