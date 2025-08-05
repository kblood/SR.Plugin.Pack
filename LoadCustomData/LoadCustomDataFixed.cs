using SRMod.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using dto = SRMod.DTOs;

namespace LoadCustomDataMod
{
    /// <summary>
    /// Satellite Reign LoadCustomData mod - Fixed version for ISrPlugin compatibility
    /// </summary>
    public class LoadCustomDataPlugin : ISrPlugin
    {
        private bool isInitialized = false;

        /// <summary>
        /// Plugin initialization 
        /// </summary>
        public void Initialize()
        {
            Debug.Log("LoadCustomDataPlugin: Starting initialization");
            SRInfoHelper.Log("LoadCustomDataPlugin: Starting initialization");
            
            try
            {
                // Basic initialization without the complex DataExportImportManager
                var sm = SpawnManager.Get();
                SRInfoHelper.Log("Spawnmanager has " + sm.m_EnemyDefinitions.Count + " m_EnemyDefinitions");

                // Initialize core managers with basic error handling
                try
                {
                    ItemDataManager.Instance.Initialize();
                    if (!File.Exists(Path.Combine(Manager.GetPluginManager().PluginPath, "itemDefinitions.xml")))
                        ItemDataManager.Instance.SaveItemDefinitionsToFile();
                    SRInfoHelper.Log("LoadCustomDataPlugin: ItemDataManager initialized");
                }
                catch (Exception ex)
                {
                    SRInfoHelper.Log("LoadCustomDataPlugin: ItemDataManager failed - " + ex.Message);
                }

                try
                {
                    QuestDataManager.Instance.Initialize();
                    if (!QuestDataManager.Instance.CheckIfXMLFileExists())
                        QuestDataManager.Instance.SaveQuestDataToFile();
                    SRInfoHelper.Log("LoadCustomDataPlugin: QuestDataManager initialized");
                }
                catch (Exception ex)
                {
                    SRInfoHelper.Log("LoadCustomDataPlugin: QuestDataManager failed - " + ex.Message);
                }

                try
                {
                    SpawnCardManager.Instance.Initialize();
                    if (!SpawnCardManager.Instance.CheckIfXMLFileExists())
                        SpawnCardManager.Instance.SaveSpawnCardsToFile();
                    SRInfoHelper.Log("LoadCustomDataPlugin: SpawnCardManager initialized");
                }
                catch (Exception ex)
                {
                    SRInfoHelper.Log("LoadCustomDataPlugin: SpawnCardManager failed - " + ex.Message);
                }

                // Load existing translations
                try
                {
                    var translations = TranslationManager.LoadTranslations();
                    var langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");

                    foreach (var kvp in translations)
                    {
                        if (langLookup.ContainsKey(kvp.Key))
                        {
                            langLookup[kvp.Key] = kvp.Value;
                        }
                        else
                        {
                            langLookup.Add(kvp.Key, kvp.Value);
                        }
                    }
                    SRInfoHelper.Log("LoadCustomDataPlugin: Translations loaded");
                }
                catch (Exception ex)
                {
                    SRInfoHelper.Log("LoadCustomDataPlugin: Translation loading failed - " + ex.Message);
                }

                isInitialized = true;
                SRInfoHelper.Log("LoadCustomDataPlugin: Initialization complete");
                
                // Show success message to player
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("LoadCustomData mod loaded successfully!", 3);
                }
            }
            catch (Exception e)
            {
                SRInfoHelper.Log("LoadCustomDataPlugin: Critical initialization error - " + e.Message);
                Debug.LogError("LoadCustomDataPlugin: Critical initialization error - " + e.Message);
                
                // Create basic directory structure on failure
                try
                {
                    System.IO.Directory.CreateDirectory(FileManager.FilePathCheck(@"icons\"));
                }
                catch { }
            }
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            if (!isInitialized) return;

            try
            {
                // Simplified hotkey handling
                if (Input.GetKeyDown(KeyCode.Insert))
                {
                    SRInfoHelper.Log("LoadCustomDataPlugin: Manual reinitialization triggered");
                    try
                    {
                        ItemDataManager.Instance.Initialize();
                        QuestDataManager.Instance.Initialize();
                        SpawnCardManager.Instance.Initialize();
                        Manager.GetUIManager()?.ShowMessagePopup("Data managers reinitialized!", 3);
                    }
                    catch (Exception ex)
                    {
                        SRInfoHelper.Log("LoadCustomDataPlugin: Reinitialization failed - " + ex.Message);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    SRInfoHelper.Log("LoadCustomDataPlugin: Manual data export triggered");
                    try
                    {
                        ItemDataManager.Instance.SaveItemDefinitionsToFile();
                        QuestDataManager.Instance.SaveQuestDataToFile();
                        SpawnCardManager.Instance.SaveSpawnCardsToFile();
                        Manager.GetUIManager()?.ShowMessagePopup("Game data exported!", 3);
                    }
                    catch (Exception ex)
                    {
                        SRInfoHelper.Log("LoadCustomDataPlugin: Export failed - " + ex.Message);
                        Manager.GetUIManager()?.ShowMessagePopup("Export failed - see logs", 3);
                    }
                }

                if (Input.GetKeyDown(KeyCode.End))
                {
                    SRInfoHelper.Log("LoadCustomDataPlugin: Manual data import triggered");
                    try
                    {
                        ItemDataManager.Instance.LoadItemDefinitionsFromFileAndUpdateItemManager();
                        QuestDataManager.Instance.LoadQuestDataFromFile();
                        SpawnCardManager.Instance.LoadSpawnCardsFromFileAndUpdateSpawnManager();
                        Manager.GetUIManager()?.ShowMessagePopup("Game data imported!", 3);
                    }
                    catch (Exception ex)
                    {
                        SRInfoHelper.Log("LoadCustomDataPlugin: Import failed - " + ex.Message);
                        Manager.GetUIManager()?.ShowMessagePopup("Import failed - see logs", 3);
                    }
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("LoadCustomDataPlugin: Update error - " + ex.Message);
            }
        }

        public string GetName()
        {
            return "LoadCustomData Plugin";
        }
    }
}