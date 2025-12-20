using System;
using System.Drawing;
using System.Windows.Forms;
using SatelliteReignModdingTools.Properties;
using SatelliteReignModdingTools.Services;

namespace SatelliteReignModdingTools
{
    public partial class MainMenu : Form
    {
        private DataWatcher _watcher;
        private readonly SkillDataManager _skillManager = new SkillDataManager();
        private string _dataPath = "."; // managed via settings

        public MainMenu()
        {
            InitializeComponent();

            InitializeSettingsButton();

            // Load settings
            _dataPath = string.IsNullOrWhiteSpace(Settings.Default.DataDirectory) ? SRMod.Services.FileManager.ExecPath : Settings.Default.DataDirectory;

            // Initialize managers (non-fatal if files missing)
            _skillManager.Load(_dataPath);

            // Watch for changes to XML and reload
            if (Settings.Default.AutoReload)
                _watcher = new DataWatcher(_dataPath, "*.xml", OnDataChanged);
        }

        private void OnDataChanged()
        {
            try
            {
                // Reload managers
                _skillManager.Load(_dataPath);
                if (Settings.Default.AutoValidate)
                {
                    var res = _skillManager.Validate();
                    // Optionally surface warnings in future via status bar/event bus
                }
            }
            catch { }
        }

        private void btnItemBrowser_Click(object sender, EventArgs e)
        {
            var itemBrowser = new ItemBrowser();
            itemBrowser.Show();
        }

        private void btnEnemyBrowser_Click(object sender, EventArgs e)
        {
            //open enemy browser
            var itemBrowser = new EnemyBrowser();
            itemBrowser.Show();
        }

        private void btnQuestEditor_Click(object sender, EventArgs e)
        {
            var questBrowser = new QuestBrowser();
            questBrowser.Show();
        }

        private void btnTranslationsBrowser_Click(object sender, EventArgs e)
        {
            var translationsBrowser = new TranslationsBrowser();
            translationsBrowser.Show();
        }

        private void btnSkillsBrowser_Click(object sender, EventArgs e)
        {
            var skills = new SkillsBrowser(_skillManager);
            skills.Show();
        }

        private void btnEconomyBrowser_Click(object sender, EventArgs e)
        {
            var economyBrowser = new EconomyBrowser();
            economyBrowser.Show();
        }

        private void btnFactionBrowser_Click(object sender, EventArgs e)
        {
            var factionBrowser = new FactionBrowser();
            factionBrowser.Show();
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            _watcher?.Dispose();
        }
        private Button btnSettings;

        private void InitializeSettingsButton()
        {
            btnSettings = new Button();
            btnSettings.BackColor = Color.SeaGreen;
            btnSettings.ForeColor = Color.Aquamarine;
            btnSettings.Location = new System.Drawing.Point(100, 460);
            btnSettings.Name = "btnSettings";
            btnSettings.Size = new System.Drawing.Size(200, 28);
            btnSettings.TabIndex = 6;
            btnSettings.Text = "Settings";
            btnSettings.UseVisualStyleBackColor = false;
            btnSettings.Click += BtnSettings_Click;

            this.Controls.Add(btnSettings);
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            using (var dlg = new SettingsForm())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    // Restart watcher with new settings
                    _watcher?.Dispose();
                    if (Settings.Default.AutoReload)
                    {
                        _watcher = new Services.DataWatcher(Settings.Default.DataDirectory, "*.xml", OnDataChanged);
                    }
                }
            }
        }
    }
}