using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SRMod.DTOs;
using ItemEditorMod.Services;
using ItemEditorMod.UI.Builders;

namespace ItemEditorMod.UI.Controls
{
    /// <summary>
    /// ModifierEditorUI - Modifier list editor (add/remove/edit)
    /// Allows adding, removing, and editing item modifiers
    /// </summary>
    public class ModifierEditorUI
    {
        #region Fields

        private ItemEditorService _editorService;
        private Transform _tabContentPanel;
        private List<GameObject> _modifierRows;

        #endregion

        #region Constructor

        public ModifierEditorUI(ItemEditorService editorService)
        {
            _editorService = editorService;
            _modifierRows = new List<GameObject>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create modifier editor UI
        /// </summary>
        public void CreateUI(Transform tabContentPanel)
        {
            try
            {
                _tabContentPanel = tabContentPanel;
                Debug.Log("ModifierEditorUI: Creating modifier editor UI");

                // Get the content scroll area
                var scrollContent = _tabContentPanel.Find("TabContent/Panel_Modifiers/ScrollView/Content");
                if (scrollContent == null)
                {
                    Debug.LogError("ModifierEditorUI: Cannot find scroll content panel");
                    return;
                }

                // Title
                var titleGO = new GameObject("Title");
                titleGO.transform.SetParent(scrollContent);
                var titleText = titleGO.AddComponent<Text>();
                titleText.text = "Item Modifiers";
                titleText.font = Resources.Load<Font>("Arial");
                titleText.fontSize = 14;
                titleText.fontStyle = FontStyle.Bold;
                titleText.color = Color.white;

                var titleLE = titleGO.AddComponent<LayoutElement>();
                titleLE.preferredHeight = 30;

                // Info text
                var infoGO = new GameObject("Info");
                infoGO.transform.SetParent(scrollContent);
                var infoText = infoGO.AddComponent<Text>();
                infoText.text = "(Placeholder: Modifier editing coming in full version)";
                infoText.font = Resources.Load<Font>("Arial");
                infoText.fontSize = 12;
                infoText.color = new Color(0.7f, 0.7f, 0.7f, 1);

                var infoLE = infoGO.AddComponent<LayoutElement>();
                infoLE.preferredHeight = 30;

                // Add button
                var addButtonGO = new GameObject("AddButton");
                addButtonGO.transform.SetParent(scrollContent);
                var addButtonImage = addButtonGO.AddComponent<Image>();
                addButtonImage.color = new Color(0.2f, 0.5f, 0.2f, 1);

                var addButton = addButtonGO.AddComponent<Button>();
                var addButtonText = new GameObject("Text");
                addButtonText.transform.SetParent(addButtonGO.transform);
                var addText = addButtonText.AddComponent<Text>();
                addText.text = "+ Add Modifier";
                addText.font = Resources.Load<Font>("Arial");
                addText.color = Color.white;
                addText.alignment = TextAnchor.MiddleCenter;

                var addRect = addButtonText.GetComponent<RectTransform>();
                addRect.offsetMin = Vector2.zero;
                addRect.offsetMax = Vector2.zero;

                var addBLE = addButtonGO.AddComponent<LayoutElement>();
                addBLE.preferredHeight = 30;

                addButton.onClick.AddListener(() =>
                {
                    Debug.Log("ModifierEditorUI: Add modifier clicked (placeholder)");
                });

                Debug.Log("ModifierEditorUI: Modifier editor UI created successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"ModifierEditorUI: CreateUI failed: {e.Message}");
            }
        }

        /// <summary>
        /// Refresh UI with current item data
        /// </summary>
        public void RefreshUI(SerializableItemData item)
        {
            try
            {
                if (item == null)
                    return;

                // TODO: Populate modifier list from item.m_Modifiers
            }
            catch (Exception e)
            {
                Debug.LogError($"ModifierEditorUI: RefreshUI failed: {e.Message}");
            }
        }

        #endregion
    }
}
