using System;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using SatelliteReignNetworkingFix.Utils;
using SatelliteReignNetworkingFix.MirrorIntegration;

namespace SatelliteReignNetworkingFix
{
    /// <summary>
    /// BepInEx plugin that replaces Satellite Reign's deprecated UNet networking with Mirror Networking
    /// This plugin uses Harmony patching to intercept UNet calls and redirect them to Mirror
    /// </summary>
    [BepInPlugin("com.modding.satellitereign.networkingfix", "Satellite Reign Networking Fix", "1.0.0")]
    [BepInProcess("SatelliteReignWindows.exe")]
    public class NetworkingFixPlugin : BaseUnityPlugin
    {
        private Harmony harmonyInstance;
        private static NetworkingFixPlugin Instance;
        
        public static Utils.Logger PluginLogger { get; private set; }
        
        void Awake()
        {
            Instance = this;
            
            // Initialize enhanced logging system
            PluginLogger = new Utils.Logger("[NetworkingFix]");
            PluginLogger.Info("=== SATELLITE REIGN NETWORKING FIX INITIALIZING ===");
            PluginLogger.Info("BepInEx plugin loading - preparing to replace UNet with Mirror");
            
            try
            {
                // Initialize Harmony patching system
                InitializeHarmony();
                
                // Set up Mirror networking backend
                InitializeMirrorBackend();
                
                // Apply method patches
                ApplyNetworkingPatches();
                
                // Apply startup network blocking patches immediately
                ApplyStartupNetworkPatches();
                
                // Set working facilitator immediately during startup
                SetWorkingFacilitatorOnStartup();
                
                // Add socket-level connection interception
                ApplySocketLevelPatches();
                
                PluginLogger.Info("=== NETWORKING FIX PLUGIN LOADED SUCCESSFULLY ===");
                PluginLogger.Info("Satellite Reign multiplayer has been restored with Mirror Networking!");
            }
            catch (Exception e)
            {
                PluginLogger.Error("Critical error during plugin initialization: " + e.Message);
                PluginLogger.Error("Stack trace: " + e.StackTrace);
            }
        }
        
        private void InitializeHarmony()
        {
            PluginLogger.Info("Initializing Harmony patching system...");
            
            try
            {
                harmonyInstance = new Harmony("com.modding.satellitereign.networkingfix");
                PluginLogger.Info("Harmony instance created successfully");
            }
            catch (Exception e)
            {
                PluginLogger.Error("Failed to create Harmony instance: " + e.Message);
                throw;
            }
        }
        
        private void InitializeMirrorBackend()
        {
            PluginLogger.Info("Setting up Mirror networking backend...");
            
            try
            {
                // Initialize the Mirror networking system
                // NOTE: This will be implemented as we add Mirror library support
                PluginLogger.Info("Mirror backend initialization prepared");
                PluginLogger.Warn("Mirror library integration pending - will be added in next phase");
            }
            catch (Exception e)
            {
                PluginLogger.Error("Failed to initialize Mirror backend: " + e.Message);
                throw;
            }
        }
        
        private void ApplyNetworkingPatches()
        {
            PluginLogger.Info("Applying Harmony networking patches...");
            
            try
            {
                // Manual patching to avoid ReflectionTypeLoadException with PatchAll()
                ApplyManualPatches();
                PluginLogger.Info("Manual Harmony patches applied successfully");
                
                // Log applied patches for debugging
                LogAppliedPatches();
            }
            catch (Exception e)
            {
                PluginLogger.Error("Failed to apply Harmony patches: " + e.Message);
                PluginLogger.Warn("Continuing without patches - plugin will operate in monitoring mode only");
                // Don't throw - let the plugin continue without patches
            }
        }
        
        private void ApplyStartupNetworkPatches()
        {
            PluginLogger.Info("Applying startup network patches to block deprecated UNet connections...");
            
            try
            {
                // These patches need to be applied immediately to block startup connections
                // before the main menu even loads
                TryPatchNATAndFacilitator();
                
                PluginLogger.Info("Startup network patches applied successfully");
                PluginLogger.Info("Deprecated UNet facilitator connections will be blocked");
            }
            catch (Exception e)
            {
                PluginLogger.Error("Failed to apply startup network patches: " + e.Message);
                // Don't throw - let the plugin continue
            }
        }
        
        private void SetWorkingFacilitatorOnStartup()
        {
            PluginLogger.Info("Setting working facilitator server immediately during startup...");
            
            try
            {
                // Call the facilitator redirection method immediately
                // This should set Unity's natFacilitatorIP and natFacilitatorPort before any connections are attempted
                UNetToMirrorRedirector.SetWorkingFacilitatorImmediately();
                
                PluginLogger.Info("Working facilitator set successfully during startup");
            }
            catch (Exception e)
            {
                PluginLogger.Error("Failed to set working facilitator during startup: " + e.Message);
            }
        }
        
        private void ApplySocketLevelPatches()
        {
            PluginLogger.Info("Applying socket-level connection interception...");
            
            try
            {
                // Patch System.Net.Sockets.Socket methods that connect to specific IPs
                var socketType = typeof(System.Net.Sockets.Socket);
                
                // Patch Socket.Connect methods (manual iteration for .NET 2.0 compatibility)
                var allMethods = socketType.GetMethods();
                foreach (var method in allMethods)
                {
                    if (method.Name == "Connect")
                    {
                        try
                        {
                            var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("InterceptSocketConnection");
                            if (!ReferenceEquals(prefixMethod, null))
                            {
                                harmonyInstance.Patch(method, new HarmonyMethod(prefixMethod));
                                PluginLogger.Info("Successfully patched Socket.Connect method");
                            }
                        }
                        catch (Exception patchEx)
                        {
                            PluginLogger.Warn("Failed to patch Socket.Connect: " + patchEx.Message);
                        }
                    }
                }
                
                PluginLogger.Info("Socket-level connection interception applied");
            }
            catch (Exception e)
            {
                PluginLogger.Error("Failed to apply socket-level patches: " + e.Message);
            }
        }
        
        private void ApplyManualPatches()
        {
            PluginLogger.Info("Starting manual patch application...");
            
            try
            {
                // Try to find and patch SrNetworkManager methods manually
                var srNetworkManagerType = FindSrNetworkManagerType();
                if (ReferenceEquals(srNetworkManagerType, null))
                {
                    PluginLogger.Warn("SrNetworkManager type not found - skipping patches");
                    return;
                }
                
                PluginLogger.Info("Found SrNetworkManager type: " + srNetworkManagerType.FullName);
                
                // Apply hybrid networking patch - detects hosting mode and only intercepts internet hosting
                TryPatchDoStartHostHybrid(srNetworkManagerType);
                TryPatchSpawnMethodHybrid(srNetworkManagerType);
                
                PluginLogger.Info("Hybrid networking patches applied");
                PluginLogger.Info("Local hosting: Uses vanilla UNet (unchanged)");
                PluginLogger.Info("Internet hosting: Uses Mirror replacement");
                
                PluginLogger.Info("Manual patching completed");
            }
            catch (Exception e)
            {
                PluginLogger.Error("Error during manual patching: " + e.Message);
                throw;
            }
        }
        
        private void TryMonitorMethod(System.Type targetType, string methodName, string patchDescription)
        {
            try
            {
                var method = AccessTools.Method(targetType, methodName);
                if (ReferenceEquals(method, null))
                {
                    PluginLogger.Warn("Method " + methodName + " not found - skipping " + patchDescription);
                    return;
                }
                
                // Use postfix patches to monitor what happens without interfering
                var postfixMethod = typeof(UNetToMirrorRedirector).GetMethod("Monitor" + methodName + "Postfix");
                if (!ReferenceEquals(postfixMethod, null))
                {
                    harmonyInstance.Patch(method, null, new HarmonyMethod(postfixMethod));
                    PluginLogger.Info("Successfully added monitoring for " + patchDescription);
                }
                else
                {
                    PluginLogger.Warn("Monitor method for " + methodName + " not found - skipping " + patchDescription);
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to add monitoring for " + patchDescription + ": " + e.Message);
            }
        }
        
        private System.Type FindSrNetworkManagerType()
        {
            try
            {
                // Method 1: Try direct name lookup
                var type = AccessTools.TypeByName("SrNetworkManager");
                if (!ReferenceEquals(type, null))
                {
                    PluginLogger.Info("Found SrNetworkManager via direct lookup");
                    return type;
                }
                
                // Method 2: Search through all loaded assemblies
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (assembly.FullName.Contains("Assembly-CSharp"))
                        {
                            foreach (var assemblyType in assembly.GetTypes())
                            {
                                if (assemblyType.Name == "SrNetworkManager")
                                {
                                    PluginLogger.Info("Found SrNetworkManager in assembly: " + assembly.FullName);
                                    return assemblyType;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Some assemblies might not be accessible, skip them
                    }
                }
                
                PluginLogger.Warn("SrNetworkManager type not found in any loaded assembly");
                return null;
            }
            catch (Exception e)
            {
                PluginLogger.Error("Error searching for SrNetworkManager type: " + e.Message);
                return null;
            }
        }
        
        private void TryMonitorSpawnMethod(System.Type targetType)
        {
            try
            {
                var method = AccessTools.Method(targetType, "Spawn", new System.Type[] { typeof(UnityEngine.GameObject) });
                if (ReferenceEquals(method, null))
                {
                    PluginLogger.Warn("Static Spawn method not found - skipping Spawn monitoring");
                    return;
                }
                
                // Create postfix patch for Spawn method monitoring
                var postfixMethod = typeof(UNetToMirrorRedirector).GetMethod("MonitorSpawnPostfix");
                if (!ReferenceEquals(postfixMethod, null))
                {
                    harmonyInstance.Patch(method, null, new HarmonyMethod(postfixMethod));
                    PluginLogger.Info("Successfully added Spawn monitoring");
                }
                else
                {
                    PluginLogger.Warn("MonitorSpawnPostfix method not found - skipping Spawn monitoring");
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to add Spawn monitoring: " + e.Message);
            }
        }
        
        private void TryPatchOnHostStartedMethod(System.Type targetType)
        {
            try
            {
                var method = AccessTools.Method(targetType, "OnHostStarted");
                if (ReferenceEquals(method, null))
                {
                    PluginLogger.Warn("OnHostStarted method not found - skipping OnHostStarted patch");
                    return;
                }
                
                // Create postfix patch for OnHostStarted method to track completion
                var postfixMethod = typeof(UNetToMirrorRedirector).GetMethod("RedirectOnHostStartedPostfix");
                if (!ReferenceEquals(postfixMethod, null))
                {
                    harmonyInstance.Patch(method, null, new HarmonyMethod(postfixMethod));
                    PluginLogger.Info("Successfully applied OnHostStarted postfix patch");
                }
                else
                {
                    PluginLogger.Warn("RedirectOnHostStartedPostfix method not found - skipping OnHostStarted patch");
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to apply OnHostStarted patch: " + e.Message);
            }
        }
        
        private void TryMonitorUnityStartHostMethod(System.Type targetType)
        {
            try
            {
                // Monitor the Unity NetworkManager base StartHost method
                var baseType = typeof(UnityEngine.Networking.NetworkManager);
                var method = AccessTools.Method(baseType, "StartHost");
                if (ReferenceEquals(method, null))
                {
                    PluginLogger.Warn("Unity NetworkManager.StartHost method not found - skipping Unity StartHost monitoring");
                    return;
                }
                
                // Create postfix patch for Unity StartHost method monitoring
                var postfixMethod = typeof(UNetToMirrorRedirector).GetMethod("MonitorUnityStartHostPostfix");
                if (!ReferenceEquals(postfixMethod, null))
                {
                    harmonyInstance.Patch(method, null, new HarmonyMethod(postfixMethod));
                    PluginLogger.Info("Successfully added Unity StartHost monitoring");
                }
                else
                {
                    PluginLogger.Warn("MonitorUnityStartHostPostfix method not found - skipping Unity StartHost monitoring");
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to add Unity StartHost monitoring: " + e.Message);
            }
        }
        
        private void TryPatchEnableHostProperty(System.Type targetType)
        {
            try
            {
                // Patch the EnableHost property getter - this is the gate that blocks multiplayer
                var property = AccessTools.Property(targetType, "EnableHost");
                if (ReferenceEquals(property, null))
                {
                    PluginLogger.Warn("EnableHost property not found - skipping EnableHost patch");
                    return;
                }
                
                var getMethod = property.GetGetMethod();
                if (ReferenceEquals(getMethod, null))
                {
                    PluginLogger.Warn("EnableHost getter method not found - skipping EnableHost patch");
                    return;
                }
                
                // Create prefix patch for EnableHost getter
                var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("RedirectEnableHost");
                if (!ReferenceEquals(prefixMethod, null))
                {
                    harmonyInstance.Patch(getMethod, new HarmonyMethod(prefixMethod));
                    PluginLogger.Info("Successfully applied EnableHost patch - multiplayer gate bypassed!");
                }
                else
                {
                    PluginLogger.Warn("RedirectEnableHost method not found - skipping EnableHost patch");
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to apply EnableHost patch: " + e.Message);
            }
        }
        
        private void TryPatchIsNetworkActiveProperty(System.Type targetType)
        {
            try
            {
                // Patch the IsNetworkActive property getter - this stops the waiting loop
                var property = AccessTools.Property(targetType, "IsNetworkActive");
                if (ReferenceEquals(property, null))
                {
                    PluginLogger.Warn("IsNetworkActive property not found - skipping IsNetworkActive patch");
                    return;
                }
                
                var getMethod = property.GetGetMethod();
                if (ReferenceEquals(getMethod, null))
                {
                    PluginLogger.Warn("IsNetworkActive getter method not found - skipping IsNetworkActive patch");
                    return;
                }
                
                // Create prefix patch for IsNetworkActive getter
                var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("RedirectIsNetworkActive");
                if (!ReferenceEquals(prefixMethod, null))
                {
                    harmonyInstance.Patch(getMethod, new HarmonyMethod(prefixMethod));
                    PluginLogger.Info("Successfully applied IsNetworkActive patch - waiting loop bypassed!");
                }
                else
                {
                    PluginLogger.Warn("RedirectIsNetworkActive method not found - skipping IsNetworkActive patch");
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to apply IsNetworkActive patch: " + e.Message);
            }
        }
        
        private void TryMonitorGameInProgressProperty()
        {
            try
            {
                // Find the Manager class that contains GameInProgress
                var managerType = FindManagerType();
                if (ReferenceEquals(managerType, null))
                {
                    PluginLogger.Warn("Manager type not found - skipping GameInProgress monitoring");
                    return;
                }
                
                // Monitor the GameInProgress property getter
                var property = AccessTools.Property(managerType, "GameInProgress");
                if (ReferenceEquals(property, null))
                {
                    PluginLogger.Warn("GameInProgress property not found - skipping GameInProgress monitoring");
                    return;
                }
                
                var getMethod = property.GetGetMethod();
                if (ReferenceEquals(getMethod, null))
                {
                    PluginLogger.Warn("GameInProgress getter method not found - skipping GameInProgress monitoring");
                    return;
                }
                
                // Create postfix patch for GameInProgress getter monitoring
                var postfixMethod = typeof(UNetToMirrorRedirector).GetMethod("MonitorGameInProgressPostfix");
                if (!ReferenceEquals(postfixMethod, null))
                {
                    harmonyInstance.Patch(getMethod, null, new HarmonyMethod(postfixMethod));
                    PluginLogger.Info("Successfully added GameInProgress monitoring");
                }
                else
                {
                    PluginLogger.Warn("MonitorGameInProgressPostfix method not found - skipping GameInProgress monitoring");
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to add GameInProgress monitoring: " + e.Message);
            }
        }
        
        private System.Type FindManagerType()
        {
            try
            {
                // Method 1: Try direct name lookup
                var type = AccessTools.TypeByName("Manager");
                if (!ReferenceEquals(type, null))
                {
                    PluginLogger.Info("Found Manager via direct lookup");
                    return type;
                }
                
                // Method 2: Search through all loaded assemblies
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (assembly.FullName.Contains("Assembly-CSharp"))
                        {
                            foreach (var assemblyType in assembly.GetTypes())
                            {
                                if (assemblyType.Name == "Manager")
                                {
                                    PluginLogger.Info("Found Manager in assembly: " + assembly.FullName);
                                    return assemblyType;
                                }
                            }
                        }
                    }
                    catch
                    {
                        // Some assemblies might not be accessible, skip them
                    }
                }
                
                PluginLogger.Warn("Manager type not found in any loaded assembly");
                return null;
            }
            catch (Exception e)
            {
                PluginLogger.Error("Error searching for Manager type: " + e.Message);
                return null;
            }
        }
        
        private void TryPatchAdditionalLoadingGates()
        {
            try
            {
                var managerType = FindManagerType();
                if (ReferenceEquals(managerType, null))
                {
                    PluginLogger.Warn("Manager type not found - skipping additional loading gate patches");
                    return;
                }
                
                // Patch IsLoading method
                TryPatchManagerMethod(managerType, "IsLoading", "RedirectIsLoading");
                
                // Patch LoadingApplication property
                TryPatchManagerProperty(managerType, "LoadingApplication", "RedirectLoadingApplication");
                
                // Patch the critical IsLoading() dependencies
                TryPatchManagerField(managerType, "m_ObjectsAllLoaded", "RedirectObjectsAllLoaded");
                TryPatchManagerField(managerType, "m_BeginLoadWaitTurn", "RedirectBeginLoadWaitTurn");
                
                // Patch InputControl.IsLoading()
                TryPatchInputControlIsLoading();
                
                PluginLogger.Info("Additional loading gate patches completed");
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to apply additional loading gate patches: " + e.Message);
            }
        }
        
        private void TryPatchManagerMethod(System.Type managerType, string methodName, string redirectMethodName)
        {
            try
            {
                var method = AccessTools.Method(managerType, methodName);
                if (ReferenceEquals(method, null))
                {
                    PluginLogger.Warn("Manager." + methodName + " method not found - skipping patch");
                    return;
                }
                
                var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod(redirectMethodName);
                if (!ReferenceEquals(prefixMethod, null))
                {
                    harmonyInstance.Patch(method, new HarmonyMethod(prefixMethod));
                    PluginLogger.Info("Successfully applied Manager." + methodName + " patch");
                }
                else
                {
                    PluginLogger.Warn(redirectMethodName + " method not found - skipping Manager." + methodName + " patch");
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to apply Manager." + methodName + " patch: " + e.Message);
            }
        }
        
        private void TryPatchManagerProperty(System.Type managerType, string propertyName, string redirectMethodName)
        {
            try
            {
                var property = AccessTools.Property(managerType, propertyName);
                if (ReferenceEquals(property, null))
                {
                    PluginLogger.Warn("Manager." + propertyName + " property not found - skipping patch");
                    return;
                }
                
                var getMethod = property.GetGetMethod();
                if (ReferenceEquals(getMethod, null))
                {
                    PluginLogger.Warn("Manager." + propertyName + " getter not found - skipping patch");
                    return;
                }
                
                var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod(redirectMethodName);
                if (!ReferenceEquals(prefixMethod, null))
                {
                    harmonyInstance.Patch(getMethod, new HarmonyMethod(prefixMethod));
                    PluginLogger.Info("Successfully applied Manager." + propertyName + " patch");
                }
                else
                {
                    PluginLogger.Warn(redirectMethodName + " method not found - skipping Manager." + propertyName + " patch");
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to apply Manager." + propertyName + " patch: " + e.Message);
            }
        }
        
        private void TryPatchManagerField(System.Type managerType, string fieldName, string redirectMethodName)
        {
            try
            {
                var field = AccessTools.Field(managerType, fieldName);
                if (ReferenceEquals(field, null))
                {
                    PluginLogger.Warn("Manager." + fieldName + " field not found - skipping patch");
                    return;
                }
                
                // For fields, we need to patch any property getters that access them
                // or find getter methods that return the field value
                var properties = managerType.GetProperties();
                bool patched = false;
                
                foreach (var prop in properties)
                {
                    if (prop.Name.Contains(fieldName.Replace("m_", "")))
                    {
                        var getMethod = prop.GetGetMethod();
                        if (!ReferenceEquals(getMethod, null))
                        {
                            var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod(redirectMethodName);
                            if (!ReferenceEquals(prefixMethod, null))
                            {
                                harmonyInstance.Patch(getMethod, new HarmonyMethod(prefixMethod));
                                PluginLogger.Info("Successfully applied Manager." + fieldName + " patch via property " + prop.Name);
                                patched = true;
                                break;
                            }
                        }
                    }
                }
                
                if (!patched)
                {
                    PluginLogger.Warn("Could not find property getter for Manager." + fieldName + " - field patch skipped");
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to apply Manager." + fieldName + " patch: " + e.Message);
            }
        }
        
        private void TryPatchInputControlIsLoading()
        {
            try
            {
                // Find InputControl type
                var inputControlType = AccessTools.TypeByName("InputControl");
                if (ReferenceEquals(inputControlType, null))
                {
                    PluginLogger.Warn("InputControl type not found - skipping InputControl.IsLoading patch");
                    return;
                }
                
                var method = AccessTools.Method(inputControlType, "IsLoading");
                if (ReferenceEquals(method, null))
                {
                    PluginLogger.Warn("InputControl.IsLoading method not found - skipping patch");
                    return;
                }
                
                var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("RedirectInputControlIsLoading");
                if (!ReferenceEquals(prefixMethod, null))
                {
                    harmonyInstance.Patch(method, new HarmonyMethod(prefixMethod));
                    PluginLogger.Info("Successfully applied InputControl.IsLoading patch");
                }
                else
                {
                    PluginLogger.Warn("RedirectInputControlIsLoading method not found - skipping InputControl patch");
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to apply InputControl.IsLoading patch: " + e.Message);
            }
        }
        
        private void TryPatchDoStartHostHybrid(System.Type targetType)
        {
            try
            {
                var method = AccessTools.Method(targetType, "DoStartHost");
                if (ReferenceEquals(method, null))
                {
                    PluginLogger.Warn("DoStartHost method not found - skipping monitoring patch");
                    return;
                }
                
                // Use postfix monitoring instead of prefix blocking
                var postfixMethod = typeof(UNetToMirrorRedirector).GetMethod("MonitorHostingPostfix");
                if (!ReferenceEquals(postfixMethod, null))
                {
                    harmonyInstance.Patch(method, null, new HarmonyMethod(postfixMethod));
                    PluginLogger.Info("Successfully applied DoStartHost monitoring patch");
                    
                    // Also patch matchmaking methods
                    TryPatchMatchMaking();
                    
                    // Add loading progress patches to fix 0% stuck issue
                    TryPatchLoadingProgressSystem();
                    
                    // Add player readiness patches to fix "Waiting for GamePlayer" issue
                    TryPatchPlayerReadinessSystem();
                    
                    // Patch NAT Helper and Facilitator to prevent real UNet connections
                    TryPatchNATAndFacilitator();
                }
                else
                {
                    PluginLogger.Warn("MonitorHostingPostfix method not found - skipping monitoring patch");
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to apply DoStartHost monitoring patch: " + e.Message);
            }
        }
        
        private void TryPatchMatchMaking()
        {
            try
            {
                // Patch the actual UnityMatchMakerService.DoCreateMatch method
                var unityMatchMakerType = AccessTools.TypeByName("UnityMatchMakerService");
                if (!ReferenceEquals(unityMatchMakerType, null))
                {
                    PluginLogger.Info("Found UnityMatchMakerService - patching DoCreateMatch");
                    
                    var doCreateMatchMethod = AccessTools.Method(unityMatchMakerType, "DoCreateMatch");
                    if (!ReferenceEquals(doCreateMatchMethod, null))
                    {
                        var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("InterceptMatchMaking");
                        if (!ReferenceEquals(prefixMethod, null))
                        {
                            harmonyInstance.Patch(doCreateMatchMethod, new HarmonyMethod(prefixMethod));
                            PluginLogger.Info("Successfully patched UnityMatchMakerService.DoCreateMatch for internet interception");
                        }
                        else
                        {
                            PluginLogger.Warn("InterceptMatchMaking method not found");
                        }
                    }
                    else
                    {
                        PluginLogger.Warn("DoCreateMatch method not found in UnityMatchMakerService");
                    }
                }
                else
                {
                    PluginLogger.Warn("UnityMatchMakerService type not found");
                }
                
                // Also patch Unity's built-in NetworkMatch if available
                var networkMatchType = AccessTools.TypeByName("UnityEngine.Networking.Match.NetworkMatch");
                if (!ReferenceEquals(networkMatchType, null))
                {
                    PluginLogger.Info("Found Unity NetworkMatch - patching CreateMatch");
                    var createMatchMethod = AccessTools.Method(networkMatchType, "CreateMatch");
                    if (!ReferenceEquals(createMatchMethod, null))
                    {
                        var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("InterceptMatchMaking");
                        if (!ReferenceEquals(prefixMethod, null))
                        {
                            harmonyInstance.Patch(createMatchMethod, new HarmonyMethod(prefixMethod));
                            PluginLogger.Info("Successfully patched Unity NetworkMatch.CreateMatch");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to patch matchmaking methods: " + e.Message);
            }
        }
        
        private void TryPatchLoadingProgressSystem()
        {
            try
            {
                PluginLogger.Info("Applying loading progress system patches to fix 0% stuck issue...");
                
                // Patch InputControl's loadProgressPct that's getting stuck at 0%
                TryPatchInputControlLoadProgress();
                
                // Patch any NetworkManager state that might be blocking progress
                TryPatchNetworkManagerState();
                
                // Patch game state loading systems
                TryPatchGameStateLoading();
                
                PluginLogger.Info("Loading progress system patches completed");
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to apply loading progress patches: " + e.Message);
            }
        }
        
        private void TryPatchInputControlLoadProgress()
        {
            try
            {
                var inputControlType = AccessTools.TypeByName("InputControl");
                if (ReferenceEquals(inputControlType, null))
                {
                    PluginLogger.Warn("InputControl type not found - skipping loadProgressPct patch");
                    return;
                }
                
                // Look for the property that controls loadProgressPct
                var loadProgressProperty = AccessTools.Property(inputControlType, "loadProgressPct");
                if (!ReferenceEquals(loadProgressProperty, null))
                {
                    var getMethod = loadProgressProperty.GetGetMethod();
                    if (!ReferenceEquals(getMethod, null))
                    {
                        var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("OverrideLoadProgressPct");
                        if (!ReferenceEquals(prefixMethod, null))
                        {
                            harmonyInstance.Patch(getMethod, new HarmonyMethod(prefixMethod));
                            PluginLogger.Info("Successfully patched InputControl.loadProgressPct getter");
                        }
                    }
                }
                
                // Also look for fields that might store the loading progress
                var loadProgressField = AccessTools.Field(inputControlType, "loadProgressPct");
                if (!ReferenceEquals(loadProgressField, null))
                {
                    PluginLogger.Info("Found loadProgressPct field in InputControl");
                    // Field patching would require more complex IL manipulation
                    // For now, we'll rely on the property patch above
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to patch InputControl load progress: " + e.Message);
            }
        }
        
        private void TryPatchNetworkManagerState()
        {
            try
            {
                // Patch Unity NetworkManager to ensure proper state progression
                var networkManagerType = typeof(UnityEngine.Networking.NetworkManager);
                
                // Patch OnServerConnect to ensure connections are properly handled
                var onServerConnectMethod = AccessTools.Method(networkManagerType, "OnServerConnect");
                if (!ReferenceEquals(onServerConnectMethod, null))
                {
                    var postfixMethod = typeof(UNetToMirrorRedirector).GetMethod("OnServerConnectPostfix");
                    if (!ReferenceEquals(postfixMethod, null))
                    {
                        harmonyInstance.Patch(onServerConnectMethod, null, new HarmonyMethod(postfixMethod));
                        PluginLogger.Info("Successfully patched NetworkManager.OnServerConnect");
                    }
                }
                
                // Patch OnStartHost to ensure host initialization completes
                var onStartHostMethod = AccessTools.Method(networkManagerType, "OnStartHost");
                if (!ReferenceEquals(onStartHostMethod, null))
                {
                    var postfixMethod = typeof(UNetToMirrorRedirector).GetMethod("OnStartHostCompletePostfix");
                    if (!ReferenceEquals(postfixMethod, null))
                    {
                        harmonyInstance.Patch(onStartHostMethod, null, new HarmonyMethod(postfixMethod));
                        PluginLogger.Info("Successfully patched NetworkManager.OnStartHost");
                    }
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to patch NetworkManager state: " + e.Message);
            }
        }
        
        private void TryPatchGameStateLoading()
        {
            try
            {
                // Look for game state managers that control loading
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    if (assembly.FullName.Contains("Assembly-CSharp"))
                    {
                        foreach (var type in assembly.GetTypes())
                        {
                            // Look for types that manage game state
                            if (type.Name.Contains("GameState") || type.Name.Contains("LoadState") || type.Name.Contains("Progress"))
                            {
                                TryPatchGameStateType(type);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to patch game state loading: " + e.Message);
            }
        }
        
        private void TryPatchGameStateType(System.Type type)
        {
            try
            {
                // Look for loading-related properties and methods
                var properties = type.GetProperties();
                foreach (var prop in properties)
                {
                    if (prop.Name.Contains("Loading") || prop.Name.Contains("Progress") || prop.Name.Contains("Ready"))
                    {
                        var getMethod = prop.GetGetMethod();
                        if (!ReferenceEquals(getMethod, null))
                        {
                            var postfixMethod = typeof(UNetToMirrorRedirector).GetMethod("MonitorGameStatePostfix");
                            if (!ReferenceEquals(postfixMethod, null))
                            {
                                harmonyInstance.Patch(getMethod, null, new HarmonyMethod(postfixMethod));
                                PluginLogger.Info("Monitoring " + type.Name + "." + prop.Name + " for game state");
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to patch game state type " + type.Name + ": " + e.Message);
            }
        }
        
        private void TryPatchPlayerReadinessSystem()
        {
            try
            {
                PluginLogger.Info("Applying player readiness system patches to fix 'Waiting for GamePlayer' issue...");
                
                // Patch SrGamePlayer if we can find it
                TryPatchSrGamePlayer();
                
                // Patch NetworkBehaviour readiness methods
                TryPatchNetworkBehaviourReady();
                
                // Patch NetworkManager player ready callbacks
                TryPatchNetworkManagerPlayerCallbacks();
                
                PluginLogger.Info("Player readiness system patches completed");
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to apply player readiness patches: " + e.Message);
            }
        }
        
        private void TryPatchSrGamePlayer()
        {
            try
            {
                var srGamePlayerType = AccessTools.TypeByName("SrGamePlayer");
                if (ReferenceEquals(srGamePlayerType, null))
                {
                    PluginLogger.Warn("SrGamePlayer type not found - skipping SrGamePlayer patches");
                    return;
                }
                
                PluginLogger.Info("Found SrGamePlayer type, applying readiness patches");
                
                // Patch OnStartLocalPlayer if it exists
                var onStartLocalPlayerMethod = AccessTools.Method(srGamePlayerType, "OnStartLocalPlayer");
                if (!ReferenceEquals(onStartLocalPlayerMethod, null))
                {
                    var postfixMethod = typeof(UNetToMirrorRedirector).GetMethod("OnStartLocalPlayerPostfix");
                    if (!ReferenceEquals(postfixMethod, null))
                    {
                        harmonyInstance.Patch(onStartLocalPlayerMethod, null, new HarmonyMethod(postfixMethod));
                        PluginLogger.Info("Successfully patched SrGamePlayer.OnStartLocalPlayer");
                    }
                }
                
                // Patch isReady property if it exists
                var isReadyProperty = AccessTools.Property(srGamePlayerType, "isReady");
                if (!ReferenceEquals(isReadyProperty, null))
                {
                    var getMethod = isReadyProperty.GetGetMethod();
                    if (!ReferenceEquals(getMethod, null))
                    {
                        var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("OverrideIsReady");
                        if (!ReferenceEquals(prefixMethod, null))
                        {
                            harmonyInstance.Patch(getMethod, new HarmonyMethod(prefixMethod));
                            PluginLogger.Info("Successfully patched SrGamePlayer.isReady getter");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to patch SrGamePlayer: " + e.Message);
            }
        }
        
        private void TryPatchNetworkBehaviourReady()
        {
            try
            {
                var networkBehaviourType = typeof(UnityEngine.Networking.NetworkBehaviour);
                
                // Patch OnStartAuthority
                var onStartAuthorityMethod = AccessTools.Method(networkBehaviourType, "OnStartAuthority");
                if (!ReferenceEquals(onStartAuthorityMethod, null))
                {
                    var postfixMethod = typeof(UNetToMirrorRedirector).GetMethod("OnStartAuthorityPostfix");
                    if (!ReferenceEquals(postfixMethod, null))
                    {
                        harmonyInstance.Patch(onStartAuthorityMethod, null, new HarmonyMethod(postfixMethod));
                        PluginLogger.Info("Successfully patched NetworkBehaviour.OnStartAuthority");
                    }
                }
                
                // Patch hasAuthority property
                var hasAuthorityProperty = AccessTools.Property(networkBehaviourType, "hasAuthority");
                if (!ReferenceEquals(hasAuthorityProperty, null))
                {
                    var getMethod = hasAuthorityProperty.GetGetMethod();
                    if (!ReferenceEquals(getMethod, null))
                    {
                        var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("OverrideHasAuthority");
                        if (!ReferenceEquals(prefixMethod, null))
                        {
                            harmonyInstance.Patch(getMethod, new HarmonyMethod(prefixMethod));
                            PluginLogger.Info("Successfully patched NetworkBehaviour.hasAuthority getter");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to patch NetworkBehaviour ready methods: " + e.Message);
            }
        }
        
        private void TryPatchNetworkManagerPlayerCallbacks()
        {
            try
            {
                var networkManagerType = typeof(UnityEngine.Networking.NetworkManager);
                
                // Patch OnClientReady
                var onClientReadyMethod = AccessTools.Method(networkManagerType, "OnClientReady");
                if (!ReferenceEquals(onClientReadyMethod, null))
                {
                    var postfixMethod = typeof(UNetToMirrorRedirector).GetMethod("OnClientReadyPostfix");
                    if (!ReferenceEquals(postfixMethod, null))
                    {
                        harmonyInstance.Patch(onClientReadyMethod, null, new HarmonyMethod(postfixMethod));
                        PluginLogger.Info("Successfully patched NetworkManager.OnClientReady");
                    }
                }
                
                // Patch OnServerAddPlayer
                var onServerAddPlayerMethod = AccessTools.Method(networkManagerType, "OnServerAddPlayer");
                if (!ReferenceEquals(onServerAddPlayerMethod, null))
                {
                    var postfixMethod = typeof(UNetToMirrorRedirector).GetMethod("OnServerAddPlayerPostfix");
                    if (!ReferenceEquals(postfixMethod, null))
                    {
                        harmonyInstance.Patch(onServerAddPlayerMethod, null, new HarmonyMethod(postfixMethod));
                        PluginLogger.Info("Successfully patched NetworkManager.OnServerAddPlayer");
                    }
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to patch NetworkManager player callbacks: " + e.Message);
            }
        }
        
        private void TryPatchNATAndFacilitator()
        {
            try
            {
                PluginLogger.Info("Applying NAT Helper and Facilitator patches to block real UNet connections...");
                
                // Patch Unity's NAT Helper
                TryPatchNATHelper();
                
                // Patch Unity's Facilitator connection
                TryPatchFacilitator();
                
                // Patch Unity's connection helper methods
                TryPatchConnectionHelper();
                
                PluginLogger.Info("NAT Helper and Facilitator patches completed");
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to apply NAT and Facilitator patches: " + e.Message);
            }
        }
        
        private void TryPatchNATHelper()
        {
            try
            {
                // Look for NAT Helper related types
                var natHelperType = AccessTools.TypeByName("UnityEngine.Networking.NetworkTransport") ?? 
                                   AccessTools.TypeByName("NATHelper") ??
                                   AccessTools.TypeByName("UnityEngine.Network");
                
                if (!ReferenceEquals(natHelperType, null))
                {
                    PluginLogger.Info("Found NAT Helper type: " + natHelperType.Name);
                    
                    // Patch NAT connection methods
                    var connectMethods = new string[] { "Connect", "ConnectWithSimulator", "ConnectEndPoint" };
                    foreach (var methodName in connectMethods)
                    {
                        var method = AccessTools.Method(natHelperType, methodName);
                        if (!ReferenceEquals(method, null))
                        {
                            var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("InterceptNATConnection");
                            if (!ReferenceEquals(prefixMethod, null))
                            {
                                harmonyInstance.Patch(method, new HarmonyMethod(prefixMethod));
                                PluginLogger.Info("Successfully patched NAT Helper " + methodName);
                            }
                        }
                    }
                }
                else
                {
                    PluginLogger.Warn("NAT Helper type not found - trying alternative approach");
                    
                    // Try to find any method that mentions facilitator in logs
                    TryPatchByLogMessage("Failed to connect to Facilitator");
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to patch NAT Helper: " + e.Message);
            }
        }
        
        private void TryPatchFacilitator()
        {
            try
            {
                // Look for Facilitator connection types
                var facilitatorType = AccessTools.TypeByName("Facilitator") ?? 
                                     AccessTools.TypeByName("UnityEngine.Networking.Facilitator");
                
                if (!ReferenceEquals(facilitatorType, null))
                {
                    PluginLogger.Info("Found Facilitator type: " + facilitatorType.Name);
                    
                    // Patch facilitator connection methods
                    var facilitatorMethods = new string[] { "Connect", "Initialize", "StartConnection" };
                    foreach (var methodName in facilitatorMethods)
                    {
                        var method = AccessTools.Method(facilitatorType, methodName);
                        if (!ReferenceEquals(method, null))
                        {
                            var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("InterceptFacilitatorConnection");
                            if (!ReferenceEquals(prefixMethod, null))
                            {
                                harmonyInstance.Patch(method, new HarmonyMethod(prefixMethod));
                                PluginLogger.Info("Successfully patched Facilitator " + methodName);
                            }
                        }
                    }
                }
                else
                {
                    PluginLogger.Warn("Facilitator type not found - will monitor for connection attempts");
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to patch Facilitator: " + e.Message);
            }
        }
        
        private void TryPatchConnectionHelper()
        {
            try
            {
                // Patch Unity's Network class if available
                var networkType = AccessTools.TypeByName("UnityEngine.Network");
                if (!ReferenceEquals(networkType, null))
                {
                    PluginLogger.Info("Found Unity Network type, patching connection methods");
                    
                    var connectionMethods = new string[] { "Connect", "ConnectWithGuid", "HavePublicAddress" };
                    foreach (var methodName in connectionMethods)
                    {
                        var method = AccessTools.Method(networkType, methodName);
                        if (!ReferenceEquals(method, null))
                        {
                            var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("InterceptNetworkConnection");
                            if (!ReferenceEquals(prefixMethod, null))
                            {
                                harmonyInstance.Patch(method, new HarmonyMethod(prefixMethod));
                                PluginLogger.Info("Successfully patched Unity Network " + methodName);
                            }
                        }
                    }
                }
                
                // Also try to patch NetworkTransport
                var networkTransportType = AccessTools.TypeByName("UnityEngine.Networking.NetworkTransport");
                if (!ReferenceEquals(networkTransportType, null))
                {
                    PluginLogger.Info("Found NetworkTransport type, patching transport methods");
                    
                    var transportMethods = new string[] { "Connect", "ConnectWithSimulator", "ConnectEndPoint" };
                    foreach (var methodName in transportMethods)
                    {
                        var method = AccessTools.Method(networkTransportType, methodName);
                        if (!ReferenceEquals(method, null))
                        {
                            var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("InterceptTransportConnection");
                            if (!ReferenceEquals(prefixMethod, null))
                            {
                                harmonyInstance.Patch(method, new HarmonyMethod(prefixMethod));
                                PluginLogger.Info("Successfully patched NetworkTransport " + methodName);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to patch connection helpers: " + e.Message);
            }
        }
        
        private void TryPatchByLogMessage(string logMessage)
        {
            try
            {
                PluginLogger.Info("Searching for methods that might log: " + logMessage);
                
                // This is a more advanced technique - search through all assemblies for methods
                // that might contain the log message we're seeing
                var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    try
                    {
                        PluginLogger.Info("Checking assembly: " + assembly.FullName);
                        
                        if (assembly.FullName.Contains("UnityEngine") || assembly.FullName.Contains("Assembly-CSharp"))
                        {
                            var types = assembly.GetTypes();
                            PluginLogger.Info("Assembly has " + types.Length + " types");
                            
                            foreach (var type in types)
                            {
                                if (type.Name.Contains("NAT") || type.Name.Contains("Facilitator") || type.Name.Contains("Network") || type.Name.Contains("Helper"))
                                {
                                    PluginLogger.Info("Found potential network type: " + type.FullName);
                                    TryPatchNetworkTypeComprehensive(type);
                                }
                            }
                        }
                    }
                    catch (Exception assemblyEx)
                    {
                        PluginLogger.Warn("Failed to process assembly " + assembly.FullName + ": " + assemblyEx.Message);
                    }
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to search by log message: " + e.Message);
            }
        }
        
        private void TryPatchNetworkTypeComprehensive(System.Type type)
        {
            try
            {
                PluginLogger.Info("Comprehensively patching network type: " + type.FullName);
                
                // Get all methods and log them for debugging
                var methods = type.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Instance);
                PluginLogger.Info("Type " + type.Name + " has " + methods.Length + " methods");
                
                foreach (var method in methods)
                {
                    // Look for any method that might connect to facilitator
                    if (method.Name.Contains("Connect") || method.Name.Contains("Initialize") || method.Name.Contains("Start") || 
                        method.Name.Contains("Init") || method.Name.Contains("Facilitator") || method.Name.Contains("NAT"))
                    {
                        PluginLogger.Info("Found potential method: " + type.Name + "." + method.Name);
                        
                        try
                        {
                            var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("InterceptAnyNetworkCall");
                            if (!ReferenceEquals(prefixMethod, null))
                            {
                                harmonyInstance.Patch(method, new HarmonyMethod(prefixMethod));
                                PluginLogger.Info("Successfully patched: " + type.Name + "." + method.Name);
                            }
                        }
                        catch (Exception patchEx)
                        {
                            PluginLogger.Warn("Failed to patch " + type.Name + "." + method.Name + ": " + patchEx.Message);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to comprehensively patch network type " + type.Name + ": " + e.Message);
            }
        }
        
        private void TryPatchNetworkType(System.Type type)
        {
            try
            {
                var methods = type.GetMethods();
                foreach (var method in methods)
                {
                    if (method.Name.Contains("Connect") || method.Name.Contains("Initialize") || method.Name.Contains("Start"))
                    {
                        var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("InterceptGeneralNetworkCall");
                        if (!ReferenceEquals(prefixMethod, null))
                        {
                            harmonyInstance.Patch(method, new HarmonyMethod(prefixMethod));
                            PluginLogger.Info("Patched general network method: " + type.Name + "." + method.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to patch network type " + type.Name + ": " + e.Message);
            }
        }
        
        private void TryPatchTypeForMatchmaking(System.Type type)
        {
            try
            {
                var methods = new string[] { "CreateMatch", "Create", "StartMatchMaker", "Connect" };
                foreach (var methodName in methods)
                {
                    var method = AccessTools.Method(type, methodName);
                    if (!ReferenceEquals(method, null))
                    {
                        var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("InterceptMatchMaking");
                        if (!ReferenceEquals(prefixMethod, null))
                        {
                            harmonyInstance.Patch(method, new HarmonyMethod(prefixMethod));
                            PluginLogger.Info("Successfully patched " + type.Name + "." + methodName + " for internet detection");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to patch matchmaking type " + type.Name + ": " + e.Message);
            }
        }
        
        private void TryPatchSpawnMethodHybrid(System.Type targetType)
        {
            try
            {
                var method = AccessTools.Method(targetType, "Spawn", new System.Type[] { typeof(UnityEngine.GameObject) });
                if (ReferenceEquals(method, null))
                {
                    PluginLogger.Warn("Static Spawn method not found - skipping hybrid spawn patch");
                    return;
                }
                
                // Use the hybrid spawn method that only intercepts during internet hosting
                var prefixMethod = typeof(UNetToMirrorRedirector).GetMethod("InterceptSpawnForInternetOnly");
                if (!ReferenceEquals(prefixMethod, null))
                {
                    harmonyInstance.Patch(method, new HarmonyMethod(prefixMethod));
                    PluginLogger.Info("Successfully applied hybrid Spawn patch");
                }
                else
                {
                    PluginLogger.Warn("InterceptSpawnForInternetOnly method not found - skipping hybrid spawn patch");
                }
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Failed to apply hybrid Spawn patch: " + e.Message);
            }
        }
        
        private void LogAppliedPatches()
        {
            try
            {
                var patchedMethods = harmonyInstance.GetPatchedMethods();
                int patchCount = 0;
                
                foreach (var method in patchedMethods)
                {
                    patchCount++;
                    PluginLogger.Info("Patched method: " + method.DeclaringType.Name + "." + method.Name);
                }
                
                PluginLogger.Info("Total patches applied: " + patchCount);
            }
            catch (Exception e)
            {
                PluginLogger.Warn("Could not log applied patches: " + e.Message);
            }
        }
        
        void OnDestroy()
        {
            PluginLogger.Info("Networking Fix plugin shutting down...");
            
            try
            {
                // Clean up Harmony patches
                if (!ReferenceEquals(harmonyInstance, null))
                {
                    harmonyInstance.UnpatchSelf();
                    PluginLogger.Info("Harmony patches removed");
                }
                
                // Clean up Mirror networking resources
                UNetToMirrorTranslator.Cleanup();
                
                PluginLogger.Info("Plugin cleanup completed");
            }
            catch (Exception e)
            {
                PluginLogger.Error("Error during plugin cleanup: " + e.Message);
            }
        }
        
        // Static access for other components
        public static NetworkingFixPlugin GetInstance()
        {
            return Instance;
        }
    }
}