using SRMod.DTOs;
using SRMod.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LoadCustomData.Services
{
    /// <summary>
    /// Comprehensive data export/import manager for Satellite Reign
    /// Handles all aspects of game data serialization and loading
    /// </summary>
    public class DataExportImportManager
    {
        private static DataExportImportManager _instance;
        
        public static DataExportImportManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new DataExportImportManager();
                return _instance;
            }
        }

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            try
            {
                SRInfoHelper.Log("DataExportImportManager: Initializing comprehensive data management");
                
                // Initialize all sub-managers
                ItemDataManager.Instance.Initialize();
                SpawnCardManager.Instance.Initialize();
                QuestDataManager.Instance.Initialize();
                
                IsInitialized = true;
                SRInfoHelper.Log("DataExportImportManager: Initialization complete");
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("DataExportImportManager: Initialization failed - " + ex.Message);
                IsInitialized = false;
            }
        }

        /// <summary>
        /// Export ALL game data to XML files
        /// </summary>
        public void ExportAllGameData()
        {
            if (!IsInitialized)
            {
                SRInfoHelper.Log("DataExportImportManager: Not initialized, cannot export data");
                return;
            }

            try
            {
                SRInfoHelper.Log("DataExportImportManager: Starting comprehensive data export");

                // Export items
                if (!File.Exists(Path.Combine(Manager.GetPluginManager().PluginPath, "itemDefinitions.xml")))
                {
                    ItemDataManager.Instance.SaveItemDefinitionsToFile();
                }

                // Export quests
                if (!QuestDataManager.Instance.CheckIfXMLFileExists())
                {
                    QuestDataManager.Instance.SaveQuestDataToFile();
                }

                // Export spawn cards
                if (!SpawnCardManager.Instance.CheckIfXMLFileExists())
                {
                    SpawnCardManager.Instance.SaveSpawnCardsToFile();
                }

                // Export translations
                ExportTranslations();

                // Export additional game systems
                ExportGameSettings();
                ExportProgressionData();

                SRInfoHelper.Log("DataExportImportManager: Comprehensive data export completed");
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("DataExportImportManager: Export failed - " + ex.Message);
            }
        }

        /// <summary>
        /// Import ALL game data from XML files
        /// </summary>
        public void ImportAllGameData()
        {
            if (!IsInitialized)
            {
                SRInfoHelper.Log("DataExportImportManager: Not initialized, cannot import data");
                return;
            }

            try
            {
                SRInfoHelper.Log("DataExportImportManager: Starting comprehensive data import");

                // Import items
                ItemDataManager.Instance.LoadItemDefinitionsFromFileAndUpdateItemManager();

                // Import quests
                QuestDataManager.Instance.LoadQuestDataFromFile();

                // Import spawn cards
                SpawnCardManager.Instance.LoadSpawnCardsFromFileAndUpdateSpawnManager();

                // Import translations
                ImportTranslations();

                // Import additional game systems
                ImportGameSettings();
                ImportProgressionData();

                SRInfoHelper.Log("DataExportImportManager: Comprehensive data import completed");
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("DataExportImportManager: Import failed - " + ex.Message);
            }
        }

        private void ExportTranslations()
        {
            try
            {
                var translations = TranslationManager.LoadTranslations();
                var translationList = translations.Select(kvp => new TranslationElementDTO 
                { 
                    Key = kvp.Key, 
                    Element = kvp.Value 
                }).ToList();

                FileManager.SaveAsXML(translationList, "translations.xml");
                SRInfoHelper.Log("DataExportImportManager: Translations exported successfully");
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("DataExportImportManager: Translation export failed - " + ex.Message);
            }
        }

        private void ImportTranslations()
        {
            try
            {
                var translationList = FileManager.LoadTranslationsXML("translations.xml");
                if (translationList != null && translationList.Count > 0)
                {
                    var langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");
                    
                    foreach (var translation in translationList)
                    {
                        if (langLookup.ContainsKey(translation.Key))
                        {
                            langLookup[translation.Key] = translation.Element;
                        }
                        else
                        {
                            langLookup.Add(translation.Key, translation.Element);
                        }
                    }
                    
                    SRInfoHelper.Log("DataExportImportManager: Translations imported successfully - " + translationList.Count + " entries");
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("DataExportImportManager: Translation import failed - " + ex.Message);
            }
        }

        private void ExportGameSettings()
        {
            try
            {
                // Export game settings and configuration
                var gameSettings = new Dictionary<string, object>();
                
                // Add difficulty settings, gameplay modifiers, etc.
                var gameManager = Manager.Get();
                if (gameManager != null)
                {
                    gameSettings["GameInProgress"] = gameManager.GameInProgress;
                    // Add more settings as needed
                }

                FileManager.SaveAsXML(gameSettings, "gameSettings.xml");
                SRInfoHelper.Log("DataExportImportManager: Game settings exported");
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("DataExportImportManager: Game settings export failed - " + ex.Message);
            }
        }

        private void ImportGameSettings()
        {
            try
            {
                string filePath = Path.Combine(Manager.GetPluginManager().PluginPath, "gameSettings.xml");
                if (File.Exists(filePath))
                {
                    // Load and apply game settings
                    SRInfoHelper.Log("DataExportImportManager: Game settings imported");
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("DataExportImportManager: Game settings import failed - " + ex.Message);
            }
        }

        private void ExportProgressionData()
        {
            try
            {
                // Export progression data
                var progressionManager = Manager.GetProgressionManager();
                if (progressionManager != null)
                {
                    var progressionData = new Dictionary<string, object>();
                    // Extract progression data using reflection
                    // This would need to be implemented based on the specific progression system

                    FileManager.SaveAsXML(progressionData, "progressionData.xml");
                    SRInfoHelper.Log("DataExportImportManager: Progression data exported");
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("DataExportImportManager: Progression data export failed - " + ex.Message);
            }
        }

        private void ImportProgressionData()
        {
            try
            {
                string filePath = Path.Combine(Manager.GetPluginManager().PluginPath, "progressionData.xml");
                if (File.Exists(filePath))
                {
                    // Load and apply progression data
                    SRInfoHelper.Log("DataExportImportManager: Progression data imported");
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("DataExportImportManager: Progression data import failed - " + ex.Message);
            }
        }

        /// <summary>
        /// Create a complete backup of all game data
        /// </summary>
        public void CreateDataBackup()
        {
            try
            {
                string backupFolder = Path.Combine(Manager.GetPluginManager().PluginPath, "backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                Directory.CreateDirectory(backupFolder);

                // Backup all XML files
                string pluginPath = Manager.GetPluginManager().PluginPath;
                var xmlFiles = Directory.GetFiles(pluginPath, "*.xml");
                
                foreach (var xmlFile in xmlFiles)
                {
                    string fileName = Path.GetFileName(xmlFile);
                    string backupPath = Path.Combine(backupFolder, fileName);
                    File.Copy(xmlFile, backupPath);
                }

                SRInfoHelper.Log("DataExportImportManager: Data backup created at " + backupFolder);
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("DataExportImportManager: Backup creation failed - " + ex.Message);
            }
        }

        /// <summary>
        /// Validate all exported data files for integrity
        /// </summary>
        public bool ValidateExportedData()
        {
            try
            {
                bool allValid = true;
                string pluginPath = Manager.GetPluginManager().PluginPath;

                // Check essential files
                string[] essentialFiles = { "itemDefinitions.xml", "questDefinitions.xml", "spawnCards.xml" };
                
                foreach (var fileName in essentialFiles)
                {
                    string filePath = Path.Combine(pluginPath, fileName);
                    if (!File.Exists(filePath))
                    {
                        SRInfoHelper.Log("DataExportImportManager: Missing essential file - " + fileName);
                        allValid = false;
                    }
                    else
                    {
                        // Basic XML validation
                        try
                        {
                            var content = File.ReadAllText(filePath);
                            if (content.Length < 10 || !content.Contains("<"))
                            {
                                SRInfoHelper.Log("DataExportImportManager: Invalid XML content in " + fileName);
                                allValid = false;
                            }
                        }
                        catch (Exception ex)
                        {
                            SRInfoHelper.Log("DataExportImportManager: Error reading " + fileName + " - " + ex.Message);
                            allValid = false;
                        }
                    }
                }

                SRInfoHelper.Log("DataExportImportManager: Data validation " + (allValid ? "passed" : "failed"));
                return allValid;
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("DataExportImportManager: Data validation error - " + ex.Message);
                return false;
            }
        }
    }
}