using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ItemEditorMod.UI.Controls
{
    /// <summary>
    /// TabManager - Manages tab navigation (Combat, Research, Economic, Modifiers, Abilities)
    /// Handles tab button clicks and content panel visibility
    /// </summary>
    public class TabManager
    {
        #region Fields

        private Dictionary<string, GameObject> _tabPanels;
        private Dictionary<string, Button> _tabButtons;
        private string _activeTab;
        private Transform _tabButtonsContainer;
        private Transform _tabContentContainer;

        #endregion

        #region Properties

        public string ActiveTab
        {
            get { return _activeTab; }
        }

        #endregion

        #region Constructor

        public TabManager()
        {
            _tabPanels = new Dictionary<string, GameObject>();
            _tabButtons = new Dictionary<string, Button>();
            _activeTab = "Combat";
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create tab buttons and content panels
        /// </summary>
        public void CreateTabs(Transform parent)
        {
            try
            {
                Debug.Log("TabManager: Creating tabs");

                // Create tab buttons container
                var tabButtonsGO = new GameObject("TabButtons");
                _tabButtonsContainer = tabButtonsGO.AddComponent<RectTransform>();
                _tabButtonsContainer.SetParent(parent);
                (_tabButtonsContainer as RectTransform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 40);
                (_tabButtonsContainer as RectTransform).SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);

                var hlg = tabButtonsGO.AddComponent<HorizontalLayoutGroup>();
                hlg.spacing = 5;
                hlg.padding = new RectOffset(5, 5, 5, 5);

                // Create content container
                var contentGO = new GameObject("TabContent");
                _tabContentContainer = contentGO.AddComponent<RectTransform>();
                _tabContentContainer.SetParent(parent);
                var contentRect = _tabContentContainer as RectTransform;
                contentRect.offsetMin = new Vector2(0, 40);
                contentRect.offsetMax = Vector2.zero;

                // Create tabs
                string[] tabNames = { "Combat", "Research", "Economic", "Modifiers", "Abilities" };
                foreach (var tabName in tabNames)
                {
                    CreateTab(tabName);
                }

                // Activate first tab
                ShowTab("Combat");
            }
            catch (Exception e)
            {
                Debug.LogError($"TabManager: CreateTabs failed: {e.Message}");
            }
        }

        /// <summary>
        /// Switch to a specific tab
        /// </summary>
        public void ShowTab(string tabName)
        {
            try
            {
                if (!_tabPanels.ContainsKey(tabName))
                {
                    Debug.LogWarning($"TabManager: Tab {tabName} not found");
                    return;
                }

                // Hide all panels
                foreach (var panel in _tabPanels.Values)
                {
                    if (panel != null)
                    {
                        panel.SetActive(false);
                    }
                }

                // Update button states
                foreach (var button in _tabButtons.Values)
                {
                    if (button != null)
                    {
                        var colors = button.colors;
                        colors.normalColor = new Color(0.3f, 0.3f, 0.3f, 1);
                        button.colors = colors;
                    }
                }

                // Show active panel
                if (_tabPanels[tabName] != null)
                {
                    _tabPanels[tabName].SetActive(true);
                }

                // Highlight active button
                if (_tabButtons.ContainsKey(tabName) && _tabButtons[tabName] != null)
                {
                    var colors = _tabButtons[tabName].colors;
                    colors.normalColor = new Color(0.2f, 0.5f, 0.2f, 1);
                    _tabButtons[tabName].colors = colors;
                }

                _activeTab = tabName;
                Debug.Log($"TabManager: Switched to tab {tabName}");
            }
            catch (Exception e)
            {
                Debug.LogError($"TabManager: ShowTab failed: {e.Message}");
            }
        }

        /// <summary>
        /// Get the content panel for a tab (used to add fields)
        /// </summary>
        public Transform GetTabContentPanel(string tabName)
        {
            if (_tabPanels.ContainsKey(tabName))
            {
                return _tabPanels[tabName].transform;
            }
            return null;
        }

        #endregion

        #region Private Methods

        private void CreateTab(string tabName)
        {
            try
            {
                // Create button
                var buttonGO = new GameObject("Tab_" + tabName);
                var buttonRect = buttonGO.AddComponent<RectTransform>();
                buttonRect.SetParent(_tabButtonsContainer);

                var buttonLE = buttonGO.AddComponent<LayoutElement>();
                buttonLE.preferredWidth = 100;
                buttonLE.preferredHeight = 30;

                var buttonImage = buttonGO.AddComponent<Image>();
                buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 1);

                var button = buttonGO.AddComponent<Button>();
                _tabButtons[tabName] = button;

                // Create button text
                var buttonTextGO = new GameObject("Text");
                var buttonTextRect = buttonTextGO.AddComponent<RectTransform>();
                buttonTextRect.SetParent(buttonGO.transform);
                buttonTextRect.offsetMin = Vector2.zero;
                buttonTextRect.offsetMax = Vector2.zero;

                var buttonText = buttonTextGO.AddComponent<Text>();
                buttonText.text = tabName;
                buttonText.font = Resources.Load<Font>("Arial");
                buttonText.fontSize = 14;
                buttonText.alignment = TextAnchor.MiddleCenter;
                buttonText.color = Color.white;

                button.targetGraphic = buttonImage;

                // Add click handler
                button.onClick.AddListener(() =>
                {
                    ShowTab(tabName);
                });

                // Create content panel
                var panelGO = new GameObject("Panel_" + tabName);
                var panelRect = panelGO.AddComponent<RectTransform>();
                panelRect.SetParent(_tabContentContainer);
                panelRect.offsetMin = Vector2.zero;
                panelRect.offsetMax = Vector2.zero;

                var panelImage = panelGO.AddComponent<Image>();
                panelImage.color = new Color(0.15f, 0.15f, 0.15f, 1);

                // Add scroll view for content
                var scrollGO = new GameObject("ScrollView");
                var scrollRect = scrollGO.AddComponent<RectTransform>();
                scrollRect.SetParent(panelGO.transform);
                scrollRect.offsetMin = new Vector2(10, 10);
                scrollRect.offsetMax = new Vector2(-10, -10);

                var scrollView = scrollGO.AddComponent<ScrollRect>();
                scrollView.horizontal = false;
                scrollView.vertical = true;

                // Create scroll content
                var contentGO = new GameObject("Content");
                var contentRect = contentGO.AddComponent<RectTransform>();
                contentRect.SetParent(scrollGO.transform);

                var contentLE = contentGO.AddComponent<LayoutElement>();
                contentLE.preferredWidth = 300;
                contentLE.preferredHeight = 200;

                var vlg = contentGO.AddComponent<VerticalLayoutGroup>();
                vlg.padding = new RectOffset(5, 5, 5, 5);
                vlg.spacing = 5;
                vlg.childForceExpandWidth = true;

                scrollView.content = contentRect;

                _tabPanels[tabName] = panelGO;

                Debug.Log($"TabManager: Created tab {tabName}");
            }
            catch (Exception e)
            {
                Debug.LogError($"TabManager: CreateTab failed for {tabName}: {e.Message}");
            }
        }

        #endregion
    }
}
