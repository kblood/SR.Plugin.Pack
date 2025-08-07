using System;
using System.Drawing;
using System.Windows.Forms;

namespace SatelliteReignModdingTools.Controls
{
    public sealed class DiffViewerForm : Form
    {
        private readonly TextBox _text;
        private readonly Button _ok;
        private readonly Button _cancel;

        private DiffViewerForm(string title, string text)
        {
            Text = title;
            Width = 900;
            Height = 700;
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.Black;
            ForeColor = Color.Aquamarine;

            _text = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Both,
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.Aquamarine,
                Font = new Font(FontFamily.GenericMonospace, 9f),
                Text = text
            };

            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 40,
                FlowDirection = FlowDirection.RightToLeft
            };

            _ok = new Button { Text = "OK", DialogResult = DialogResult.OK, BackColor = Color.SeaGreen, ForeColor = Color.Aquamarine, Width = 100 };
            _cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, BackColor = Color.SeaGreen, ForeColor = Color.Aquamarine, Width = 100 };
            panel.Controls.Add(_ok);
            panel.Controls.Add(_cancel);

            Controls.Add(_text);
            Controls.Add(panel);

            AcceptButton = _ok;
            CancelButton = _cancel;
        }

        public static DialogResult ShowDiff(IWin32Window owner, string title, string diffText)
        {
            using (var dlg = new DiffViewerForm(title, diffText))
            {
                return dlg.ShowDialog(owner);
            }
        }
    }
}
