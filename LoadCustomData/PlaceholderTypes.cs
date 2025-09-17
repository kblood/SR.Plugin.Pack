using System;
using System.Collections.Generic;
using UnityEngine;
using LoadCustomDataMod;

// Placeholder types to resolve compilation errors when game assembly references fail
// These are minimal implementations to allow building - the actual types come from Assembly-CSharp.dll

namespace SRMod.Placeholders
{
    // Minimal placeholder for missing GameObject (should come from UnityEngine)
    // This suggests UnityEngine assembly reference is broken
    public class GameObject_Placeholder
    {
        public bool activeSelf;
        public void SetActive(bool active) { activeSelf = active; }
    }

    // Quest system placeholders - define in order to avoid circular references
    public class QuestAction_Placeholder
    {
        public int m_ID = -1;
        public string m_Address;
        public bool m_DeactivateAfterPerformingAction;
    }

    public class QuestReaction_Placeholder
    {
        public int m_ID = -1;
        public string m_Address;
    }

    public class QuestElement_Placeholder
    {
        public int m_ID = -1;
        public string m_Title;
        public bool m_Hidden;
        public bool m_ShowDebrief;
        public int m_State; // QuestState enum
        public bool m_IsNew;
        public string m_Address;
        public bool m_DeactivateAfterPerformingAction;
        public bool m_CanLoadFromSave = true;
        public int m_LocationID = -1;
        public int m_VIPID = -1;
        public int m_WakeOnLocation = -1;
        public List<int> m_WakeOnLocationList = new List<int>();
        public List<int> m_MandatoryQuestIDs = new List<int>();
        public List<int> m_ChildQuestIDs = new List<int>();
        public List<QuestAction_Placeholder> m_Actions = new List<QuestAction_Placeholder>();
        public List<QuestReaction_Placeholder> m_Reactions = new List<QuestReaction_Placeholder>();
        
        public static QuestElement_Placeholder FindParentQuestElement(Transform transform) { return null; }
        public void UpdatePings() { }
    }

    public class QuestManager_Placeholder
    {
        public static bool m_IsLoading;
        public List<QuestElement_Placeholder> m_QuestElements = new List<QuestElement_Placeholder>();
        public QuestElement_Placeholder m_BaseQuestElement;
        public int m_CurrentDisplayQuestID = -1;
        public int m_RandomSeed;
        public List<int> m_UsedRandomDataTerminalLocations = new List<int>();
        public List<int> m_UsedRandomVIPLocations = new List<int>();
    }

    // Manager placeholders for types that should exist in Assembly-CSharp
    public class ItemManager_Placeholder
    {
        public List<object> m_ItemDefinitions = new List<object>();
        public List<object> GetAllItems() { return new List<object>(); }
    }

    public class TextManager_Placeholder
    {
        public class TextElement
        {
            public int m_ID;
            public string m_Token;
            public string m_Translation;
        }
        
        public static Dictionary<int, TextElement> m_TextElements = new Dictionary<int, TextElement>();
        public static List<TextElement> GetTextElements() { return new List<TextElement>(); }
    }

    public class WardrobeManager_Placeholder
    {
        // Placeholder for missing WardrobeManager
    }

    public class SpawnManager_EnemyEntry_Placeholder
    {
        // Placeholder for missing SpawnManager_EnemyEntry
    }

    public class WeaponAttachmentAmmo_Placeholder
    {
        // Placeholder for missing WeaponAttachmentAmmo
    }

    // SaveQuestManager should be accessible but create placeholder if needed
    public class SaveQuestManager_Placeholder
    {
        private QuestManager questManager;
        public List<object> m_QuestBits = new List<object>();
        
        public SaveQuestManager_Placeholder(QuestManager qm)
        {
            this.questManager = qm;
        }
    }
}