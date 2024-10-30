using System.Windows.Forms;

namespace SatelliteReignModdingTools
{
    partial class EnemyBrowser
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ListBox EnemyListBox;
        private System.Windows.Forms.ListBox ModifierListBox;
        private System.Windows.Forms.TextBox EnemyNameTextBox;
        private System.Windows.Forms.TextBox ModifierNameTextBox;
        private System.Windows.Forms.TextBox ModifierDescriptionTextBox;
        private System.Windows.Forms.TextBox AmountTextBox;
        private System.Windows.Forms.ComboBox GroupIDDropDown;
        private System.Windows.Forms.ComboBox WardrobeTypeDropDown;
        private System.Windows.Forms.ComboBox WeaponTypeDropDown;
        private System.Windows.Forms.ComboBox ModifierDropdown;
        private System.Windows.Forms.CheckBox SpawnableCheckbox;
        private System.Windows.Forms.CheckBox SoloCheckbox;
        private System.Windows.Forms.CheckBox UseWardrobeOverrideCheckbox;
        private System.Windows.Forms.Button SaveEnemyButton;
        private System.Windows.Forms.Button CopyEnemyButton;
        private System.Windows.Forms.Button DeleteEnemyButton;
        private System.Windows.Forms.Button AddModifierButton;
        private System.Windows.Forms.Button SaveModifierButton;
        private System.Windows.Forms.Button DeleteModifierButton;
        private System.Windows.Forms.ComboBox ItemDropDown;
        private System.Windows.Forms.Button MoveItemUpButton;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();

            // Main Enemy Controls
            this.EnemyListBox = new System.Windows.Forms.ListBox();
            this.EnemyNameTextBox = new System.Windows.Forms.TextBox();
            this.GroupIDDropDown = new System.Windows.Forms.ComboBox();
            this.WardrobeTypeDropDown = new System.Windows.Forms.ComboBox();
            this.WeaponTypeDropDown = new System.Windows.Forms.ComboBox();
            this.SpawnableCheckbox = new System.Windows.Forms.CheckBox();
            this.SoloCheckbox = new System.Windows.Forms.CheckBox();
            this.UseWardrobeOverrideCheckbox = new System.Windows.Forms.CheckBox();
            this.SaveEnemyButton = new System.Windows.Forms.Button();
            this.CopyEnemyButton = new System.Windows.Forms.Button();
            this.DeleteEnemyButton = new System.Windows.Forms.Button();

            // Count Keys Controls
            this.CountKeysGrid = new System.Windows.Forms.DataGridView();
            this.AddCountKeyButton = new System.Windows.Forms.Button();
            this.DeleteCountKeyButton = new System.Windows.Forms.Button();

            // Items Controls
            this.ItemListBox = new System.Windows.Forms.ListBox();
            this.ItemDropDown = new System.Windows.Forms.ComboBox();
            this.AddItemButton = new System.Windows.Forms.Button();
            this.RemoveItemButton = new System.Windows.Forms.Button();
            this.MoveItemUpButton = new System.Windows.Forms.Button();


            // Modifier Controls
            this.ModifierListBox = new System.Windows.Forms.ListBox();
            this.ModifierDropdown = new System.Windows.Forms.ComboBox();
            this.ModifierNameTextBox = new System.Windows.Forms.TextBox();
            this.ModifierDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.AmountTextBox = new System.Windows.Forms.TextBox();
            this.AddModifierButton = new System.Windows.Forms.Button();
            this.SaveModifierButton = new System.Windows.Forms.Button();
            this.DeleteModifierButton = new System.Windows.Forms.Button();

            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CountKeysGrid)).BeginInit();
            this.SuspendLayout();

            // MenuStrip Setup
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] 
            {
                this.fileToolStripMenuItem
            });
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(918, 24);
            this.menuStrip1.TabIndex = 0;

            // File Menu Setup
            this.fileToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.Aquamarine;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] 
            {
                this.saveToolStripMenuItem,
                this.loadToolStripMenuItem
            });
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";

            this.saveToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);

            this.loadToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);

            // Enemy List Section
            this.EnemyListBox.BackColor = System.Drawing.Color.SeaGreen;
            this.EnemyListBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.EnemyListBox.FormattingEnabled = true;
            this.EnemyListBox.Location = new System.Drawing.Point(12, 45);
            this.EnemyListBox.Name = "EnemyListBox";
            this.EnemyListBox.Size = new System.Drawing.Size(312, 455);
            this.EnemyListBox.TabIndex = 1;
            this.EnemyListBox.SelectedIndexChanged += new System.EventHandler(this.EnemyListBox_SelectedIndexChanged);

            // Enemy Details Section
            this.EnemyNameTextBox.BackColor = System.Drawing.Color.SeaGreen;
            this.EnemyNameTextBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.EnemyNameTextBox.Location = new System.Drawing.Point(330, 45);
            this.EnemyNameTextBox.Size = new System.Drawing.Size(250, 20);

            Label lblGroup = new Label();
            lblGroup.Text = "Group:";
            lblGroup.ForeColor = System.Drawing.Color.Aquamarine;
            lblGroup.Location = new System.Drawing.Point(330, 75);
            lblGroup.Size = new System.Drawing.Size(100, 20);

            this.GroupIDDropDown.BackColor = System.Drawing.Color.DimGray;
            this.GroupIDDropDown.ForeColor = System.Drawing.Color.Aquamarine;
            this.GroupIDDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GroupIDDropDown.Location = new System.Drawing.Point(330, 95);
            this.GroupIDDropDown.Size = new System.Drawing.Size(250, 21);

            Label lblWardrobe = new Label();
            lblWardrobe.Text = "Wardrobe Type:";
            lblWardrobe.ForeColor = System.Drawing.Color.Aquamarine;
            lblWardrobe.Location = new System.Drawing.Point(330, 125);
            lblWardrobe.Size = new System.Drawing.Size(100, 20);

            this.WardrobeTypeDropDown.BackColor = System.Drawing.Color.DimGray;
            this.WardrobeTypeDropDown.ForeColor = System.Drawing.Color.Aquamarine;
            this.WardrobeTypeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WardrobeTypeDropDown.Location = new System.Drawing.Point(330, 145);
            this.WardrobeTypeDropDown.Size = new System.Drawing.Size(250, 21);

            Label lblWeapon = new Label();
            lblWeapon.Text = "Weapon Type:";
            lblWeapon.ForeColor = System.Drawing.Color.Aquamarine;
            lblWeapon.Location = new System.Drawing.Point(330, 175);
            lblWeapon.Size = new System.Drawing.Size(100, 20);

            this.WeaponTypeDropDown.BackColor = System.Drawing.Color.DimGray;
            this.WeaponTypeDropDown.ForeColor = System.Drawing.Color.Aquamarine;
            this.WeaponTypeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WeaponTypeDropDown.Location = new System.Drawing.Point(330, 195);
            this.WeaponTypeDropDown.Size = new System.Drawing.Size(250, 21);

            // Checkboxes
            this.SpawnableCheckbox.AutoSize = true;
            this.SpawnableCheckbox.ForeColor = System.Drawing.Color.Aquamarine;
            this.SpawnableCheckbox.Location = new System.Drawing.Point(330, 225);
            this.SpawnableCheckbox.Text = "Spawnable";

            this.SoloCheckbox.AutoSize = true;
            this.SoloCheckbox.ForeColor = System.Drawing.Color.Aquamarine;
            this.SoloCheckbox.Location = new System.Drawing.Point(430, 225);
            this.SoloCheckbox.Text = "Solo";

            this.UseWardrobeOverrideCheckbox.AutoSize = true;
            this.UseWardrobeOverrideCheckbox.ForeColor = System.Drawing.Color.Aquamarine;
            this.UseWardrobeOverrideCheckbox.Location = new System.Drawing.Point(330, 245);
            this.UseWardrobeOverrideCheckbox.Size = new System.Drawing.Size(130, 17);
            this.UseWardrobeOverrideCheckbox.Text = "Use Wardrobe Override";

            // Enemy Action Buttons
            this.SaveEnemyButton.BackColor = System.Drawing.Color.SeaGreen;
            this.SaveEnemyButton.Location = new System.Drawing.Point(330, 275);
            this.SaveEnemyButton.Size = new System.Drawing.Size(80, 23);
            this.SaveEnemyButton.Text = "Save Enemy";
            this.SaveEnemyButton.Click += new System.EventHandler(this.SaveEnemyButton_Click);

            this.CopyEnemyButton.BackColor = System.Drawing.Color.SeaGreen;
            this.CopyEnemyButton.Location = new System.Drawing.Point(415, 275);
            this.CopyEnemyButton.Size = new System.Drawing.Size(80, 23);
            this.CopyEnemyButton.Text = "Copy Enemy";
            this.CopyEnemyButton.Click += new System.EventHandler(this.CopyEnemyButton_Click);

            this.DeleteEnemyButton.BackColor = System.Drawing.Color.SeaGreen;
            this.DeleteEnemyButton.Location = new System.Drawing.Point(500, 275);
            this.DeleteEnemyButton.Size = new System.Drawing.Size(80, 23);
            this.DeleteEnemyButton.Text = "Delete Enemy";
            this.DeleteEnemyButton.Click += new System.EventHandler(this.DeleteEnemyButton_Click);

            // Count Keys Section
            Label lblCountKeys = new Label();
            lblCountKeys.Text = "Count Keys (Count/Progression):";
            lblCountKeys.ForeColor = System.Drawing.Color.Aquamarine;
            lblCountKeys.Location = new System.Drawing.Point(330, 310);
            lblCountKeys.Size = new System.Drawing.Size(200, 20);

            this.CountKeysGrid.BackgroundColor = System.Drawing.Color.SeaGreen;
            this.CountKeysGrid.ForeColor = System.Drawing.Color.Black;
            this.CountKeysGrid.GridColor = System.Drawing.Color.Aquamarine;
            this.CountKeysGrid.Location = new System.Drawing.Point(330, 330);
            this.CountKeysGrid.Size = new System.Drawing.Size(250, 120);
            this.CountKeysGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.CountKeysGrid.RowHeadersVisible = false;
            this.CountKeysGrid.AllowUserToResizeRows = false;
            this.CountKeysGrid.AllowUserToAddRows = false;
            this.CountKeysGrid.MultiSelect = false;
            this.CountKeysGrid.EditMode = DataGridViewEditMode.EditOnEnter;

            var countColumn = new DataGridViewTextBoxColumn
            {
                Name = "Count",
                HeaderText = "Count",
                DataPropertyName = "Count"
            };

            var progressionColumn = new DataGridViewTextBoxColumn
            {
                Name = "Progression",
                HeaderText = "Progression",
                DataPropertyName = "Progression"
            };

            this.CountKeysGrid.Columns.AddRange(new DataGridViewColumn[] 
            {
                countColumn,
                progressionColumn
            });
            this.CountKeysGrid.CellValidating += CountKeysGrid_CellValidating;

            this.AddCountKeyButton.BackColor = System.Drawing.Color.SeaGreen;
            this.AddCountKeyButton.Location = new System.Drawing.Point(590, 330);
            this.AddCountKeyButton.Size = new System.Drawing.Size(60, 23);
            this.AddCountKeyButton.Text = "Add";
            this.AddCountKeyButton.Click += new System.EventHandler(this.AddCountKeyButton_Click);

            this.DeleteCountKeyButton.BackColor = System.Drawing.Color.SeaGreen;
            this.DeleteCountKeyButton.Location = new System.Drawing.Point(590, 360);
            this.DeleteCountKeyButton.Size = new System.Drawing.Size(60, 23);
            this.DeleteCountKeyButton.Text = "Delete";
            this.DeleteCountKeyButton.Click += new System.EventHandler(this.DeleteCountKeyButton_Click);

            // Items Section
            Label lblItems = new Label();
            lblItems.Text = "Equipped Items:";
            lblItems.ForeColor = System.Drawing.Color.Aquamarine;
            lblItems.Location = new System.Drawing.Point(330, 460);
            lblItems.Size = new System.Drawing.Size(90, 20);

            this.ItemListBox.BackColor = System.Drawing.Color.SeaGreen;
            this.ItemListBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.ItemListBox.Location = new System.Drawing.Point(330, 480);
            this.ItemListBox.Size = new System.Drawing.Size(250, 80);
            this.ItemListBox.SelectionMode = SelectionMode.One;

            Label lblAvailableItems = new Label();
            lblAvailableItems.Text = "Available Items:";
            lblAvailableItems.ForeColor = System.Drawing.Color.Aquamarine;
            lblAvailableItems.Location = new System.Drawing.Point(590, 460);
            lblAvailableItems.Size = new System.Drawing.Size(90, 20);

            this.ItemDropDown.BackColor = System.Drawing.Color.DimGray;
            this.ItemDropDown.ForeColor = System.Drawing.Color.Aquamarine;
            this.ItemDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ItemDropDown.Location = new System.Drawing.Point(590, 480);
            this.ItemDropDown.Size = new System.Drawing.Size(250, 21);

            // Adjust button positions
            this.AddItemButton.BackColor = System.Drawing.Color.SeaGreen;
            this.AddItemButton.Location = new System.Drawing.Point(850, 480);
            this.AddItemButton.Size = new System.Drawing.Size(60, 23);
            this.AddItemButton.Text = "Add";
            this.AddItemButton.Click += new System.EventHandler(this.AddItemButton_Click);

            this.RemoveItemButton.BackColor = System.Drawing.Color.SeaGreen;
            this.RemoveItemButton.Location = new System.Drawing.Point(590, 510);
            this.RemoveItemButton.Size = new System.Drawing.Size(60, 23);
            this.RemoveItemButton.Text = "Remove";
            this.RemoveItemButton.Click += new System.EventHandler(this.RemoveItemButton_Click);

            this.MoveItemUpButton.BackColor = System.Drawing.Color.SeaGreen;
            this.MoveItemUpButton.Location = new System.Drawing.Point(660, 510);
            this.MoveItemUpButton.Size = new System.Drawing.Size(60, 23);
            this.MoveItemUpButton.Text = "Move Up";
            this.MoveItemUpButton.Click += new System.EventHandler(this.MoveItemUpButton_Click);

            // Increase the form size
            this.ClientSize = new System.Drawing.Size(918, 620); // Increased height from 580 to 620

            // Modifier Section
            Label lblModifiers = new Label();
            lblModifiers.Text = "Modifiers:";
            lblModifiers.ForeColor = System.Drawing.Color.Aquamarine;
            lblModifiers.Location = new System.Drawing.Point(660, 45);
            lblModifiers.Size = new System.Drawing.Size(90, 20);

            this.ModifierListBox.Location = new System.Drawing.Point(660, 65);
            this.ModifierListBox.Size = new System.Drawing.Size(250, 160);
            this.ModifierListBox.BackColor = System.Drawing.Color.SeaGreen;
            this.ModifierListBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.ModifierListBox.FormattingEnabled = true;
            this.ModifierListBox.SelectedIndexChanged += new System.EventHandler(this.ModifierListBox_SelectedIndexChanged);

            this.ModifierDropdown.BackColor = System.Drawing.Color.DimGray;
            this.ModifierDropdown.ForeColor = System.Drawing.Color.Aquamarine;
            this.ModifierDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModifierDropdown.Location = new System.Drawing.Point(660, 235);
            this.ModifierDropdown.Size = new System.Drawing.Size(147, 21);

            this.ModifierNameTextBox.BackColor = System.Drawing.Color.SeaGreen;
            this.ModifierNameTextBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.ModifierNameTextBox.Location = new System.Drawing.Point(660, 265);
            this.ModifierNameTextBox.Size = new System.Drawing.Size(250, 20);
            this.ModifierNameTextBox.ReadOnly = true;

            this.ModifierDescriptionTextBox.BackColor = System.Drawing.Color.SeaGreen;
            this.ModifierDescriptionTextBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.ModifierDescriptionTextBox.Location = new System.Drawing.Point(660, 295);
            this.ModifierDescriptionTextBox.Multiline = true;
            this.ModifierDescriptionTextBox.Size = new System.Drawing.Size(250, 100);
            this.ModifierDescriptionTextBox.ReadOnly = true;

            Label lblAmount = new Label();
            lblAmount.Text = "Amount:";
            lblAmount.ForeColor = System.Drawing.Color.Aquamarine;
            lblAmount.Location = new System.Drawing.Point(660, 405);
            lblAmount.Size = new System.Drawing.Size(50, 20);

            this.AmountTextBox.BackColor = System.Drawing.Color.SeaGreen;
            this.AmountTextBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.AmountTextBox.Location = new System.Drawing.Point(715, 405);
            this.AmountTextBox.Size = new System.Drawing.Size(100, 20);

            this.AddModifierButton.BackColor = System.Drawing.Color.SeaGreen;
            this.AddModifierButton.Location = new System.Drawing.Point(813, 235);
            this.AddModifierButton.Size = new System.Drawing.Size(97, 23);
            this.AddModifierButton.Text = "Add Modifier";
            this.AddModifierButton.Click += new System.EventHandler(this.AddModifierButton_Click);

            this.SaveModifierButton.BackColor = System.Drawing.Color.SeaGreen;
            this.SaveModifierButton.Location = new System.Drawing.Point(825, 405);
            this.SaveModifierButton.Size = new System.Drawing.Size(85, 23);
            this.SaveModifierButton.Text = "Save Modifier";
            this.SaveModifierButton.Click += new System.EventHandler(this.SaveModifierButton_Click);

            this.DeleteModifierButton.BackColor = System.Drawing.Color.SeaGreen;
            this.DeleteModifierButton.Location = new System.Drawing.Point(825, 435);
            this.DeleteModifierButton.Size = new System.Drawing.Size(85, 23);
            this.DeleteModifierButton.Text = "Delete Modifier";
            this.DeleteModifierButton.Click += new System.EventHandler(this.DeleteModifierButton_Click);

            // Form Settings
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(918, 580);

            // Add all controls
            this.Controls.AddRange(new System.Windows.Forms.Control[] 
            {
                this.menuStrip1,
                this.EnemyListBox,
                this.EnemyNameTextBox,
                lblGroup,
                this.GroupIDDropDown,
                lblWardrobe,
                this.WardrobeTypeDropDown,
                lblWeapon,
                this.WeaponTypeDropDown,
                this.SpawnableCheckbox,
                this.SoloCheckbox,
                this.UseWardrobeOverrideCheckbox,
                this.SaveEnemyButton,
                this.CopyEnemyButton,
                this.DeleteEnemyButton,
                lblCountKeys,
                this.CountKeysGrid,
                this.AddCountKeyButton,
                this.DeleteCountKeyButton,
                lblItems,
                lblAvailableItems,
                this.ItemListBox,
                this.ItemDropDown,
                this.AddItemButton,
                this.RemoveItemButton,
                this.MoveItemUpButton,
                lblModifiers,
                this.ModifierListBox,
                this.ModifierDropdown,
                this.ModifierNameTextBox,
                this.ModifierDescriptionTextBox,
                lblAmount,
                this.AmountTextBox,
                this.AddModifierButton,
                this.SaveModifierButton,
                this.DeleteModifierButton
            });

            this.MainMenuStrip = this.menuStrip1;
            this.Name = "EnemyBrowser";
            this.Text = "Satellite Reign Enemy Browser";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CountKeysGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        
    }
}