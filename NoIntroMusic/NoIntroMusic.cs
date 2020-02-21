using UnityEngine;

/// <summary>
/// Satellite Reign NoIntroMusic mod.
/// </summary>
public class NoIntroMusic : ISrPlugin
{
    /// <summary>
    /// Plugin initialization 
    /// </summary>
    public void Initialize()
    {
        Debug.Log("Initializing Satellite Reign NoIntroMusic mod");
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
        if (Manager.GetAudioManager().IsLoginMusicPlaying())
        {
            Manager.GetAudioManager().StopAllMusic(true);
        }
    }

    public string GetName()
    {
        return "No intro music.";
    }
}

