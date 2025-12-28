using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ItemEditorMod.UIHelper;

namespace ItemEditorMod
{
    /// <summary>
    /// ItemEditorMod - Minimal working version using UIHelper pattern
    /// Press F8 to open the item browser
    /// </summary>
    public class ItemEditorModMinimal : ISrPlugin
    {
        #region Fields
        private bool isInitialized = false;
        private const KeyCode EDITOR_HOTKEY = KeyCode.F8;
        
        // Pagination state
        private int currentPage = 0;
        private const int ITEMS_PER_PAGE = 15;
        
        // Current item being edited
        private ItemManager.ItemData currentEditItem = null;
        
        // Search/filter
        private string searchFilter = "";
        private List<ItemManager.ItemData> filteredItems = null;
        #endregion

        #region ISrPlugin Implementation

        public string GetName()
        {
            return "ItemEditorMod (UIHelper) v2.0";
        }

        public void Initialize()
        {
            try
            {
                Debug.Log("ItemEditorMod: Initializing with UIHelper pattern...");
                isInitialized = true;
                Debug.Log("ItemEditorMod: Initialization complete");

                // Show initialization message
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("ItemEditorMod loaded! Press F8 to browse items", 5);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: Initialization failed: {e.Message}\n{e.StackTrace}");
            }
        }

        public void Update()
        {
            if (!isInitialized)
                return;

            try
            {
                // Check for F8 hotkey to open editor
                if (Input.GetKeyDown(EDITOR_HOTKEY))
                {
                    ShowMainMenu();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: Update error: {e.Message}\n{e.StackTrace}");
            }
        }

        #endregion

        #region UI Methods

        private void ShowMainMenu()
        {
            try
            {
                Debug.Log("ItemEditorMod: Showing main menu");

                var buttons = new List<SRModButtonElement>();

                // Browse Items button
                buttons.Add(new SRModButtonElement(
                    "Browse All Items",
                    new UnityAction(() => ShowItemList()),
                    "View all items in the game"));

                // Item Statistics button
                buttons.Add(new SRModButtonElement(
                    "Item Statistics",
                    new UnityAction(() => ShowItemStats()),
                    "Show count and categories of items"));

                // About button
                buttons.Add(new SRModButtonElement(
                    "About",
                    new UnityAction(() => ShowAbout()),
                    "Information about this mod"));

                // Close button
                buttons.Add(new SRModButtonElement(
                    "Close",
                    new UnityAction(() => Debug.Log("ItemEditorMod: Closed")),
                    "Close the item editor"));

                // Use UIHelper to show the menu
                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine("Item Editor", buttons));

                Debug.Log("ItemEditorMod: Main menu shown successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowMainMenu failed: {e.Message}\n{e.StackTrace}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        private void ShowItemList()
        {
            try
            {
                Debug.Log("ItemEditorMod: Loading items from game...");

                // Get items from ItemManager
                var itemManager = Manager.GetItemManager();
                if (itemManager == null)
                {
                    Manager.GetUIManager()?.ShowMessagePopup("ItemManager not available", 3);
                    return;
                }

                var allItems = itemManager.GetAllItems();
                if (allItems == null || allItems.Count == 0)
                {
                    Manager.GetUIManager()?.ShowMessagePopup("No items found in game", 3);
                    return;
                }

                // Apply search filter if set
                List<ItemManager.ItemData> displayItems;
                if (!string.IsNullOrEmpty(searchFilter) && filteredItems != null)
                {
                    displayItems = filteredItems;
                }
                else
                {
                    displayItems = allItems;
                }

                Debug.Log($"ItemEditorMod: Showing {displayItems.Count} items");

                // Calculate pagination
                int totalPages = (displayItems.Count + ITEMS_PER_PAGE - 1) / ITEMS_PER_PAGE;
                int startIdx = currentPage * ITEMS_PER_PAGE;
                int endIdx = Math.Min(startIdx + ITEMS_PER_PAGE, displayItems.Count);

                var buttons = new List<SRModButtonElement>();

                // Search/Filter button
                buttons.Add(new SRModButtonElement(
                    string.IsNullOrEmpty(searchFilter) ? "🔍 Search Items" : $"🔍 Filter: {searchFilter}",
                    new UnityAction(() => ShowSearchMenu()),
                    string.IsNullOrEmpty(searchFilter) ? "Filter items by name" : "Change or clear filter"));

                // Add items for current page
                for (int i = startIdx; i < endIdx; i++)
                {
                    var item = displayItems[i];
                    string itemName = !string.IsNullOrEmpty(item.m_FriendlyName) ? item.m_FriendlyName : $"Item {item.m_ID}";
                    string description = $"ID: {item.m_ID} | Slot: {item.m_Slot}";

                    // Capture item in closure
                    var itemCopy = item;
                    buttons.Add(new SRModButtonElement(
                        itemName,
                        new UnityAction(() => ShowItemEditMenu(itemCopy)),
                        description));
                }

                // Navigation buttons
                if (currentPage > 0)
                {
                    buttons.Add(new SRModButtonElement(
                        "◄ Previous Page",
                        new UnityAction(() => {
                            currentPage--;
                            ShowItemList();
                        }),
                        $"Go to page {currentPage}"));
                }

                if (currentPage < totalPages - 1)
                {
                    buttons.Add(new SRModButtonElement(
                        "Next Page ►",
                        new UnityAction(() => {
                            currentPage++;
                            ShowItemList();
                        }),
                        $"Go to page {currentPage + 2}"));
                }

                // Statistics button
                buttons.Add(new SRModButtonElement(
                    $"📊 Statistics",
                    new UnityAction(() => ShowItemStats()),
                    $"Total: {allItems.Count} items"));

                // Back button
                buttons.Add(new SRModButtonElement(
                    "Back",
                    new UnityAction(() => {
                        currentPage = 0; // Reset to first page
                        searchFilter = "";
                        filteredItems = null;
                        ShowMainMenu();
                    }),
                    "Return to main menu"));

                string title = string.IsNullOrEmpty(searchFilter) 
                    ? $"Items (Page {currentPage + 1}/{totalPages})"
                    : $"Search Results (Page {currentPage + 1}/{totalPages})";

                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine(title, buttons));
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowItemList failed: {e.Message}\n{e.StackTrace}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error loading items: {e.Message}", 5);
            }
        }

        #endregion

        #region Item Edit Menu

        private void ShowItemEditMenu(ItemManager.ItemData item)
        {
            try
            {
                currentEditItem = item;
                string itemName = !string.IsNullOrEmpty(item.m_FriendlyName) ? item.m_FriendlyName : $"Item {item.m_ID}";

                var buttons = new List<SRModButtonElement>();

                // View full details
                buttons.Add(new SRModButtonElement(
                    "📋 View Details",
                    new UnityAction(() => ShowItemDetails(item)),
                    "Show all item properties"));

                // Edit basic properties
                buttons.Add(new SRModButtonElement(
                    "✏️ Edit Name & Cost",
                    new UnityAction(() => ShowEditBasicProperties(item)),
                    "Edit name and cost"));

                // Edit research properties
                buttons.Add(new SRModButtonElement(
                    "🔬 Edit Research",
                    new UnityAction(() => ShowEditResearch(item)),
                    "Edit research and unlock status"));

                // Edit modifiers
                buttons.Add(new SRModButtonElement(
                    "⚡ Edit Modifiers",
                    new UnityAction(() => ShowEditModifiers(item)),
                    $"Edit stats ({item.m_Modifiers?.Length ?? 0} modifiers)"));

                // Clone item
                buttons.Add(new SRModButtonElement(
                    "📄 Clone Item",
                    new UnityAction(() => ShowCloneConfirm(item)),
                    "Create a copy of this item"));

                // Back to list
                buttons.Add(new SRModButtonElement(
                    "Back",
                    new UnityAction(() => ShowItemList()),
                    "Return to item list"));

                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine($"Edit: {itemName}", buttons));
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowItemEditMenu failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        #endregion

        #region Edit Submenus

        private void ShowEditBasicProperties(ItemManager.ItemData item)
        {
            try
            {
                var buttons = new List<SRModButtonElement>();

                // Show current values and allow editing
                buttons.Add(new SRModButtonElement(
                    $"Name: {item.m_FriendlyName}",
                    new UnityAction(() => EditItemName(item)),
                    "Click to change name"));

                buttons.Add(new SRModButtonElement(
                    $"Cost: ${item.m_Cost}",
                    new UnityAction(() => EditItemCost(item)),
                    "Click to change cost"));

                buttons.Add(new SRModButtonElement(
                    $"Slot: {item.m_Slot}",
                    new UnityAction(() => ShowSlotOptions(item)),
                    "Click to change equipment slot"));

                buttons.Add(new SRModButtonElement(
                    $"Icon: {(item.m_UIIcon != null ? item.m_UIIcon.name : "None")}",
                    new UnityAction(() => ShowIconPicker(item)),
                    "Click to change item icon"));

                buttons.Add(new SRModButtonElement(
                    "Back",
                    new UnityAction(() => ShowItemEditMenu(item)),
                    "Return to edit menu"));

                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine("Edit Basic Properties", buttons));
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowEditBasicProperties failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        private void ShowEditResearch(ItemManager.ItemData item)
        {
            try
            {
                var buttons = new List<SRModButtonElement>();

                // Toggle blueprints
                string blueprintStatus = item.m_PlayerHasBlueprints ? "✓ Has Blueprints" : "✗ No Blueprints";
                buttons.Add(new SRModButtonElement(
                    blueprintStatus,
                    new UnityAction(() => {
                        item.m_PlayerHasBlueprints = !item.m_PlayerHasBlueprints;
                        Manager.GetUIManager()?.ShowMessagePopup(
                            $"Blueprints: {(item.m_PlayerHasBlueprints ? "Enabled" : "Disabled")}", 2);
                        ShowEditResearch(item);
                    }),
                    "Toggle blueprint ownership"));

                // Toggle prototype
                string prototypeStatus = item.m_PlayerHasPrototype ? "✓ Has Prototype" : "✗ No Prototype";
                buttons.Add(new SRModButtonElement(
                    prototypeStatus,
                    new UnityAction(() => {
                        item.m_PlayerHasPrototype = !item.m_PlayerHasPrototype;
                        Manager.GetUIManager()?.ShowMessagePopup(
                            $"Prototype: {(item.m_PlayerHasPrototype ? "Enabled" : "Disabled")}", 2);
                        ShowEditResearch(item);
                    }),
                    "Toggle prototype ownership"));

                // Toggle available to player
                string availableStatus = item.m_AvailableToPlayer ? "✓ Available" : "✗ Not Available";
                buttons.Add(new SRModButtonElement(
                    availableStatus,
                    new UnityAction(() => {
                        item.m_AvailableToPlayer = !item.m_AvailableToPlayer;
                        Manager.GetUIManager()?.ShowMessagePopup(
                            $"Available: {(item.m_AvailableToPlayer ? "Yes" : "No")}", 2);
                        ShowEditResearch(item);
                    }),
                    "Toggle availability to player"));

                buttons.Add(new SRModButtonElement(
                    "Back",
                    new UnityAction(() => ShowItemEditMenu(item)),
                    "Return to edit menu"));

                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine("Edit Research", buttons));
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowEditResearch failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        private void ShowEditModifiers(ItemManager.ItemData item)
        {
            try
            {
                var buttons = new List<SRModButtonElement>();

                if (item.m_Modifiers != null && item.m_Modifiers.Length > 0)
                {
                    for (int i = 0; i < item.m_Modifiers.Length && i < 10; i++)
                    {
                        var mod = item.m_Modifiers[i];
                        string modInfo = $"{mod.m_Type}: {mod.m_Ammount:F2}";
                        
                        buttons.Add(new SRModButtonElement(
                            modInfo,
                            new UnityAction(() => Manager.GetUIManager()?.ShowMessagePopup(
                                "Modifier editing coming soon!", 2)),
                            "Modifier details"));
                    }
                }
                else
                {
                    buttons.Add(new SRModButtonElement(
                        "No Modifiers",
                        new UnityAction(() => {}),
                        "This item has no modifiers"));
                }

                buttons.Add(new SRModButtonElement(
                    "Back",
                    new UnityAction(() => ShowItemEditMenu(item)),
                    "Return to edit menu"));

                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine(
                        $"Modifiers ({item.m_Modifiers?.Length ?? 0})", 
                        buttons));
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowEditModifiers failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        #endregion

        #region Edit Actions

        private void EditItemName(ItemManager.ItemData item)
        {
            try
            {
                // Create a simple name edit menu with preset options
                var buttons = new List<SRModButtonElement>();

                // Common name prefixes/suffixes
                buttons.Add(new SRModButtonElement(
                    "Add 'Custom' Prefix",
                    new UnityAction(() => {
                        item.m_FriendlyName = "Custom " + item.m_FriendlyName;
                        UpdateItemTranslation(item);
                        Manager.GetUIManager()?.ShowMessagePopup($"Name changed to:\n{item.m_FriendlyName}", 3);
                        ShowEditBasicProperties(item);
                    }),
                    $"Rename to: Custom {item.m_FriendlyName}"));

                buttons.Add(new SRModButtonElement(
                    "Add 'Modified' Prefix",
                    new UnityAction(() => {
                        item.m_FriendlyName = "Modified " + item.m_FriendlyName;
                        UpdateItemTranslation(item);
                        Manager.GetUIManager()?.ShowMessagePopup($"Name changed to:\n{item.m_FriendlyName}", 3);
                        ShowEditBasicProperties(item);
                    }),
                    $"Rename to: Modified {item.m_FriendlyName}"));

                buttons.Add(new SRModButtonElement(
                    "Add 'Enhanced' Prefix",
                    new UnityAction(() => {
                        item.m_FriendlyName = "Enhanced " + item.m_FriendlyName;
                        UpdateItemTranslation(item);
                        Manager.GetUIManager()?.ShowMessagePopup($"Name changed to:\n{item.m_FriendlyName}", 3);
                        ShowEditBasicProperties(item);
                    }),
                    $"Rename to: Enhanced {item.m_FriendlyName}"));

                buttons.Add(new SRModButtonElement(
                    "Add 'Mk II' Suffix",
                    new UnityAction(() => {
                        item.m_FriendlyName = item.m_FriendlyName + " Mk II";
                        UpdateItemTranslation(item);
                        Manager.GetUIManager()?.ShowMessagePopup($"Name changed to:\n{item.m_FriendlyName}", 3);
                        ShowEditBasicProperties(item);
                    }),
                    $"Rename to: {item.m_FriendlyName} Mk II"));

                buttons.Add(new SRModButtonElement(
                    "Add 'Plus' Suffix",
                    new UnityAction(() => {
                        item.m_FriendlyName = item.m_FriendlyName + " Plus";
                        UpdateItemTranslation(item);
                        Manager.GetUIManager()?.ShowMessagePopup($"Name changed to:\n{item.m_FriendlyName}", 3);
                        ShowEditBasicProperties(item);
                    }),
                    $"Rename to: {item.m_FriendlyName} Plus"));

                // Remove last word
                if (item.m_FriendlyName.Contains(" "))
                {
                    buttons.Add(new SRModButtonElement(
                        "Remove Last Word",
                        new UnityAction(() => {
                            int lastSpace = item.m_FriendlyName.LastIndexOf(' ');
                            if (lastSpace > 0)
                            {
                                item.m_FriendlyName = item.m_FriendlyName.Substring(0, lastSpace);
                                UpdateItemTranslation(item);
                                Manager.GetUIManager()?.ShowMessagePopup($"Name changed to:\n{item.m_FriendlyName}", 3);
                                ShowEditBasicProperties(item);
                            }
                        }),
                        "Remove the last word"));
                }

                buttons.Add(new SRModButtonElement(
                    "Back",
                    new UnityAction(() => ShowEditBasicProperties(item)),
                    "Cancel"));

                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine($"Edit Name: {item.m_FriendlyName}", buttons));
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: EditItemName failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        private void UpdateItemTranslation(ItemManager.ItemData item)
        {
            try
            {
                var textManager = TextManager.Get();
                if (textManager == null) return;

                // Use reflection to get the language lookup dictionary
                var langLookupField = textManager.GetType().GetField("m_FastLanguageLookup", 
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (langLookupField == null) return;

                var langLookup = langLookupField.GetValue(textManager) as Dictionary<string, TextManager.LocElement>;
                if (langLookup == null) return;

                string nameKey = "ITEM_" + item.m_ID + "_NAME";

                // Update or create translation
                if (langLookup.ContainsKey(nameKey))
                {
                    langLookup[nameKey].m_Translations[2] = item.m_FriendlyName; // English
                }
                else
                {
                    var nameElement = new TextManager.LocElement
                    {
                        m_token = nameKey,
                        m_Translations = new string[8]
                    };
                    nameElement.m_Translations[2] = item.m_FriendlyName;
                    langLookup[nameKey] = nameElement;
                }

                Debug.Log($"ItemEditorMod: Updated translation for item {item.m_ID} to '{item.m_FriendlyName}'");
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: UpdateItemTranslation failed: {e.Message}");
            }
        }

        private void EditItemCost(ItemManager.ItemData item)
        {
            try
            {
                // Create cost adjustment menu
                var buttons = new List<SRModButtonElement>();

                // Quick adjustments
                buttons.Add(new SRModButtonElement(
                    $"Set to $100",
                    new UnityAction(() => {
                        item.m_Cost = 100f;
                        Manager.GetUIManager()?.ShowMessagePopup($"Cost set to $100", 2);
                        ShowEditBasicProperties(item);
                    }),
                    "Low cost"));

                buttons.Add(new SRModButtonElement(
                    $"Set to $500",
                    new UnityAction(() => {
                        item.m_Cost = 500f;
                        Manager.GetUIManager()?.ShowMessagePopup($"Cost set to $500", 2);
                        ShowEditBasicProperties(item);
                    }),
                    "Medium cost"));

                buttons.Add(new SRModButtonElement(
                    $"Set to $1000",
                    new UnityAction(() => {
                        item.m_Cost = 1000f;
                        Manager.GetUIManager()?.ShowMessagePopup($"Cost set to $1000", 2);
                        ShowEditBasicProperties(item);
                    }),
                    "High cost"));

                buttons.Add(new SRModButtonElement(
                    $"Set to $2500",
                    new UnityAction(() => {
                        item.m_Cost = 2500f;
                        Manager.GetUIManager()?.ShowMessagePopup($"Cost set to $2500", 2);
                        ShowEditBasicProperties(item);
                    }),
                    "Very high cost"));

                // Relative adjustments
                buttons.Add(new SRModButtonElement(
                    $"Double Cost (${item.m_Cost * 2:F0})",
                    new UnityAction(() => {
                        item.m_Cost *= 2f;
                        Manager.GetUIManager()?.ShowMessagePopup($"Cost doubled to ${item.m_Cost:F0}", 2);
                        ShowEditBasicProperties(item);
                    }),
                    "Multiply by 2"));

                buttons.Add(new SRModButtonElement(
                    $"Half Cost (${item.m_Cost / 2:F0})",
                    new UnityAction(() => {
                        item.m_Cost /= 2f;
                        Manager.GetUIManager()?.ShowMessagePopup($"Cost halved to ${item.m_Cost:F0}", 2);
                        ShowEditBasicProperties(item);
                    }),
                    "Divide by 2"));

                buttons.Add(new SRModButtonElement(
                    $"Make Free ($0)",
                    new UnityAction(() => {
                        item.m_Cost = 0f;
                        Manager.GetUIManager()?.ShowMessagePopup($"Item is now free!", 2);
                        ShowEditBasicProperties(item);
                    }),
                    "Set cost to zero"));

                buttons.Add(new SRModButtonElement(
                    "Back",
                    new UnityAction(() => ShowEditBasicProperties(item)),
                    "Cancel"));

                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine($"Edit Cost (Current: ${item.m_Cost:F0})", buttons));
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: EditItemCost failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        private void ShowSlotOptions(ItemManager.ItemData item)
        {
            try
            {
                var buttons = new List<SRModButtonElement>();
                
                // Add slot type options
                var slotTypes = Enum.GetValues(typeof(ItemSlotTypes));
                foreach (ItemSlotTypes slot in slotTypes)
                {
                    string current = (item.m_Slot == slot) ? "✓ " : "";
                    buttons.Add(new SRModButtonElement(
                        current + slot.ToString(),
                        new UnityAction(() => {
                            item.m_Slot = slot;
                            Manager.GetUIManager()?.ShowMessagePopup($"Slot changed to: {slot}", 2);
                            ShowEditBasicProperties(item);
                        }),
                        $"Set slot to {slot}"));
                }

                buttons.Add(new SRModButtonElement(
                    "Back",
                    new UnityAction(() => ShowEditBasicProperties(item)),
                    "Cancel"));

                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine("Select Slot", buttons));
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowSlotOptions failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        #endregion

        #region Item Operations

        private void ShowItemDetails(ItemManager.ItemData item)
        {
            try
            {
                // Build comprehensive item details
                string details = $"Item: {item.m_FriendlyName}\n" +
                                $"ID: {item.m_ID}\n" +
                                $"Slot: {item.m_Slot}\n" +
                                $"Cost: ${item.m_Cost}\n";

                // Add category if available
                if (item.m_GearSubCategory != 0)
                {
                    details += $"Category: {item.m_GearSubCategory}\n";
                }

                // Add weapon type
                if (item.m_WeaponType != 0)
                {
                    details += $"Weapon: {item.m_WeaponType}\n";
                }

                // Add research info
                if (item.m_PlayerHasBlueprints)
                {
                    details += $"Has Blueprint: Yes\n";
                }
                if (item.m_PlayerHasPrototype)
                {
                    details += $"Has Prototype: Yes\n";
                }

                // Add modifiers count
                if (item.m_Modifiers != null && item.m_Modifiers.Length > 0)
                {
                    details += $"Modifiers: {item.m_Modifiers.Length}\n";
                }

                Manager.GetUIManager()?.ShowMessagePopup(details, 10);
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowItemDetails failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        private void ShowItemStats()
        {
            try
            {
                var itemManager = Manager.GetItemManager();
                if (itemManager == null)
                {
                    Manager.GetUIManager()?.ShowMessagePopup("ItemManager not available", 3);
                    return;
                }

                var allItems = itemManager.GetAllItems();

                // Count by slot type
                var slotCount = new Dictionary<string, int>();
                foreach (var item in allItems)
                {
                    string slot = item.m_Slot.ToString();
                    if (!slotCount.ContainsKey(slot))
                        slotCount[slot] = 0;
                    slotCount[slot]++;
                }

                string stats = $"Total Items: {allItems.Count}\n\nBy Slot:\n";
                foreach (var kvp in slotCount.OrderByDescending(x => x.Value))
                {
                    stats += $"{kvp.Key}: {kvp.Value}\n";
                }

                Manager.GetUIManager()?.ShowMessagePopup(stats, 10);
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowItemStats failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        private void ShowAbout()
        {
            string about = "Item Editor Mod v2.0\n\n" +
                          "Uses the proven UIHelper pattern\n" +
                          "from Satellite Reign modding.\n\n" +
                          "Features:\n" +
                          "- Browse all items\n" +
                          "- View item statistics\n" +
                          "- Clean button-based UI\n\n" +
                          "Press F8 to open";

            Manager.GetUIManager()?.ShowMessagePopup(about, 10);
        }

        #endregion

        #region Save/Load Features

        private void SaveAllItemsToXML()
        {
            try
            {
                Debug.Log("ItemEditorMod: Saving all items to XML...");

                var itemManager = Manager.GetItemManager();
                if (itemManager == null)
                {
                    Manager.GetUIManager()?.ShowMessagePopup("ItemManager not available", 3);
                    return;
                }

                // Get all items and convert to serializable format
                var allItems = itemManager.GetAllItems();
                // Note: Full serialization requires SRMod.DTOs which has LoadCustomData dependencies
                // For now, we'll just log what would be saved
                Debug.Log($"ItemEditorMod: Would save {allItems.Count} items");
                Manager.GetUIManager()?.ShowMessagePopup(
                    $"Save feature requires LoadCustomData\n\n" +
                    $"Items in memory: {allItems.Count}\n\n" +
                    $"Use LoadCustomData mod to save items to XML", 5);

                /* TODO: Add SerializableItemData once LoadCustomData is properly referenced
                foreach (var item in allItems)
                {
                    try
                    {
                        serializableItems.Add(new SRMod.DTOs.SerializableItemData(item));
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"ItemEditorMod: Failed to serialize item {item.m_ID}: {ex.Message}");
                    }
                }

                // Serialize to XML
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(List<SRMod.DTOs.SerializableItemData>));
                string path = System.IO.Path.Combine(Manager.GetPluginManager().PluginPath, "itemDefinitions.xml");

                using (var streamWriter = new System.IO.StreamWriter(path))
                {
                    serializer.Serialize(streamWriter, serializableItems);
                }

                Debug.Log($"ItemEditorMod: Saved {serializableItems.Count} items to {path}");
                Manager.GetUIManager()?.ShowMessagePopup(
                    $"✅ Saved Successfully!\n\n" +
                    $"Saved {serializableItems.Count} items\n" +
                    $"to: itemDefinitions.xml\n\n" +
                    $"Restart game to load changes", 8);
                */
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: SaveAllItemsToXML failed: {e.Message}\n{e.StackTrace}");
                Manager.GetUIManager()?.ShowMessagePopup($"❌ Save Failed!\n\n{e.Message}", 5);
            }
        }

        #endregion

        #region Search/Filter

        private void ShowSearchMenu()
        {
            try
            {
                var buttons = new List<SRModButtonElement>();

                // Common search filters
                buttons.Add(new SRModButtonElement(
                    "🔍 Search by Weapon",
                    new UnityAction(() => ApplySearchFilter("weapon")),
                    "Show only weapons"));

                buttons.Add(new SRModButtonElement(
                    "🔍 Search by Aug",
                    new UnityAction(() => ApplySearchFilter("aug")),
                    "Show only augmentations"));

                buttons.Add(new SRModButtonElement(
                    "🔍 Search by Grenade",
                    new UnityAction(() => ApplySearchFilter("grenade")),
                    "Show only grenades"));

                buttons.Add(new SRModButtonElement(
                    "🔍 Search by Item",
                    new UnityAction(() => ApplySearchFilter("item")),
                    "Show only items"));

                // Slot filters (using actual enum values from game)
                var firstSlot = (ItemSlotTypes)0;
                foreach (ItemSlotTypes slot in Enum.GetValues(typeof(ItemSlotTypes)))
                {
                    if ((int)slot > 10) break; // Limit to first few slots
                    
                    buttons.Add(new SRModButtonElement(
                        $"📦 Filter by {slot}",
                        new UnityAction(() => ApplySlotFilter(slot)),
                        $"Show {slot} slot items"));
                }

                // Clear filter
                if (!string.IsNullOrEmpty(searchFilter))
                {
                    buttons.Add(new SRModButtonElement(
                        "✖ Clear Filter",
                        new UnityAction(() => {
                            searchFilter = "";
                            filteredItems = null;
                            currentPage = 0;
                            ShowItemList();
                        }),
                        "Show all items"));
                }

                buttons.Add(new SRModButtonElement(
                    "Back",
                    new UnityAction(() => ShowItemList()),
                    "Return to item list"));

                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine("Search & Filter", buttons));
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowSearchMenu failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        private void ApplySearchFilter(string filter)
        {
            try
            {
                var itemManager = Manager.GetItemManager();
                if (itemManager == null) return;

                var allItems = itemManager.GetAllItems();
                searchFilter = filter;
                filteredItems = allItems.Where(item => 
                    (!string.IsNullOrEmpty(item.m_FriendlyName) && 
                     item.m_FriendlyName.ToLower().Contains(filter.ToLower()))
                ).ToList();

                currentPage = 0;
                Manager.GetUIManager()?.ShowMessagePopup($"Found {filteredItems.Count} items matching '{filter}'", 2);
                ShowItemList();
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ApplySearchFilter failed: {e.Message}");
            }
        }

        private void ApplySlotFilter(ItemSlotTypes slot)
        {
            try
            {
                var itemManager = Manager.GetItemManager();
                if (itemManager == null) return;

                var allItems = itemManager.GetAllItems();
                searchFilter = slot.ToString();
                filteredItems = allItems.Where(item => item.m_Slot == slot).ToList();

                currentPage = 0;
                Manager.GetUIManager()?.ShowMessagePopup($"Found {filteredItems.Count} items in {slot} slot", 2);
                ShowItemList();
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ApplySlotFilter failed: {e.Message}");
            }
        }

        #endregion

        #region Create New Item

        private void ShowCreateNewItemMenu()
        {
            try
            {
                var buttons = new List<SRModButtonElement>();

                buttons.Add(new SRModButtonElement(
                    "📄 Clone Existing Item",
                    new UnityAction(() => {
                        searchFilter = "";
                        filteredItems = null;
                        currentPage = 0;
                        Manager.GetUIManager()?.ShowMessagePopup("Select an item to clone", 2);
                        ShowItemList();
                    }),
                    "Select an item to clone"));

                buttons.Add(new SRModButtonElement(
                    "➕ Create from Template",
                    new UnityAction(() => ShowTemplateSelector()),
                    "Create new item from template"));

                buttons.Add(new SRModButtonElement(
                    "Back",
                    new UnityAction(() => ShowMainMenu()),
                    "Return to main menu"));

                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine("Create New Item", buttons));
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowCreateNewItemMenu failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        private void ShowTemplateSelector()
        {
            Manager.GetUIManager()?.ShowMessagePopup(
                "Template system coming soon!\n\n" +
                "Will allow creating items from:\n" +
                "- Weapon templates\n" +
                "- Augmentation templates\n" +
                "- Item templates\n" +
                "- Grenade templates", 5);
        }

        private void ShowCloneConfirm(ItemManager.ItemData item)
        {
            try
            {
                var buttons = new List<SRModButtonElement>();

                buttons.Add(new SRModButtonElement(
                    $"✓ Clone '{item.m_FriendlyName}'",
                    new UnityAction(() => CloneItem(item)),
                    $"Create copy of item {item.m_ID}"));

                buttons.Add(new SRModButtonElement(
                    "Cancel",
                    new UnityAction(() => ShowItemEditMenu(item)),
                    "Go back"));

                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine("Confirm Clone", buttons));
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowCloneConfirm failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        private void CloneItem(ItemManager.ItemData sourceItem)
        {
            try
            {
                Debug.Log($"ItemEditorMod: Cloning item {sourceItem.m_ID}...");

                var itemManager = Manager.GetItemManager();
                if (itemManager == null)
                {
                    Manager.GetUIManager()?.ShowMessagePopup("ItemManager not available", 3);
                    return;
                }

                // Find next available ID
                var allItems = itemManager.GetAllItems();
                int newID = allItems.Max(i => i.m_ID) + 1;

                // Create new item data
                var newItem = new ItemManager.ItemData();
                
                // Copy all properties
                newItem.m_ID = newID;
                newItem.m_FriendlyName = sourceItem.m_FriendlyName + " (Copy)";
                newItem.m_PrereqID = sourceItem.m_PrereqID;
                newItem.m_Slot = sourceItem.m_Slot;
                newItem.m_GearSubCategory = sourceItem.m_GearSubCategory;
                newItem.m_UIIcon = sourceItem.m_UIIcon;
                newItem.m_WeaponType = sourceItem.m_WeaponType;
                newItem.m_ValidWeaponAugmentationWeaponMask = sourceItem.m_ValidWeaponAugmentationWeaponMask;
                newItem.m_PrototypeProgressionValue = sourceItem.m_PrototypeProgressionValue;
                newItem.m_BlueprintProgressionValue = sourceItem.m_BlueprintProgressionValue;
                newItem.m_StealthVsCombat = sourceItem.m_StealthVsCombat;
                newItem.m_Cost = sourceItem.m_Cost;
                newItem.m_ResearchCost = sourceItem.m_ResearchCost;
                newItem.m_Progression = sourceItem.m_Progression;
                newItem.m_MinResearchersRequired = sourceItem.m_MinResearchersRequired;
                newItem.m_AvailableToPlayer = true;
                newItem.m_PlayerHasBlueprints = false;
                newItem.m_PlayerHasPrototype = false;

                // Copy modifiers
                if (sourceItem.m_Modifiers != null && sourceItem.m_Modifiers.Length > 0)
                {
                    newItem.m_Modifiers = new ModifierData5L[sourceItem.m_Modifiers.Length];
                    for (int i = 0; i < sourceItem.m_Modifiers.Length; i++)
                    {
                        newItem.m_Modifiers[i] = new ModifierData5L
                        {
                            m_Type = sourceItem.m_Modifiers[i].m_Type,
                            m_Ammount = sourceItem.m_Modifiers[i].m_Ammount
                        };
                    }
                }

                // Copy abilities
                if (sourceItem.m_AbilityIDs != null)
                {
                    newItem.m_AbilityIDs = new List<int>(sourceItem.m_AbilityIDs);
                }
                if (sourceItem.m_AbilityMasks != null)
                {
                    newItem.m_AbilityMasks = new List<int>(sourceItem.m_AbilityMasks);
                }

                // Add to item manager
                itemManager.m_ItemDefinitions.Add(newItem);

                Debug.Log($"ItemEditorMod: Created new item with ID {newID}");
                Manager.GetUIManager()?.ShowMessagePopup(
                    $"✅ Item Cloned!\n\n" +
                    $"New ID: {newID}\n" +
                    $"Name: {newItem.m_FriendlyName}\n\n" +
                    $"Don't forget to save!", 5);

                ShowItemEditMenu(newItem);
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: CloneItem failed: {e.Message}\n{e.StackTrace}");
                Manager.GetUIManager()?.ShowMessagePopup($"❌ Clone Failed!\n\n{e.Message}", 5);
            }
        }

        #endregion

        #region Icon Picker

        private void ShowIconPicker(ItemManager.ItemData item)
        {
            try
            {
                Debug.Log("ItemEditorMod: Showing icon picker");
                
                // Get all items to extract unique icon names
                var itemManager = Manager.GetItemManager();
                if (itemManager == null) return;

                var allItems = itemManager.GetAllItems();
                var iconNames = new HashSet<string>();

                // Collect all unique icon names
                foreach (var gameItem in allItems)
                {
                    if (gameItem.m_UIIcon != null && !string.IsNullOrEmpty(gameItem.m_UIIcon.name))
                    {
                        iconNames.Add(gameItem.m_UIIcon.name);
                    }
                }

                var iconList = iconNames.OrderBy(x => x).ToList();
                Debug.Log($"ItemEditorMod: Found {iconList.Count} unique icons");

                ShowIconPickerPage(item, iconList, 0);
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowIconPicker failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        private void ShowIconPickerPage(ItemManager.ItemData item, List<string> allIcons, int page)
        {
            try
            {
                const int ICONS_PER_PAGE = 12;
                int totalPages = (allIcons.Count + ICONS_PER_PAGE - 1) / ICONS_PER_PAGE;
                int startIdx = page * ICONS_PER_PAGE;
                int endIdx = Math.Min(startIdx + ICONS_PER_PAGE, allIcons.Count);

                var buttons = new List<SRModButtonElement>();

                // Add icon options for current page
                for (int i = startIdx; i < endIdx; i++)
                {
                    string iconName = allIcons[i];
                    bool isCurrent = item.m_UIIcon != null && item.m_UIIcon.name == iconName;
                    string displayName = isCurrent ? $"✓ {iconName}" : iconName;

                    buttons.Add(new SRModButtonElement(
                        displayName,
                        new UnityAction(() => SelectIcon(item, iconName)),
                        isCurrent ? "Current icon" : "Click to select"));
                }

                // Navigation
                if (page > 0)
                {
                    buttons.Add(new SRModButtonElement(
                        "◄ Previous",
                        new UnityAction(() => ShowIconPickerPage(item, allIcons, page - 1)),
                        "Previous page"));
                }

                if (page < totalPages - 1)
                {
                    buttons.Add(new SRModButtonElement(
                        "Next ►",
                        new UnityAction(() => ShowIconPickerPage(item, allIcons, page + 1)),
                        "Next page"));
                }

                buttons.Add(new SRModButtonElement(
                    "Back",
                    new UnityAction(() => ShowEditBasicProperties(item)),
                    "Cancel"));

                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine(
                        $"Select Icon (Page {page + 1}/{totalPages})", 
                        buttons));
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: ShowIconPickerPage failed: {e.Message}");
            }
        }

        private void SelectIcon(ItemManager.ItemData item, string iconName)
        {
            try
            {
                Debug.Log($"ItemEditorMod: Selecting icon '{iconName}' for item {item.m_ID}");

                // Find an item with this icon to copy the sprite reference
                var itemManager = Manager.GetItemManager();
                if (itemManager == null) return;

                var allItems = itemManager.GetAllItems();
                var sourceItem = allItems.FirstOrDefault(x => 
                    x.m_UIIcon != null && x.m_UIIcon.name == iconName);

                if (sourceItem != null && sourceItem.m_UIIcon != null)
                {
                    item.m_UIIcon = sourceItem.m_UIIcon;
                    Debug.Log($"ItemEditorMod: Icon changed to '{iconName}'");
                    Manager.GetUIManager()?.ShowMessagePopup($"Icon changed to:\n{iconName}", 3);
                    ShowEditBasicProperties(item);
                }
                else
                {
                    Manager.GetUIManager()?.ShowMessagePopup($"Could not load icon: {iconName}", 3);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorMod: SelectIcon failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error selecting icon: {e.Message}", 5);
            }
        }

        #endregion
    }
}
