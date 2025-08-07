using System;
using System.IO;
using System.Windows.Forms;
using SatelliteReignModdingTools.Properties;

namespace SatelliteReignModdingTools
{
    public partial class SettingsForm : Form
    {
        private TextBox _dataDir;
        private CheckBox _autoReload;
        private CheckBox _autoValidate;
        private CheckBox _autoBackup;
        private Button _browse;
        private Button _save;

        public SettingsForm()
        {
            Text = "Settings";
            Width = 500;
            Height = 220;
            BackColor = System.Drawing.Color.Black;
            ForeColor = System.Drawing.Color.Aquamarine;

            var lbl = new Label { Text = "Data Directory", Left = 12, Top = 20, Width = 120, ForeColor = System.Drawing.Color.Aquamarine };
            _dataDir = new TextBox { Left = 140, Top = 18, Width = 260, BackColor = System.Drawing.Color.SeaGreen, ForeColor = System.Drawing.Color.Aquamarine };
            _browse = new Button { Text = "...", Left = 408, Top = 16, Width = 50, BackColor = System.Drawing.Color.SeaGreen };

            _autoReload = new CheckBox { Text = "Auto reload on change", Left = 140, Top = 56, Width = 220, Checked = true };
            _autoValidate = new CheckBox { Text = "Auto validate after reload", Left = 140, Top = 82, Width = 220 };
            _autoBackup = new CheckBox { Text = "Auto backup on save", Left = 140, Top = 108, Width = 220, Checked = true };

            _save = new Button { Text = "Save", Left = 380, Top = 140, Width = 78, BackColor = System.Drawing.Color.SeaGreen };

            Controls.Add(lbl);
            Controls.Add(_dataDir);
            Controls.Add(_browse);
            Controls.Add(_autoReload);
            Controls.Add(_autoValidate);
            Controls.Add(_autoBackup);
            Controls.Add(_save);

            Load += OnLoad;
            _browse.Click += OnBrowse;
            _save.Click += OnSave;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            _dataDir.Text = string.IsNullOrWhiteSpace(Settings.Default.DataDirectory)
                ? SRMod.Services.FileManager.ExecPath
                : Settings.Default.DataDirectory;
            _autoReload.Checked = Settings.Default.AutoReload;
            _autoValidate.Checked = Settings.Default.AutoValidate;
            _autoBackup.Checked = Settings.Default.AutoBackup;
        }

        private void OnBrowse(object sender, EventArgs e)
        {
            using (var dlg = new FolderBrowserDialog())
            {
                dlg.Description = "Select the folder containing exported XML (e.g. skillTree.xml, itemDefinitions.xml)";
                dlg.SelectedPath = Directory.Exists(_dataDir.Text) ? _dataDir.Text : SRMod.Services.FileManager.ExecPath;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    _dataDir.Text = dlg.SelectedPath;
                }
            }
        }

        private void OnSave(object sender, EventArgs e)
        {
            Settings.Default.DataDirectory = _dataDir.Text;
            Settings.Default.AutoReload = _autoReload.Checked;
            Settings.Default.AutoValidate = _autoValidate.Checked;
            Settings.Default.AutoBackup = _autoBackup.Checked;
            Settings.Default.Save();
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
