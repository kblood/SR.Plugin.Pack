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
        Debug.Log("Initializing Satellite Reign Mod Manager mod");
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
        if (Input.GetKeyDown(KeyCode.Asterisk))
        {
            // If there is any mods loaded, the unload them, otherwise load them.
            if (m_LoadedPlugins.Any())
            {
                // Make a string with all mod names
                string modsUnloaded = m_LoadedPlugins.Select(m => "Unloading " + m.Plugin.GetName()).Aggregate((x,y) => x +". " +y);

                // unload
                m_LoadedPlugins.Clear();
                
                Manager.GetUIManager().ShowMessagePopup("--- Unload ManagedMods plugins. " + modsUnloaded, 10);
            }
            else
            {
                //load
                LoadPlugins();

                // Make a string with all mod names
                string modsUnloaded = m_LoadedPlugins.Select(m => "Loading " + m.Plugin.GetName()).Aggregate((x, y) => x + ". " + y);

                Manager.GetUIManager().ShowMessagePopup("+++ Load ManagedMods plugins." + modsUnloaded, 10);
            }            
        }

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

