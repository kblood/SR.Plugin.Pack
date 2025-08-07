using SatelliteReignModdingTools.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SatelliteReignModdingTools
{
    public partial class TranslationEditDialog : Form
    {
        private List<Translation> translations;
        private bool isNewTranslation = false;
        
        public string TranslationKey { get; private set; }
        public string TranslationText { get; private set; }
        public bool IsNewTranslation => isNewTranslation;
        public bool IsNew { get; set; } = true;
        public bool HasBeenSeen { get; set; } = false;
        
        public TranslationEditDialog(string existingKey, List<Translation> availableTranslations)
        {
            InitializeComponent();
            translations = availableTranslations ?? new List<Translation>();
            
            if (!string.IsNullOrEmpty(existingKey))
            {
                TranslationKey = existingKey;
                LoadExistingTranslation();
            }
            else
            {
                isNewTranslation = true;
                GenerateNewTranslationKey();
            }
            
            PopulateExistingKeys();
        }
        
        private void LoadExistingTranslation()
        {
            var existing = translations.FirstOrDefault(t => t.Key == TranslationKey);
            if (existing != null)
            {
                txtTranslationKey.Text = existing.Key;
                txtTranslationText.Text = existing.Element?.PrimaryTranslation ?? "";
                isNewTranslation = false;
            }
        }
        
        private void GenerateNewTranslationKey()
        {
            // Generate a new unique translation key
            int counter = 1;
            string baseKey = "CUSTOM_QUEST_DESC";
            string newKey;
            
            do
            {
                newKey = $"{baseKey}_{counter:000}";
                counter++;
            } while (translations.Any(t => t.Key == newKey));
            
            txtTranslationKey.Text = newKey;
            TranslationKey = newKey;
        }
        
        private void PopulateExistingKeys()
        {
            cmbExistingKeys.Items.Clear();
            foreach (var translation in translations.OrderBy(t => t.Key))
            {
                cmbExistingKeys.Items.Add(translation.Key);
            }
        }
        
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (ValidateInput())
            {
                TranslationKey = txtTranslationKey.Text.Trim();
                TranslationText = txtTranslationText.Text.Trim();
                
                // Check if this is a new key
                isNewTranslation = !translations.Any(t => t.Key == TranslationKey);
                
                DialogResult = DialogResult.OK;
                Close();
            }
        }
        
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        
        private void btnOpenTranslationsEditor_Click(object sender, EventArgs e)
        {
            try
            {
                var translationsBrowser = new TranslationsBrowser();
                if (translationsBrowser.ShowDialog() == DialogResult.OK)
                {
                    MessageBox.Show("Translations Browser closed. You may need to refresh the Quest Browser to see updated translations.",
                        "Translations Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Translations Browser: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
        private void cmbExistingKeys_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbExistingKeys.SelectedItem != null)
            {
                string selectedKey = cmbExistingKeys.SelectedItem.ToString();
                var translation = translations.FirstOrDefault(t => t.Key == selectedKey);
                if (translation != null)
                {
                    txtTranslationKey.Text = translation.Key;
                    txtTranslationText.Text = translation.Element?.PrimaryTranslation ?? "";
                    isNewTranslation = false;
                }
            }
        }
        
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtTranslationKey.Text))
            {
                MessageBox.Show("Translation key cannot be empty.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTranslationKey.Focus();
                return false;
            }
            
            if (string.IsNullOrWhiteSpace(txtTranslationText.Text))
            {
                MessageBox.Show("Translation text cannot be empty.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtTranslationText.Focus();
                return false;
            }
            
            return true;
        }
    }
}