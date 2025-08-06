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
        private List<Translation> translations = new List<Translation>();
        private Quest activeQuest = null;
        private bool isEditing = false;
        private bool hasUnsavedChanges = false;
        private const string _questDataFileName = "questDefinitions.xml";
        private const string _translationDataFileName = "translations.xml";

        public QuestBrowser()
        {
            InitializeComponent();
            InitializeEditor();
            LoadAllData();
            UpdateQuestList();
            SetupFormTitle();
            
            // Subscribe to form closing event
            this.FormClosing += QuestBrowser_FormClosing;
            
            // Subscribe to text change events
            this.txtQuestTitle.TextChanged += txtQuestTitle_TextChanged;
            this.numLocationID.ValueChanged += numLocationID_ValueChanged;
        }

        private void InitializeEditor()
        {
            // Initialize dropdown controls with validation data
            PopulateDistrictDropdown();
            SetupEditorControls();
            EnableEditMode(false);
        }

        private void SetupFormTitle()
        {
            this.Text = "Quest Editor - Satellite Reign Modding Tools";
        }

        private void LoadAllData()
        {
            LoadQuestData();
            LoadTranslationData();
        }

        private void LoadQuestData()
        {
            try
            {
                string questFilePath = System.IO.Path.Combine(FileManager.ExecPath, _questDataFileName);
                quests = FileManager.LoadQuestDefinitionsXML(_questDataFileName, FileManager.ExecPath) ?? new List<Quest>();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading quest data: {ex.Message}\n\nMake sure you've exported quest data from the game using the LoadCustomData mod (press F4 or Delete).", 
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                quests = new List<Quest>();
            }
        }

        private void LoadTranslationData()
        {
            try
            {
                string translationFilePath = System.IO.Path.Combine(FileManager.ExecPath, _translationDataFileName);
                translations = FileManager.LoadTranslationDefinitionsXML(_translationDataFileName, FileManager.ExecPath) ?? new List<Translation>();
            }
            catch (Exception ex)
            {
                // Translations are optional - don't show error, just use empty list
                System.Diagnostics.Debug.WriteLine($"No translations file found: {ex.Message}");
                translations = new List<Translation>();
            }
        }

        private void PopulateDistrictDropdown()
        {
            cmbDistrict.Items.Clear();
            foreach (QuestDistrict district in Enum.GetValues(typeof(QuestDistrict)))
            {
                cmbDistrict.Items.Add(new { 
                    Text = district.ToString(), 
                    Value = district 
                });
            }
            cmbDistrict.DisplayMember = "Text";
            cmbDistrict.ValueMember = "Value";
        }

        private void SetupEditorControls()
        {
            // Populate translation key dropdown
            PopulateTitleKeyDropdown();
            
            // Set up numeric control for wake locations
            numWakeOnLocation.Minimum = -1;
            numWakeOnLocation.Maximum = 500;
            numWakeOnLocation.Value = -1;
        }

        private void PopulateTitleKeyDropdown()
        {
            cmbTitleKey.Items.Clear();
            
            // Add existing translation keys
            var translationKeys = GetTranslationKeys();
            foreach (string key in translationKeys)
            {
                cmbTitleKey.Items.Add(key);
            }
            
            // If no translations loaded, add some common prefixes for new keys
            if (translationKeys.Count == 0)
            {
                cmbTitleKey.Items.Add("CUSTOM_QUEST_TITLE_001");
                cmbTitleKey.Items.Add("CUSTOM_QUEST_TITLE_002"); 
                cmbTitleKey.Items.Add("Q_GEN_CUSTOM_TITLE_01");
            }
        }

        private void EnableEditMode(bool enabled)
        {
            isEditing = enabled;
            
            // Toggle visibility of edit controls vs read-only controls
            cmbDistrict.Visible = enabled;
            txtDistrict.Visible = !enabled;
            
            cmbTitleKey.Visible = enabled;
            txtTitleKey.Visible = !enabled;
            
            numWakeOnLocation.Visible = enabled;
            txtWakeOnLocation.Visible = !enabled;
            
            numLocationID.Visible = enabled;
            txtLocation.Visible = !enabled;
            
            // Toggle edit button visibility
            btnEditQuest.Visible = !enabled;
            btnSaveQuest.Visible = enabled;
            btnCancelEdit.Visible = enabled;
            
            // Enable/disable text fields
            txtQuestTitle.ReadOnly = !enabled;
            
            // Enable/disable checkboxes
            chkHidden.Enabled = enabled;
            chkShowDebrief.Enabled = enabled;
            
            // Update form title to indicate edit mode
            if (enabled)
            {
                this.Text = "Quest Editor - EDITING MODE - Satellite Reign Modding Tools";
            }
            else
            {
                this.Text = "Quest Editor - Satellite Reign Modding Tools";
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

                // Update dropdown controls if in edit mode
                if (isEditing)
                {
                    // Set district dropdown
                    for (int i = 0; i < cmbDistrict.Items.Count; i++)
                    {
                        var item = (dynamic)cmbDistrict.Items[i];
                        if (item.Text == activeQuest.District)
                        {
                            cmbDistrict.SelectedIndex = i;
                            break;
                        }
                    }
                    
                    // Set title key dropdown
                    if (!string.IsNullOrEmpty(activeQuest.TitleKey))
                    {
                        for (int i = 0; i < cmbTitleKey.Items.Count; i++)
                        {
                            if (cmbTitleKey.Items[i].ToString() == activeQuest.TitleKey)
                            {
                                cmbTitleKey.SelectedIndex = i;
                                break;
                            }
                        }
                    }
                    
                    // Set wake on location numeric control
                    numWakeOnLocation.Value = activeQuest.WakeOnLocation;
                    
                    // Set location ID numeric control
                    if (activeQuest.Location != null && activeQuest.Location.LocationID > 0)
                    {
                        numLocationID.Value = activeQuest.Location.LocationID;
                    }
                    else
                    {
                        numLocationID.Value = -1;
                    }
                }

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
            if (hasUnsavedChanges)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to discard them and reload?", 
                    "Unsaved Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return;
            }

            LoadAllData();
            UpdateQuestList();
            hasUnsavedChanges = false;
            EnableEditMode(false);
            
            string message = $"Reloaded {quests.Count} quests";
            if (translations.Count > 0)
                message += $" and {translations.Count} translations";
            
            MessageBox.Show(message, "Refresh Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        // New editing methods
        private void btnEditQuest_Click(object sender, EventArgs e)
        {
            if (activeQuest == null)
            {
                MessageBox.Show("Please select a quest to edit.", "No Quest Selected", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            EnableEditMode(true);
            MessageBox.Show("Quest editing enabled. You can now modify quest properties using the dropdown controls and text fields.", 
                "Edit Mode Enabled", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSaveQuest_Click(object sender, EventArgs e)
        {
            if (!isEditing || activeQuest == null)
                return;

            try
            {
                // Save changes to the active quest
                SaveCurrentQuestChanges();

                // Save the entire quest collection to file
                string questFilePath = System.IO.Path.Combine(FileManager.ExecPath, "questDefinitions_modified.xml");
                FileManager.SaveQuestDefinitionsXML(quests, "questDefinitions_modified.xml", FileManager.ExecPath);

                hasUnsavedChanges = false;
                EnableEditMode(false);
                UpdateQuestList();

                MessageBox.Show($"Quest changes saved to questDefinitions_modified.xml", "Save Complete", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving quest: {ex.Message}", "Save Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNewQuest_Click(object sender, EventArgs e)
        {
            try
            {
                // Create new quest with next available ID
                int newId = quests.Count > 0 ? quests.Max(q => q.ID) + 1 : 1;
                
                var newQuest = new Quest
                {
                    ID = newId,
                    Title = "New Quest",
                    TitleKey = $"CUSTOM_QUEST_TITLE_{newId:000}",
                    District = "NONE",
                    Hidden = false,
                    ShowDebrief = true,
                    SubQuests = new List<int>(),
                    Descriptions = new List<QuestDescription>()
                };

                quests.Add(newQuest);
                activeQuest = newQuest;
                hasUnsavedChanges = true;
                
                UpdateQuestList();
                
                // Select the new quest in the list
                for (int i = 0; i < QuestListBox.Items.Count; i++)
                {
                    var item = QuestListBox.Items[i];
                    if (item.GetType().GetProperty("Value")?.GetValue(item) as Quest == newQuest)
                    {
                        QuestListBox.SelectedIndex = i;
                        break;
                    }
                }

                EnableEditMode(true);
                MessageBox.Show($"New quest created with ID {newId}. You can now edit its properties.", 
                    "New Quest Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating new quest: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveCurrentQuestChanges()
        {
            if (activeQuest == null)
                return;

            // This method will be expanded when we add the proper editing controls
            // For now, we'll prepare the framework
            hasUnsavedChanges = true;
        }

        private List<string> GetTranslationKeys()
        {
            return translations.Select(t => t.Key).OrderBy(k => k).ToList();
        }

        private string GetTranslationText(string key)
        {
            var translation = translations.FirstOrDefault(t => t.Key == key);
            return translation?.Element?.PrimaryTranslation ?? "";
        }

        private void OnQuestPropertyChanged()
        {
            hasUnsavedChanges = true;
        }

        private void QuestBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hasUnsavedChanges)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to save before closing?", 
                    "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    btnSaveQuest_Click(this, EventArgs.Empty);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        // New event handlers for editing controls
        private void cmbDistrict_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isEditing && activeQuest != null && cmbDistrict.SelectedItem != null)
            {
                var selectedDistrict = ((dynamic)cmbDistrict.SelectedItem).Value;
                activeQuest.District = selectedDistrict.ToString();
                OnQuestPropertyChanged();
            }
        }

        private void cmbTitleKey_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isEditing && activeQuest != null && cmbTitleKey.SelectedItem != null)
            {
                string selectedKey = cmbTitleKey.SelectedItem.ToString();
                activeQuest.TitleKey = selectedKey;
                
                // Auto-populate title from translation if available
                string translationText = GetTranslationText(selectedKey);
                if (!string.IsNullOrEmpty(translationText))
                {
                    activeQuest.Title = translationText;
                    txtQuestTitle.Text = translationText;
                }
                
                OnQuestPropertyChanged();
            }
        }

        private void numWakeOnLocation_ValueChanged(object sender, EventArgs e)
        {
            if (isEditing && activeQuest != null)
            {
                activeQuest.WakeOnLocation = (int)numWakeOnLocation.Value;
                OnQuestPropertyChanged();
                
                // Update display text
                if (activeQuest.WakeOnLocation == -1)
                {
                    txtWakeOnLocation.Text = "No wake location";
                }
                else
                {
                    string locationName = QuestValidation.GetLocationName(activeQuest.WakeOnLocation);
                    txtWakeOnLocation.Text = $"Location ID: {activeQuest.WakeOnLocation} ({locationName})";
                }
            }
        }

        private void btnCancelEdit_Click(object sender, EventArgs e)
        {
            if (hasUnsavedChanges)
            {
                var result = MessageBox.Show("Are you sure you want to cancel editing? All changes will be lost.", 
                    "Cancel Editing", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                    return;
            }

            // Reload the quest data to revert changes
            if (activeQuest != null)
            {
                var originalQuest = quests.FirstOrDefault(q => q.ID == activeQuest.ID);
                if (originalQuest != null)
                {
                    activeQuest = originalQuest;
                    UpdateQuestDetails();
                }
            }

            hasUnsavedChanges = false;
            EnableEditMode(false);
        }

        private void txtQuestTitle_TextChanged(object sender, EventArgs e)
        {
            if (isEditing && activeQuest != null)
            {
                activeQuest.Title = txtQuestTitle.Text;
                OnQuestPropertyChanged();
                
                // Update quest list display since DisplayName includes the title
                UpdateQuestList();
            }
        }

        private void numLocationID_ValueChanged(object sender, EventArgs e)
        {
            if (isEditing && activeQuest != null)
            {
                if (activeQuest.Location == null)
                    activeQuest.Location = new QuestLocation();
                
                activeQuest.Location.LocationID = (int)numLocationID.Value;
                OnQuestPropertyChanged();
                
                // Update display text
                if (activeQuest.Location.LocationID == -1)
                {
                    txtLocation.Text = "No Location";
                }
                else
                {
                    string locationName = QuestValidation.GetLocationName(activeQuest.Location.LocationID);
                    txtLocation.Text = $"Location ID: {activeQuest.Location.LocationID} ({locationName})";
                }
            }
        }
    }
}