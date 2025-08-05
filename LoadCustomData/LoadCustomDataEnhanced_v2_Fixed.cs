using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LoadCustomData
{
    /// <summary>
    /// Fixed LoadCustomData mod - back to basics with reliable functionality
    /// Removes problematic reflection and focuses on core item export/import
    /// </summary>
    public class LoadCustomDataEnhanced_v2_Fixed : ISrPlugin
    {
        #region Fields
        private bool isInitialized = false;
        private bool itemSystemReady = false;
        private const string ITEM_EXPORT_FILE = "itemDefinitions.xml";
        #endregion

        #region ISrPlugin Implementation
        public void Initialize()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Starting initialization");
                
                // Initialize core systems
                InitializeItemSystem();
                
                isInitialized = true;
                
                // Show success message
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    string message = itemSystemReady 
                        ? "LoadCustomData Fixed loaded! Item export/import ready."
                        : "LoadCustomData Fixed loaded! (Item system not ready yet)";
                    Manager.GetUIManager().ShowMessagePopup(message, 5);
                }
                
                Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Initialization complete - Item system ready: " + itemSystemReady);
            }
            catch (Exception e)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2_Fixed: Initialization failed: " + e.Message);
                
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("LoadCustomData Fixed: Initialization failed - see logs", 5);
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
                Debug.LogError("LoadCustomDataEnhanced_v2_Fixed: Update error: " + e.Message);
            }
        }

        public string GetName()
        {
            return "LoadCustomData Enhanced v2.0 Fixed";
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
                    Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Item system ready - " + itemManager.GetAllItems().Count + " items found");
                }
                else
                {
                    Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Item system not ready yet");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2_Fixed: Item system initialization failed: " + ex.Message);
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

            // Delete - Export item data only (reliable core function)
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                ExportItemData();
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
        }
        #endregion

        #region Core Functionality
        private void ShowHelp()
        {
            try
            {
                string helpText = "=== LoadCustomData Fixed v2.0 ===\n\n";
                helpText += "HOTKEYS:\n";
                helpText += "F1 - Show this help\n";
                helpText += "Insert - Reinitialize systems\n";
                helpText += "Delete - Export items (XML)\n";
                helpText += "End - Import item data (XML)\n";
                helpText += "Page Up - Create backup\n";
                helpText += "Page Down - Validate data\n";
                helpText += "Home - Export item list (text)\n\n";
                helpText += "Status: " + (itemSystemReady ? "Ready" : "Item system not ready");

                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup(helpText, 10);
                }

                Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Help displayed");
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2_Fixed: Show help failed: " + ex.Message);
            }
        }

        private void ReinitializeSystems()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Reinitializing systems");
                
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
                Debug.LogError("LoadCustomDataEnhanced_v2_Fixed: Reinitialization failed: " + ex.Message);
                ShowUserMessage("Reinitialization failed - see logs");
            }
        }

        private void ExportItemData()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Starting item data export");
                
                if (!itemSystemReady)
                {
                    ShowUserMessage("Item system not ready - try Insert to reinitialize");
                    return;
                }

                var itemManager = Manager.GetItemManager();
                if (itemManager == null)
                {
                    ShowUserMessage("ItemManager not available");
                    return;
                }

                var items = itemManager.GetAllItems();
                if (items == null || items.Count == 0)
                {
                    ShowUserMessage("No items found to export");
                    return;
                }

                // Create XML export
                string xmlContent = CreateItemXML(items);
                
                // Write to file
                string filePath = Path.Combine(Manager.GetPluginManager().PluginPath, ITEM_EXPORT_FILE);
                File.WriteAllText(filePath, xmlContent);
                
                Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Exported " + items.Count + " items to " + filePath);
                ShowUserMessage($"Successfully exported {items.Count} items!");
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2_Fixed: Export failed: " + ex.Message);
                ShowUserMessage("Export failed - see logs");
            }
        }

        private void ImportItemData()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Starting item data import");
                
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
                    Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Successfully imported " + importCount + " item modifications");
                    ShowUserMessage($"Imported {importCount} item modifications successfully!");
                }
                else
                {
                    ShowUserMessage("Import completed - no changes applied");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2_Fixed: Import failed: " + ex.Message);
                ShowUserMessage("Import failed - see logs");
            }
        }

        private void CreateBackup()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Creating backup");
                
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
                
                Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Backup created at " + backupDir);
                ShowUserMessage("Backup created: " + Path.GetFileName(backupDir));
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2_Fixed: Backup failed: " + ex.Message);
                ShowUserMessage("Backup failed - see logs");
            }
        }

        private void ValidateData()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Validating data");
                
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
                
                Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Validation result: " + isValid);
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2_Fixed: Validation failed: " + ex.Message);
                ShowUserMessage("Validation error - see logs");
            }
        }

        private void ExportItemList()
        {
            try
            {
                Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Exporting item list");
                
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
                
                Debug.Log("LoadCustomDataEnhanced_v2_Fixed: Item list exported to " + filePath);
                ShowUserMessage($"Item list exported - {items.Count} items");
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataEnhanced_v2_Fixed: Item list export failed: " + ex.Message);
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
                Debug.LogError("LoadCustomDataEnhanced_v2_Fixed: XML parsing error: " + ex.Message);
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
                Debug.LogError("LoadCustomDataEnhanced_v2_Fixed: Failed to show user message: " + ex.Message);
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
        #endregion
    }
}