using System;
using System.Drawing;
using System.Windows.Forms;

namespace SatelliteReignModdingTools.Controls
{
    // Lightweight shared toolbar for browsers. Programmatic UI to avoid designer churn.
    public sealed class SharedToolbar : UserControl
    {
        private readonly Button _btnReload;
        private readonly Button _btnValidate;
        private readonly Button _btnSave;
        private readonly Button _btnSaveDiff;
        private readonly TextBox _txtSearch;
        private readonly Label _lblSearch;

        public event Action ReloadClicked;
        public event Action ValidateClicked;
        public event Action SaveClicked;
        public event Action SaveWithDiffClicked;
        public event Action<string> SearchTextChanged;

        public SharedToolbar()
        {
            Height = 32;
            Dock = DockStyle.Top;
            BackColor = Color.FromArgb(48, 48, 48);

            _btnReload = MakeButton("Reload", 8, 4, (s, e) => ReloadClicked?.Invoke());
            _btnValidate = MakeButton("Validate", 84, 4, (s, e) => ValidateClicked?.Invoke());
            _btnSave = MakeButton("Save", 170, 4, (s, e) => SaveClicked?.Invoke());
            _btnSaveDiff = MakeButton("Save+Diff", 230, 4, (s, e) => SaveWithDiffClicked?.Invoke());

            _lblSearch = new Label
            {
                Text = "Search:",
                ForeColor = Color.Aquamarine,
                AutoSize = true,
                Location = new Point(320, 9)
            };

            _txtSearch = new TextBox
            {
                Width = 220,
                Location = new Point(380, 6),
                BackColor = Color.DimGray,
                ForeColor = Color.Aquamarine,
                BorderStyle = BorderStyle.FixedSingle
            };
            _txtSearch.TextChanged += (s, e) => SearchTextChanged?.Invoke(_txtSearch.Text);

            Controls.Add(_btnReload);
            Controls.Add(_btnValidate);
            Controls.Add(_btnSave);
            Controls.Add(_btnSaveDiff);
            Controls.Add(_lblSearch);
            Controls.Add(_txtSearch);
        }

        private Button MakeButton(string text, int x, int y, EventHandler onClick)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(x, y),
                Width = 72,
                Height = 24,
                BackColor = Color.SeaGreen,
                ForeColor = Color.Aquamarine,
                FlatStyle = FlatStyle.Standard
            };
            btn.Click += onClick;
            return btn;
        }

        // Allow host to set/get search text
        public string SearchText
        {
            get => _txtSearch.Text;
            set => _txtSearch.Text = value ?? string.Empty;
        }
    }
}
