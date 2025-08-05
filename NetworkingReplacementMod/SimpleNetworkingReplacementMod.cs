using System;
using UnityEngine;

namespace NetworkingReplacementMod
{
    /// <summary>
    /// Simplified Networking Replacement Mod - Compatible with Satellite Reign's .NET 2.0 runtime
    /// </summary>
    public class SimpleNetworkingReplacementMod : ISrPlugin
    {
        private bool isInitialized = false;
        private float lastLogTime = 0f;

        public void Initialize()
        {
            try
            {
                Debug.Log("NetworkingReplacementMod: Simple compatibility mod initializing...");
                
                // Basic initialization - no complex operations
                isInitialized = true;
                
                Debug.Log("NetworkingReplacementMod: Initialized successfully (compatible mode)");
                
                // Show warning to player if possible
                try
                {
                    if (Manager.Get() != null && Manager.GetUIManager() != null)
                    {
                        Manager.GetUIManager().ShowWarningPopup(
                            "NETWORKING WARNING\n\nMultiplayer may be affected by Unity UNet deprecation.\nSingle-player mode is fully supported.",
                            8);
                    }
                }
                catch
                {
                    // Ignore UI errors - this is optional
                }
            }
            catch (Exception e)
            {
                Debug.LogError("NetworkingReplacementMod: Initialization failed: " + e.Message);
            }
        }

        public void Update()
        {
            if (!isInitialized)
                return;

            try
            {
                // Simple periodic logging (every 30 seconds)
                if (Time.time > lastLogTime + 30f)
                {
                    Debug.Log("NetworkingReplacementMod: Running in compatibility mode");
                    lastLogTime = Time.time;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("NetworkingReplacementMod: Update error: " + e.Message);
            }
        }

        public string GetName()
        {
            return "Simple Networking Replacement Mod v1.0";
        }
    }
}