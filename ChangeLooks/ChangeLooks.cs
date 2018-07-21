using System;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Satellite Reign ChangeLook mod.
/// </summary>
public class ChangeLooks : ISrPlugin
{
    protected int[] teamSeeds = new int[4];
    protected int[] seedMem = new int[5];
    protected int[] nameIDs = new int[5];
    protected Clone[] cloneMem = new Clone[5];
    protected Clone[] clones = new Clone[5];
    

    /// <summary>
    /// Plugin initialization 
    /// </summary>
    public void Initialize()
    {
        Debug.Log("Initializing Satellite Reign Change Look mod");
    }

    public void Start()
    {
        teamSeeds = new int[4];
        seedMem = new int[5];
        cloneMem = new Clone[5];
        clones = new Clone[5];

        int n = 0;
        foreach (AgentAI a in AgentAI.GetAgents())
        {
            teamSeeds[n] = CloneManager.Get().GetCloneableData(a.CurrentCloneableId).m_RandomSeed;
            clones[n] = new Clone(a.CurrentCloneableId);
            a.AddAmmo(50);
            seedMem[n + 1] = CloneManager.Get().GetCloneableData(a.CurrentCloneableId).m_RandomSeed;
            cloneMem[n + 1] = new Clone(a.CurrentCloneableId);
            n++;
        }
    }

    /// <summary>
    /// Called once per frame.
    /// </summary>
    public void Update()
    {
        if (Manager.Get().GameInProgress)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                foreach (AgentAI a in AgentAI.GetAgents())
                {
                    if (a.IsSelected())
                    {
                        if (CloneManager.Get().GetCloneableData(a.CurrentCloneableId).Sex == WardrobeManager.Sex.Female)
                            CloneManager.Get().GetCloneableData(a.CurrentCloneableId).Sex = WardrobeManager.Sex.Male;
                        else
                            CloneManager.Get().GetCloneableData(a.CurrentCloneableId).Sex = WardrobeManager.Sex.Female;
                        //CloneManager.Get().GetCloneableData(a.CurrentCloneableId).m_RandomSeed -= 10;
                        RespawnAgent(a);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.End))
            {
                string info = "";
                int n = 0;
                foreach (AgentAI a in Manager.GetInputControl().GetSelectedAgents())
                {
                    //CloneManager.Get().GetCloneableData(a.CurrentCloneableId).m_RandomSeed = teamSeeds[n];
                    clones[n].ApplyClone(a.CurrentCloneableId);
                    RespawnAgent(a);

                    if (info != "")
                        info += "\n";
                    info += a.AgentClassName() + " got " + clones[n].GetInfo();
                    n++;
                }
                setEntityInfo("Agent Clone Data applied", info);
            }

            if (Input.GetKeyDown(KeyCode.Home))
            {
                string info = "";
                int n = 0;
                foreach (AgentAI a in AgentAI.GetAgents())
                {
                    //copyClone(CloneManager.Get().GetCloneableData(a.CurrentCloneableId), ref clones[n]);
                    teamSeeds[n] = CloneManager.Get().GetCloneableData(a.CurrentCloneableId).m_RandomSeed;
                    if (clones.Length != 5)
                        clones = new Clone[5];
                    clones[n] = new Clone(a.CurrentCloneableId);
                    if (info != "")
                        info += "\n";
                    info += a.AgentClassName() + " info: " + clones[n].GetInfo();
                    n++;
                }
                setEntityInfo("Agent Clone Data Stored", info);
            }

            if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Keypad1))
            {
                memSet(1);
            }
            else if ((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Keypad2))
            {
                memSet(2);
            }
            else if((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Keypad4))
            {
                memGet(1);
            }
            else if((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Keypad5))
            {
                memGet(2);
            }
            else if(Input.GetKeyDown(KeyCode.Keypad1))
            {
                memSet(1);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                memSet(2);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                memSet(3);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                memGet(1);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                memGet(2);
            }
            else if (Input.GetKeyDown(KeyCode.Keypad6))
            {
                memGet(3);
            }

            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                string info = "";
                foreach (AgentAI a in AgentAI.GetAgents())
                {
                    if (a.IsSelected())
                    {
                        int numberOfWardrobeTypes = System.Enum.GetValues(typeof(WardrobeManager.WardrobeType)).Length;
                        int typeNo = UnityEngine.Random.Range(0, numberOfWardrobeTypes);
                        WardrobeManager.WardrobeType wType = (WardrobeManager.WardrobeType)typeNo;
                        //a.name = "Caldor";
                        //a.InitRandomSeed();
                        //a.m_Identity.NameID = IdentityManager.GetRandomNameID(CloneManager.Get().GetCloneableData(a.CurrentCloneableId).Sex);
                        CloneManager.Get().GetCloneableData(a.CurrentCloneableId).m_RandomSeed = Wardrobe.GenerateRandomSeed();
                        a.m_Wardrobe.m_Sex = Wardrobe.GenerateRandomSex();
                        a.m_Wardrobe.m_WardrobeType = wType;
                        //a.m_Wardrobe.bod = Wardrobe.CreateRandomBodyData();
                        Wardrobe.BodyData body = a.m_Wardrobe.GetBodyData();
                        //body.m_HairColor1 = Color.yellow;
                        //body.m_HairColor2 = Color.green;
                        //body.m_LowerPrimaryColor = Color.red;
                        //body.m_LowerSecondaryColor = Color.red;
                        //body.m_SkinColor = Color.blue;
                        //a.m_Wardrobe.SetBodyData(body, a.m_Wardrobe.m_Sex, a.m_Wardrobe.RandomSeed, a.m_Wardrobe.m_WardrobeType);
                        RespawnAgent(a);

                        info += "Hair:" + body.m_HairColor1.ToString() + " & " + body.m_HairColor2.ToString() + ". skin: " + body.m_SkinColor.ToString();
                    }
                }
                setEntityInfo("Randomized", info);
            }

            if((Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)) && Input.GetKeyDown(KeyCode.Keypad9))
            {
                int numberOfWardrobeTypes = System.Enum.GetValues(typeof(WardrobeManager.WardrobeType)).Length;
                int typeNo = UnityEngine.Random.Range(0, numberOfWardrobeTypes);
                WardrobeManager.WardrobeType wType = (WardrobeManager.WardrobeType)typeNo;
                foreach (AgentAI a in Manager.GetInputControl().GetSelectedAgents())
                {
                    a.m_Wardrobe.m_WardrobeType = wType;
                    RespawnAgent(a);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                string info = "";
                string first; string last;
                foreach (AgentAI a in Manager.GetInputControl().GetSelectedAgents()) //AgentAI.GetAgents())
                {
                    int nameId = IdentityManager.GetRandomNameID(a.m_Wardrobe.m_Sex);
                    int currentNameId = Mathf.RoundToInt(a.CurrentCloneableId);
                    int cloneId = a.CurrentCloneableId;
                    CloneableData cData = CloneManager.Get().GetCloneableData(cloneId);
                    IdentityManager.Get().GetName(cData.IdentityId, out first, out last);
                    info += "Name based on identity Id: " + cData.IdentityId + " is " + first + " " + last + "\n";
                    //info += "Clonable name: " + a.GetCloneable().name + "\n";
                    info += currentNameId + ": " + first + " " + last;
                    IdentityManager.Get().GetName(cData.m_IdentityID, out first, out last);
                    //a.m_Identity.NameID = nameId;
                    //a.m_Wardrobe.m_WardrobeType = WardrobeManager.WardrobeType.AgentSupportBacker;
                    info += " changing name To " + nameId + ": " + first + " " + last + "\n";
                    Manager.GetUIManager().ShowMessagePopup(info, 6);
                    cData.IdentityId = nameId;
                    cData.m_IdentityID = nameId;
                    //cData.WardrobeType = WardrobeManager.WardrobeType.Prostitute2;

                    //a.name = "Caldor";
                    //a.InitRandomSeed();
                    //a.m_Identity.NameID = IdentityManager.GetRandomNameID(CloneManager.Get().GetCloneableData(a.CurrentCloneableId).Sex);
                    //CloneManager.Get().GetCloneableData(a.CurrentCloneableId).m_RandomSeed = Wardrobe.GenerateRandomSeed();
                    //a.m_Wardrobe.m_Sex = WardrobeManager.Sex.Male;
                    //a.m_Wardrobe.bod = Wardrobe.CreateRandomBodyData();
                    Wardrobe.BodyData body = a.m_Wardrobe.GetBodyData();
                    //body.m_HairColor1 = Color.black;
                    //body.m_HairColor2 = Color.black;
                    //body.m_LowerPrimaryColor = Color.black;
                    //body.m_LowerSecondaryColor = Color.black;
                    //body.m_SkinColor = Color.black;

                    //a.m_Wardrobe.SetBodyData(body, a.m_Wardrobe.m_Sex, a.m_Wardrobe.RandomSeed, a.m_Wardrobe.m_WardrobeType);
                    RespawnAgent(a);

                    info += a.AgentClassName() + " got Hair: " + body.m_HairColor1 + " & " + body.m_HairColor1 + ".\nskin: " + body.m_SkinColor;
                    info += "\nWardrope type: " + a.m_Wardrobe.m_WardrobeType + "\n";
                }
                setEntityInfo("Selected agent", info);
            }

            

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                string info = "";
                string first; string last;
                AgentAI a = AgentAI.FirstSelectedAgentAi();
                if (a != null)
                {
                    int cloneId = a.CurrentCloneableId;
                    CloneableData cData = CloneManager.Get().GetCloneableData(cloneId);
                    IdentityManager.Get().GetName(cData.IdentityId, out first, out last);

                    info += "Selected person is " + first + " " + last + ", gender " + cData.Sex;
                    info += ". Seed is:" + cData.RandomSeed + ", mSeed is:" + cData.m_RandomSeed + " and wardrobe type is " + cData.WardrobeType;
                    info += ". Palette: " + cData.WardrobeConfigurationData.m_DefaultColorPaletteName;
                }
                else
                {
                    foreach (AIEntity ae in AIEntity.FindObjectsOfType(typeof(AIEntity)))
                    {
                        if(ae.IsSelected())
                        {
                            ae.m_IsControllable = true;
                            IdentityManager.Get().GetName(ae.m_Identity.NameID, out first, out last);
                            info += "Selected person is " + first + " " + last + ", gender " + ae.m_Wardrobe.m_Sex;
                            info += ". Seed is:" + ae.m_Wardrobe.RandomSeed + " Wardrobe type is " + ae.m_Wardrobe.m_WardrobeType;
                            info += ". Palette: " + ae.m_Wardrobe.DefaultColorPaletteName;
                            break;
                        }
                    }
                }
                setEntityInfo("Selected AI info", info);
            }
        }
    }

    public void RespawnAgent(AgentAI a)
    {
        if (!a.IsInDanger && !a.m_Dead && !a.IsDowned)
        {
            AgentAI.AgentClass aClass = a.GetClass();
            a.RespawnAtCurrentLocation();
            a = AgentAI.GetAgent(aClass);
            a.SetSelected(true);
            Manager.GetInputManager().Select(a.m_UID.GetSUID(), true);
            //Manager.GetInputManager().sele
        }
    }

    public void memSet(int n)
    {
        int seed = 0;
        Clone c = null;

        foreach (AgentAI a in AgentAI.GetAgents())
        {
            if (a.IsSelected() && (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)))
            {
                nameIDs[n] = a.m_Identity.NameID;
            }
            else if (a.IsSelected())
            {
                c = new Clone(a.CurrentCloneableId);
                seed = c.seed;
            }
        }
        //CloneableData data = null;
        if (seed != 0)
            seedMem[n] = seed;
        else
            foreach (AIEntity ae in AIEntity.FindObjectsOfType(typeof(AIEntity)))
            {
                //if (a.IsSelected())
                //    seedMem[n] = a.GetComponent<CloneableData>().RandomSeed;
                //CloneManager.Get().GetCloneableData(a.CurrentCloneableId).m_RandomSeed;
                if (ae.IsSelected() && (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)))
                {
                    nameIDs[n] = ae.m_Identity.NameID;
                }
                else if (ae.IsSelected())
                {
                    c = new Clone(ae);
                    seed = ae.m_Wardrobe.RandomSeed;
                    //w = new Wardrobe();
                    //w.CopyFrom(a.m_Wardrobe);

                    //data = CloneManager.Get().NewCloneableFromPrefab(a);
                    //if (a.GetComponentInParent<CloneableData>() != null)
                    //    seed = a.GetCloneable().GetComponentInParent<CloneableData>().m_RandomSeed;
                    //if (a.GetComponent<CloneableData>() != null)
                    //    seed = a.GetComponent<CloneableData>().m_RandomSeed;
                    //if (a.GetCloneable().GetComponentInChildren<CloneableData>() != null)
                    //    seed = a.GetCloneable().GetComponentInChildren<CloneableData>().m_RandomSeed;
                    //if (seed == 0)
                    //    seed = data.m_RandomSeed;
                }
            }
        if (seed != 0)
            seedMem[n] = seed;
        string info = "Seed: " + seed;
        if (c != null)
        {
            cloneMem[n] = c;
            Manager.GetUIManager().ShowMessagePopup(info + " saved into " + n, 8);
            setEntityInfo("Seed saved into " + n, info);
        }
        else if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl) && nameIDs[n] != 0)
        {
            string first; string last;
            IdentityManager.Get().GetName(nameIDs[n], out first, out last);
            Manager.GetUIManager().ShowMessagePopup("NameId:" + nameIDs[n] + " put in nameId mem slot " + n + ", name " + first + " " + last, 8);
        }
        else
        {
            Manager.GetUIManager().ShowMessagePopup("Cannot store a clone in slot " + n + " if no clonable person is selected.", 8);
        }
    }

    public void memGet(int n)
    {
        bool done = false;
        string info = "";
        AgentAI ai = null;
        foreach (AgentAI a in AgentAI.GetAgents())
        {
            if(a.IsSelected() && (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl)))
            {
                a.m_Identity.NameID = nameIDs[n];
            }
            else if (a.IsSelected() && cloneMem[n] != null)
            {
                cloneMem[n].ApplyClone(a.CurrentCloneableId);
                //a.m_Wardrobe.CopyFrom();
                //a.RespawnAtCurrentLocation();
                RespawnAgent(a);
                info += "Clone seed: " + cloneMem[n].seed;
                done = true;
            }
            else if (a.IsSelected() && 2 < seedMem[n])
            {
                //applyClone(cloneMem[n], a.CurrentCloneableId);
                CloneManager.Get().GetCloneableData(a.CurrentCloneableId).m_RandomSeed = seedMem[n];

                //a.RespawnAtCurrentLocation();
                RespawnAgent(a);
                //break;
                info += "Seed: " + seedMem[n];
                done = true;
            }
        }

        if (cloneMem[1] != null)
            info += "Seed 1: " + cloneMem[1].seed;
        if (cloneMem[2] != null)
            info += "\nSeed 2: " + cloneMem[2].seed;
        if (cloneMem[3] != null)
            info += "\nSeed 3: " + cloneMem[3].seed;
        setEntityInfo("Seeds in memory: ", info);

        if (done)
            Manager.GetUIManager().ShowMessagePopup("Clone seed " + n + ": " + cloneMem[n].seed + " put into" + ai.AgentClassName(), 8);
        else
        {
            string first; string last;
            if (Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftControl) && nameIDs[n] != 0)
            {
                IdentityManager.Get().GetName(nameIDs[n], out first, out last);
                Manager.GetUIManager().ShowMessagePopup("NameId " + nameIDs[n] + " from nameId mem slot " + n + ", name " + first + " " + last + " applied.", 8);
            }
            else
            {
                IdentityManager.Get().GetName(cloneMem[n].nameId, out first, out last);
                Manager.GetUIManager().ShowMessagePopup("Clone seed " + n + ": " + cloneMem[n].seed + " name id: " + cloneMem[n].nameId + ", name " + first + " " + last, 8);
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
        return "ChangeLooks";
    }
}

public class Clone
{
    public WardrobeManager.Sex sex;
    public int seed;
    public int nameId;
    public WardrobeManager.WardrobeType wType;

    public Clone()
    {
        int numberOfWardrobeTypes = System.Enum.GetValues(typeof(WardrobeManager.WardrobeType)).Length;
        sex = Wardrobe.GenerateRandomSex();
        seed = Wardrobe.GenerateRandomSeed();
        nameId = IdentityManager.GetRandomNameID(sex);
        int typeNo = UnityEngine.Random.Range(0, numberOfWardrobeTypes);
        wType = (WardrobeManager.WardrobeType)typeNo;
    }

    public Clone(int id)
    {
        sex = CloneManager.Get().GetCloneableData(id).Sex;
        seed = CloneManager.Get().GetCloneableData(id).RandomSeed;
        nameId = CloneManager.Get().GetCloneableData(id).IdentityId;
        wType = CloneManager.Get().GetCloneableData(id).WardrobeType;
    }

    public Clone(AIEntity ae)
    {
        nameId = ae.m_Identity.GetNameID();
        sex = ae.m_Wardrobe.m_Sex;
        seed = ae.m_Wardrobe.RandomSeed;
        wType = ae.m_Wardrobe.m_WardrobeType;
    }

    public void ApplyClone(int id)
    {
        CloneManager.Get().GetCloneableData(id).Sex = sex;
        CloneManager.Get().GetCloneableData(id).m_Sex = sex;
        CloneManager.Get().GetCloneableData(id).RandomSeed = seed;
        CloneManager.Get().GetCloneableData(id).m_RandomSeed = seed;
        CloneManager.Get().GetCloneableData(id).IdentityId = nameId;
        CloneManager.Get().GetCloneableData(id).m_IdentityID = nameId;
        CloneManager.Get().GetCloneableData(id).WardrobeType = wType;
        CloneManager.Get().GetCloneableData(id).m_WardrobeType = wType;
        //CloneManager.Get().GetCloneableData(id).IdentityId = dataFrom.IdentityId;
    }

    public void ApplyClone(AIEntity ae)
    {
        ae.m_Wardrobe.m_Sex = sex;
        ae.m_Identity.NameID = nameId;
        ae.m_Wardrobe.RandomSeed = seed;
        ae.m_Wardrobe.m_WardrobeType = wType;
    }

    public string GetInfo()
    {
        string info = "";
        string first; string last;
        IdentityManager.Get().GetName(nameId, out first, out last);
        info += "Clone is a " + sex + " named " + nameId + ": " + first + " " + last + " with seed: " + seed;
        return info;
    }
}

