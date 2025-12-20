using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using SatelliteReignModdingTools;
using SatelliteReignModdingTools.Services;
using SRMod.DTOs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

namespace SRMod.Services
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
            var itemManager = Manager.Get().m_ItemManager_template.GetComponent<ItemManager>();

            if(Manager.GetItemManager() != null || Manager.GetItemManager().GetAllItems() != null || Manager.GetItemManager().GetAllItems().Count > 60)
            {
                itemManager = Manager.GetItemManager();
                SRInfoHelper.Log($"Adding item copy of item {itemNumber} to m_ItemManager");
            }
            else
            {
                SRInfoHelper.Log($"Adding item copy of item {itemNumber} to m_ItemManager_template");
            }

            ItemManager.ItemData sourceItem = itemManager.GetAllItems().Where(i => i.m_ID == itemNumber).First();

            ItemManager.ItemData newItem = new ItemManager.ItemData();

            int newitemNumber = itemManager.GetAllItems().Max(i => i.m_ID)+1;

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

        public static SerializableItemData CopyItem(SerializableItemData activeItemData)
        {
            int itemNumber = activeItemData.m_ID;
            SerializableItemData sourceItem = activeItemData;

            SerializableItemData newItem = new SerializableItemData();

            int newitemNumber = ItemBrowser.itemDTOs.Max(i => i.m_ID) + 1;

            newItem.m_ID = newitemNumber;

            try { newItem.m_AbilityIDs = sourceItem.m_AbilityIDs.ToList(); } catch { }
            try { newItem.m_AbilityMasks = sourceItem.m_AbilityMasks.ToList(); } catch { }
            try { newItem.m_BlueprintCost = sourceItem.m_BlueprintCost; } catch { }
            try { newItem.m_Cost = sourceItem.m_Cost; } catch { }
            try { newItem.m_GearSubCategory = sourceItem.m_GearSubCategory; } catch { }
            try { newItem.m_MinResearchersRequired = sourceItem.m_MinResearchersRequired; } catch { }
            try { newItem.m_Modifiers = sourceItem.m_Modifiers; } catch { }
            try { newItem.m_PrereqID = sourceItem.m_PrereqID; } catch { }
            try { newItem.m_Slot = sourceItem.m_Slot; } catch { }
            try { newItem.m_UIIconName = sourceItem.m_UIIconName; } catch { }
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
            try { newItem.m_StealthVsCombat = sourceItem.m_StealthVsCombat; } catch { }
            try { newItem.m_ValidWeaponAugmentationWeaponMask = sourceItem.m_ValidWeaponAugmentationWeaponMask; } catch { }
            try { newItem.m_WeaponType = sourceItem.m_WeaponType; } catch { }
            try { } catch { }

            var langLookup = ItemBrowser._translations;
            var name = langLookup.Where(l => l.Key == $"ITEM_{itemNumber}_NAME").First().Element;
            var company = langLookup.Where(l => l.Key == $"ITEM_{itemNumber}_COMPANY").First().Element;
            var desc = langLookup.Where(l => l.Key == $"ITEM_{itemNumber}_DESCRIPTION").First().Element;

            TextManager.LocElement newItemName = new TextManager.LocElement();
            newItemName.m_token = $"ITEM_{newitemNumber}_NAME";
            newItemName.m_Translations = name.m_Translations.ToArray();
            //newItemName.m_Translations[2] = itemName;
            langLookup.Add(new TranslationElementDTO(newItemName.m_token, newItemName));

            try
            {
                TextManager.LocElement newItemCompany = new TextManager.LocElement();
                newItemCompany.m_token = $"ITEM_{newitemNumber}_COMPANY";
                newItemCompany.m_Translations = company.m_Translations.ToArray();
                langLookup.Add(new TranslationElementDTO(newItemCompany.m_token, newItemCompany));
            }
            catch { }

            newItem.m_PlayerHasPrototype = true;
            newItem.m_PlayerHasBlueprints = true;
            newItem.m_Count = 5;

            TextManager.LocElement newItemDescription = new TextManager.LocElement();
            newItemDescription.m_token = $"ITEM_{newitemNumber}_DESCRIPTION";
            newItemDescription.m_Translations = desc.m_Translations.ToArray();
            //newItemDescription.m_Translations[2] = @"Grenade V2 now with even more bang.";
            langLookup.Add(new TranslationElementDTO(newItemDescription.m_token, newItemDescription));
            ItemBrowser.itemDTOs.Add(newItem);

            return newItem;
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

        /// <summary>
        /// Safe item copying method that handles all edge cases and ensures proper ID generation
        /// </summary>
        /// <param name="sourceItem">Item to copy</param>
        /// <param name="allItems">Collection of all existing items</param>
        /// <param name="translations">Translation collection</param>
        /// <returns>New copied item with unique ID</returns>
        public static SerializableItemData CopyItemSafe(SerializableItemData sourceItem, 
            List<SerializableItemData> allItems, 
            List<TranslationElementDTO> translations)
        {
            if (sourceItem == null)
                throw new ArgumentNullException(nameof(sourceItem), "Source item cannot be null");
            
            if (allItems == null)
                throw new ArgumentNullException(nameof(allItems), "Items collection cannot be null");

            try
            {
                SRInfoHelper.Log($"Starting safe copy of item ID: {sourceItem.m_ID}");
                
                var newItem = new SerializableItemData();
                
                // Generate safe unique ID
                newItem.m_ID = ItemIdManager.GenerateUniqueItemId(allItems);
                
                // Copy all properties safely
                CopyItemProperties(sourceItem, newItem);
                
                // Handle translations safely
                CreateTranslationsForNewItem(sourceItem, newItem, translations);
                
                SRInfoHelper.Log($"Safe copy completed: {sourceItem.m_ID} → {newItem.m_ID}");
                return newItem;
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log($"CopyItemSafe failed: {ex}");
                throw;
            }
        }

        /// <summary>
        /// Safely copies all properties from source to target item
        /// </summary>
        private static void CopyItemProperties(SerializableItemData source, SerializableItemData target)
        {
            try
            {
                // Copy collections safely with null checks and validation
                target.m_AbilityIDs = ValidateAbilityIDs(source.m_AbilityIDs);
                target.m_AbilityMasks = ValidateAbilityMasks(source.m_AbilityMasks);
                target.m_Modifiers = ValidateModifiers(source.m_Modifiers);

                // Copy value types
                target.m_FriendlyName = source.m_FriendlyName ?? "New Item";
                target.m_BlueprintCost = source.m_BlueprintCost;
                target.m_Cost = source.m_Cost;
                target.m_GearSubCategory = source.m_GearSubCategory;
                target.m_MinResearchersRequired = source.m_MinResearchersRequired;
                target.m_PrereqID = source.m_PrereqID;
                target.m_Slot = source.m_Slot;
                target.m_WeaponType = source.m_WeaponType;
                target.m_BlueprintProgressionValue = source.m_BlueprintProgressionValue;
                target.m_CurrentResearchCost = source.m_CurrentResearchCost;
                target.m_Expanded = source.m_Expanded;
                target.m_FindBlueprintCost = source.m_FindBlueprintCost;
                target.m_FindPrototypeCost = source.m_FindPrototypeCost;
                target.m_OverrideAmmo = source.m_OverrideAmmo;
                target.m_PlayerCanResearchFromStart = source.m_PlayerCanResearchFromStart;
                target.m_Progression = source.m_Progression;
                target.m_PrototypeCost = source.m_PrototypeCost;
                target.m_PrototypeIsInTheWorld = source.m_PrototypeIsInTheWorld;
                target.m_PrototypeProgressionValue = source.m_PrototypeProgressionValue;
                target.m_PrototypeRandomReleaseStage = source.m_PrototypeRandomReleaseStage;
                target.m_ResearchCost = source.m_ResearchCost;
                target.m_StealthVsCombat = source.m_StealthVsCombat;
                target.m_ValidWeaponAugmentationWeaponMask = source.m_ValidWeaponAugmentationWeaponMask;

                // CRITICAL: Validate and copy icon name safely
                target.m_UIIconName = ValidateIconName(source.m_UIIconName);

                // Copy remaining fields that haven't been copied yet
                target.m_PlayerHasPrototype = source.m_PlayerHasPrototype;
                target.m_PlayerHasBlueprints = source.m_PlayerHasBlueprints;
                target.m_Count = source.m_Count;
                target.m_AvailableToPlayer = source.m_AvailableToPlayer;
                target.m_AvailableFor_ALPHA_BETA_EARLYACCESS = source.m_AvailableFor_ALPHA_BETA_EARLYACCESS;
                target.m_PlayerStartsWithBlueprints = source.m_PlayerStartsWithBlueprints;
                target.m_PlayerStartsWithPrototype = source.m_PlayerStartsWithPrototype;
                target.m_PlayerCanResearchFromStart = source.m_PlayerCanResearchFromStart;
                target.m_ResearchStarted = source.m_ResearchStarted;
                target.m_ItemHasBeenLocated = source.m_ItemHasBeenLocated;
                target.m_ResearchProgress = source.m_ResearchProgress;
                target.m_ResearchTimeToDate = source.m_ResearchTimeToDate;
                target.m_ResearchCostToDate = source.m_ResearchCostToDate;
                target.m_TotalResearchTime = source.m_TotalResearchTime;
                target.m_InHouseResearchersResearching = source.m_InHouseResearchersResearching;
                target.m_ExternalResearchersResearching = source.m_ExternalResearchersResearching;

                SRInfoHelper.Log($"Properties copied successfully for item {target.m_ID} with validated icon '{target.m_UIIconName}'");
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log($"Error copying item properties: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Validates ability IDs to prevent LoadCustomData errors
        /// </summary>
        private static List<int> ValidateAbilityIDs(List<int> abilityIDs)
        {
            if (abilityIDs == null)
                return new List<int>();

            // Filter out invalid ability IDs (negative or excessively high values)
            var validIDs = abilityIDs.Where(id => id >= 0 && id <= 1000).ToList();

            if (validIDs.Count != abilityIDs.Count)
            {
                SRInfoHelper.Log($"Filtered {abilityIDs.Count - validIDs.Count} invalid ability IDs");
            }

            return validIDs;
        }

        /// <summary>
        /// Validates ability masks to prevent LoadCustomData errors
        /// </summary>
        private static List<int> ValidateAbilityMasks(List<int> abilityMasks)
        {
            if (abilityMasks == null)
                return new List<int>();

            // Filter out invalid masks (should be positive integers)
            var validMasks = abilityMasks.Where(mask => mask >= 0).ToList();

            if (validMasks.Count != abilityMasks.Count)
            {
                SRInfoHelper.Log($"Filtered {abilityMasks.Count - validMasks.Count} invalid ability masks");
            }

            return validMasks;
        }

        /// <summary>
        /// Validates modifiers to prevent LoadCustomData errors
        /// </summary>
        private static List<SerializableModifierData> ValidateModifiers(List<SerializableModifierData> modifiers)
        {
            if (modifiers == null)
                return new List<SerializableModifierData>();

            var validModifiers = new List<SerializableModifierData>();

            foreach (var modifier in modifiers)
            {
                if (modifier != null)
                {
                    // Create a safe copy with validated values
                    var validModifier = new SerializableModifierData
                    {
                        m_Type = modifier.m_Type,
                        m_Ammount = float.IsNaN(modifier.m_Ammount) || float.IsInfinity(modifier.m_Ammount) ? 0f : modifier.m_Ammount,
                        m_AmountModifier = modifier.m_AmountModifier,
                        m_TimeOut = float.IsNaN(modifier.m_TimeOut) || float.IsInfinity(modifier.m_TimeOut) ? 0f : modifier.m_TimeOut
                    };
                    validModifiers.Add(validModifier);
                }
            }

            if (validModifiers.Count != modifiers.Count)
            {
                SRInfoHelper.Log($"Filtered {modifiers.Count - validModifiers.Count} invalid modifiers");
            }

            return validModifiers;
        }

        /// <summary>
        /// Validates icon names to prevent LoadCustomData null reference errors
        /// </summary>
        private static string ValidateIconName(string iconName)
        {
            if (string.IsNullOrEmpty(iconName))
            {
                SRInfoHelper.Log("Warning: Empty icon name, using default");
                return "ico_gear_standardammo"; // Known safe default
            }

            // List of validated icon names that are known to exist and work
            var validIcons = new HashSet<string>
            {
                "ico_gear_standardammo", "ico_gear_explosiveammo", "ico_gear_shieldgenerator",
                "ico_gear_stealthgenerator", "ico_gear_energygenerator", "ico_gear_bodyarmor",
                "ico_gear_headarmor", "ico_gear_healthstim", "ico_gear_energystim",
                "ico_weapon_pistol", "ico_weapon_smg", "ico_weapon_rifle", "ico_weapon_shotgun",
                "ico_weapon_minigun", "ico_weapon_grenade", "ico_weapon_rocket",
                "ico_aug_head_standard", "ico_aug_body_standard", "ico_aug_arms_standard", "ico_aug_legs_standard"
            };

            // If the icon name is in our validated list, use it
            if (validIcons.Contains(iconName))
            {
                return iconName;
            }

            // Try to check if the icon file exists
            try
            {
                string iconPath = System.IO.Path.Combine(FileManager.ExecPath, "icons", iconName + ".png");
                if (System.IO.File.Exists(iconPath))
                {
                    SRInfoHelper.Log($"Validated icon file exists: {iconName}");
                    return iconName;
                }
                else
                {
                    SRInfoHelper.Log($"Warning: Icon file not found '{iconName}', using safe default");
                    return "ico_gear_standardammo"; // Safe fallback
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log($"Warning: Could not validate icon '{iconName}': {ex.Message}, using safe default");
                return "ico_gear_standardammo"; // Safe fallback
            }
        }

        /// <summary>
        /// Creates translation entries for the new copied item
        /// </summary>
        private static void CreateTranslationsForNewItem(SerializableItemData sourceItem, 
            SerializableItemData newItem, 
            List<TranslationElementDTO> translations)
        {
            if (translations == null) 
            {
                SRInfoHelper.Log("No translations collection provided, skipping translation copying");
                return;
            }

            try
            {
                SRInfoHelper.Log($"Creating translations for new item {newItem.m_ID}");
                
                // Find source translations
                var sourceName = translations.FirstOrDefault(t => t.Key == $"ITEM_{sourceItem.m_ID}_NAME");
                var sourceDesc = translations.FirstOrDefault(t => t.Key == $"ITEM_{sourceItem.m_ID}_DESCRIPTION");
                var sourceCompany = translations.FirstOrDefault(t => t.Key == $"ITEM_{sourceItem.m_ID}_COMPANY");

                // Create new name translation with " (Copy)" suffix
                if (sourceName?.Element != null)
                {
                    var newNameElement = new TextManager.LocElement
                    {
                        m_token = $"ITEM_{newItem.m_ID}_NAME",
                        m_Translations = (string[])sourceName.Element.m_Translations.Clone()
                    };
                    
                    // Add " (Copy)" to each language translation
                    for (int i = 0; i < newNameElement.m_Translations.Length; i++)
                    {
                        if (!string.IsNullOrEmpty(newNameElement.m_Translations[i]))
                        {
                            newNameElement.m_Translations[i] += " (Copy)";
                        }
                    }
                    
                    translations.Add(new TranslationElementDTO(newNameElement.m_token, newNameElement));
                    SRInfoHelper.Log($"Created name translation: {newNameElement.m_token}");
                }

                // Copy description translation
                if (sourceDesc?.Element != null)
                {
                    var newDescElement = new TextManager.LocElement
                    {
                        m_token = $"ITEM_{newItem.m_ID}_DESCRIPTION",
                        m_Translations = (string[])sourceDesc.Element.m_Translations.Clone()
                    };
                    translations.Add(new TranslationElementDTO(newDescElement.m_token, newDescElement));
                    SRInfoHelper.Log($"Created description translation: {newDescElement.m_token}");
                }

                // Copy company translation
                if (sourceCompany?.Element != null)
                {
                    var newCompanyElement = new TextManager.LocElement
                    {
                        m_token = $"ITEM_{newItem.m_ID}_COMPANY",
                        m_Translations = (string[])sourceCompany.Element.m_Translations.Clone()
                    };
                    translations.Add(new TranslationElementDTO(newCompanyElement.m_token, newCompanyElement));
                    SRInfoHelper.Log($"Created company translation: {newCompanyElement.m_token}");
                }
                
                SRInfoHelper.Log($"Translation creation completed for item {newItem.m_ID}");
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log($"Translation copying failed (non-fatal): {ex.Message}");
                // Continue without translations rather than failing the entire copy operation
            }
        }

    }
}
