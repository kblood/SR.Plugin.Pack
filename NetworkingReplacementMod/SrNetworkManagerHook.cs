using System;
using System.Reflection;
using UnityEngine;
using NetworkingReplacementMod.Services;

namespace NetworkingReplacementMod
{
    /// <summary>
    /// Runtime hook that intercepts SrNetworkManager calls and redirects them to Mirror
    /// This component gets attached to the game and monitors for networking method calls
    /// </summary>
    public class SrNetworkManagerHook : MonoBehaviour
    {
        private static SrNetworkManagerHook _instance;
        private object _originalSrNetworkManager;
        private Type _srNetworkManagerType;
        private MirrorNetworkManager _mirrorManager;
        
        public static SrNetworkManagerHook Instance => _instance;
        
        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            _instance = this;
            DontDestroyOnLoad(gameObject);
            
            Debug.Log("[SrNetworkManagerHook] *** HOOK COMPONENT CREATED - READY TO INTERCEPT ***");
            FileManager.Log("SrNetworkManagerHook created and ready to intercept networking calls");
        }
        
        /// <summary>
        /// Initialize the hook with the Mirror manager and original SrNetworkManager
        /// </summary>
        public void Initialize(MirrorNetworkManager mirrorManager)
        {
            _mirrorManager = mirrorManager;
            
            try
            {
                // Find the original SrNetworkManager instance
                _srNetworkManagerType = Type.GetType("SrNetworkManager, Assembly-CSharp");
                if (!ReferenceEquals(_srNetworkManagerType, null))
                {
                    // Try to find the singleton instance
                    var singletonField = _srNetworkManagerType.GetField("singleton", BindingFlags.Public | BindingFlags.Static);
                    if (!ReferenceEquals(singletonField, null))
                    {
                        _originalSrNetworkManager = singletonField.GetValue(null);
                        FileManager.Log("Found original SrNetworkManager singleton instance");
                    }
                    
                    // Also try instance field
                    if (ReferenceEquals(_originalSrNetworkManager, null))
                    {
                        var instanceField = _srNetworkManagerType.GetField("_instance", BindingFlags.NonPublic | BindingFlags.Static);
                        if (!ReferenceEquals(instanceField, null))
                        {
                            _originalSrNetworkManager = instanceField.GetValue(null);
                            FileManager.Log("Found original SrNetworkManager _instance field");
                        }
                    }
                }
                
                if (!ReferenceEquals(_originalSrNetworkManager, null))
                {
                    FileManager.Log("SrNetworkManagerHook initialized successfully with original instance");
                    StartMonitoring();
                }
                else
                {
                    FileManager.LogError("Could not find original SrNetworkManager instance");
                }
            }
            catch (Exception e)
            {
                FileManager.LogException("SrNetworkManagerHook.Initialize", e);
            }
        }
        
        private void StartMonitoring()
        {
            // Start monitoring for networking calls
            FileManager.Log("Started monitoring for SrNetworkManager method calls");
            
            // Try to replace the DoStartHost method dynamically
            ReplaceDoStartHostMethod();
        }
        
        private void ReplaceDoStartHostMethod()
        {
            try
            {
                FileManager.Log("Attempting to replace DoStartHost method with Mirror implementation...");
                
                // Get the DoStartHost method
                var doStartHostMethod = _srNetworkManagerType.GetMethod("DoStartHost", BindingFlags.Public | BindingFlags.Instance);
                if (!ReferenceEquals(doStartHostMethod, null))
                {
                    FileManager.Log("Found DoStartHost method - will intercept calls");
                    
                    // Since we can't directly replace the method without Harmony,
                    // we'll use a different approach - monitor the network state
                    StartCoroutine(MonitorNetworkingAttempts());
                }
            }
            catch (Exception e)
            {
                FileManager.LogException("ReplaceDoStartHostMethod", e);
            }
        }
        
        private System.Collections.IEnumerator MonitorNetworkingAttempts()
        {
            FileManager.Log("Started coroutine to monitor networking attempts...");
            
            bool hasLoggedMonitoring = false;
            bool hasFoundInstance = false;
            
            while (true)
            {
                // First, try to find the SrNetworkManager instance if we don't have it
                if (ReferenceEquals(_originalSrNetworkManager, null) && !hasFoundInstance)
                {
                    TryFindSrNetworkManagerInstance();
                    if (!ReferenceEquals(_originalSrNetworkManager, null))
                    {
                        hasFoundInstance = true;
                        FileManager.Log("*** FOUND SrNetworkManager instance during monitoring! ***");
                    }
                }
                
                // Check networking state without try-catch to avoid coroutine issues
                bool networkingDetected = CheckForNetworkingActivity();
                
                if (networkingDetected)
                {
                    FileManager.Log("*** DETECTED NETWORKING ACTIVITY - INTERCEPTING WITH MIRROR ***");
                    InterceptWithMirror();
                    yield return new WaitForSeconds(5.0f); // Wait longer after interception
                }
                else
                {
                    if (!hasLoggedMonitoring)
                    {
                        if (ReferenceEquals(_originalSrNetworkManager, null))
                        {
                            FileManager.Log("Monitoring for SrNetworkManager instance creation and networking attempts...");
                        }
                        else
                        {
                            FileManager.Log("Monitoring SrNetworkManager for networking attempts...");
                        }
                        hasLoggedMonitoring = true;
                    }
                    yield return new WaitForSeconds(0.5f); // Check twice per second
                }
            }
        }
        
        private void TryFindSrNetworkManagerInstance()
        {
            try
            {
                if (!ReferenceEquals(_srNetworkManagerType, null))
                {
                    // Try different ways to find the instance
                    
                    // Method 1: Try static singleton field
                    var singletonField = _srNetworkManagerType.GetField("singleton", BindingFlags.Public | BindingFlags.Static);
                    if (!ReferenceEquals(singletonField, null))
                    {
                        _originalSrNetworkManager = singletonField.GetValue(null);
                        if (!ReferenceEquals(_originalSrNetworkManager, null))
                        {
                            FileManager.Log("Found SrNetworkManager via singleton field");
                            return;
                        }
                    }
                    
                    // Method 2: Try private _instance field
                    var instanceField = _srNetworkManagerType.GetField("_instance", BindingFlags.NonPublic | BindingFlags.Static);
                    if (!ReferenceEquals(instanceField, null))
                    {
                        _originalSrNetworkManager = instanceField.GetValue(null);
                        if (!ReferenceEquals(_originalSrNetworkManager, null))
                        {
                            FileManager.Log("Found SrNetworkManager via _instance field");
                            return;
                        }
                    }
                    
                    // Method 3: Try FindObjectOfType
                    var foundObject = GameObject.FindObjectOfType(_srNetworkManagerType);
                    if (!ReferenceEquals(foundObject, null))
                    {
                        _originalSrNetworkManager = foundObject;
                        FileManager.Log("Found SrNetworkManager via FindObjectOfType");
                        return;
                    }
                }
            }
            catch (Exception e)
            {
                FileManager.LogException("TryFindSrNetworkManagerInstance", e);
            }
        }
        
        private bool CheckForNetworkingActivity()
        {
            try
            {
                if (!ReferenceEquals(_originalSrNetworkManager, null) && !ReferenceEquals(_srNetworkManagerType, null))
                {
                    // Check various network state properties to detect when hosting is attempted
                    var isServerProperty = _srNetworkManagerType.GetProperty("isServer", BindingFlags.Public | BindingFlags.Instance);
                    var isClientProperty = _srNetworkManagerType.GetProperty("isClient", BindingFlags.Public | BindingFlags.Instance);
                    var isNetworkActiveProperty = _srNetworkManagerType.GetProperty("isNetworkActive", BindingFlags.Public | BindingFlags.Instance);
                    
                    if (!ReferenceEquals(isServerProperty, null) && !ReferenceEquals(isClientProperty, null) && !ReferenceEquals(isNetworkActiveProperty, null))
                    {
                        var isServer = (bool)isServerProperty.GetValue(_originalSrNetworkManager);
                        var isClient = (bool)isClientProperty.GetValue(_originalSrNetworkManager);
                        var isNetworkActive = (bool)isNetworkActiveProperty.GetValue(_originalSrNetworkManager);
                        
                        // If we detect networking activity, return true
                        if (isNetworkActive || isServer || isClient)
                        {
                            FileManager.Log("Original SrNetworkManager state - Server: " + isServer + ", Client: " + isClient + ", Active: " + isNetworkActive);
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                FileManager.LogException("CheckForNetworkingActivity", e);
            }
            
            return false;
        }
        
        private void InterceptWithMirror()
        {
            try
            {
                FileManager.Log("INTERCEPT SUCCESS: Redirecting to Mirror networking implementation");
                Debug.Log("[SrNetworkManagerHook] *** INTERCEPTED NETWORKING CALL - USING MIRROR ***");
                
                if (!ReferenceEquals(_mirrorManager, null))
                {
                    FileManager.Log("Starting Mirror host instead of UNet...");
                    bool success = _mirrorManager.StartHost();
                    
                    if (success)
                    {
                        FileManager.Log("MIRROR SUCCESS: Host started successfully via interception!");
                        Debug.Log("[SrNetworkManagerHook] *** MIRROR HOST STARTED SUCCESSFULLY ***");
                    }
                    else
                    {
                        FileManager.LogError("Mirror host start failed");
                    }
                }
                else
                {
                    FileManager.LogError("Mirror manager is null - cannot intercept");
                }
            }
            catch (Exception e)
            {
                FileManager.LogException("InterceptWithMirror", e);
            }
        }
        
        void OnDestroy()
        {
            if (_instance == this)
            {
                _instance = null;
                FileManager.Log("SrNetworkManagerHook destroyed");
            }
        }
    }
}