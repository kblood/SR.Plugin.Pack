using System;
using System.Linq;
using HarmonyLib;
using SatelliteReignNetworkingFix.Utils;
using SatelliteReignNetworkingFix.MirrorIntegration;

namespace SatelliteReignNetworkingFix.HarmonyPatches
{
    /// <summary>
    /// Harmony patches for SrNetworkManager class
    /// These patches intercept UNet networking calls and redirect them to Mirror
    /// </summary>
    public class SrNetworkManagerPatches
    {
        private static Utils.Logger logger = NetworkingFixPlugin.PluginLogger;
        
        // Shared method to find SrNetworkManager type
        private static System.Type FindSrNetworkManagerType()
        {
            try
            {
                // Method 1: Try direct name lookup
                var type = AccessTools.TypeByName("SrNetworkManager");
                if (type != null)
                {
                    logger.Info("Found SrNetworkManager via direct lookup");
                    return type;
                }
                
                // Method 2: Search through Assembly-CSharp
                var assemblyCSharp = System.Reflection.Assembly.GetAssembly(typeof(object))
                    .GetReferencedAssemblies()
                    .FirstOrDefault(a => a.Name == "Assembly-CSharp");
                
                if (assemblyCSharp != null)
                {
                    var assembly = System.Reflection.Assembly.Load(assemblyCSharp);
                    type = assembly.GetTypes().FirstOrDefault(t => t.Name == "SrNetworkManager");
                    if (type != null)
                    {
                        logger.Info("Found SrNetworkManager in Assembly-CSharp");
                        return type;
                    }
                }
                
                // Method 3: Search all loaded assemblies
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        type = assembly.GetTypes().FirstOrDefault(t => t.Name == "SrNetworkManager");
                        if (type != null)
                        {
                            logger.Info("Found SrNetworkManager in assembly: " + assembly.FullName);
                            return type;
                        }
                    }
                    catch
                    {
                        // Some assemblies might not be accessible, skip them
                    }
                }
                
                logger.Warn("SrNetworkManager type not found in any loaded assembly");
                return null;
            }
            catch (Exception e)
            {
                logger.Error("Error searching for SrNetworkManager type: " + e.Message);
                return null;
            }
        }
        
        /// <summary>
        /// Patch for SrNetworkManager.DoStartHost method
        /// Intercepts host start attempts and redirects to Mirror implementation
        /// </summary>
        [HarmonyPatch]
        public class DoStartHost_Patch
        {
            // Dynamic patching - we'll discover the method at runtime
            static bool Prepare()
            {
                try
                {
                    logger.Info("Attempting to find SrNetworkManager for DoStartHost patch...");
                    
                    // Try different ways to find the type
                    var srNetworkManagerType = FindSrNetworkManagerType();
                    if (srNetworkManagerType == null)
                    {
                        logger.Warn("Could not find SrNetworkManager type - patch will be skipped");
                        return false; // Skip this patch, don't fail the whole thing
                    }
                    
                    // Find the DoStartHost method
                    var doStartHostMethod = AccessTools.Method(srNetworkManagerType, "DoStartHost");
                    if (doStartHostMethod == null)
                    {
                        logger.Warn("Could not find DoStartHost method - patch will be skipped");
                        return false; // Skip this patch
                    }
                    
                    logger.Info("Successfully prepared DoStartHost patch for " + srNetworkManagerType.Name);
                    return true;
                }
                catch (Exception e)
                {
                    logger.Warn("Error preparing DoStartHost patch (will skip): " + e.Message);
                    return false; // Don't fail, just skip
                }
            }
            
            static System.Reflection.MethodBase TargetMethod()
            {
                try
                {
                    var srNetworkManagerType = FindSrNetworkManagerType();
                    if (srNetworkManagerType == null) return null;
                    
                    var method = AccessTools.Method(srNetworkManagerType, "DoStartHost");
                    if (method != null)
                    {
                        logger.Info("Targeting method: " + method.DeclaringType.Name + "." + method.Name);
                    }
                    return method;
                }
                catch (Exception e)
                {
                    logger.Error("Error finding target method: " + e.Message);
                    return null;
                }
            }
            
            
            static bool Prefix(object __instance, ref bool __result)
            {
                try
                {
                    logger.Info("*** INTERCEPTED DoStartHost CALL ***");
                    logger.Info("Redirecting UNet host start to Mirror implementation");
                    
                    // Prevent original UNet method from executing
                    // Redirect to Mirror networking
                    __result = UNetToMirrorTranslator.StartHost();
                    
                    if (__result)
                    {
                        logger.Info("SUCCESS: Mirror host started successfully!");
                    }
                    else
                    {
                        logger.Error("FAILED: Mirror host start failed");
                    }
                    
                    // Return false to skip original method execution
                    return false;
                }
                catch (Exception e)
                {
                    logger.Error("Error in DoStartHost patch: " + e.Message);
                    // If patch fails, allow original method to run (fallback)
                    return true;
                }
            }
        }
        
        /// <summary>
        /// Patch for SrNetworkManager.DoStartServer method
        /// </summary>
        [HarmonyPatch]
        public class DoStartServer_Patch
        {
            static bool Prepare()
            {
                try
                {
                    var srNetworkManagerType = FindSrNetworkManagerType();
                    if (srNetworkManagerType == null) return false;
                    var method = AccessTools.Method(srNetworkManagerType, "DoStartServer");
                    return method != null;
                }
                catch
                {
                    return false;
                }
            }
            
            static System.Reflection.MethodBase TargetMethod()
            {
                var srNetworkManagerType = FindSrNetworkManagerType();
                return srNetworkManagerType != null ? AccessTools.Method(srNetworkManagerType, "DoStartServer") : null;
            }
            
            static bool Prefix(object __instance, ref bool __result)
            {
                try
                {
                    logger.Info("*** INTERCEPTED DoStartServer CALL ***");
                    __result = UNetToMirrorTranslator.StartServer();
                    logger.Info("Mirror server start result: " + __result);
                    return false;
                }
                catch (Exception e)
                {
                    logger.Error("Error in DoStartServer patch: " + e.Message);
                    return true;
                }
            }
        }
        
        /// <summary>
        /// Patch for SrNetworkManager.DoStartClient method
        /// </summary>
        [HarmonyPatch]
        public class DoStartClient_Patch
        {
            static bool Prepare()
            {
                try
                {
                    var srNetworkManagerType = FindSrNetworkManagerType();
                    if (srNetworkManagerType == null) return false;
                    var method = AccessTools.Method(srNetworkManagerType, "DoStartClient");
                    return method != null;
                }
                catch
                {
                    return false;
                }
            }
            
            static System.Reflection.MethodBase TargetMethod()
            {
                var srNetworkManagerType = FindSrNetworkManagerType();
                return srNetworkManagerType != null ? AccessTools.Method(srNetworkManagerType, "DoStartClient") : null;
            }
            
            static bool Prefix(object __instance, ref bool __result)
            {
                try
                {
                    logger.Info("*** INTERCEPTED DoStartClient CALL ***");
                    __result = UNetToMirrorTranslator.StartClient();
                    logger.Info("Mirror client start result: " + __result);
                    return false;
                }
                catch (Exception e)
                {
                    logger.Error("Error in DoStartClient patch: " + e.Message);
                    return true;
                }
            }
        }
        
        /// <summary>
        /// Patch for SrNetworkManager.StopHost method
        /// </summary>
        [HarmonyPatch]
        public class StopHost_Patch
        {
            static bool Prepare()
            {
                try
                {
                    var srNetworkManagerType = FindSrNetworkManagerType();
                    if (srNetworkManagerType == null) return false;
                    var method = AccessTools.Method(srNetworkManagerType, "StopHost");
                    return method != null;
                }
                catch
                {
                    return false;
                }
            }
            
            static System.Reflection.MethodBase TargetMethod()
            {
                var srNetworkManagerType = FindSrNetworkManagerType();
                return srNetworkManagerType != null ? AccessTools.Method(srNetworkManagerType, "StopHost") : null;
            }
            
            static bool Prefix(object __instance)
            {
                try
                {
                    logger.Info("*** INTERCEPTED StopHost CALL ***");
                    UNetToMirrorTranslator.StopHost();
                    logger.Info("Mirror host stopped");
                    return false;
                }
                catch (Exception e)
                {
                    logger.Error("Error in StopHost patch: " + e.Message);
                    return true;
                }
            }
        }
        
        /// <summary>
        /// Patch for SrNetworkManager.Spawn method - CRITICAL for game loading
        /// This method is called during OnHostStarted and must succeed for multiplayer to work
        /// </summary>
        [HarmonyPatch]
        public class Spawn_Patch
        {
            static bool Prepare()
            {
                try
                {
                    var srNetworkManagerType = FindSrNetworkManagerType();
                    if (srNetworkManagerType == null) return false;
                    var method = AccessTools.Method(srNetworkManagerType, "Spawn", new System.Type[] { typeof(UnityEngine.GameObject) });
                    return method != null;
                }
                catch
                {
                    return false;
                }
            }
            
            static System.Reflection.MethodBase TargetMethod()
            {
                var srNetworkManagerType = FindSrNetworkManagerType();
                return srNetworkManagerType != null ? AccessTools.Method(srNetworkManagerType, "Spawn", new System.Type[] { typeof(UnityEngine.GameObject) }) : null;
            }
            
            static bool Prefix(UnityEngine.GameObject _gameObject)
            {
                try
                {
                    logger.Info("*** INTERCEPTED Spawn CALL *** for: " + (_gameObject != null ? _gameObject.name : "null"));
                    
                    // For local multiplayer, we'll just simulate successful spawning
                    if (_gameObject != null)
                    {
                        logger.Info("✓ Simulating successful spawn for: " + _gameObject.name);
                        // The game expects this to succeed, so we just return without doing UNet spawning
                        return false; // Skip original method
                    }
                    else
                    {
                        logger.Warn("Attempted to spawn null GameObject - allowing original method");
                        return true; // Let original method handle error
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Error in Spawn patch: " + e.Message);
                    return true; // Fallback to original method
                }
            }
        }
        
        /// <summary>
        /// Patch for OnHostStarted method - this is where the critical spawning happens
        /// </summary>
        [HarmonyPatch]
        public class OnHostStarted_Patch
        {
            static bool Prepare()
            {
                try
                {
                    var srNetworkManagerType = FindSrNetworkManagerType();
                    if (srNetworkManagerType == null) return false;
                    var method = AccessTools.Method(srNetworkManagerType, "OnHostStarted", new System.Type[] { });
                    return method != null;
                }
                catch
                {
                    return false;
                }
            }
            
            static System.Reflection.MethodBase TargetMethod()
            {
                var srNetworkManagerType = FindSrNetworkManagerType();
                return srNetworkManagerType != null ? AccessTools.Method(srNetworkManagerType, "OnHostStarted", new System.Type[] { }) : null;
            }
            
            static bool Prefix(object __instance)
            {
                try
                {
                    logger.Info("*** INTERCEPTED OnHostStarted CALL ***");
                    logger.Info("This is where the game initializes networked managers - simulating success");
                    
                    // For local multiplayer, we'll skip the complex UNet initialization
                    // and just let the game think everything was spawned successfully
                    logger.Info("✓ Simulating successful networked manager initialization");
                    logger.Info("✓ Skipping UNet spawn calls - using local multiplayer mode");
                    
                    return false; // Skip original method to avoid UNet dependency
                }
                catch (Exception e)
                {
                    logger.Error("Error in OnHostStarted patch: " + e.Message);
                    return true; // Fallback to original method
                }
            }
        }
    }
}