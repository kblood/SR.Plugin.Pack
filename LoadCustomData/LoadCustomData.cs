using SRMod.Services;
using LoadCustomData.Services;
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
                // Initialize the comprehensive data manager
                DataExportImportManager.Instance.Initialize();

                // Check spawn manager
                var sm = SpawnManager.Get();
                SRInfoHelper.Log("Spawnmanager has " + sm.m_EnemyDefinitions.Count + " m_EnemyDefinitions");

                // Load existing translations
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

                // Auto-export data if files don't exist
                DataExportImportManager.Instance.ExportAllGameData();

                SRInfoHelper.Log("LoadCustomData mod initialization complete - comprehensive data management enabled");
            }
            catch (Exception e)
            {
                SRInfoHelper.isLogging = true;
                SRInfoHelper.Log("Exception thrown while initializing: " + e.Message + " inner: " + e.InnerException);

                SRInfoHelper.isLogging = false;
                System.IO.Directory.CreateDirectory(FileManager.FilePathCheck(@"icons\"));
                
                // Fallback initialization
                try
                {
                    ItemDataManager.Instance.Initialize();
                    QuestDataManager.Instance.Initialize();
                    SpawnCardManager.Instance.Initialize();
                }
                catch (Exception fallbackEx)
                {
                    SRInfoHelper.Log("Fallback initialization failed: " + fallbackEx.Message);
                }
            }

            SRInfoHelper.isLogging = true;
            SRInfoHelper.Log("LoadCustomData mod initialized");
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
                    // Reinitialize all data managers
                    SRInfoHelper.Log("Manual reinitialization triggered");
                    DataExportImportManager.Instance.Initialize();
                    Manager.GetUIManager()?.ShowMessagePopup("Data managers reinitialized!", 3);
                }

                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    // Export all data manually
                    SRInfoHelper.Log("Manual comprehensive data export triggered");
                    DataExportImportManager.Instance.ExportAllGameData();
                    Manager.GetUIManager()?.ShowMessagePopup("All game data exported!", 3);
                }

                if (Input.GetKeyDown(KeyCode.End))
                {
                    // Import all data manually
                    SRInfoHelper.Log("Manual comprehensive data import triggered");
                    DataExportImportManager.Instance.ImportAllGameData();
                    Manager.GetUIManager()?.ShowMessagePopup("All game data imported!", 3);
                }

                if (Input.GetKeyDown(KeyCode.PageUp))
                {
                    // Create data backup
                    SRInfoHelper.Log("Manual data backup triggered");
                    DataExportImportManager.Instance.CreateDataBackup();
                    Manager.GetUIManager()?.ShowMessagePopup("Data backup created!", 3);
                }

                if (Input.GetKeyDown(KeyCode.PageDown))
                {
                    // Validate exported data
                    SRInfoHelper.Log("Manual data validation triggered");
                    bool isValid = DataExportImportManager.Instance.ValidateExportedData();
                    string message = isValid ? "Data validation passed!" : "Data validation failed!";
                    Manager.GetUIManager()?.ShowMessagePopup(message, 3);
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