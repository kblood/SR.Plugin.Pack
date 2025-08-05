# Mirror Networking Implementation Plan

## Goal
Replace deprecated Unity UNet with Mirror Networking to restore full multiplayer functionality in Satellite Reign.

## Approach: UNet → Mirror Translation Layer

### Phase 1: Core Infrastructure
1. **Download Mirror assemblies** 
   - Mirror.dll
   - Mirror.Components.dll
   - Mirror.Examples.dll

2. **Create Translation Classes**
   - `MirrorNetworkManager` → replaces `UnityEngine.Networking.NetworkManager`
   - `MirrorNetworkBehaviour` → replaces `UnityEngine.Networking.NetworkBehaviour`
   - `MirrorNetworkServer` → replaces `UnityEngine.Networking.NetworkServer`
   - `MirrorNetworkClient` → replaces `UnityEngine.Networking.NetworkClient`

### Phase 2: API Translation
Map UNet API calls to Mirror equivalents:

**NetworkManager:**
```csharp
// UNet → Mirror
NetworkManager.StartHost() → NetworkManager.StartHost()
NetworkManager.StartServer() → NetworkManager.StartServer()  
NetworkManager.StartClient() → NetworkManager.StartClient()
NetworkManager.isServer → NetworkServer.active
NetworkManager.isClient → NetworkClient.active
```

**NetworkBehaviour:**
```csharp
// UNet → Mirror
[Command] → [Command]
[ClientRpc] → [ClientRpc]
[SyncVar] → [SyncVar]
isServer → isServer
isClient → isClient
hasAuthority → hasAuthority
```

**NetworkServer:**
```csharp
// UNet → Mirror
NetworkServer.Spawn() → NetworkServer.Spawn()
NetworkServer.Destroy() → NetworkServer.Destroy()
NetworkServer.active → NetworkServer.active
```

### Phase 3: Game Integration
1. **Patch SrNetworkManager** to use Mirror backend
2. **Replace UNet imports** with Mirror equivalents
3. **Handle networking events** and callbacks
4. **Preserve game networking logic** while changing underlying transport

### Phase 4: Testing
1. **Local hosting** - restore local multiplayer
2. **Network discovery** - LAN game finding
3. **Steam integration** - maintain Steam multiplayer features
4. **Save compatibility** - ensure saves work across networking modes

## Implementation Strategy

### Step 1: Mirror Integration
- Add Mirror.dll to project references
- Create wrapper classes that implement UNet interfaces using Mirror
- Use composition pattern: UNet interface → Mirror implementation

### Step 2: Runtime Patching
- Use reflection to replace UNet types with Mirror wrappers at runtime
- Patch static references in Satellite Reign code
- Maintain API compatibility for existing game code

### Step 3: Transport Layer
- Configure Mirror transport (TCP, WebSockets, or Steam)
- Handle connection management
- Implement discovery services

## Technical Challenges

1. **Assembly Loading**: Mirror assemblies need to be loaded into Unity domain
2. **Type Replacement**: Replace UNet types without breaking existing code
3. **Networking Events**: Mirror events → UNet event callbacks
4. **Steam Integration**: Maintain Steamworks multiplayer features
5. **Performance**: Ensure Mirror performs as well as UNet for this game

## Benefits
- ✅ **Restore full multiplayer** - local hosting, joining, discovery
- ✅ **Future-proof** - Mirror is actively maintained
- ✅ **Better performance** - Mirror often outperforms UNet
- ✅ **Modern networking** - WebGL support, better transport options
- ✅ **Community support** - Large Mirror community for troubleshooting