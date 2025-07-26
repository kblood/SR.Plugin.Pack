using SRMod.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace SRMod.Services
{
    public class QuestDataManager
    {
        private static QuestDataManager _instance;
        public static QuestDataManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new QuestDataManager();
                return _instance;
            }
        }

        public bool IsInitialized { get; private set; }

        public void Initialize()
        {
            try
            {
                SRInfoHelper.Log("QuestDataManager: Initializing quest data management");
                IsInitialized = true;
                SRInfoHelper.Log("QuestDataManager: Initialization complete");
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Initialization failed - {0}", ex.Message));
                IsInitialized = false;
            }
        }

        public void SaveQuestDataToFile()
        {
            try
            {
                if (!IsInitialized)
                {
                    SRInfoHelper.Log("QuestDataManager: Not initialized, cannot save quest data");
                    return;
                }

                SRInfoHelper.Log("QuestDataManager: Starting quest data export");

                var questManager = Manager.GetQuestManager();
                if (questManager == null)
                {
                    SRInfoHelper.Log("QuestDataManager: Quest manager not available");
                    return;
                }

                var serializedData = ExtractQuestData(questManager);
                
                string filePath = Path.Combine(Manager.GetPluginManager().PluginPath, "questDefinitions.xml");
                FileManager.SaveAsXML(serializedData, "questDefinitions.xml");
                
                SRInfoHelper.Log(string.Format("QuestDataManager: Saved quest data to {0}", filePath));
                
                // Also save in human-readable format for debugging
                SaveQuestDataAsText(serializedData);
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to save quest data - {0}", ex.Message));
            }
        }

        private SerializableQuestManager ExtractQuestData(QuestManager questManager)
        {
            var serializedManager = new SerializableQuestManager();

            try
            {
                // Get basic quest manager data
                serializedManager.m_CurrentDisplayQuestID = GetFieldValue<int>(questManager, "m_CurrentDisplayQuestID", -1);
                serializedManager.m_RandomSeed = GetFieldValue<int>(questManager, "m_RandomSeed", 0);
                
                // Get used location lists
                var usedDataTerminals = GetFieldValue<List<int>>(questManager, "m_UsedRandomDataTerminalLocations", null);
                if (usedDataTerminals != null)
                    serializedManager.m_UsedRandomDataTerminalLocations = new List<int>(usedDataTerminals);
                
                var usedVIPLocations = GetFieldValue<List<int>>(questManager, "m_UsedRandomVIPLocations", null);
                if (usedVIPLocations != null)
                    serializedManager.m_UsedRandomVIPLocations = new List<int>(usedVIPLocations);

                // Get base quest element
                var baseQuestElement = GetFieldValue<QuestElement>(questManager, "m_BaseQuestElement", null);
                if (baseQuestElement != null)
                {
                    serializedManager.m_BaseQuestElement = ExtractQuestElement(baseQuestElement);
                    
                    // Extract all child quest elements recursively
                    ExtractQuestElementsRecursive(baseQuestElement, serializedManager.m_QuestElements);
                }

                SRInfoHelper.Log(string.Format("QuestDataManager: Extracted {0} quest elements", serializedManager.m_QuestElements.Count));
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to extract quest data - {0}", ex.Message));
            }

            return serializedManager;
        }

        private SerializableQuestElement ExtractQuestElement(QuestElement questElement)
        {
            var serialized = new SerializableQuestElement();

            try
            {
                // Basic quest element data
                serialized.m_ID = GetFieldValue<int>(questElement, "m_ID", -1);
                serialized.m_Title = GetFieldValue<string>(questElement, "m_Title", "");
                serialized.m_Hidden = GetFieldValue<bool>(questElement, "m_Hidden", false);
                serialized.m_ShowDebrief = GetFieldValue<bool>(questElement, "m_ShowDebrief", true);
                serialized.m_Address = GetFieldValue<string>(questElement, "m_Address", "");
                serialized.m_DeactivateAfterPerformingAction = GetFieldValue<bool>(questElement, "m_DeactivateAfterPerformingAction", false);
                serialized.m_CanLoadFromSave = GetFieldValue<bool>(questElement, "m_CanLoadFromSave", true);

                // Location and VIP associations
                serialized.m_WakeOnLocation = GetFieldValue<int>(questElement, "m_WakeOnLocation", -1);
                
                var wakeOnLocationList = GetFieldValue<List<int>>(questElement, "m_WakeOnLocationList", null);
                if (wakeOnLocationList != null)
                    serialized.m_WakeOnLocationList = new List<int>(wakeOnLocationList);

                // Extract quest actions
                ExtractQuestActions(questElement, serialized.m_Actions);
                
                // Extract quest reactions
                ExtractQuestReactions(questElement, serialized.m_Reactions);
                
                // Extract descriptions
                ExtractQuestDescriptions(questElement, serialized.m_Descriptions);

                SRInfoHelper.Log(string.Format("QuestDataManager: Extracted quest element '{0}' with {1} actions and {2} reactions", serialized.m_Title, serialized.m_Actions.Count, serialized.m_Reactions.Count));
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to extract quest element - {0}", ex.Message));
            }

            return serialized;
        }

        private void ExtractQuestElementsRecursive(QuestElement parent, List<SerializableQuestElement> targetList)
        {
            try
            {
                // Get all child quest elements from the parent
                var children = GetChildQuestElements(parent);
                
                foreach (var child in children)
                {
                    var serializedChild = ExtractQuestElement(child);
                    targetList.Add(serializedChild);
                    
                    // Recursively extract children
                    ExtractQuestElementsRecursive(child, targetList);
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to extract child quest elements - {0}", ex.Message));
            }
        }

        private List<QuestElement> GetChildQuestElements(QuestElement parent)
        {
            var children = new List<QuestElement>();
            
            try
            {
                // Use reflection to find child quest elements
                // This is a simplified approach - the actual implementation would need to 
                // traverse the Unity GameObject hierarchy or quest system structure
                var transform = parent.transform;
                
                for (int i = 0; i < transform.childCount; i++)
                {
                    var child = transform.GetChild(i);
                    var questElement = child.GetComponent<QuestElement>();
                    
                    if (questElement != null)
                        children.Add(questElement);
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to get child quest elements - {0}", ex.Message));
            }
            
            return children;
        }

        private void ExtractQuestActions(QuestElement questElement, List<SerializableQuestAction> actions)
        {
            try
            {
                // Get all QuestAction components from the quest element
                var questActions = questElement.GetComponentsInChildren<QuestAction>();
                
                foreach (var action in questActions)
                {
                    var serializedAction = ExtractQuestAction(action);
                    if (serializedAction != null)
                        actions.Add(serializedAction);
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to extract quest actions - {0}", ex.Message));
            }
        }

        private SerializableQuestAction ExtractQuestAction(QuestAction action)
        {
            try
            {
                var actionType = action.GetType().Name;
                
                // Create specific action types for common actions
                switch (actionType)
                {
                    case "QAGiveItem":
                        return ExtractQAGiveItem(action);
                    case "QAGiveCash":
                        return ExtractQAGiveCash(action);
                    case "QASelectLocation":
                        return ExtractQASelectLocation(action);
                    case "QASpawnVIP":
                        return ExtractQASpawnVIP(action);
                    case "QABroadcastMessage":
                        return ExtractQABroadcastMessage(action);
                    case "QAActivateQuest":
                        return ExtractQAActivateQuest(action);
                    default:
                        return ExtractGenericQuestAction(action);
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to extract quest action - {0}", ex.Message));
                return null;
            }
        }

        private SerializableQAGiveItem ExtractQAGiveItem(QuestAction action)
        {
            var serialized = new SerializableQAGiveItem();
            PopulateBaseActionData(action, serialized);
            
            // Extract item awarder data
            var itemAwarder = GetFieldValue<ItemAwarder>(action, "m_ItemAwarder", null);
            if (itemAwarder != null)
            {
                // Extract item awarder properties
                serialized.m_ItemAwarder.m_AwardItemsInstantly = GetFieldValue<bool>(itemAwarder, "m_AwardItemsInstantly", true);
                // Add more item awarder fields as needed
            }
            
            return serialized;
        }

        private SerializableQAGiveCash ExtractQAGiveCash(QuestAction action)
        {
            var serialized = new SerializableQAGiveCash();
            PopulateBaseActionData(action, serialized);
            
            serialized.m_CashAmount = GetFieldValue<int>(action, "m_CashAmount", 0);
            serialized.m_ShowFeedback = GetFieldValue<bool>(action, "m_ShowFeedback", true);
            serialized.m_PlaySound = GetFieldValue<bool>(action, "m_PlaySound", true);
            
            return serialized;
        }

        private SerializableQASelectLocation ExtractQASelectLocation(QuestAction action)
        {
            var serialized = new SerializableQASelectLocation();
            PopulateBaseActionData(action, serialized);
            
            // Extract location selector data
            // This would need to be implemented based on the actual QASelectLocation structure
            
            return serialized;
        }

        private SerializableQASpawnVIP ExtractQASpawnVIP(QuestAction action)
        {
            var serialized = new SerializableQASpawnVIP();
            PopulateBaseActionData(action, serialized);
            
            // Extract VIP spawner data
            serialized.m_VIPSpawner.m_VIPToSpawnID = GetFieldValue<int>(action, "m_VIPToSpawnID", -1);
            serialized.m_VIPSpawner.m_NameID = GetFieldValue<int>(action, "m_NameID", -1);
            // Add more VIP spawner fields as needed
            
            return serialized;
        }

        private SerializableQABroadcastMessage ExtractQABroadcastMessage(QuestAction action)
        {
            var serialized = new SerializableQABroadcastMessage();
            PopulateBaseActionData(action, serialized);
            
            serialized.m_MessageTitle = GetFieldValue<string>(action, "m_MessageTitle", "");
            serialized.m_MessageText = GetFieldValue<string>(action, "m_MessageText", "");
            serialized.m_DisplayDuration = GetFieldValue<float>(action, "m_DisplayDuration", 5f);
            
            return serialized;
        }

        private SerializableQAActivateQuest ExtractQAActivateQuest(QuestAction action)
        {
            var serialized = new SerializableQAActivateQuest();
            PopulateBaseActionData(action, serialized);
            
            serialized.m_QuestIDToActivate = GetFieldValue<int>(action, "m_QuestIDToActivate", -1);
            
            return serialized;
        }

        private SerializableQuestAction ExtractGenericQuestAction(QuestAction action)
        {
            var serialized = new SerializableQuestAction();
            PopulateBaseActionData(action, serialized);
            
            // For generic actions, store all public fields in parameters dictionary
            var fields = action.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                try
                {
                    var value = field.GetValue(action);
                    if (value != null && IsSerializableType(value.GetType()))
                    {
                        serialized.m_Parameters[field.Name] = value;
                    }
                }
                catch (Exception ex)
                {
                    SRInfoHelper.Log(string.Format("QuestDataManager: Failed to extract field {0} - {1}", field.Name, ex.Message));
                }
            }
            
            return serialized;
        }

        private void PopulateBaseActionData(QuestAction action, SerializableQuestAction serialized)
        {
            serialized.m_ActionType = action.GetType().Name;
            serialized.m_ID = GetFieldValue<int>(action, "m_ID", -1);
            serialized.m_Address = GetFieldValue<string>(action, "m_Address", "");
            serialized.m_DeactivateAfterPerformingAction = GetFieldValue<bool>(action, "m_DeactivateAfterPerformingAction", false);
        }

        private void ExtractQuestReactions(QuestElement questElement, List<SerializableQuestReaction> reactions)
        {
            try
            {
                // Get all QuestReaction components from the quest element
                var questReactions = questElement.GetComponentsInChildren<QuestReaction>();
                
                foreach (var reaction in questReactions)
                {
                    var serializedReaction = ExtractQuestReaction(reaction);
                    if (serializedReaction != null)
                        reactions.Add(serializedReaction);
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to extract quest reactions - {0}", ex.Message));
            }
        }

        private SerializableQuestReaction ExtractQuestReaction(QuestReaction reaction)
        {
            try
            {
                var reactionType = reaction.GetType().Name;
                
                // Create specific reaction types for common reactions
                switch (reactionType)
                {
                    case "QRWait":
                        return ExtractQRWait(reaction);
                    case "QRDataTerminalAccessed":
                        return ExtractQRDataTerminalAccessed(reaction);
                    case "QRFacilityBreached":
                        return ExtractQRFacilityBreached(reaction);
                    case "QRProgressionData":
                        return ExtractQRProgressionData(reaction);
                    default:
                        return ExtractGenericQuestReaction(reaction);
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to extract quest reaction - {0}", ex.Message));
                return null;
            }
        }

        private SerializableQRWait ExtractQRWait(QuestReaction reaction)
        {
            var serialized = new SerializableQRWait();
            PopulateBaseReactionData(reaction, serialized);
            
            serialized.m_WaitTime = GetFieldValue<float>(reaction, "m_WaitTime", 0f);
            
            return serialized;
        }

        private SerializableQRDataTerminalAccessed ExtractQRDataTerminalAccessed(QuestReaction reaction)
        {
            var serialized = new SerializableQRDataTerminalAccessed();
            PopulateBaseReactionData(reaction, serialized);
            
            serialized.m_RequiredLocationID = GetFieldValue<int>(reaction, "m_RequiredLocationID", -1);
            serialized.m_AnyDataTerminal = GetFieldValue<bool>(reaction, "m_AnyDataTerminal", false);
            
            return serialized;
        }

        private SerializableQRFacilityBreached ExtractQRFacilityBreached(QuestReaction reaction)
        {
            var serialized = new SerializableQRFacilityBreached();
            PopulateBaseReactionData(reaction, serialized);
            
            serialized.m_RequiredLocationID = GetFieldValue<int>(reaction, "m_RequiredLocationID", -1);
            serialized.m_AnyFacility = GetFieldValue<bool>(reaction, "m_AnyFacility", false);
            
            return serialized;
        }

        private SerializableQRProgressionData ExtractQRProgressionData(QuestReaction reaction)
        {
            var serialized = new SerializableQRProgressionData();
            PopulateBaseReactionData(reaction, serialized);
            
            serialized.m_ProgressionKey = GetFieldValue<string>(reaction, "m_ProgressionKey", "");
            serialized.m_RequiredValue = GetFieldValue<object>(reaction, "m_RequiredValue", null);
            
            return serialized;
        }

        private SerializableQuestReaction ExtractGenericQuestReaction(QuestReaction reaction)
        {
            var serialized = new SerializableQuestReaction();
            PopulateBaseReactionData(reaction, serialized);
            
            // For generic reactions, store all public fields in parameters dictionary
            var fields = reaction.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var field in fields)
            {
                try
                {
                    var value = field.GetValue(reaction);
                    if (value != null && IsSerializableType(value.GetType()))
                    {
                        serialized.m_Parameters[field.Name] = value;
                    }
                }
                catch (Exception ex)
                {
                    SRInfoHelper.Log(string.Format("QuestDataManager: Failed to extract field {0} - {1}", field.Name, ex.Message));
                }
            }
            
            return serialized;
        }

        private void PopulateBaseReactionData(QuestReaction reaction, SerializableQuestReaction serialized)
        {
            serialized.m_ReactionType = reaction.GetType().Name;
            serialized.m_ID = GetFieldValue<int>(reaction, "m_ID", -1);
            serialized.m_Address = GetFieldValue<string>(reaction, "m_Address", "");
        }

        private void ExtractQuestDescriptions(QuestElement questElement, List<SerializableDescriptionData> descriptions)
        {
            try
            {
                // Extract quest descriptions - this would need to be implemented based on
                // the actual structure of how descriptions are stored in QuestElement
                var descriptionObjects = GetFieldValue<GameObject[]>(questElement, "m_DescriptionObjects", null);
                
                if (descriptionObjects != null)
                {
                    foreach (var descObj in descriptionObjects)
                    {
                        if (descObj != null)
                        {
                            var description = ExtractDescriptionFromGameObject(descObj);
                            if (description != null)
                                descriptions.Add(description);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to extract quest descriptions - {0}", ex.Message));
            }
        }

        private SerializableDescriptionData ExtractDescriptionFromGameObject(GameObject descObj)
        {
            // This would need to be implemented based on how description data is stored
            // Likely in a specific component on the GameObject
            return new SerializableDescriptionData
            {
                m_LocTitle = descObj.name,
                m_Token = "",
                m_Translation = ""
            };
        }

        private void SaveQuestDataAsText(SerializableQuestManager questData)
        {
            try
            {
                var textOutput = new List<string>();
                
                textOutput.Add("=== SATELLITE REIGN QUEST DATA EXPORT ===");
                textOutput.Add(string.Format("Current Display Quest ID: {0}", questData.m_CurrentDisplayQuestID));
                textOutput.Add(string.Format("Random Seed: {0}", questData.m_RandomSeed));
                textOutput.Add(string.Format("Total Quest Elements: {0}", questData.m_QuestElements.Count));
                textOutput.Add("");
                
                foreach (var quest in questData.m_QuestElements)
                {
                    textOutput.Add(string.Format("Quest: {0} (ID: {1})", quest.m_Title, quest.m_ID));
                    textOutput.Add(string.Format("  Hidden: {0}, Show Debrief: {1}", quest.m_Hidden, quest.m_ShowDebrief));
                    textOutput.Add(string.Format("  Actions: {0}, Reactions: {1}", quest.m_Actions.Count, quest.m_Reactions.Count));
                    textOutput.Add(string.Format("  Descriptions: {0}", quest.m_Descriptions.Count));
                    textOutput.Add("");
                }
                
                string filePath = Path.Combine(Manager.GetPluginManager().PluginPath, "questDefinitions.txt");
                FileManager.SaveList(textOutput, filePath);
                
                SRInfoHelper.Log(string.Format("QuestDataManager: Saved readable quest data to {0}", filePath));
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to save text quest data - {0}", ex.Message));
            }
        }

        public bool CheckIfXMLFileExists()
        {
            try
            {
                string filePath = Path.Combine(Manager.GetPluginManager().PluginPath, "questDefinitions.xml");
                return File.Exists(filePath);
            }
            catch
            {
                return false;
            }
        }

        public void LoadQuestDataFromFile()
        {
            try
            {
                if (!IsInitialized)
                {
                    SRInfoHelper.Log("QuestDataManager: Not initialized, cannot load quest data");
                    return;
                }

                string filePath = Path.Combine(Manager.GetPluginManager().PluginPath, "questDefinitions.xml");
                
                if (!File.Exists(filePath))
                {
                    SRInfoHelper.Log("QuestDataManager: Quest definitions file not found");
                    return;
                }

                SRInfoHelper.Log("QuestDataManager: Loading quest data from file");

                var questData = FileManager.LoadQuestDataXML("questDefinitions.xml");
                if (questData != null)
                {
                    ApplyQuestData(questData);
                    SRInfoHelper.Log("QuestDataManager: Successfully loaded quest data");
                }
                else
                {
                    SRInfoHelper.Log("QuestDataManager: Failed to load quest data from XML");
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to load quest data - {0}", ex.Message));
            }
        }

        private void ApplyQuestData(SerializableQuestManager questData)
        {
            try
            {
                var questManager = Manager.GetQuestManager();
                if (questManager == null)
                {
                    SRInfoHelper.Log("QuestDataManager: Quest manager not available for loading");
                    return;
                }

                SRInfoHelper.Log("QuestDataManager: Applying loaded quest data");

                // Apply basic quest manager settings
                SetFieldValue(questManager, "m_CurrentDisplayQuestID", questData.m_CurrentDisplayQuestID);
                SetFieldValue(questManager, "m_RandomSeed", questData.m_RandomSeed);

                // Update used location lists
                SetFieldValue(questManager, "m_UsedRandomDataTerminalLocations", questData.m_UsedRandomDataTerminalLocations);
                SetFieldValue(questManager, "m_UsedRandomVIPLocations", questData.m_UsedRandomVIPLocations);

                // Note: Applying quest elements would require more complex logic to integrate
                // with the existing quest system without breaking save compatibility
                // This is a placeholder for the full implementation

                SRInfoHelper.Log(string.Format("QuestDataManager: Applied quest data with {0} quest elements", questData.m_QuestElements.Count));
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to apply quest data - {0}", ex.Message));
            }
        }

        public SerializableQuestElement CreateNewQuestElement()
        {
            return new SerializableQuestElement
            {
                m_ID = GenerateNewQuestID(),
                m_Title = "NEW_QUEST_TITLE",
                m_Hidden = false,
                m_ShowDebrief = true,
                m_State = global::QuestState.inactive,
                m_IsNew = true,
                m_Address = "",
                m_DeactivateAfterPerformingAction = false,
                m_CanLoadFromSave = true
            };
        }

        public SerializableQAGiveItem CreateGiveItemAction()
        {
            return new SerializableQAGiveItem
            {
                m_ID = GenerateNewActionID(),
                m_Address = "",
                m_DeactivateAfterPerformingAction = true
            };
        }

        public SerializableQAGiveCash CreateGiveCashAction(int amount = 1000)
        {
            return new SerializableQAGiveCash
            {
                m_ID = GenerateNewActionID(),
                m_Address = "",
                m_DeactivateAfterPerformingAction = true,
                m_CashAmount = amount,
                m_ShowFeedback = true,
                m_PlaySound = true
            };
        }

        public SerializableQABroadcastMessage CreateBroadcastMessageAction(string title, string message)
        {
            return new SerializableQABroadcastMessage
            {
                m_ID = GenerateNewActionID(),
                m_Address = "",
                m_DeactivateAfterPerformingAction = true,
                m_MessageTitle = title,
                m_MessageText = message,
                m_DisplayDuration = 5f
            };
        }

        public SerializableQRDataTerminalAccessed CreateDataTerminalReaction()
        {
            return new SerializableQRDataTerminalAccessed
            {
                m_ID = GenerateNewReactionID(),
                m_Address = "",
                m_RequiredLocationID = -1,
                m_AnyDataTerminal = true
            };
        }

        public SerializableQRFacilityBreached CreateFacilityBreachedReaction()
        {
            return new SerializableQRFacilityBreached
            {
                m_ID = GenerateNewReactionID(),
                m_Address = "",
                m_RequiredLocationID = -1,
                m_AnyFacility = true
            };
        }

        private int GenerateNewQuestID()
        {
            // This would need to integrate with the actual quest ID system
            return UnityEngine.Random.Range(10000, 99999);
        }

        private int GenerateNewActionID()
        {
            return UnityEngine.Random.Range(10000, 99999);
        }

        private int GenerateNewReactionID()
        {
            return UnityEngine.Random.Range(10000, 99999);
        }

        // Utility methods
        private T GetFieldValue<T>(object obj, string fieldName, T defaultValue)
        {
            try
            {
                var field = obj.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null)
                {
                    var value = field.GetValue(obj);
                    if (value is T)
                        return (T)value;
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to get field {0} - {1}", fieldName, ex.Message));
            }
            
            return defaultValue;
        }

        private void SetFieldValue<T>(object obj, string fieldName, T value)
        {
            try
            {
                var field = obj.GetType().GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (field != null && field.FieldType.IsAssignableFrom(typeof(T)))
                {
                    field.SetValue(obj, value);
                }
            }
            catch (Exception ex)
            {
                SRInfoHelper.Log(string.Format("QuestDataManager: Failed to set field {0} - {1}", fieldName, ex.Message));
            }
        }

        private bool IsSerializableType(Type type)
        {
            return type.IsPrimitive || 
                   type == typeof(string) || 
                   type == typeof(DateTime) || 
                   type.IsEnum ||
                   type.IsSerializable;
        }
    }
}