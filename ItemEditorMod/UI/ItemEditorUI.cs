using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ItemEditorMod.Services;
using ItemEditorMod.UI.Controls;
using ItemEditorMod.Models;

namespace ItemEditorMod.UI
{
    /// <summary>
    /// ItemEditorUI - Main UI orchestrator for the item editor
    /// Clones InputBoxUi and manages the overall editor interface with tabs
    /// </summary>
    public class ItemEditorUI
    {
        #region Fields

        private ItemEditorService _editorService;
        private ValidationService _validationService;
        private IconManagementService _iconService;
        private ItemCloneService _cloneService;

        private GameObject _rootCanvas;
        private InputBoxUi _clonedUI;
        private bool _isVisible = false;

        // Tab system
        private TabManager _tabManager;
        private CombatTabUI _combatTabUI;
        private ResearchTabUI _researchTabUI;
        private EconomicTabUI _economicTabUI;
        private ModifierEditorUI _modifierEditorUI;
        private AbilitiesEditorUI _abilitiesEditorUI;
        private IconGridUI _iconGridUI;

        // Toolbar and list
        private ToolbarUI _toolbarUI;
        private ItemListUI _itemListUI;

        #endregion

        #region Constructor

        public ItemEditorUI(ItemEditorService editorService, ValidationService validationService,
            IconManagementService iconService, ItemCloneService cloneService)
        {
            _editorService = editorService;
            _validationService = validationService;
            _iconService = iconService;
            _cloneService = cloneService;

            // Create tab UIs
            _tabManager = new TabManager();
            _combatTabUI = new CombatTabUI(editorService);
            _researchTabUI = new ResearchTabUI(editorService);
            _economicTabUI = new EconomicTabUI(editorService);
            _modifierEditorUI = new ModifierEditorUI(editorService);
            _abilitiesEditorUI = new AbilitiesEditorUI(editorService);
            _iconGridUI = new IconGridUI(iconService);

            // Create toolbar and item list
            _toolbarUI = new ToolbarUI(editorService, validationService, cloneService);
            _itemListUI = new ItemListUI(editorService);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Show the editor UI
        /// </summary>
        public void Show()
        {
            try
            {
                Debug.Log("ItemEditorUI: Show() called");

                if (_rootCanvas == null)
                {
                    Debug.Log("ItemEditorUI: _rootCanvas is null, initializing...");
                    InitializeUI();
                }
                else
                {
                    Debug.Log("ItemEditorUI: _rootCanvas already initialized");
                }

                if (_rootCanvas != null)
                {
                    Debug.Log("ItemEditorUI: _rootCanvas is valid, activating...");
                    _rootCanvas.SetActive(true);
                    _isVisible = true;
                    Debug.Log("ItemEditorUI: Editor shown successfully");

                    // Reload items when showing
                    _editorService.ReloadFromGame();
                }
                else
                {
                    Debug.LogError("ItemEditorUI: Show failed - _rootCanvas is still null after initialization");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorUI: Show failed: {e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// Hide the editor UI
        /// </summary>
        public void Hide()
        {
            try
            {
                if (_rootCanvas != null)
                {
                    _rootCanvas.SetActive(false);
                    _isVisible = false;
                    Debug.Log("ItemEditorUI: Editor hidden");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorUI: Hide failed: {e.Message}");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Setup event callbacks for toolbar and list
        /// </summary>
        private void SetupCallbacks()
        {
            try
            {
                // Toolbar close button callback
                _toolbarUI.SetOnCloseCallback(() => Hide());

                // Toolbar validation callback - show validation results modal
                _toolbarUI.SetOnValidationCallback((validationResult) =>
                {
                    ShowValidationModal(validationResult);
                });

                // Item list selection callback - refresh UI when item is selected
                _itemListUI.SetOnItemSelectedCallback((itemId) =>
                {
                    RefreshEditorForCurrentItem();
                });

                Debug.Log("ItemEditorUI: Callbacks setup complete");
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorUI: SetupCallbacks failed: {e.Message}");
            }
        }

        /// <summary>
        /// Refresh all editor tabs with current item data
        /// </summary>
        private void RefreshEditorForCurrentItem()
        {
            try
            {
                if (_editorService.CurrentItem == null)
                    return;

                // Refresh all tab UIs with current item
                _combatTabUI.RefreshUI(_editorService.CurrentItem);
                _researchTabUI.RefreshUI(_editorService.CurrentItem);
                _economicTabUI.RefreshUI(_editorService.CurrentItem);
                _modifierEditorUI.RefreshUI(_editorService.CurrentItem);
                _abilitiesEditorUI.RefreshUI(_editorService.CurrentItem);

                Debug.Log($"ItemEditorUI: Refreshed editor for item {_editorService.CurrentItem.m_ID}");
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorUI: RefreshEditorForCurrentItem failed: {e.Message}");
            }
        }

        /// <summary>
        /// Show validation results in a modal dialog
        /// </summary>
        private void ShowValidationModal(ValidationResult validationResult)
        {
            try
            {
                Debug.Log("ItemEditorUI: Showing validation modal");

                // Create modal background
                var modalBG = new GameObject("ValidationModal");
                var modalRect = modalBG.AddComponent<RectTransform>();
                modalRect.SetParent(_rootCanvas.transform);
                modalRect.offsetMin = Vector2.zero;
                modalRect.offsetMax = Vector2.zero;

                var bgImage = modalBG.AddComponent<Image>();
                bgImage.color = new Color(0, 0, 0, 0.5f); // Semi-transparent black

                // Create modal panel
                var panelGO = new GameObject("Panel");
                var panelRect = panelGO.AddComponent<RectTransform>();
                panelRect.SetParent(modalBG.transform);
                panelRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 100, 600);
                panelRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 100, 400);

                var panelImage = panelGO.AddComponent<Image>();
                panelImage.color = new Color(0.2f, 0.2f, 0.2f, 1);

                var panelVLG = panelGO.AddComponent<VerticalLayoutGroup>();
                panelVLG.padding = new RectOffset(10, 10, 10, 10);
                panelVLG.spacing = 5;
                panelVLG.childForceExpandHeight = true;

                // Title
                var titleGO = new GameObject("Title");
                titleGO.transform.SetParent(panelGO.transform);
                var titleText = titleGO.AddComponent<Text>();
                titleText.text = validationResult.IsValid ? "✓ Validation Passed" : "✗ Validation Failed";
                titleText.font = Resources.Load<Font>("Arial");
                titleText.fontSize = 16;
                titleText.fontStyle = FontStyle.Bold;
                titleText.color = validationResult.IsValid ? new Color(0.2f, 0.8f, 0.2f, 1) : new Color(0.8f, 0.2f, 0.2f, 1);
                titleText.alignment = TextAnchor.MiddleCenter;

                var titleLE = titleGO.AddComponent<LayoutElement>();
                titleLE.preferredHeight = 30;

                // Errors and warnings scroll area
                var scrollGO = new GameObject("ScrollArea");
                scrollGO.transform.SetParent(panelGO.transform);
                var scrollLE = scrollGO.AddComponent<LayoutElement>();
                scrollLE.flexibleHeight = 1;

                var scrollImage = scrollGO.AddComponent<Image>();
                scrollImage.color = new Color(0.15f, 0.15f, 0.15f, 1);

                var scroll = scrollGO.AddComponent<ScrollRect>();
                scroll.horizontal = false;
                scroll.vertical = true;

                var contentGO = new GameObject("Content");
                contentGO.transform.SetParent(scrollGO.transform);
                var contentRect = contentGO.AddComponent<RectTransform>();
                var contentVLG = contentGO.AddComponent<VerticalLayoutGroup>();
                contentVLG.padding = new RectOffset(5, 5, 5, 5);
                contentVLG.spacing = 3;
                contentVLG.childForceExpandWidth = true;

                scroll.content = contentRect;

                // Display errors
                if (validationResult.Errors.Count > 0)
                {
                    var errorHeaderGO = new GameObject("ErrorHeader");
                    errorHeaderGO.transform.SetParent(contentGO.transform);
                    var errorHeaderText = errorHeaderGO.AddComponent<Text>();
                    errorHeaderText.text = "ERRORS:";
                    errorHeaderText.font = Resources.Load<Font>("Arial");
                    errorHeaderText.fontSize = 12;
                    errorHeaderText.fontStyle = FontStyle.Bold;
                    errorHeaderText.color = new Color(0.8f, 0.2f, 0.2f, 1);

                    foreach (var error in validationResult.Errors)
                    {
                        var errorGO = new GameObject("Error");
                        errorGO.transform.SetParent(contentGO.transform);
                        var errorText = errorGO.AddComponent<Text>();
                        errorText.text = "• " + error;
                        errorText.font = Resources.Load<Font>("Arial");
                        errorText.fontSize = 11;
                        errorText.color = new Color(1, 0.5f, 0.5f, 1);
                        errorText.horizontalOverflow = HorizontalWrapMode.Wrap;

                        var errorLE = errorGO.AddComponent<LayoutElement>();
                        errorLE.preferredHeight = 25;
                    }
                }

                // Display warnings
                if (validationResult.Warnings.Count > 0)
                {
                    var warningHeaderGO = new GameObject("WarningHeader");
                    warningHeaderGO.transform.SetParent(contentGO.transform);
                    var warningHeaderText = warningHeaderGO.AddComponent<Text>();
                    warningHeaderText.text = "WARNINGS:";
                    warningHeaderText.font = Resources.Load<Font>("Arial");
                    warningHeaderText.fontSize = 12;
                    warningHeaderText.fontStyle = FontStyle.Bold;
                    warningHeaderText.color = new Color(0.8f, 0.8f, 0.2f, 1);

                    foreach (var warning in validationResult.Warnings)
                    {
                        var warningGO = new GameObject("Warning");
                        warningGO.transform.SetParent(contentGO.transform);
                        var warningText = warningGO.AddComponent<Text>();
                        warningText.text = "⚠ " + warning;
                        warningText.font = Resources.Load<Font>("Arial");
                        warningText.fontSize = 11;
                        warningText.color = new Color(1, 1, 0.5f, 1);
                        warningText.horizontalOverflow = HorizontalWrapMode.Wrap;

                        var warningLE = warningGO.AddComponent<LayoutElement>();
                        warningLE.preferredHeight = 25;
                    }
                }

                // Close button
                var buttonGO = new GameObject("CloseButton");
                buttonGO.transform.SetParent(panelGO.transform);
                var buttonLE = buttonGO.AddComponent<LayoutElement>();
                buttonLE.preferredHeight = 35;

                var buttonImage = buttonGO.AddComponent<Image>();
                buttonImage.color = new Color(0.3f, 0.3f, 0.3f, 1);

                var button = buttonGO.AddComponent<Button>();
                var buttonTextGO = new GameObject("Text");
                buttonTextGO.transform.SetParent(buttonGO.transform);
                var buttonText = buttonTextGO.AddComponent<Text>();
                buttonText.text = "Close";
                buttonText.font = Resources.Load<Font>("Arial");
                buttonText.fontSize = 12;
                buttonText.fontStyle = FontStyle.Bold;
                buttonText.color = Color.white;
                buttonText.alignment = TextAnchor.MiddleCenter;

                var buttonTextRect = buttonTextGO.GetComponent<RectTransform>();
                buttonTextRect.offsetMin = Vector2.zero;
                buttonTextRect.offsetMax = Vector2.zero;

                button.onClick.AddListener(() => UnityEngine.Object.Destroy(modalBG));

                Debug.Log("ItemEditorUI: Validation modal shown");
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorUI: ShowValidationModal failed: {e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// Initialize the UI by cloning InputBoxUi and building the editor layout
        /// </summary>
        private void InitializeUI()
        {
            try
            {
                Debug.Log("ItemEditorUI: Initializing UI");

                // Clone the game's InputBoxUi as base
                var gameInputBoxUi = Manager.GetUIManager().m_InputBoxUi;
                if (gameInputBoxUi == null)
                {
                    Debug.LogError("ItemEditorUI: Cannot find game's InputBoxUi");
                    return;
                }

                _clonedUI = UnityEngine.Object.Instantiate(gameInputBoxUi);
                _clonedUI.gameObject.name = "ItemEditorUI";

                // Get or create root canvas
                _rootCanvas = _clonedUI.gameObject;

                // Disable game's input controls and unwanted buttons
                if (_clonedUI.m_InputControlContainer != null)
                {
                    _clonedUI.m_InputControlContainer.gameObject.SetActive(false);
                }

                if (_clonedUI.m_OkButtonContainer != null)
                {
                    _clonedUI.m_OkButtonContainer.gameObject.SetActive(false);
                }

                // Get the main content area to rebuild it
                Transform mainContentArea = _clonedUI.transform.Find("Content");
                if (mainContentArea == null)
                {
                    Debug.LogError("ItemEditorUI: Cannot find Content area in cloned UI");
                    return;
                }

                // Clear existing content
                foreach (Transform child in mainContentArea)
                {
                    UnityEngine.Object.Destroy(child.gameObject);
                }

                // Create toolbar first
                _toolbarUI.CreateUI(mainContentArea);

                // Create editor layout (toolbar is pinned to top, so order doesn't matter)
                BuildEditorLayout(mainContentArea);

                // Setup callbacks
                SetupCallbacks();

                Debug.Log("ItemEditorUI: UI initialized successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorUI: InitializeUI failed: {e.Message}\n{e.StackTrace}");
            }
        }

        /// <summary>
        /// Build the main editor layout with tabs
        /// </summary>
        private void BuildEditorLayout(Transform parent)
        {
            try
            {
                Debug.Log("ItemEditorUI: Building editor layout");

                // Create main horizontal layout (item list on left, editor on right)
                var mainLayout = new GameObject("MainLayout");
                var mainLayoutRect = mainLayout.AddComponent<RectTransform>();
                mainLayoutRect.SetParent(parent);
                // Adjust for toolbar height (45px at top)
                mainLayoutRect.offsetMin = new Vector2(0, 0);
                mainLayoutRect.offsetMax = new Vector2(0, -45);
                mainLayoutRect.anchoredPosition = new Vector2(0, -22.5f);

                var hlg = mainLayout.AddComponent<HorizontalLayoutGroup>();
                hlg.spacing = 10;
                hlg.padding = new RectOffset(10, 10, 10, 10);
                hlg.childForceExpandWidth = true;
                hlg.childForceExpandHeight = true;

                // Left panel: Item list (30% width)
                var leftPanel = new GameObject("ItemListPanel");
                var leftPanelRect = leftPanel.AddComponent<RectTransform>();
                leftPanelRect.SetParent(mainLayout.transform);
                leftPanel.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f, 1);
                var leftLE = leftPanel.AddComponent<LayoutElement>();
                leftLE.preferredWidth = 200;

                var leftVLG = leftPanel.AddComponent<VerticalLayoutGroup>();
                leftVLG.padding = new RectOffset(5, 5, 5, 5);
                leftVLG.spacing = 5;
                leftVLG.childForceExpandHeight = true;

                // Create item list UI
                _itemListUI.CreateUI(leftPanel.transform);

                // Right panel: Editor (70% width)
                var rightPanel = new GameObject("EditorPanel");
                var rightPanelRect = rightPanel.AddComponent<RectTransform>();
                rightPanelRect.SetParent(mainLayout.transform);
                rightPanel.AddComponent<Image>().color = new Color(0.15f, 0.15f, 0.15f, 1);

                var rightVLG = rightPanel.AddComponent<VerticalLayoutGroup>();
                rightVLG.padding = new RectOffset(5, 5, 5, 5);
                rightVLG.spacing = 5;
                rightVLG.childForceExpandHeight = true;

                // Create tabs within right panel
                _tabManager.CreateTabs(rightPanel.transform);

                // Populate tabs with fields
                var combatPanel = _tabManager.GetTabContentPanel("Combat");
                if (combatPanel != null)
                {
                    _combatTabUI.CreateUI(rightPanel.transform);
                }

                var researchPanel = _tabManager.GetTabContentPanel("Research");
                if (researchPanel != null)
                {
                    _researchTabUI.CreateUI(rightPanel.transform);
                }

                var economicPanel = _tabManager.GetTabContentPanel("Economic");
                if (economicPanel != null)
                {
                    _economicTabUI.CreateUI(rightPanel.transform);
                }

                var modifiersPanel = _tabManager.GetTabContentPanel("Modifiers");
                if (modifiersPanel != null)
                {
                    _modifierEditorUI.CreateUI(rightPanel.transform);
                }

                var abilitiesPanel = _tabManager.GetTabContentPanel("Abilities");
                if (abilitiesPanel != null)
                {
                    _abilitiesEditorUI.CreateUI(rightPanel.transform);
                }

                Debug.Log("ItemEditorUI: Editor layout built successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemEditorUI: BuildEditorLayout failed: {e.Message}\n{e.StackTrace}");
            }
        }

        #endregion
    }
}
