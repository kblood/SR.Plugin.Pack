using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SatelliteReignModdingTools.Controls;

namespace SatelliteReignModdingTools
{
    public sealed class EconomyBrowser : Form
    {
        private readonly SplitContainer _split = new SplitContainer();
        private readonly ListBox _economyList = new ListBox();
        private readonly PropertyGrid _grid = new PropertyGrid();
        private SharedToolbar _toolbar;

        public EconomyBrowser()
        {
            Text = "Economy & Pricing";
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

            _economyList.BackColor = Color.SeaGreen;
            _economyList.ForeColor = Color.Aquamarine;
            _economyList.Dock = DockStyle.Fill;

            _grid.Dock = DockStyle.Fill;
            _grid.HelpVisible = false;

            var rightPanel = new Panel { Dock = DockStyle.Fill };
            rightPanel.Controls.Add(_grid);

            _split.Panel1.Controls.Add(_economyList);
            _split.Panel2.Controls.Add(rightPanel);
            Controls.Add(_split);

            Load += OnLoad;
            _economyList.SelectedIndexChanged += (s, e) => _grid.SelectedObject = _economyList.SelectedItem;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            ReloadData();
        }

        private void ReloadData()
        {
            _economyList.Items.Clear();
            // Placeholder for future economy data integration
            _economyList.Items.Add("Economy data not yet implemented");
            _economyList.Items.Add("Future: Item pricing");
            _economyList.Items.Add("Future: District economies");
            _economyList.Items.Add("Future: Trade routes");
        }

        private void SaveData(bool showDiff)
        {
            MessageBox.Show(this, "Economy data saving not yet implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowValidationResults()
        {
            MessageBox.Show(this, "Economy validation not yet implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ApplySearch(string text)
        {
            // Placeholder search implementation
            _economyList.Items.Clear();
            if (string.IsNullOrWhiteSpace(text))
            {
                ReloadData();
            }
            else
            {
                _economyList.Items.Add($"Searching for: {text}");
                _economyList.Items.Add("Search not yet implemented");
            }
        }
    }
}