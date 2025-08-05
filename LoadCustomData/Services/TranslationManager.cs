using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SRMod.Services;

public class TranslationManager
{
    private const string TranslationFileName = "translations.xml";
    private static string TranslationFilePath
    {
        get
        {
            return Path.Combine(Manager.GetPluginManager().PluginPath, TranslationFileName);
        }
    }

    public static Dictionary<string, TextManager.LocElement> LoadTranslations()
    {
        Dictionary<string, TextManager.LocElement> translations;

        if (File.Exists(TranslationFilePath))
        {
            translations = LoadTranslationsFromFile();
        }
        else
        {
            translations = LoadTranslationsFromGame();
            SaveTranslationsToFile(translations);
        }

        return translations;
    }

    private static Dictionary<string, TextManager.LocElement> LoadTranslationsFromFile()
    {
        try
        {
            var translationList = FileManager.LoadTranslationsXML(TranslationFileName);
            var translations = new Dictionary<string, TextManager.LocElement>();
            
            foreach (var translation in translationList)
            {
                translations[translation.Key] = translation.Element;
            }
            
            return translations;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading translations from file: " + e.Message);
            return new Dictionary<string, TextManager.LocElement>();
        }
    }

    private static Dictionary<string, TextManager.LocElement> LoadTranslationsFromGame()
    {
        try
        {
            var textManager = TextManager.Get();
            return textManager.GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading translations from game: " + e.Message);
            return new Dictionary<string, TextManager.LocElement>();
        }
    }

    private static void SaveTranslationsToFile(Dictionary<string, TextManager.LocElement> translations)
    {
        try
        {
            var translationList = translations.Select(kvp => new SRMod.DTOs.TranslationElementDTO 
            { 
                Key = kvp.Key, 
                Element = kvp.Value 
            }).ToList();

            FileManager.SaveAsXML(translationList, TranslationFileName);
            Debug.Log("Translations saved to " + TranslationFilePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving translations to file: " + e.Message);
        }
    }
}

// TranslationManager now uses XML serialization through FileManager and DTOs
// Old JSON-based classes removed for consistency