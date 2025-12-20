using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using SatelliteReignModdingTools.Services;
using SRMod.Services;

namespace SatelliteReignModdingTools
{
    public partial class IconPickerDialog : Form
    {
        public string SelectedIconName { get; private set; }
        private List<string> _availableIcons;
        private const int ICON_SIZE = 64;
        private const int ICONS_PER_ROW = 8;
        private Panel _iconPanel;
        private TextBox _searchBox;
        private Label _statusLabel;

        public IconPickerDialog(string currentIconName = null)
        {
            try
            {
                SRMod.Services.SRInfoHelper.Log($"IconPickerDialog: Initializing with icon '{currentIconName}'");
                InitializeComponent();
                SelectedIconName = currentIconName;

                SRMod.Services.SRInfoHelper.Log("IconPickerDialog: Loading available icons");
                LoadAvailableIcons();

                SRMod.Services.SRInfoHelper.Log("IconPickerDialog: Creating icon grid");
                CreateIconGrid();

                if (!string.IsNullOrEmpty(currentIconName))
                {
                    SRMod.Services.SRInfoHelper.Log($"IconPickerDialog: Highlighting current icon '{currentIconName}'");
                    HighlightCurrentIcon(currentIconName);
                }

                SRMod.Services.SRInfoHelper.Log("IconPickerDialog: Initialization complete");
            }
            catch (Exception ex)
            {
                SRMod.Services.SRInfoHelper.Log($"IconPickerDialog: Error during initialization - {ex}");
                throw;
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            
            // Form properties
            this.Text = "Select Item Icon";
            this.Size = new Size(600, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.Black;
            this.ForeColor = Color.Aquamarine;

            // Search box
            _searchBox = new TextBox
            {
                Location = new Point(10, 10),
                Size = new Size(200, 23),
                BackColor = Color.DarkGray,
                ForeColor = Color.Black,
                Text = ""
            };
            _searchBox.TextChanged += SearchBox_TextChanged;
            _searchBox.ForeColor = Color.Gray;
            _searchBox.Text = "Search icons...";
            _searchBox.Enter += (s, e) => {
                if (_searchBox.Text == "Search icons...")
                {
                    _searchBox.Text = "";
                    _searchBox.ForeColor = Color.Black;
                }
            };
            _searchBox.Leave += (s, e) => {
                if (string.IsNullOrEmpty(_searchBox.Text))
                {
                    _searchBox.Text = "Search icons...";
                    _searchBox.ForeColor = Color.Gray;
                }
            };
            this.Controls.Add(_searchBox);

            // Search label
            var searchLabel = new Label
            {
                Text = "Search:",
                Location = new Point(10, 40),
                Size = new Size(50, 20),
                ForeColor = Color.Aquamarine
            };
            this.Controls.Add(searchLabel);

            // Status label
            _statusLabel = new Label
            {
                Location = new Point(220, 12),
                Size = new Size(200, 20),
                ForeColor = Color.Aquamarine,
                Text = "Loading icons..."
            };
            this.Controls.Add(_statusLabel);

            // Icon panel with scrolling
            _iconPanel = new Panel
            {
                Location = new Point(10, 70),
                Size = new Size(560, 350),
                BackColor = Color.DarkSlateGray,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };
            this.Controls.Add(_iconPanel);

            // OK button
            var okButton = new Button
            {
                Text = "OK",
                Location = new Point(420, 430),
                Size = new Size(75, 30),
                BackColor = Color.SeaGreen,
                ForeColor = Color.Aquamarine,
                DialogResult = DialogResult.OK
            };
            okButton.Click += (s, e) => this.DialogResult = DialogResult.OK;
            this.Controls.Add(okButton);

            // Cancel button
            var cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(505, 430),
                Size = new Size(75, 30),
                BackColor = Color.DarkRed,
                ForeColor = Color.White,
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(cancelButton);

            this.ResumeLayout();
        }

        private void LoadAvailableIcons()
        {
            try
            {
                string iconsPath = SRMod.Services.FileManager.GetIconsPath();
                if (Directory.Exists(iconsPath))
                {
                    _availableIcons = Directory.GetFiles(iconsPath, "*.png")
                        .Select(Path.GetFileNameWithoutExtension)
                        .Where(name => !string.IsNullOrEmpty(name))
                        .OrderBy(name => name)
                        .ToList();
                        
                    SRMod.Services.SRInfoHelper.Log($"Loaded {_availableIcons.Count} icons from {iconsPath}");
                }
                else
                {
                    _availableIcons = new List<string>();
                    SRMod.Services.SRInfoHelper.Log($"Icons directory not found: {iconsPath}");
                }

                _statusLabel.Text = $"Found {_availableIcons.Count} icons";
            }
            catch (Exception ex)
            {
                _availableIcons = new List<string>();
                _statusLabel.Text = $"Error loading icons: {ex.Message}";
                _statusLabel.ForeColor = Color.Red;
                SRMod.Services.SRInfoHelper.Log($"Error loading icons: {ex}");
            }
        }

        private void CreateIconGrid()
        {
            try
            {
                _iconPanel.Controls.Clear();

                if (_availableIcons == null || _availableIcons.Count == 0)
                {
                    var noIconsLabel = new Label
                    {
                        Text = "No icons found in icons/ directory",
                        Location = new Point(10, 10),
                        Size = new Size(400, 30),
                        ForeColor = Color.Orange,
                        Font = new Font("Arial", 10, FontStyle.Bold)
                    };
                    _iconPanel.Controls.Add(noIconsLabel);
                    return;
                }

                var filteredIcons = GetFilteredIcons();
                if (filteredIcons == null || filteredIcons.Count == 0)
                {
                    var noResultsLabel = new Label
                    {
                        Text = "No icons match your search",
                        Location = new Point(10, 10),
                        Size = new Size(300, 30),
                        ForeColor = Color.Yellow
                    };
                    _iconPanel.Controls.Add(noResultsLabel);
                    _statusLabel.Text = $"0 of {_availableIcons.Count} icons match search";
                    return;
                }

                int x = 10, y = 10;
                int iconIndex = 0;

                foreach (string iconName in filteredIcons)
                {
                    if (string.IsNullOrEmpty(iconName)) continue;

                    var iconButton = CreateIconButton(iconName, x, y);
                    if (iconButton != null)
                    {
                        _iconPanel.Controls.Add(iconButton);
                    }

                    x += ICON_SIZE + 10;
                    iconIndex++;

                    if (iconIndex % ICONS_PER_ROW == 0)
                    {
                        x = 10;
                        y += ICON_SIZE + 10;
                    }
                }

                _statusLabel.Text = $"Showing {filteredIcons.Count} of {_availableIcons.Count} icons";
                _statusLabel.ForeColor = Color.Aquamarine;
            }
            catch (Exception ex)
            {
                SRMod.Services.SRInfoHelper.Log($"Error creating icon grid: {ex}");
                var errorLabel = new Label
                {
                    Text = $"Error loading icons: {ex.Message}",
                    Location = new Point(10, 10),
                    Size = new Size(400, 60),
                    ForeColor = Color.Red,
                    Font = new Font("Arial", 9)
                };
                _iconPanel.Controls.Clear();
                _iconPanel.Controls.Add(errorLabel);
                _statusLabel.Text = "Error loading icons";
                _statusLabel.ForeColor = Color.Red;
            }
        }

        private List<string> GetFilteredIcons()
        {
            try
            {
                if (_availableIcons == null)
                    return new List<string>();

                if (string.IsNullOrEmpty(_searchBox?.Text) || _searchBox.Text == "Search icons...")
                    return _availableIcons;

                var searchTerm = _searchBox.Text.ToLower();
                return _availableIcons
                    .Where(icon => !string.IsNullOrEmpty(icon) && icon.ToLower().Contains(searchTerm))
                    .ToList();
            }
            catch (Exception ex)
            {
                SRMod.Services.SRInfoHelper.Log($"Error filtering icons: {ex.Message}");
                return _availableIcons ?? new List<string>();
            }
        }

        private Button CreateIconButton(string iconName, int x, int y)
        {
            try
            {
                if (string.IsNullOrEmpty(iconName))
                {
                    SRMod.Services.SRInfoHelper.Log("Skipping null or empty icon name");
                    return null;
                }

                var button = new Button
                {
                    Size = new Size(ICON_SIZE, ICON_SIZE),
                    Location = new Point(x, y),
                    BackColor = Color.DarkGray,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 1, BorderColor = Color.Gray },
                    Tag = iconName,
                    Cursor = Cursors.Hand
                };

                // Load and set icon image
                try
                {
                    var iconImage = SRMod.Services.FileManager.LoadImageFromFile(iconName);
                    if (iconImage != null)
                    {
                        // Scale image to fit button
                        var scaledImage = new Bitmap(iconImage, new Size(ICON_SIZE - 4, ICON_SIZE - 4));
                        button.Image = scaledImage;
                        button.ImageAlign = ContentAlignment.MiddleCenter;
                    }
                    else
                    {
                        // If icon fails to load, show text instead
                        string displayText = iconName.Length > 8 ? iconName.Substring(0, 8) + "..." : iconName;
                        button.Text = displayText;
                        button.Font = new Font("Arial", 6);
                        button.ForeColor = Color.White;
                    }
                }
                catch (Exception imageEx)
                {
                    SRMod.Services.SRInfoHelper.Log($"Failed to load icon image '{iconName}': {imageEx.Message}");
                    // Fallback to text display
                    string displayText = iconName.Length > 8 ? iconName.Substring(0, 8) + "..." : iconName;
                    button.Text = displayText;
                    button.Font = new Font("Arial", 6);
                    button.ForeColor = Color.White;
                }

                // Click handler
                button.Click += (sender, e) =>
                {
                    try
                    {
                        SelectedIconName = iconName;
                        HighlightSelectedIcon(button);
                    }
                    catch (Exception clickEx)
                    {
                        SRMod.Services.SRInfoHelper.Log($"Error handling icon click: {clickEx.Message}");
                    }
                };

                // Double-click to select and close
                button.DoubleClick += (sender, e) =>
                {
                    try
                    {
                        SelectedIconName = iconName;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    catch (Exception dblClickEx)
                    {
                        SRMod.Services.SRInfoHelper.Log($"Error handling icon double-click: {dblClickEx.Message}");
                    }
                };

                // Tooltip with full icon name
                try
                {
                    var tooltip = new System.Windows.Forms.ToolTip();
                    tooltip.SetToolTip(button, iconName);
                }
                catch (Exception tooltipEx)
                {
                    SRMod.Services.SRInfoHelper.Log($"Failed to set tooltip for icon '{iconName}': {tooltipEx.Message}");
                }

                return button;
            }
            catch (Exception ex)
            {
                SRMod.Services.SRInfoHelper.Log($"Error creating icon button for '{iconName}': {ex}");
                return null;
            }
        }

        private void HighlightCurrentIcon(string iconName)
        {
            try
            {
                if (string.IsNullOrEmpty(iconName) || _iconPanel == null) return;

                foreach (Control control in _iconPanel.Controls)
                {
                    if (control is Button button && button.Tag?.ToString() == iconName)
                    {
                        HighlightSelectedIcon(button);
                        
                        // Scroll to show the selected icon
                        try
                        {
                            _iconPanel.ScrollControlIntoView(button);
                        }
                        catch (Exception scrollEx)
                        {
                            SRMod.Services.SRInfoHelper.Log($"Error scrolling to selected icon: {scrollEx.Message}");
                        }
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                SRMod.Services.SRInfoHelper.Log($"Error highlighting current icon '{iconName}': {ex.Message}");
            }
        }

        private void HighlightSelectedIcon(Button selectedButton)
        {
            try
            {
                if (selectedButton == null || _iconPanel == null) return;

                // Reset all buttons to default appearance
                foreach (Control control in _iconPanel.Controls)
                {
                    if (control is Button button)
                    {
                        try
                        {
                            button.FlatAppearance.BorderColor = Color.Gray;
                            button.FlatAppearance.BorderSize = 1;
                        }
                        catch (Exception buttonEx)
                        {
                            SRMod.Services.SRInfoHelper.Log($"Error resetting button appearance: {buttonEx.Message}");
                        }
                    }
                }

                // Highlight selected button
                selectedButton.FlatAppearance.BorderColor = Color.Lime;
                selectedButton.FlatAppearance.BorderSize = 3;
            }
            catch (Exception ex)
            {
                SRMod.Services.SRInfoHelper.Log($"Error highlighting selected icon: {ex.Message}");
            }
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            try
            {
                CreateIconGrid();
            }
            catch (Exception ex)
            {
                SRMod.Services.SRInfoHelper.Log($"Error handling search box text change: {ex.Message}");
            }
        }
    }
}