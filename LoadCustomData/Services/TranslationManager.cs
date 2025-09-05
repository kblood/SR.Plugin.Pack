using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SRMod.Services;
using SRMod.DTOs;

public class TranslationManager
{
    private const string TranslationFileName = "Translations.json";
    private static string TranslationFilePath => Path.Combine(Manager.GetPluginManager().PluginPath, TranslationFileName);

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
            Debug.Log("TranslationManager: Starting LoadTranslationsFromGame");
            var textManager = TextManager.Get();
            if (textManager == null)
            {
                Debug.LogError("TranslationManager: TextManager.Get() returned null");
                return new Dictionary<string, TextManager.LocElement>();
            }
            
            Debug.Log("TranslationManager: TextManager obtained successfully");
            
            // First try to get the field using reflection with better error handling
            var textManagerType = textManager.GetType();
            Debug.Log($"TranslationManager: TextManager type: {textManagerType.Name}");
            
            // List all available fields for debugging
            var allFields = textManagerType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            Debug.Log("TranslationManager: Available fields in TextManager:");
            foreach (var field in allFields)
            {
                Debug.Log($"TranslationManager:   - {field.Name} ({field.FieldType.Name})");
            }
            
            // Try different possible field names
            string[] possibleFieldNames = { "m_FastLanguageLookup", "m_fastLanguageLookup", "FastLanguageLookup", "languageLookup", "m_LanguageLookup" };
            
            foreach (var fieldName in possibleFieldNames)
            {
                try
                {
                    var field = textManagerType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    if (field != null)
                    {
                        Debug.Log($"TranslationManager: Found field '{fieldName}' of type {field.FieldType.Name}");
                        var fieldValue = field.GetValue(textManager);
                        
                        if (fieldValue is Dictionary<string, TextManager.LocElement> dict)
                        {
                            Debug.Log($"TranslationManager: Successfully loaded {dict.Count} translations from field '{fieldName}'");
                            return dict;
                        }
                        else
                        {
                            Debug.LogWarning($"TranslationManager: Field '{fieldName}' is not the expected Dictionary type, it's {fieldValue?.GetType().Name ?? "null"}");
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogWarning($"TranslationManager: Failed to access field '{fieldName}': {ex.Message}");
                }
            }
            
            // Fallback - try the original extension method
            try
            {
                var result = textManager.GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");
                if (result != null && result.Count > 0)
                {
                    Debug.Log($"TranslationManager: Extension method fallback successful with {result.Count} translations");
                    return result;
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"TranslationManager: Extension method fallback failed: {ex.Message}");
            }
            
            Debug.LogError("TranslationManager: Could not find any suitable translation dictionary field");
            return new Dictionary<string, TextManager.LocElement>();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"TranslationManager: Error loading translations from game: {e.Message}");
            Debug.LogError($"TranslationManager: Stack trace: {e.StackTrace}");
            return new Dictionary<string, TextManager.LocElement>();
        }
    }

    /// <summary>
    /// Ensure translation keys exist for an item using game's naming pattern
    /// </summary>
    public static void EnsureItemTranslationKeys(ItemManager.ItemData item)
    {
        try
        {
            var textManager = TextManager.Get();
            if (textManager == null)
            {
                Debug.LogError($"TranslationManager: TextManager not available for item {item.m_ID}");
                return;
            }

            // Try to get the language lookup dictionary with improved error handling
            Dictionary<string, TextManager.LocElement> langLookup = null;
            
            try
            {
                langLookup = textManager.GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"TranslationManager: Failed to get language lookup via extension method: {ex.Message}");
                
                // Try direct reflection as backup
                var textManagerType = textManager.GetType();
                var field = textManagerType.GetField("m_FastLanguageLookup", BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    var fieldValue = field.GetValue(textManager);
                    if (fieldValue is Dictionary<string, TextManager.LocElement> dict)
                    {
                        langLookup = dict;
                        Debug.Log($"TranslationManager: Successfully got language lookup via direct reflection");
                    }
                }
            }
            
            if (langLookup == null)
            {
                Debug.LogError($"TranslationManager: Could not access language lookup dictionary for item {item.m_ID}");
                return;
            }
            
            // Use game's standard key pattern
            string nameKey = "ITEM_" + item.m_ID + "_NAME";
            string descKey = "ITEM_" + item.m_ID + "_DESCRIPTION";
            
            // Auto-create missing name translation
            if (!langLookup.ContainsKey(nameKey))
            {
                var nameElement = new TextManager.LocElement
                {
                    m_token = nameKey,
                    m_Translations = new string[8] // Support all 8 languages
                };
                nameElement.m_Translations[0] = item.m_FriendlyName ?? ("Item " + item.m_ID); // English
                langLookup[nameKey] = nameElement;
                Debug.Log($"TranslationManager: Created translation key: {nameKey}");
            }
            
            // Auto-create missing description translation  
            if (!langLookup.ContainsKey(descKey))
            {
                var descElement = new TextManager.LocElement
                {
                    m_token = descKey,
                    m_Translations = new string[8]
                };
                descElement.m_Translations[0] = "Description for " + (item.m_FriendlyName ?? ("Item " + item.m_ID));
                langLookup[descKey] = descElement;
                Debug.Log($"TranslationManager: Created translation key: {descKey}");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"TranslationManager: Error ensuring translation keys for item {item.m_ID}: {e.Message}");
            Debug.LogError($"TranslationManager: Stack trace: {e.StackTrace}");
        }
    }

    public static void SaveTranslationsToFile(Dictionary<string, TextManager.LocElement> translations)
    {
        try
        {
            if (translations == null)
            {
                Debug.LogError("TranslationManager: Cannot save null translations dictionary");
                return;
            }
            
            if (translations.Count == 0)
            {
                Debug.LogWarning("TranslationManager: Saving empty translations dictionary");
            }
            
            var translationList = translations.Select(kvp => new TranslationElementDTO 
            { 
                Key = kvp.Key, 
                Element = kvp.Value 
            }).ToList();

            FileManager.SaveAsXML(translationList, "translations.xml");
            SRInfoHelper.Log($"TranslationManager: Translations exported successfully ({translations.Count} entries)");
        }
        catch (System.Exception e)
        {
            SRInfoHelper.Log($"TranslationManager: Error saving translations to file: {e.Message}");
            SRInfoHelper.Log($"TranslationManager: Stack trace: {e.StackTrace}");
        }
    }

    public static void UpdateGameTranslations(List<TranslationElementDTO> translationElements)
    {
        try
        {
            Debug.Log($"TranslationManager: Starting to update game translations with {translationElements.Count} elements");
            
            var textManager = TextManager.Get();
            if (textManager == null)
            {
                Debug.LogError("TranslationManager: TextManager.Get() returned null");
                return;
            }

            var langLookup = textManager.GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");
            if (langLookup == null)
            {
                Debug.LogError("TranslationManager: Failed to get m_FastLanguageLookup from TextManager");
                return;
            }

            int updatedCount = 0;
            int newCount = 0;

            foreach (var translationElement in translationElements)
            {
                if (langLookup.ContainsKey(translationElement.Key))
                {
                    langLookup[translationElement.Key] = translationElement.Element;
                    updatedCount++;
                    Debug.Log($"TranslationManager: Updated translation '{translationElement.Key}' -> '{translationElement.Element.m_Translations[2]}'");
                }
                else
                {
                    langLookup[translationElement.Key] = translationElement.Element;
                    newCount++;
                    Debug.Log($"TranslationManager: Added new translation '{translationElement.Key}' -> '{translationElement.Element.m_Translations[2]}'");
                }
            }

            Debug.Log($"TranslationManager: Translation update complete - updated {updatedCount}, added {newCount}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"TranslationManager: Error updating game translations: {e.Message}");
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
        if (translations != null)
        {
            foreach (var kvp in translations)
            {
                if (kvp.Value != null)
                {
                    entries.Add(new TranslationEntry(kvp.Key, kvp.Value));
                }
            }
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
        this.key = key ?? "";
        this.token = locElement?.m_token ?? "";
        this.translations = locElement?.m_Translations != null ? new List<string>(locElement.m_Translations) : new List<string>();
    }

    public TextManager.LocElement ToLocElement()
    {
        return new TextManager.LocElement
        {
            m_token = token ?? "",
            m_Translations = translations?.ToArray() ?? new string[0]
        };
    }
}