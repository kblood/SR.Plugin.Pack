using System.Xml.Serialization;
using System.IO;
using System.Text;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using SRMod.DTOs;
using SRMod.Services;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager _instance;

    private Dictionary<GroupID, List<SpawnCard>> _spawnDecks;
    private List<SerializableEnemyEntry> _serializableEnemyEntries;
    private List<Transform> _prefabs = new List<Transform>();
    //private const string SAVE_FILE_NAME = "spawnCardDecks.xml";
    private const string SAVE_FILE_NAME = "enemyentries.xml";

    public static EnemyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EnemyManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("SpawnCardManager");
                    _instance = go.AddComponent<EnemyManager>();
                }
            }
            return _instance;
        }
    }

    public bool CheckIfXMLFileExists()
    {
        string path = Path.Combine(Manager.GetPluginManager().PluginPath, SAVE_FILE_NAME);
        return File.Exists(path);
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void Initialize()
    {
        SRInfoHelper.Log($"Initializing spawn card manager.");

        var spawnmanager = SpawnManager.Get();

        _serializableEnemyEntries = new List<SerializableEnemyEntry>();
        foreach (var enemy in spawnmanager.m_EnemyDefinitions)
        {
            _serializableEnemyEntries.Add(new SerializableEnemyEntry(enemy));
        }

        foreach (var prefab in spawnmanager.m_EnemyDefinitions.Select(d => d.m_Prefab).Distinct().ToList())
        {
            if (prefab != null && !_prefabs.Contains(prefab))
            {
                _prefabs.Add(prefab);
            }
        }
        SRInfoHelper.Log($"Found {_prefabs.Count} unique prefabs on the enemy definitions.");
        if (!LoadEnemyEntriesFromFileAndUpdateSpawnManager())
            SaveEnemyEntries();

        //_spawnDecks = new Dictionary<GroupID, List<SpawnCard>>();
        //foreach (GroupID groupId in Enum.GetValues(typeof(GroupID)))
        //{
        //    _spawnDecks[groupId] = new List<SpawnCard>();
        //}

        //// Try to load spawn cards from file, if it exists
        //if (!LoadSpawnCardsFromFileAndUpdateSpawnManager())
        //{
        //    SRInfoHelper.Log($"Cannot load from file, populating spawn cards from spawn manager...");
        //    // If loading fails, populate from SpawnManager
        //    PopulateFromSpawnManager();
        //}
    }

    private IEnumerator SaveSpawnCardsCoroutine()
    {
        try
        {
            var cardlist = new List<SerializableSpawnCardList>();
            foreach (var kvp in _spawnDecks)
            {
                var serializableData = new SerializableSpawnCardData();

                foreach (var card in kvp.Value)
                {
                    var enemy = card.m_Enemy;
                    serializableData.Cards.Add(new SerializableSpawnCard
                    {
                        GroupId = kvp.Key,
                        EnemyUID = enemy.m_UID,
                        EnemyName = enemy.m_EnemyName,
                        MinProgression = card.m_MinProgression,
                        MaxProgression = card.m_MaxProgression,
                        Spawnable = enemy.m_Spawnable,
                        Solo = enemy.m_Solo,
                        Modifiers = enemy.m_Modifiers.Select(m => new SerializableModifierData(m)).ToList(),
                        UseWardrobeOverride = enemy.m_UseWardrobeOverride,
                        WardrobeOverride = enemy.m_WardrobeOverride,
                        WeaponOverrides = enemy.m_WeaponOverrides,
                        ItemIds = enemy.m_ItemIds,
                        AbilityOverrides = enemy.m_AbilityOverrides
                    });
                }
                cardlist.Add(new SerializableSpawnCardList { groupId = kvp.Key, cards = serializableData.Cards });
            }

            SRInfoHelper.Log($"Preparing to save {cardlist.Count} spawncarddecks with a total of {cardlist.SelectMany(c => c.cards).Count()} cards to file...");

            var serializer = new XmlSerializer(typeof(List<SerializableSpawnCardList>));
            using (var stringWriter = new StringWriter())
            {
                serializer.Serialize(stringWriter, cardlist);
                string xml = stringWriter.ToString();

                SRInfoHelper.Log($"XML serialization complete. XML length: {xml.Length}");
                SRInfoHelper.Log($"XML preview: {xml.Substring(0, System.Math.Min(xml.Length, 500))}");

                string path = Path.Combine(Manager.GetPluginManager().PluginPath, SAVE_FILE_NAME);
                File.WriteAllText(path, xml);
                SRInfoHelper.Log($"Spawn cards saved to {path}. File size {new FileInfo(path).Length} bytes");
            }
        }
        catch (System.Exception e)
        {
            SRInfoHelper.Log($"Error saving spawn cards: {e.Message}\nStackTrace: {e.StackTrace}");
        }
        yield return null; // Wait for the next frame
    }

    private void SaveEnemyEntries()
    {
        try
        {
            SRInfoHelper.Log($"Preparing to save {_serializableEnemyEntries.Count} enemy entries to file...");

            var serializer = new XmlSerializer(typeof(List<SerializableEnemyEntry>));
            string path = Path.Combine(Manager.GetPluginManager().PluginPath, SAVE_FILE_NAME);

            using (var streamWriter = new StreamWriter(path))
            {
                serializer.Serialize(streamWriter, _serializableEnemyEntries);
            }
            SRInfoHelper.Log($"Enemy entries saved to {path}. File size {new FileInfo(path).Length} bytes");

            /*
            SRInfoHelper.Log($"Saving all trackerIcons");
            var icons = SpawnManager.Get().m_EnemyDefinitions.Select(d => d.m_TrackerIconSprite.texture).Distinct().ToList();
            foreach (var icon in icons)
                FileManager.SaveTextureToFile(icon);
            */
        }
        catch (System.Exception e)
        {
            SRInfoHelper.Log($"Error saving enemy definitions: {e.Message}\nStackTrace: {e.StackTrace}");
        }
    }

    public bool LoadEnemyEntriesFromFileAndUpdateSpawnManager()
    {
        string path = Path.Combine(Manager.GetPluginManager().PluginPath, SAVE_FILE_NAME);
        if (File.Exists(path))
        {
            var serializer = new XmlSerializer(typeof(List<SerializableEnemyEntry>));
            using (var streamReader = new StreamReader(path))
            {
                var serializableData = (List<SerializableEnemyEntry>)serializer.Deserialize(streamReader);
                _serializableEnemyEntries = serializableData;
                SpawnManager spawnManager = SpawnManager.Get();
                //spawnManager.m_EnemyDefinitions.Clear();
                spawnManager.m_EnemyDefinitions = new List<SpawnManager_EnemyEntry>();
                foreach (var enemyEntry in serializableData)
                {
                    var enemyData = spawnManager.GetEnemyDataFromUID(enemyEntry.EnemyUID) ?? enemyEntry.ToEnemyEntry();

                    // Update the enemy data with the deserialized values
                    enemyData.m_Group = enemyEntry.GroupId;
                    enemyData.m_EnemyName = enemyEntry.EnemyName;
                    enemyData.m_Spawnable = enemyEntry.Spawnable;
                    enemyData.m_Solo = enemyEntry.Solo;
                    enemyData.m_Modifiers = enemyEntry.Modifiers.Select(m => m.ToModifierData()).ToArray();
                    enemyData.m_UseWardrobeOverride = enemyEntry.UseWardrobeOverride;
                    enemyData.m_WardrobeOverride = enemyEntry.WardrobeOverride;
                    enemyData.m_WeaponOverrides = enemyEntry.WeaponOverrides;
                    enemyData.m_ItemIds = enemyEntry.ItemIds;
                    enemyData.m_AbilityOverrides = enemyEntry.AbilityOverrides;
                    enemyData.m_CountKeys = enemyEntry.m_CountKeys;
                    if (_prefabs.Any(p => p.name == enemyEntry.m_Prefab_name))
                        enemyData.m_Prefab = _prefabs.First(p => p.name == enemyEntry.m_Prefab_name);

                    // Update m_EnemyDefinitions in SpawnManager
                    if (!spawnManager.m_EnemyDefinitions.Any(e => e.m_UID == enemyData.m_UID))
                        spawnManager.m_EnemyDefinitions.Add(enemyData);
                }
                // Use reflection to access the private m_SpawnDecks field in SpawnManager
                var spawnDecksField = typeof(SpawnManager).GetField("m_SpawnDecks", BindingFlags.NonPublic | BindingFlags.Instance);
                SRInfoHelper.Log($"Updated enemy definitions. Clearing SpawnManager's m_SpawnDecks. {((Dictionary<GroupID, List<SpawnCard>>)spawnDecksField.GetValue(spawnManager)).Count()} decks in the dictionary now.");
                // Set the private field m_SpawnDecks to the new value (_spawnDecks)
                var emptyDecks = new Dictionary<GroupID, List<SpawnCard>>();
                spawnDecksField.SetValue(spawnManager, emptyDecks);
                SRInfoHelper.Log($"Cleared SpawnManager's m_SpawnDecks. {((Dictionary<GroupID, List<SpawnCard>>)spawnDecksField.GetValue(spawnManager)).Count()} decks in the dictionary now.");

                // Get the MethodInfo for the Awake method
                MethodInfo awakeMethod = typeof(SpawnManager).GetMethod("Awake",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                if (awakeMethod != null)
                {
                    // Invoke the Awake method
                    awakeMethod.Invoke(spawnManager, null);
                    SRInfoHelper.Log("Successfully ran SpawnManager's Awake method via reflection.");
                    SRInfoHelper.Log($"{((Dictionary<GroupID, List<SpawnCard>>)spawnDecksField.GetValue(spawnManager)).Count()} decks in the m_SpawnDecks dictionary now.");
                }
                else
                {
                    SRInfoHelper.Log("Failed to find Awake method in SpawnManager via reflection.");
                }
            }
            SRInfoHelper.Log($"Loaded and updated SpawnManager with {_serializableEnemyEntries.Count} enemy entries from file");
            return true;
        }
        else
            SRInfoHelper.Log($"No {SAVE_FILE_NAME} file found.");
        return false;
    }

    public bool LoadSpawnCardsFromFileAndUpdateSpawnManager()
    {
        string path = Path.Combine(Manager.GetPluginManager().PluginPath, SAVE_FILE_NAME);
        if (File.Exists(path))
        {
            try
            {
                var serializer = new XmlSerializer(typeof(List<SerializableSpawnCardList>));
                using (var streamReader = new StreamReader(path))
                {
                    var serializableData = (List<SerializableSpawnCardList>)serializer.Deserialize(streamReader);

                    _spawnDecks.Clear();
                    SpawnManager spawnManager = SpawnManager.Get();
                    //spawnManager.m_EnemyDefinitions.Clear();
                    spawnManager.m_EnemyDefinitions = new List<SpawnManager_EnemyEntry>();
                    foreach (var carddeck in serializableData)
                    {
                        if (!_spawnDecks.ContainsKey(carddeck.groupId))
                            _spawnDecks[carddeck.groupId] = new List<SpawnCard>();

                        foreach (var card in carddeck.cards)
                        {
                            var enemyData = spawnManager.GetEnemyDataFromUID(card.EnemyUID) ?? new SpawnManager_EnemyEntry(card.EnemyUID);

                            // Update the enemy data with the deserialized values
                            enemyData.m_EnemyName = card.EnemyName;
                            enemyData.m_Spawnable = card.Spawnable;
                            enemyData.m_Solo = card.Solo;
                            enemyData.m_Modifiers = card.Modifiers.Select(m => m.ToModifierData()).ToArray();
                            enemyData.m_UseWardrobeOverride = card.UseWardrobeOverride;
                            enemyData.m_WardrobeOverride = card.WardrobeOverride;
                            enemyData.m_WeaponOverrides = card.WeaponOverrides;
                            enemyData.m_ItemIds = card.ItemIds;
                            enemyData.m_AbilityOverrides = card.AbilityOverrides;

                            // Update m_EnemyDefinitions in SpawnManager
                            if (!spawnManager.m_EnemyDefinitions.Any(e => e.m_UID == enemyData.m_UID))
                                spawnManager.m_EnemyDefinitions.Add(enemyData);

                            // Create the spawn card and add it to the correct spawn deck
                            var spawnCard = new SpawnCard
                            {
                                m_Enemy = enemyData,
                                m_MinProgression = card.MinProgression,
                                m_MaxProgression = card.MaxProgression
                            };

                            // Add to internal spawn deck structure
                            if (!_spawnDecks.ContainsKey(card.GroupId))
                            {
                                _spawnDecks[card.GroupId] = new List<SpawnCard>();
                            }
                            _spawnDecks[card.GroupId].Add(spawnCard);
                        }
                    }

                    // Use reflection to access the private m_SpawnDecks field in SpawnManager
                    var spawnDecksField = typeof(SpawnManager).GetField("m_SpawnDecks", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (spawnDecksField != null)
                    {
                        // Set the private field m_SpawnDecks to the new value (_spawnDecks)
                        var test = new Dictionary<GroupID, List<SpawnCard>>();
                        spawnDecksField.SetValue(spawnManager, test);
                        SRInfoHelper.Log($"m_SpawnDecks cleared.");
                        spawnDecksField.SetValue(spawnManager, _spawnDecks);
                        typeof(SpawnManager).GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(spawnManager, null);
                        SRInfoHelper.Log($"Successfully updated m_SpawnDecks via reflection running the awake method.");
                    }
                    else
                    {
                        SRInfoHelper.Log($"Failed to find m_SpawnDecks field via reflection.");
                    }

                    SRInfoHelper.Log($"Loaded and updated SpawnManager with {serializableData.Count} spawn cards from file");
                    return true;
                }
            }
            catch (System.Exception e)
            {
                SRInfoHelper.Log($"Error loading spawn cards: {e.Message}");
            }
        }
        SRInfoHelper.Log("No spawn cards found in file.");
        return false;
    }

    private void PopulateFromSpawnManager()
    {
        SpawnManager spawnManager = SpawnManager.Get();
        foreach (SpawnManager_EnemyEntry enemyEntry in spawnManager.m_EnemyDefinitions)
        {
            List<SpawnCard> cards = enemyEntry.CreateCards();
            _spawnDecks[enemyEntry.m_Group].AddRange(cards);
        }
        SRInfoHelper.Log($"Populated {_spawnDecks.Count} spawn cards from SpawnManager");
    }

    public List<SpawnCard> GetAllSpawnCards()
    {
        return _spawnDecks.SelectMany(kv => kv.Value).ToList();
    }

    public List<SpawnCard> GetSpawnCardsForGroup(GroupID groupId)
    {
        return _spawnDecks.TryGetValue(groupId, out var cards) ? cards : new List<SpawnCard>();
    }

    public void UpdateSpawnCard(SpawnCard updatedCard)
    {
        foreach (var deck in _spawnDecks.Values)
        {
            var existingCard = deck.FirstOrDefault(c => c.m_Enemy.m_UID == updatedCard.m_Enemy.m_UID);
            if (existingCard != null)
            {
                int index = deck.IndexOf(existingCard);
                deck[index] = updatedCard;
                break;
            }
        }
    }

    public void AddNewSpawnCard(GroupID groupId, SpawnCard newCard)
    {
        if (_spawnDecks.TryGetValue(groupId, out var deck))
        {
            deck.Add(newCard);
        }
    }

    public void RemoveSpawnCard(GroupID groupId, int cardUid)
    {
        if (_spawnDecks.TryGetValue(groupId, out var deck))
        {
            deck.RemoveAll(c => c.m_Enemy.m_UID == cardUid);
        }
    }

    public void SaveSpawnCardsToFile()
    {
        StartCoroutine(SaveSpawnCardsCoroutine());
    }
}

[System.Serializable]
public class SerializableEnemyEntry
{
    public GroupID GroupId;
    public int EnemyUID;
    public string EnemyName;
    public bool Spawnable;
    public bool Solo;
    public string m_Prefab_name;
    public bool UseWardrobeOverride;
    public WardrobeManager.WardrobeType WardrobeOverride;
    public List<WardrobeManager.WardrobeItemOverride> m_WardrobeItemOverrides = new List<WardrobeManager.WardrobeItemOverride>();
    public List<EnemyCountKey> m_CountKeys = new List<EnemyCountKey>();
    public List<SerializableModifierData> Modifiers;
    public List<WeaponType> WeaponOverrides;
    public List<int> ItemIds;
    public List<int> AbilityOverrides;
    public string m_TrackerIconSprite_name;

    public SerializableEnemyEntry() { }

    public SerializableEnemyEntry(SpawnManager_EnemyEntry enemy)
    {
        GroupId = enemy.m_Group;
        EnemyUID = enemy.m_UID;
        EnemyName = enemy.m_EnemyName;
        Spawnable = enemy.m_Spawnable;
        Solo = enemy.m_Solo;
        m_Prefab_name = enemy.m_Prefab ? enemy.m_Prefab.name : "";
        m_TrackerIconSprite_name = enemy.m_TrackerIconSprite ? enemy.m_TrackerIconSprite.name : "";
        UseWardrobeOverride = enemy.m_UseWardrobeOverride;
        WardrobeOverride = enemy.m_WardrobeOverride;
        m_WardrobeItemOverrides = new List<WardrobeManager.WardrobeItemOverride>(enemy.m_WardrobeItemOverrides);
        m_CountKeys = new List<EnemyCountKey>(enemy.m_CountKeys);
        Modifiers = enemy.m_Modifiers != null ?
                Array.ConvertAll(enemy.m_Modifiers, m => new SerializableModifierData(m)).ToList() :
                new List<SerializableModifierData>();
        WeaponOverrides = new List<WeaponType>(enemy.m_WeaponOverrides);
        ItemIds = new List<int>(enemy.m_ItemIds);
        AbilityOverrides = new List<int>(enemy.m_AbilityOverrides);
    }

    public SpawnManager_EnemyEntry ToEnemyEntry()
    {
        return new SpawnManager_EnemyEntry(EnemyUID)
        {
            m_Group = GroupId,
            m_EnemyName = EnemyName,
            m_Spawnable = Spawnable,
            m_Solo = Solo,
            //m_Prefab = _prefabs.FirstOrDefault(p => p.name == m_Prefab_name),
            //m_TrackerIconSprite = _prefabs.FirstOrDefault(p => p.name == m_TrackerIconSprite_name),
            m_UseWardrobeOverride = UseWardrobeOverride,
            m_WardrobeOverride = WardrobeOverride,
            m_WardrobeItemOverrides = m_WardrobeItemOverrides,
            m_CountKeys = m_CountKeys,
            m_Modifiers = Modifiers.Select(m => m.ToModifierData()).ToArray(),
            m_WeaponOverrides = WeaponOverrides,
            m_ItemIds = ItemIds,
            m_AbilityOverrides = AbilityOverrides
        };
    }
}

[System.Serializable]
public class SerializableSpawnCard
{
    [XmlAttribute]
    public GroupID GroupId;

    [XmlAttribute]
    public int EnemyUID;

    [XmlAttribute]
    public string EnemyName;

    [XmlAttribute]
    public float MinProgression;

    [XmlAttribute]
    public float MaxProgression;

    // Additional fields to capture more details from SpawnManager_EnemyEntry
    [XmlAttribute]
    public bool Spawnable;

    [XmlAttribute]
    public bool Solo;

    [XmlAttribute]
    public bool UseWardrobeOverride;

    [XmlAttribute]
    public WardrobeManager.WardrobeType WardrobeOverride;

    [XmlArray("Modifiers")]
    public List<SerializableModifierData> Modifiers;

    [XmlArray("WeaponOverrides")]
    public List<WeaponType> WeaponOverrides;

    [XmlArray("ItemIds")]
    public List<int> ItemIds;

    [XmlArray("AbilityOverrides")]
    public List<int> AbilityOverrides;
}


[System.Serializable]
[XmlRoot("SpawnCardData")]
public class SerializableSpawnCardData
{
    [XmlArray("Cards")]
    [XmlArrayItem("Card")]
    public List<SerializableSpawnCard> Cards = new List<SerializableSpawnCard>();
}

[System.Serializable]
public class SerializableSpawnCardList
{
    public GroupID groupId;
    public List<SerializableSpawnCard> cards;
}

//[System.Serializable]
//public class SerializableSpawnCardData
//{
//    public List<SerializableSpawnCardList> spawnCardLists;
//}