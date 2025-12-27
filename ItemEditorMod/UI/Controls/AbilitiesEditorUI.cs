using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SRMod.DTOs;
using ItemEditorMod.Services;

namespace ItemEditorMod.UI.Controls
{
    /// <summary>
    /// AbilitiesEditorUI - Ability assignment editor
    /// Allows selecting and managing abilities for items
    /// </summary>
    public class AbilitiesEditorUI
    {
        #region Fields

        private ItemEditorService _editorService;
        private Transform _tabContentPanel;

        #endregion

        #region Constructor

        public AbilitiesEditorUI(ItemEditorService editorService)
        {
            _editorService = editorService;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create abilities editor UI
        /// </summary>
        public void CreateUI(Transform tabContentPanel)
        {
            try
            {
                _tabContentPanel = tabContentPanel;
                Debug.Log("AbilitiesEditorUI: Creating abilities editor UI");

                // Get the content scroll area
                var scrollContent = _tabContentPanel.Find("TabContent/Panel_Abilities/ScrollView/Content");
                if (scrollContent == null)
                {
                    Debug.LogError("AbilitiesEditorUI: Cannot find scroll content panel");
                    return;
                }

                // Title
                var titleGO = new GameObject("Title");
                titleGO.transform.SetParent(scrollContent);
                var titleText = titleGO.AddComponent<Text>();
                titleText.text = "Item Abilities";
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
                infoText.text = "(Placeholder: Ability assignment coming in full version)";
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
                addText.text = "+ Add Ability";
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
                    Debug.Log("AbilitiesEditorUI: Add ability clicked (placeholder)");
                });

                Debug.Log("AbilitiesEditorUI: Abilities editor UI created successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"AbilitiesEditorUI: CreateUI failed: {e.Message}");
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

                // TODO: Populate ability list from item.m_AbilityIDs
            }
            catch (Exception e)
            {
                Debug.LogError($"AbilitiesEditorUI: RefreshUI failed: {e.Message}");
            }
        }

        #endregion
    }
}
