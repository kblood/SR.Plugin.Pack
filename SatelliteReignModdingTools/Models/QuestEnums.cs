using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SatelliteReignModdingTools.Models
{
    public enum QuestDistrict
    {
        [Description("None")]
        NONE,
        
        [Description("Red Light District")]
        RedLight,
        
        [Description("Industrial District")]
        Industrial,
        
        [Description("Grid District")]
        Grid,
        
        [Description("CBD District")]
        CBD,
        
        [Description("Boss Fight Area")]
        BossFight
    }

    public static class QuestValidation
    {
        // Valid location ID ranges based on game data analysis
        public static readonly Dictionary<QuestDistrict, List<int>> ValidLocationsByDistrict = new Dictionary<QuestDistrict, List<int>>
        {
            { QuestDistrict.RedLight, new List<int> { 17, 18, 19, 20, 21, 27, 28, 29, 30, 31, 32, 33, 34 } },
            { QuestDistrict.Industrial, new List<int> { 47, 48, 49, 50, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 70, 95, 97, 98 } },
            { QuestDistrict.Grid, new List<int> { 51, 52, 53, 54, 71, 72, 73, 74, 75, 76, 77, 78 } },
            { QuestDistrict.CBD, new List<int> { 55, 56, 57, 58, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89 } },
            { QuestDistrict.BossFight, new List<int> { 137 } },
            { QuestDistrict.NONE, new List<int> { 127, 128, 129, 130, 138, 139 } }
        };

        // Common checkpoint locations that span districts
        public static readonly List<int> CheckpointLocations = new List<int> { 127, 128, 129, 130, 138, 139 };

        public static bool IsValidLocationForDistrict(int locationId, QuestDistrict district)
        {
            if (locationId == -1) return true; // -1 is always valid (disabled)
            
            return ValidLocationsByDistrict.ContainsKey(district) && 
                   ValidLocationsByDistrict[district].Contains(locationId);
        }

        public static List<int> GetValidLocationsForDistrict(QuestDistrict district)
        {
            return ValidLocationsByDistrict.ContainsKey(district) 
                ? ValidLocationsByDistrict[district] 
                : new List<int>();
        }

        // Location name mappings (where known)
        public static readonly Dictionary<int, string> LocationNames = new Dictionary<int, string>
        {
            { -1, "Disabled" },
            { 17, "Dracogenics Guard Station (RL)" },
            { 18, "Bank (RL)" },
            { 19, "Police Station (RL)" },
            { 20, "Ronin Inc Droid Repairs (RL)" },
            { 21, "City Surveillance Authority (RL)" },
            { 27, "Armory (RL Military)" },
            { 28, "Barracks (RL Military)" },
            { 29, "Military Supply Docks (RL Military)" },
            { 30, "Storage Area (Wyvern)" },
            { 31, "Cyber-clinic (Wyvern)" },
            { 32, "Continental Haven Hotel (Wyvern)" },
            { 33, "Wyvern Inc Main (RL)" },
            { 34, "Military Compound (RL)" },
            
            { 47, "Bank (Industrial)" },
            { 48, "City Surveillance Authority (Industrial)" },
            { 49, "Ronin Inc Droid Repairs (Industrial)" },
            { 50, "Dracogenics Guard Station (Industrial)" },
            
            { 51, "Bank (Grid)" },
            { 52, "City Surveillance Authority (Grid)" },
            { 53, "Ronin Inc Droid Repairs (Grid)" },
            { 54, "Dracogenics Guard Station (Grid)" },
            
            { 55, "Bank (CBD)" },
            { 56, "City Surveillance Authority (CBD)" },
            { 57, "Ronin Inc City Drone Central (CBD)" },
            { 58, "Dracogenics Guard Station (CBD)" },
            
            { 127, "Industrial Checkpoint Access" },
            { 128, "Grid Checkpoint Access" },
            { 129, "CBD Checkpoint North Access" },
            { 130, "CBD Checkpoint South Access" },
            { 137, "Boss Fight Arena" },
            { 138, "Industrial Checkpoint Entry" },
            { 139, "Grid Checkpoint Entry" }
        };

        public static string GetLocationName(int locationId)
        {
            return LocationNames.ContainsKey(locationId) 
                ? LocationNames[locationId] 
                : $"Location {locationId}";
        }
    }

    // Helper class for quest field descriptions
    public static class QuestFieldDescriptions
    {
        public static readonly Dictionary<string, string> FieldHelp = new Dictionary<string, string>
        {
            { "ID", "Unique quest identifier (auto-generated for new quests)" },
            { "TitleKey", "Translation key for quest title (links to translations.xml)" },
            { "Title", "Displayed quest title (auto-populated from translation key)" },
            { "Hidden", "Whether quest is hidden from quest log" },
            { "ShowDebrief", "Whether to show completion debrief screen" },
            { "District", "Game district where quest is located" },
            { "WakeOnLocation", "Location ID that triggers quest activation (-1 to disable)" },
            { "WakeOnLocationList", "Multiple locations that can trigger quest activation" },
            { "HasVIP", "Whether quest involves VIP extraction mechanics" },
            { "Descriptions", "Quest description entries with translation keys" },
            { "SubQuests", "Child quests that belong to this quest" },
            { "Rewards", "Item rewards given upon quest completion" }
        };
    }
}