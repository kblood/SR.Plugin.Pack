using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

namespace SyndicateMod.Services
{
    public static class SREditor
    {
        public static Button CreateButton(UnityAction method, ref List<string> output)
        {
            output.Add("Finding Canvas");
            GameObject mCanvas = new GameObject();
            mCanvas.AddComponent<Canvas>();
            mCanvas.AddComponent<RectTransform>();
            mCanvas.AddComponent<CanvasScaler>();

            GameObject button = new GameObject();

            output.Add("Adding components");
            button.AddComponent<CanvasRenderer>();
            button.AddComponent<RectTransform>();

            Button mButton = button.AddComponent<Button>();

            output.Add("Adding setting image");

            Image mImage = button.AddComponent<Image>();
            mButton.targetGraphic = mImage;

            //output.Add("Setting canvas as parent");


            button.transform.SetParent(mCanvas.transform);
            button.transform.position = new Vector3(0, 0, 0);

            button.GetComponent<Button>().onClick.AddListener(method);
            return mButton;
        }

        public static List<string> UpdateEnum(Type enumType, Dictionary<int, string> enumBody)
        {
            // sample "file":
            //string fileContent = @"
            //                        btn1 = 0,
            //                        btn2 = 1,
            //                        btn3 = 2,
            //                        ";
            //var enumBody = fileContent.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
            //    .Select(line => new { bothToken = line.Trim().Trim(',').Split('=') })
            //    .Where(x => x.bothToken.Length == 2)
            //    .Select(x => new { Name = x.bothToken[0].Trim(), Value = int.Parse(x.bothToken[1].Trim()) });

            AppDomain currentDomain = AppDomain.CurrentDomain;
            AssemblyName asmName = new AssemblyName("EnumAssembly");
            AssemblyBuilder asmBuilder = currentDomain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder mb = asmBuilder.DefineDynamicModule(asmName.Name, asmName.Name + ".dll");
            string enumTypeName = string.Format("{0}.{1}", enumType.Namespace, enumType.Name);
            EnumBuilder eb = mb.DefineEnum(enumTypeName, TypeAttributes.Public, typeof(int));
            foreach (var element in enumBody)
            {
                FieldBuilder fb1 = eb.DefineLiteral(element.Value, element.Key);
            }

            { FieldBuilder fb2 = eb.DefineLiteral("None", enumBody.Count - 1); }

            Type eType = eb.CreateType();

            List<string> output = new List<string>();

            foreach (object obj in Enum.GetValues(eType))
            {
                Console.WriteLine("{0}.{1} = {2}", eType, obj, ((int)obj));
                output.Add($"{eType}.{obj} = {((int)obj)}");
            }
            return output;
        }
        //public static void Save()
        //{
        //    PrefabUtility
        //    var go = Section.activeGameObject;

        //    var mt = go.GetComponent<MeshFilter>.mesh;
        //    var word1 = Random.Range(0, pent.length - 1);
        //    AssetDatabase.CreateAsset(mt, "Assets/savedplat/assets/" + "-= " + writeName + mt.vertices.length + pent[word1] + "_ " + ".asset");


        //    var prefab = EditorUtility.CreateEmptyPrefab("Assets/savedplat/prefabs/" + "-= " + writeName + mt.vertices.length + pent[word1] + "_ " + ".prefab");
        //    EditorUtility.ReplacePrefab(go, prefab);
        //    AssetDatabase.Refresh();

        //    var comptime = System.DateTime.Now.ToString("MM-dd_HH:mm:ss");
        //    print("saved " + "-= " + writeName + mt.vertices.length + pent[word1] + ".files......  time was: " + comptime);
        //}

        public static void EditItemDescription()
        {
            //typeof(AbilityThrowProjectile).GetField("m_Range", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(ability, fogRange * 3);
            //FileManager.SaveObjectAsJson("test", "test");
            //FileManager.SaveObjectAsJson(Manager.GetPluginManager(), Manager.GetPluginManager().PluginPath+ "PluginManager");
            var langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");

            int itemNumber = 64;
            string name = @"Lincoln P97 Enchanced";
            string company = @"Caldor Inc.";
            string description = @"Wassup! This standard-issue ballistic pistol is the sidearm of choice for nearly all security forces. The current design is the 7th in the series, and while it is renowned for its reliability and low - cost, the last 3 designs are criticized to have been mere cosmetic changes, with the manufacturer accused of spending more time and money marketing the new look rather than practically improving the design of the weapon.
Regardless, it's fast, reliable, cheap, and uses standard micro-round bullets, making it a must-have for anyone who wants to shoot things. Also some changes by Caldor.";

            langLookup[$"ITEM_{itemNumber}_NAME"].m_Translations[2] = name;
            langLookup[$"ITEM_{itemNumber}_COMPANY"].m_Translations[2] = company;
            langLookup[$"ITEM_{itemNumber}_DESCRIPTION"].m_Translations[2] = description;

            //            string hijackText = @"TOKEN: SKILL_HIJACK_TITLE TXT: Hijack
            // TOKEN: SKILL_HIJACK_00 TXT: Hijack the neural implants of other citizens, giving you full control of them. Upgrades allow more simultaneous hijacks and stronger targets.
            //Each hijacked unit reduces energy regeneration. Better-equipped units require higher energy rates to maintain, so be wary of how many people you are hijacking.  Hijacking too many targets simultaneously will result in slower reaction times and lowered weapon accuracy in the hijacked unit.";

            langLookup[$"SKILL_HIJACK_TITLE"].m_Translations[2] = "Persuade";
            langLookup[$"SKILL_HIJACK_00"].m_Translations[2] = @"Persuade the neural implants of other citizens, giving you full control of them. Upgrades allow more simultaneous hijacks and stronger targets. Each hijacked unit reduces energy regeneration. Better-equipped units require higher energy rates to maintain, so be wary of how many people you are hijacking.  Hijacking too many targets simultaneously will result in slower reaction times and lowered weapon accuracy in the hijacked unit.";

            var translations = langLookup[$"SKILL_HIJACK_00"].m_Translations;

            TextManager.LocElement loc = new TextManager.LocElement();
            loc.m_token = "SKILL_HIJACK_01";
            loc.m_Translations = translations.ToList().Select(t => t).ToArray();
            //langLookup[$"SKILL_HIJACK_00"]
            loc.m_Translations[2] = loc.m_Translations[2].Replace("Hijack", "Persuade");
            loc.m_Translations[2] = loc.m_Translations[2].Replace("hijack", "hersuade");
            langLookup.Add("SKILL_HIJACK_01", loc);

            //ReflectionExtensions.SetMemberValue(TextManager.Get(), "m_FastLanguageLookup", langLookup);

            //           string pistolDescription = @"TOKEN: ITEM_64_NAME TXT: Lincoln P97
            // TOKEN: ITEM_64_COMPANY TXT: Lincoln Ballistics
            // TOKEN: ITEM_64_DESCRIPTION TXT: This standard-issue ballistic pistol is the sidearm of choice for nearly all security forces.
            //The current design is the 7th in the series, and while it is renowned for its reliability and low - cost, the last 3 designs are criticized to have been mere cosmetic changes, with the manufacturer accused of spending more time and money marketing the new look rather than practically improving the design of the weapon.
            //Regardless, it's fast, reliable, cheap, and uses standard micro-round bullets, making it a must-have for anyone who wants to shoot things.";


            //langLookup.First().Value = new TextManager.LocElement() { m_token = "", m_Translations = new string[] { } };

            //langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");

            //FileManager.SaveObjectAsJson(langLookup, "localizationfile2");

            //FileManager.SaveObjectAsJson(Manager.GetAchievements(), "StatsAndArchievements");

            //var abilities = Manager.GetAbilityManager().GetFieldValue<Dictionary<int, Ability.AbilityData>>("m_ALLAbilityData");

            //FileManager.SaveObjectAsJson(abilities, "Abilities");
            //FileManager.SaveObjectAsJson(Manager.GetPluginManager(), "PluginManager");
        }
    }
}
