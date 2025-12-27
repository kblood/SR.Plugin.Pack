using System;
using UnityEngine;
using UnityEngine.UI;
using ItemEditorMod.Services;
using ItemEditorMod.Models;

namespace ItemEditorMod.UI.Controls
{
    /// <summary>
    /// ToolbarUI - Main action toolbar with Save, Validate, Clone, and Close buttons
    /// Provides quick access to core editor functions
    /// </summary>
    public class ToolbarUI
    {
        #region Fields

        private ItemEditorService _editorService;
        private ValidationService _validationService;
        private ItemCloneService _cloneService;
        private Transform _toolbarParent;

        private Button _saveButton;
        private Button _validateButton;
        private Button _cloneButton;
        private Button _closeButton;

        // Callbacks for external events
        private Action _onCloseClicked;
        private Action<ValidationResult> _onValidationResult;

        #endregion

        #region Constructor

        public ToolbarUI(ItemEditorService editorService, ValidationService validationService,
            ItemCloneService cloneService)
        {
            _editorService = editorService;
            _validationService = validationService;
            _cloneService = cloneService;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Set the callback for when close button is clicked
        /// </summary>
        public void SetOnCloseCallback(Action onCloseClicked)
        {
            _onCloseClicked = onCloseClicked;
        }

        /// <summary>
        /// Set the callback for validation results
        /// </summary>
        public void SetOnValidationCallback(Action<ValidationResult> onValidationResult)
        {
            _onValidationResult = onValidationResult;
        }

        /// <summary>
        /// Create the toolbar UI
        /// </summary>
        public void CreateUI(Transform parent)
        {
            try
            {
                _toolbarParent = parent;
                Debug.Log("ToolbarUI: Creating toolbar");

                // Create toolbar background container
                var toolbarGO = new GameObject("Toolbar");
                var toolbarRect = toolbarGO.AddComponent<RectTransform>();
                toolbarRect.SetParent(parent);
                toolbarRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 45);
                toolbarRect.offsetMin = new Vector2(0, toolbarRect.offsetMin.y);
                toolbarRect.offsetMax = new Vector2(0, toolbarRect.offsetMax.y);

                var toolbarImage = toolbarGO.AddComponent<Image>();
                toolbarImage.color = new Color(0.1f, 0.1f, 0.1f, 1); // Dark background

                // Add horizontal layout
                var hlg = toolbarGO.AddComponent<HorizontalLayoutGroup>();
                hlg.padding = new RectOffset(10, 10, 5, 5);
                hlg.spacing = 10;
                hlg.childForceExpandHeight = true;
                hlg.childForceExpandWidth = false;

                // Save button (green)
                CreateToolbarButton(toolbarGO.transform, "Save", new Color(0.2f, 0.7f, 0.2f, 1),
                    () => OnSaveClicked(), 100);

                // Validate button (blue)
                CreateToolbarButton(toolbarGO.transform, "Validate", new Color(0.2f, 0.5f, 0.8f, 1),
                    () => OnValidateClicked(), 100);

                // Clone button (orange)
                CreateToolbarButton(toolbarGO.transform, "Clone", new Color(0.8f, 0.5f, 0.2f, 1),
                    () => OnCloneClicked(), 100);

                // Spacer (flex space)
                var spacerGO = new GameObject("Spacer");
                spacerGO.transform.SetParent(toolbarGO.transform);
                var spacerLE = spacerGO.AddComponent<LayoutElement>();
                spacerLE.preferredWidth = 1;
                spacerLE.flexibleWidth = 1;

                // Close button (red)
                CreateToolbarButton(toolbarGO.transform, "Close", new Color(0.7f, 0.2f, 0.2f, 1),
                    () => OnCloseClicked(), 100);

                Debug.Log("ToolbarUI: Toolbar created successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"ToolbarUI: CreateUI failed: {e.Message}\n{e.StackTrace}");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Create a single toolbar button with styling
        /// </summary>
        private void CreateToolbarButton(Transform parent, string label, Color buttonColor,
            Action onClickCallback, float preferredWidth)
        {
            try
            {
                var buttonGO = new GameObject(label + "Button");
                buttonGO.transform.SetParent(parent);

                var buttonRect = buttonGO.AddComponent<RectTransform>();
                var buttonLE = buttonGO.AddComponent<LayoutElement>();
                buttonLE.preferredWidth = preferredWidth;
                buttonLE.preferredHeight = 30;

                var buttonImage = buttonGO.AddComponent<Image>();
                buttonImage.color = buttonColor;

                var button = buttonGO.AddComponent<Button>();

                // Button color transition
                var colors = button.colors;
                colors.normalColor = buttonColor;
                colors.highlightedColor = buttonColor * 1.2f; // Brighten on hover
                colors.pressedColor = buttonColor * 0.8f; // Darken on press
                colors.disabledColor = buttonColor * 0.5f; // Dim when disabled
                button.colors = colors;

                // Add text label
                var textGO = new GameObject("Text");
                textGO.transform.SetParent(buttonGO.transform);
                var textRect = textGO.AddComponent<RectTransform>();
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;

                var text = textGO.AddComponent<Text>();
                text.text = label;
                text.font = Resources.Load<Font>("Arial");
                text.fontSize = 12;
                text.fontStyle = FontStyle.Bold;
                text.color = Color.white;
                text.alignment = TextAnchor.MiddleCenter;

                // Add click listener
                button.onClick.AddListener(() => onClickCallback?.Invoke());

                Debug.Log($"ToolbarUI: Created {label} button");
            }
            catch (Exception e)
            {
                Debug.LogError($"ToolbarUI: CreateToolbarButton failed for {label}: {e.Message}");
            }
        }

        /// <summary>
        /// Handle Save button click
        /// </summary>
        private void OnSaveClicked()
        {
            try
            {
                Debug.Log("ToolbarUI: Save button clicked");

                if (_editorService.CurrentItem == null)
                {
                    Debug.LogWarning("ToolbarUI: No item selected to save");
                    return;
                }

                _editorService.SaveCurrentItem();
                _editorService.ExportToXML();

                Debug.Log("ToolbarUI: Item saved and exported to XML");
            }
            catch (Exception e)
            {
                Debug.LogError($"ToolbarUI: Save operation failed: {e.Message}");
            }
        }

        /// <summary>
        /// Handle Validate button click
        /// </summary>
        private void OnValidateClicked()
        {
            try
            {
                Debug.Log("ToolbarUI: Validate button clicked");

                if (_editorService.CurrentItem == null)
                {
                    Debug.LogWarning("ToolbarUI: No item selected to validate");
                    return;
                }

                var validationResult = _validationService.ValidateItem(_editorService.CurrentItem);

                Debug.Log($"ToolbarUI: Validation complete - Valid: {validationResult.IsValid}, " +
                    $"Errors: {validationResult.Errors.Count}, Warnings: {validationResult.Warnings.Count}");

                _onValidationResult?.Invoke(validationResult);
            }
            catch (Exception e)
            {
                Debug.LogError($"ToolbarUI: Validation failed: {e.Message}");
            }
        }

        /// <summary>
        /// Handle Clone button click
        /// </summary>
        private void OnCloneClicked()
        {
            try
            {
                Debug.Log("ToolbarUI: Clone button clicked");

                if (_editorService.CurrentItem == null)
                {
                    Debug.LogWarning("ToolbarUI: No item selected to clone");
                    return;
                }

                int nextItemId = _editorService.GetNextAvailableItemId();
                var clonedItem = _cloneService.CloneItem(_editorService.CurrentItem, nextItemId);

                _editorService.AddItem(clonedItem);
                _editorService.LoadItem(nextItemId);

                Debug.Log($"ToolbarUI: Item cloned with new ID: {nextItemId}");
            }
            catch (Exception e)
            {
                Debug.LogError($"ToolbarUI: Clone operation failed: {e.Message}");
            }
        }

        /// <summary>
        /// Handle Close button click
        /// </summary>
        private void OnCloseClicked()
        {
            try
            {
                Debug.Log("ToolbarUI: Close button clicked");
                _onCloseClicked?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError($"ToolbarUI: Close operation failed: {e.Message}");
            }
        }

        #endregion
    }
}
