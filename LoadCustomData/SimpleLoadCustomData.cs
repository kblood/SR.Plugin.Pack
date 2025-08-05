using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Simple LoadCustomData mod - working ISrPlugin version without complex dependencies
/// </summary>
public class SimpleLoadCustomData : ISrPlugin
{
    private bool isInitialized = false;

    public void Initialize()
    {
        try
        {
            Debug.Log("SimpleLoadCustomData: Starting initialization");
            
            // Basic mod setup without complex dependencies
            var pluginPath = Manager.GetPluginManager().PluginPath;
            Debug.Log("SimpleLoadCustomData: Plugin path is " + pluginPath);
            
            // Create icons directory
            var iconsPath = Path.Combine(pluginPath, "icons");
            if (!Directory.Exists(iconsPath))
            {
                Directory.CreateDirectory(iconsPath);
                Debug.Log("SimpleLoadCustomData: Created icons directory");
            }
            
            // Basic item export without complex serialization
            ExportBasicItemData();
            
            isInitialized = true;
            Debug.Log("SimpleLoadCustomData: Initialization complete");
            
            // Show success message
            if (Manager.Get() != null && Manager.GetUIManager() != null)
            {
                Manager.GetUIManager().ShowMessagePopup("SimpleLoadCustomData loaded!", 3);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SimpleLoadCustomData: Initialization failed - " + e.Message);
            Debug.LogError("SimpleLoadCustomData: Stack trace - " + e.StackTrace);
        }
    }

    public void Update()
    {
        if (!isInitialized) return;

        try
        {
            // Simple hotkey handling
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                Debug.Log("SimpleLoadCustomData: Export triggered");
                ExportBasicItemData();
                if (Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("Basic data exported!", 2);
                }
            }

            if (Input.GetKeyDown(KeyCode.Insert))
            {
                Debug.Log("SimpleLoadCustomData: Manual test triggered");
                if (Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("SimpleLoadCustomData is working!", 2);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SimpleLoadCustomData: Update error - " + e.Message);
        }
    }

    private void ExportBasicItemData()
    {
        try
        {
            var pluginPath = Manager.GetPluginManager().PluginPath;
            var filePath = Path.Combine(pluginPath, "basicItemData.txt");
            
            var itemManager = Manager.GetItemManager();
            if (itemManager != null)
            {
                var items = itemManager.GetAllItems();
                
                using (var writer = new StreamWriter(filePath))
                {
                    writer.WriteLine("=== SATELLITE REIGN BASIC ITEM EXPORT ===");
                    writer.WriteLine("Export Date: " + DateTime.Now.ToString());
                    writer.WriteLine("Total Items: " + items.Count);
                    writer.WriteLine("");
                    
                    foreach (var item in items)
                    {
                        if (item != null)
                        {
                            writer.WriteLine("Item ID: " + item.m_ID);
                            writer.WriteLine("Name: " + item.m_FriendlyName);
                            writer.WriteLine("Slot: " + item.m_Slot);
                            writer.WriteLine("Cost: " + item.m_Cost);
                            writer.WriteLine("Available: " + item.m_AvailableToPlayer);
                            writer.WriteLine("---");
                        }
                    }
                }
                
                Debug.Log("SimpleLoadCustomData: Exported " + items.Count + " items to " + filePath);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("SimpleLoadCustomData: Export failed - " + e.Message);
        }
    }

    public string GetName()
    {
        return "Simple LoadCustomData";
    }
}