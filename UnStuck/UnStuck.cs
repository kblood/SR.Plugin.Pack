using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Satellite Reign unstuck mod.
/// </summary>
public class Unstuck : ISrPlugin
{
    protected TimeManager.TimeScaler m_TacticalPauseTimeScaler;
    //private bool cheatMode = false;
    private int numberOfWeaponTypes = System.Enum.GetValues(typeof(WeaponType)).Length;
    private float timer;

    /// <summary>
    /// Plugin initialization 
    /// </summary>
    public void Initialize()
    {
        Debug.Log("Initializing Satellite Reign unstuck mod");
        m_TacticalPauseTimeScaler = TimeManager.AddTimeScaler();
        timer = Time.time;
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
        if(Manager.Get().IsLoading())
            timer = Time.time;

        if (Manager.Get().GameInProgress)
        {
            //Check if unstuck is ready
            if (timer < Time.time)
            {
                AIEntity spawnLocation = null;
                if (Input.GetKeyDown(KeyCode.F11) || Input.GetKeyDown(KeyCode.Minus))
                {
                    foreach (AgentAI a in AgentAI.GetAgents())
                    {
                        if (a.IsDowned || a.m_Dead)
                            a.RespawnAt(Manager.GetInputControl().GetClosestPos(a.transform.position), spawnLocation.transform.rotation);
                        //a.RespawnAt(spawnLocation.transform.position, spawnLocation.transform.rotation);
                        else
                            a.transform.position = Manager.GetInputControl().GetClosestPos(a.transform.position);
                        //a.Teleport(spawnLocation.transform);
                    }
                    timer = Time.time + 600f;
                    Manager.GetUIManager().ShowMessagePopup("All agents unstuck. Now the unstuck function will be disabled for 10 minutes", 3);
                    //setEntityInfo("Made all agents unstuck", "Now the unstuck function will be disabled for 15 minutes");
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
            else if (Input.GetKeyDown(KeyCode.Minus))
            {
                AIEntity spawnLocation = null;
                var selected = Manager.GetInputManager().GetNonPlayerSelectedEntities();
                selected.AddRange(Manager.GetInputManager().GetPlayerSelectedEntities());
                foreach (AIEntity ae in selected)//AIEntity.FindObjectsOfType(typeof(AIEntity)))
                {
                    if (ae.IsHuman() && ae.IsAddedToWorld && ae.IsSelected())
                    {
                        spawnLocation = ae;
                    }
                }
                if (spawnLocation != null)
                {
                    foreach (AgentAI a in AgentAI.GetAgents())
                    {
                        if (a.IsDowned || a.m_Dead)
                            a.RespawnAt(spawnLocation.transform.position, spawnLocation.transform.rotation);
                        else
                            a.transform.position = spawnLocation.transform.position;
                        //a.Teleport(spawnLocation.transform);
                    }
                    //timer = Time.time + 900f;
                    Manager.GetUIManager().ShowMessagePopup("All agents should now be unstuck.", 6);
                    //setEntityInfo("Made all agents unstuck", "Now the unstuck function will be disabled for 15 minutes");
                }
                else
                {
                    Manager.GetUIManager().ShowMessagePopup("No unstuck spot selected. You need to select a human as a location for your agents to get moved to, to be unstuck.", 9);
                    //setEntityInfo("No unstuck spot selected", "You need to select a human or a mech as a location for your agents to get moved to, to be unstuck.");
                }
            }

            if (Input.GetKeyDown(KeyCode.F8))
            {
                string info = "";
                foreach (AIEntity ae in Manager.GetInputControl().GetPlayerSelectedEntities())
                {
                    // Standard: Melee, SpeedAdjust, OnlyNearCover
                    ae.m_AIAbilities = (AIAbilities)197672;
                    // Scan, Melee. Shoot, LeaveCover, GetWeaponOut
                    //ae.m_AIAbilities = (AIAbilities)197645;
                    // Scan, Melee. Shoot, LeaveCover, GetWeaponOut, OnlyNearCover
                    //ae.m_AIAbilities = (AIAbilities)197709;

                    info += "\nAgent: " + ae.GetName();
                    info += "\nNow has AIAbilties: " + ae.m_AIAbilities.ToString();
                }
                setEntityInfo("Setting AI abilities", info);
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                string info = "";
                foreach (AIEntity ae in Manager.GetInputControl().GetPlayerSelectedEntities())
                {
                    //ae.m_AIAbilities = AIAbilities.LicensedToKill;
                    //ae.m_AIAbilities = AIAbilities.FindBetterCover;
                    //ae.m_AIAbilities = AIAbilities.Shoot;
                    //ae.m_AIAbilities = AIAbilities.OnlyNearCover;
                    //ae.m_AIAbilities = AIAbilities.LeaveCover;
                    //ae.m_AIAbilities = AIAbilities.GetWeaponOut;
                    //AIAbilities.HasSpeedAdjust;
                    //AIAbilities.CanMelee;
                    //AIAbilities.ScanEnemy;

                    // Standard: Melee, SpeedAdjust, OnlyNearCover
                    //ae.m_AIAbilities = (AIAbilities)197672;
                    // Scan, Melee. Shoot, LeaveCover, GetWeaponOut, SpeedAdjust
                    ae.m_AIAbilities = (AIAbilities)197645;
                    // Scan, Melee. Shoot, LeaveCover, GetWeaponOut, OnlyNearCover, SpeedAdjust
                    //ae.m_AIAbilities = (AIAbilities)197709;

                    info += "\nAgent: " + ae.GetName();
                    info += "\nNow has AIAbilties: " + ae.m_AIAbilities.ToString();
                }
                setEntityInfo("Setting AI abilities", info);
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                string info = "";
                foreach (AIEntity ae in Manager.GetInputControl().GetPlayerSelectedEntities())
                {
                    info += "\nAgent: " + ae.GetName();
                    info += "\n" + ae.m_AIAbilities.ToString();
                    if (ae.HasAIAbility(AIAbilities.IgnoreBeingShot))
                        info += "\nIgnores being shot";
                    else
                        info += "\nDoes not ignore being shot";
                    if (ae.HasAIAbility(AIAbilities.LicensedToKill))
                        info += "\nIs licensed to kill";
                    else
                        info += "\nNot licensed to kill";
                    if (ae.HasAIAbility(AIAbilities.GetWeaponOut))
                        info += "\nCan get weapon out";
                    else
                        info += "\nCannot get weapon out";
                    if (ae.HasAIAbility(AIAbilities.Shoot))
                        info += "\nCan shoot";
                    else
                        info += "\nCannot shoot";
                    if (ae.HasAIAbility(AIAbilities.CanMelee))
                        info += "\nCan melee";
                    else
                        info += "\nCannot melee";
                }
                setEntityInfo("Info about AI abilities", info);
            }
        }
    }

    public EntityInfoPanel setEntityInfo(string title, string info)
    {
        //UIEntityInterface uiei = ((UIEntityInterface)UIEntityInterface.FindObjectOfType(typeof(UIEntityInterface)));
        //uiei.m_selected = true;
        //uiei.m_AttachTracker = true;
        //uiei.m_ShowAgentUI = true;
        //uiei.m_SelectedText = info;
        //uiei.TargetVisible = true;
        //uiei.enabled = true;
        //uiei.SetSelected(true);

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
        //ai.SetSelected(true);
        ai.m_Selectable.SetSelected(true);

        EntityInfoPanel ui = (EntityInfoPanel)EntityInfoPanel.FindObjectOfType(typeof(EntityInfoPanel));
        //ui.SetAiEntity(ai);
        ui.m_DetailText.Text = info; // + "(GUI m_DetailText)";
        ui.m_DetailText.m_Text.text = info; // + "(GUI m_Text)";
        ui.name = title; // + "(GUI name)";
        ui.m_SummaryText.text = title; // + "(GUI m_SummaryText)";
        //ui.m_BuyButton.enabled = true;
        if (!ui.IsVisible())
            ui.Show();
        //AIEntityInfoUi aiui = (AIEntityInfoUi)AIEntityInfoUi.FindObjectOfType(typeof(AIEntityInfoUi));

        //ui.m_SummaryText.color = Color.red;

        //ai.m_Selectable.SetSelected(false);
        //ai.SetSelected(false);

        return ui;
    }

    public string GetName()
    {
        return "UnStuck";
    }
}