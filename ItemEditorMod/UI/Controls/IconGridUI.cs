using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ItemEditorMod.Services;

namespace ItemEditorMod.UI.Controls
{
    /// <summary>
    /// IconGridUI - Visual icon picker with 8x8 grid and search
    /// Displays all available icons and allows selection
    /// </summary>
    public class IconGridUI
    {
        #region Fields

        private List<string> _availableIcons;
        private List<string> _filteredIcons;
        private IconManagementService _iconService;
        private string _selectedIcon;
        private GameObject _gridPanel;
        private InputField _searchField;
        private Action<string> _onIconSelected;

        #endregion

        #region Constructor

        public IconGridUI(IconManagementService iconService)
        {
            _iconService = iconService;
            _availableIcons = new List<string>();
            _filteredIcons = new List<string>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Show icon picker modal
        /// </summary>
        public void Show(Action<string> onIconSelected, string currentIcon = "")
        {
            try
            {
                _onIconSelected = onIconSelected;
                _selectedIcon = currentIcon;

                // Load available icons
                _availableIcons = _iconService.GetAvailableIconNames();
                _filteredIcons = new List<string>(_availableIcons);

                if (_gridPanel == null)
                {
                    CreateIconGrid();
                }

                _gridPanel.SetActive(true);
                Debug.Log("IconGridUI: Icon picker shown");
            }
            catch (Exception e)
            {
                Debug.LogError($"IconGridUI: Show failed: {e.Message}");
            }
        }

        /// <summary>
        /// Hide icon picker
        /// </summary>
        public void Hide()
        {
            try
            {
                if (_gridPanel != null)
                {
                    _gridPanel.SetActive(false);
                    Debug.Log("IconGridUI: Icon picker hidden");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"IconGridUI: Hide failed: {e.Message}");
            }
        }

        /// <summary>
        /// Filter icons by search term
        /// </summary>
        public void FilterIcons(string searchTerm)
        {
            try
            {
                _filteredIcons.Clear();
                string searchLower = searchTerm.ToLower();

                foreach (var icon in _availableIcons)
                {
                    if (icon.ToLower().Contains(searchLower))
                    {
                        _filteredIcons.Add(icon);
                    }
                }

                RefreshGrid();
                Debug.Log($"IconGridUI: Filtered to {_filteredIcons.Count} icons");
            }
            catch (Exception e)
            {
                Debug.LogError($"IconGridUI: FilterIcons failed: {e.Message}");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create the icon grid UI
        /// </summary>
        private void CreateIconGrid()
        {
            try
            {
                Debug.Log("IconGridUI: Creating icon grid");

                // Create modal background
                var modalBG = new GameObject("IconPickerModal");
                var modalRect = modalBG.AddComponent<RectTransform>();
                modalRect.SetParent(Manager.GetUIManager().m_InputBoxUi.transform);
                modalRect.offsetMin = Vector2.zero;
                modalRect.offsetMax = Vector2.zero;

                var bgImage = modalBG.AddComponent<Image>();
                bgImage.color = new Color(0, 0, 0, 0.5f); // Semi-transparent black

                // Create main panel
                _gridPanel = new GameObject("GridPanel");
                var panelRect = _gridPanel.AddComponent<RectTransform>();
                panelRect.SetParent(modalBG.transform);
                panelRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 50, 700);
                panelRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 50, 500);

                var panelImage = _gridPanel.AddComponent<Image>();
                panelImage.color = new Color(0.2f, 0.2f, 0.2f, 1);

                // Add vertical layout
                var vlg = _gridPanel.AddComponent<VerticalLayoutGroup>();
                vlg.padding = new RectOffset(10, 10, 10, 10);
                vlg.spacing = 5;
                vlg.childForceExpandHeight = true;
                vlg.childForceExpandWidth = true;

                // Title
                var titleGO = new GameObject("Title");
                titleGO.transform.SetParent(_gridPanel.transform);
                var titleText = titleGO.AddComponent<Text>();
                titleText.text = "Select Icon";
                titleText.font = Resources.Load<Font>("Arial");
                titleText.fontSize = 16;
                titleText.fontStyle = FontStyle.Bold;
                titleText.color = Color.white;
                titleText.alignment = TextAnchor.MiddleCenter;
                var titleLE = titleGO.AddComponent<LayoutElement>();
                titleLE.preferredHeight = 30;

                // Search field
                var searchGO = new GameObject("SearchField");
                searchGO.transform.SetParent(_gridPanel.transform);
                var searchRect = searchGO.AddComponent<RectTransform>();
                var searchLE = searchGO.AddComponent<LayoutElement>();
                searchLE.preferredHeight = 30;

                var searchImage = searchGO.AddComponent<Image>();
                searchImage.color = new Color(0.15f, 0.15f, 0.15f, 1);

                _searchField = searchGO.AddComponent<InputField>();

                var searchTextGO = new GameObject("Text");
                searchTextGO.transform.SetParent(searchGO.transform);
                var searchTextRect = searchTextGO.AddComponent<RectTransform>();
                searchTextRect.offsetMin = new Vector2(5, 0);
                searchTextRect.offsetMax = new Vector2(-5, 0);

                var searchText = searchTextGO.AddComponent<Text>();
                searchText.text = "";
                searchText.font = Resources.Load<Font>("Arial");
                searchText.color = Color.white;

                _searchField.textComponent = searchText;
                _searchField.onValueChange.AddListener(FilterIcons);

                // Grid container (ScrollRect)
                var scrollGO = new GameObject("GridScroll");
                scrollGO.transform.SetParent(_gridPanel.transform);
                var scrollRect = scrollGO.AddComponent<RectTransform>();
                scrollRect.offsetMin = Vector2.zero;
                scrollRect.offsetMax = Vector2.zero;

                var scroll = scrollGO.AddComponent<ScrollRect>();
                scroll.horizontal = false;
                scroll.vertical = true;

                var scrollImage = scrollGO.AddComponent<Image>();
                scrollImage.color = new Color(0.1f, 0.1f, 0.1f, 1);

                // Grid content
                var gridGO = new GameObject("Grid");
                var gridRect = gridGO.AddComponent<RectTransform>();
                gridRect.SetParent(scrollGO.transform);

                var glg = gridGO.AddComponent<GridLayoutGroup>();
                glg.cellSize = new Vector2(64, 64);
                glg.spacing = new Vector2(5, 5);
                glg.padding = new RectOffset(5, 5, 5, 5);
                glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
                glg.constraintCount = 8; // 8 columns

                scroll.content = gridRect;

                // Create icon buttons
                RefreshGrid();

                // Button panel
                var buttonPanelGO = new GameObject("ButtonPanel");
                buttonPanelGO.transform.SetParent(_gridPanel.transform);
                var buttonPanelRect = buttonPanelGO.AddComponent<RectTransform>();
                var buttonLE = buttonPanelGO.AddComponent<LayoutElement>();
                buttonLE.preferredHeight = 40;

                var buttonHLG = buttonPanelGO.AddComponent<HorizontalLayoutGroup>();
                buttonHLG.spacing = 10;
                buttonHLG.childForceExpandHeight = true;
                buttonHLG.childForceExpandWidth = true;

                // OK button
                var okGO = new GameObject("OkButton");
                okGO.transform.SetParent(buttonPanelGO.transform);
                var okImage = okGO.AddComponent<Image>();
                okImage.color = new Color(0.2f, 0.5f, 0.2f, 1);

                var okButton = okGO.AddComponent<Button>();
                var okTextGO = new GameObject("Text");
                okTextGO.transform.SetParent(okGO.transform);
                var okText = okTextGO.AddComponent<Text>();
                okText.text = "OK";
                okText.font = Resources.Load<Font>("Arial");
                okText.alignment = TextAnchor.MiddleCenter;
                okText.color = Color.white;
                var okRect = okTextGO.GetComponent<RectTransform>();
                okRect.offsetMin = Vector2.zero;
                okRect.offsetMax = Vector2.zero;

                okButton.onClick.AddListener(() =>
                {
                    _onIconSelected?.Invoke(_selectedIcon);
                    Hide();
                });

                // Cancel button
                var cancelGO = new GameObject("CancelButton");
                cancelGO.transform.SetParent(buttonPanelGO.transform);
                var cancelImage = cancelGO.AddComponent<Image>();
                cancelImage.color = new Color(0.5f, 0.2f, 0.2f, 1);

                var cancelButton = cancelGO.AddComponent<Button>();
                var cancelTextGO = new GameObject("Text");
                cancelTextGO.transform.SetParent(cancelGO.transform);
                var cancelText = cancelTextGO.AddComponent<Text>();
                cancelText.text = "Cancel";
                cancelText.font = Resources.Load<Font>("Arial");
                cancelText.alignment = TextAnchor.MiddleCenter;
                cancelText.color = Color.white;
                var cancelRect = cancelTextGO.GetComponent<RectTransform>();
                cancelRect.offsetMin = Vector2.zero;
                cancelRect.offsetMax = Vector2.zero;

                cancelButton.onClick.AddListener(Hide);

                _gridPanel.SetActive(false);
                Debug.Log("IconGridUI: Icon grid created successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"IconGridUI: CreateIconGrid failed: {e.Message}");
            }
        }

        /// <summary>
        /// Refresh the icon grid with current filtered icons
        /// </summary>
        private void RefreshGrid()
        {
            try
            {
                var scrollView = _gridPanel.transform.Find("GridScroll");
                if (scrollView == null)
                    return;

                var grid = scrollView.Find("Grid");
                if (grid == null)
                    return;

                // Clear existing buttons
                foreach (Transform child in grid)
                {
                    UnityEngine.Object.Destroy(child.gameObject);
                }

                // Create icon buttons for filtered icons
                foreach (var iconName in _filteredIcons)
                {
                    CreateIconButton(grid, iconName);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"IconGridUI: RefreshGrid failed: {e.Message}");
            }
        }

        /// <summary>
        /// Create a single icon button
        /// </summary>
        private void CreateIconButton(Transform gridParent, string iconName)
        {
            try
            {
                var buttonGO = new GameObject("Icon_" + iconName);
                buttonGO.transform.SetParent(gridParent);

                var button = buttonGO.AddComponent<Button>();
                var image = buttonGO.AddComponent<Image>();

                // Load icon texture
                var texture = _iconService.GetIconTexture(iconName);
                if (texture != null)
                {
                    var sprite = Sprite.Create(texture,
                        new Rect(0, 0, texture.width, texture.height),
                        new Vector2(0.5f, 0.5f), 100.0f);
                    image.sprite = sprite;
                }
                else
                {
                    image.color = new Color(0.3f, 0.3f, 0.3f, 1);
                }

                // Default color
                var colors = button.colors;
                colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 1);
                colors.highlightedColor = new Color(0.5f, 0.5f, 0.5f, 1);
                colors.pressedColor = new Color(0.2f, 0.8f, 0.2f, 1);
                button.colors = colors;

                // Add click handler
                string capturedIcon = iconName;
                button.onClick.AddListener(() =>
                {
                    _selectedIcon = capturedIcon;
                    Debug.Log($"IconGridUI: Selected icon '{capturedIcon}'");
                });

                // Highlight current icon
                if (iconName == _selectedIcon)
                {
                    button.OnSelect(null);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"IconGridUI: CreateIconButton failed for {iconName}: {e.Message}");
            }
        }

        #endregion
    }
}
