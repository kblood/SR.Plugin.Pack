using SRMod.Services;
using LoadCustomData.Services;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace LoadCustomData
{
    /// <summary>
    /// Enhanced LoadCustomData mod with improved error handling and .NET 2.0 compatibility
    /// </summary>
    public class LoadCustomDataEnhanced : ISrPlugin
    {
        private bool isInitialized = false;
        private bool initializationFailed = false;

        /// <summary>
        /// Plugin initialization with comprehensive error handling
        /// </summary>
        public void Initialize()
        {
            Debug.Log("LoadCustomDataEnhanced: Starting initialization");
            SRInfoHelper.Log("LoadCustomDataEnhanced: Starting initialization");
            
            try
            {
                // Initialize core directories
                CreateRequiredDirectories();

                // Initialize the comprehensive data manager with fallback
                bool dataManagerInitialized = false;
                try
                {
                    DataExportImportManager.Instance.Initialize();
                    dataManagerInitialized = true;
                    SRInfoHelper.Log("LoadCustomDataEnhanced: DataExportImportManager initialized successfully");
                }
                catch (Exception ex)
                {
                    SRInfoHelper.Log("LoadCustomDataEnhanced: DataExportImportManager initialization failed - " + ex.Message);
                }

                // Initialize individual managers with fallback handling
                InitializeItemDataManager();
                InitializeQuestDataManager();
                InitializeSpawnCardManager();
                InitializeTranslationSystem();

                // Auto-export data if files don't exist and managers are working
                if (dataManagerInitialized)
                {
                    try
                    {
                        DataExportImportManager.Instance.ExportAllGameData();
                        SRInfoHelper.Log("LoadCustomDataEnhanced: Auto-export completed");
                    }
                    catch (Exception ex)
                    {
                        SRInfoHelper.Log("LoadCustomDataEnhanced: Auto-export failed - " + ex.Message);
                    }
                }

                isInitialized = true;
                SRInfoHelper.Log("LoadCustomDataEnhanced: Initialization complete - comprehensive data management enabled");
                
                // Show success message to player
                ShowPlayerMessage("LoadCustomData Enhanced mod loaded successfully!", 3);
            }
            catch (Exception e)
            {
                initializationFailed = true;
                SRInfoHelper.Log("LoadCustomDataEnhanced: Critical initialization error - " + e.Message);
                Debug.LogError("LoadCustomDataEnhanced: Critical initialization error - " + e.Message);
                
                // Show error message to player
                ShowPlayerMessage("LoadCustomData mod had initialization errors - check logs", 5);
            }

            SRInfoHelper.isLogging = true;
            SRInfoHelper.Log("LoadCustomDataEnhanced: Plugin initialized (success: " + isInitialized + ")");
        }

        private void CreateRequiredDirectories()
        {
            try
            {
                string pluginPath = Manager.GetPluginManager().PluginPath;
                string iconsPath = Path.Combine(pluginPath, "icons");
                
                if (!Directory.Exists(iconsPath))
                {
                    Directory.CreateDirectory(iconsPath);
                    SRInfoHelper.Log("LoadCustomDataEnhanced: Created icons directory");
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("LoadCustomDataEnhanced: Failed to create directories - " + ex.Message);
            }
        }

        private void InitializeItemDataManager()
        {
            try
            {
                ItemDataManager.Instance.Initialize();
                if (!File.Exists(Path.Combine(Manager.GetPluginManager().PluginPath, "itemDefinitions.xml")))
                {
                    ItemDataManager.Instance.SaveItemDefinitionsToFile();
                }
                SRInfoHelper.Log("LoadCustomDataEnhanced: ItemDataManager initialized successfully");
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("LoadCustomDataEnhanced: ItemDataManager initialization failed - " + ex.Message);
            }
        }

        private void InitializeQuestDataManager()
        {
            try
            {
                QuestDataManager.Instance.Initialize();
                if (!QuestDataManager.Instance.CheckIfXMLFileExists())
                {
                    QuestDataManager.Instance.SaveQuestDataToFile();
                }
                SRInfoHelper.Log("LoadCustomDataEnhanced: QuestDataManager initialized successfully");
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("LoadCustomDataEnhanced: QuestDataManager initialization failed - " + ex.Message);
            }
        }

        private void InitializeSpawnCardManager()
        {
            try
            {
                SpawnCardManager.Instance.Initialize();
                if (!SpawnCardManager.Instance.CheckIfXMLFileExists())
                {
                    SpawnCardManager.Instance.SaveSpawnCardsToFile();
                }
                SRInfoHelper.Log("LoadCustomDataEnhanced: SpawnCardManager initialized successfully");
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("LoadCustomDataEnhanced: SpawnCardManager initialization failed - " + ex.Message);
            }
        }

        private void InitializeTranslationSystem()
        {
            try
            {
                var translations = TranslationManager.LoadTranslations();
                var langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");

                if (langLookup != null)
                {
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
                    SRInfoHelper.Log("LoadCustomDataEnhanced: Translation system initialized - " + translations.Count + " entries loaded");
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("LoadCustomDataEnhanced: Translation system initialization failed - " + ex.Message);
            }
        }

        private void ShowPlayerMessage(string message, int duration)
        {
            try
            {
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup(message, duration);
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("LoadCustomDataEnhanced: Failed to show player message - " + ex.Message);
            }
        }

        /// <summary>
        /// Enhanced update loop with comprehensive hotkey handling
        /// </summary>
        public void Update()
        {
            if (!isInitialized && !initializationFailed) return;

            try
            {
                // Core data management hotkeys
                HandleDataManagementHotkeys();
                
                // Advanced functionality hotkeys
                HandleAdvancedHotkeys();
                
                // Debug and utility hotkeys
                HandleDebugHotkeys();
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log("LoadCustomDataEnhanced: Update error - " + ex.Message);
            }
        }

        private void HandleDataManagementHotkeys()
        {
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                SRInfoHelper.Log("LoadCustomDataEnhanced: Manual reinitialization triggered");
                try
                {
                    DataExportImportManager.Instance.Initialize();
                    ShowPlayerMessage("Data managers reinitialized!", 3);
                }
                catch (Exception ex)
                {
                    SRInfoHelper.Log("LoadCustomDataEnhanced: Reinitialization failed - " + ex.Message);
                    ShowPlayerMessage("Reinitialization failed - see logs", 3);
                }
            }

            if (Input.GetKeyDown(KeyCode.Delete))
            {
                SRInfoHelper.Log("LoadCustomDataEnhanced: Manual comprehensive data export triggered");
                try
                {
                    DataExportImportManager.Instance.ExportAllGameData();
                    ShowPlayerMessage("All game data exported!", 3);
                }
                catch (Exception ex)
                {
                    SRInfoHelper.Log("LoadCustomDataEnhanced: Export failed - " + ex.Message);
                    ShowPlayerMessage("Export failed - see logs", 3);
                }
            }

            if (Input.GetKeyDown(KeyCode.End))
            {
                SRInfoHelper.Log("LoadCustomDataEnhanced: Manual comprehensive data import triggered");
                try
                {
                    DataExportImportManager.Instance.ImportAllGameData();
                    ShowPlayerMessage("All game data imported!", 3);
                }
                catch (Exception ex)
                {
                    SRInfoHelper.Log("LoadCustomDataEnhanced: Import failed - " + ex.Message);
                    ShowPlayerMessage("Import failed - see logs", 3);
                }
            }
        }

        private void HandleAdvancedHotkeys()
        {
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                SRInfoHelper.Log("LoadCustomDataEnhanced: Manual data backup triggered");
                try
                {
                    DataExportImportManager.Instance.CreateDataBackup();
                    ShowPlayerMessage("Data backup created!", 3);
                }
                catch (Exception ex)
                {
                    SRInfoHelper.Log("LoadCustomDataEnhanced: Backup creation failed - " + ex.Message);
                    ShowPlayerMessage("Backup failed - see logs", 3);
                }
            }

            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                SRInfoHelper.Log("LoadCustomDataEnhanced: Manual data validation triggered");
                try
                {
                    bool isValid = DataExportImportManager.Instance.ValidateExportedData();
                    string message = isValid ? "Data validation passed!" : "Data validation failed!";
                    ShowPlayerMessage(message, 3);
                }
                catch (Exception ex)
                {
                    SRInfoHelper.Log("LoadCustomDataEnhanced: Validation failed - " + ex.Message);
                    ShowPlayerMessage("Validation error - see logs", 3);
                }
            }
        }

        private void HandleDebugHotkeys()
        {
            if (Input.GetKeyDown(KeyCode.Home))
            {
                SRInfoHelper.Log("LoadCustomDataEnhanced: Debug mesh export triggered");
                try
                {
                    // Original mesh export functionality preserved for compatibility
                    var areas = Manager.GetGlobalCompound();
                    if (areas != null)
                    {
                        MeshHelper.targetFolder = Manager.GetPluginManager().PluginPath;
                        SRInfoHelper.Log("LoadCustomDataEnhanced: Mesh export target set");
                        ShowPlayerMessage("Mesh export prepared", 2);
                    }
                }
                catch (Exception ex)
                {
                    SRInfoHelper.Log("LoadCustomDataEnhanced: Mesh export setup failed - " + ex.Message);
                }
            }

            // Toggle detailed logging
            if (Input.GetKeyDown(KeyCode.F12))
            {
                SRInfoHelper.isLogging = !SRInfoHelper.isLogging;
                SRInfoHelper.Log("LoadCustomDataEnhanced: Detailed logging " + (SRInfoHelper.isLogging ? "enabled" : "disabled"));
                ShowPlayerMessage("Logging " + (SRInfoHelper.isLogging ? "enabled" : "disabled"), 2);
            }
        }

        public string GetName()
        {
            return "LoadCustomData Enhanced Mod";
        }
    }
}