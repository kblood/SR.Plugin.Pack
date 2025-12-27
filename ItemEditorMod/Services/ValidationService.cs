using System;
using System.Collections.Generic;
using UnityEngine;
using SRMod.DTOs;
using ItemEditorMod.Models;

namespace ItemEditorMod.Services
{
    /// <summary>
    /// ValidationService - Comprehensive item data validation
    /// Checks enums, ranges, translations, icons, and more
    /// </summary>
    public class ValidationService
    {
        #region Public Methods

        /// <summary>
        /// Validate item data comprehensively
        /// </summary>
        public ValidationResult ValidateItem(SerializableItemData item, List<TranslationElementDTO> translations = null, IconManagementService iconService = null)
        {
            var result = new ValidationResult();

            try
            {
                if (item == null)
                {
                    result.Errors.Add("Item data is null");
                    return result;
                }

                // ID validation
                ValidateID(item, result);

                // Name validation
                ValidateName(item, result);

                // Enum validation
                ValidateEnums(item, result);

                // Range validation
                ValidateRanges(item, result);

                // Cost validation
                ValidateCosts(item, result);

                // Progression validation
                ValidateProgression(item, result);

                // Icon validation
                if (iconService != null)
                {
                    ValidateIcon(item, iconService, result);
                }

                // Translation validation
                if (translations != null)
                {
                    ValidateTranslations(item, translations, result);
                }

                // Set valid status
                result.IsValid = result.Errors.Count == 0;
            }
            catch (Exception e)
            {
                result.Errors.Add($"Validation error: {e.Message}");
                result.IsValid = false;
            }

            return result;
        }

        #endregion

        #region Private Validation Methods

        private void ValidateID(SerializableItemData item, ValidationResult result)
        {
            if (item.m_ID < 1)
            {
                result.Errors.Add("Item ID must be greater than 0");
            }
            else if (item.m_ID > 10000)
            {
                result.Warnings.Add("Item ID is unusually high (> 10000)");
            }
        }

        private void ValidateName(SerializableItemData item, ValidationResult result)
        {
            if (string.IsNullOrEmpty(item.m_FriendlyName) || item.m_FriendlyName.Trim().Length == 0)
            {
                result.Errors.Add("Item name cannot be empty");
            }
            else if (item.m_FriendlyName.Length > 100)
            {
                result.Warnings.Add("Item name is very long (> 100 characters)");
            }
        }

        private void ValidateEnums(SerializableItemData item, ValidationResult result)
        {
            // Validate ItemSlotTypes (0-7 valid)
            try
            {
                int slotValue = (int)item.m_Slot;
                if (slotValue < 0 || slotValue > 7)
                {
                    result.Warnings.Add($"Item slot type {slotValue} may be invalid");
                }
            }
            catch
            {
                result.Warnings.Add("Item slot type could not be validated");
            }

            // Validate ItemSubCategories
            try
            {
                int subCatValue = (int)item.m_GearSubCategory;
                if (subCatValue < 0 || subCatValue > 20)
                {
                    result.Warnings.Add($"Item sub-category {subCatValue} may be invalid");
                }
            }
            catch
            {
                result.Warnings.Add("Item sub-category could not be validated");
            }

            // Validate WeaponType (0-29 valid)
            try
            {
                int weaponValue = (int)item.m_WeaponType;
                if (weaponValue < 0 || weaponValue > 29)
                {
                    result.Warnings.Add($"Weapon type {weaponValue} may be invalid");
                }
            }
            catch
            {
                result.Warnings.Add("Weapon type could not be validated");
            }
        }

        private void ValidateRanges(SerializableItemData item, ValidationResult result)
        {
            // Stealth vs Combat (-1 to 1)
            if (item.m_StealthVsCombat < -1 || item.m_StealthVsCombat > 1)
            {
                result.Errors.Add("Stealth vs Combat must be between -1 and 1");
            }
        }

        private void ValidateCosts(SerializableItemData item, ValidationResult result)
        {
            if (item.m_Cost < 0)
                result.Warnings.Add("Base cost is negative");

            if (item.m_ResearchCost < 0)
                result.Warnings.Add("Research cost is negative");

            if (item.m_BlueprintCost < 0)
                result.Warnings.Add("Blueprint cost is negative");

            if (item.m_PrototypeCost < 0)
                result.Warnings.Add("Prototype cost is negative");

            // Check if all costs are zero
            if (item.m_Cost == 0 && item.m_ResearchCost == 0 &&
                item.m_BlueprintCost == 0 && item.m_PrototypeCost == 0)
            {
                result.Warnings.Add("All costs are zero - item may be free");
            }
        }

        private void ValidateProgression(SerializableItemData item, ValidationResult result)
        {
            if (item.m_Progression < 0 || item.m_Progression > 1)
            {
                result.Errors.Add("Progression must be between 0 and 1");
            }
        }

        private void ValidateIcon(SerializableItemData item, IconManagementService iconService, ValidationResult result)
        {
            try
            {
                if (string.IsNullOrEmpty(item.m_UIIconName))
                {
                    result.Warnings.Add("Item has no icon assigned");
                }
                else
                {
                    var availableIcons = iconService.GetAvailableIconNames();
                    if (!availableIcons.Contains(item.m_UIIconName))
                    {
                        result.Warnings.Add($"Icon '{item.m_UIIconName}' not found in icons directory");
                    }
                }
            }
            catch (Exception e)
            {
                result.Warnings.Add($"Icon validation failed: {e.Message}");
            }
        }

        private void ValidateTranslations(SerializableItemData item, List<TranslationElementDTO> translations, ValidationResult result)
        {
            try
            {
                string nameKey = $"ITEM_{item.m_ID}_NAME";
                string descKey = $"ITEM_{item.m_ID}_DESCRIPTION";

                bool hasNameKey = false;
                bool hasDescKey = false;

                foreach (var translation in translations)
                {
                    if (translation.Key == nameKey)
                        hasNameKey = true;
                    if (translation.Key == descKey)
                        hasDescKey = true;
                }

                if (!hasNameKey)
                    result.Warnings.Add($"Translation key '{nameKey}' not found");

                if (!hasDescKey)
                    result.Warnings.Add($"Translation key '{descKey}' not found");
            }
            catch (Exception e)
            {
                result.Warnings.Add($"Translation validation failed: {e.Message}");
            }
        }

        #endregion
    }
}
