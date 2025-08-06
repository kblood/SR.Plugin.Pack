using SRMod.Services;
using SatelliteReignModdingTools.Models;
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
    public partial class QuestBrowser : Form
    {
        private List<Quest> quests = new List<Quest>();
        private Quest activeQuest = null;
        private const string _questDataFileName = "questDefinitions.xml";

        public QuestBrowser()
        {
            InitializeComponent();
            LoadQuestData();
            UpdateQuestList();
        }

        private void LoadQuestData()
        {
            try
            {
                string questFilePath = System.IO.Path.Combine(FileManager.ExecPath, _questDataFileName);
                
                // Debug information
                MessageBox.Show($"Looking for quest file at: {questFilePath}\nFile exists: {System.IO.File.Exists(questFilePath)}", 
                    "Debug Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                quests = FileManager.LoadQuestDefinitionsXML(_questDataFileName, FileManager.ExecPath) ?? new List<Quest>();
                
                MessageBox.Show($"Successfully loaded {quests.Count} quests from XML file", 
                    "Load Result", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading quest data: {ex.Message}\n\nStack trace: {ex.StackTrace}\n\nMake sure you've exported quest data from the game using the LoadCustomData mod (press F4 or Delete).", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateQuestList()
        {
            try
            {
                // Create data source for quest list
                var questItems = quests.Select(quest => new
                {
                    Key = quest.DisplayName,
                    Value = quest
                }).ToList();

                QuestListBox.DataSource = questItems;
                QuestListBox.DisplayMember = "Key";
                QuestListBox.ValueMember = "Value";

                // Update status label
                lblQuestCount.Text = $"Loaded {quests.Count} quests";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating quest list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateQuestDetails()
        {
            if (activeQuest == null)
            {
                ClearQuestDetails();
                return;
            }

            try
            {
                // Basic quest info
                txtQuestID.Text = activeQuest.ID.ToString();
                txtQuestTitle.Text = activeQuest.Title ?? "";
                txtTitleKey.Text = activeQuest.TitleKey ?? "";
                txtDistrict.Text = activeQuest.DistrictText;
                txtStatus.Text = activeQuest.StatusText;
                txtLocation.Text = activeQuest.LocationText;
                
                chkHidden.Checked = activeQuest.Hidden;
                chkShowDebrief.Checked = activeQuest.ShowDebrief;

                // Quest description
                txtDescription.Text = activeQuest.DescriptionText;

                // Sub-quests
                txtSubQuests.Text = activeQuest.SubQuestText;
                if (activeQuest.SubQuests?.Count > 0)
                {
                    var subQuestDetails = string.Join(", ", activeQuest.SubQuests.Select(id => 
                    {
                        var subQuest = quests.FirstOrDefault(q => q.ID == id);
                        return subQuest != null ? $"{id}: {subQuest.Title}" : id.ToString();
                    }));
                    txtSubQuests.Text += $"\nDetails: {subQuestDetails}";
                }

                // Wake on location info
                if (activeQuest.WakeOnLocation > 0)
                {
                    txtWakeOnLocation.Text = $"Location ID: {activeQuest.WakeOnLocation}";
                }
                else
                {
                    txtWakeOnLocation.Text = "No wake location";
                }

                // Update descriptions list
                lstDescriptions.Items.Clear();
                if (activeQuest.Descriptions?.Count > 0)
                {
                    foreach (var desc in activeQuest.Descriptions)
                    {
                        lstDescriptions.Items.Add($"{desc.LocTitle}: {desc.Translation.Substring(0, Math.Min(100, desc.Translation.Length))}...");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating quest details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearQuestDetails()
        {
            txtQuestID.Text = "";
            txtQuestTitle.Text = "";
            txtTitleKey.Text = "";
            txtDistrict.Text = "";
            txtStatus.Text = "";
            txtLocation.Text = "";
            txtDescription.Text = "";
            txtSubQuests.Text = "";
            txtWakeOnLocation.Text = "";
            chkHidden.Checked = false;
            chkShowDebrief.Checked = false;
            lstDescriptions.Items.Clear();
        }

        private void QuestListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (QuestListBox.SelectedValue is Quest selectedQuest)
                {
                    activeQuest = selectedQuest;
                    UpdateQuestDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting quest: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadQuestData();
            UpdateQuestList();
            MessageBox.Show($"Reloaded {quests.Count} quests from questDefinitions.xml", "Refresh Complete", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            try
            {
                string filterText = txtFilter.Text.ToLower();
                if (string.IsNullOrWhiteSpace(filterText))
                {
                    UpdateQuestList();
                    return;
                }

                var filteredQuests = quests.Where(q => 
                    q.Title.ToLower().Contains(filterText) ||
                    q.ID.ToString().Contains(filterText) ||
                    q.District.ToLower().Contains(filterText) ||
                    (q.DescriptionText?.ToLower().Contains(filterText) ?? false)
                ).ToList();

                var questItems = filteredQuests.Select(quest => new
                {
                    Key = quest.DisplayName,
                    Value = quest
                }).ToList();

                QuestListBox.DataSource = questItems;
                QuestListBox.DisplayMember = "Key";
                QuestListBox.ValueMember = "Value";

                lblQuestCount.Text = $"Showing {filteredQuests.Count} of {quests.Count} quests";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering quests: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnFilter_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}