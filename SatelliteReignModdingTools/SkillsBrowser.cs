using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SatelliteReignModdingTools.Services;
using SatelliteReignModdingTools.Controls;

namespace SatelliteReignModdingTools
{
    public sealed class SkillsBrowser : Form
    {
        private readonly SkillDataManager _mgr;
        private readonly SplitContainer _split = new SplitContainer();
        private readonly ListBox _skills = new ListBox();
        private readonly PropertyGrid _grid = new PropertyGrid();
        private readonly TextBox _search = new TextBox();
        private readonly Button _btnValidate = new Button();
        private readonly Button _btnSave = new Button();

        public SkillsBrowser(SkillDataManager mgr)
        {
            _mgr = mgr;
            Text = "Skills & Progression";
            Width = 900;
            Height = 600;
            BackColor = Color.Black;
            ForeColor = Color.Aquamarine;

            _split.Dock = DockStyle.Fill;
            _split.Orientation = Orientation.Vertical;
            _split.SplitterDistance = 300;

            _skills.BackColor = Color.SeaGreen;
            _skills.ForeColor = Color.Aquamarine;
            _skills.Dock = DockStyle.Fill;

            _grid.Dock = DockStyle.Fill;
            _grid.HelpVisible = false;

            var rightPanel = new Panel { Dock = DockStyle.Fill };
            var buttons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 36, FlowDirection = FlowDirection.LeftToRight };
            var lblSearch = new Label { Text = "Search:", AutoSize = true, ForeColor = Color.Aquamarine, Margin = new Padding(6, 8, 6, 0) };
            _search.Width = 220;
            _search.BackColor = Color.DimGray;
            _search.ForeColor = Color.Aquamarine;
            _search.BorderStyle = BorderStyle.FixedSingle;
            _btnValidate.Text = "Validate";
            _btnValidate.BackColor = Color.SeaGreen;
            _btnSave.Text = "Save (with diff)";
            _btnSave.BackColor = Color.SeaGreen;
            buttons.Controls.AddRange(new Control[] { lblSearch, _search, _btnValidate, _btnSave });
            rightPanel.Controls.Add(buttons);
            rightPanel.Controls.Add(_grid);

            _split.Panel1.Controls.Add(_skills);
            _split.Panel2.Controls.Add(rightPanel);
            Controls.Add(_split);

            Load += OnLoad;
            _skills.SelectedIndexChanged += (s, e) => _grid.SelectedObject = _skills.SelectedItem;
            _btnValidate.Click += OnValidateClick;
            _btnSave.Click += OnSaveClick;
            _search.TextChanged += (s, e) => ApplySearch(_search.Text);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            var list = _mgr.SkillTree?.Skills ?? Array.Empty<DTOs.SerializableSkillData>();
            _skills.Items.Clear();
            foreach (var s in list.OrderBy(x => x.Id))
                _skills.Items.Add(s);

            _grid.SelectedObject = _mgr.Progression;
        }

        private void ApplySearch(string text)
        {
            text = text ?? string.Empty;
            var list = _mgr.SkillTree?.Skills ?? Array.Empty<DTOs.SerializableSkillData>();
            var filter = text.ToLowerInvariant();
            var filtered = string.IsNullOrWhiteSpace(filter)
                ? list
                : list.Where(s => ($"{s.Id}".Contains(filter)) || (s.DisplayName?.ToLowerInvariant().Contains(filter) ?? false) || (s.Key?.ToLowerInvariant().Contains(filter) ?? false)).ToArray();

            _skills.Items.Clear();
            foreach (var s in filtered.OrderBy(x => x.Id))
                _skills.Items.Add(s);
        }

        private void OnValidateClick(object sender, EventArgs e)
        {
            var res = _mgr.Validate();
            var msg = (res.IsValid ? "OK\n" : "ERRORS\n") + string.Join("\n", res.Errors ?? Array.Empty<string>()) + (res.Warnings?.Length > 0 ? "\n\nWarnings\n" + string.Join("\n", res.Warnings) : string.Empty);
            MessageBox.Show(this, msg, "Validation", MessageBoxButtons.OK, res.IsValid ? MessageBoxIcon.Information : MessageBoxIcon.Warning);
        }

        private void OnSaveClick(object sender, EventArgs e)
        {
            var basePath = SRMod.Services.FileManager.ExecPath;
            var oldSkill = XmlDiffUtil.ReadAllTextOrEmpty(Path.Combine(basePath, SkillDataManager.SkillTreeFile));
            var oldProg = XmlDiffUtil.ReadAllTextOrEmpty(Path.Combine(basePath, SkillDataManager.ProgressionFile));
            var newSkill = _mgr.GetSkillTreeXml();
            var newProg = _mgr.GetProgressionXml();

            var diffSkill = XmlDiffUtil.Diff(oldSkill, newSkill);
            var diffProg = XmlDiffUtil.Diff(oldProg, newProg);
            var msg = "skillTree.xml\n" + diffSkill + "\n\nprogressionData.xml\n" + diffProg;

            var dlgResult = DiffViewerForm.ShowDiff(this, "Confirm Save", msg);
            if (dlgResult == DialogResult.OK)
            {
                var ok = _mgr.Save(basePath, backupExisting: true);
                MessageBox.Show(this, ok ? "Saved." : "Save failed.", ok ? "Save" : "Error", MessageBoxButtons.OK, ok ? MessageBoxIcon.Information : MessageBoxIcon.Error);
            }
        }
    }
}
