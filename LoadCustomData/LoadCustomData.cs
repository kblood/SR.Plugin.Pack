using SRMod.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using dto = SRMod.DTOs;

namespace LoadCustomData
{

    /// <summary>
    /// Example Satellite Reign LoadCustomData mod.
    /// </summary>
    public class LoadCustomData : ISrPlugin
    {
        /// <summary>
        /// Plugin initialization 
        /// </summary>
        public void Initialize()
        {
            Debug.Log("Initializing Satellite Reign LoadCustomData mod");
            SRInfoHelper.Log("Initializing Satellite Reign LoadCustomData mod");
            try
            {
                //var translations = FileManager.LoadTranslationsXML("Translations.xml");
                //var langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");

                //foreach (var e in translations)
                //{
                //    if (langLookup.ContainsKey(e.Key))
                //    {
                //        langLookup[e.Key] = e.Element;
                //    }
                //    else
                //    {
                //        langLookup.Add(e.Key, e.Element);
                //    }
                //}
                //SRInfoHelper.Log("Updated LangLookup with new translations");
                var sm = SpawnManager.Get();
                SRInfoHelper.Log($"Spawnmanager has {sm.m_EnemyDefinitions.Count} m_EnemyDefinitions");

                SpawnCardManager.Instance.Initialize();
                //if(!SpawnCardManager.Instance.CheckIfXMLFileExists())
                //    SpawnCardManager.Instance.SaveSpawnCardsToFile();
                

                var translations = TranslationManager.LoadTranslations();
                var langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");

                foreach (var kvp in translations)
                {
                    if (langLookup.ContainsKey(kvp.Key))
                    {
                        langLookup[kvp.Key] = kvp.Value;
                    }
                    else
                    {
                        langLookup.Add(kvp.Key, kvp.Value);
                    }
                }
                SRInfoHelper.Log("Updated LangLookup with new translations");

                ItemDataManager.Instance.Initialize();
                if (!File.Exists(Path.Combine(Manager.GetPluginManager().PluginPath, "itemDefinitions.xml")))
                    ItemDataManager.Instance.SaveItemDefinitionsToFile();
                /*
                var items = FileManager.LoadXML("ItemData.xml");

                SRInfoHelper.isLogging = false;

                var remappedItems = items.Select(d => SRMapper.ReflectionObjectBuilder<ItemManager.ItemData>(d)).ToList();

                Manager.GetItemManager().m_ItemDefinitions = remappedItems;
                */
                //Debug.Log("Updated LangLookup with new translations");
            }
            catch (Exception e)
            {
                SRInfoHelper.isLogging = true;
                SRInfoHelper.Log("Exception thrown while serializing: " + e.Message + " inner: " + e.InnerException);

                SRInfoHelper.isLogging = false;
                System.IO.Directory.CreateDirectory(FileManager.FilePathCheck($@"icons\"));
                //ExportData();
            }

            SRInfoHelper.isLogging = true;
            SRInfoHelper.Log("Initialized");
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            //if (Manager.Get().GameInProgress)
            {
                if (Input.GetKeyDown(KeyCode.Home))
                {
                    var areas= Manager.GetGlobalCompound().m_Areas;

                    MeshHelper.targetFolder = Manager.GetPluginManager().PluginPath;

                    //GameObject[] allRoot = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

                    /*
                    List<GameObject> allRoots = new List<GameObject>();
                    var scenes = UnityEngine.SceneManagement.SceneManager.GetAllScenes().ToList();

                    SRInfoHelper.Log("Scenes found total: " + scenes.Count);
                    SRInfoHelper.Log("Scenes found: " + scenes.Select(s => s.name).Aggregate((a,b) => a + ", "+ b));

                    for (int i = 0; i < UnityEngine.SceneManagement.SceneManager.sceneCount; i++)
                    {
                        var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(i);

                        if (!scenes.Contains(scene))
                        {
                            SRInfoHelper.Log("New scene found " + scene.name);

                            scenes.Add(scene);
                        }
                    }

                    foreach(var scene in scenes)
                    {
                        allRoots.AddRange(scene.GetRootGameObjects());
                    }

                    var meshFilters = MeshHelper.GetMeshfiltersFromTransforms(allRoots.ToArray()).ToList();

                    foreach(var filter in Manager.Get().transform.root.GetComponentsInChildren<MeshFilter>())
                    {
                        if (!meshFilters.Contains(filter))
                            meshFilters.Add(filter);
                    }

                    foreach (var filter in Camera.allCameras.First().transform.root.GetComponentsInChildren<MeshFilter>())
                    {
                        if (!meshFilters.Contains(filter))
                            meshFilters.Add(filter);
                    }

                    foreach (var filter in Manager.GetTrashMan().transform.root.GetComponentsInChildren<MeshFilter>())
                    {
                        if (!meshFilters.Contains(filter))
                            meshFilters.Add(filter);
                    }

                    if (!allRoots.Contains(Manager.Get().transform.root.gameObject))
                        allRoots.Add(Manager.Get().transform.root.gameObject);

                    if (!allRoots.Contains(Camera.allCameras.First().transform.root.gameObject))
                        allRoots.Add(Camera.allCameras.First().transform.root.gameObject);

                    if (!allRoots.Contains(Manager.GetTrashMan().transform.root.gameObject))
                        allRoots.Add(Manager.GetTrashMan().transform.root.gameObject);

                    SRInfoHelper.Log("Gameobjects found total: " + allRoots.Count);

                    SRInfoHelper.Log("Gameobjects found: " + allRoots.Select(s => s.name).Aggregate((a, b) => a + ", " + b));
                    */

                    //var filters = GameObject.FindObjectsOfType<MeshFilter>();

                    //MeshHelper.ExportSelectionToSeparate(filters);
                    //MeshHelper.ExportSelectionToSeparate(allRoots.ToArray());

                    //MeshHelper.ExportAllToSingle(meshFilters.ToArray());
                }

                if (Input.GetKeyDown(KeyCode.Insert))
                {
                    ItemDataManager.Instance.Initialize();
                    SpawnCardManager.Instance.Initialize();
                    //var newItems = Manager.GetItemManager().m_ItemDefinitions.Where(d => d.m_ID > 146).ToList();
                    //if (newItems.Any())
                    //{
                    //    foreach (var item in newItems)
                    //    {
                    //        item.PlayerHasPrototype = true;
                    //        item.m_Count = 1;
                    //    }
                    //}
                }
            }
        }

        //public void ExportData()
        //{
        //    try
        //    {
        //        var data = Manager.GetItemManager().m_ItemDefinitions.OrderBy(d => d.m_ID).Select(d => SRMapper.ReflectionObjectBuilder<dto.ItemData>(d)).ToList();
        //        var langLookup = TextManager.Get().GetFieldValue<Dictionary<string, TextManager.LocElement>>("m_FastLanguageLookup");

        //        var mappedTranslations = langLookup.OrderBy(l => l.Key).Select(l => new dto.TranslationElementDTO() { Key = l.Key, Element = l.Value }).ToList();

        //        try
        //        {
        //            FileManager.SaveAsXML(data, "ItemData.xml");
        //            FileManager.SaveAsXML(mappedTranslations, "Translations.xml");
        //        }
        //        catch (Exception e)
        //        {
        //            SRInfoHelper.Log("Exception thrown while serializing: " + e.Message);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        SRInfoHelper.isLogging = true;
        //        SRInfoHelper.Log("Exception thrown while mapping. Message: " + e.Message + " --InnerException: " + e.InnerException);
        //    }
        //}

        public string GetName()
        {
            return "Load Custom Data Mod.";
        }
    }
}