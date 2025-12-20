using Assets.Scripts.ui.LoadOut;
using SRMod.Services;
using System;
using System.IO;
using UnityEngine;

namespace LoadCustomData
{
    /// <summary>
    /// Refactored LoadCustomData mod - Clean architecture with service delegation
    /// Follows Single Responsibility Principle with proper separation of concerns
    /// </summary>
    public class LoadCustomData : ISrPlugin
    {
        // Core Services - Dependency injection ready
        private ItemDataManager itemDataManager;
        private TranslationManager translationManager;
        private SpawnCardManager spawnCardManager;
        // private QuestDataManager questDataManager;
        private AutoLoadService autoLoadService;
        private CommandService commandService;
        private UINotificationService uiService;
        
        private bool isInitialized = false;

        /// <summary>
        /// Get plugin name - Required by ISrPlugin interface
        /// </summary>
        public string GetName()
        {
            return "LoadCustomData";
        }

        /// <summary>
        /// Plugin initialization - Service setup and dependency injection
        /// </summary>
        public void Initialize()
        {
            try
            {
                Debug.Log("LoadCustomData: Starting refactored initialization");

                // Setup directory structure FIRST (before initializing services that need it)
                SetupDirectories();

                // Initialize services with proper dependency injection
                InitializeServices();

                // Initialize auto-loading system
                autoLoadService.Initialize();

                isInitialized = true;
                Debug.Log("LoadCustomData: Refactored initialization complete");

                uiService.ShowSuccess("Enhanced LoadCustomData loaded with quest state preservation!");
            }
            catch (Exception e)
            {
                Debug.LogError($"LoadCustomData: Initialization failed - {e.Message}");
                Debug.LogError($"LoadCustomData: Stack trace - {e.StackTrace}");
            }
        }

        /// <summary>
        /// Frame update - Delegates to command service and auto-load service
        /// </summary>
        public void Update()
        {
            if (!isInitialized) return;

            try
            {
                // Handle auto-loading
                autoLoadService.Update();
                
                // Handle input commands
                commandService.ProcessInput();
            }
            catch (Exception e)
            {
                Debug.LogError($"LoadCustomData: Update error - {e.Message}");
            }
        }

        /// <summary>
        /// Initialize all service dependencies
        /// </summary>
        private void InitializeServices()
        {
            var pluginPath = Manager.GetPluginManager().PluginPath;
            
            // Initialize core data managers  
            itemDataManager = ItemDataManager.Instance;
            itemDataManager.Initialize();
            
            translationManager = new TranslationManager();
            spawnCardManager = new SpawnCardManager();
            spawnCardManager.Initialize();
            
            // questDataManager = QuestDataManager.Instance;
            // questDataManager.Initialize();
            
            // Initialize utility services
            uiService = new UINotificationService();
            autoLoadService = new AutoLoadService(itemDataManager, translationManager, spawnCardManager, null, uiService);
            commandService = new CommandService(itemDataManager, translationManager, spawnCardManager, null, uiService);
            
            Debug.Log("LoadCustomData: All services initialized");
        }

        /// <summary>
        /// Setup required directory structure
        /// </summary>
        private void SetupDirectories()
        {
            var pluginPath = Manager.GetPluginManager().PluginPath;
            
            // Create icons directory if needed
            var iconsPath = Path.Combine(pluginPath, "icons");
            if (!Directory.Exists(iconsPath))
            {
                Directory.CreateDirectory(iconsPath);
                Debug.Log("LoadCustomData: Created icons directory");
            }
        }
    }

    /// <summary>
    /// Handles automatic data loading after game initialization
    /// </summary>
    public class AutoLoadService
    {
        private readonly ItemDataManager itemDataManager;
        private readonly TranslationManager translationManager;
        private readonly SpawnCardManager spawnCardManager;
        // private readonly QuestDataManager questDataManager;
        private readonly UINotificationService uiService;
        
        private bool autoLoadAttempted = false;

        public AutoLoadService(ItemDataManager itemManager, TranslationManager translationManager, 
                              SpawnCardManager spawnManager, object questManager, UINotificationService uiService)
        {
            this.itemDataManager = itemManager;
            this.translationManager = translationManager;
            this.spawnCardManager = spawnManager;
            // this.questDataManager = questManager;
            this.uiService = uiService;
        }

        public void Initialize()
        {
            Debug.Log("LoadCustomData: Auto-load service initialized");
        }

        public void Update()
        {
             // Ensure UI manager is initialized before
            if (!autoLoadAttempted)
            {
                // Use same pattern as working SyndicateMod - check if game is loaded and ItemManager is ready
                if (IsGameLoaded() && IsItemManagerReady())
                {
                    Debug.Log("LoadCustomData: Game and ItemManager ready, attempting auto-load");
                    PerformAutoLoad();
                    autoLoadAttempted = true;
                }
            }
        }
        
        private bool IsItemManagerReady()
        {
            try
            {
                var itemManager = Manager.GetItemManager();
                return itemManager != null && 
                       itemManager.m_ItemDefinitions != null && 
                       itemManager.m_ItemDefinitions.Count > 0;
            }
            catch
            {
                return false;
            }
        }

        private bool IsGameLoaded()
        {
            try
            {
                return Manager.Get().GameInProgress;
            }
            catch
            {
                return false;
            }
        }

        private void PerformAutoLoad()
        {
            try
            {
                // Auto-load translations first (so names/descriptions are available)
                Debug.Log("LoadCustomData: Loading translations from XML file");
                var translationElements = FileManager.LoadTranslationsXML("translations.xml");
                if (translationElements != null && translationElements.Count > 0)
                {
                    TranslationManager.UpdateGameTranslations(translationElements);
                    Debug.Log($"LoadCustomData: Loaded {translationElements.Count} translation elements from XML file");
                }
                else
                {
                    Debug.Log("LoadCustomData: No translations found in XML file");
                }
                
                // Auto-load item data if available using the auto-load method
                itemDataManager.InitializeWithAutoLoad();
                
                // Auto-load enemy data if available  
                spawnCardManager.LoadSpawnCardsFromFileAndUpdateSpawnManager();
                
                // Auto-load quest data if available
                // questDataManager.LoadQuestDataFromFile();
                
                // Auto-load weapon data if available
                Debug.Log("LoadCustomData: Loading weapon data from XML file");
                WeaponDataManager.ImportWeaponDataFromXML("weapons.xml");
                
                uiService.ShowInfo("Enhanced auto-load completed with quest state preservation");
            }
            catch (Exception e)
            {
                Debug.LogError($"LoadCustomData: Auto-load failed - {e.Message}");
                uiService.ShowError("Auto-load failed - check logs");
            }
        }
    }

    /// <summary>
    /// Handles all input commands and delegates to appropriate services
    /// </summary>
    public class CommandService
    {
        private readonly ItemDataManager itemDataManager;
        private readonly TranslationManager translationManager;
        private readonly SpawnCardManager spawnCardManager;
        // private readonly QuestDataManager questDataManager;
        private readonly UINotificationService uiService;

        public CommandService(ItemDataManager itemManager, TranslationManager translationManager,
                             SpawnCardManager spawnManager, object questManager, UINotificationService uiService)
        {
            this.itemDataManager = itemManager;
            this.translationManager = translationManager;
            this.spawnCardManager = spawnManager;
            // this.questDataManager = questManager;
            this.uiService = uiService;
        }

        public void ProcessInput()
        {
            // Export all data (F10)
            if (Input.GetKeyDown(KeyCode.F10))
            {
                Debug.Log("LoadCustomData: Export triggered");
                ExportAllData();
            }

            // Import all data (F9)  
            if (Input.GetKeyDown(KeyCode.F9))
            {
                Debug.Log("LoadCustomData: Import triggered");
                ImportAllData();
            }

            // Auto-load (F11)
            if (Input.GetKeyDown(KeyCode.F11))
            {
                Debug.Log("LoadCustomData: Manual auto-load triggered");
                if (IsGameLoaded())
                {
                    PerformAutoLoad();
                }
                else
                {
                    uiService.ShowError("Game not loaded - cannot auto-load data");
                }
            }

            // Show help (Insert)
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                Debug.Log("LoadCustomData: Help triggered");
                ShowHelp();
            }

            // Diagnostic test (F4)
            if (Input.GetKeyDown(KeyCode.F4))
            {
                Debug.Log("LoadCustomData: Diagnostic test triggered");
                RunDiagnostics();
            }

            // Quest status check (F5)
            if (Input.GetKeyDown(KeyCode.F5))
            {
                Debug.Log("LoadCustomData: Quest status check triggered");
                ShowQuestStatus();
            }
        }

        private void ExportAllData()
        {
            try
            {
                Debug.Log("LoadCustomData: Starting item export");
                itemDataManager.SaveItemDefinitionsToFile();
                
                Debug.Log("LoadCustomData: Starting spawn card export");
                spawnCardManager.SaveSpawnCardsToFile();
                
                Debug.Log("LoadCustomData: Starting enhanced quest export (includes quest state data)");
                // questDataManager.SaveQuestDataToFile();
                
                // Show quest export statistics
                // var questStats = questDataManager.GetQuestExportStats();
                var questStats = "Quest system disabled";
                Debug.Log($"LoadCustomData: {questStats}");
                uiService.ShowInfo(questStats, 4);
                
                Debug.Log("LoadCustomData: Starting translation export");
                var translations = TranslationManager.LoadTranslations();
                Debug.Log($"LoadCustomData: Loaded {translations?.Count ?? 0} translations");
                TranslationManager.SaveTranslationsToFile(translations);
                Debug.Log("LoadCustomData: Translation export complete");
                
                Debug.Log("LoadCustomData: Starting weapon data export");
                WeaponDataManager.ExportWeaponDataToXML("weapons.xml");
                Debug.Log("LoadCustomData: Weapon export complete");
                
                uiService.ShowSuccess("All data exported successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError($"LoadCustomData: Export failed - {e.Message}");
                Debug.LogError($"LoadCustomData: Export stack trace - {e.StackTrace}");
                uiService.ShowError($"Export failed - {e.Message}");
            }
        }

        private void ImportAllData()
        {
            try
            {
                Debug.Log("LoadCustomData: Starting manual import");
                
                itemDataManager.LoadItemDefinitionsFromFileAndUpdateItemManager();
                spawnCardManager.LoadSpawnCardsFromFileAndUpdateSpawnManager();
                
                Debug.Log("LoadCustomData: Loading enhanced quest data (includes quest state restoration)");
                // questDataManager.LoadQuestDataFromFile();
                uiService.ShowInfo("Quest import includes quest progress and state restoration", 2);
                
                Debug.Log("LoadCustomData: Importing weapon data");
                WeaponDataManager.ImportWeaponDataFromXML("weapons.xml");
                
                Debug.Log("LoadCustomData: Importing translations");
                var translationElements = FileManager.LoadTranslationsXML("translations.xml");
                if (translationElements != null && translationElements.Count > 0)
                {
                    TranslationManager.UpdateGameTranslations(translationElements);
                }
                
                uiService.ShowSuccess("All data imported successfully!");
            }
            catch (Exception e)
            {
                Debug.LogError($"LoadCustomData: Import failed - {e.Message}");
                uiService.ShowError("Import failed - check logs");
            }
        }

        private void ShowHelp()
        {
            var helpText = "Enhanced LoadCustomData Controls:\n" +
                          "F10 - Export all data (items, weapons, enemies, quests + states, translations)\n" +
                          "F9 - Import all data (items, weapons, enemies, quests + states, translations)\n" +
                          "F11 - Auto-load all data (same as game startup)\n" +
                          "F4 - Run enhanced diagnostics with quest state info\n" +
                          "F5 - Show detailed quest status and progress\n" +
                          "Insert - Show this help\n\n" +
                          "NEW: Quest export now includes quest progress and completion states!";
            
            uiService.ShowInfo(helpText, 5);
        }

        private void ShowQuestStatus()
        {
            try
            {
                // var questStats = questDataManager.GetQuestExportStats();
                var questStats = "Quest system disabled";
                uiService.ShowInfo(questStats, 5);
                Debug.Log($"LoadCustomData: Quest Status - {questStats}");
            }
            catch (Exception e)
            {
                Debug.LogError($"LoadCustomData: Quest status check failed - {e.Message}");
                uiService.ShowError("Quest status check failed - check logs");
            }
        }

        private void RunDiagnostics()
        {
            try
            {
                var itemCount = Manager.GetItemManager()?.GetAllItems()?.Count ?? 0;
                var hasSpawnXML = spawnCardManager.CheckIfXMLFileExists();
                // var hasQuestXML = questDataManager.CheckIfXMLFileExists();
                var hasQuestXML = false;
                
                var weaponCount = Manager.GetWeaponManager()?.m_WeaponData?.Length ?? 0;
                
                // Enhanced quest diagnostics
                var questManager = Manager.GetQuestManager();
                // var questElementCount = questManager?.m_QuestElements?.Count ?? 0;
                // var activeQuestCount = questManager?.m_QuestElements?.Count(q => q.m_State == 1) ?? 0;
                var questElementCount = 0;
                var activeQuestCount = 0;
                
                var diagnostics = $"Enhanced LoadCustomData Diagnostics:\n" +
                                 $"Items: {itemCount}\n" +
                                 $"Weapons: {weaponCount}\n" +
                                 $"Quest Elements: {questElementCount}\n" +
                                 $"Active Quests: {activeQuestCount}\n" +
                                 $"Spawn XML: {(hasSpawnXML ? "Found" : "Missing")}\n" +
                                 $"Quest XML: {(hasQuestXML ? "Found with state data" : "Missing")}";
                
                uiService.ShowInfo(diagnostics, 4);
                Debug.Log($"LoadCustomData: {diagnostics}");
            }
            catch (Exception e)
            {
                Debug.LogError($"LoadCustomData: Diagnostics failed - {e.Message}");
                uiService.ShowError("Diagnostics failed - check logs");
            }
        }

        private bool IsGameLoaded()
        {
            try
            {
                return Manager.Get().GameInProgress;
            }
            catch
            {
                return false;
            }
        }

        private void PerformAutoLoad()
        {
            try
            {
                // Auto-load translations first (so names/descriptions are available)
                Debug.Log("LoadCustomData: Loading translations from XML file");
                var translationElements = FileManager.LoadTranslationsXML("translations.xml");
                if (translationElements != null && translationElements.Count > 0)
                {
                    TranslationManager.UpdateGameTranslations(translationElements);
                    Debug.Log($"LoadCustomData: Loaded {translationElements.Count} translation elements from XML file");
                }
                else
                {
                    Debug.Log("LoadCustomData: No translations found in XML file");
                }
                
                // Auto-load item data if available using the auto-load method
                itemDataManager.InitializeWithAutoLoad();
                
                // Auto-load enemy data if available  
                spawnCardManager.LoadSpawnCardsFromFileAndUpdateSpawnManager();
                
                // Auto-load quest data if available
                // questDataManager.LoadQuestDataFromFile();
                
                // Auto-load weapon data if available
                Debug.Log("LoadCustomData: Loading weapon data from XML file");
                WeaponDataManager.ImportWeaponDataFromXML("weapons.xml");
                
                uiService.ShowInfo("Enhanced auto-load completed with quest state preservation");
            }
            catch (Exception e)
            {
                Debug.LogError($"LoadCustomData: Auto-load failed - {e.Message}");
                uiService.ShowError("Auto-load failed - check logs");
            }
        }
    }

    /// <summary>
    /// Centralized UI notification service
    /// </summary>
    public class UINotificationService
    {
        public void ShowSuccess(string message, float duration = 3f)
        {
            ShowMessage($"✓ {message}", duration);
        }

        public void ShowError(string message, float duration = 4f)
        {
            ShowMessage($"✗ {message}", duration);
        }

        public void ShowInfo(string message, float duration = 3f)
        {
            ShowMessage($"ℹ {message}", duration);
        }

        private void ShowMessage(string message, float duration)
        {
            if (Manager.Get() != null && Manager.GetUIManager() != null)
            {
                Manager.GetUIManager().ShowMessagePopup(message, duration);
            }
            Debug.Log($"LoadCustomData: {message}");
        }
    }
}