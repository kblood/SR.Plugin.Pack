using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ItemEditorMod.Services;
using ItemEditorMod.UIHelper;
using ItemEditorMod.Models;

namespace ItemEditorMod.UI
{
    /// <summary>
    /// ItemEditorUI - Simplified UI using proven UIHelper pattern
    /// Uses UIHelper.ModalVerticalButtonsRoutine for display
    /// </summary>
    public class ItemEditorUI
    {
        #region Fields

        private ItemEditorService _editorService;
        private ValidationService _validationService;
        private IconManagementService _iconService;
        private ItemCloneService _cloneService;
        private bool _isVisible = false;

        #endregion

        #region Constructor

        public ItemEditorUI(ItemEditorService editorService, ValidationService validationService,
            IconManagementService iconService, ItemCloneService cloneService)
        {
            _editorService = editorService;
            _validationService = validationService;
            _iconService = iconService;
            _cloneService = cloneService;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Show the main item list menu
        /// </summary>
        public void Show()
        {
            try
            {
                Debug.Log("ItemEditorUI: Show() called");
                _isVisible = true;

                // Reload items from game
                _editorService.ReloadFromGame();

                // Show main menu
                ShowMainMenu();
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorUI: Show failed: {e.Message}\n{e.StackTrace}");
                Manager.GetUIManager()?.ShowMessagePopup($"UI Error: {e.Message}", 5);
            }
        }

        /// <summary>
        /// Hide the editor UI
        /// </summary>
        public void Hide()
        {
            try
            {
                _isVisible = false;
                Debug.Log("ItemEditorUI: Editor hidden");
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorUI: Hide failed: {e.Message}");
            }
        }

        #endregion

        #region Main Menu

        private void ShowMainMenu()
        {
            var buttons = new List<SRModButtonElement>();

            // Browse all items
            buttons.Add(new SRModButtonElement(
                "Browse All Items",
                new UnityAction(() => ShowItemList()),
                "View and edit all items in the game"));

            // Create new item
            buttons.Add(new SRModButtonElement(
                "Create New Item",
                new UnityAction(() => CreateNewItem()),
                "Clone an existing item to create a new one"));

            // Save all items
            buttons.Add(new SRModButtonElement(
                "Save All Items",
                new UnityAction(() => SaveAllItems()),
                "Save all item changes to XML files"));

            // Validate items
            buttons.Add(new SRModButtonElement(
                "Validate All Items",
                new UnityAction(() => ValidateAllItems()),
                "Check all items for errors"));

            // Close button
            buttons.Add(new SRModButtonElement(
                "Close",
                new UnityAction(() => Hide()),
                "Close the item editor"));

            Manager.Get().StartCoroutine(
                UIHelper.UIHelper.ModalVerticalButtonsRoutine("Item Editor", buttons));
        }

        #endregion

        #region Item List

        private void ShowItemList()
        {
            try
            {
                var items = _editorService.GetAllItems();
                var buttons = new List<SRModButtonElement>();

                // Add items grouped by type
                foreach (var item in items)
                {
                    string itemName = item.m_Name ?? $"Item {item.m_ID}";
                    string description = $"ID: {item.m_ID} | Type: {item.m_ItemCategory}";

                    buttons.Add(new SRModButtonElement(
                        itemName,
                        new UnityAction(() => ShowItemMenu(item)),
                        description));

                    // Limit to 20 items per page to avoid UI clutter
                    if (buttons.Count >= 20)
                    {
                        buttons.Add(new SRModButtonElement(
                            "More items...",
                            new UnityAction(() => Manager.GetUIManager().ShowMessagePopup(
                                "Too many items! Use search/filter (coming soon)", 3)),
                            "Browse more items"));
                        break;
                    }
                }

                // Back button
                buttons.Add(new SRModButtonElement(
                    "Back",
                    new UnityAction(() => ShowMainMenu()),
                    "Return to main menu"));

                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine("Select Item", buttons));
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorUI: ShowItemList failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        #endregion

        #region Item Edit Menu

        private void ShowItemMenu(ItemData item)
        {
            try
            {
                _editorService.SelectItem(item.m_ID);

                var buttons = new List<SRModButtonElement>();

                string itemName = item.m_Name ?? $"Item {item.m_ID}";

                // Edit basic info
                buttons.Add(new SRModButtonElement(
                    "Edit Basic Info",
                    new UnityAction(() => ShowBasicInfoMenu(item)),
                    "Name, description, cost"));

                // Edit combat stats
                buttons.Add(new SRModButtonElement(
                    "Edit Combat Stats",
                    new UnityAction(() => ShowCombatStatsMenu(item)),
                    "Damage, range, accuracy"));

                // Edit abilities
                buttons.Add(new SRModButtonElement(
                    "Edit Abilities",
                    new UnityAction(() => ShowAbilitiesMenu(item)),
                    "Add or remove item abilities"));

                // Clone item
                buttons.Add(new SRModButtonElement(
                    "Clone This Item",
                    new UnityAction(() => CloneItem(item)),
                    "Create a copy of this item"));

                // Delete item
                buttons.Add(new SRModButtonElement(
                    "Delete Item",
                    new UnityAction(() => DeleteItem(item)),
                    "Remove this item (cannot be undone)"));

                // Back button
                buttons.Add(new SRModButtonElement(
                    "Back",
                    new UnityAction(() => ShowItemList()),
                    "Return to item list"));

                Manager.Get().StartCoroutine(
                    UIHelper.UIHelper.ModalVerticalButtonsRoutine($"Edit: {itemName}", buttons));
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorUI: ShowItemMenu failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Error: {e.Message}", 5);
            }
        }

        #endregion

        #region Item Operations

        private void ShowBasicInfoMenu(ItemData item)
        {
            Manager.GetUIManager()?.ShowMessagePopup(
                $"Basic Info for {item.m_Name}\n" +
                $"ID: {item.m_ID}\n" +
                $"Cost: {item.m_ItemCost}\n" +
                $"Category: {item.m_ItemCategory}\n" +
                $"\nEditing coming soon!", 5);
        }

        private void ShowCombatStatsMenu(ItemData item)
        {
            Manager.GetUIManager()?.ShowMessagePopup(
                $"Combat Stats for {item.m_Name}\n" +
                $"Damage: {item.m_DamageValue}\n" +
                $"Range: {item.m_FireRange}\n" +
                $"Accuracy: {item.m_Accuracy}\n" +
                $"\nEditing coming soon!", 5);
        }

        private void ShowAbilitiesMenu(ItemData item)
        {
            string abilities = "None";
            if (item.m_Abilities != null && item.m_Abilities.Length > 0)
            {
                abilities = string.Join(", ", System.Array.ConvertAll(item.m_Abilities, a => a.ToString()));
            }

            Manager.GetUIManager()?.ShowMessagePopup(
                $"Abilities for {item.m_Name}\n" +
                $"Current: {abilities}\n" +
                $"\nEditing coming soon!", 5);
        }

        private void CloneItem(ItemData item)
        {
            try
            {
                var newItem = _cloneService.CloneItem(item);
                _editorService.AddItem(newItem);

                Manager.GetUIManager()?.ShowMessagePopup(
                    $"Cloned {item.m_Name}\n" +
                    $"New ID: {newItem.m_ID}\n" +
                    $"Remember to save!", 5);
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorUI: CloneItem failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Clone failed: {e.Message}", 5);
            }
        }

        private void DeleteItem(ItemData item)
        {
            // Show confirmation
            Manager.GetUIManager()?.ShowMessagePopup(
                $"Delete {item.m_Name}?\n" +
                $"(Not implemented - for safety)", 3);
        }

        private void CreateNewItem()
        {
            Manager.GetUIManager()?.ShowMessagePopup(
                "Create New Item\n" +
                "Select an item to clone from the item list", 3);
            ShowItemList();
        }

        private void SaveAllItems()
        {
            try
            {
                _editorService.SaveAllItems();
                Manager.GetUIManager()?.ShowMessagePopup(
                    "All items saved to XML!\n" +
                    "Restart game to load changes", 5);
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorUI: SaveAllItems failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Save failed: {e.Message}", 5);
            }
        }

        private void ValidateAllItems()
        {
            try
            {
                var results = _validationService.ValidateAllItems(_editorService.GetAllItems());

                string message = results.IsValid
                    ? $"✓ All items valid!\n{results.Errors.Count} errors\n{results.Warnings.Count} warnings"
                    : $"✗ Validation failed!\n{results.Errors.Count} errors\n{results.Warnings.Count} warnings";

                Manager.GetUIManager()?.ShowMessagePopup(message, 5);
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorUI: ValidateAllItems failed: {e.Message}");
                Manager.GetUIManager()?.ShowMessagePopup($"Validation failed: {e.Message}", 5);
            }
        }

        #endregion
    }
}
