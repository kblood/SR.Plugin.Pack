using UltimateIronmanGameMode.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UltimateIronmanGameMode
{
    public class UltimateIronman : ISrPlugin
    {
        protected TimeManager.TimeScaler m_TacticalPauseTimeScaler;
        private float timer;

        private float autoSaveTimer;

        private bool shownModInfo = false;
        private bool ironmanMode = false;

        public void Initialize()
        {
            Debug.Log("Initializing Satellite Reign Ultimate Ironman mod");
            m_TacticalPauseTimeScaler = TimeManager.AddTimeScaler();
            shownModInfo = false;
            ironmanMode = false;
        }

        public void Update()
        {
            if (Manager.Get().IsLoading())
                shownModInfo = false;

            //Check if unstuck is ready
            if (Manager.Get().GameInProgress)
            if (timer < Time.time)
            {
                AIEntity spawnLocation = null;
                if (Input.GetKeyDown(KeyCode.F11) || Input.GetKeyDown(KeyCode.Minus))
                {
                    foreach (AgentAI a in AgentAI.GetAgents())
                    {
                        if (a.IsDowned)
                            a.RespawnAt(Manager.GetInputControl().GetClosestPos(a.transform.position), spawnLocation.transform.rotation);
                        //a.RespawnAt(spawnLocation.transform.position, spawnLocation.transform.rotation);
                        else
                            a.transform.position = Manager.GetInputControl().GetClosestPos(a.transform.position);
                        //a.Teleport(spawnLocation.transform);
                    }
                    timer = Time.time + 120;
                    Manager.GetUIManager().ShowMessagePopup("All agents unstuck. Now the unstuck function will be disabled for 2 minutes", 3);
                }
            }
            else if (Input.GetKeyDown(KeyCode.F11))
            {
                int secondsLeft = Mathf.RoundToInt(timer - Time.time);
                int minutesLeft = secondsLeft / 60;
                secondsLeft -= (minutesLeft * 60);
                Manager.GetUIManager().ShowMessagePopup("Unstuck function locked." + " There is " + minutesLeft + " minutes and " + secondsLeft + " seconds until the UnStuck function is ready again.", 7);
                //setEntityInfo("Unstuck function unnavailable", "There is " + minutesLeft + " minutes and " + secondsLeft + " seconds until the UnStuck function is ready again.");
            }

            if (Manager.Get().GameInProgress)
            {
                if(!shownModInfo)
                {
                    shownModInfo = true;
                    if (SaveGame.CurrentUser.UserConfiguration.IronmanMode)
                    {
                        ironmanMode = true;
                        StopSaves();
                        Manager.GetUIManager().DoModalMessageBox("PermaDeath!", "Ultimate Ironman Gamemode Mod is active. Beware that this mod deletes your savegames if an agent dies, removes the agent and then autosaves. Good luck and have fun", InputBoxUi.InputBoxTypes.MbOk);
                        //+ string.Join(",", Manager.GetPluginManager().LoadedPlugins.Select(p => p.FullPath).ToArray())
                    }
                    else
                        ironmanMode = false;

                }
                if(ironmanMode && autoSaveTimer < Time.time)
                {
                    autoSaveTimer = Time.time + 150;
                    DeleteAllSaves();
                    Manager.Get().DoSaveGame(0);
                    Manager.Get().AutoSave();
                    Manager.DoSaveAuto();
                }

                if (Input.GetKeyDown(KeyCode.Pause))
                {
                    if (m_TacticalPauseTimeScaler.Paused)
                    {
                        m_TacticalPauseTimeScaler.Reset();
                    }
                    else
                    {
                        m_TacticalPauseTimeScaler.Pause();
                    }
                }
            }
        }

        void StopSaves()
        {
            if (SaveGame.CurrentUser.UserConfiguration.IronmanMode)
                return;

            System.Collections.Generic.List<AgentAI> agents = AgentAI.GetAgents();
            foreach (AgentAI agentAI in agents)
            {
                agentAI.gameObject.AddComponent(typeof(OnAgentDeath));
            }
            //int agentCount = Manager.GetAIWorld().m_AgentPrefabs.Keys.Count();
            //Manager.GetUIManager().ShowMessagePopup($"You current have {agentCount} agents", 3);
            //var TestSaveGlobals = new TestSaveGlobals();
            //var save = new SaveComponentBase();
            Manager.m_DevModeEnabled = true;
            
            //LoadSavePanelUi.SaveGameListUi.m_LoadSaveButton.gameObject.SetActive(false);

        }

        public EntityInfoPanel setEntityInfo(string title, string info)
        {
            AIEntity ai = null;
            foreach (AIEntity a in AIEntity.FindObjectsOfType(typeof(AIEntity)))
            {
                if (a.IsSelected() && !a.m_IsControllable)
                {
                    ai = a;
                }
                if (ai == null && a.IsAddedToWorld && !a.m_IsControllable)
                {
                    ai = a;
                }
            }
            ai.m_Selectable.SetSelected(false);
            ai.CurrentlySelected();
            ai.m_Selectable.SetSelected(true);

            EntityInfoPanel ui = (EntityInfoPanel)EntityInfoPanel.FindObjectOfType(typeof(EntityInfoPanel));
            ui.m_DetailText.Text = info; // + "(GUI m_DetailText)";
            ui.m_DetailText.m_Text.text = info; // + "(GUI m_Text)";
            ui.name = title; // + "(GUI name)";
            ui.m_SummaryText.text = title; // + "(GUI m_SummaryText)";
                                           //ui.m_BuyButton.enabled = true;
            if (!ui.IsVisible())
                ui.Show();

            return ui;
        }
        void DeleteAllSaves()
        {
            var user = SaveGame.CurrentUser;
            for (int i = 0; i < SaveGame.MaxSaveSlots; i++)
            {
                SaveGame.DeleteSave(i, user);
            }
        }

        public string GetName()
        {
            return "Ultimate Ironman Mod";
        }
    }
}
