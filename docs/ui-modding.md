# UI Modding and Custom User Interfaces

## Overview

Satellite Reign's UI system is built on Unity's UI framework and provides several ways to create custom interfaces, display messages, and enhance user interaction. This guide covers UI integration, custom UI creation, and advanced interface techniques.

## Core UI Systems

### UIManager - Primary UI Control

The `UIManager` is the central system for all UI operations:

```csharp
UIManager uiManager = Manager.GetUIManager();

// Basic message display
uiManager.ShowMessagePopup("Hello World!", 5);
uiManager.ShowWarningPopup("Warning message", 3);
uiManager.ShowSubtitle("Subtitle text", 10);

// Advanced UI elements
uiManager.ShowBannerMessage("Title", "Top Text", "Bottom Text", 5);
uiManager.DoModalMessageBox("Title", "Message", 
    InputBoxUi.InputBoxTypes.MbOkcancel, "OK", "Cancel", callbackFunction);
```

### Message Types and Display

```csharp
public class MessageDisplayer
{
    public void ShowDifferentMessageTypes()
    {
        var ui = Manager.GetUIManager();
        
        // Standard popup (most common)
        ui.ShowMessagePopup("Standard message", 5);
        
        // Warning popup (yellow/orange styling)
        ui.ShowWarningPopup("This is a warning!", 4);
        
        // Subtitle (bottom of screen)
        ui.ShowSubtitle("Subtitle appears at bottom", 6);
        
        // Banner message (prominent display)
        ui.ShowBannerMessage("Mission Update", 
            "Objective Complete", 
            "Bonus credits awarded", 8);
        
        // Full screen blank with text
        ui.ShowBlank("Loading custom content...");
        
        // Toggle text window
        ui.ToggleTextWindow("This is a toggleable text window");
    }
    
    public void HideMessages()
    {
        var ui = Manager.GetUIManager();
        ui.HideSubtitle();
        // Other hide methods as available
    }
}
```

## Custom UI Framework

### SRModButtonElement - Basic Button System

Create custom buttons using the provided framework:

```csharp
using Cheats.CustomUI;

public class CustomButtonCreator
{
    public SRModButtonElement CreateCustomButton(string text, string description, System.Action action)
    {
        return new SRModButtonElement(text, 
            new UnityEngine.Events.UnityAction(action), description);
    }
    
    public List<SRModButtonElement> CreateButtonSet()
    {
        var buttons = new List<SRModButtonElement>();
        
        buttons.Add(CreateCustomButton(
            "Health Boost", 
            "Restore all agents to full health",
            () => BoostAllAgentHealth()));
            
        buttons.Add(CreateCustomButton(
            "Add Money", 
            "Add 10,000 credits",
            () => Manager.GetMoneyManager().OffsetFunds(10000, true, true)));
            
        buttons.Add(CreateCustomButton(
            "Teleport Agents", 
            "Move all agents to mouse position",
            () => TeleportAgentsToMousePosition()));
        
        return buttons;
    }
    
    private void BoostAllAgentHealth()
    {
        foreach (var agent in AgentAI.GetAgents())
        {
            agent.SetHealthValue(100);
            agent.m_Energy.SetInfiniteEnergy(false);
            agent.m_Energy.AddEnergy(500);
        }
        Manager.GetUIManager().ShowMessagePopup("All agents healed!", 3);
    }
    
    private void TeleportAgentsToMousePosition()
    {
        Vector3 mousePos;
        if (Manager.GetInputControl().GetGroundPointUnderMouse(out mousePos))
        {
            foreach (var agent in AgentAI.GetAgents())
            {
                agent.transform.position = mousePos;
            }
            Manager.GetUIManager().ShowMessagePopup("Agents teleported!", 2);
        }
    }
}
```

### SRModVerticalButtonsUI - Advanced Button Layout

Create vertical button layouts with the advanced UI system:

```csharp
using Cheats.Services;
using Cheats.CustomUI;

public class AdvancedUICreator : ISrPlugin
{
    private List<SRModButtonElement> uiButtons;
    
    public void Initialize()
    {
        CreateUIButtons();
    }
    
    public void Update()
    {
        if (!Manager.Get().GameInProgress)
            return;
            
        // Show UI on F5 key
        if (Input.GetKeyDown(KeyCode.F5))
        {
            ShowCustomUI();
        }
    }
    
    private void CreateUIButtons()
    {
        uiButtons = new List<SRModButtonElement>();
        
        // Create toggle buttons with state management
        uiButtons.Add(new SRModButtonElement(
            "Toggle God Mode",
            new UnityEngine.Events.UnityAction(() => ToggleGodMode()),
            "Make agents invulnerable"));
            
        uiButtons.Add(new SRModButtonElement(
            "Weapon Enhancer",
            new UnityEngine.Events.UnityAction(() => EnhanceWeapons()),
            "Upgrade all weapon stats"));
            
        uiButtons.Add(new SRModButtonElement(
            "Speed Control",
            new UnityEngine.Events.UnityAction(() => ShowSpeedMenu()),
            "Adjust game speed"));
    }
    
    private void ShowCustomUI()
    {
        Manager.Get().StartCoroutine(
            UIHelper.ModalVerticalButtonsRoutine("Advanced Mod Menu", uiButtons));
    }
    
    private bool godModeActive = false;
    
    private void ToggleGodMode()
    {
        godModeActive = !godModeActive;
        
        foreach (var agent in AgentAI.GetAgents())
        {
            agent.m_Health.Invulnerable = godModeActive;
            agent.m_Energy.SetInfiniteEnergy(godModeActive);
        }
        
        string status = godModeActive ? "enabled" : "disabled";
        UIHelper.ShowMessage($"God mode {status}", 3);
    }
    
    private void EnhanceWeapons()
    {
        // Weapon enhancement logic
        UIHelper.ShowMessage("All weapons enhanced!", 3);
    }
    
    private void ShowSpeedMenu()
    {
        var speedButtons = new List<SRModButtonElement>
        {
            new SRModButtonElement("0.5x Speed", 
                new UnityEngine.Events.UnityAction(() => SetGameSpeed(0.5f)), 
                "Half speed"),
            new SRModButtonElement("Normal Speed", 
                new UnityEngine.Events.UnityAction(() => SetGameSpeed(1.0f)), 
                "Default speed"),
            new SRModButtonElement("2x Speed", 
                new UnityEngine.Events.UnityAction(() => SetGameSpeed(2.0f)), 
                "Double speed")
        };
        
        Manager.Get().StartCoroutine(
            UIHelper.ModalVerticalButtonsRoutine("Speed Control", speedButtons));
    }
    
    private void SetGameSpeed(float speed)
    {
        var timeScaler = TimeManager.AddTimeScaler();
        timeScaler.TimeScale = speed;
        UIHelper.ShowMessage($"Game speed set to {speed}x", 2);
    }
    
    public string GetName() => "Advanced UI Creator";
}
```

## Entity Information Panel Integration

### Custom Entity Info Display

Extend the game's entity information system:

```csharp
public class EntityInfoEnhancer
{
    public EntityInfoPanel ShowCustomEntityInfo(string title, string info)
    {
        // Find a suitable AI entity to use as anchor
        AIEntity targetEntity = FindDisplayableEntity();
        
        if (targetEntity == null)
        {
            Manager.GetUIManager().ShowMessagePopup("No suitable entity found for display", 3);
            return null;
        }
        
        // Set entity selection for info panel
        targetEntity.m_Selectable.SetSelected(false);
        targetEntity.CurrentlySelected();
        targetEntity.m_Selectable.SetSelected(true);
        
        // Get and configure entity info panel
        EntityInfoPanel infoPanel = EntityInfoPanel.FindObjectOfType<EntityInfoPanel>();
        
        if (infoPanel != null)
        {
            // Customize the info panel
            infoPanel.m_DetailText.Text = info;
            infoPanel.m_DetailText.m_Text.text = info;
            infoPanel.name = title;
            infoPanel.m_SummaryText.text = title;
            
            // Show the panel
            if (!infoPanel.IsVisible())
            {
                infoPanel.Show();
            }
        }
        
        return infoPanel;
    }
    
    private AIEntity FindDisplayableEntity()
    {
        // Find a non-controllable entity for display
        foreach (AIEntity entity in AIEntity.FindObjectsOfType<AIEntity>())
        {
            if (entity.IsAddedToWorld && !entity.m_IsControllable)
            {
                return entity;
            }
        }
        
        // Fallback: find any entity
        var entities = AIEntity.FindObjectsOfType<AIEntity>();
        return entities.Length > 0 ? entities[0] : null;
    }
    
    public void ShowAgentDetailedInfo(AgentAI agent)
    {
        if (agent == null) return;
        
        string detailedInfo = BuildAgentInfoString(agent);
        ShowCustomEntityInfo($"Agent Details: {agent.GetName()}", detailedInfo);
    }
    
    private string BuildAgentInfoString(AgentAI agent)
    {
        var info = new System.Text.StringBuilder();
        
        info.AppendLine($"Class: {agent.AgentClassName()}");
        info.AppendLine($"Health: {agent.m_Health.HealthValue:F1}");
        info.AppendLine($"Energy: {agent.m_Energy.EnergyValue:F1}");
        info.AppendLine($"Weapon: {agent.GetWeaponType(0)}");
        info.AppendLine($"Selected: {agent.IsSelected()}");
        info.AppendLine($"In Danger: {agent.IsInDanger}");
        
        // Add modifier information
        info.AppendLine("\nModifiers:");
        foreach (var modifier in agent.m_Modifiers.m_DefaultModifiers)
        {
            info.AppendLine($"  {modifier.m_Type}: {modifier.m_Ammount}");
        }
        
        return info.ToString();
    }
}
```

## Modal Dialog System

### Custom Modal Dialogs

Create custom modal dialogs for user interaction:

```csharp
public class ModalDialogCreator
{
    public void ShowConfirmationDialog(string title, string message, 
        System.Action onConfirm, System.Action onCancel = null)
    {
        var uiManager = Manager.GetUIManager();
        
        uiManager.DoModalMessageBox(title, message,
            InputBoxUi.InputBoxTypes.MbOkcancel,
            "Confirm", "Cancel",
            (confirmed) => {
                if (confirmed)
                {
                    onConfirm?.Invoke();
                }
                else
                {
                    onCancel?.Invoke();
                }
            });
    }
    
    public void ShowInputDialog(string title, string prompt, 
        System.Action<string> onInput)
    {
        var uiManager = Manager.GetUIManager();
        
        // Create input dialog
        uiManager.DoModalMessageBox(title, prompt,
            InputBoxUi.InputBoxTypes.MbInputOk,
            "OK", "Cancel",
            (confirmed) => {
                if (confirmed)
                {
                    string inputText = uiManager.InputBoxUi.InputText;
                    onInput?.Invoke(inputText);
                }
            });
    }
    
    public void ShowOptionsDialog(string title, string message, 
        Dictionary<string, System.Action> options)
    {
        // For multiple options, create a custom button interface
        var buttons = new List<SRModButtonElement>();
        
        foreach (var option in options)
        {
            string optionText = option.Key;
            System.Action optionAction = option.Value;
            
            buttons.Add(new SRModButtonElement(
                optionText,
                new UnityEngine.Events.UnityAction(optionAction),
                optionText));
        }
        
        Manager.Get().StartCoroutine(
            UIHelper.ModalVerticalButtonsRoutine(title, buttons));
    }
}

// Usage example:
public class DialogUsageExample : ISrPlugin
{
    private ModalDialogCreator dialogCreator = new ModalDialogCreator();
    
    public void Update()
    {
        if (!Manager.Get().GameInProgress)
            return;
            
        if (Input.GetKeyDown(KeyCode.F6))
        {
            ShowExampleDialogs();
        }
    }
    
    private void ShowExampleDialogs()
    {
        // Confirmation dialog
        dialogCreator.ShowConfirmationDialog(
            "Confirmation Required",
            "Are you sure you want to enhance all weapons?",
            () => {
                EnhanceAllWeapons();
                Manager.GetUIManager().ShowMessagePopup("Weapons enhanced!", 3);
            },
            () => {
                Manager.GetUIManager().ShowMessagePopup("Enhancement cancelled", 2);
            });
    }
    
    private void ShowInputExample()
    {
        dialogCreator.ShowInputDialog(
            "Set Money Amount",
            "Enter the amount of credits to add:",
            (input) => {
                if (int.TryParse(input, out int amount))
                {
                    Manager.GetMoneyManager().OffsetFunds(amount, true, true);
                    Manager.GetUIManager().ShowMessagePopup($"Added ${amount} credits", 3);
                }
                else
                {
                    Manager.GetUIManager().ShowWarningPopup("Invalid amount entered", 3);
                }
            });
    }
    
    private void ShowOptionsExample()
    {
        var options = new Dictionary<string, System.Action>
        {
            ["Heal All Agents"] = () => HealAllAgents(),
            ["Add Money"] = () => Manager.GetMoneyManager().OffsetFunds(10000, true, true),
            ["Enhance Weapons"] = () => EnhanceAllWeapons(),
            ["Cancel"] = () => Manager.GetUIManager().ShowMessagePopup("Cancelled", 1)
        };
        
        dialogCreator.ShowOptionsDialog("Choose Action", "Select an action to perform:", options);
    }
    
    private void HealAllAgents()
    {
        foreach (var agent in AgentAI.GetAgents())
        {
            agent.SetHealthValue(100);
            agent.m_Energy.AddEnergy(500);
        }
        Manager.GetUIManager().ShowMessagePopup("All agents healed!", 3);
    }
    
    private void EnhanceAllWeapons()
    {
        // Weapon enhancement logic
        Manager.GetUIManager().ShowMessagePopup("All weapons enhanced!", 3);
    }
    
    public void Initialize() { }
    public string GetName() => "Dialog Usage Example";
}
```

## UI State Management

### UI Visibility and Control

```csharp
public class UIStateManager
{
    public void ToggleAllUI(bool visible)
    {
        var uiManager = Manager.GetUIManager();
        uiManager.ToggleEverything(visible);
        
        if (visible)
        {
            Manager.ptr.EnableKeyCommands();
        }
        else
        {
            Manager.ptr.DisableKeyCommands();
        }
    }
    
    public void ShowFullScreenMessage(string message)
    {
        var uiManager = Manager.GetUIManager();
        
        // Hide all UI
        ToggleAllUI(false);
        
        // Show full screen message
        uiManager.ShowBlank(message);
        
        // Restore UI after delay (would need coroutine implementation)
        // RestoreUIAfterDelay(3f);
    }
    
    public bool IsInputBoxActive()
    {
        var uiManager = Manager.GetUIManager();
        return uiManager.m_InputBoxUi != null && 
               uiManager.m_InputBoxUi.isActiveAndEnabled;
    }
    
    public void DisableInputControls()
    {
        var inputControl = Manager.GetUIManager().InputControlUi;
        if (inputControl != null)
        {
            inputControl.gameObject.SetActive(false);
        }
    }
    
    public void EnableInputControls()
    {
        var inputControl = Manager.GetUIManager().InputControlUi;
        if (inputControl != null)
        {
            inputControl.gameObject.SetActive(true);
        }
    }
}
```

## Custom UI Actions System

### Action-Based UI Framework

```csharp
using Cheats.CustomUI;

public class CustomUIActions
{
    public string Name { get; set; }
    public System.Func<bool> Action { get; set; }
    public bool Active { get; set; }
    
    public CustomUIActions(string name, System.Func<bool> action)
    {
        Name = name;
        Action = action;
        Active = false;
    }
}

public class UIActionManager : ISrPlugin
{
    private List<CustomUIActions> uiActions;
    
    public void Initialize()
    {
        SetupUIActions();
    }
    
    private void SetupUIActions()
    {
        uiActions = new List<CustomUIActions>();
        
        // Toggle-based actions
        uiActions.Add(new CustomUIActions(
            "God Mode",
            () => {
                ToggleGodMode();
                return true; // Return true to indicate this is a toggle
            }));
            
        uiActions.Add(new CustomUIActions(
            "Speed Boost",
            () => {
                ToggleSpeedBoost();
                return true;
            }));
            
        // One-time actions
        uiActions.Add(new CustomUIActions(
            "Heal All",
            () => {
                HealAllAgents();
                return false; // Return false for one-time actions
            }));
    }
    
    public void ShowActionUI()
    {
        var buttons = new List<SRModButtonElement>();
        
        foreach (var action in uiActions)
        {
            string buttonText = action.Active ? $"Deactivate {action.Name}" : $"Activate {action.Name}";
            
            buttons.Add(new SRModButtonElement(
                buttonText,
                new UnityEngine.Events.UnityAction(() => {
                    bool isToggle = action.Action();
                    if (isToggle)
                    {
                        action.Active = !action.Active;
                        UpdateButtonText(action);
                    }
                }),
                action.Name));
        }
        
        Manager.Get().StartCoroutine(
            UIHelper.ModalVerticalButtonsRoutine("Mod Actions", buttons));
    }
    
    private void UpdateButtonText(CustomUIActions action)
    {
        string status = action.Active ? "enabled" : "disabled";
        UIHelper.ShowMessage($"{action.Name} {status}", 2);
    }
    
    private bool godModeActive = false;
    private bool speedBoostActive = false;
    private TimeManager.TimeScaler speedScaler;
    
    private void ToggleGodMode()
    {
        godModeActive = !godModeActive;
        
        foreach (var agent in AgentAI.GetAgents())
        {
            agent.m_Health.Invulnerable = godModeActive;
            agent.m_Energy.SetInfiniteEnergy(godModeActive);
        }
    }
    
    private void ToggleSpeedBoost()
    {
        speedBoostActive = !speedBoostActive;
        
        if (speedScaler == null)
        {
            speedScaler = TimeManager.AddTimeScaler();
        }
        
        speedScaler.TimeScale = speedBoostActive ? 2f : 1f;
    }
    
    private void HealAllAgents()
    {
        foreach (var agent in AgentAI.GetAgents())
        {
            agent.SetHealthValue(100);
            agent.m_Energy.AddEnergy(500);
        }
        UIHelper.ShowMessage("All agents healed!", 3);
    }
    
    public void Update()
    {
        if (!Manager.Get().GameInProgress)
            return;
            
        if (Input.GetKeyDown(KeyCode.F7))
        {
            ShowActionUI();
        }
    }
    
    public string GetName() => "UI Action Manager";
}
```

## UI Best Practices

### Performance Considerations

```csharp
public class UIPerformanceManager
{
    private float lastUIUpdate = 0f;
    private const float UI_UPDATE_INTERVAL = 0.1f; // Update UI 10 times per second max
    
    public void UpdateUI()
    {
        if (Time.time - lastUIUpdate < UI_UPDATE_INTERVAL)
            return;
            
        lastUIUpdate = Time.time;
        
        // Perform UI updates here
        UpdateUIElements();
    }
    
    private void UpdateUIElements()
    {
        // Batch UI updates together
        // Avoid creating UI elements every frame
        // Cache UI references when possible
    }
}
```

### Error Handling in UI Operations

```csharp
public class SafeUIOperations
{
    public static void SafeShowMessage(string message, int duration = 3)
    {
        try
        {
            var uiManager = Manager.GetUIManager();
            if (uiManager != null && !string.IsNullOrEmpty(message))
            {
                uiManager.ShowMessagePopup(message, duration);
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to show UI message: {e.Message}");
        }
    }
    
    public static void SafeShowCustomUI(string title, List<SRModButtonElement> buttons)
    {
        try
        {
            if (buttons != null && buttons.Count > 0 && !string.IsNullOrEmpty(title))
            {
                Manager.Get().StartCoroutine(
                    UIHelper.ModalVerticalButtonsRoutine(title, buttons));
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to show custom UI: {e.Message}");
            SafeShowMessage("UI Error: Could not display custom interface", 5);
        }
    }
}
```

This comprehensive guide covers all aspects of UI modding in Satellite Reign, from basic message display to complex custom interfaces and state management systems.