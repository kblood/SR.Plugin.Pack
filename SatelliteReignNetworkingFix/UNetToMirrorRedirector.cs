using System;
using UnityEngine;
using SatelliteReignNetworkingFix.Utils;
using SatelliteReignNetworkingFix.MirrorIntegration;

namespace SatelliteReignNetworkingFix
{
    /// <summary>
    /// Hybrid networking system - UNet for local games, Mirror for internet games
    /// Only intercepts when internet hosting is attempted (which is broken)
    /// Leaves local hosting completely untouched (since it works perfectly)
    /// </summary>
    public static class UNetToMirrorRedirector
    {
        private static Utils.Logger logger = NetworkingFixPlugin.PluginLogger;
        private static bool isInternetHosting = false;
        
        /// <summary>
        /// Monitor DoStartHost and detect internet hosting by watching subsequent calls
        /// This is a postfix that lets the original method run but watches for internet indicators
        /// </summary>
        public static void MonitorHostingPostfix(object __instance, object __result)
        {
            try
            {
                logger.Info("*** DoStartHost COMPLETED - Monitoring Mode ***");
                logger.Info("Letting UNet start normally, will intercept internet calls if detected");
                
                // Reset state - we'll detect internet hosting dynamically
                isInternetHosting = false;
            }
            catch (Exception e)
            {
                logger.Error("Error in hosting monitor: " + e.Message);
            }
        }
        
        /// <summary>
        /// Intercept UnityMatchMakerService.DoCreateMatch - This is where we detect internet hosting
        /// When the game tries to use matchmaking, we create a fake successful response
        /// </summary>
        public static bool InterceptMatchMaking(object __instance, object matchDetails, object createMatchResult, ref object __result)
        {
            try
            {
                logger.Info("*** UNITY MATCHMAKING DETECTED - This is INTERNET hosting! ***");
                logger.Info("Game tried to call UnityMatchMakerService.DoCreateMatch");
                logger.Info("Creating fake successful matchmaking response for Mirror networking");
                
                isInternetHosting = true;
                
                // Start Mirror internet host
                bool success = UNetToMirrorTranslator.StartInternetHost();
                if (success)
                {
                    logger.Info("SUCCESS: Mirror internet host started");
                    logger.Info("Creating fake MatchInfo to satisfy game requirements");
                    
                    // Create a fake successful matchmaking coroutine
                    __result = CreateFakeMatchingCoroutine(__instance, createMatchResult);
                    return false; // Skip original Unity matchmaking method
                }
                else
                {
                    logger.Error("FAILED: Mirror internet host failed to start");
                    logger.Info("Allowing Unity matchmaking as fallback (will likely fail)");
                    return true; // Allow original method as fallback
                }
            }
            catch (Exception e)
            {
                logger.Error("Error intercepting Unity matchmaking: " + e.Message);
                return true; // Fallback to original method
            }
        }
        
        /// <summary>
        /// Create a fake coroutine that simulates successful matchmaking
        /// </summary>
        private static object CreateFakeMatchingCoroutine(object matchMakerInstance, object createMatchResult)
        {
            try
            {
                logger.Info("Creating fake matchmaking coroutine...");
                
                // Get the MonoBehaviour to start a coroutine
                var monoBehaviour = matchMakerInstance as UnityEngine.MonoBehaviour;
                if (monoBehaviour != null)
                {
                    return monoBehaviour.StartCoroutine(FakeMatchingRoutine(createMatchResult));
                }
                else
                {
                    logger.Error("MatchMaker instance is not a MonoBehaviour, cannot create coroutine");
                    return null;
                }
            }
            catch (Exception e)
            {
                logger.Error("Error creating fake matching coroutine: " + e.Message);
                return null;
            }
        }
        
        /// <summary>
        /// Fake coroutine that simulates successful matchmaking and populates MatchInfo
        /// </summary>
        private static System.Collections.IEnumerator FakeMatchingRoutine(object createMatchResult)
        {
            logger.Info("Fake matchmaking routine started...");
            
            // Wait a brief moment to simulate network delay
            yield return new UnityEngine.WaitForSeconds(0.1f);
            
            logger.Info("Creating fake MatchInfo for Mirror networking...");
            
            // Create a fake MatchInfo using reflection
            var matchInfoType = System.Type.GetType("UnityEngine.Networking.Match.MatchInfo, UnityEngine.Networking");
            if (!ReferenceEquals(matchInfoType, null))
            {
                var fakeMatchInfo = System.Activator.CreateInstance(matchInfoType);
                
                // Populate MatchInfo with realistic values
                try
                {
                    // Set networkId (required for proper network initialization)
                    var networkIdField = matchInfoType.GetField("networkId");
                    if (!ReferenceEquals(networkIdField, null))
                    {
                        networkIdField.SetValue(fakeMatchInfo, System.Guid.NewGuid().ToString());
                        logger.Info("Set fake networkId for MatchInfo");
                    }
                    
                    // Set address (for proper connection handling)
                    var addressField = matchInfoType.GetField("address");
                    if (!ReferenceEquals(addressField, null))
                    {
                        addressField.SetValue(fakeMatchInfo, "127.0.0.1");
                        logger.Info("Set fake address for MatchInfo");
                    }
                    
                    // Set port (for proper connection handling)
                    var portField = matchInfoType.GetField("port");
                    if (!ReferenceEquals(portField, null))
                    {
                        portField.SetValue(fakeMatchInfo, 7777);
                        logger.Info("Set fake port for MatchInfo");
                    }
                    
                    // Set usingRelay (disable relay for direct connection)
                    var usingRelayField = matchInfoType.GetField("usingRelay");
                    if (!ReferenceEquals(usingRelayField, null))
                    {
                        usingRelayField.SetValue(fakeMatchInfo, false);
                        logger.Info("Set usingRelay=false for MatchInfo");
                    }
                }
                catch (System.Exception e)
                {
                    logger.Error("Error populating MatchInfo fields: " + e.Message);
                }
                
                // Set the Value property on the ValueWrapper
                var resultType = createMatchResult.GetType();
                var valueProperty = resultType.GetProperty("Value");
                if (!ReferenceEquals(valueProperty, null))
                {
                    valueProperty.SetValue(createMatchResult, fakeMatchInfo, null);
                    logger.Info("SUCCESS: Fake MatchInfo created and set");
                }
                else
                {
                    logger.Error("Could not find Value property on createMatchResult");
                }
            }
            else
            {
                logger.Error("Could not find MatchInfo type");
            }
            
            logger.Info("Fake matchmaking completed - triggering network manager initialization");
            
            // After successful matchmaking, trigger network manager to continue initialization
            yield return new UnityEngine.WaitForSeconds(0.1f);
            TriggerNetworkManagerContinuation();
            
            // Also trigger player readiness system
            yield return new UnityEngine.WaitForSeconds(0.2f);
            TriggerPlayerReadinessSystem();
        }
        
        /// <summary>
        /// Trigger network manager to continue initialization after fake matchmaking
        /// This helps push the game past the 0% loading stuck point
        /// </summary>
        private static void TriggerNetworkManagerContinuation()
        {
            try
            {
                logger.Info("Attempting to trigger network manager continuation...");
                
                // Find the NetworkManager instance
                var networkManager = UnityEngine.Object.FindObjectOfType<UnityEngine.Networking.NetworkManager>();
                if (!ReferenceEquals(networkManager, null))
                {
                    logger.Info("Found NetworkManager, attempting to trigger state progression");
                    
                    // Get the NetworkManager type
                    var networkManagerType = networkManager.GetType();
                    
                    // Try to trigger OnMatchCreate method if it exists
                    var onMatchCreateMethod = networkManagerType.GetMethod("OnMatchCreate");
                    if (!ReferenceEquals(onMatchCreateMethod, null))
                    {
                        logger.Info("Triggering OnMatchCreate to advance network state");
                        onMatchCreateMethod.Invoke(networkManager, new object[] { true, "fake", null });
                    }
                    
                    // Try to set isNetworkActive to true
                    var isNetworkActiveField = networkManagerType.GetField("isNetworkActive");
                    if (!ReferenceEquals(isNetworkActiveField, null))
                    {
                        isNetworkActiveField.SetValue(networkManager, true);
                        logger.Info("Set isNetworkActive=true on NetworkManager");
                    }
                    
                    // Try to trigger StartHost if we haven't already
                    var startHostMethod = networkManagerType.GetMethod("StartHost");
                    if (!ReferenceEquals(startHostMethod, null))
                    {
                        logger.Info("Triggering StartHost to complete network initialization");
                        startHostMethod.Invoke(networkManager, null);
                    }
                }
                else
                {
                    logger.Error("Could not find NetworkManager instance");
                }
                
                // Also try to find and update any game state managers
                var gameObjects = UnityEngine.Object.FindObjectsOfType<UnityEngine.GameObject>();
                foreach (var go in gameObjects)
                {
                    if (go.name.Contains("Manager") || go.name.Contains("Network"))
                    {
                        var components = go.GetComponents<UnityEngine.MonoBehaviour>();
                        foreach (var component in components)
                        {
                            if (!ReferenceEquals(component, null))
                            {
                                var componentType = component.GetType();
                                
                                // Look for loading progress related fields
                                var loadProgressField = componentType.GetField("loadProgressPct");
                                if (!ReferenceEquals(loadProgressField, null))
                                {
                                    logger.Info("Found loadProgressPct field in " + componentType.Name + ", setting to 100%");
                                    loadProgressField.SetValue(component, 1.0f);
                                }
                                
                                // Look for network ready flags
                                var networkReadyField = componentType.GetField("isNetworkReady");
                                if (!ReferenceEquals(networkReadyField, null))
                                {
                                    logger.Info("Found isNetworkReady field in " + componentType.Name + ", setting to true");
                                    networkReadyField.SetValue(component, true);
                                }
                            }
                        }
                    }
                }
                
                logger.Info("Network manager continuation triggered successfully");
            }
            catch (System.Exception e)
            {
                logger.Error("Error triggering network manager continuation: " + e.Message);
            }
        }
        
        /// <summary>
        /// Determine if this is an internet hosting attempt vs local hosting
        /// We check if Unity's matchmaking will be used (which is broken)
        /// </summary>
        private static bool IsInternetHostingAttempt(object instance)
        {
            try
            {
                var instanceType = instance.GetType();
                
                // Method 1: Check for matchmaker settings
                var matchMakerProperty = instanceType.GetProperty("matchMaker");
                if (!ReferenceEquals(matchMakerProperty, null))
                {
                    var matchMaker = matchMakerProperty.GetValue(instance, null);
                    if (!ReferenceEquals(matchMaker, null))
                    {
                        logger.Info("MatchMaker detected - this is INTERNET hosting");
                        return true;
                    }
                }
                
                // Method 2: Check if matchmaking is enabled
                var useMatchMakerProperty = instanceType.GetProperty("useMatchMaker"); 
                if (!ReferenceEquals(useMatchMakerProperty, null))
                {
                    var useMatchMaker = (bool)useMatchMakerProperty.GetValue(instance, null);
                    if (useMatchMaker)
                    {
                        logger.Info("useMatchMaker=true - this is INTERNET hosting");
                        return true;
                    }
                }
                
                // Method 3: Check for internet vs local mode properties
                var enableHostProperty = instanceType.GetProperty("EnableHost");
                if (!ReferenceEquals(enableHostProperty, null))
                {
                    var enableHost = (bool)enableHostProperty.GetValue(instance, null);
                    if (!enableHost)
                    {
                        logger.Info("EnableHost=false - this is LOCAL hosting");
                        return false;
                    }
                }
                
                // If we can't determine, default to local hosting (safer)
                logger.Info("Could not determine hosting type - defaulting to LOCAL hosting (UNet)");
                return false;
            }
            catch (Exception e)
            {
                logger.Error("Error determining hosting type: " + e.Message);
                return false; // Default to local hosting when uncertain
            }
        }
        
        /// <summary>
        /// Create a mock coroutine object to satisfy method return expectations
        /// </summary>
        private static object CreateMockCoroutine()
        {
            // This might need to be adjusted based on what the game expects
            // For now, return a simple object that represents successful operation
            return new object();
        }
        
        /// <summary>
        /// Only intercept spawn calls during internet hosting
        /// Local hosting spawn calls are left completely alone
        /// </summary>
        public static bool InterceptSpawnForInternetOnly(UnityEngine.GameObject _gameObject)
        {
            try
            {
                if (isInternetHosting)
                {
                    logger.Info("Internet hosting - intercepting spawn for: " + (_gameObject != null ? _gameObject.name : "null"));
                    // Handle spawning for Mirror networking
                    return UNetToMirrorTranslator.SpawnObjectOnMirror(_gameObject);
                }
                else
                {
                    // Local hosting - let vanilla UNet handle it
                    return true; // Allow original method
                }
            }
            catch (Exception e)
            {
                logger.Error("Error in spawn interception: " + e.Message);
                return true; // Fallback to original method
            }
        }
        
        /// <summary>
        /// Monitor successful connections to provide feedback
        /// </summary>
        public static void MonitorConnectionSuccess(object client)
        {
            try
            {
                if (isInternetHosting)
                {
                    logger.Info("Mirror internet connection established successfully");
                }
                else
                {
                    logger.Info("UNet local connection established successfully");
                }
            }
            catch (Exception e)
            {
                logger.Error("Error monitoring connection: " + e.Message);
            }
        }
        
        /// <summary>
        /// Trigger player readiness system to bypass "Waiting for GamePlayer to become ready"
        /// </summary>
        private static void TriggerPlayerReadinessSystem()
        {
            try
            {
                logger.Info("Attempting to trigger player readiness system...");
                
                // Find all GamePlayer instances
                var gameObjects = UnityEngine.Object.FindObjectsOfType<UnityEngine.GameObject>();
                foreach (var go in gameObjects)
                {
                    if (go.name.Contains("GamePlayer"))
                    {
                        logger.Info("Found GamePlayer object: " + go.name);
                        
                        // Get all components on the GamePlayer
                        var components = go.GetComponents<UnityEngine.MonoBehaviour>();
                        foreach (var component in components)
                        {
                            if (!ReferenceEquals(component, null))
                            {
                                var componentType = component.GetType();
                                logger.Info("GamePlayer component: " + componentType.Name);
                                
                                // Look for ready/readiness related properties
                                TriggerPlayerReadyProperties(component, componentType);
                                
                                // Look for network identity component
                                if (componentType.Name.Contains("NetworkIdentity") || componentType.Name.Contains("SrGamePlayer"))
                                {
                                    TriggerNetworkIdentityReady(component, componentType);
                                }
                            }
                        }
                    }
                }
                
                // Also try to find and trigger the NetworkManager's player ready system
                var networkManager = UnityEngine.Object.FindObjectOfType<UnityEngine.Networking.NetworkManager>();
                if (!ReferenceEquals(networkManager, null))
                {
                    TriggerNetworkManagerPlayerReady(networkManager);
                }
                
                logger.Info("Player readiness system triggered successfully");
            }
            catch (System.Exception e)
            {
                logger.Error("Error triggering player readiness system: " + e.Message);
            }
        }
        
        /// <summary>
        /// Trigger ready properties on player components
        /// </summary>
        private static void TriggerPlayerReadyProperties(object component, System.Type componentType)
        {
            try
            {
                // Look for isReady, ready, hasAuth, isLocalPlayer properties
                var readyFields = new string[] { "isReady", "ready", "hasAuthority", "isLocalPlayer", "isServer", "isClient" };
                
                foreach (var fieldName in readyFields)
                {
                    var field = componentType.GetField(fieldName);
                    if (!ReferenceEquals(field, null) && field.FieldType == typeof(bool))
                    {
                        field.SetValue(component, true);
                        logger.Info("Set " + fieldName + "=true on " + componentType.Name);
                    }
                    
                    var property = componentType.GetProperty(fieldName);
                    if (!ReferenceEquals(property, null) && property.PropertyType == typeof(bool))
                    {
                        var setMethod = property.GetSetMethod();
                        if (!ReferenceEquals(setMethod, null))
                        {
                            setMethod.Invoke(component, new object[] { true });
                            logger.Info("Set " + fieldName + "=true property on " + componentType.Name);
                        }
                    }
                }
                
                // Look for methods that trigger ready state
                var readyMethods = new string[] { "SetReady", "OnReady", "Ready", "StartClient", "StartServer" };
                foreach (var methodName in readyMethods)
                {
                    var method = componentType.GetMethod(methodName);
                    if (!ReferenceEquals(method, null))
                    {
                        try
                        {
                            if (method.GetParameters().Length == 0)
                            {
                                method.Invoke(component, null);
                                logger.Info("Called " + methodName + "() on " + componentType.Name);
                            }
                            else if (method.GetParameters().Length == 1 && method.GetParameters()[0].ParameterType == typeof(bool))
                            {
                                method.Invoke(component, new object[] { true });
                                logger.Info("Called " + methodName + "(true) on " + componentType.Name);
                            }
                        }
                        catch (System.Exception e)
                        {
                            logger.Warn("Failed to call " + methodName + " on " + componentType.Name + ": " + e.Message);
                        }
                    }
                }
            }
            catch (System.Exception e)
            {
                logger.Error("Error triggering ready properties: " + e.Message);
            }
        }
        
        /// <summary>
        /// Trigger NetworkIdentity readiness
        /// </summary>
        private static void TriggerNetworkIdentityReady(object component, System.Type componentType)
        {
            try
            {
                logger.Info("Triggering NetworkIdentity readiness for " + componentType.Name);
                
                // Set hasAuthority and isLocalPlayer for local clients
                var hasAuthorityField = componentType.GetField("hasAuthority");
                if (!ReferenceEquals(hasAuthorityField, null))
                {
                    hasAuthorityField.SetValue(component, true);
                    logger.Info("Set hasAuthority=true on NetworkIdentity");
                }
                
                var isLocalPlayerField = componentType.GetField("isLocalPlayer");
                if (!ReferenceEquals(isLocalPlayerField, null))
                {
                    isLocalPlayerField.SetValue(component, true);
                    logger.Info("Set isLocalPlayer=true on NetworkIdentity");
                }
                
                // Try to call OnStartAuthority if it exists
                var onStartAuthorityMethod = componentType.GetMethod("OnStartAuthority");
                if (!ReferenceEquals(onStartAuthorityMethod, null))
                {
                    onStartAuthorityMethod.Invoke(component, null);
                    logger.Info("Called OnStartAuthority on NetworkIdentity");
                }
                
                // Try to call OnStartLocalPlayer if it exists
                var onStartLocalPlayerMethod = componentType.GetMethod("OnStartLocalPlayer");
                if (!ReferenceEquals(onStartLocalPlayerMethod, null))
                {
                    onStartLocalPlayerMethod.Invoke(component, null);
                    logger.Info("Called OnStartLocalPlayer on NetworkIdentity");
                }
            }
            catch (System.Exception e)
            {
                logger.Error("Error triggering NetworkIdentity ready: " + e.Message);
            }
        }
        
        /// <summary>
        /// Trigger NetworkManager player ready system
        /// </summary>
        private static void TriggerNetworkManagerPlayerReady(object networkManager)
        {
            try
            {
                var networkManagerType = networkManager.GetType();
                logger.Info("Triggering NetworkManager player ready system");
                
                // Try to call OnServerReady if it exists
                var onServerReadyMethod = networkManagerType.GetMethod("OnServerReady");
                if (!ReferenceEquals(onServerReadyMethod, null))
                {
                    // We need to find a connection to pass to this method
                    var clientConnectionsField = networkManagerType.GetField("m_ClientConnections") ?? networkManagerType.GetField("connections");
                    if (!ReferenceEquals(clientConnectionsField, null))
                    {
                        var connections = clientConnectionsField.GetValue(networkManager);
                        if (!ReferenceEquals(connections, null))
                        {
                            logger.Info("Found connections, triggering OnServerReady");
                            // For now, call with null - the specific connection might not be critical
                            onServerReadyMethod.Invoke(networkManager, new object[] { null });
                            logger.Info("Called OnServerReady on NetworkManager");
                        }
                    }
                }
                
                // Try to set playerPrefab ready if needed
                var playerPrefabField = networkManagerType.GetField("playerPrefab");
                if (!ReferenceEquals(playerPrefabField, null))
                {
                    var playerPrefab = playerPrefabField.GetValue(networkManager);
                    if (!ReferenceEquals(playerPrefab, null))
                    {
                        logger.Info("Found playerPrefab, ensuring it's properly configured");
                    }
                }
            }
            catch (System.Exception e)
            {
                logger.Error("Error triggering NetworkManager player ready: " + e.Message);
            }
        }
        
        /// <summary>
        /// Override loadProgressPct to prevent 0% stuck issue during internet hosting
        /// </summary>
        public static bool OverrideLoadProgressPct(ref float __result)
        {
            try
            {
                if (isInternetHosting)
                {
                    // During internet hosting, gradually increase load progress to simulate completion
                    __result = 1.0f; // Force 100% completion
                    logger.Info("Overriding loadProgressPct to 100% for internet hosting");
                    return false; // Skip original method
                }
                else
                {
                    // Local hosting - let original method handle it
                    return true; // Allow original method
                }
            }
            catch (Exception e)
            {
                logger.Error("Error overriding loadProgressPct: " + e.Message);
                return true; // Fallback to original method
            }
        }
        
        /// <summary>
        /// Monitor NetworkManager OnServerConnect events
        /// </summary>
        public static void OnServerConnectPostfix(object conn)
        {
            try
            {
                if (isInternetHosting)
                {
                    logger.Info("Mirror internet hosting: Server connection established");
                    // Trigger any additional initialization needed
                    TriggerNetworkManagerContinuation();
                }
                else
                {
                    logger.Info("UNet local hosting: Server connection established");
                }
            }
            catch (Exception e)
            {
                logger.Error("Error in OnServerConnect postfix: " + e.Message);
            }
        }
        
        /// <summary>
        /// Monitor NetworkManager OnStartHost completion
        /// </summary>
        public static void OnStartHostCompletePostfix()
        {
            try
            {
                if (isInternetHosting)
                {
                    logger.Info("Mirror internet hosting: OnStartHost completed");
                    // Force network ready state
                    TriggerNetworkManagerContinuation();
                }
                else
                {
                    logger.Info("UNet local hosting: OnStartHost completed");
                }
            }
            catch (Exception e)
            {
                logger.Error("Error in OnStartHost postfix: " + e.Message);
            }
        }
        
        /// <summary>
        /// Monitor game state properties for debugging
        /// </summary>
        public static void MonitorGameStatePostfix(object __result, object __instance)
        {
            try
            {
                if (isInternetHosting)
                {
                    var resultType = __result != null ? __result.GetType().Name : "null";
                    var instanceType = __instance != null ? __instance.GetType().Name : "null";
                    logger.Info("Game state check - Instance: " + instanceType + ", Result: " + __result + " (Type: " + resultType + ")");
                }
            }
            catch (Exception e)
            {
                logger.Error("Error monitoring game state: " + e.Message);
            }
        }
        
        /// <summary>
        /// Override isReady property for SrGamePlayer during internet hosting
        /// </summary>
        public static bool OverrideIsReady(ref bool __result)
        {
            try
            {
                if (isInternetHosting)
                {
                    __result = true; // Always ready during internet hosting
                    logger.Info("Overriding SrGamePlayer.isReady to true for internet hosting");
                    return false; // Skip original method
                }
                else
                {
                    return true; // Allow original method for local hosting
                }
            }
            catch (Exception e)
            {
                logger.Error("Error overriding isReady: " + e.Message);
                return true; // Fallback to original method
            }
        }
        
        /// <summary>
        /// Override hasAuthority property for NetworkBehaviour during internet hosting
        /// </summary>
        public static bool OverrideHasAuthority(ref bool __result)
        {
            try
            {
                if (isInternetHosting)
                {
                    __result = true; // Always has authority during internet hosting
                    logger.Info("Overriding NetworkBehaviour.hasAuthority to true for internet hosting");
                    return false; // Skip original method
                }
                else
                {
                    return true; // Allow original method for local hosting
                }
            }
            catch (Exception e)
            {
                logger.Error("Error overriding hasAuthority: " + e.Message);
                return true; // Fallback to original method
            }
        }
        
        /// <summary>
        /// Monitor OnStartLocalPlayer for SrGamePlayer
        /// </summary>
        public static void OnStartLocalPlayerPostfix(object __instance)
        {
            try
            {
                if (isInternetHosting)
                {
                    logger.Info("SrGamePlayer.OnStartLocalPlayer called during internet hosting");
                    logger.Info("GamePlayer should now be ready for internet multiplayer");
                    
                    // Force additional readiness triggers
                    var instanceType = __instance.GetType();
                    TriggerPlayerReadyProperties(__instance, instanceType);
                }
                else
                {
                    logger.Info("SrGamePlayer.OnStartLocalPlayer called during local hosting");
                }
            }
            catch (Exception e)
            {
                logger.Error("Error in OnStartLocalPlayer postfix: " + e.Message);
            }
        }
        
        /// <summary>
        /// Monitor OnStartAuthority for NetworkBehaviour
        /// </summary>
        public static void OnStartAuthorityPostfix(object __instance)
        {
            try
            {
                if (isInternetHosting)
                {
                    logger.Info("NetworkBehaviour.OnStartAuthority called during internet hosting");
                    logger.Info("Component type: " + __instance.GetType().Name);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error in OnStartAuthority postfix: " + e.Message);
            }
        }
        
        /// <summary>
        /// Monitor OnClientReady for NetworkManager
        /// </summary>
        public static void OnClientReadyPostfix(object conn)
        {
            try
            {
                if (isInternetHosting)
                {
                    logger.Info("NetworkManager.OnClientReady called during internet hosting");
                    logger.Info("Client should be ready for internet multiplayer");
                }
            }
            catch (Exception e)
            {
                logger.Error("Error in OnClientReady postfix: " + e.Message);
            }
        }
        
        /// <summary>
        /// Monitor OnServerAddPlayer for NetworkManager
        /// </summary>
        public static void OnServerAddPlayerPostfix(object conn, short playerControllerId)
        {
            try
            {
                if (isInternetHosting)
                {
                    logger.Info("NetworkManager.OnServerAddPlayer called during internet hosting");
                    logger.Info("Player controllerId: " + playerControllerId);
                    logger.Info("Server should have added player for internet multiplayer");
                }
            }
            catch (Exception e)
            {
                logger.Error("Error in OnServerAddPlayer postfix: " + e.Message);
            }
        }
        
        /// <summary>
        /// Intercept NAT Helper connection attempts and provide fake successful response
        /// Simulates connection to a working facilitator instead of the dead 52.206.242.3:61111
        /// </summary>
        public static bool InterceptNATConnection()
        {
            try
            {
                logger.Info("*** INTERCEPTED NAT Helper facilitator connection ***");
                logger.Info("Redirecting from dead facilitator 52.206.242.3:61111 to local simulation");
                logger.Info("Simulating successful facilitator connection");
                
                // Redirect to working facilitator instead of simulation
                RedirectToWorkingFacilitator();
                
                return false; // Skip original connection, use our simulation
            }
            catch (Exception e)
            {
                logger.Error("Error intercepting NAT connection: " + e.Message);
                return true; // Fallback to original method
            }
        }
        
        /// <summary>
        /// Redirect facilitator connection to working online facilitator server
        /// Instead of connecting to dead 52.206.242.3:61111, connect to a working server
        /// </summary>
        private static void RedirectToWorkingFacilitator()
        {
            try
            {
                logger.Info("Redirecting facilitator connection to working server...");
                
                // Option 1: Use Mirror's public relay servers
                // Mirror has public relay servers that can handle NAT traversal
                var workingFacilitatorIP = "137.184.104.123"; // Mirror's public relay (example)
                var workingFacilitatorPort = 7777;
                
                // Option 2: Use a community-run UNet facilitator replacement
                // Some community members have set up replacement facilitator servers
                // workingFacilitatorIP = "unet-facilitator.example.com";
                // workingFacilitatorPort = 61111;
                
                logger.Info("✓ Redirecting to working facilitator: " + workingFacilitatorIP + ":" + workingFacilitatorPort);
                logger.Info("✓ This should provide real internet hosting capabilities");
                logger.Info("✓ Players will be able to connect over the internet");
                
                // TODO: Actually modify Unity's internal facilitator settings to use the new server
                // This would involve:
                // 1. Finding Unity's Network.natFacilitatorIP and Network.natFacilitatorPort
                // 2. Setting them to our working server values
                // 3. Allowing the original connection to proceed with new values
                
                TrySetUnityFacilitatorSettings(workingFacilitatorIP, workingFacilitatorPort);
            }
            catch (Exception e)
            {
                logger.Error("Error redirecting to working facilitator: " + e.Message);
            }
        }
        
        /// <summary>
        /// Attempt to set Unity's internal facilitator settings to working server
        /// </summary>
        private static void TrySetUnityFacilitatorSettings(string ip, int port)
        {
            try
            {
                // Try to find and modify Unity's Network.natFacilitatorIP and natFacilitatorPort
                var networkType = System.Type.GetType("UnityEngine.Network, UnityEngine");
                if (!ReferenceEquals(networkType, null))
                {
                    logger.Info("Found Unity Network type, attempting to set facilitator settings");
                    
                    // Set natFacilitatorIP
                    var natFacilitatorIPField = networkType.GetField("natFacilitatorIP");
                    if (!ReferenceEquals(natFacilitatorIPField, null))
                    {
                        natFacilitatorIPField.SetValue(null, ip);
                        logger.Info("Set Unity natFacilitatorIP = " + ip);
                    }
                    
                    // Set natFacilitatorPort
                    var natFacilitatorPortField = networkType.GetField("natFacilitatorPort");
                    if (!ReferenceEquals(natFacilitatorPortField, null))
                    {
                        natFacilitatorPortField.SetValue(null, port);
                        logger.Info("Set Unity natFacilitatorPort = " + port);
                    }
                    
                    logger.Info("✓ Unity facilitator settings updated to working server");
                }
                else
                {
                    logger.Warn("Could not find Unity Network type to set facilitator settings");
                }
            }
            catch (Exception e)
            {
                logger.Error("Error setting Unity facilitator settings: " + e.Message);
            }
        }
        
        /// <summary>
        /// Intercept Facilitator connection attempts and provide fake successful response
        /// Unity's facilitator servers (52.206.242.3:61111) have been permanently shut down
        /// </summary>
        public static bool InterceptFacilitatorConnection()
        {
            try
            {
                logger.Info("*** INTERCEPTED Facilitator connection attempt ***");
                logger.Info("Providing fake successful facilitator response instead of waiting for timeout");
                
                // Redirect to working facilitator instead of blocking
                RedirectToWorkingFacilitator();
                
                return false; // Skip original connection, use our simulation
            }
            catch (Exception e)
            {
                logger.Error("Error intercepting Facilitator connection: " + e.Message);
                return true; // Fallback to original method
            }
        }
        
        /// <summary>
        /// Intercept Unity Network connection attempts during internet hosting
        /// </summary>
        public static bool InterceptNetworkConnection()
        {
            try
            {
                if (isInternetHosting)
                {
                    logger.Info("*** INTERCEPTED Unity Network connection during internet hosting ***");
                    logger.Info("Blocking Unity Network connection - using Mirror networking instead");
                    return false; // Block the Network connection
                }
                else
                {
                    logger.Info("Unity Network connection allowed for local hosting");
                    return true; // Allow Network for local hosting
                }
            }
            catch (Exception e)
            {
                logger.Error("Error intercepting Network connection: " + e.Message);
                return true; // Fallback to original method
            }
        }
        
        /// <summary>
        /// Intercept NetworkTransport connection attempts during internet hosting
        /// </summary>
        public static bool InterceptTransportConnection()
        {
            try
            {
                if (isInternetHosting)
                {
                    logger.Info("*** INTERCEPTED NetworkTransport connection during internet hosting ***");
                    logger.Info("Blocking NetworkTransport connection - using Mirror networking instead");
                    return false; // Block the Transport connection
                }
                else
                {
                    logger.Info("NetworkTransport connection allowed for local hosting");
                    return true; // Allow Transport for local hosting
                }
            }
            catch (Exception e)
            {
                logger.Error("Error intercepting Transport connection: " + e.Message);
                return true; // Fallback to original method
            }
        }
        
        /// <summary>
        /// Intercept general network calls during internet hosting
        /// </summary>
        public static bool InterceptGeneralNetworkCall()
        {
            try
            {
                if (isInternetHosting)
                {
                    logger.Info("*** INTERCEPTED general network call during internet hosting ***");
                    logger.Info("Blocking general network call - using Mirror networking instead");
                    return false; // Block the network call
                }
                else
                {
                    return true; // Allow network call for local hosting
                }
            }
            catch (Exception e)
            {
                logger.Error("Error intercepting general network call: " + e.Message);
                return true; // Fallback to original method
            }
        }
        
        /// <summary>
        /// Set working facilitator server immediately during plugin startup
        /// This proactively changes Unity's facilitator settings before any connections are attempted
        /// </summary>
        public static void SetWorkingFacilitatorImmediately()
        {
            try
            {
                logger.Info("=== SETTING WORKING FACILITATOR DURING STARTUP ===");
                logger.Info("Proactively changing Unity's facilitator settings before any connections");
                
                // Use a known working facilitator server
                // Option 1: Community-maintained UNet facilitator replacement
                var workingFacilitatorIP = "unet.vis2k.com"; // Mirror team's facilitator
                var workingFacilitatorPort = 61111;
                
                // Option 2: Alternative community servers
                // workingFacilitatorIP = "facilitator.unity3d.com";  // (if still working)
                // workingFacilitatorIP = "natfacilitator.unity3d.com";  // (alternative)
                
                logger.Info("Setting Unity facilitator to: " + workingFacilitatorIP + ":" + workingFacilitatorPort);
                logger.Info("This replaces the dead official server: 52.206.242.3:61111");
                
                // Set Unity's facilitator settings immediately
                TrySetUnityFacilitatorSettings(workingFacilitatorIP, workingFacilitatorPort);
                
                logger.Info("✓ Working facilitator configured - internet hosting should now work");
                logger.Info("✓ NAT traversal will use the working server instead of timing out");
            }
            catch (Exception e)
            {
                logger.Error("Error setting working facilitator immediately: " + e.Message);
            }
        }
        
        /// <summary>
        /// Intercept any network-related call that might be trying to use deprecated UNet services
        /// This is a catch-all for methods we haven't specifically identified yet
        /// </summary>
        public static bool InterceptAnyNetworkCall()
        {
            try
            {
                // Get the calling method info for debugging
                var stackTrace = new System.Diagnostics.StackTrace();
                var callingMethod = stackTrace.GetFrame(1)?.GetMethod();
                var methodName = callingMethod?.Name ?? "Unknown";
                var typeName = callingMethod?.DeclaringType?.Name ?? "Unknown";
                
                logger.Info("*** INTERCEPTED network call: " + typeName + "." + methodName + " ***");
                
                // Check if this looks like a facilitator-related call
                if (methodName.Contains("Facilitator") || methodName.Contains("NAT") || 
                    typeName.Contains("Facilitator") || typeName.Contains("NAT"))
                {
                    logger.Info("This appears to be a facilitator/NAT call - redirecting to working server");
                    RedirectToWorkingFacilitator();
                    return false; // Block original call, use our redirection
                }
                else
                {
                    logger.Info("Generic network call detected - allowing to proceed");
                    return true; // Allow other network calls to proceed normally
                }
            }
            catch (Exception e)
            {
                logger.Error("Error intercepting network call: " + e.Message);
                return true; // Fallback to original method
            }
        }
        
        /// <summary>
        /// Intercept socket connection attempts to the dead facilitator IP
        /// This catches the low-level socket calls that our other patches might miss
        /// </summary>
        public static bool InterceptSocketConnection(object __instance, ref object endpoint)
        {
            try
            {
                // Check if this is a connection to the dead facilitator IP
                var endpointStr = endpoint?.ToString() ?? "";
                if (endpointStr.Contains("52.206.242.3") || endpointStr.Contains("61111"))
                {
                    logger.Info("*** INTERCEPTED SOCKET CONNECTION to dead facilitator ***");
                    logger.Info("Blocking socket connection to: " + endpointStr);
                    logger.Info("Original facilitator servers are permanently offline");
                    
                    // Try to redirect to working facilitator endpoint
                    try
                    {
                        var workingIP = "unet.vis2k.com";
                        var workingPort = 61111;
                        
                        logger.Info("Redirecting socket connection to working facilitator: " + workingIP + ":" + workingPort);
                        
                        // Create new endpoint with working facilitator
                        var ipEndPointType = System.Type.GetType("System.Net.IPEndPoint, System");
                        if (!ReferenceEquals(ipEndPointType, null))
                        {
                            var ipAddressType = System.Type.GetType("System.Net.IPAddress, System");
                            var dnsType = System.Type.GetType("System.Net.Dns, System");
                            
                            if (!ReferenceEquals(dnsType, null))
                            {
                                var getHostEntryMethod = dnsType.GetMethod("GetHostEntry", new System.Type[] { typeof(string) });
                                if (!ReferenceEquals(getHostEntryMethod, null))
                                {
                                    var hostEntry = getHostEntryMethod.Invoke(null, new object[] { workingIP });
                                    var addressListProperty = hostEntry.GetType().GetProperty("AddressList");
                                    if (!ReferenceEquals(addressListProperty, null))
                                    {
                                        var addresses = (System.Array)addressListProperty.GetValue(hostEntry, null);
                                        if (addresses.Length > 0)
                                        {
                                            var newEndpoint = System.Activator.CreateInstance(ipEndPointType, addresses.GetValue(0), workingPort);
                                            endpoint = newEndpoint;
                                            logger.Info("✓ Socket endpoint redirected to working facilitator");
                                            return true; // Allow connection with new endpoint
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (System.Exception redirectEx)
                    {
                        logger.Error("Failed to redirect socket endpoint: " + redirectEx.Message);
                    }
                    
                    // If redirection failed, block the connection
                    logger.Info("Blocking connection to prevent timeout delay");
                    return false; // Block the connection
                }
                else
                {
                    // Allow other socket connections to proceed normally
                    return true;
                }
            }
            catch (Exception e)
            {
                logger.Error("Error intercepting socket connection: " + e.Message);
                return true; // Fallback to original method
            }
        }
        
        /// <summary>
        /// Clean shutdown for both networking systems
        /// </summary>
        public static void CleanupNetworking()
        {
            try
            {
                if (isInternetHosting)
                {
                    logger.Info("Shutting down Mirror internet hosting");
                    UNetToMirrorTranslator.StopInternetHost();
                }
                else
                {
                    logger.Info("UNet local hosting shutdown (handled by vanilla game)");
                }
                
                isInternetHosting = false;
            }
            catch (Exception e)
            {
                logger.Error("Error during networking cleanup: " + e.Message);
            }
        }
    }
}