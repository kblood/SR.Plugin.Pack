using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using LoadCustomData;
using SRMod.DTOs;
using SRMod.Services;

namespace ItemEditorMod.Services
{
    /// <summary>
    /// ItemEditorService - Core service managing item data state and persistence
    /// Handles loading from game, editing, validation, and saving to XML
    /// Integration point with LoadCustomData for auto-loading
    /// </summary>
    public class ItemEditorService
    {
        #region Fields

        private SerializableItemData _currentItem;
        private List<SerializableItemData> _allItems;
        private List<TranslationElementDTO> _translations;
        private bool _isDirty = false;

        private string _pluginDataPath;
        private const string ITEMS_FILE = "itemDefinitions.xml";
        private const string TRANSLATIONS_FILE = "translations.xml";

        #endregion

        #region Events

        public event Action<SerializableItemData> OnItemChanged;
        public event Action<bool> OnDirtyStateChanged;
        public event Action<List<SerializableItemData>> OnItemListUpdated;

        #endregion

        #region Properties

        public SerializableItemData CurrentItem
        {
            get { return _currentItem; }
        }

        public List<SerializableItemData> AllItems
        {
            get { return _allItems ?? new List<SerializableItemData>(); }
        }

        public bool IsDirty
        {
            get { return _isDirty; }
            private set
            {
                if (_isDirty != value)
                {
                    _isDirty = value;
                    OnDirtyStateChanged?.Invoke(_isDirty);
                }
            }
        }

        #endregion

        #region Constructor

        public ItemEditorService()
        {
            _pluginDataPath = Path.Combine(GetPluginDirectoryPath(), "ItemEditorData");
            EnsureDataDirectoryExists();
            ReloadFromGame();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load all items from game's ItemManager
        /// </summary>
        public void ReloadFromGame()
        {
            try
            {
                Debug.Log("ItemEditorService: Reloading items from game");

                _allItems = new List<SerializableItemData>();
                var itemManager = Manager.GetItemManager();

                if (itemManager == null || itemManager.m_ItemDefinitions == null)
                {
                    Debug.LogWarning("ItemEditorService: ItemManager not available");
                    return;
                }

                // Convert game items to serializable DTOs
                foreach (var gameItem in itemManager.m_ItemDefinitions)
                {
                    _allItems.Add(ConvertGameItemToDTO(gameItem));
                }

                _allItems = _allItems.OrderBy(i => i.m_ID).ToList();

                Debug.Log($"ItemEditorService: Loaded {_allItems.Count} items from game");
                OnItemListUpdated?.Invoke(_allItems);
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorService: ReloadFromGame failed: {e.Message}");
            }
        }

        /// <summary>
        /// Load specific item by ID
        /// </summary>
        public void LoadItem(int itemId)
        {
            try
            {
                var item = _allItems.FirstOrDefault(i => i.m_ID == itemId);
                if (item != null)
                {
                    // Clone the item to avoid modifying original
                    _currentItem = CloneItem(item);
                    IsDirty = false;
                    Debug.Log($"ItemEditorService: Loaded item ID {itemId}");
                    OnItemChanged?.Invoke(_currentItem);
                }
                else
                {
                    Debug.LogWarning($"ItemEditorService: Item ID {itemId} not found");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorService: LoadItem failed: {e.Message}");
            }
        }

        /// <summary>
        /// Update a field in the current item
        /// </summary>
        public void UpdateField(string fieldName, object value)
        {
            try
            {
                if (_currentItem == null)
                {
                    Debug.LogWarning("ItemEditorService: No item loaded");
                    return;
                }

                var field = _currentItem.GetType().GetField(fieldName,
                    System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public);

                if (field != null)
                {
                    field.SetValue(_currentItem, value);
                    IsDirty = true;
                    OnItemChanged?.Invoke(_currentItem);
                }
                else
                {
                    Debug.LogWarning($"ItemEditorService: Field {fieldName} not found");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorService: UpdateField failed: {e.Message}");
            }
        }

        /// <summary>
        /// Save current item and export to XML
        /// </summary>
        public bool SaveCurrentItem()
        {
            try
            {
                if (_currentItem == null)
                {
                    Debug.LogWarning("ItemEditorService: No item to save");
                    return false;
                }

                Debug.Log($"ItemEditorService: Saving item ID {_currentItem.m_ID}");

                // Update or add item to collection
                var existingIndex = _allItems.FindIndex(i => i.m_ID == _currentItem.m_ID);
                if (existingIndex >= 0)
                {
                    _allItems[existingIndex] = _currentItem;
                }
                else
                {
                    _allItems.Add(_currentItem);
                    _allItems = _allItems.OrderBy(i => i.m_ID).ToList();
                }

                // Export to XML
                ExportToXML();

                IsDirty = false;
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorService: SaveCurrentItem failed: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Export all items to XML file (for LoadCustomData)
        /// </summary>
        public bool ExportToXML()
        {
            try
            {
                Debug.Log("ItemEditorService: Exporting items to XML");

                // Create XML container
                var itemDataList = new ItemDataList
                {
                    Items = _allItems
                };

                // Serialize to file
                string filePath = Path.Combine(_pluginDataPath, ITEMS_FILE);
                XmlSerializer serializer = new XmlSerializer(typeof(ItemDataList));
                using (TextWriter writer = new StreamWriter(filePath))
                {
                    serializer.Serialize(writer, itemDataList);
                }

                Debug.Log($"ItemEditorService: Exported {_allItems.Count} items to {filePath}");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorService: ExportToXML failed: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Get next available item ID
        /// </summary>
        public int GetNextAvailableItemId()
        {
            if (_allItems == null || _allItems.Count == 0)
                return 147; // Start custom items at 147

            return _allItems.Max(i => i.m_ID) + 1;
        }

        /// <summary>
        /// Add a new item to the collection
        /// </summary>
        public void AddItem(SerializableItemData item)
        {
            try
            {
                if (item == null)
                {
                    Debug.LogWarning("ItemEditorService: Cannot add null item");
                    return;
                }

                _allItems.Add(item);
                _allItems = _allItems.OrderBy(i => i.m_ID).ToList();
                OnItemListUpdated?.Invoke(_allItems);
                Debug.Log($"ItemEditorService: Added item ID {item.m_ID}");
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorService: AddItem failed: {e.Message}");
            }
        }

        #endregion

        #region Private Methods

        private SerializableItemData ConvertGameItemToDTO(ItemManager.ItemData gameItem)
        {
            var dto = new SerializableItemData
            {
                m_ID = gameItem.m_ID,
                m_FriendlyName = gameItem.m_FriendlyName,
                m_Slot = gameItem.m_Slot,
                m_GearSubCategory = gameItem.m_GearSubCategory,
                m_WeaponType = gameItem.m_WeaponType,
                m_Cost = gameItem.m_Cost,
                m_ResearchCost = gameItem.m_ResearchCost,
                m_BlueprintCost = gameItem.m_BlueprintCost,
                m_PrototypeCost = gameItem.m_PrototypeCost,
                m_Progression = gameItem.m_Progression,
                m_StealthVsCombat = gameItem.m_StealthVsCombat,
                m_AvailableToPlayer = gameItem.m_AvailableToPlayer,
                m_PlayerStartsWithBlueprints = gameItem.m_PlayerStartsWithBlueprints,
                m_PlayerStartsWithPrototype = gameItem.m_PlayerStartsWithPrototype,
                m_PlayerCanResearchFromStart = gameItem.m_PlayerCanResearchFromStart
            };

            return dto;
        }

        private SerializableItemData CloneItem(SerializableItemData item)
        {
            var clone = new SerializableItemData
            {
                m_ID = item.m_ID,
                m_FriendlyName = item.m_FriendlyName,
                m_Slot = item.m_Slot,
                m_GearSubCategory = item.m_GearSubCategory,
                m_WeaponType = item.m_WeaponType,
                m_Cost = item.m_Cost,
                m_ResearchCost = item.m_ResearchCost,
                m_BlueprintCost = item.m_BlueprintCost,
                m_PrototypeCost = item.m_PrototypeCost,
                m_Progression = item.m_Progression,
                m_StealthVsCombat = item.m_StealthVsCombat,
                m_AvailableToPlayer = item.m_AvailableToPlayer,
                m_PlayerStartsWithBlueprints = item.m_PlayerStartsWithBlueprints,
                m_PlayerStartsWithPrototype = item.m_PlayerStartsWithPrototype,
                m_PlayerCanResearchFromStart = item.m_PlayerCanResearchFromStart,
                m_UIIconName = item.m_UIIconName
            };

            return clone;
        }

        private string GetPluginDirectoryPath()
        {
            return Path.Combine(Application.persistentDataPath, "ItemEditorMod");
        }

        private void EnsureDataDirectoryExists()
        {
            if (!Directory.Exists(_pluginDataPath))
            {
                Directory.CreateDirectory(_pluginDataPath);
                Debug.Log($"ItemEditorService: Created data directory at {_pluginDataPath}");
            }
        }

        #endregion
    }
}
