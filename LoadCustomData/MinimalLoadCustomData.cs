using System;
using UnityEngine;

namespace LoadCustomData
{
    public class MinimalLoadCustomData : ISrPlugin
    {
        private bool isInitialized = false;

        public void Initialize()
        {
            try
            {
                Debug.Log("MinimalLoadCustomData: Initializing...");
                
                isInitialized = true;
                
                // Show initialization message to player
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("LoadCustomData mod loaded! Press Insert/Delete to test.", 5);
                }
                
                Debug.Log("MinimalLoadCustomData: Initialization complete");
            }
            catch (Exception e)
            {
                Debug.LogError("MinimalLoadCustomData: Initialization failed: " + e.Message);
            }
        }

        public void Update()
        {
            if (!isInitialized)
                return;

            try
            {
                // Test hotkeys
                if (Input.GetKeyDown(KeyCode.Insert))
                {
                    Debug.Log("MinimalLoadCustomData: INSERT key pressed");
                    if (Manager.Get() != null && Manager.GetUIManager() != null)
                    {
                        Manager.GetUIManager().ShowMessagePopup("LoadCustomData: INSERT works!", 3);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    Debug.Log("MinimalLoadCustomData: DELETE key pressed - exporting basic data");
                    ExportBasicData();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("MinimalLoadCustomData: Update error: " + e.Message);
            }
        }

        private void ExportBasicData()
        {
            try
            {
                string exportText = "=== LOADCUSTOMDATA BASIC EXPORT ===\n";
                
                // Basic item export
                var itemManager = Manager.GetItemManager();
                if (itemManager != null)
                {
                    var items = itemManager.GetAllItems();
                    exportText += "Items Found: " + items.Count + "\n\n";
                    
                    foreach (var item in items)
                    {
                        exportText += "ID: " + item.m_ID + " - " + item.m_FriendlyName + "\n";
                    }
                }
                else
                {
                    exportText += "ItemManager not available\n";
                }
                
                // Write to file
                string filePath = Manager.GetPluginManager().PluginPath + @"\basicExport.txt";
                System.IO.File.WriteAllText(filePath, exportText);
                
                Debug.Log("MinimalLoadCustomData: Export completed to " + filePath);
                
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("LoadCustomData: Export completed!", 3);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("MinimalLoadCustomData: Export failed: " + e.Message);
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("LoadCustomData: Export failed - see log", 3);
                }
            }
        }

        public string GetName()
        {
            return "Minimal LoadCustomData v1.0";
        }
    }
}