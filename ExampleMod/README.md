# ExampleMod - Simple Satellite Reign Mod

A beginner-friendly example mod demonstrating core Satellite Reign modding concepts.

## Features

### Health Monitoring System
- **Automatic Monitoring**: Continuously monitors all agent health levels
- **Low Health Alerts**: Shows warnings when 2+ agents have less than 30% health
- **Real-time Updates**: Checks every 2 seconds during gameplay

### Interactive Controls
- **F9 Key**: Display detailed health and energy information for all agents
- **F10 Key**: Instantly heal all agents to full health and energy

### Information Display
- Health percentages with visual status indicators (✓, ⚡, ⚠)
- Energy levels alongside health information
- Agent count and summary statistics
- User-friendly popup messages

## Installation

1. **Build the Mod**:
   - Open `SRMod.sln` in Visual Studio
   - Build the ExampleMod project
   - Find `ExampleMod.dll` in `ExampleMod\bin\Debug\` or `ExampleMod\bin\Release\`

2. **Install to Game**:
   - Copy `ExampleMod.dll` to your Satellite Reign installation's `Mods` folder
   - Example: `C:\Program Files (x86)\Steam\steamapps\common\SatelliteReign\Mods\`

3. **Verify Installation**:
   - Launch Satellite Reign
   - Start or load a game
   - You should see "Example Mod Loaded! Press F9 for agent health info." message

## Usage

### Health Information (F9)
Press F9 to see a detailed popup showing:
- Each agent's current health and maximum health
- Health percentage with status icons
- Energy levels and percentages
- Total agent count and low health count

### Quick Heal (F10)
Press F10 to instantly:
- Restore all agents to full health
- Refill all agents' energy
- Get confirmation of how many agents were healed

### Automatic Alerts
The mod automatically:
- Monitors agent health every 2 seconds
- Shows subtitle warnings when multiple agents are low on health
- Provides helpful reminders about healing options

## Code Structure

### Core Components
- **Initialize()**: Sets up the mod when the game loads
- **Update()**: Handles input and periodic health checks
- **MonitorAgentHealth()**: Automatic health monitoring system
- **ShowAgentHealthInfo()**: Detailed health information display
- **HealAllAgents()**: Healing functionality

### Error Handling
- Try-catch blocks protect against game state changes
- Null checks prevent crashes when agents are unavailable
- Debug logging for troubleshooting

### Game Integration
- Uses `Manager.GetUIManager()` for user interface
- Accesses agent data through `AgentAI.GetAgents()`
- Integrates with game's health and energy systems
- Respects game state with `Manager.Get().GameInProgress`

## Modding Concepts Demonstrated

1. **Plugin Interface Implementation**: Shows how to implement `ISrPlugin`
2. **Game State Monitoring**: Demonstrates safe checking of game progression
3. **Agent System Access**: Shows how to iterate through and modify agents
4. **User Interface Integration**: Uses game's popup and subtitle systems
5. **Input Handling**: Responds to keyboard input during gameplay
6. **Periodic Operations**: Implements timer-based background tasks
7. **Error Handling**: Robust exception handling for stable operation

## Extension Ideas

This mod provides a foundation for more advanced features:

- **Custom Health Thresholds**: Configurable warning levels
- **Agent Selection**: Focus on specific agent types or selected agents
- **Status Effects**: Monitor buffs, debuffs, and other conditions
- **Save/Load Integration**: Persistent health tracking across sessions
- **Network Synchronization**: Multiplayer health sharing
- **Advanced UI**: Custom UI panels instead of popup messages

## Technical Notes

- **Target Framework**: .NET Framework 3.5 (Unity compatibility)
- **Dependencies**: Uses standard Satellite Reign game assemblies
- **Performance**: Optimized for minimal impact on game performance
- **Compatibility**: Works with all existing mods and game features

This example demonstrates the power and simplicity of Satellite Reign's modding system while providing practical functionality for players.