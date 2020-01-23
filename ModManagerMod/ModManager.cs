using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Example Satellite Reign ModManager mod.
/// </summary>
public class ModManager : ISrPlugin
{
    
    /// <summary>
    /// Plugin initialization 
    /// </summary>
    public void Initialize()
    {
        
        Debug.Log("Initializing Satellite Reign Mod Manager mod");

    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Pause))
        {
            var mods = Manager.GetPluginManager().LoadedPlugins.ToArray();

            if (mods.Count() > 1)
            {
                string output = @"";

                foreach (var mod in mods)
                {
                    if (mod.Plugin.GetName() != GetName())
                    {
                        Manager.GetPluginManager().LoadedPlugins.Remove(mod);
                        output += "Removed mod " + mod.Plugin.GetName() + ". ";
                    }
                }

                if(Manager.GetPluginManager().LoadedPlugins.Count() > 1)
                {
                    output += "Removed mod " + Manager.GetPluginManager().LoadedPlugins[1].Plugin.GetName() + ". ";
                    Manager.GetPluginManager().LoadedPlugins.RemoveAt(1);
                }

                Manager.GetUIManager().DoModalMessageBox("Removing mods", output, InputBoxUi.InputBoxTypes.MbOk);
            }
            else
            {
                Manager.GetPluginManager().LoadedPlugins.Clear();
                Manager.GetPluginManager().Start();

                mods = Manager.GetPluginManager().LoadedPlugins.ToArray();
                string output = @"";

                foreach (var mod in mods)
                {
                    if (mod.Plugin.GetName() != GetName())
                    {
                        //Manager.GetPluginManager().LoadedPlugins.Remove(mod);
                        output += "Loaded mod " + mod.Plugin.GetName() + ". ";
                    }
                }
                Manager.GetUIManager().DoModalMessageBox("Reloading mods", output, InputBoxUi.InputBoxTypes.MbOk);
            }

        }

        //if (Manager.Get().GameInProgress)
        //{

        //}
    }

    public string GetName()
    {
        return "Mod Manager.";
    }
}

