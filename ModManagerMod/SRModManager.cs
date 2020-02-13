using SRModManager.CustomUI;
using SRModManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SRModManager
{
    /// <summary>
    /// Example Satellite Reign ModManager mod.
    /// </summary>
    public class SRModManager : ISrPlugin
    {
        private bool active = false;

        private List<SrPlugin> m_LoadedPlugins = new List<SrPlugin>();

        private List<SrPlugin> m_activePlugins = new List<SrPlugin>();

        private bool loadedManagedPlugins = false;

        /// <summary>
        /// Plugin initialization 
        /// </summary>
        public void Initialize()
        {
            Debug.Log("Initializing Satellite Reign Mod Manager mod");
            /*
            UIHelper.ShowMessage("Initializing Satellite Reign Mod Manager mod");
            */
        }

        public List<SrPlugin> LoadedPlugins
        {
            get
            {
                return this.m_LoadedPlugins;
            }
        }

        public List<SrPlugin> ActivePlugins
        {
            get
            {
                return this.m_activePlugins;
            }
        }

        /// <summary>
        /// Called once per frame.
        /// </summary>
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Asterisk) || Input.GetKeyDown(KeyCode.KeypadMultiply))
            {
                //UIHelper.ShowMessage("Test?");
                if(!m_LoadedPlugins.Any())
                {
                    try
                    {
                        m_activePlugins.AddRange(Manager.GetPluginManager().LoadedPlugins.Where(p => p.Plugin.GetName() != GetName()));
                        var thisPlugin = Manager.GetPluginManager().LoadedPlugins.Where(p => p.Plugin.GetName() == GetName()).First();

                        m_LoadedPlugins.AddRange(m_activePlugins);

                        Manager.GetPluginManager().LoadedPlugins.Clear();
                        Manager.GetPluginManager().LoadedPlugins.Add(thisPlugin);
                    }
                    catch (Exception e)
                    {
                        UIHelper.ShowMessage("Exception thrown. Message: " + e);
                    }
                }
                // Takes all regular SR plugins / mods already loaded, except this one, and move them to this mod manager.

                ShowPluginManagerUI();

                // If there is any mods loaded, the unload them, otherwise load them.
                //if (m_LoadedPlugins.Any())
                //{
                //    // Make a string with all mod names
                //    string modsUnloaded = m_LoadedPlugins.Select(m => "Unloading " + m.Plugin.GetName()).Aggregate((x, y) => x + ". " + y);

                //    // unload
                //    m_LoadedPlugins.Clear();

                //    Manager.GetUIManager().ShowMessagePopup("--- Unload ManagedMods plugins. " + modsUnloaded, 10);
                //}
                //else
                //{
                //    //load
                //    LoadPlugins();

                //    // Make a string with all mod names
                //    string modsUnloaded = m_LoadedPlugins.Select(m => "Loading " + m.Plugin.GetName()).Aggregate((x, y) => x + ". " + y);

                //    Manager.GetUIManager().ShowMessagePopup("+++ Load ManagedMods plugins." + modsUnloaded, 10);
                //}
            }

            // Update all other plugins
            foreach (SrPlugin srPlugin in this.m_activePlugins)
            {
                srPlugin.Plugin.Update();
            }
        }

        public void ShowPluginManagerUI()
        {
            List<SRModButtonElement> buttons = new List<SRModButtonElement>();

            buttons.AddRange(m_LoadedPlugins.Select
                (
                    p =>
                    {
                        string buttonText = "Enable Plugin";
                        string buttonDescription = "Plugin: " + p.Plugin.GetName();
                        if (ActivePlugins.Contains(p))
                            buttonText = "Disable Plugin";
                        var button = new SRModButtonElement(buttonText,
                            delegate
                            {
                                var thisButton = UIHelper.VerticalButtonsUi.Buttons.Where(b => b.Description == "Plugin: " + p.Plugin.GetName()).First();

                                if (ActivePlugins.Contains(p))
                                {
                                    ActivePlugins.Remove(p);
                                    thisButton.Text.text = "Enable Plugin";
                                    UIHelper.ShowMessage("Disabled " + p.Plugin.GetName(), 10, 2);
                                }
                                else
                                {
                                    ActivePlugins.Add(p);
                                    thisButton.Text.text = "Disable Plugin";
                                    UIHelper.ShowMessage("Enabled " + p.Plugin.GetName(), 10, 2);
                                }
                                //thisButton.Text.SetAllDirty();
                                Canvas.ForceUpdateCanvases();
                            }
                        , buttonDescription);
                        return button;
                    }
                ).ToList());

            if (!loadedManagedPlugins)
            {
                if (Directory.Exists(this.PluginPath))
                {
                    buttons.Add(new SRModButtonElement("Load", delegate { LoadPlugins(); loadedManagedPlugins = true; ShowPluginManagerUI(); }, "Load managed plugins"));
                }
                else
                {
                    loadedManagedPlugins = true;
                }
            }

            Manager.Get().StartCoroutine(UIHelper.ModalVerticalButtonsRoutine("Plugin Manager", buttons));
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
}

