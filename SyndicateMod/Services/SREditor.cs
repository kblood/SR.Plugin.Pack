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
        public static void AddNewItem()
        {
            int itemNumber = 30;
            int newitemNumber = 147;
            string itemName = "Grenade V2";

            ItemManager.ItemData sourceItem = Manager.GetItemManager().GetAllItems().Where(i => i.m_ID == itemNumber).First();

            ItemManager.ItemData newItem = new ItemManager.ItemData();

            newItem.m_ID = newitemNumber;
            newItem.m_FriendlyName = itemName;
            //energyGeneratorV2.m_AbilityIDs.Add();
            //energyGeneratorV2.m_LocName = "Energy Generator V3";
            
            try { newItem.m_AbilityIDs = sourceItem.m_AbilityIDs.ToList(); } catch { }
            try { newItem.m_AbilityMasks = sourceItem.m_AbilityMasks.ToList(); } catch { }
            try { newItem.m_BlueprintCost = sourceItem.m_BlueprintCost; } catch { }
            try { newItem.m_Cost = sourceItem.m_Cost; } catch { }
            try { newItem.m_GearSubCategory = sourceItem.m_GearSubCategory; } catch { }
            try { newItem.m_MinResearchersRequired = sourceItem.m_MinResearchersRequired; } catch { }
            try { newItem.m_Modifiers = sourceItem.m_Modifiers.ToArray(); } catch { }
            try { newItem.m_PrereqID = sourceItem.m_PrereqID; } catch { }
            try { newItem.m_Slot = sourceItem.m_Slot; } catch { }
            try { newItem.m_UIIcon = sourceItem.m_UIIcon; } catch { }
            try { newItem.m_BlueprintProgressionValue = sourceItem.m_BlueprintProgressionValue; } catch { }
            try { newItem.m_CurrentResearchCost = sourceItem.m_CurrentResearchCost; } catch { }
            try { newItem.m_Expanded = sourceItem.m_Expanded; } catch { }
            try { newItem.m_FindBlueprintCost = sourceItem.m_FindBlueprintCost; } catch { }
            try { newItem.m_FindPrototypeCost = sourceItem.m_FindPrototypeCost; } catch { }
            try { newItem.m_OverrideAmmo = sourceItem.m_OverrideAmmo; } catch { }
            try { newItem.m_PlayerCanResearchFromStart = sourceItem.m_PlayerCanResearchFromStart; } catch { }
            try { newItem.m_Progression = sourceItem.m_Progression; } catch { }
            try { newItem.m_PrototypeCost = sourceItem.m_PrototypeCost; } catch { }
            try { newItem.m_PrototypeIsInTheWorld = sourceItem.m_PrototypeIsInTheWorld; } catch { }
            try { newItem.m_PrototypeProgressionValue = sourceItem.m_PrototypeProgressionValue; } catch { }
            try { newItem.m_PrototypeRandomReleaseStage = sourceItem.m_PrototypeRandomReleaseStage; } catch { }
            try { newItem.m_ResearchCost = sourceItem.m_ResearchCost; } catch { }
            try { newItem.m_ResearchDataPoints = sourceItem.m_ResearchDataPoints; } catch { }
            try { newItem.m_StealthVsCombat = sourceItem.m_StealthVsCombat; } catch { }
            try { newItem.m_ValidWeaponAugmentationWeaponMask = sourceItem.m_ValidWeaponAugmentationWeaponMask; } catch { }
            try { newItem.m_WeaponType = sourceItem.m_WeaponType; } catch { }
            try {  } catch { }
            try {  } catch { }
            try {  } catch { }
            try {  } catch { }
            try {  } catch { }
            try {  } catch { }
            try {  } catch { }
            try {  } catch { }
            try {  } catch { }

            var langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");
            var name = langLookup.Where(l => l.Key == $"ITEM_{itemNumber}_NAME").First().Value;
            var company = langLookup.Where(l => l.Key == $"ITEM_{itemNumber}_COMPANY").First().Value;
            var desc = langLookup.Where(l => l.Key == $"ITEM_{itemNumber}_DESCRIPTION").First().Value;

            TextManager.LocElement newItemName = new TextManager.LocElement();
            newItemName.m_token = $"ITEM_{newitemNumber}_NAME";
            newItemName.m_Translations = name.m_Translations.ToArray();
            newItemName.m_Translations[2] = itemName;
            langLookup.Add(newItemName.m_token, newItemName);

            try
            {
                TextManager.LocElement newItemCompany = new TextManager.LocElement();
                newItemCompany.m_token = $"ITEM_{newitemNumber}_COMPANY";
                newItemCompany.m_Translations = company.m_Translations.ToArray();
                langLookup.Add(newItemCompany.m_token, newItemCompany);
            }
            catch { }

            newItem.m_PlayerHasPrototype = true;
            newItem.m_PlayerHasBlueprints = true;
            newItem.m_Count = 5;

            TextManager.LocElement newItemDescription = new TextManager.LocElement();
            newItemDescription.m_token = $"ITEM_{newitemNumber}_DESCRIPTION";
            newItemDescription.m_Translations = desc.m_Translations.ToArray();
            newItemDescription.m_Translations[2] = @"Grenade V2 now with even more bang.";
            langLookup.Add(newItemDescription.m_token, newItemDescription);

            Manager.GetItemManager().m_ItemDefinitions.Add(newItem);
        }

        public static ItemManager.ItemData CopyItem(int itemNumber)
        {
            ItemManager.ItemData sourceItem = Manager.GetItemManager().GetAllItems().Where(i => i.m_ID == itemNumber).First();

            ItemManager.ItemData newItem = new ItemManager.ItemData();

            int newitemNumber = Manager.GetItemManager().GetAllItems().Max(i => i.m_ID)+1;

            newItem.m_ID = newitemNumber;
            //energyGeneratorV2.m_AbilityIDs.Add();
            //energyGeneratorV2.m_LocName = "Energy Generator V3";

            try { newItem.m_AbilityIDs = sourceItem.m_AbilityIDs.ToList(); } catch { }
            try { newItem.m_AbilityMasks = sourceItem.m_AbilityMasks.ToList(); } catch { }
            try { newItem.m_BlueprintCost = sourceItem.m_BlueprintCost; } catch { }
            try { newItem.m_Cost = sourceItem.m_Cost; } catch { }
            try { newItem.m_GearSubCategory = sourceItem.m_GearSubCategory; } catch { }
            try { newItem.m_MinResearchersRequired = sourceItem.m_MinResearchersRequired; } catch { }
            try { newItem.m_Modifiers = sourceItem.m_Modifiers.ToArray(); } catch { }
            try { newItem.m_PrereqID = sourceItem.m_PrereqID; } catch { }
            try { newItem.m_Slot = sourceItem.m_Slot; } catch { }
            try { newItem.m_UIIcon = sourceItem.m_UIIcon; } catch { }
            try { newItem.m_BlueprintProgressionValue = sourceItem.m_BlueprintProgressionValue; } catch { }
            try { newItem.m_CurrentResearchCost = sourceItem.m_CurrentResearchCost; } catch { }
            try { newItem.m_Expanded = sourceItem.m_Expanded; } catch { }
            try { newItem.m_FindBlueprintCost = sourceItem.m_FindBlueprintCost; } catch { }
            try { newItem.m_FindPrototypeCost = sourceItem.m_FindPrototypeCost; } catch { }
            try { newItem.m_OverrideAmmo = sourceItem.m_OverrideAmmo; } catch { }
            try { newItem.m_PlayerCanResearchFromStart = sourceItem.m_PlayerCanResearchFromStart; } catch { }
            try { newItem.m_Progression = sourceItem.m_Progression; } catch { }
            try { newItem.m_PrototypeCost = sourceItem.m_PrototypeCost; } catch { }
            try { newItem.m_PrototypeIsInTheWorld = sourceItem.m_PrototypeIsInTheWorld; } catch { }
            try { newItem.m_PrototypeProgressionValue = sourceItem.m_PrototypeProgressionValue; } catch { }
            try { newItem.m_PrototypeRandomReleaseStage = sourceItem.m_PrototypeRandomReleaseStage; } catch { }
            try { newItem.m_ResearchCost = sourceItem.m_ResearchCost; } catch { }
            try { newItem.m_ResearchDataPoints = sourceItem.m_ResearchDataPoints; } catch { }
            try { newItem.m_StealthVsCombat = sourceItem.m_StealthVsCombat; } catch { }
            try { newItem.m_ValidWeaponAugmentationWeaponMask = sourceItem.m_ValidWeaponAugmentationWeaponMask; } catch { }
            try { newItem.m_WeaponType = sourceItem.m_WeaponType; } catch { }
            try { } catch { }

            var langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");
            var name = langLookup.Where(l => l.Key == $"ITEM_{itemNumber}_NAME").First().Value;
            var company = langLookup.Where(l => l.Key == $"ITEM_{itemNumber}_COMPANY").First().Value;
            var desc = langLookup.Where(l => l.Key == $"ITEM_{itemNumber}_DESCRIPTION").First().Value;

            TextManager.LocElement newItemName = new TextManager.LocElement();
            newItemName.m_token = $"ITEM_{newitemNumber}_NAME";
            newItemName.m_Translations = name.m_Translations.ToArray();
            //newItemName.m_Translations[2] = itemName;
            langLookup.Add(newItemName.m_token, newItemName);

            try
            {
                TextManager.LocElement newItemCompany = new TextManager.LocElement();
                newItemCompany.m_token = $"ITEM_{newitemNumber}_COMPANY";
                newItemCompany.m_Translations = company.m_Translations.ToArray();
                langLookup.Add(newItemCompany.m_token, newItemCompany);
            }
            catch { }

            newItem.m_PlayerHasPrototype = true;
            newItem.m_PlayerHasBlueprints = true;
            newItem.m_Count = 5;

            TextManager.LocElement newItemDescription = new TextManager.LocElement();
            newItemDescription.m_token = $"ITEM_{newitemNumber}_DESCRIPTION";
            newItemDescription.m_Translations = desc.m_Translations.ToArray();
            //newItemDescription.m_Translations[2] = @"Grenade V2 now with even more bang.";
            langLookup.Add(newItemDescription.m_token, newItemDescription);
            Manager.GetItemManager().m_ItemDefinitions.Add(newItem);

            return newItem;
        }

        public static void EditItemDescription(int itemNumber, string name, string description = null, string company = null)
        {
            var langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");
            if(!string.IsNullOrEmpty(name))
            langLookup[$"ITEM_{itemNumber}_NAME"].m_Translations[2] = name;
            if (!string.IsNullOrEmpty(company))
                try { langLookup[$"ITEM_{itemNumber}_COMPANY"].m_Translations[2] = company; } catch { }
            if (!string.IsNullOrEmpty(description))
            langLookup[$"ITEM_{itemNumber}_DESCRIPTION"].m_Translations[2] = description;
        }

        public static ModifierData5L CreateNewModifer(ModifierType type, float amount, float timeOut = 0f, ModifierType amountModifier = ModifierType.NONE)
        {
            ModifierData5L modifierData5L = new ModifierData5L();
            modifierData5L.m_Type = type;
            modifierData5L.m_Ammount = amount;
            modifierData5L.m_AmountModifier = ModifierType.NONE;
            modifierData5L.m_TimeOut = 0.0f;

            return modifierData5L;
        }

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


    }
}
