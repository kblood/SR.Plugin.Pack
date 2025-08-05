using System;
using UnityEngine;
using NetworkingReplacementMod.Services;

namespace NetworkingReplacementMod
{
    /// <summary>
    /// Main entry point for the Networking Replacement Mod
    /// Initializes the UNet compatibility layer to handle deprecated networking
    /// </summary>
    public class NetworkingReplacementMod : ISrPlugin
    {
        private bool _hasInitialized = false;

        public void Initialize()
        {
            try
            {
                Debug.Log("[NetworkingReplacementMod] Starting UNet compatibility layer initialization...");
                FileManager.isLogging = true;
                FileManager.Log("NetworkingReplacementMod initializing.");

                // Initialize the UNet compatibility layer
                UNetCompatibilityLayer.Initialize();

                // Check what backend we're using
                var backend = UNetCompatibilityLayer.GetCurrentBackend();
                Debug.Log("[NetworkingReplacementMod] Using networking backend: " + backend.ToString());
                FileManager.Log("NetworkingReplacementMod detected backend: " + backend.ToString());

                if (UNetCompatibilityLayer.IsFallbackMode())
                {
                    Debug.LogWarning("[NetworkingReplacementMod] === MULTIPLAYER WARNING ===");
                    Debug.LogWarning("[NetworkingReplacementMod] Running in fallback mode - multiplayer is DISABLED");
                    Debug.LogWarning("[NetworkingReplacementMod] This prevents crashes but multiplayer will not work");
                    Debug.LogWarning("[NetworkingReplacementMod] Single-player mode continues to work normally");
                    Debug.LogWarning("[NetworkingReplacementMod] ================================");
                }
                else
                {
                    Debug.Log("[NetworkingReplacementMod] Multiplayer available with " + backend.ToString() + " backend");
                }

                _hasInitialized = true;
                Debug.Log("[NetworkingReplacementMod] Initialization completed successfully");
                FileManager.Log("NetworkingReplacementMod initialization complete.");
            }
            catch (Exception ex)
            {
                Debug.LogError("[NetworkingReplacementMod] Failed to initialize: " + ex.Message);
                Debug.LogError("[NetworkingReplacementMod] Stack trace: " + ex.StackTrace);
                FileManager.LogException("NetworkingReplacementMod.Initialize", ex);
            }
        }

        public void Update()
        {
            if (!_hasInitialized)
                return;

            // Monitor networking state and provide warnings if needed
            MonitorNetworkingAttempts();
        }

        private void MonitorNetworkingAttempts()
        {
            // Check periodically if user is trying to access multiplayer features
            if (Time.time % 60f < Time.deltaTime) // Every minute
            {
                if (UNetCompatibilityLayer.IsFallbackMode())
                {
                    FileManager.LogVerbose("Reminder: Running in fallback mode - multiplayer disabled");
                }
            }
        }

        public string GetName()
        {
            if (!_hasInitialized)
                return "Networking Replacement Mod (Initializing...)";

            var backend = UNetCompatibilityLayer.GetCurrentBackend();
            bool isMultiplayerAvailable = UNetCompatibilityLayer.IsMultiplayerAvailable();
            
            string mpStatus = isMultiplayerAvailable ? "ON" : "OFF";
            return "Networking Replacement Mod v1.0 (" + backend.ToString() + " - MP: " + mpStatus + ")";
        }
    }
}