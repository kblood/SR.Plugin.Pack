using System;
using UnityEngine;

namespace ExampleMod
{
    /// <summary>
    /// Simple example mod demonstrating basic Satellite Reign modding concepts.
    /// Shows agent health monitoring, input handling, and UI interaction.
    /// </summary>
    public class ExampleMod : ISrPlugin
    {
        #region Fields
        private bool isInitialized = false;
        private float lastHealthCheck = 0f;
        private const float healthCheckInterval = 2f; // Check every 2 seconds
        private int lowHealthAgentCount = 0;
        #endregion

        #region ISrPlugin Implementation
        public void Initialize()
        {
            try
            {
                Debug.Log("ExampleMod: Initializing...");
                
                lastHealthCheck = Time.time;
                isInitialized = true;
                
                // Show initialization message to player
                if (Manager.Get() != null && Manager.GetUIManager() != null)
                {
                    Manager.GetUIManager().ShowMessagePopup("Example Mod Loaded! Press F9 for agent health info.", 5);
                }
                
                Debug.Log("ExampleMod: Initialization complete");
            }
            catch (Exception e)
            {
                Debug.LogError($"ExampleMod: Initialization failed: {e.Message}");
            }
        }

        public void Update()
        {
            if (!isInitialized || !Manager.Get().GameInProgress)
                return;

            try
            {
                // Handle input
                HandleUserInput();
                
                // Periodic health monitoring
                if (Time.time > lastHealthCheck + healthCheckInterval)
                {
                    MonitorAgentHealth();
                    lastHealthCheck = Time.time;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"ExampleMod: Update error: {e.Message}");
            }
        }

        public string GetName()
        {
            return "Example Mod v1.0";
        }
        #endregion

        #region Private Methods
        private void HandleUserInput()
        {
            // F9 - Show detailed agent health information
            if (Input.GetKeyDown(KeyCode.F9))
            {
                ShowAgentHealthInfo();
            }
            
            // F10 - Heal all agents (demonstration of game modification)
            if (Input.GetKeyDown(KeyCode.F10))
            {
                HealAllAgents();
            }
        }

        private void MonitorAgentHealth()
        {
            lowHealthAgentCount = 0;
            
            foreach (AgentAI agent in AgentAI.GetAgents())
            {
                if (agent != null && agent.m_Health != null)
                {
                    float healthPercentage = agent.m_Health.HealthValue / agent.m_Health.GetMaxHealth();
                    
                    // Count agents with less than 30% health
                    if (healthPercentage < 0.3f && healthPercentage > 0f)
                    {
                        lowHealthAgentCount++;
                    }
                }
            }
            
            // Alert player if multiple agents are low on health
            if (lowHealthAgentCount >= 2)
            {
                Manager.GetUIManager().ShowSubtitle(
                    $"Warning: {lowHealthAgentCount} agents have low health! Press F10 to heal all agents.", 
                    3);
            }
        }

        private void ShowAgentHealthInfo()
        {
            string healthInfo = "=== Agent Health Status ===\n\n";
            int agentCount = 0;
            
            foreach (AgentAI agent in AgentAI.GetAgents())
            {
                if (agent != null && agent.m_Health != null)
                {
                    agentCount++;
                    string agentName = agent.AgentClassName();
                    float currentHealth = agent.m_Health.HealthValue;
                    float maxHealth = agent.m_Health.GetMaxHealth();
                    float healthPercentage = (currentHealth / maxHealth) * 100f;
                    
                    string statusIcon = "✓";
                    if (healthPercentage < 30f) statusIcon = "⚠";
                    else if (healthPercentage < 60f) statusIcon = "⚡";
                    
                    healthInfo += $"{statusIcon} {agentName}: {currentHealth:F0}/{maxHealth:F0} ({healthPercentage:F1}%)\n";
                    
                    // Add energy info
                    if (agent.m_Energy != null)
                    {
                        float currentEnergy = agent.m_Energy.EnergyValue;
                        float maxEnergy = agent.m_Energy.GetMaxEnergy();
                        float energyPercentage = (currentEnergy / maxEnergy) * 100f;
                        healthInfo += $"   Energy: {currentEnergy:F0}/{maxEnergy:F0} ({energyPercentage:F1}%)\n";
                    }
                    
                    healthInfo += "\n";
                }
            }
            
            if (agentCount == 0)
            {
                healthInfo += "No agents found in the current game state.";
            }
            else
            {
                healthInfo += $"\nTotal Agents: {agentCount}\n";
                healthInfo += $"Low Health Agents: {lowHealthAgentCount}\n\n";
                healthInfo += "Press F10 to heal all agents";
            }
            
            // Display the information using the game's popup system
            Manager.GetUIManager().ShowMessagePopup(healthInfo, 10);
        }

        private void HealAllAgents()
        {
            int healedCount = 0;
            
            foreach (AgentAI agent in AgentAI.GetAgents())
            {
                if (agent != null && agent.m_Health != null)
                {
                    float healthBefore = agent.m_Health.HealthValue;
                    
                    // Heal the agent to full health
                    agent.m_Health.SetHealthFull();
                    
                    // Also restore energy
                    if (agent.m_Energy != null)
                    {
                        agent.m_Energy.AddEnergy(agent.m_Energy.GetMaxEnergy());
                    }
                    
                    float healthAfter = agent.m_Health.HealthValue;
                    
                    // Only count as healed if there was actual healing done
                    if (healthAfter > healthBefore)
                    {
                        healedCount++;
                    }
                }
            }
            
            string message = healedCount > 0 
                ? $"Healed {healedCount} agents to full health and energy!"
                : "All agents are already at full health.";
                
            Manager.GetUIManager().ShowMessagePopup(message, 5);
            
            // Reset the low health count after healing
            lowHealthAgentCount = 0;
        }
        #endregion
    }
}