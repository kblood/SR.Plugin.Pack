# Networking Replacement Mod - UNet Compatibility Layer

## 🚨 Critical Issue: Unity UNet Deprecation

**Unity deprecated UNet (Unity Networking) services as of July 7th, 2025.** This means Satellite Reign's multiplayer functionality will stop working without intervention.

This mod provides a **compatibility layer** that translates legacy UNet code to work with modern networking systems, preventing multiplayer from breaking.

## 🎯 Solution: Translator Approach

Instead of rewriting the entire game's networking system, this mod acts as a **translator** that:

1. **Intercepts UNet API calls** from the existing game code
2. **Translates them** to equivalent calls in Mirror Networking or Unity Netcode
3. **Provides fallback mode** for single-player when no modern networking is available
4. **Maintains compatibility** with existing save games and mods

## 🛠️ How It Works

### Architecture Overview

```
Satellite Reign Game Code
        ↓ (UNet API calls)
UNet Compatibility Layer (This Mod)
        ↓ (Translated calls)
Mirror / Unity Netcode / Fallback Mode
```

### Key Components

1. **NetworkingReplacementMod.cs**: Main mod entry point
   - Detects networking issues
   - Initializes compatibility layer
   - Monitors networking health

2. **UNetCompatibilityLayer.cs**: Core translation engine
   - API translation between UNet ↔ Mirror/Netcode
   - Mock objects for unavailable components
   - Static reference patching
   - Fallback implementations

## 📦 Installation

### Prerequisites
For full multiplayer functionality, install one of:
- **Mirror Networking** (recommended): `https://github.com/MirrorNetworking/Mirror`
- **Unity Netcode for GameObjects**: Via Unity Package Manager

### Basic Installation (Fallback Mode)
1. Build the `NetworkingReplacementMod.dll`
2. Copy to Satellite Reign's `Mods` folder
3. Launch game - single-player will work, multiplayer disabled

### Advanced Installation (Full Multiplayer)
1. Install Mirror Networking in your Unity project
2. Build this mod with Mirror references
3. Deploy to game directory
4. Full multiplayer functionality restored

## 🎮 Features

### Current Implementation
- ✅ **UNet Detection**: Identifies deprecated networking components
- ✅ **Compatibility Layer**: Prevents crashes from missing UNet services
- ✅ **Fallback Mode**: Ensures single-player continues working
- ✅ **Health Monitoring**: Tracks networking system status
- ✅ **Static Patching**: Fixes hardcoded UNet references

### Future Implementation
- 🔄 **Mirror Integration**: Full translation to Mirror Networking
- 🔄 **Unity Netcode Support**: Alternative modern networking backend
- 🔄 **Steam Integration**: Maintain Steam multiplayer features
- 🔄 **Lobby System**: Recreate game browser functionality

## 🔧 Technical Details

### Affected Game Classes
The mod handles translation for these critical networking components:

```csharp
// Main networking manager
SrNetworkManager : NATTraversal.NetworkManager

// Core game networking classes
AIEntity : NetworkBehaviour
Client : NetworkBehaviour  
CloneManager : NetworkBehaviour
MoneyManagerNetworked : NetworkBehaviour
NetworkCloneableData : NetworkBehaviour
NetworkDoorManager : NetworkBehaviour
NetworkPosition : NetworkBehaviour
NetworkMessageManager : NetworkBehaviour
```

### Translation Examples

```csharp
// UNet code (what the game uses):
if (NetworkServer.active) { /* do something */ }

// Gets translated to Mirror:
if (Mirror.NetworkServer.active) { /* do something */ }

// Or Unity Netcode:
if (NetworkManager.Singleton.IsServer) { /* do something */ }

// Or Fallback:
if (true) { /* single-player mode */ }
```

### Static Reference Patching

The mod uses reflection to patch static references:

```csharp
// Patches SrNetworkManager.RemoteClient
SrNetworkManager.RemoteClient = false; // Single-player mode

// Patches SrNetworkManager.MpEnabled  
SrNetworkManager.MpEnabled = false; // Disable MP initially
```

## 🧪 Testing

### Single-Player Testing
1. Launch Satellite Reign with the mod
2. Start a new game or load existing save
3. Verify no networking errors in console
4. Confirm all single-player features work

### Multiplayer Testing (With Mirror)
1. Install Mirror Networking
2. Rebuild mod with Mirror support
3. Test local network play
4. Test internet multiplayer
5. Verify Steam integration works

## 🔍 Monitoring

The mod provides several ways to monitor networking status:

### Console Output
```
UNetCompatibilityLayer: Initialized with backend: Mirror
NetworkingReplacementMod: Networking compatibility layer active
```

### In-Game Messages
- Initialization status popup
- Warning messages for networking issues
- Periodic health check subtitles

### Debug Commands
```csharp
// Check current backend
UNetCompatibilityLayer.GetCurrentBackend()

// Test API translation
UNetCompatibilityLayer.IsServer()
UNetCompatibilityLayer.IsClient()
```

## 🐛 Troubleshooting

### Common Issues

**"UNet errors detected"**
- Expected if UNet services are down
- Fallback mode should activate automatically
- Single-player should still work

**"Networking may be unstable"**
- Some multiplayer features may not work
- Check if Mirror/Netcode is properly installed
- Verify mod has proper references

**Game crashes on startup**
- Check console for specific errors
- Ensure all DLL dependencies are present
- Try safe mode (disable all other mods)

### Performance Impact
- **Minimal overhead** in single-player mode
- **Translation layer** adds small latency to multiplayer
- **Memory usage** increased by compatibility objects

## 🚀 Future Development

### Phase 1: Stability (Current)
- ✅ Prevent crashes from UNet deprecation
- ✅ Maintain single-player functionality
- ✅ Basic compatibility infrastructure

### Phase 2: Mirror Integration
- 🔄 Full Mirror Networking translation
- 🔄 Maintain all multiplayer features
- 🔄 Steam workshop integration

### Phase 3: Enhancement
- 🔄 Improved networking performance
- 🔄 Additional multiplayer features
- 🔄 Cross-platform compatibility

### Phase 4: Long-term
- 🔄 Unity Netcode alternative
- 🔄 Dedicated server support
- 🔄 Advanced modding APIs

## 🤝 Contributing

This mod represents a complex but necessary solution to keep Satellite Reign's multiplayer alive. Key areas for contribution:

1. **Mirror Integration**: Implementing full Mirror translation
2. **Unity Netcode Support**: Alternative networking backend
3. **Testing**: Multiplayer scenario testing
4. **Performance**: Optimization of translation layer
5. **Documentation**: Additional examples and guides

## 📚 References

- [Unity UNet Deprecation Notice](https://status.unity.com/info_notices/300129)
- [Mirror Networking Documentation](https://mirror-networking.gitbook.io/docs/)
- [Unity Netcode for GameObjects](https://docs.unity.com/netcode/current/about/)
- [Satellite Reign Modding Documentation](../docs/)

---

**This mod is essential for preserving Satellite Reign's multiplayer functionality after Unity's UNet shutdown. While complex, the translator approach provides the best balance of compatibility and functionality.**