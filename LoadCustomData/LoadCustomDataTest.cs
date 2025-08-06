using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace LoadCustomData
{
    /// <summary>
    /// Enhanced LoadCustomData with translation and sprite export support
    /// Built on the proven working foundation
    /// </summary>
    public class LoadCustomDataTest : ISrPlugin
    {
        public void Initialize()
        {
            Debug.Log("LoadCustomDataTest: Starting initialization");
            
            try
            {
                // Show immediate feedback that mod is loading
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("LoadCustomData Test mod loaded successfully!", 3);
                }
                
                Debug.Log("LoadCustomDataTest: Initialization complete");
            }
            catch (Exception e)
            {
                Debug.LogError("LoadCustomDataTest: Initialization failed - " + e.Message);
            }
        }

        public void Update()
        {
            try
            {
                // Test hotkeys
                if (Input.GetKeyDown(KeyCode.Insert))
                {
                    Debug.Log("LoadCustomDataTest: INSERT key pressed - mod is working!");
                    
                    try
                    {
                        if (Manager.Get() != null && Manager.GetUIManager() != null)
                        {
                            Manager.GetUIManager().ShowMessagePopup("LoadCustomData Test: INSERT key works!", 3);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("LoadCustomDataTest: Failed to show message - " + ex.Message);
                    }
                }

                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    Debug.Log("LoadCustomDataTest: DELETE key pressed - comprehensive export");
                    ExportAllData();
                }

                // F1 - Show help
                if (Input.GetKeyDown(KeyCode.F1))
                {
                    ShowHelp();
                }

                // F2 - Export translations only
                if (Input.GetKeyDown(KeyCode.F2))
                {
                    Debug.Log("LoadCustomDataTest: F2 key pressed - translation export");
                    ExportTranslations();
                }

                // F3 - Export sprites only  
                if (Input.GetKeyDown(KeyCode.F3))
                {
                    Debug.Log("LoadCustomDataTest: F3 key pressed - sprite export");
                    ExportSprites();
                }

                // F4 - Export quests only
                if (Input.GetKeyDown(KeyCode.F4))
                {
                    Debug.Log("LoadCustomDataTest: F4 key pressed - quest export");
                    ExportQuests();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataTest: Update error - " + ex.Message);
            }
        }

        public string GetName()
        {
            return "LoadCustomData Enhanced - Built on Working Foundation";
        }

        private void ShowHelp()
        {
            try
            {
                string helpText = "=== LoadCustomData Enhanced ===\n\n";
                helpText += "HOTKEYS:\n";
                helpText += "Insert - Test mod is working\n";
                helpText += "Delete - Export ALL data (items + translations + sprites + quests)\n";
                helpText += "F1 - Show this help\n";
                helpText += "F2 - Export translations only\n";
                helpText += "F3 - Export sprites only\n";
                helpText += "F4 - Export quests (dual: templates + runtime state)\n";

                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup(helpText, 8);
                }
                
                Debug.Log("LoadCustomDataTest: Help displayed");
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataTest: Show help failed - " + ex.Message);
            }
        }

        private void ExportAllData()
        {
            try
            {
                Debug.Log("LoadCustomDataTest: Starting comprehensive export");
                
                int successCount = 0;
                List<string> results = new List<string>();
                List<string> errors = new List<string>();

                // 1. Export Items (proven working)
                try
                {
                    if (ExportItemsXML())
                    {
                        successCount++;
                        results.Add("Items");
                    }
                    else
                    {
                        errors.Add("Items failed");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add("Items error: " + ex.Message);
                    Debug.LogError("LoadCustomDataTest: Item export error - " + ex.Message);
                }

                // 2. Export Translations (attempt)
                try
                {
                    if (ExportTranslationsInternal())
                    {
                        successCount++;
                        results.Add("Translations");
                    }
                    else
                    {
                        errors.Add("Translations failed");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add("Translations error: " + ex.Message);
                    Debug.LogError("LoadCustomDataTest: Translation export error - " + ex.Message);
                }

                // 3. Export Sprites (attempt)  
                try
                {
                    if (ExportSpritesInternal())
                    {
                        successCount++;
                        results.Add("Sprites");
                    }
                    else
                    {
                        errors.Add("Sprites failed");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add("Sprites error: " + ex.Message);
                    Debug.LogError("LoadCustomDataTest: Sprite export error - " + ex.Message);
                }

                // 4. Export Quests (attempt)
                try
                {
                    if (ExportQuestsInternal())
                    {
                        successCount++;
                        results.Add("Quests");
                    }
                    else
                    {
                        errors.Add("Quests failed");
                    }
                }
                catch (Exception ex)
                {
                    errors.Add("Quests error: " + ex.Message);
                    Debug.LogError("LoadCustomDataTest: Quest export error - " + ex.Message);
                }

                // Report results
                string message;
                if (successCount > 0)
                {
                    message = "Exported " + successCount + " types: " + string.Join(", ", results.ToArray());
                    if (errors.Count > 0)
                    {
                        message += " (" + errors.Count + " errors)";
                    }
                }
                else
                {
                    message = "Export failed - " + errors.Count + " errors";
                }

                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup(message, 5);
                }

                Debug.Log("LoadCustomDataTest: Export complete - " + message);
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataTest: ExportAllData failed - " + ex.Message);
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("Export failed - see logs", 3);
                }
            }
        }

        private bool ExportItemsXML()
        {
            try
            {
                var itemManager = Manager.GetItemManager();
                if (itemManager == null) return false;

                var items = itemManager.GetAllItems();
                if (items == null || items.Count == 0) return false;

                // Create XML content
                string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
                xml += "<ItemDefinitions>\n";
                
                foreach (var item in items)
                {
                    xml += "  <Item>\n";
                    xml += "    <ID>" + item.m_ID + "</ID>\n";
                    xml += "    <Name><![CDATA[" + item.m_FriendlyName + "]]></Name>\n";
                    xml += "    <Slot>" + item.m_Slot + "</Slot>\n";
                    xml += "    <Cost>" + item.m_Cost + "</Cost>\n";
                    xml += "    <Available>" + item.m_AvailableToPlayer + "</Available>\n";
                    xml += "    <ResearchCost>" + item.m_ResearchCost + "</ResearchCost>\n";
                    xml += "    <BlueprintCost>" + item.m_BlueprintCost + "</BlueprintCost>\n";
                    xml += "    <PrototypeCost>" + item.m_PrototypeCost + "</PrototypeCost>\n";
                    
                    // Add modifiers if they exist
                    if (item.m_Modifiers != null && item.m_Modifiers.Length > 0)
                    {
                        xml += "    <Modifiers>\n";
                        foreach (var modifier in item.m_Modifiers)
                        {
                            xml += "      <Modifier>\n";
                            xml += "        <Type>" + modifier.m_Type + "</Type>\n";
                            xml += "        <Amount>" + modifier.m_Ammount + "</Amount>\n";
                            xml += "      </Modifier>\n";
                        }
                        xml += "    </Modifiers>\n";
                    }
                    
                    xml += "  </Item>\n";
                }
                
                xml += "</ItemDefinitions>";

                // Write to file
                string filePath = Manager.GetPluginManager().PluginPath + @"\itemDefinitions.xml";
                File.WriteAllText(filePath, xml);
                
                Debug.Log("LoadCustomDataTest: Items exported to " + filePath + " (" + items.Count + " items)");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataTest: ExportItemsXML failed - " + ex.Message);
                return false;
            }
        }

        private void ExportTranslations()
        {
            try
            {
                Debug.Log("LoadCustomDataTest: F2 - Starting translation export");
                bool success = ExportTranslationsInternal();
                
                string message = success ? "Translations exported successfully!" : "Translation export failed - see logs";
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup(message, 3);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataTest: Translation export failed - " + ex.Message);
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("Translation export failed - see logs", 3);
                }
            }
        }

        private bool ExportTranslationsInternal()
        {
            try
            {
                Debug.Log("LoadCustomDataTest: === TRANSLATION EXPORT DEBUG START ===");
                Debug.Log("LoadCustomDataTest: Getting TextManager for translations");
                
                var textManager = TextManager.Get();
                if (textManager == null)
                {
                    Debug.LogError("LoadCustomDataTest: TextManager not available - TRANSLATION EXPORT FAILED");
                    return false;
                }
                Debug.Log("LoadCustomDataTest: TextManager obtained successfully");

                // Access translation data via reflection using the correct field name from decompiled code
                var textManagerType = textManager.GetType();
                Debug.Log("LoadCustomDataTest: TextManager type: " + textManagerType.Name);
                
                Dictionary<string, TextManager.LocElement> translations = null;
                
                Debug.Log("LoadCustomDataTest: Accessing m_FastLanguageLookup field...");
                
                try
                {
                    // Based on decompiled code: private Dictionary<string, TextManager.LocElement> m_FastLanguageLookup
                    var field = textManagerType.GetField("m_FastLanguageLookup", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (!ReferenceEquals(field, null))
                    {
                        Debug.Log("LoadCustomDataTest: m_FastLanguageLookup field found, getting value...");
                        var fieldValue = field.GetValue(textManager);
                        Debug.Log("LoadCustomDataTest: Field value type: " + (fieldValue?.GetType().Name ?? "null"));
                        
                        if (fieldValue is Dictionary<string, TextManager.LocElement>)
                        {
                            translations = fieldValue as Dictionary<string, TextManager.LocElement>;
                            Debug.Log("LoadCustomDataTest: SUCCESS! Found translations with " + translations.Count + " entries");
                        }
                        else
                        {
                            Debug.LogError("LoadCustomDataTest: m_FastLanguageLookup is wrong type: " + (fieldValue?.GetType().Name ?? "null"));
                        }
                    }
                    else
                    {
                        Debug.LogError("LoadCustomDataTest: m_FastLanguageLookup field not found");
                        
                        // List all available fields for debugging
                        var allFields = textManagerType.GetFields(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        Debug.Log("LoadCustomDataTest: Available private fields in TextManager:");
                        foreach (var debugField in allFields)
                        {
                            Debug.Log("LoadCustomDataTest:   - " + debugField.Name + " (" + debugField.FieldType.Name + ")");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("LoadCustomDataTest: Exception accessing m_FastLanguageLookup: " + ex.Message);
                    Debug.LogError("LoadCustomDataTest: Stack trace: " + ex.StackTrace);
                }
                
                if (translations == null)
                {
                    Debug.LogError("LoadCustomDataTest: TRANSLATION EXPORT FAILED - Could not access any translation data fields");
                    Debug.Log("LoadCustomDataTest: === TRANSLATION EXPORT DEBUG END ===");
                    return false;
                }
                
                if (translations.Count == 0)
                {
                    Debug.LogError("LoadCustomDataTest: TRANSLATION EXPORT FAILED - Translation data is empty");
                    Debug.Log("LoadCustomDataTest: === TRANSLATION EXPORT DEBUG END ===");
                    return false;
                }

                // Create XML content
                string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
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

                // Write to file
                string filePath = Manager.GetPluginManager().PluginPath + @"\translations.xml";
                File.WriteAllText(filePath, xml);
                
                Debug.Log("LoadCustomDataTest: Translations exported to " + filePath + " (" + translations.Count + " entries)");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataTest: ExportTranslationsInternal failed - " + ex.Message);
                return false;
            }
        }

        private void ExportSprites()
        {
            try
            {
                Debug.Log("LoadCustomDataTest: F3 - Starting sprite export");
                bool success = ExportSpritesInternal();
                
                string message = success ? "Sprites exported successfully!" : "Sprite export failed - see logs";
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup(message, 3);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataTest: Sprite export failed - " + ex.Message);
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("Sprite export failed - see logs", 3);
                }
            }
        }

        private bool ExportSpritesInternal()
        {
            try
            {
                Debug.Log("LoadCustomDataTest: === SPRITE EXPORT DEBUG START ===");
                Debug.Log("LoadCustomDataTest: Getting items for sprite export");
                
                var itemManager = Manager.GetItemManager();
                if (itemManager == null) 
                {
                    Debug.LogError("LoadCustomDataTest: SPRITE EXPORT FAILED - ItemManager is null");
                    Debug.Log("LoadCustomDataTest: === SPRITE EXPORT DEBUG END ===");
                    return false;
                }
                Debug.Log("LoadCustomDataTest: ItemManager obtained successfully");

                var items = itemManager.GetAllItems();
                if (items == null) 
                {
                    Debug.LogError("LoadCustomDataTest: SPRITE EXPORT FAILED - Items list is null");
                    Debug.Log("LoadCustomDataTest: === SPRITE EXPORT DEBUG END ===");
                    return false;
                }
                
                if (items.Count == 0) 
                {
                    Debug.LogError("LoadCustomDataTest: SPRITE EXPORT FAILED - Items list is empty");
                    Debug.Log("LoadCustomDataTest: === SPRITE EXPORT DEBUG END ===");
                    return false;
                }
                
                Debug.Log("LoadCustomDataTest: Found " + items.Count + " items to process");

                // Create icons directory
                string pluginPath = Manager.GetPluginManager().PluginPath;
                Debug.Log("LoadCustomDataTest: Plugin path: " + pluginPath);
                
                string iconsDir = Path.Combine(pluginPath, "icons");
                Debug.Log("LoadCustomDataTest: Icons directory: " + iconsDir);
                
                if (!Directory.Exists(iconsDir))
                {
                    try
                    {
                        Directory.CreateDirectory(iconsDir);
                        Debug.Log("LoadCustomDataTest: Created icons directory successfully");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError("LoadCustomDataTest: SPRITE EXPORT FAILED - Could not create icons directory: " + ex.Message);
                        Debug.Log("LoadCustomDataTest: === SPRITE EXPORT DEBUG END ===");
                        return false;
                    }
                }

                int spriteCount = 0;
                int itemsWithSprites = 0;
                int itemsProcessed = 0;
                int exportErrors = 0;
                
                Debug.Log("LoadCustomDataTest: Starting to process items for sprites...");
                
                foreach (var item in items)
                {
                    itemsProcessed++;
                    
                    try
                    {
                        // Based on decompiled code: ItemData has m_UIIcon field of type Sprite
                        if (item.m_UIIcon != null && item.m_UIIcon.texture != null)
                        {
                            itemsWithSprites++;
                            Debug.Log("LoadCustomDataTest: Processing sprite for item " + item.m_FriendlyName + " (texture: " + item.m_UIIcon.texture.name + ")");
                            
                            string fileName = Path.Combine(iconsDir, item.m_UIIcon.texture.name + ".png");
                            Debug.Log("LoadCustomDataTest: Target file: " + fileName);
                            
                            // Only export if file doesn't exist (avoid overwriting)
                            if (!File.Exists(fileName))
                            {
                                Debug.Log("LoadCustomDataTest: File doesn't exist, using existing FileManager.SaveTextureToFile...");
                                try
                                {
                                    // Use your existing working SaveTextureToFile method (from git history)
                                    string savedPath = MinimalFileManager.SaveTextureToFile(item.m_UIIcon.texture);
                                    
                                    if (!string.IsNullOrEmpty(savedPath))
                                    {
                                        Debug.Log("LoadCustomDataTest: Successfully saved texture using FileManager to: " + savedPath);
                                        spriteCount++;
                                    }
                                    else
                                    {
                                        Debug.LogError("LoadCustomDataTest: MinimalFileManager.SaveTextureToFile returned empty for " + item.m_FriendlyName);
                                        exportErrors++;
                                    }
                                }
                                catch (Exception texEx)
                                {
                                    Debug.LogError("LoadCustomDataTest: MinimalFileManager.SaveTextureToFile failed for " + item.m_FriendlyName + ": " + texEx.Message);
                                    Debug.LogError("LoadCustomDataTest: Texture error stack: " + texEx.StackTrace);
                                    exportErrors++;
                                }
                            }
                            else
                            {
                                Debug.Log("LoadCustomDataTest: File already exists, counting as successful");
                                spriteCount++; // Count existing files as successful
                            }
                        }
                        else
                        {
                            if (item.m_UIIcon == null)
                            {
                                Debug.Log("LoadCustomDataTest: Item " + item.m_FriendlyName + " has no UIIcon");
                            }
                            else if (item.m_UIIcon.texture == null)
                            {
                                Debug.Log("LoadCustomDataTest: Item " + item.m_FriendlyName + " UIIcon has no texture");
                            }
                        }
                    }
                    catch (Exception itemEx)
                    {
                        Debug.LogError("LoadCustomDataTest: Failed to process item " + item.m_FriendlyName + ": " + itemEx.Message);
                        Debug.LogError("LoadCustomDataTest: Item error stack: " + itemEx.StackTrace);
                        exportErrors++;
                    }
                }

                Debug.Log("LoadCustomDataTest: Sprite export complete:");
                Debug.Log("LoadCustomDataTest: - Items processed: " + itemsProcessed);
                Debug.Log("LoadCustomDataTest: - Items with sprites: " + itemsWithSprites);
                Debug.Log("LoadCustomDataTest: - Sprites exported/found: " + spriteCount);
                Debug.Log("LoadCustomDataTest: - Export errors: " + exportErrors);
                Debug.Log("LoadCustomDataTest: === SPRITE EXPORT DEBUG END ===");
                
                return spriteCount > 0;
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataTest: SPRITE EXPORT CRITICAL FAILURE: " + ex.Message);
                Debug.LogError("LoadCustomDataTest: Critical error stack: " + ex.StackTrace);
                Debug.Log("LoadCustomDataTest: === SPRITE EXPORT DEBUG END ===");
                return false;
            }
        }

        private void ExportQuests()
        {
            try
            {
                Debug.Log("LoadCustomDataTest: F4 - Starting quest export");
                bool success = ExportQuestsInternal();
                
                string message = success ? "Quests exported successfully!" : "Quest export failed - see logs";
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup(message, 3);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataTest: Quest export failed - " + ex.Message);
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("Quest export failed - see logs", 3);
                }
            }
        }

        private bool ExportQuestsInternal()
        {
            try
            {
                Debug.Log("LoadCustomDataTest: === DUAL QUEST EXPORT DEBUG START ===");
                Debug.Log("LoadCustomDataTest: Getting QuestManager for dual quest export");
                
                var questManager = Manager.GetQuestManager();
                if (questManager == null)
                {
                    Debug.LogError("LoadCustomDataTest: QuestManager not available - QUEST EXPORT FAILED");
                    return false;
                }
                Debug.Log("LoadCustomDataTest: QuestManager obtained successfully");

                var questManagerType = questManager.GetType();
                Debug.Log("LoadCustomDataTest: QuestManager type: " + questManagerType.Name);
                
                QuestElement templateQuestElement = null;
                QuestElement runtimeQuestElement = null;
                bool templateExported = false;
                bool runtimeExported = false;
                
                // 1. Try to export TEMPLATE data (raw quest definitions)
                Debug.Log("LoadCustomDataTest: === TEMPLATE EXPORT START ===");
                try
                {
                    var templateField = questManagerType.GetField("m_BaseQuestTemplate", 
                        System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    
                    if (!ReferenceEquals(templateField, null))
                    {
                        var templateValue = templateField.GetValue(questManager);
                        if (templateValue is QuestElement)
                        {
                            templateQuestElement = templateValue as QuestElement;
                            Debug.Log("LoadCustomDataTest: Found template quest data - exporting questTemplates.xml");
                            templateExported = ExportQuestData(templateQuestElement, "questTemplates.xml", "RAW_TEMPLATE_DEFINITIONS");
                        }
                        else
                        {
                            Debug.Log("LoadCustomDataTest: m_BaseQuestTemplate is null or wrong type - skipping template export");
                        }
                    }
                    else
                    {
                        Debug.Log("LoadCustomDataTest: m_BaseQuestTemplate field not found - skipping template export");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("LoadCustomDataTest: Template export failed: " + ex.Message);
                }
                
                // 2. Try to export RUNTIME data (current game state)
                Debug.Log("LoadCustomDataTest: === RUNTIME EXPORT START ===");
                try
                {
                    var runtimeField = questManagerType.GetField("m_BaseQuestElement", 
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    if (!ReferenceEquals(runtimeField, null))
                    {
                        var runtimeValue = runtimeField.GetValue(questManager);
                        if (runtimeValue == null)
                        {
                            // Try to initialize quest tree to get runtime state
                            Debug.Log("LoadCustomDataTest: Runtime state null, attempting initialization...");
                            try
                            {
                                var initMethod = questManagerType.GetMethod("InitQuestTree", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                                if (!ReferenceEquals(initMethod, null))
                                {
                                    Debug.Log("LoadCustomDataTest: Calling InitQuestTree...");
                                    initMethod.Invoke(questManager, new object[] { true });
                                    runtimeValue = runtimeField.GetValue(questManager);
                                }
                            }
                            catch (Exception initEx)
                            {
                                Debug.LogError("LoadCustomDataTest: Failed to initialize quest tree: " + initEx.Message);
                            }
                        }
                        
                        if (runtimeValue is QuestElement)
                        {
                            runtimeQuestElement = runtimeValue as QuestElement;
                            Debug.Log("LoadCustomDataTest: Found runtime quest data - exporting questRuntimeState.xml");
                            runtimeExported = ExportQuestData(runtimeQuestElement, "questRuntimeState.xml", "RUNTIME_GAME_STATE");
                        }
                        else
                        {
                            Debug.Log("LoadCustomDataTest: m_BaseQuestElement is null or wrong type - skipping runtime export");
                        }
                    }
                    else
                    {
                        Debug.Log("LoadCustomDataTest: m_BaseQuestElement field not found - skipping runtime export");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError("LoadCustomDataTest: Runtime export failed: " + ex.Message);
                }
                
                // 3. Create legacy single file if either source is available (for backwards compatibility)
                if (templateQuestElement != null || runtimeQuestElement != null)
                {
                    Debug.Log("LoadCustomDataTest: === LEGACY EXPORT START ===");
                    QuestElement legacyElement = templateQuestElement ?? runtimeQuestElement;
                    string legacySource = templateQuestElement != null ? "RAW_TEMPLATE_DEFINITIONS" : "RUNTIME_GAME_STATE";
                    ExportQuestData(legacyElement, "questDefinitions.xml", legacySource + "_LEGACY");
                    Debug.Log("LoadCustomDataTest: Legacy questDefinitions.xml created for backwards compatibility");
                }
                
                // Report results
                string results = "";
                if (templateExported) results += "Templates ";
                if (runtimeExported) results += "Runtime ";
                
                if (templateExported || runtimeExported)
                {
                    Debug.Log("LoadCustomDataTest: DUAL QUEST EXPORT SUCCESS - Exported: " + results);
                    Debug.Log("LoadCustomDataTest: === DUAL QUEST EXPORT DEBUG END ===");
                    return true;
                }
                else
                {
                    Debug.LogError("LoadCustomDataTest: DUAL QUEST EXPORT FAILED - No quest data sources available");
                    Debug.Log("LoadCustomDataTest: === DUAL QUEST EXPORT DEBUG END ===");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataTest: ExportQuestsInternal failed - " + ex.Message);
                return false;
            }
        }
        
        private bool ExportQuestData(QuestElement baseQuestElement, string fileName, string dataSource)
        {
            try
            {
                if (baseQuestElement == null)
                {
                    Debug.LogError("LoadCustomDataTest: Cannot export quest data - baseQuestElement is null");
                    return false;
                }
                
                // Get all quest elements from the base quest element
                var allQuestElements = baseQuestElement.GetComponentsInChildren<QuestElement>(true);
                Debug.Log("LoadCustomDataTest: Found " + allQuestElements.Length + " quest elements for " + fileName);

                if (allQuestElements.Length == 0)
                {
                    Debug.LogError("LoadCustomDataTest: No quest elements found for " + fileName);
                    return false;
                }

                // Create XML content
                string xml = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n";
                xml += "<QuestDefinitions>\n";
                
                // Add metadata comment to indicate data source
                xml += "  <!-- Data Source: " + dataSource + " -->\n";
                xml += "  <!-- Export Time: " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " -->\n";
                xml += "  <!-- File: " + fileName + " -->\n";
                
                foreach (var questElement in allQuestElements)
                {
                    try
                    {
                        xml += "  <Quest>\n";
                        xml += "    <ID>" + questElement.m_ID + "</ID>\n";
                        xml += "    <Title><![CDATA[" + questElement.GetTitle() + "]]></Title>\n";
                        xml += "    <Hidden>" + (questElement.m_Hidden ? "true" : "false") + "</Hidden>\n";
                        xml += "    <ShowDebrief>" + (questElement.m_ShowDebrief ? "true" : "false") + "</ShowDebrief>\n";
                        xml += "    <State>" + (questElement.IsCompleted() ? "true" : "false") + "</State>\n";
                        xml += "    <TitleKey><![CDATA[" + questElement.m_Title + "]]></TitleKey>\n";
                        
                        // Location information if available
                        if (questElement.m_Location != null)
                        {
                            xml += "    <Location>\n";
                            xml += "      <LocationID>" + questElement.m_Location.m_LocationID + "</LocationID>\n";
                            xml += "    </Location>\n";
                        }
                        
                        // VIP information if available
                        if (questElement.m_VIP != null)
                        {
                            xml += "    <VIP>\n";
                            xml += "      <HasVIP>true</HasVIP>\n";
                            xml += "    </VIP>\n";
                        }
                        
                        // Wake on location information
                        if (questElement.HasWakeOnLocation())
                        {
                            xml += "    <WakeOnLocation>" + questElement.m_WakeOnLocation + "</WakeOnLocation>\n";
                            if (questElement.m_WakeOnLocationList.Count > 0)
                            {
                                xml += "    <WakeOnLocationList>\n";
                                foreach (var loc in questElement.m_WakeOnLocationList)
                                {
                                    xml += "      <Location>" + loc + "</Location>\n";
                                }
                                xml += "    </WakeOnLocationList>\n";
                            }
                        }
                        
                        // District information
                        var district = questElement.GetDistrict();
                        xml += "    <District>" + district + "</District>\n";
                        
                        // Description data
                        var descriptions = questElement.GetDescriptionData();
                        if (descriptions.Count > 0)
                        {
                            xml += "    <Descriptions>\n";
                            foreach (var desc in descriptions)
                            {
                                xml += "      <Description>\n";
                                xml += "        <LocTitle><![CDATA[" + desc.m_LocTitle + "]]></LocTitle>\n";
                                xml += "        <Translation><![CDATA[" + desc.m_Translation + "]]></Translation>\n";
                                xml += "        <IsNew>" + (desc.m_IsNew ? "true" : "false") + "</IsNew>\n";
                                xml += "        <HasBeenSeen>" + (desc.m_HasBeenSeen ? "true" : "false") + "</HasBeenSeen>\n";
                                xml += "      </Description>\n";
                            }
                            xml += "    </Descriptions>\n";
                        }
                        
                        // Sub-quests
                        var subQuests = questElement.GetSubQuests();
                        if (subQuests.Count > 0)
                        {
                            xml += "    <SubQuests>\n";
                            foreach (var subQuest in subQuests)
                            {
                                xml += "      <SubQuestID>" + subQuest.m_ID + "</SubQuestID>\n";
                            }
                            xml += "    </SubQuests>\n";
                        }
                        
                        xml += "  </Quest>\n";
                    }
                    catch (Exception questEx)
                    {
                        Debug.LogError("LoadCustomDataTest: Failed to process quest element " + questElement.m_ID + " for " + fileName + ": " + questEx.Message);
                    }
                }
                
                xml += "</QuestDefinitions>";

                // Write to file
                string filePath = Manager.GetPluginManager().PluginPath + @"\" + fileName;
                File.WriteAllText(filePath, xml);
                
                Debug.Log("LoadCustomDataTest: " + dataSource + " exported to " + filePath + " (" + allQuestElements.Length + " quests)");
                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("LoadCustomDataTest: ExportQuestData failed for " + fileName + " - " + ex.Message);
                return false;
            }
        }

    }
}