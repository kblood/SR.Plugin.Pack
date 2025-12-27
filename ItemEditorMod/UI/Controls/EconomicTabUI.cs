using System;
using UnityEngine;
using UnityEngine.UI;
using SRMod.DTOs;
using ItemEditorMod.Services;
using ItemEditorMod.UI.Builders;

namespace ItemEditorMod.UI.Controls
{
    /// <summary>
    /// EconomicTabUI - Economic tab with item costs and pricing
    /// </summary>
    public class EconomicTabUI
    {
        #region Fields

        private ItemEditorService _editorService;
        private Transform _tabContentPanel;

        #endregion

        #region Constructor

        public EconomicTabUI(ItemEditorService editorService)
        {
            _editorService = editorService;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create economic tab UI
        /// </summary>
        public void CreateUI(Transform tabContentPanel)
        {
            try
            {
                _tabContentPanel = tabContentPanel;
                Debug.Log("EconomicTabUI: Creating economic tab UI");

                // Get the content scroll area
                var scrollContent = _tabContentPanel.Find("TabContent/Panel_Economic/ScrollView/Content");
                if (scrollContent == null)
                {
                    Debug.LogError("EconomicTabUI: Cannot find scroll content panel");
                    return;
                }

                // Base Cost
                InputFieldBuilder.CreateLabeledFloatField(scrollContent, "Base Cost",
                    _editorService.CurrentItem?.m_Cost ?? 0,
                    (value) =>
                    {
                        if (_editorService.CurrentItem != null)
                        {
                            _editorService.UpdateField("m_Cost", Mathf.Max(0, value));
                        }
                    });

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

                // Progression Value (affects costs)
                InputFieldBuilder.CreateLabeledFloatField(scrollContent, "Progression (0.0 - 1.0)",
                    _editorService.CurrentItem?.m_Progression ?? 0,
                    (value) =>
                    {
                        if (_editorService.CurrentItem != null)
                        {
                            _editorService.UpdateField("m_Progression", Mathf.Clamp01(value));
                        }
                    });

                // Info text
                var infoGO = new GameObject("Info");
                infoGO.transform.SetParent(scrollContent);
                var infoText = infoGO.AddComponent<Text>();
                infoText.text = "Progression affects item availability in tech tiers.\n0 = Early game, 1 = Late game";
                infoText.font = Resources.Load<Font>("Arial");
                infoText.fontSize = 12;
                infoText.fontStyle = FontStyle.Italic;
                infoText.color = new Color(0.7f, 0.7f, 0.7f, 1);
                infoText.alignment = TextAnchor.MiddleLeft;

                var infoRect = infoGO.AddComponent<RectTransform>();
                infoRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
                infoRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 50);

                var le = infoGO.AddComponent<LayoutElement>();
                le.preferredHeight = 50;

                Debug.Log("EconomicTabUI: Economic tab UI created successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"EconomicTabUI: CreateUI failed: {e.Message}");
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
                Debug.LogError($"EconomicTabUI: RefreshUI failed: {e.Message}");
            }
        }

        #endregion
    }
}
