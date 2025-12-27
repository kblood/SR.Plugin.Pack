using System;
using UnityEngine;
using UnityEngine.UI;
using SRMod.DTOs;
using ItemEditorMod.Services;
using ItemEditorMod.UI.Builders;

namespace ItemEditorMod.UI.Controls
{
    /// <summary>
    /// CombatTabUI - Combat tab with stealth, weapon augmentation, and override ammo fields
    /// </summary>
    public class CombatTabUI
    {
        #region Fields

        private ItemEditorService _editorService;
        private Transform _tabContentPanel;
        private InputField _stealthVsCombatField;
        private InputField _weaponAugMaskField;
        private InputField _overrideAmmoField;

        #endregion

        #region Constructor

        public CombatTabUI(ItemEditorService editorService)
        {
            _editorService = editorService;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create combat tab UI
        /// </summary>
        public void CreateUI(Transform tabContentPanel)
        {
            try
            {
                _tabContentPanel = tabContentPanel;
                Debug.Log("CombatTabUI: Creating combat tab UI");

                // Get the content scroll area
                var scrollContent = _tabContentPanel.Find("TabContent/Panel_Combat/ScrollView/Content");
                if (scrollContent == null)
                {
                    Debug.LogError("CombatTabUI: Cannot find scroll content panel");
                    return;
                }

                // Stealth vs Combat (-1 to 1)
                InputFieldBuilder.CreateLabeledFloatField(scrollContent, "Stealth vs Combat",
                    _editorService.CurrentItem?.m_StealthVsCombat ?? 0,
                    (value) =>
                    {
                        if (_editorService.CurrentItem != null)
                        {
                            _editorService.UpdateField("m_StealthVsCombat", Mathf.Clamp(value, -1, 1));
                        }
                    });

                // Weapon Augmentation Mask (bitmask)
                InputFieldBuilder.CreateLabeledIntField(scrollContent, "Weapon Augmentation Mask",
                    0,
                    (value) =>
                    {
                        if (_editorService.CurrentItem != null)
                        {
                            _editorService.UpdateField("m_WeaponAugmentationMask", value);
                        }
                    });

                // Override Ammo (0 = none)
                InputFieldBuilder.CreateLabeledIntField(scrollContent, "Override Ammo Type",
                    0,
                    (value) =>
                    {
                        if (_editorService.CurrentItem != null)
                        {
                            _editorService.UpdateField("m_OverrideAmmo", value);
                        }
                    });

                Debug.Log("CombatTabUI: Combat tab UI created successfully");
            }
            catch (Exception e)
            {
                Debug.LogError($"CombatTabUI: CreateUI failed: {e.Message}");
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

                // Update fields from item data
                if (_stealthVsCombatField != null)
                {
                    _stealthVsCombatField.text = item.m_StealthVsCombat.ToString("F2");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"CombatTabUI: RefreshUI failed: {e.Message}");
            }
        }

        #endregion
    }
}
