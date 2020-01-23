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

    private List<SrPlugin> m_LoadedPlugins = new List<SrPlugin>();

    /// <summary>
    /// Plugin initialization 
    /// </summary>
    public void Initialize()
    {
        Start();
    }

    public List<SrPlugin> LoadedPlugins
    {
        get
        {
            return this.m_LoadedPlugins;
        }
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {        
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (active){
                // unload
                active = false;
                m_LoadedPlugins = new List<SrPlugin>();
                
                Manager.GetUIManager().ShowMessagePopup("--- unload ManagedMods plugins");

            }
            else{
                //load
                this.Start();
                Manager.GetUIManager().ShowMessagePopup("+++ load ManagedMods plugins");
            }            
        }

        if (!active)
            return;

        foreach (SrPlugin srPlugin in this.LoadedPlugins)
        {
            srPlugin.Plugin.Update();
        }


    }

    public string GetName()
    {
        return "Mod Manager.";
    }


    public string PluginPath
    {
        get
        {
            return Directory.GetCurrentDirectory() + "\\Mods\\ManagedMods";
        }
    }

    public void Start()
    {        
        this.LoadPlugins();
        active = true;
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

    
    public void OnApplicationQuit()
    {
        Debug.Log("Releasing all plugins objects");
        this.LoadedPlugins.Clear();
    }

    

}

