using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SRMod.DTOs
{
    [Serializable]
    public class SerializableQuestElement
    {
        public int m_ID;
        public string m_Title; // Localization key
        public bool m_Hidden; // Whether quest appears in mission control
        public bool m_ShowDebrief; // Whether to show completion debrief
        public QuestState m_State;
        public bool m_IsNew;
        public string m_Address; // Hierarchical address in quest tree
        public bool m_DeactivateAfterPerformingAction;
        public bool m_CanLoadFromSave;
        
        // Location and VIP associations
        public int m_LocationID = -1;
        public int m_VIPID = -1;
        public int m_WakeOnLocation = -1;
        public List<int> m_WakeOnLocationList;
        
        // Quest relationships
        public List<int> m_MandatoryQuestIDs;
        public List<int> m_ChildQuestIDs;
        
        // Quest components
        public List<SerializableQuestAction> m_Actions;
        public List<SerializableQuestReaction> m_Reactions;
        public List<SerializableDescriptionData> m_Descriptions;
        
        public SerializableQuestElement()
        {
            m_WakeOnLocationList = new List<int>();
            m_MandatoryQuestIDs = new List<int>();
            m_ChildQuestIDs = new List<int>();
            m_Actions = new List<SerializableQuestAction>();
            m_Reactions = new List<SerializableQuestReaction>();
            m_Descriptions = new List<SerializableDescriptionData>();
        }
    }

    [Serializable]
    public class SerializableQuestAction
    {
        public string m_ActionType; // Type name for reconstruction
        public int m_ID;
        public string m_Address;
        public bool m_DeactivateAfterPerformingAction;
        
        // Common action parameters
        public Dictionary<string, object> m_Parameters;
        
        public SerializableQuestAction()
        {
            m_Parameters = new Dictionary<string, object>();
        }
    }

    [Serializable]
    public class SerializableQuestReaction
    {
        public string m_ReactionType; // Type name for reconstruction
        public int m_ID;
        public string m_Address;
        
        // Common reaction parameters
        public Dictionary<string, object> m_Parameters;
        
        public SerializableQuestReaction()
        {
            m_Parameters = new Dictionary<string, object>();
        }
    }

    [Serializable]
    public class SerializableDescriptionData
    {
        public bool m_IsNew = true;
        public bool m_HasBeenSeen;
        public string m_LocTitle = "[TAG]";
        public string m_Token = string.Empty;
        public string m_Translation = string.Empty;
        public bool m_Expanded = true;
        public int m_DisplayOrderOffset;
        public float m_PauseBeforeAudio;
    }

    [Serializable]
    public class SerializableLocationSelector
    {
        public int m_LocationID = -1;
        public bool m_VIPFriendly;
        public bool m_DataTerminalFriendly;
        public DistrictFilterType m_DistricFilterType;
        public SecureFilterType m_SecureFilterType;
        public int m_PickupPointCount;
        public List<int> m_ValidDistrictIDs;
        
        public SerializableLocationSelector()
        {
            m_ValidDistrictIDs = new List<int>();
        }
    }

    [Serializable]
    public class SerializableVIPSpawner
    {
        public int m_VIPToSpawnID = -1;
        public int m_NameID = -1;
        public VIPType m_type;
        public int m_BribeAmount;
        public List<int> m_BodyGuardIDs;
        public bool m_SpawnAtLocation;
        
        public SerializableVIPSpawner()
        {
            m_BodyGuardIDs = new List<int>();
        }
    }

    [Serializable]
    public class SerializableItemAwarder
    {
        public List<int> m_SpecificPrototypeIDs;
        public List<int> m_SpecificBlueprintIDs;
        public bool m_AwardItemsInstantly;
        public int m_RandomItemCount;
        public ItemSlotTypes m_RandomItemSlotFilter;
        
        public SerializableItemAwarder()
        {
            m_SpecificPrototypeIDs = new List<int>();
            m_SpecificBlueprintIDs = new List<int>();
        }
    }

    [Serializable]
    public class SerializableQuestManager
    {
        public int m_CurrentDisplayQuestID = -1;
        public int m_RandomSeed;
        public List<int> m_UsedRandomDataTerminalLocations;
        public List<int> m_UsedRandomVIPLocations;
        public List<SerializableQuestElement> m_QuestElements;
        public SerializableQuestElement m_BaseQuestElement;
        
        public SerializableQuestManager()
        {
            m_UsedRandomDataTerminalLocations = new List<int>();
            m_UsedRandomVIPLocations = new List<int>();
            m_QuestElements = new List<SerializableQuestElement>();
        }
    }

    // Specialized action DTOs for common quest actions
    [Serializable]
    public class SerializableQAGiveItem : SerializableQuestAction
    {
        public SerializableItemAwarder m_ItemAwarder;
        
        public SerializableQAGiveItem()
        {
            m_ActionType = "QAGiveItem";
            m_ItemAwarder = new SerializableItemAwarder();
        }
    }

    [Serializable]
    public class SerializableQAGiveCash : SerializableQuestAction
    {
        public int m_CashAmount;
        public bool m_ShowFeedback;
        public bool m_PlaySound;
        
        public SerializableQAGiveCash()
        {
            m_ActionType = "QAGiveCash";
        }
    }

    [Serializable]
    public class SerializableQASelectLocation : SerializableQuestAction
    {
        public SerializableLocationSelector m_LocationSelector;
        
        public SerializableQASelectLocation()
        {
            m_ActionType = "QASelectLocation";
            m_LocationSelector = new SerializableLocationSelector();
        }
    }

    [Serializable]
    public class SerializableQASpawnVIP : SerializableQuestAction
    {
        public SerializableVIPSpawner m_VIPSpawner;
        
        public SerializableQASpawnVIP()
        {
            m_ActionType = "QASpawnVIP";
            m_VIPSpawner = new SerializableVIPSpawner();
        }
    }

    [Serializable]
    public class SerializableQABroadcastMessage : SerializableQuestAction
    {
        public string m_MessageTitle;
        public string m_MessageText;
        public float m_DisplayDuration;
        
        public SerializableQABroadcastMessage()
        {
            m_ActionType = "QABroadcastMessage";
        }
    }

    [Serializable]
    public class SerializableQAActivateQuest : SerializableQuestAction
    {
        public int m_QuestIDToActivate;
        
        public SerializableQAActivateQuest()
        {
            m_ActionType = "QAActivateQuest";
        }
    }

    // Specialized reaction DTOs for common quest reactions
    [Serializable]
    public class SerializableQRWait : SerializableQuestReaction
    {
        public float m_WaitTime;
        
        public SerializableQRWait()
        {
            m_ReactionType = "QRWait";
        }
    }

    [Serializable]
    public class SerializableQRDataTerminalAccessed : SerializableQuestReaction
    {
        public int m_RequiredLocationID = -1;
        public bool m_AnyDataTerminal;
        
        public SerializableQRDataTerminalAccessed()
        {
            m_ReactionType = "QRDataTerminalAccessed";
        }
    }

    [Serializable]
    public class SerializableQRFacilityBreached : SerializableQuestReaction
    {
        public int m_RequiredLocationID = -1;
        public bool m_AnyFacility;
        
        public SerializableQRFacilityBreached()
        {
            m_ReactionType = "QRFacilityBreached";
        }
    }

    [Serializable]
    public class SerializableQRProgressionData : SerializableQuestReaction
    {
        public string m_ProgressionKey;
        public object m_RequiredValue;
        public ComparisonType m_ComparisonType;
        
        public SerializableQRProgressionData()
        {
            m_ReactionType = "QRProgressionData";
        }
    }

    // Note: QuestState enum is defined in the game assembly
    // Values: inactive, active, complete

    public enum DistrictFilterType
    {
        Any,
        Specific,
        Current,
        Unlocked
    }

    public enum SecureFilterType
    {
        Any,
        LowSecurity,
        MediumSecurity,
        HighSecurity
    }

    public enum ComparisonType
    {
        Equals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual
    }
}