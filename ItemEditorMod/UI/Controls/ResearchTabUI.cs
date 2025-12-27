using System;
using UnityEngine;
using UnityEngine.UI;
using SRMod.DTOs;
using ItemEditorMod.Services;
using ItemEditorMod.UI.Builders;

namespace ItemEditorMod.UI.Controls
{
    /// <summary>
    /// ResearchTabUI - Research tab with progression, costs, and research checkboxes
    /// </summary>
    public class ResearchTabUI
    {
        #region Fields

        private ItemEditorService _editorService;
        private Transform _tabContentPanel;

        #endregion

        #region Constructor

        public ResearchTabUI(ItemEditorService editorService)
        {
            _editorService = editorService;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create research tab UI
        /// </summary>
        public void CreateUI(Transform tabContentPanel)
        {
            try
            {
                _tabContentPanel = tabContentPanel;
                Debug.Log("ResearchTabUI: Creating research tab UI");

                // Get the content scroll area
                var scrollContent = _tabContentPanel.Find("TabContent/Panel_Research/ScrollView/Content");
                if (scrollContent == null)
                {
                    Debug.LogError("ResearchTabUI: Cannot find scroll content panel");
                    return;
                }

                // Research Cost
                InputFieldBuilder.CreateLabeledFloatField(scrollContent, "Research Cost",
                    _editorService.CurrentItem?.m_ResearchCost ?? 0,
                    (value) =>
                    {
                        if (_editorService.CurrentItem != null)
                        {
                            _editorService.UpdateField("m_ResearchCost", Mathf.Max(0, value));
                        }
                    });

                // Blueprint Cost
                InputFieldBuilder.CreateLabeledFloatField(scrollContent, "Blueprint Cost",
                    _editorService.CurrentItem?.m_BlueprintCost ?? 0,
                    (value) =>
                    {
                        if (_editorService.CurrentItem != null)
                        {
                            _editorService.UpdateField("m_BlueprintCost", Mathf.Max(0, value));
                        }
                    });

                // Prototype Cost
                InputFieldBuilder.CreateLabeledFloatField(scrollContent, "Prototype Cost",
                    _editorService.CurrentItem?.m_PrototypeCost ?? 0,
                    (value) =>
                    {
                        if (_editorService.CurrentItem != null)
                        {
                            _editorService.UpdateField("m_PrototypeCost", Mathf.Max(0, value));
                        }
                    });

                // Progression Value (0.0 - 1.0)
                InputFieldBuilder.CreateLabeledFloatField(scrollContent, "Progression",
                    _editorService.CurrentItem?.m_Progression ?? 0,
                    (value) =>
                    {
                        if (_editorService.CurrentItem != null)
                        {
                            _editorService.UpdateField("m_Progression", Mathf.Clamp01(value));
                        }
                    });

                // Add spacing
                var spacer = new GameObject("Spacer");
                spacer.transform.SetParent(scrollContent);

                // Checkboxes
                CheckboxBuilder.CreateLabeledCheckbox(scrollContent, "Available to Player",
                    _editorService.CurrentItem?.m_AvailableToPlayer ?? false,
                    (value) =>
                    {
                        if (_editorService.CurrentItem != null)
                        {
                            _editorService.UpdateField("m_AvailableToPlayer", value);
                        }
                    });

                CheckboxBuilder.CreateLabeledCheckbox(scrollContent, "Player Starts with Blueprints",
                    _editorService.CurrentItem?.m_PlayerStartsWithBlueprints ?? false,
                    (value) =>
                    {
                        if (_editorService.CurrentItem != null)
                        {
                            _editorService.UpdateField("m_PlayerStartsWithBlueprints", value);
                        }
                    });

                CheckboxBuilder.CreateLabeledCheckbox(scrollContent, "Player Starts with Prototype",
                    _editorService.CurrentItem?.m_PlayerStartsWithPrototype ?? false,
                    (value) =>
                    {
                        if (_editorService.CurrentItem != null)
                        {
                            _editorService.UpdateField("m_PlayerStartsWithPrototype", value);
                        }
                    });

                CheckboxBuilder.CreateLabeledCheckbox(scrollContent, "Player Can Research from Start",
                    _editorService.CurrentItem?.m_PlayerCanResearchFromStart ?? false,
                    (value) =>
                    {
                        if (_editorService.CurrentItem != null)
                        {
                            _editorService.UpdateField("m_PlayerCanResearchFromStart", value);
                        }
                    });

                Debug.Log("ResearchTabUI: Research tab UI created successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"ResearchTabUI: CreateUI failed: {e.Message}");
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

                // Fields will be updated when item is loaded
            }
            catch (Exception e)
            {
                Debug.LogError($"ResearchTabUI: RefreshUI failed: {e.Message}");
            }
        }

        #endregion
    }
}
