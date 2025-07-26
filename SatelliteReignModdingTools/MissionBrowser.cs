using dto = SRMod.DTOs;
using SRMod.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SatelliteReignModdingTools
{
    public partial class MissionBrowser : Form
    {
        public static List<dto.TranslationElementDTO> _translations = new List<dto.TranslationElementDTO>();
        public static dto.SerializableQuestManager questManager = new dto.SerializableQuestManager();
        static dto.SerializableQuestElement activeQuestElement = new dto.SerializableQuestElement();
        static dto.SerializableQuestAction activeQuestAction = null;
        static dto.SerializableQuestReaction activeQuestReaction = null;
        private const string _questDataFileName = "questDefinitions.xml";

        public static int activeLanguage = 2;

        public MissionBrowser()
        {
            InitializeComponent();
            LoadMissionData();
            UpdateMissionInfo();
            SetupDropdowns();
        }

        private void LoadMissionData()
        {
            try
            {
                _translations = FileManager.LoadTranslationsXML("Translations.xml", FileManager.ExecPath)?.ToList() ?? new List<dto.TranslationElementDTO>();
                questManager = FileManager.LoadQuestDataXML(_questDataFileName, FileManager.ExecPath) ?? new dto.SerializableQuestManager();
                
                if (questManager.m_QuestElements == null)
                    questManager.m_QuestElements = new List<dto.SerializableQuestElement>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading mission data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupDropdowns()
        {
            try
            {
                // Quest state dropdown
                QuestStateDropDown.DataSource = Enum.GetValues(typeof(QuestState)).Cast<QuestState>().ToList();
                
                // Action type dropdown
                ActionTypeDropDown.Items.Clear();
                ActionTypeDropDown.Items.AddRange(new string[]
                {
                    "QAGiveItem",
                    "QAGiveCash", 
                    "QASelectLocation",
                    "QASpawnVIP",
                    "QABroadcastMessage",
                    "QAActivateQuest"
                });
                
                // Reaction type dropdown
                ReactionTypeDropDown.Items.Clear();
                ReactionTypeDropDown.Items.AddRange(new string[]
                {
                    "QRWait",
                    "QRDataTerminalAccessed",
                    "QRFacilityBreached",
                    "QRProgressionData"
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error setting up dropdowns: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateMissionInfo()
        {
            try
            {
                // Update quest list
                QuestListBox.Items.Clear();
                foreach (var quest in questManager.m_QuestElements.OrderBy(q => q.m_ID))
                {
                    string displayName = $"[{quest.m_ID}] {quest.m_Title}";
                    QuestListBox.Items.Add(displayName);
                }

                // Update quest details if one is selected
                if (activeQuestElement != null && activeQuestElement.m_ID > 0)
                {
                    UpdateQuestDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating mission info: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateQuestDetails()
        {
            try
            {
                // Basic quest info
                QuestIDTextBox.Text = activeQuestElement.m_ID.ToString();
                QuestTitleTextBox.Text = activeQuestElement.m_Title ?? "";
                QuestStateDropDown.SelectedItem = activeQuestElement.m_State;
                HiddenCheckBox.Checked = activeQuestElement.m_Hidden;
                ShowDebriefCheckBox.Checked = activeQuestElement.m_ShowDebrief;
                
                // Update actions list
                ActionsListBox.Items.Clear();
                foreach (var action in activeQuestElement.m_Actions)
                {
                    ActionsListBox.Items.Add($"{action.m_ActionType} [{action.m_ID}]");
                }
                
                // Update reactions list
                ReactionsListBox.Items.Clear();
                foreach (var reaction in activeQuestElement.m_Reactions)
                {
                    ReactionsListBox.Items.Add($"{reaction.m_ReactionType} [{reaction.m_ID}]");
                }
                
                // Update descriptions list
                DescriptionsListBox.Items.Clear();
                foreach (var desc in activeQuestElement.m_Descriptions)
                {
                    DescriptionsListBox.Items.Add($"{desc.m_LocTitle}: {desc.m_Token}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating quest details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void QuestListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (QuestListBox.SelectedIndex >= 0 && QuestListBox.SelectedIndex < questManager.m_QuestElements.Count)
                {
                    activeQuestElement = questManager.m_QuestElements[QuestListBox.SelectedIndex];
                    UpdateQuestDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting quest: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNewQuest_Click(object sender, EventArgs e)
        {
            try
            {
                var newQuest = new dto.SerializableQuestElement
                {
                    m_ID = GetNextQuestID(),
                    m_Title = "NEW_QUEST_TITLE",
                    m_Hidden = false,
                    m_ShowDebrief = true,
                    m_State = dto.QuestState.NotStarted,
                    m_IsNew = true
                };
                
                questManager.m_QuestElements.Add(newQuest);
                UpdateMissionInfo();
                
                // Select the new quest
                QuestListBox.SelectedIndex = questManager.m_QuestElements.Count - 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating new quest: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDeleteQuest_Click(object sender, EventArgs e)
        {
            try
            {
                if (activeQuestElement != null && activeQuestElement.m_ID > 0)
                {
                    var result = MessageBox.Show($"Are you sure you want to delete quest '{activeQuestElement.m_Title}'?",
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    
                    if (result == DialogResult.Yes)
                    {
                        questManager.m_QuestElements.Remove(activeQuestElement);
                        activeQuestElement = new dto.SerializableQuestElement();
                        UpdateMissionInfo();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting quest: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddAction_Click(object sender, EventArgs e)
        {
            try
            {
                if (activeQuestElement == null || string.IsNullOrEmpty(ActionTypeDropDown.Text))
                    return;

                dto.SerializableQuestAction newAction = null;
                
                switch (ActionTypeDropDown.Text)
                {
                    case "QAGiveItem":
                        newAction = new dto.SerializableQAGiveItem();
                        break;
                    case "QAGiveCash":
                        newAction = new dto.SerializableQAGiveCash { m_CashAmount = 1000 };
                        break;
                    case "QABroadcastMessage":
                        newAction = new dto.SerializableQABroadcastMessage 
                        { 
                            m_MessageTitle = "Mission Update", 
                            m_MessageText = "Objective completed" 
                        };
                        break;
                    default:
                        newAction = new dto.SerializableQuestAction { m_ActionType = ActionTypeDropDown.Text };
                        break;
                }
                
                if (newAction != null)
                {
                    newAction.m_ID = GetNextActionID();
                    activeQuestElement.m_Actions.Add(newAction);
                    UpdateQuestDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding action: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAddReaction_Click(object sender, EventArgs e)
        {
            try
            {
                if (activeQuestElement == null || string.IsNullOrEmpty(ReactionTypeDropDown.Text))
                    return;

                dto.SerializableQuestReaction newReaction = null;
                
                switch (ReactionTypeDropDown.Text)
                {
                    case "QRWait":
                        newReaction = new dto.SerializableQRWait { m_WaitTime = 5.0f };
                        break;
                    case "QRDataTerminalAccessed":
                        newReaction = new dto.SerializableQRDataTerminalAccessed { m_AnyDataTerminal = true };
                        break;
                    case "QRFacilityBreached":
                        newReaction = new dto.SerializableQRFacilityBreached { m_AnyFacility = true };
                        break;
                    default:
                        newReaction = new dto.SerializableQuestReaction { m_ReactionType = ReactionTypeDropDown.Text };
                        break;
                }
                
                if (newReaction != null)
                {
                    newReaction.m_ID = GetNextReactionID();
                    activeQuestElement.m_Reactions.Add(newReaction);
                    UpdateQuestDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding reaction: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSaveQuest_Click(object sender, EventArgs e)
        {
            try
            {
                if (activeQuestElement != null && activeQuestElement.m_ID > 0)
                {
                    // Update quest from UI
                    activeQuestElement.m_Title = QuestTitleTextBox.Text;
                    activeQuestElement.m_State = (dto.QuestState)QuestStateDropDown.SelectedItem;
                    activeQuestElement.m_Hidden = HiddenCheckBox.Checked;
                    activeQuestElement.m_ShowDebrief = ShowDebriefCheckBox.Checked;
                    
                    MessageBox.Show("Quest updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    UpdateMissionInfo();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving quest: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSaveAll_Click(object sender, EventArgs e)
        {
            try
            {
                FileManager.SaveAsXML(questManager, _questDataFileName, FileManager.ExecPath);
                MessageBox.Show("All mission data saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving mission data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnLoadData_Click(object sender, EventArgs e)
        {
            try
            {
                LoadMissionData();
                UpdateMissionInfo();
                MessageBox.Show("Mission data reloaded successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading mission data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int GetNextQuestID()
        {
            if (questManager.m_QuestElements.Count == 0)
                return 1000;
            
            return questManager.m_QuestElements.Max(q => q.m_ID) + 1;
        }

        private int GetNextActionID()
        {
            var random = new Random();
            return random.Next(10000, 99999);
        }

        private int GetNextReactionID()
        {
            var random = new Random();
            return random.Next(10000, 99999);
        }
    }
}