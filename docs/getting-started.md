# Getting Started with Satellite Reign Modding

## Overview

Satellite Reign is a cyberpunk real-time tactical game built in Unity that supports C# modding through a plugin system. This guide will help you set up your development environment and create your first mod.

## Prerequisites

### Required Software
- **Microsoft Visual Studio** (Visual Studio 2015 recommended, newer versions should work)
- **C# Programming Knowledge** - Basic understanding of C# syntax and object-oriented programming
- **Unity Familiarity** (helpful) - Understanding of Unity's component system and GameObject architecture

### Game Installation
- Satellite Reign installed via Steam, GOG, or other platform
- Access to game installation directory

## Development Environment Setup

### 1. Locate Game Files

Find your Satellite Reign installation directory. The default locations are:
- **Steam**: `C:\Program Files (x86)\Steam\steamapps\common\SatelliteReign\`
- **GOG**: `C:\GOG Games\Satellite Reign\`

### 2. Required Assembly References

In your Satellite Reign installation folder, navigate to `SatelliteReignWindows_Data\Managed\` and locate these DLL files:

- `UnityEngine.dll` - Unity engine functionality
- `Assembly-CSharp.dll` - Game-specific classes and systems
- `Assembly-CSharp-firstpass.dll` - Additional game assemblies

### 3. Create Your First Project

1. **Create New Project**
   - Open Visual Studio
   - Create a new "Class Library (.NET Framework)" project
   - Name it something like "MyFirstSRMod"
   - Target .NET Framework 3.5 or 4.0 (compatible with Unity)

2. **Add Assembly References**
   - Right-click your project in Solution Explorer
   - Select "Add" → "Reference"
   - Click "Browse" and navigate to the game's Managed folder
   - Add all three required DLL files

3. **Set Up Using Statements**
   ```csharp
   using System;
   using System.Collections.Generic;
   using UnityEngine;
   ```

## Your First Plugin

Create a basic plugin by implementing the `ISrPlugin` interface:

```csharp
using UnityEngine;

public class MyFirstMod : ISrPlugin
{
    private float messageTimer = 0f;
    
    public void Initialize()
    {
        Debug.Log("MyFirstMod: Plugin initialized!");
        messageTimer = Time.time + 5f; // Show message after 5 seconds
    }

    public void Update()
    {
        // Only run during gameplay
        if (!Manager.Get().GameInProgress)
            return;
            
        // Show a welcome message once
        if (Time.time > messageTimer && messageTimer > 0)
        {
            Manager.GetUIManager().ShowMessagePopup("Welcome to your first Satellite Reign mod!", 5);
            messageTimer = 0f; // Don't show again
        }
        
        // Press F1 to show another message
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Manager.GetUIManager().ShowMessagePopup("F1 pressed - mod is working!", 3);
        }
    }

    public string GetName()
    {
        return "My First Mod";
    }
}
```

## Building and Installing Your Mod

### 1. Build the Project
- Build your project in Visual Studio (Build → Build Solution)
- The compiled DLL will be in your project's `bin\Debug\` or `bin\Release\` folder

### 2. Install the Mod
1. Navigate to your Satellite Reign installation folder
2. Create a `Mods` folder if it doesn't exist
3. Copy your compiled DLL (e.g., `MyFirstSRMod.dll`) into the `Mods` folder

### 3. Test Your Mod
1. Launch Satellite Reign
2. Start or load a game
3. You should see your welcome message after 5 seconds
4. Press F1 to test the key input functionality

## Common Issues and Solutions

### Assembly Reference Issues
**Problem**: Build errors about missing references
**Solution**: Ensure all three DLL files are properly referenced and the target framework matches

### Mod Not Loading
**Problem**: Mod doesn't appear to work in-game
**Solutions**:
- Check that the DLL is in the correct `Mods` folder
- Verify the class implements `ISrPlugin` correctly
- Look for any compilation errors

### Game Crashes
**Problem**: Game crashes when mod loads
**Solutions**:
- Check for exceptions in your `Initialize()` method
- Ensure all referenced classes exist in the game assemblies
- Add try-catch blocks around risky operations

## Project Structure Best Practices

Organize your mod project like this:
```
MyMod/
├── MyMod.cs                # Main plugin class
├── Services/               # Utility classes
│   ├── DataManager.cs
│   └── UIHelper.cs
├── Models/                 # Data structures
│   └── CustomData.cs
└── Properties/
    └── AssemblyInfo.cs
```

## Next Steps

Once you have a working basic mod:

1. Explore the [Plugin Architecture](plugin-architecture.md) guide for advanced plugin patterns
2. Learn about [Game Systems](game-systems.md) to interact with different game components
3. Check out the [Examples](examples/) folder for more complex implementations
4. Read about [Custom Weapons](custom-weapons.md) and [Custom Items](custom-items.md) for content creation

## Debugging Tips

### Console Output
Use `Debug.Log()` for console output, but note that Satellite Reign may not show Unity console by default.

### In-Game Messages
Use the UI system for debugging:
```csharp
Manager.GetUIManager().ShowMessagePopup("Debug: Variable value is " + myVariable, 5);
```

### Exception Handling
Always wrap risky code in try-catch blocks:
```csharp
try
{
    // Potentially risky operation
    DoSomethingRisky();
}
catch (Exception e)
{
    Manager.GetUIManager().ShowMessagePopup("Error: " + e.Message, 10);
}
```

This concludes the getting started guide. You now have the foundation to create Satellite Reign mods!