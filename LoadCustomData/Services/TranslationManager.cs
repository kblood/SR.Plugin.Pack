using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SRMod.Services;

public class TranslationManager
{
    private const string TranslationFileName = "Translations.json";
    private static string TranslationFilePath => Path.Combine(Application.persistentDataPath, TranslationFileName);

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
            string json = File.ReadAllText(TranslationFilePath);
            TranslationData data = JsonUtility.FromJson<TranslationData>(json);
            return data.ToDictionary();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading translations from file: {e.Message}");
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
            Debug.LogError($"Error loading translations from game: {e.Message}");
            return new Dictionary<string, TextManager.LocElement>();
        }
    }

    private static void SaveTranslationsToFile(Dictionary<string, TextManager.LocElement> translations)
    {
        try
        {
            TranslationData data = new TranslationData(translations);
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(TranslationFilePath, json);
            Debug.Log($"Translations saved to {TranslationFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving translations to file: {e.Message}");
        }
    }
}

[System.Serializable]
public class TranslationData
{
    public List<TranslationEntry> entries = new List<TranslationEntry>();

    public TranslationData() { }

    public TranslationData(Dictionary<string, TextManager.LocElement> translations)
    {
        foreach (var kvp in translations)
        {
            entries.Add(new TranslationEntry(kvp.Key, kvp.Value));
        }
    }

    public Dictionary<string, TextManager.LocElement> ToDictionary()
    {
        return entries.ToDictionary(e => e.key, e => e.ToLocElement());
    }
}

[System.Serializable]
public class TranslationEntry
{
    public string key;
    public string token;
    public List<string> translations = new List<string>();

    public TranslationEntry() { }

    public TranslationEntry(string key, TextManager.LocElement locElement)
    {
        this.key = key;
        this.token = locElement.m_token;
        this.translations = new List<string>(locElement.m_Translations);
    }

    public TextManager.LocElement ToLocElement()
    {
        return new TextManager.LocElement
        {
            m_token = token,
            m_Translations = translations.ToArray()
        };
    }
}