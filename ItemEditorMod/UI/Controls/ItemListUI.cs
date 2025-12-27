using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SRMod.DTOs;
using ItemEditorMod.Services;

namespace ItemEditorMod.UI.Controls
{
    /// <summary>
    /// ItemListUI - Left panel with searchable item list
    /// Allows browsing and selecting items for editing
    /// </summary>
    public class ItemListUI
    {
        #region Fields

        private ItemEditorService _editorService;
        private Transform _listParent;
        private InputField _searchField;
        private Transform _itemButtonContainer;
        private List<Button> _itemButtons;
        private List<SerializableItemData> _filteredItems;

        // Callbacks
        private Action<int> _onItemSelected;

        #endregion

        #region Constructor

        public ItemListUI(ItemEditorService editorService)
        {
            _editorService = editorService;
            _itemButtons = new List<Button>();
            _filteredItems = new List<SerializableItemData>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the callback for when an item is selected
        /// </summary>
        public void SetOnItemSelectedCallback(Action<int> onItemSelected)
        {
            _onItemSelected = onItemSelected;
        }

        /// <summary>
        /// Create the item list UI
        /// </summary>
        public void CreateUI(Transform parent)
        {
            try
            {
                _listParent = parent;
                Debug.Log("ItemListUI: Creating item list UI");

                // Title
                var titleGO = new GameObject("Title");
                titleGO.transform.SetParent(parent);
                var titleText = titleGO.AddComponent<Text>();
                titleText.text = "Items";
                titleText.font = Resources.Load<Font>("Arial");
                titleText.fontSize = 14;
                titleText.fontStyle = FontStyle.Bold;
                titleText.color = Color.white;
                titleText.alignment = TextAnchor.MiddleLeft;

                var titleLE = titleGO.AddComponent<LayoutElement>();
                titleLE.preferredHeight = 25;

                // Search field container
                var searchContainerGO = new GameObject("SearchContainer");
                searchContainerGO.transform.SetParent(parent);
                var searchContainerLE = searchContainerGO.AddComponent<LayoutElement>();
                searchContainerLE.preferredHeight = 30;

                var searchContainerRect = searchContainerGO.AddComponent<RectTransform>();
                var searchContainerImage = searchContainerGO.AddComponent<Image>();
                searchContainerImage.color = new Color(0.15f, 0.15f, 0.15f, 1);

                var searchContainerHLG = searchContainerGO.AddComponent<HorizontalLayoutGroup>();
                searchContainerHLG.padding = new RectOffset(3, 3, 3, 3);
                searchContainerHLG.childForceExpandHeight = true;

                // Search field
                var searchGO = new GameObject("SearchField");
                searchGO.transform.SetParent(searchContainerGO.transform);
                var searchRect = searchGO.AddComponent<RectTransform>();
                var searchImage = searchGO.AddComponent<Image>();
                searchImage.color = new Color(0.2f, 0.2f, 0.2f, 1);

                _searchField = searchGO.AddComponent<InputField>();
                _searchField.contentType = InputField.ContentType.Standard;

                var searchTextGO = new GameObject("Text");
                searchTextGO.transform.SetParent(searchGO.transform);
                var searchTextRect = searchTextGO.AddComponent<RectTransform>();
                searchTextRect.offsetMin = new Vector2(5, 0);
                searchTextRect.offsetMax = new Vector2(-5, 0);

                var searchText = searchTextGO.AddComponent<Text>();
                searchText.text = "";
                searchText.font = Resources.Load<Font>("Arial");
                searchText.color = Color.white;
                searchText.fontSize = 12;

                _searchField.textComponent = searchText;
                _searchField.onValueChange.AddListener(OnSearchChanged);

                // Clear button
                var clearButtonGO = new GameObject("ClearButton");
                clearButtonGO.transform.SetParent(searchContainerGO.transform);
                var clearButtonImage = clearButtonGO.AddComponent<Image>();
                clearButtonImage.color = new Color(0.3f, 0.3f, 0.3f, 1);

                var clearButton = clearButtonGO.AddComponent<Button>();
                var clearButtonLE = clearButtonGO.AddComponent<LayoutElement>();
                clearButtonLE.preferredWidth = 30;

                var clearButtonTextGO = new GameObject("Text");
                clearButtonTextGO.transform.SetParent(clearButtonGO.transform);
                var clearButtonText = clearButtonTextGO.AddComponent<Text>();
                clearButtonText.text = "X";
                clearButtonText.font = Resources.Load<Font>("Arial");
                clearButtonText.color = Color.white;
                clearButtonText.fontSize = 14;
                clearButtonText.fontStyle = FontStyle.Bold;
                clearButtonText.alignment = TextAnchor.MiddleCenter;

                var clearButtonTextRect = clearButtonTextGO.GetComponent<RectTransform>();
                clearButtonTextRect.offsetMin = Vector2.zero;
                clearButtonTextRect.offsetMax = Vector2.zero;

                clearButton.onClick.AddListener(() =>
                {
                    _searchField.text = "";
                });

                // Scroll view for items
                var scrollGO = new GameObject("ItemScroll");
                scrollGO.transform.SetParent(parent);
                var scrollRect = scrollGO.AddComponent<RectTransform>();
                var scrollLE = scrollGO.AddComponent<LayoutElement>();
                scrollLE.flexibleHeight = 1;

                var scrollImage = scrollGO.AddComponent<Image>();
                scrollImage.color = new Color(0.1f, 0.1f, 0.1f, 1);

                var scroll = scrollGO.AddComponent<ScrollRect>();
                scroll.horizontal = false;
                scroll.vertical = true;

                // Item button container
                var containerGO = new GameObject("Container");
                containerGO.transform.SetParent(scrollGO.transform);
                _itemButtonContainer = containerGO.transform;

                var containerRect = containerGO.AddComponent<RectTransform>();
                var containerVLG = containerGO.AddComponent<VerticalLayoutGroup>();
                containerVLG.padding = new RectOffset(3, 3, 3, 3);
                containerVLG.spacing = 2;
                containerVLG.childForceExpandWidth = true;

                var containerLE = containerGO.AddComponent<LayoutElement>();
                containerLE.preferredWidth = 200;

                scroll.content = containerRect;

                // Populate item list
                RefreshItemList();

                Debug.Log("ItemListUI: Item list UI created successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemListUI: CreateUI failed: {e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// Refresh the item list from the editor service
        /// </summary>
        public void RefreshItemList()
        {
            try
            {
                _filteredItems.Clear();
                _filteredItems.AddRange(_editorService.AllItems);
                RefreshItemButtons();
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemListUI: RefreshItemList failed: {e.Message}");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handle search field value changed
        /// </summary>
        private void OnSearchChanged(string searchTerm)
        {
            try
            {
                _filteredItems.Clear();

                if (string.IsNullOrEmpty(searchTerm))
                {
                    _filteredItems.AddRange(_editorService.AllItems);
                }
                else
                {
                    string searchLower = searchTerm.ToLower();
                    foreach (var item in _editorService.AllItems)
                    {
                        if (item.m_FriendlyName.ToLower().Contains(searchLower) ||
                            item.m_ID.ToString().Contains(searchTerm))
                        {
                            _filteredItems.Add(item);
                        }
                    }
                }

                RefreshItemButtons();
                Debug.Log($"ItemListUI: Filtered to {_filteredItems.Count} items");
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemListUI: OnSearchChanged failed: {e.Message}");
            }
        }

        /// <summary>
        /// Refresh item button list display
        /// </summary>
        private void RefreshItemButtons()
        {
            try
            {
                // Clear existing buttons
                foreach (Transform child in _itemButtonContainer)
                {
                    UnityEngine.Object.Destroy(child.gameObject);
                }
                _itemButtons.Clear();

                // Create buttons for filtered items
                foreach (var item in _filteredItems)
                {
                    CreateItemButton(item);
                }

                Debug.Log($"ItemListUI: Refreshed {_itemButtons.Count} item buttons");
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemListUI: RefreshItemButtons failed: {e.Message}");
            }
        }

        /// <summary>
        /// Create a single item button
        /// </summary>
        private void CreateItemButton(SerializableItemData item)
        {
            try
            {
                var buttonGO = new GameObject($"Item_{item.m_ID}");
                buttonGO.transform.SetParent(_itemButtonContainer);

                var buttonRect = buttonGO.AddComponent<RectTransform>();
                var buttonLE = buttonGO.AddComponent<LayoutElement>();
                buttonLE.preferredHeight = 30;

                var buttonImage = buttonGO.AddComponent<Image>();
                buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 1);

                var button = buttonGO.AddComponent<Button>();

                // Button color states
                var colors = button.colors;
                colors.normalColor = new Color(0.2f, 0.2f, 0.2f, 1);
                colors.highlightedColor = new Color(0.3f, 0.3f, 0.3f, 1);
                colors.pressedColor = new Color(0.15f, 0.15f, 0.15f, 1);
                button.colors = colors;

                // Add text label
                var textGO = new GameObject("Text");
                textGO.transform.SetParent(buttonGO.transform);
                var textRect = textGO.AddComponent<RectTransform>();
                textRect.offsetMin = new Vector2(5, 0);
                textRect.offsetMax = new Vector2(-5, 0);

                var text = textGO.AddComponent<Text>();
                text.text = $"{item.m_ID}: {item.m_FriendlyName}";
                text.font = Resources.Load<Font>("Arial");
                text.color = Color.white;
                text.fontSize = 11;
                text.alignment = TextAnchor.MiddleLeft;
                text.horizontalOverflow = HorizontalWrapMode.Overflow;

                // Add click listener
                int itemId = item.m_ID;
                button.onClick.AddListener(() => OnItemButtonClicked(itemId));

                _itemButtons.Add(button);

                Debug.Log($"ItemListUI: Created button for item {item.m_ID}: {item.m_FriendlyName}");
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemListUI: CreateItemButton failed for {item.m_ID}: {e.Message}");
            }
        }

        /// <summary>
        /// Handle item button click
        /// </summary>
        private void OnItemButtonClicked(int itemId)
        {
            try
            {
                Debug.Log($"ItemListUI: Item button clicked for ID {itemId}");
                _editorService.LoadItem(itemId);
                _onItemSelected?.Invoke(itemId);
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemListUI: OnItemButtonClicked failed: {e.Message}");
            }
        }

        #endregion
    }
}
