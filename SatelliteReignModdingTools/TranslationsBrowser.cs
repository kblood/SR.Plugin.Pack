using SRMod.Services;
using SatelliteReignModdingTools.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SatelliteReignModdingTools
{
    public partial class TranslationsBrowser : Form
    {
        private List<Translation> translations = new List<Translation>();
        private List<Translation> filteredTranslations = new List<Translation>();
        private Translation activeTranslation = null;
        private bool isEditing = false;
        private bool hasUnsavedChanges = false;
        private const string _translationDataFileName = "translations.xml";

        public TranslationsBrowser()
        {
            InitializeComponent();
            InitializeEditor();
            LoadAllData();
            UpdateTranslationList();
            SetupFormTitle();
            
            this.FormClosing += TranslationsBrowser_FormClosing;
        }

        private void InitializeEditor()
        {
            PopulateCategoryDropdown();
            EnableEditMode(false);
        }

        private void SetupFormTitle()
        {
            this.Text = "Translations Editor - Satellite Reign Modding Tools";
        }

        private void PopulateCategoryDropdown()
        {
            cmbCategory.Items.Clear();
            cmbCategory.Items.Add("All Categories");
            
            foreach (var category in TranslationCategories.Categories.Values.Distinct().OrderBy(c => c))
            {
                cmbCategory.Items.Add(category);
            }
            
            cmbCategory.SelectedIndex = 0;
        }

        private void LoadAllData()
        {
            LoadTranslationData();
        }

        private void LoadTranslationData()
        {
            try
            {
                string translationFilePath = System.IO.Path.Combine(FileManager.ExecPath, _translationDataFileName);
                translations = FileManager.LoadTranslationDefinitionsXML(_translationDataFileName, FileManager.ExecPath) ?? new List<Translation>();
                filteredTranslations = new List<Translation>(translations);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading translation data: {ex.Message}\\n\\nTranslations file may not exist yet. You can create new translations using this editor.", 
                    "Translation Loading", MessageBoxButtons.OK, MessageBoxIcon.Information);
                translations = new List<Translation>();
                filteredTranslations = new List<Translation>();
            }
        }

        private void EnableEditMode(bool enabled)
        {
            isEditing = enabled;
            
            // Toggle readonly state of text controls
            txtTranslationKey.ReadOnly = !enabled;
            txtToken.ReadOnly = !enabled;
            txtPrimaryTranslation.ReadOnly = !enabled;
            txtSecondaryTranslation.ReadOnly = !enabled;
            
            // Toggle button visibility
            btnEditTranslation.Visible = !enabled;
            btnSaveTranslation.Visible = enabled;
            btnCancelEdit.Visible = enabled;
            
            // Update form title to indicate edit mode
            if (enabled)
            {
                this.Text = "Translations Editor - EDITING MODE - Satellite Reign Modding Tools";
            }
            else
            {
                this.Text = "Translations Editor - Satellite Reign Modding Tools";
            }
        }

        private void UpdateTranslationList()
        {
            try
            {
                var translationItems = filteredTranslations.Select(translation => new
                {
                    Key = $"{translation.Key} - {TranslationCategories.GetCategory(translation.Key)}",
                    Value = translation
                }).ToList();

                TranslationListBox.DataSource = translationItems;
                TranslationListBox.DisplayMember = "Key";
                TranslationListBox.ValueMember = "Value";

                lblTranslationCount.Text = $"Showing {filteredTranslations.Count} of {translations.Count} translations";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating translation list: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateTranslationDetails()
        {
            if (activeTranslation == null)
            {
                ClearTranslationDetails();
                return;
            }

            try
            {
                txtTranslationKey.Text = activeTranslation.Key ?? "";
                txtToken.Text = activeTranslation.Element?.Token ?? "";
                txtPrimaryTranslation.Text = activeTranslation.Element?.PrimaryTranslation ?? "";
                
                // Show secondary translation if it exists
                if (activeTranslation.Element?.Translations?.Count > 1)
                {
                    txtSecondaryTranslation.Text = activeTranslation.Element.Translations[1];
                }
                else
                {
                    txtSecondaryTranslation.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating translation details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClearTranslationDetails()
        {
            txtTranslationKey.Text = "";
            txtToken.Text = "";
            txtPrimaryTranslation.Text = "";
            txtSecondaryTranslation.Text = "";
        }

        private void FilterTranslations()
        {
            try
            {
                var selectedCategory = cmbCategory.SelectedItem?.ToString();
                var filterText = txtFilter.Text.ToLower();

                filteredTranslations = translations.Where(t => 
                {
                    // Category filter
                    bool categoryMatch = selectedCategory == "All Categories" || 
                                       TranslationCategories.GetCategory(t.Key) == selectedCategory;

                    // Text filter
                    bool textMatch = string.IsNullOrWhiteSpace(filterText) ||
                                   t.Key.ToLower().Contains(filterText) ||
                                   (t.Element?.Token?.ToLower().Contains(filterText) ?? false) ||
                                   (t.Element?.PrimaryTranslation?.ToLower().Contains(filterText) ?? false);

                    return categoryMatch && textMatch;
                }).ToList();

                UpdateTranslationList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error filtering translations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveCurrentTranslationChanges()
        {
            if (activeTranslation == null)
                return;

            try
            {
                // Update the active translation with current form values
                activeTranslation.Key = txtTranslationKey.Text;
                
                if (activeTranslation.Element == null)
                    activeTranslation.Element = new TranslationElement();
                
                activeTranslation.Element.Token = txtToken.Text;
                activeTranslation.Element.PrimaryTranslation = txtPrimaryTranslation.Text;
                
                // Handle secondary translation
                if (!string.IsNullOrEmpty(txtSecondaryTranslation.Text))
                {
                    if (activeTranslation.Element.Translations.Count < 2)
                    {
                        activeTranslation.Element.Translations.Add(txtSecondaryTranslation.Text);
                    }
                    else
                    {
                        activeTranslation.Element.Translations[1] = txtSecondaryTranslation.Text;
                    }
                }

                hasUnsavedChanges = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving translation changes: {ex.Message}", "Save Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnTranslationPropertyChanged()
        {
            hasUnsavedChanges = true;
        }

        // Event Handlers
        private void TranslationListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (TranslationListBox.SelectedValue is Translation selectedTranslation)
                {
                    activeTranslation = selectedTranslation;
                    UpdateTranslationDetails();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting translation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            FilterTranslations();
            hasUnsavedChanges = false;
            EnableEditMode(false);
            
            MessageBox.Show($"Reloaded {translations.Count} translations", "Refresh Complete", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterTranslations();
        }

        private void btnFilter_Click(object sender, EventArgs e)
        {
            FilterTranslations();
        }

        private void txtFilter_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                btnFilter_Click(sender, e);
                e.Handled = true;
            }
        }

        private void btnNewTranslation_Click(object sender, EventArgs e)
        {
            try
            {
                // Get next available ID for key generation
                int nextId = 1;
                string baseKey = "CUSTOM_TRANSLATION_";
                
                while (translations.Any(t => t.Key == $"{baseKey}{nextId:000}"))
                {
                    nextId++;
                }

                var newTranslation = new Translation
                {
                    Key = $"{baseKey}{nextId:000}",
                    Element = new TranslationElement
                    {
                        Token = $"{baseKey}{nextId:000}",
                        Translations = new List<string> { "New Translation Text" }
                    }
                };

                translations.Add(newTranslation);
                filteredTranslations.Add(newTranslation);
                activeTranslation = newTranslation;
                hasUnsavedChanges = true;
                
                UpdateTranslationList();
                
                // Select the new translation in the list
                for (int i = 0; i < TranslationListBox.Items.Count; i++)
                {
                    var item = TranslationListBox.Items[i];
                    if (item.GetType().GetProperty("Value")?.GetValue(item) as Translation == newTranslation)
                    {
                        TranslationListBox.SelectedIndex = i;
                        break;
                    }
                }

                EnableEditMode(true);
                MessageBox.Show($"New translation created with key {newTranslation.Key}. You can now edit its properties.", 
                    "New Translation Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating new translation: {ex.Message}", "Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnEditTranslation_Click(object sender, EventArgs e)
        {
            if (activeTranslation == null)
            {
                MessageBox.Show("Please select a translation to edit.", "No Translation Selected", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            EnableEditMode(true);
            MessageBox.Show("Translation editing enabled. You can now modify the translation key, token, and text values.", 
                "Edit Mode Enabled", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnSaveTranslation_Click(object sender, EventArgs e)
        {
            if (!isEditing || activeTranslation == null)
                return;

            try
            {
                // Save changes to the active translation
                SaveCurrentTranslationChanges();

                // Save the entire translation collection to file
                string translationFilePath = System.IO.Path.Combine(FileManager.ExecPath, "translations_modified.xml");
                FileManager.SaveTranslationDefinitionsXML(translations, "translations_modified.xml", FileManager.ExecPath);

                hasUnsavedChanges = false;
                EnableEditMode(false);
                FilterTranslations(); // Refresh the list to show any category changes
                UpdateTranslationDetails(); // Refresh the details view

                MessageBox.Show($"Translation changes saved to translations_modified.xml", "Save Complete", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving translation: {ex.Message}", "Save Error", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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

            // Reload the translation data to revert changes
            if (activeTranslation != null)
            {
                var originalTranslation = translations.FirstOrDefault(t => t.Key == activeTranslation.Key);
                if (originalTranslation != null)
                {
                    activeTranslation = originalTranslation;
                    UpdateTranslationDetails();
                }
            }

            hasUnsavedChanges = false;
            EnableEditMode(false);
        }

        private void TranslationsBrowser_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (hasUnsavedChanges)
            {
                var result = MessageBox.Show("You have unsaved changes. Do you want to save before closing?", 
                    "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    btnSaveTranslation_Click(this, EventArgs.Empty);
                }
                else if (result == DialogResult.Cancel)
                {
                    e.Cancel = true;
                }
            }
        }

        // Text change event handlers to track changes
        private void txtTranslationKey_TextChanged(object sender, EventArgs e)
        {
            if (isEditing)
                OnTranslationPropertyChanged();
        }

        private void txtToken_TextChanged(object sender, EventArgs e)
        {
            if (isEditing)
                OnTranslationPropertyChanged();
        }

        private void txtPrimaryTranslation_TextChanged(object sender, EventArgs e)
        {
            if (isEditing)
                OnTranslationPropertyChanged();
        }

        private void txtSecondaryTranslation_TextChanged(object sender, EventArgs e)
        {
            if (isEditing)
                OnTranslationPropertyChanged();
        }
    }
}