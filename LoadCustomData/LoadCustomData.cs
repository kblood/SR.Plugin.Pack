using LoadCustomDataMod.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using dto = LoadCustomDataMod.DTOs;

/// <summary>
/// Example Satellite Reign LoadCustomData mod.
/// </summary>
public class LoadCustomData : ISrPlugin
{
    /// <summary>
    /// Plugin initialization 
    /// </summary>
    public void Initialize()
    {
        Debug.Log("Initializing Satellite Reign LoadCustomData mod");
        try
        {
            var translations = FileManager.LoadTranslationsXML("Translations.xml");
            var langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");

            foreach (var e in translations)
            {
                if (langLookup.ContainsKey(e.Key))
                {
                    langLookup[e.Key] = e.Element;
                }
                else
                {
                    langLookup.Add(e.Key, e.Element);
                }
            }
            SRInfoHelper.Log("Updated LangLookup with new translations");

            var items = FileManager.LoadXML("ItemData.xml");

            SRInfoHelper.isLogging = false;

            var remappedItems = items.Select(d => SRMapper.ReflectionObjectBuilder<ItemManager.ItemData>(d)).ToList();

            Manager.GetItemManager().m_ItemDefinitions = remappedItems;
        }
        catch (Exception e)
        {
            SRInfoHelper.isLogging = true;
            SRInfoHelper.Log("Exception thrown while serializing: " + e.Message + " inner: " + e.InnerException);

            SRInfoHelper.isLogging = false;
            System.IO.Directory.CreateDirectory(FileManager.FilePathCheck($@"icons\"));
            ExportData();
        }

        SRInfoHelper.isLogging = true;
        SRInfoHelper.Log("Initialized");
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
        if (Manager.Get().GameInProgress)
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                var newItems =Manager.GetItemManager().m_ItemDefinitions.Where(d => d.m_ID > 146).ToList();
                if(newItems.Any())
                {
                    foreach(var item in newItems)
                    {
                        item.PlayerHasPrototype = true;
                        item.m_Count = 1;
                    }
                }

            }
        }
    }

    public void ExportData()
    {
        try
        {
            var data = Manager.GetItemManager().m_ItemDefinitions.OrderBy(d => d.m_ID).Select(d => SRMapper.ReflectionObjectBuilder<dto.ItemData>(d)).ToList();
            var langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");

            var mappedTranslations = langLookup.OrderBy(l => l.Key).Select(l => new dto.TranslationElementDTO() { Key = l.Key, Element = l.Value }).ToList();

            try
            {
                FileManager.SaveAsXML(data, "ItemData.xml");
                FileManager.SaveAsXML(mappedTranslations, "Translations.xml");
            }
            catch (Exception e)
            {
                SRInfoHelper.Log("Exception thrown while serializing: " + e.Message);
            }
        }
        catch (Exception e)
        {
            SRInfoHelper.isLogging = true;
            SRInfoHelper.Log("Exception thrown while mapping. Message: " + e.Message + " --InnerException: " + e.InnerException);
        }
    }

    public string GetName()
    {
        return "Load Custom Data Mod.";
    }
}

