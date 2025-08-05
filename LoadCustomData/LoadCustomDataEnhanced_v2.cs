using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LoadCustomData
{
    /// <summary>
    /// Enhanced LoadCustomData mod focused on reliable item export/import functionality
    /// Built on proven minimal foundation - no problematic quest system dependencies
    /// </summary>
    public class LoadCustomDataEnhanced_v2 : ISrPlugin
    {
        #region Fields
        private bool isInitialized = false;
        private bool itemSystemReady = false;
        private const string ITEM_EXPORT_FILE = "itemDefinitions.xml";
        private const string BACKUP_SUFFIX = "_backup";
        #endregion

        #region ISrPlugin Implementation
        public void Initialize()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2: Starting initialization");
                
                // Initialize core systems
                InitializeItemSystem();
                
                isInitialized = true;
                
                // Show success message
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    string message = itemSystemReady 
                        ? "LoadCustomData Enhanced loaded! Item export/import ready."
                        : "LoadCustomData Enhanced loaded! (Item system not ready yet)";
                    Manager.GetUIManager().ShowMessagePopup(message, 5);
                }
                
                Debug.Log("LoadCustomDataEnhanced_v2: Initialization complete - Item system ready: " + itemSystemReady);
            }
            catch (Exception e)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Initialization failed: " + e.Message);
                
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("LoadCustomData Enhanced: Initialization failed - see logs", 5);
                }
            }
        }

        public void Update()
        {
            if (!isInitialized)
                return;

            try
            {
                HandleHotkeys();
            }
            catch (Exception e)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Update error: " + e.Message);
            }
        }

        public string GetName()
        {
            return "LoadCustomData Enhanced v2.0";
        }
        #endregion

        #region Initialization
        private void InitializeItemSystem()
        {
            try
            {
                var itemManager = Manager.GetItemManager();
                if (itemManager != null && itemManager.GetAllItems().Count > 0)
                {
                    itemSystemReady = true;
                    Debug.Log("LoadCustomDataEnhanced_v2: Item system ready - " + itemManager.GetAllItems().Count + " items found");
                }
                else
                {
                    Debug.Log("LoadCustomDataEnhanced_v2: Item system not ready yet");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Item system initialization failed: " + ex.Message);
            }
        }
        #endregion

        #region Hotkey Handling
        private void HandleHotkeys()
        {
            // F1 - Show help
            if (Input.GetKeyDown(KeyCode.F1))
            {
                ShowHelp();
            }

            // Insert - Reinitialize systems
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                ReinitializeSystems();
            }

            // Delete - Export all data (items, translations, sprites)
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                ExportAllData();
            }

            // End - Import item data
            if (Input.GetKeyDown(KeyCode.End))
            {
                ImportItemData();
            }

            // Page Up - Create backup
            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                CreateBackup();
            }

            // Page Down - Validate data
            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                ValidateData();
            }

            // Home - Export item list (simple text format)
            if (Input.GetKeyDown(KeyCode.Home))
            {
                ExportItemList();
            }

            // F2 - Export translations only
            if (Input.GetKeyDown(KeyCode.F2))
            {
                ExportTranslations();
            }

            // F3 - Export sprites only
            if (Input.GetKeyDown(KeyCode.F3))
            {
                ExportSprites();
            }
        }
        #endregion

        #region Core Functionality
        private void ShowHelp()
        {
            try
            {
                string helpText = "=== LoadCustomData Enhanced v2.0 ===\n\n";
                helpText += "HOTKEYS:\n";
                helpText += "F1 - Show this help\n";
                helpText += "Insert - Reinitialize systems\n";
                helpText += "Delete - Export items + attempt translations/sprites\n";
                helpText += "End - Import item data (XML)\n";
                helpText += "Page Up - Create backup\n";
                helpText += "Page Down - Validate data\n";
                helpText += "Home - Export item list (text)\n";
                helpText += "F2 - Export translations only\n";
                helpText += "F3 - Export sprites only\n\n";
                helpText += "Status: " + (itemSystemReady ? "Ready" : "Item system not ready");

                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup(helpText, 10);
                }

                Debug.Log("LoadCustomDataEnhanced_v2: Help displayed");
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Show help failed: " + ex.Message);
            }
        }

        private void ReinitializeSystems()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2: Reinitializing systems");
                
                InitializeItemSystem();
                
                string message = itemSystemReady 
                    ? "Systems reinitialized successfully!"
                    : "Reinitialization complete - Item system still not ready";

                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup(message, 3);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Reinitialization failed: " + ex.Message);
                ShowUserMessage("Reinitialization failed - see logs");
            }
        }

        private void ExportAllData()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2: Starting comprehensive data export");
                
                int exportCount = 0;
                var exportResults = new List<string>();
                var exportErrors = new List<string>();

                // Export Items (most reliable)
                Debug.Log("LoadCustomDataEnhanced_v2: Attempting item export...");
                try
                {
                    if (ExportItemDataInternal())
                    {
                        exportCount++;
                        exportResults.Add("Items");
                        Debug.Log("LoadCustomDataEnhanced_v2: Item export SUCCESS");
                    }
                    else
                    {
                        exportErrors.Add("Items failed");
                        Debug.Log("LoadCustomDataEnhanced_v2: Item export FAILED");
                    }
                }
                catch (Exception ex)
                {
                    exportErrors.Add("Items error: " + ex.Message);
                    Debug.LogError("LoadCustomDataEnhanced_v2: Item export EXCEPTION: " + ex.Message);
                }

                // Export Translations (might fail due to reflection)
                Debug.Log("LoadCustomDataEnhanced_v2: Attempting translation export...");
                try
                {
                    if (ExportTranslationsInternal())
                    {
                        exportCount++;
                        exportResults.Add("Translations");
                        Debug.Log("LoadCustomDataEnhanced_v2: Translation export SUCCESS");
                    }
                    else
                    {
                        exportErrors.Add("Translations failed");
                        Debug.Log("LoadCustomDataEnhanced_v2: Translation export FAILED");
                    }
                }
                catch (Exception ex)
                {
                    exportErrors.Add("Translations error: " + ex.Message);
                    Debug.LogError("LoadCustomDataEnhanced_v2: Translation export EXCEPTION: " + ex.Message);
                }

                // Export Sprites (Unity texture access might fail)
                Debug.Log("LoadCustomDataEnhanced_v2: Attempting sprite export...");
                try
                {
                    if (ExportSpritesInternal())
                    {
                        exportCount++;
                        exportResults.Add("Sprites");
                        Debug.Log("LoadCustomDataEnhanced_v2: Sprite export SUCCESS");
                    }
                    else
                    {
                        exportErrors.Add("Sprites failed");
                        Debug.Log("LoadCustomDataEnhanced_v2: Sprite export FAILED");
                    }
                }
                catch (Exception ex)
                {
                    exportErrors.Add("Sprites error: " + ex.Message);
                    Debug.LogError("LoadCustomDataEnhanced_v2: Sprite export EXCEPTION: " + ex.Message);
                }

                // Provide detailed feedback
                if (exportCount > 0)
                {
                    string message = "Exported " + exportCount + " data types: " + string.Join(", ", exportResults.ToArray());
                    if (exportErrors.Count > 0)
                    {
                        message += " (Errors: " + exportErrors.Count + ")";
                    }
                    Debug.Log("LoadCustomDataEnhanced_v2: " + message);
                    ShowUserMessage(message);
                }
                else
                {
                    string errorMessage = "Export failed - " + exportErrors.Count + " errors: " + string.Join(", ", exportErrors.ToArray());
                    Debug.LogError("LoadCustomDataEnhanced_v2: " + errorMessage);
                    ShowUserMessage("Export failed - see console for details");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Comprehensive export CRITICAL FAILURE: " + ex.Message);
                Debug.LogError("LoadCustomDataEnhanced_v2: Stack trace: " + ex.StackTrace);
                ShowUserMessage("Export failed - critical error, see logs");
            }
        }

        private bool ExportItemDataInternal()
        {
            try
            {
                if (!itemSystemReady)
                {
                    Debug.Log("LoadCustomDataEnhanced_v2: Item system not ready for export");
                    return false;
                }

                var itemManager = Manager.GetItemManager();
                if (itemManager == null) return false;

                var items = itemManager.GetAllItems();
                if (items == null || items.Count == 0) return false;

                // Create XML export
                string xmlContent = CreateItemXML(items);
                
                // Write to file
                string filePath = Path.Combine(Manager.GetPluginManager().PluginPath, ITEM_EXPORT_FILE);
                File.WriteAllText(filePath, xmlContent);
                
                Debug.Log("LoadCustomDataEnhanced_v2: Exported " + items.Count + " items");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Item export failed: " + ex.Message);
                return false;
            }
        }

        private void ExportTranslations()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2: Starting translation export");
                
                bool success = ExportTranslationsInternal();
                string message = success ? "Translations exported successfully!" : "Translation export failed - see logs";
                ShowUserMessage(message);
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Translation export failed: " + ex.Message);
                ShowUserMessage("Translation export failed - see logs");
            }
        }

        private bool ExportTranslationsInternal()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2: Getting TextManager...");
                
                // Get translations directly from the game's TextManager
                var textManager = TextManager.Get();
                if (textManager == null)
                {
                    Debug.LogError("LoadCustomDataEnhanced_v2: TextManager not available");
                    return false;
                }
                Debug.Log("LoadCustomDataEnhanced_v2: TextManager obtained successfully");

                Debug.Log("LoadCustomDataEnhanced_v2: Attempting reflection access to translation data...");
                
                // Try multiple possible field names for translation data
                var textManagerType = textManager.GetType();
                Dictionary<string, TextManager.LocElement> translations = null;
                
                // Try common field names
                string[] possibleFieldNames = { 
                    "m_FastLanguageLookup", 
                    "m_LanguageLookup", 
                    "m_Translations", 
                    "m_TextLookup",
                    "m_LocalizedText"
                };
                
                foreach (string fieldName in possibleFieldNames)
                {
                    var field = textManagerType.GetField(fieldName, 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (field != null)
                    {
                        Debug.Log("LoadCustomDataEnhanced_v2: Found field: " + fieldName);
                        var fieldValue = field.GetValue(textManager);
                        
                        if (fieldValue is Dictionary<string, TextManager.LocElement>)
                        {
                            translations = fieldValue as Dictionary<string, TextManager.LocElement>;
                            Debug.Log("LoadCustomDataEnhanced_v2: Successfully accessed translations from " + fieldName);
                            break;
                        }
                        else
                        {
                            Debug.Log("LoadCustomDataEnhanced_v2: Field " + fieldName + " found but wrong type: " + fieldValue?.GetType().Name ?? "null");
                        }
                    }
                }
                
                if (translations == null)
                {
                    Debug.LogError("LoadCustomDataEnhanced_v2: Could not find translation data via reflection");
                    
                    // List all available fields for debugging
                    var allFields = textManagerType.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    Debug.Log("LoadCustomDataEnhanced_v2: Available private fields: ");
                    foreach (var field in allFields)
                    {
                        Debug.Log("  - " + field.Name + " (" + field.FieldType.Name + ")");
                    }
                    
                    // Try public fields and properties too
                    var publicFields = textManagerType.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    Debug.Log("LoadCustomDataEnhanced_v2: Available public fields: ");
                    foreach (var field in publicFields)
                    {
                        Debug.Log("  - " + field.Name + " (" + field.FieldType.Name + ")");
                    }
                    
                    var properties = textManagerType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    Debug.Log("LoadCustomDataEnhanced_v2: Available properties: ");
                    foreach (var prop in properties)
                    {
                        Debug.Log("  - " + prop.Name + " (" + prop.PropertyType.Name + ")");
                    }
                    
                    return false;
                }
                
                if (translations.Count == 0)
                {
                    Debug.LogError("LoadCustomDataEnhanced_v2: Translation data is empty");
                    return false;
                }
                Debug.Log("LoadCustomDataEnhanced_v2: Found " + translations.Count + " translations");

                Debug.Log("LoadCustomDataEnhanced_v2: Creating XML content...");
                // Create basic XML format
                string xmlContent = CreateTranslationXML(translations);
                Debug.Log("LoadCustomDataEnhanced_v2: XML content created, length: " + xmlContent.Length);
                
                Debug.Log("LoadCustomDataEnhanced_v2: Writing to file...");
                // Write to file
                string filePath = Path.Combine(Manager.GetPluginManager().PluginPath, "translations.xml");
                File.WriteAllText(filePath, xmlContent);
                Debug.Log("LoadCustomDataEnhanced_v2: File written to: " + filePath);

                Debug.Log("LoadCustomDataEnhanced_v2: Translation export completed successfully - " + translations.Count + " translations");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Translation export internal failed: " + ex.Message);
                Debug.LogError("LoadCustomDataEnhanced_v2: Translation export stack trace: " + ex.StackTrace);
                return false;
            }
        }

        private string CreateTranslationXML(Dictionary<string, TextManager.LocElement> translations)
        {
            var xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
            xml += "<Translations>\n";
            
            foreach (var kvp in translations)
            {
                xml += "  <Translation>\n";
                xml += "    <Key>" + kvp.Key + "</Key>\n";
                xml += "    <Element>\n";
                xml += "      <m_token>" + kvp.Value.m_token + "</m_token>\n";
                xml += "      <m_Translations>\n";
                
                if (kvp.Value.m_Translations != null)
                {
                    foreach (var translation in kvp.Value.m_Translations)
                    {
                        xml += "        <string><![CDATA[" + translation + "]]></string>\n";
                    }
                }
                
                xml += "      </m_Translations>\n";
                xml += "    </Element>\n";
                xml += "  </Translation>\n";
            }
            
            xml += "</Translations>";
            return xml;
        }

        private void ExportSprites()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2: Starting sprite export");
                
                bool success = ExportSpritesInternal();
                string message = success ? "Sprites exported successfully!" : "Sprite export failed - see logs";
                ShowUserMessage(message);
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Sprite export failed: " + ex.Message);
                ShowUserMessage("Sprite export failed - see logs");
            }
        }

        private bool ExportSpritesInternal()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2: Checking item system readiness...");
                if (!itemSystemReady)
                {
                    Debug.LogError("LoadCustomDataEnhanced_v2: Item system not ready for sprite export");
                    return false;
                }

                Debug.Log("LoadCustomDataEnhanced_v2: Getting ItemManager...");
                var itemManager = Manager.GetItemManager();
                if (itemManager == null) 
                {
                    Debug.LogError("LoadCustomDataEnhanced_v2: ItemManager is null");
                    return false;
                }

                Debug.Log("LoadCustomDataEnhanced_v2: Getting all items...");
                var items = itemManager.GetAllItems();
                if (items == null) 
                {
                    Debug.LogError("LoadCustomDataEnhanced_v2: Items list is null");
                    return false;
                }
                
                if (items.Count == 0) 
                {
                    Debug.LogError("LoadCustomDataEnhanced_v2: Items list is empty");
                    return false;
                }
                
                Debug.Log("LoadCustomDataEnhanced_v2: Found " + items.Count + " items to process");

                // Ensure icons directory exists
                string pluginPath = Manager.GetPluginManager().PluginPath;
                string iconsDir = Path.Combine(pluginPath, "icons");
                if (!Directory.Exists(iconsDir))
                {
                    Debug.Log("LoadCustomDataEnhanced_v2: Creating icons directory at " + iconsDir);
                    Directory.CreateDirectory(iconsDir);
                }

                int spriteCount = 0;
                int itemsWithSprites = 0;
                int itemsProcessed = 0;
                
                // Export item sprites using FileManager method (from existing codebase)
                foreach (var item in items)
                {
                    itemsProcessed++;
                    
                    if (item.m_UIIcon != null && item.m_UIIcon.texture != null)
                    {
                        itemsWithSprites++;
                        try
                        {
                            Debug.Log("LoadCustomDataEnhanced_v2: Processing sprite for item " + item.m_FriendlyName + " (texture: " + item.m_UIIcon.texture.name + ")");
                            
                            // Use existing FileManager sprite save functionality
                            string savedPath = SaveTexture2DUsingFileManager(item.m_UIIcon.texture);
                            if (!string.IsNullOrEmpty(savedPath))
                            {
                                spriteCount++;
                                Debug.Log("LoadCustomDataEnhanced_v2: Successfully saved sprite to " + savedPath);
                            }
                            else
                            {
                                Debug.LogError("LoadCustomDataEnhanced_v2: Failed to save texture for " + item.m_FriendlyName + " - FileManager returned empty");
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.LogError("LoadCustomDataEnhanced_v2: Failed to export sprite for item " + item.m_FriendlyName + ": " + ex.Message);
                            Debug.LogError("LoadCustomDataEnhanced_v2: Sprite error stack trace: " + ex.StackTrace);
                        }
                    }
                    else
                    {
                        if (item.m_UIIcon == null)
                        {
                            Debug.Log("LoadCustomDataEnhanced_v2: Item " + item.m_FriendlyName + " has no UIIcon");
                        }
                        else if (item.m_UIIcon.texture == null)
                        {
                            Debug.Log("LoadCustomDataEnhanced_v2: Item " + item.m_FriendlyName + " UIIcon has no texture");
                        }
                    }
                }

                Debug.Log("LoadCustomDataEnhanced_v2: Sprite export completed");
                Debug.Log("LoadCustomDataEnhanced_v2: - Items processed: " + itemsProcessed);
                Debug.Log("LoadCustomDataEnhanced_v2: - Items with sprites: " + itemsWithSprites);
                Debug.Log("LoadCustomDataEnhanced_v2: - Sprites exported: " + spriteCount);
                
                return spriteCount > 0;
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Sprite export internal failed: " + ex.Message);
                Debug.LogError("LoadCustomDataEnhanced_v2: Sprite export stack trace: " + ex.StackTrace);
                return false;
            }
        }

        private void ImportItemData()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2: Starting item data import");
                
                string filePath = Path.Combine(Manager.GetPluginManager().PluginPath, ITEM_EXPORT_FILE);
                
                if (!File.Exists(filePath))
                {
                    ShowUserMessage("No " + ITEM_EXPORT_FILE + " found - export first");
                    return;
                }

                if (!itemSystemReady)
                {
                    ShowUserMessage("Item system not ready - try Insert to reinitialize");
                    return;
                }

                // Read and parse XML (simplified approach for now)
                string xmlContent = File.ReadAllText(filePath);
                int importCount = ParseAndApplyItemData(xmlContent);
                
                if (importCount > 0)
                {
                    Debug.Log("LoadCustomDataEnhanced_v2: Successfully imported " + importCount + " item modifications");
                    ShowUserMessage($"Imported {importCount} item modifications successfully!");
                }
                else
                {
                    ShowUserMessage("Import completed - no changes applied");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Import failed: " + ex.Message);
                ShowUserMessage("Import failed - see logs");
            }
        }

        private void CreateBackup()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2: Creating backup");
                
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                string backupDir = Path.Combine(Manager.GetPluginManager().PluginPath, "backup_" + timestamp);
                
                Directory.CreateDirectory(backupDir);
                
                // Backup existing export file if it exists
                string exportPath = Path.Combine(Manager.GetPluginManager().PluginPath, ITEM_EXPORT_FILE);
                if (File.Exists(exportPath))
                {
                    string backupPath = Path.Combine(backupDir, ITEM_EXPORT_FILE);
                    File.Copy(exportPath, backupPath);
                }
                
                // Create fresh export as backup
                ExportItemDataToPath(Path.Combine(backupDir, "items_backup.xml"));
                
                Debug.Log("LoadCustomDataEnhanced_v2: Backup created at " + backupDir);
                ShowUserMessage("Backup created: " + Path.GetFileName(backupDir));
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Backup failed: " + ex.Message);
                ShowUserMessage("Backup failed - see logs");
            }
        }

        private void ValidateData()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2: Validating data");
                
                string filePath = Path.Combine(Manager.GetPluginManager().PluginPath, ITEM_EXPORT_FILE);
                
                if (!File.Exists(filePath))
                {
                    ShowUserMessage("No " + ITEM_EXPORT_FILE + " found to validate");
                    return;
                }

                // Basic validation
                string content = File.ReadAllText(filePath);
                bool isValid = ValidateXMLContent(content);
                
                string message = isValid ? "Data validation passed!" : "Data validation failed - check XML format";
                ShowUserMessage(message);
                
                Debug.Log("LoadCustomDataEnhanced_v2: Validation result: " + isValid);
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Validation failed: " + ex.Message);
                ShowUserMessage("Validation error - see logs");
            }
        }

        private void ExportItemList()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2: Exporting item list");
                
                if (!itemSystemReady)
                {
                    ShowUserMessage("Item system not ready - try Insert to reinitialize");
                    return;
                }

                var itemManager = Manager.GetItemManager();
                if (itemManager == null) return;

                var items = itemManager.GetAllItems();
                if (items == null || items.Count == 0) return;

                string listContent = CreateItemList(items);
                
                string filePath = Path.Combine(Manager.GetPluginManager().PluginPath, "itemList.txt");
                File.WriteAllText(filePath, listContent);
                
                Debug.Log("LoadCustomDataEnhanced_v2: Item list exported to " + filePath);
                ShowUserMessage($"Item list exported - {items.Count} items");
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Item list export failed: " + ex.Message);
                ShowUserMessage("Item list export failed - see logs");
            }
        }
        #endregion

        #region Data Processing
        private string CreateItemXML(List<ItemManager.ItemData> items)
        {
            var xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
            xml += "<ItemDefinitions>\n";
            
            foreach (var item in items)
            {
                xml += "  <Item>\n";
                xml += $"    <ID>{item.m_ID}</ID>\n";
                xml += $"    <Name><![CDATA[{item.m_FriendlyName}]]></Name>\n";
                xml += $"    <Slot>{item.m_Slot}</Slot>\n";
                xml += $"    <Cost>{item.m_Cost}</Cost>\n";
                xml += $"    <Available>{item.m_AvailableToPlayer}</Available>\n";
                xml += $"    <ResearchCost>{item.m_ResearchCost}</ResearchCost>\n";
                xml += $"    <BlueprintCost>{item.m_BlueprintCost}</BlueprintCost>\n";
                xml += $"    <PrototypeCost>{item.m_PrototypeCost}</PrototypeCost>\n";
                
                // Modifiers
                if (item.m_Modifiers != null && item.m_Modifiers.Length > 0)
                {
                    xml += "    <Modifiers>\n";
                    foreach (var modifier in item.m_Modifiers)
                    {
                        xml += "      <Modifier>\n";
                        xml += $"        <Type>{modifier.m_Type}</Type>\n";
                        xml += $"        <Amount>{modifier.m_Ammount}</Amount>\n";
                        xml += "      </Modifier>\n";
                    }
                    xml += "    </Modifiers>\n";
                }
                
                xml += "  </Item>\n";
            }
            
            xml += "</ItemDefinitions>";
            return xml;
        }

        private string CreateItemList(List<ItemManager.ItemData> items)
        {
            var content = "=== SATELLITE REIGN ITEM LIST ===\n";
            content += $"Export Date: {DateTime.Now}\n";
            content += $"Total Items: {items.Count}\n\n";
            
            var sortedItems = items.OrderBy(i => i.m_Slot).ThenBy(i => i.m_FriendlyName).ToList();
            
            foreach (var item in sortedItems)
            {
                content += $"ID: {item.m_ID:D3} | {item.m_FriendlyName,-25} | {item.m_Slot,-15} | Cost: {item.m_Cost,-8} | Available: {item.m_AvailableToPlayer}\n";
            }
            
            return content;
        }

        private int ParseAndApplyItemData(string xmlContent)
        {
            // Simplified XML parsing for core item properties
            // This is a basic implementation - could be enhanced with proper XML parsing
            
            int changesApplied = 0;
            var itemManager = Manager.GetItemManager();
            
            try
            {
                // Simple parsing approach for demonstration
                var lines = xmlContent.Split('\n');
                ItemManager.ItemData currentItem = null;
                
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    
                    if (trimmed.StartsWith("<ID>"))
                    {
                        int id = ExtractIntValue(trimmed);
                        currentItem = itemManager.GetAllItems().FirstOrDefault(i => i.m_ID == id);
                    }
                    else if (currentItem != null && trimmed.StartsWith("<Cost>"))
                    {
                        float cost = ExtractFloatValue(trimmed);
                        if (Math.Abs(currentItem.m_Cost - cost) > 0.01f)
                        {
                            currentItem.m_Cost = cost;
                            changesApplied++;
                        }
                    }
                    else if (currentItem != null && trimmed.StartsWith("<Available>"))
                    {
                        bool available = ExtractBoolValue(trimmed);
                        if (currentItem.m_AvailableToPlayer != available)
                        {
                            currentItem.m_AvailableToPlayer = available;
                            changesApplied++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: XML parsing error: " + ex.Message);
            }
            
            return changesApplied;
        }

        private bool ValidateXMLContent(string content)
        {
            try
            {
                return content.Contains("<ItemDefinitions>") && 
                       content.Contains("</ItemDefinitions>") &&
                       content.Contains("<Item>") &&
                       content.Contains("</Item>");
            }
            catch
            {
                return false;
            }
        }

        private void ExportItemDataToPath(string filePath)
        {
            var itemManager = Manager.GetItemManager();
            if (itemManager != null)
            {
                var items = itemManager.GetAllItems();
                string xmlContent = CreateItemXML(items);
                File.WriteAllText(filePath, xmlContent);
            }
        }
        #endregion

        #region Utility Methods
        private void ShowUserMessage(string message)
        {
            try
            {
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup(message, 3);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Failed to show user message: " + ex.Message);
            }
        }

        private int ExtractIntValue(string xmlTag)
        {
            try
            {
                int start = xmlTag.IndexOf('>') + 1;
                int end = xmlTag.LastIndexOf('<');
                return int.Parse(xmlTag.Substring(start, end - start));
            }
            catch { return 0; }
        }

        private float ExtractFloatValue(string xmlTag)
        {
            try
            {
                int start = xmlTag.IndexOf('>') + 1;
                int end = xmlTag.LastIndexOf('<');
                return float.Parse(xmlTag.Substring(start, end - start));
            }
            catch { return 0f; }
        }

        private bool ExtractBoolValue(string xmlTag)
        {
            try
            {
                int start = xmlTag.IndexOf('>') + 1;
                int end = xmlTag.LastIndexOf('<');
                return bool.Parse(xmlTag.Substring(start, end - start));
            }
            catch { return false; }
        }

        private string SaveTexture2DUsingFileManager(Texture2D texture)
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2: SaveTexture2DUsingFileManager - texture name: " + texture.name);
                
                // Create filename format that FileManager expects
                string fileName = texture.name;
                if (!fileName.EndsWith(".png"))
                {
                    // Remove any existing extension and add .png
                    fileName = Path.GetFileNameWithoutExtension(fileName) + ".png";
                }
                
                Debug.Log("LoadCustomDataEnhanced_v2: Using FileManager to save texture as: " + fileName);
                
                // Use the existing FileManager.SaveTextureToFile method from the working codebase
                // This method already handles directory creation and proper PNG encoding
                var result = SaveTextureToFile(texture);
                
                if (!string.IsNullOrEmpty(result))
                {
                    Debug.Log("LoadCustomDataEnhanced_v2: Successfully saved texture via FileManager to: " + result);
                    return result;
                }
                else
                {
                    Debug.LogError("LoadCustomDataEnhanced_v2: FileManager.SaveTextureToFile returned empty result");
                    return "";
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Error using FileManager to save texture: " + ex.Message);
                Debug.LogError("LoadCustomDataEnhanced_v2: SaveTexture2DUsingFileManager stack trace: " + ex.StackTrace);
                return "";
            }
        }

        private string SaveTextureToFile(Texture2D texture)
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2: SaveTextureToFile - texture name: " + texture.name);
                
                string pluginPath = Manager.GetPluginManager().PluginPath;
                Debug.Log("LoadCustomDataEnhanced_v2: Plugin path: " + pluginPath);
                
                string iconsDir = Path.Combine(pluginPath, "icons");
                Debug.Log("LoadCustomDataEnhanced_v2: Icons directory: " + iconsDir);
                
                if (!Directory.Exists(iconsDir))
                {
                    Debug.Log("LoadCustomDataEnhanced_v2: Creating icons directory...");
                    Directory.CreateDirectory(iconsDir);
                    Debug.Log("LoadCustomDataEnhanced_v2: Icons directory created");
                }

                string fileName = Path.Combine(iconsDir, texture.name + ".png");
                Debug.Log("LoadCustomDataEnhanced_v2: Target file path: " + fileName);
                
                if (File.Exists(fileName))
                {
                    Debug.Log("LoadCustomDataEnhanced_v2: File already exists, skipping");
                    return fileName; // Already exists
                }

                Debug.Log("LoadCustomDataEnhanced_v2: Encoding texture to PNG...");
                var bytes = texture.EncodeToPNG();
                if (bytes == null || bytes.Length == 0)
                {
                    Debug.LogError("LoadCustomDataEnhanced_v2: EncodeToPNG returned null or empty bytes");
                    return "";
                }
                Debug.Log("LoadCustomDataEnhanced_v2: PNG encoded, " + bytes.Length + " bytes");
                
                Debug.Log("LoadCustomDataEnhanced_v2: Writing file...");
                File.WriteAllBytes(fileName, bytes);
                Debug.Log("LoadCustomDataEnhanced_v2: File written successfully");
                
                return fileName;
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2: Error saving texture: " + ex.Message);
                Debug.LogError("LoadCustomDataEnhanced_v2: SaveTextureToFile stack trace: " + ex.StackTrace);
                return "";
            }
        }
        #endregion
    }
}