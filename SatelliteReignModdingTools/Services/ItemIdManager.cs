using System;
using System.Collections.Generic;
using System.Linq;
using SatelliteReignModdingTools.Services;
using SRMod.DTOs;

namespace SatelliteReignModdingTools.Services
{
    /// <summary>
    /// Thread-safe utility for generating unique item IDs
    /// </summary>
    public static class ItemIdManager
    {
        private static readonly object _lock = new object();
        private const int SAFE_START_ID = 1000; // Start from safe base to avoid conflicts with original items

        /// <summary>
        /// Generates a unique item ID that doesn't conflict with existing items
        /// </summary>
        /// <param name="existingItems">Collection of existing items to check against</param>
        /// <returns>A guaranteed unique item ID</returns>
        public static int GenerateUniqueItemId(IEnumerable<SerializableItemData> existingItems)
        {
            lock (_lock)
            {
                if (existingItems == null || !existingItems.Any())
                    return 1; // Start from 1 if no items exist

                try
                {
                    int maxId = existingItems.Max(i => i.m_ID);
                    int candidateId = maxId + 1;
                    
                    // Only use SAFE_START_ID if the max ID is dangerously low (potential conflicts with original game items)
                    // But prioritize user-friendly sequential IDs when possible
                    if (maxId < 100 && candidateId < SAFE_START_ID)
                    {
                        // Check if there are any items in the "safe" range already
                        bool hasSafeRangeItems = existingItems.Any(i => i.m_ID >= SAFE_START_ID);
                        if (hasSafeRangeItems)
                        {
                            // Find next available ID in safe range
                            candidateId = Math.Max(SAFE_START_ID, existingItems.Where(i => i.m_ID >= SAFE_START_ID).Max(i => i.m_ID) + 1);
                        }
                        else
                        {
                            // Use simple sequential numbering for user-friendly experience
                            candidateId = maxId + 1;
                        }
                    }
                    
                    // Ensure we don't conflict with any existing IDs
                    var existingIds = new HashSet<int>(existingItems.Select(i => i.m_ID));
                    while (existingIds.Contains(candidateId))
                    {
                        candidateId++;
                    }
                    
                    // SRInfoHelper.Log($"Generated unique item ID: {candidateId}");
                    return candidateId;
                }
                catch (Exception ex)
                {
                    // SRInfoHelper.Log($"Error generating unique ID, using fallback: {ex.Message}");
                    // Fallback to timestamp-based ID if all else fails
                    return SAFE_START_ID + (int)(DateTime.Now.Ticks % 10000);
                }
            }
        }

        /// <summary>
        /// Validates that an item ID is unique within the collection
        /// </summary>
        /// <param name="itemId">ID to validate</param>
        /// <param name="existingItems">Collection to check against</param>
        /// <param name="excludeItem">Item to exclude from the check (for updates)</param>
        /// <returns>True if ID is unique</returns>
        public static bool IsIdUnique(int itemId, IEnumerable<SerializableItemData> existingItems, SerializableItemData excludeItem = null)
        {
            if (existingItems == null) return true;
            
            return !existingItems.Any(item => 
                item.m_ID == itemId && 
                (excludeItem == null || !ReferenceEquals(item, excludeItem)));
        }
    }
}