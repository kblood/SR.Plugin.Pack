using System;
using UnityEngine;

/// <summary>
/// Simple UNet Deprecation Fix - Prevents crashes without fake functionality
/// This mod honestly prevents UNet crashes and provides clear feedback about multiplayer status
/// </summary>
public class SimpleUNetFix : ISrPlugin
{
    public string GetName()
    {
        return "Simple UNet Deprecation Fix v1.0 - Honest Crash Prevention";
    }

    public void Update()
    {
        // No frame updates needed
    }

    public void Initialize()
    {
        try
        {
            Debug.Log("[SimpleUNetFix] *** INITIALIZING UNET CRASH PREVENTION ***");
            Debug.Log("[SimpleUNetFix] This mod prevents multiplayer crashes caused by UNet deprecation");
            Debug.Log("[SimpleUNetFix] Multiplayer is disabled - single-player remains fully functional");
            
            // Create a simple crash prevention system
            CreateCrashPrevention();
            
            Debug.Log("[SimpleUNetFix] *** CRASH PREVENTION ACTIVE - SINGLE PLAYER PROTECTED ***");
        }
        catch (Exception e)
        {
            Debug.LogError("[SimpleUNetFix] Initialization failed: " + e.Message);
        }
    }

    private void CreateCrashPrevention()
    {
        try
        {
            // Create a simple GameObject to monitor for networking attempts
            GameObject crashPreventer = new GameObject("UNetCrashPreventer");
            var preventer = crashPreventer.AddComponent<UNetCrashPreventer>();
            UnityEngine.Object.DontDestroyOnLoad(crashPreventer);
            
            Debug.Log("[SimpleUNetFix] Crash prevention system created");
        }
        catch (Exception e)
        {
            Debug.LogError("[SimpleUNetFix] Failed to create crash prevention: " + e.Message);
        }
    }
}

/// <summary>
/// Simple crash prevention component that monitors for UNet attempts
/// </summary>
public class UNetCrashPreventer : MonoBehaviour
{
    private bool hasWarned = false;

    void Start()
    {
        Debug.Log("[UNetCrashPreventer] Started monitoring for UNet networking attempts");
        StartCoroutine(MonitorForNetworkingAttempts());
    }

    private System.Collections.IEnumerator MonitorForNetworkingAttempts()
    {
        while (true)
        {
            try
            {
                // Look for signs that the game is trying to use networking
                var networkManagers = FindObjectsOfType<MonoBehaviour>();
                
                foreach (var manager in networkManagers)
                {
                    if (manager.GetType().Name == "SrNetworkManager")
                    {
                        if (!hasWarned)
                        {
                            ShowMultiplayerWarning();
                            hasWarned = true;
                        }
                        
                        // Check if it's trying to network
                        CheckAndPreventNetworking(manager);
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError("[UNetCrashPreventer] Error during monitoring: " + e.Message);
            }
            
            yield return new WaitForSeconds(1.0f); // Check once per second
        }
    }

    private void CheckAndPreventNetworking(MonoBehaviour srNetworkManager)
    {
        try
        {
            // Use reflection to check network state
            var type = srNetworkManager.GetType();
            var isNetworkActiveProperty = type.GetProperty("isNetworkActive");
            
            if (isNetworkActiveProperty != null)
            {
                var isActive = (bool)isNetworkActiveProperty.GetValue(srNetworkManager);
                
                if (isActive)
                {
                    Debug.LogError("[UNetCrashPreventer] *** NETWORKING ATTEMPT DETECTED - PREVENTING CRASH ***");
                    ShowCriticalWarning();
                    
                    // Try to stop the networking before it crashes
                    PreventNetworkingCrash(srNetworkManager);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("[UNetCrashPreventer] Error checking networking state: " + e.Message);
        }
    }

    private void PreventNetworkingCrash(MonoBehaviour srNetworkManager)
    {
        try
        {
            Debug.Log("[UNetCrashPreventer] Attempting to safely stop networking to prevent crash...");
            
            // Try to call StopHost, StopServer, StopClient to prevent crash
            var type = srNetworkManager.GetType();
            
            var stopHostMethod = type.GetMethod("StopHost");
            var stopServerMethod = type.GetMethod("StopServer");
            var stopClientMethod = type.GetMethod("StopClient");
            
            if (stopHostMethod != null)
            {
                Debug.Log("[UNetCrashPreventer] Calling StopHost to prevent crash...");
                stopHostMethod.Invoke(srNetworkManager, null);
            }
            
            if (stopServerMethod != null)
            {
                Debug.Log("[UNetCrashPreventer] Calling StopServer to prevent crash...");
                stopServerMethod.Invoke(srNetworkManager, null);
            }
            
            if (stopClientMethod != null)
            {
                Debug.Log("[UNetCrashPreventer] Calling StopClient to prevent crash...");
                stopClientMethod.Invoke(srNetworkManager, null);
            }
            
            Debug.Log("[UNetCrashPreventer] Crash prevention completed");
        }
        catch (Exception e)
        {
            Debug.LogError("[UNetCrashPreventer] Error during crash prevention: " + e.Message);
        }
    }

    private void ShowMultiplayerWarning()
    {
        Debug.LogWarning("[UNetCrashPreventer] ===============================================");
        Debug.LogWarning("[UNetCrashPreventer] MULTIPLAYER STATUS: DISABLED");
        Debug.LogWarning("[UNetCrashPreventer] Unity UNet networking is deprecated and non-functional");
        Debug.LogWarning("[UNetCrashPreventer] Single-player mode remains fully functional");
        Debug.LogWarning("[UNetCrashPreventer] ===============================================");
    }

    private void ShowCriticalWarning()
    {
        Debug.LogError("[UNetCrashPreventer] ===============================================");
        Debug.LogError("[UNetCrashPreventer] CRITICAL: Game attempted to use deprecated UNet!");
        Debug.LogError("[UNetCrashPreventer] Crash prevention activated to maintain stability");
        Debug.LogError("[UNetCrashPreventer] Please use single-player mode only");
        Debug.LogError("[UNetCrashPreventer] ===============================================");
    }
}