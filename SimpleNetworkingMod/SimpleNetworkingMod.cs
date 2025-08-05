using System;
using UnityEngine;

/// <summary>
/// Simple .NET 2.0 compatible networking replacement mod
/// This mod provides basic networking replacements without using any .NET 3.5+ features
/// </summary>
public class SimpleNetworkingMod : ISrPlugin
{
    public string GetName()
    {
        return "Simple Networking Replacement v1.0.0 - .NET 2.0 Compatible";
    }

    public void Update()
    {
        // No frame updates needed for this mod
    }

    public void Initialize()
    {
        try
        {
            Debug.Log("[SimpleNetworkingMod] *** SIMPLE NETWORKING MOD INITIALIZING ***");
            Debug.Log("[SimpleNetworkingMod] .NET 2.0 compatible networking replacement");
            
            // Create basic networking replacements
            CreateBasicNetworkingObjects();
            
            Debug.Log("[SimpleNetworkingMod] *** INITIALIZATION COMPLETE - MOD LOADED SUCCESSFULLY ***");
        }
        catch (Exception e)
        {
            Debug.LogError("[SimpleNetworkingMod] Initialization failed: " + e.Message);
            Debug.LogError("[SimpleNetworkingMod] Stack trace: " + e.StackTrace);
        }
    }

    private void CreateBasicNetworkingObjects()
    {
        Debug.Log("[SimpleNetworkingMod] Creating basic networking objects...");
        
        // Create a very simple network manager
        GameObject networkManagerGO = new GameObject("SimpleNetworkManager");
        SimpleNetworkManager manager = networkManagerGO.AddComponent<SimpleNetworkManager>();
        UnityEngine.Object.DontDestroyOnLoad(networkManagerGO);
        
        Debug.Log("[SimpleNetworkingMod] Basic networking objects created");
    }
}

/// <summary>
/// Very simple network manager that avoids all modern .NET features
/// </summary>
public class SimpleNetworkManager : MonoBehaviour
{
    private static SimpleNetworkManager _instance;
    private bool _isHost = false;
    private bool _isServer = false;
    private bool _isClient = false;
    
    public static SimpleNetworkManager Instance
    {
        get { return _instance; }
    }
    
    public bool isHost { get { return _isHost; } }
    public bool isServer { get { return _isServer; } }
    public bool isClient { get { return _isClient; } }
    
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("[SimpleNetworkManager] Duplicate network manager found, destroying");
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        Debug.Log("[SimpleNetworkManager] *** SIMPLE NETWORK MANAGER CREATED ***");
        Debug.Log("[SimpleNetworkManager] *** THIS MESSAGE PROVES THE MOD IS LOADING ***");
    }
    
    public bool StartHost()
    {
        Debug.Log("[SimpleNetworkManager] StartHost called - attempting to start host");
        _isHost = true;
        _isServer = true;
        _isClient = true;
        
        Debug.Log("[SimpleNetworkManager] Host started successfully (simulated)");
        return true;
    }
    
    public void StopHost()
    {
        Debug.Log("[SimpleNetworkManager] StopHost called");
        _isHost = false;
        _isServer = false;
        _isClient = false;
    }
    
    public bool StartServer()
    {
        Debug.Log("[SimpleNetworkManager] StartServer called");
        _isServer = true;
        return true;
    }
    
    public void StopServer()
    {
        Debug.Log("[SimpleNetworkManager] StopServer called");
        _isServer = false;
        if (!_isClient)
        {
            _isHost = false;
        }
    }
    
    public bool StartClient()
    {
        Debug.Log("[SimpleNetworkManager] StartClient called");
        _isClient = true;
        return true;
    }
    
    public void StopClient()
    {
        Debug.Log("[SimpleNetworkManager] StopClient called");
        _isClient = false;
        if (!_isServer)
        {
            _isHost = false;
        }
    }
}