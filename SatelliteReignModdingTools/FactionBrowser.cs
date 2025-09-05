using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SatelliteReignModdingTools.Controls;

namespace SatelliteReignModdingTools
{
    public sealed class FactionBrowser : Form
    {
        private readonly SplitContainer _split = new SplitContainer();
        private readonly ListBox _factionList = new ListBox();
        private readonly PropertyGrid _grid = new PropertyGrid();
        private SharedToolbar _toolbar;

        public FactionBrowser()
        {
            Text = "Faction & Reputation";
            Width = 900;
            Height = 600;
            BackColor = Color.Black;
            ForeColor = Color.Aquamarine;

            // Insert shared toolbar at the top
            _toolbar = new SharedToolbar();
            _toolbar.ReloadClicked += () => ReloadData();
            _toolbar.SaveClicked += () => SaveData(false);
            _toolbar.SaveWithDiffClicked += () => SaveData(true);
            _toolbar.ValidateClicked += () => ShowValidationResults();
            _toolbar.SearchTextChanged += text => ApplySearch(text);
            Controls.Add(_toolbar);

            _split.Dock = DockStyle.Fill;
            _split.Orientation = Orientation.Vertical;
            _split.SplitterDistance = 300;

            _factionList.BackColor = Color.SeaGreen;
            _factionList.ForeColor = Color.Aquamarine;
            _factionList.Dock = DockStyle.Fill;

            _grid.Dock = DockStyle.Fill;
            _grid.HelpVisible = false;

            var rightPanel = new Panel { Dock = DockStyle.Fill };
            rightPanel.Controls.Add(_grid);

            _split.Panel1.Controls.Add(_factionList);
            _split.Panel2.Controls.Add(rightPanel);
            Controls.Add(_split);

            Load += OnLoad;
            _factionList.SelectedIndexChanged += (s, e) => _grid.SelectedObject = _factionList.SelectedItem;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void ReloadData()
        {
            _factionList.Items.Clear();
            // Placeholder for future faction data integration
            _factionList.Items.Add("Faction data not yet implemented");
            _factionList.Items.Add("Future: Corporate factions");
            _factionList.Items.Add("Future: Reputation systems");
            _factionList.Items.Add("Future: Faction relationships");
        }

        private void SaveData(bool showDiff)
        {
            MessageBox.Show(this, "Faction data saving not yet implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowValidationResults()
        {
            MessageBox.Show(this, "Faction validation not yet implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ApplySearch(string text)
        {
            // Placeholder search implementation
            _factionList.Items.Clear();
            if (string.IsNullOrWhiteSpace(text))
            {
                ReloadData();
            }
            else
            {
                _factionList.Items.Add($"Searching for: {text}");
                _factionList.Items.Add("Search not yet implemented");
            }
        }
    }
}