using System;
using SRMod.DTOs;
using UnityEngine;

namespace ItemEditorMod.Services
{
    /// <summary>
    /// ItemCloneService - Handles item duplication with sequential ID assignment
    /// </summary>
    public class ItemCloneService
    {
        #region Public Methods

        /// <summary>
        /// Clone an item with a new ID
        /// </summary>
        public SerializableItemData CloneItem(SerializableItemData sourceItem, int newItemId)
        {
            try
            {
                if (sourceItem == null)
                {
                    Debug.LogError("ItemCloneService: Source item is null");
                    return null;
                }

                // Create new item based on source
                var clonedItem = new SerializableItemData
                {
                    m_ID = newItemId,
                    m_FriendlyName = sourceItem.m_FriendlyName + " (Copy)",
                    m_Slot = sourceItem.m_Slot,
                    m_GearSubCategory = sourceItem.m_GearSubCategory,
                    m_WeaponType = sourceItem.m_WeaponType,
                    m_Cost = sourceItem.m_Cost,
                    m_ResearchCost = sourceItem.m_ResearchCost,
                    m_BlueprintCost = sourceItem.m_BlueprintCost,
                    m_PrototypeCost = sourceItem.m_PrototypeCost,
                    m_Progression = sourceItem.m_Progression,
                    m_StealthVsCombat = sourceItem.m_StealthVsCombat,
                    m_AvailableToPlayer = sourceItem.m_AvailableToPlayer,
                    m_PlayerStartsWithBlueprints = sourceItem.m_PlayerStartsWithBlueprints,
                    m_PlayerStartsWithPrototype = sourceItem.m_PlayerStartsWithPrototype,
                    m_PlayerCanResearchFromStart = sourceItem.m_PlayerCanResearchFromStart,
                    m_UIIconName = sourceItem.m_UIIconName
                };

                Debug.Log($"ItemCloneService: Cloned item ID {sourceItem.m_ID} to new ID {newItemId}");
                return clonedItem;
            }
            catch (Exception e)
            {
                Debug.LogError($"ItemCloneService: CloneItem failed: {e.Message}");
                return null;
            }
        }

        #endregion
    }
}
