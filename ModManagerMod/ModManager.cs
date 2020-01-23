using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Example Satellite Reign ModManager mod.
/// </summary>
public class ModManager : ISrPlugin
{

    private bool active = false;

    /// <summary>
    /// Plugin initialization 
    /// </summary>
    public void Initialize()
    {

        Debug.Log("Initializing Satellite Reign Mod Manager mod");

        if (!active)
            active = true;
        else
            active = false;
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
        if (!active)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Pause))
        {
            var mods = LoadedPlugins.ToList();

            if (LoadedPlugins.Count() > 0)
            {
                string output = @"";

                foreach (var mod in LoadedPlugins)
                {
                    mod.Plugin.Initialize();
                    //LoadedPlugins.Remove(mod);
                    output += "Removing mod " + mod.Plugin.GetName() + ". ";

                    //if (mod.Plugin.GetName() != GetName())
                    //{
                    //    LoadedPlugins.Remove(mod);
                    //    output += "Removed mod " + mod.Plugin.GetName() + ". ";
                    //}
                }

                //if (LoadedPlugins.Count() > 1)
                //{
                //    output += "Removed mod " + LoadedPlugins[1].Plugin.GetName() + ". ";
                //    LoadedPlugins.RemoveAt(1);
                //}

                Manager.GetUIManager().DoModalMessageBox("Removing mods", output, InputBoxUi.InputBoxTypes.MbOk);

                for (int i = 0; i < LoadedPlugins.Count; i++)
                {
                    LoadedPlugins[i] = null;
                }
                LoadedPlugins.Clear();

                Manager.GetUIManager().ShowMessagePopup("Mods removed", 10);
            }
            else
            {
                Start();

                //mods = LoadedPlugins.ToArray();
                string output = @"";

                foreach (var mod in LoadedPlugins)
                {
                    if (mod.Plugin.GetName() != GetName())
                    {
                        //Manager.GetPluginManager().LoadedPlugins.Remove(mod);
                        output += "Loaded mod " + mod.Plugin.GetName() + ". ";
                    }
                }
                Manager.GetUIManager().DoModalMessageBox("Reloaded mods", output, InputBoxUi.InputBoxTypes.MbOk);
            }
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            active = false;

            for (int i = 0; i < LoadedPlugins.Count; i++)
            {
                LoadedPlugins[i] = null;
            }
            LoadedPlugins.Clear();

            for (int i = 0; i < Manager.GetPluginManager().LoadedPlugins.Count; i++)
            {
                Manager.GetPluginManager().LoadedPlugins[i] = null;
            }

            Manager.GetPluginManager().LoadedPlugins.Clear();

            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();

            Manager.GetUIManager().ShowMessagePopup("Burned all bridges... to mods...", 10);

            Manager.GetPluginManager().Start();

            Manager.GetUIManager().ShowWarningPopup("Reloaded mods", 10);
        }

        foreach (SrPlugin srPlugin in this.LoadedPlugins)
        {
            srPlugin.Plugin.Update();
        }

        //if (Manager.Get().GameInProgress)
        //{

        //}
    }

    public string GetName()
    {
        return "Mod Manager.";
    }


    // Token: 0x17000541 RID: 1345
    // (get) Token: 0x06003591 RID: 13713 RVA: 0x00166B04 File Offset: 0x00164D04
    public string PluginPath
    {
        get
        {
            return Directory.GetCurrentDirectory() + "\\Mods\\ManagedMods";
        }
    }

    // Token: 0x17000542 RID: 1346
    // (get) Token: 0x06003592 RID: 13714 RVA: 0x00166B18 File Offset: 0x00164D18
    public List<SrPlugin> LoadedPlugins
    {
        get
        {
            return this.m_LoadedPlugins;
        }
    }

    // Token: 0x06003593 RID: 13715 RVA: 0x00166B20 File Offset: 0x00164D20
    public void Start()
    {
        this.LoadPlugins();
    }

    // Token: 0x06003594 RID: 13716 RVA: 0x00166B28 File Offset: 0x00164D28
    private void LoadPlugins()
    {
        Debug.LogFormat("Searching for mods in folder {0}", new object[]
        {
            this.PluginPath
        });
        if (Directory.Exists(this.PluginPath))
        {
            string[] files = Directory.GetFiles(this.PluginPath, "*.dll");
            foreach (string pluginFileName in files)
            {
                this.LoadPlugin(pluginFileName);
            }
        }
    }

    // Token: 0x06003595 RID: 13717 RVA: 0x00166B90 File Offset: 0x00164D90
    private void LoadPlugin(string _pluginFileName)
    {
        Assembly assembly = Assembly.LoadFile(_pluginFileName);
        foreach (Type type in assembly.GetExportedTypes())
        {
            if (typeof(ISrPlugin).IsAssignableFrom(type) && !type.IsAbstract && !type.IsInterface)
            {
                Debug.LogFormat("Loading plugin type {0} from {1}", new object[]
                {
                    type,
                    _pluginFileName
                });
                try
                {
                    ISrPlugin srPlugin = Activator.CreateInstance(type) as ISrPlugin;

                    if (srPlugin != null)
                    {
                        this.LoadedPlugins.Add(new SrPlugin(srPlugin, _pluginFileName));
                        srPlugin.Initialize();
                    }
                    else
                    {
                        Debug.LogErrorFormat("CreateInstance failed for type {0} from {1}", new object[]
                        {
                            type,
                            _pluginFileName
                        });
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("Error initializing {0} from {1}", new object[]
                    {
                        type,
                        _pluginFileName
                    });
                }
            }
        }
    }

    // Token: 0x06003596 RID: 13718 RVA: 0x00166C88 File Offset: 0x00164E88
    public void OnApplicationQuit()
    {
        Debug.Log("Releasing all plugins objects");
        this.LoadedPlugins.Clear();
    }

    // Token: 0x06003597 RID: 13719 RVA: 0x00166CA0 File Offset: 0x00164EA0
    //public void Update()
    //{
        
    //}

    // Token: 0x04002E1B RID: 11803
    private const string PluginFolder = "Mods";

    // Token: 0x04002E1C RID: 11804
    private readonly List<SrPlugin> m_LoadedPlugins = new List<SrPlugin>();
}

