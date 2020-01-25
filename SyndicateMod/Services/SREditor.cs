using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace SyndicateMod.Services
{
    public static class SREditor
    {
        public static Button CreateButton(UnityAction method)
        {
            GameObject mCanvas = GameObject.Find("Canvas");

            GameObject button = new GameObject();

            button.AddComponent<CanvasRenderer>();
            button.AddComponent<RectTransform>();
            Button mButton = button.AddComponent<Button>();
            Image mImage = button.AddComponent<Image>();
            mButton.targetGraphic = mImage;

            button.transform.position = new Vector3(0, 0, 0);
            button.transform.SetParent(mCanvas.transform);
            button.GetComponent<Button>().onClick.AddListener(method);
            return mButton;
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
    }
}
