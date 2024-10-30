namespace SatelliteReignModdingTools
{
    partial class ItemBrowser
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.DeleteItemButton = new System.Windows.Forms.Button();
            this.CopyItemButton = new System.Windows.Forms.Button();
            this.ItemListBox = new System.Windows.Forms.ListBox();
            this.ItemSlotTypeDropDown = new System.Windows.Forms.ComboBox();
            this.WeaponTypeDropDown = new System.Windows.Forms.ComboBox();
            this.GearSubTypeDropDown = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.filesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LoadTranslationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTranslationsFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveTranslationsAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.quitToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.languageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.englishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.czechToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.frenchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.germanToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.italianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.russianToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.spanishToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ItemNameTextBox = new System.Windows.Forms.TextBox();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.ItemIconImageBox = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AbilityListBox = new System.Windows.Forms.ListBox();
            this.ModifierListBox = new System.Windows.Forms.ListBox();
            this.ExtraDescriptionTextBox = new System.Windows.Forms.TextBox();
            this.ExtraNameTextBox = new System.Windows.Forms.TextBox();
            this.AbilityDropdown = new System.Windows.Forms.ComboBox();
            this.ModifierDropdown = new System.Windows.Forms.ComboBox();
            this.AddAbility = new System.Windows.Forms.Button();
            this.AddModiferButton = new System.Windows.Forms.Button();
            this.DeleteAbilityButton = new System.Windows.Forms.Button();
            this.DeleteModifierButton = new System.Windows.Forms.Button();
            this.SaveModifierButton = new System.Windows.Forms.Button();
            this.SaveItemButton = new System.Windows.Forms.Button();
            this.AmountTextBox = new System.Windows.Forms.TextBox();
            this.AmountLabel = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ItemIconImageBox)).BeginInit();
            this.SuspendLayout();
            // 
            // DeleteItemButton
            // 
            this.DeleteItemButton.BackColor = System.Drawing.Color.SeaGreen;
            this.DeleteItemButton.Location = new System.Drawing.Point(330, 74);
            this.DeleteItemButton.Name = "DeleteItemButton";
            this.DeleteItemButton.Size = new System.Drawing.Size(79, 23);
            this.DeleteItemButton.TabIndex = 0;
            this.DeleteItemButton.Text = "Delete Item";
            this.DeleteItemButton.UseVisualStyleBackColor = false;
            this.DeleteItemButton.Click += new System.EventHandler(this.DeleteItemButton_Click);
            // 
            // CopyItemButton
            // 
            this.CopyItemButton.BackColor = System.Drawing.Color.SeaGreen;
            this.CopyItemButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.CopyItemButton.Location = new System.Drawing.Point(330, 45);
            this.CopyItemButton.Name = "CopyItemButton";
            this.CopyItemButton.Size = new System.Drawing.Size(79, 23);
            this.CopyItemButton.TabIndex = 1;
            this.CopyItemButton.Text = "Copy Item";
            this.CopyItemButton.UseVisualStyleBackColor = false;
            this.CopyItemButton.Click += new System.EventHandler(this.CopyItemButton_Click);
            // 
            // ItemListBox
            // 
            this.ItemListBox.BackColor = System.Drawing.Color.SeaGreen;
            this.ItemListBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.ItemListBox.FormattingEnabled = true;
            this.ItemListBox.Location = new System.Drawing.Point(12, 45);
            this.ItemListBox.Name = "ItemListBox";
            this.ItemListBox.Size = new System.Drawing.Size(312, 420);
            this.ItemListBox.TabIndex = 2;
            this.ItemListBox.SelectedIndexChanged += new System.EventHandler(this.ItemListBox_SelectedIndexChanged);
            // 
            // ItemSlotTypeDropDown
            // 
            this.ItemSlotTypeDropDown.BackColor = System.Drawing.Color.DimGray;
            this.ItemSlotTypeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ItemSlotTypeDropDown.ForeColor = System.Drawing.Color.Aquamarine;
            this.ItemSlotTypeDropDown.FormattingEnabled = true;
            this.ItemSlotTypeDropDown.Location = new System.Drawing.Point(330, 129);
            this.ItemSlotTypeDropDown.Name = "ItemSlotTypeDropDown";
            this.ItemSlotTypeDropDown.Size = new System.Drawing.Size(147, 21);
            this.ItemSlotTypeDropDown.TabIndex = 3;
            // 
            // WeaponTypeDropDown
            // 
            this.WeaponTypeDropDown.BackColor = System.Drawing.Color.DimGray;
            this.WeaponTypeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.WeaponTypeDropDown.ForeColor = System.Drawing.Color.Aquamarine;
            this.WeaponTypeDropDown.FormattingEnabled = true;
            this.WeaponTypeDropDown.Location = new System.Drawing.Point(330, 207);
            this.WeaponTypeDropDown.Name = "WeaponTypeDropDown";
            this.WeaponTypeDropDown.Size = new System.Drawing.Size(147, 21);
            this.WeaponTypeDropDown.TabIndex = 4;
            // 
            // GearSubTypeDropDown
            // 
            this.GearSubTypeDropDown.BackColor = System.Drawing.Color.DimGray;
            this.GearSubTypeDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GearSubTypeDropDown.ForeColor = System.Drawing.Color.Aquamarine;
            this.GearSubTypeDropDown.FormattingEnabled = true;
            this.GearSubTypeDropDown.Location = new System.Drawing.Point(330, 168);
            this.GearSubTypeDropDown.Name = "GearSubTypeDropDown";
            this.GearSubTypeDropDown.Size = new System.Drawing.Size(147, 21);
            this.GearSubTypeDropDown.TabIndex = 5;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.filesToolStripMenuItem,
            this.languageToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(918, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // filesToolStripMenuItem
            // 
            this.filesToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.filesToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadAllToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.loadFileToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.LoadTranslationToolStripMenuItem,
            this.saveTranslationsFileToolStripMenuItem,
            this.saveTranslationsAsToolStripMenuItem,
            this.quitToolStripMenuItem1});
            this.filesToolStripMenuItem.ForeColor = System.Drawing.Color.Aquamarine;
            this.filesToolStripMenuItem.Name = "filesToolStripMenuItem";
            this.filesToolStripMenuItem.Size = new System.Drawing.Size(42, 20);
            this.filesToolStripMenuItem.Text = "Files";
            // 
            // loadAllToolStripMenuItem
            // 
            this.loadAllToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.loadAllToolStripMenuItem.Name = "loadAllToolStripMenuItem";
            this.loadAllToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.loadAllToolStripMenuItem.Text = "Reload All";
            this.loadAllToolStripMenuItem.Click += new System.EventHandler(this.loadAllToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.saveToolStripMenuItem.Text = "Save All";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // loadFileToolStripMenuItem
            // 
            this.loadFileToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.loadFileToolStripMenuItem.Name = "loadFileToolStripMenuItem";
            this.loadFileToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.loadFileToolStripMenuItem.Text = "Load Items File";
            this.loadFileToolStripMenuItem.Click += new System.EventHandler(this.loadFileToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.saveAsToolStripMenuItem.Text = "Save Items As";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // LoadTranslationToolStripMenuItem
            // 
            this.LoadTranslationToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.LoadTranslationToolStripMenuItem.Name = "LoadTranslationToolStripMenuItem";
            this.LoadTranslationToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.LoadTranslationToolStripMenuItem.Text = "Load Translations File";
            this.LoadTranslationToolStripMenuItem.Click += new System.EventHandler(this.LoadTranslationToolStripMenuItem_Click);
            // 
            // saveTranslationsFileToolStripMenuItem
            // 
            this.saveTranslationsFileToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.saveTranslationsFileToolStripMenuItem.Name = "saveTranslationsFileToolStripMenuItem";
            this.saveTranslationsFileToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.saveTranslationsFileToolStripMenuItem.Text = "Save Translations";
            // 
            // saveTranslationsAsToolStripMenuItem
            // 
            this.saveTranslationsAsToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.saveTranslationsAsToolStripMenuItem.Name = "saveTranslationsAsToolStripMenuItem";
            this.saveTranslationsAsToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.saveTranslationsAsToolStripMenuItem.Text = "Save Translations As";
            // 
            // quitToolStripMenuItem1
            // 
            this.quitToolStripMenuItem1.BackColor = System.Drawing.Color.DimGray;
            this.quitToolStripMenuItem1.Name = "quitToolStripMenuItem1";
            this.quitToolStripMenuItem1.Size = new System.Drawing.Size(187, 22);
            this.quitToolStripMenuItem1.Text = "Quit";
            // 
            // languageToolStripMenuItem
            // 
            this.languageToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.languageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.englishToolStripMenuItem,
            this.czechToolStripMenuItem,
            this.frenchToolStripMenuItem,
            this.germanToolStripMenuItem,
            this.italianToolStripMenuItem,
            this.russianToolStripMenuItem,
            this.spanishToolStripMenuItem});
            this.languageToolStripMenuItem.ForeColor = System.Drawing.Color.Aquamarine;
            this.languageToolStripMenuItem.Name = "languageToolStripMenuItem";
            this.languageToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.languageToolStripMenuItem.Text = "Language";
            // 
            // englishToolStripMenuItem
            // 
            this.englishToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.englishToolStripMenuItem.Name = "englishToolStripMenuItem";
            this.englishToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.englishToolStripMenuItem.Text = "English";
            this.englishToolStripMenuItem.Click += new System.EventHandler(this.englishToolStripMenuItem_Click);
            // 
            // czechToolStripMenuItem
            // 
            this.czechToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.czechToolStripMenuItem.Name = "czechToolStripMenuItem";
            this.czechToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.czechToolStripMenuItem.Text = "Czech";
            this.czechToolStripMenuItem.Click += new System.EventHandler(this.czechToolStripMenuItem_Click);
            // 
            // frenchToolStripMenuItem
            // 
            this.frenchToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.frenchToolStripMenuItem.Name = "frenchToolStripMenuItem";
            this.frenchToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.frenchToolStripMenuItem.Text = "French";
            this.frenchToolStripMenuItem.Click += new System.EventHandler(this.frenchToolStripMenuItem_Click);
            // 
            // germanToolStripMenuItem
            // 
            this.germanToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.germanToolStripMenuItem.Name = "germanToolStripMenuItem";
            this.germanToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.germanToolStripMenuItem.Text = "German";
            this.germanToolStripMenuItem.Click += new System.EventHandler(this.germanToolStripMenuItem_Click);
            // 
            // italianToolStripMenuItem
            // 
            this.italianToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.italianToolStripMenuItem.Name = "italianToolStripMenuItem";
            this.italianToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.italianToolStripMenuItem.Text = "Italian";
            this.italianToolStripMenuItem.Click += new System.EventHandler(this.italianToolStripMenuItem_Click);
            // 
            // russianToolStripMenuItem
            // 
            this.russianToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.russianToolStripMenuItem.Name = "russianToolStripMenuItem";
            this.russianToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.russianToolStripMenuItem.Text = "Russian";
            this.russianToolStripMenuItem.Click += new System.EventHandler(this.russianToolStripMenuItem_Click);
            // 
            // spanishToolStripMenuItem
            // 
            this.spanishToolStripMenuItem.BackColor = System.Drawing.Color.DimGray;
            this.spanishToolStripMenuItem.Name = "spanishToolStripMenuItem";
            this.spanishToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.spanishToolStripMenuItem.Text = "Spanish";
            this.spanishToolStripMenuItem.Click += new System.EventHandler(this.spanishToolStripMenuItem_Click);
            // 
            // ItemNameTextBox
            // 
            this.ItemNameTextBox.BackColor = System.Drawing.Color.SeaGreen;
            this.ItemNameTextBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.ItemNameTextBox.Location = new System.Drawing.Point(483, 45);
            this.ItemNameTextBox.Name = "ItemNameTextBox";
            this.ItemNameTextBox.Size = new System.Drawing.Size(338, 20);
            this.ItemNameTextBox.TabIndex = 7;
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.BackColor = System.Drawing.Color.SeaGreen;
            this.DescriptionTextBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.DescriptionTextBox.Location = new System.Drawing.Point(483, 71);
            this.DescriptionTextBox.Multiline = true;
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.Size = new System.Drawing.Size(338, 171);
            this.DescriptionTextBox.TabIndex = 8;
            // 
            // ItemIconImageBox
            // 
            this.ItemIconImageBox.BackColor = System.Drawing.Color.Black;
            this.ItemIconImageBox.Location = new System.Drawing.Point(827, 45);
            this.ItemIconImageBox.Name = "ItemIconImageBox";
            this.ItemIconImageBox.Size = new System.Drawing.Size(78, 68);
            this.ItemIconImageBox.TabIndex = 9;
            this.ItemIconImageBox.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // AbilityListBox
            // 
            this.AbilityListBox.BackColor = System.Drawing.Color.SeaGreen;
            this.AbilityListBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.AbilityListBox.FormattingEnabled = true;
            this.AbilityListBox.Location = new System.Drawing.Point(330, 277);
            this.AbilityListBox.Name = "AbilityListBox";
            this.AbilityListBox.Size = new System.Drawing.Size(193, 160);
            this.AbilityListBox.TabIndex = 11;
            this.AbilityListBox.SelectedIndexChanged += new System.EventHandler(this.AbilityListBox_SelectedIndexChanged);
            // 
            // ModifierListBox
            // 
            this.ModifierListBox.BackColor = System.Drawing.Color.SeaGreen;
            this.ModifierListBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.ModifierListBox.FormattingEnabled = true;
            this.ModifierListBox.Location = new System.Drawing.Point(529, 277);
            this.ModifierListBox.Name = "ModifierListBox";
            this.ModifierListBox.Size = new System.Drawing.Size(189, 160);
            this.ModifierListBox.TabIndex = 12;
            this.ModifierListBox.SelectedIndexChanged += new System.EventHandler(this.ModifierListBox_SelectedIndexChanged);
            // 
            // ExtraDescriptionTextBox
            // 
            this.ExtraDescriptionTextBox.BackColor = System.Drawing.Color.SeaGreen;
            this.ExtraDescriptionTextBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.ExtraDescriptionTextBox.Location = new System.Drawing.Point(724, 277);
            this.ExtraDescriptionTextBox.Multiline = true;
            this.ExtraDescriptionTextBox.Name = "ExtraDescriptionTextBox";
            this.ExtraDescriptionTextBox.Size = new System.Drawing.Size(172, 94);
            this.ExtraDescriptionTextBox.TabIndex = 13;
            // 
            // ExtraNameTextBox
            // 
            this.ExtraNameTextBox.BackColor = System.Drawing.Color.SeaGreen;
            this.ExtraNameTextBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.ExtraNameTextBox.Location = new System.Drawing.Point(724, 251);
            this.ExtraNameTextBox.Name = "ExtraNameTextBox";
            this.ExtraNameTextBox.Size = new System.Drawing.Size(172, 20);
            this.ExtraNameTextBox.TabIndex = 14;
            // 
            // AbilityDropdown
            // 
            this.AbilityDropdown.BackColor = System.Drawing.Color.DimGray;
            this.AbilityDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.AbilityDropdown.ForeColor = System.Drawing.Color.Aquamarine;
            this.AbilityDropdown.FormattingEnabled = true;
            this.AbilityDropdown.Location = new System.Drawing.Point(330, 252);
            this.AbilityDropdown.Name = "AbilityDropdown";
            this.AbilityDropdown.Size = new System.Drawing.Size(147, 21);
            this.AbilityDropdown.TabIndex = 15;
            // 
            // ModifierDropdown
            // 
            this.ModifierDropdown.BackColor = System.Drawing.Color.DimGray;
            this.ModifierDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ModifierDropdown.ForeColor = System.Drawing.Color.Aquamarine;
            this.ModifierDropdown.FormattingEnabled = true;
            this.ModifierDropdown.Location = new System.Drawing.Point(529, 252);
            this.ModifierDropdown.Name = "ModifierDropdown";
            this.ModifierDropdown.Size = new System.Drawing.Size(147, 21);
            this.ModifierDropdown.TabIndex = 16;
            // 
            // AddAbility
            // 
            this.AddAbility.BackColor = System.Drawing.Color.SeaGreen;
            this.AddAbility.ForeColor = System.Drawing.SystemColors.ControlText;
            this.AddAbility.Location = new System.Drawing.Point(487, 252);
            this.AddAbility.Name = "AddAbility";
            this.AddAbility.Size = new System.Drawing.Size(36, 23);
            this.AddAbility.TabIndex = 17;
            this.AddAbility.Text = "Add";
            this.AddAbility.UseVisualStyleBackColor = false;
            this.AddAbility.Click += new System.EventHandler(this.AddAbility_Click);
            // 
            // AddModiferButton
            // 
            this.AddModiferButton.BackColor = System.Drawing.Color.SeaGreen;
            this.AddModiferButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.AddModiferButton.Location = new System.Drawing.Point(682, 252);
            this.AddModiferButton.Name = "AddModiferButton";
            this.AddModiferButton.Size = new System.Drawing.Size(36, 23);
            this.AddModiferButton.TabIndex = 18;
            this.AddModiferButton.Text = "Add";
            this.AddModiferButton.UseVisualStyleBackColor = false;
            this.AddModiferButton.Click += new System.EventHandler(this.AddModiferButton_Click);
            // 
            // DeleteAbilityButton
            // 
            this.DeleteAbilityButton.BackColor = System.Drawing.Color.SeaGreen;
            this.DeleteAbilityButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.DeleteAbilityButton.Location = new System.Drawing.Point(435, 440);
            this.DeleteAbilityButton.Name = "DeleteAbilityButton";
            this.DeleteAbilityButton.Size = new System.Drawing.Size(88, 23);
            this.DeleteAbilityButton.TabIndex = 19;
            this.DeleteAbilityButton.Text = "Delete Ability";
            this.DeleteAbilityButton.UseVisualStyleBackColor = false;
            this.DeleteAbilityButton.Click += new System.EventHandler(this.DeleteAbilityButton_Click);
            // 
            // DeleteModifierButton
            // 
            this.DeleteModifierButton.BackColor = System.Drawing.Color.SeaGreen;
            this.DeleteModifierButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.DeleteModifierButton.Location = new System.Drawing.Point(626, 440);
            this.DeleteModifierButton.Name = "DeleteModifierButton";
            this.DeleteModifierButton.Size = new System.Drawing.Size(92, 23);
            this.DeleteModifierButton.TabIndex = 20;
            this.DeleteModifierButton.Text = "Delete Modifier";
            this.DeleteModifierButton.UseVisualStyleBackColor = false;
            this.DeleteModifierButton.Click += new System.EventHandler(this.DeleteModifierButton_Click);
            // 
            // SaveModifierButton
            // 
            this.SaveModifierButton.BackColor = System.Drawing.Color.SeaGreen;
            this.SaveModifierButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.SaveModifierButton.Location = new System.Drawing.Point(529, 440);
            this.SaveModifierButton.Name = "SaveModifierButton";
            this.SaveModifierButton.Size = new System.Drawing.Size(88, 23);
            this.SaveModifierButton.TabIndex = 22;
            this.SaveModifierButton.Text = "Save Modifier";
            this.SaveModifierButton.UseVisualStyleBackColor = false;
            this.SaveModifierButton.Click += new System.EventHandler(this.SaveModifierButton_Click);
            // 
            // SaveItemButton
            // 
            this.SaveItemButton.BackColor = System.Drawing.Color.SeaGreen;
            this.SaveItemButton.Location = new System.Drawing.Point(330, 103);
            this.SaveItemButton.Name = "SaveItemButton";
            this.SaveItemButton.Size = new System.Drawing.Size(79, 23);
            this.SaveItemButton.TabIndex = 23;
            this.SaveItemButton.Text = "Save Item";
            this.SaveItemButton.UseVisualStyleBackColor = false;
            this.SaveItemButton.Click += new System.EventHandler(this.SaveItemButton_Click);
            // 
            // AmountTextBox
            // 
            this.AmountTextBox.BackColor = System.Drawing.Color.SeaGreen;
            this.AmountTextBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.AmountTextBox.Location = new System.Drawing.Point(793, 377);
            this.AmountTextBox.Name = "AmountTextBox";
            this.AmountTextBox.Size = new System.Drawing.Size(103, 20);
            this.AmountTextBox.TabIndex = 24;
            this.AmountTextBox.TextChanged += new System.EventHandler(this.AmountTextBox_TextChanged);
            // 
            // AmountLabel
            // 
            this.AmountLabel.AutoSize = true;
            this.AmountLabel.ForeColor = System.Drawing.Color.Aquamarine;
            this.AmountLabel.Location = new System.Drawing.Point(733, 380);
            this.AmountLabel.Name = "AmountLabel";
            this.AmountLabel.Size = new System.Drawing.Size(43, 13);
            this.AmountLabel.TabIndex = 27;
            this.AmountLabel.Text = "Amount";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // ItemBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.WindowText;
            this.ClientSize = new System.Drawing.Size(918, 478);
            this.Controls.Add(this.AmountLabel);
            this.Controls.Add(this.AmountTextBox);
            this.Controls.Add(this.SaveItemButton);
            this.Controls.Add(this.SaveModifierButton);
            this.Controls.Add(this.DeleteModifierButton);
            this.Controls.Add(this.DeleteAbilityButton);
            this.Controls.Add(this.AddModiferButton);
            this.Controls.Add(this.AddAbility);
            this.Controls.Add(this.ModifierDropdown);
            this.Controls.Add(this.AbilityDropdown);
            this.Controls.Add(this.ExtraNameTextBox);
            this.Controls.Add(this.ExtraDescriptionTextBox);
            this.Controls.Add(this.ModifierListBox);
            this.Controls.Add(this.AbilityListBox);
            this.Controls.Add(this.ItemIconImageBox);
            this.Controls.Add(this.DescriptionTextBox);
            this.Controls.Add(this.ItemNameTextBox);
            this.Controls.Add(this.GearSubTypeDropDown);
            this.Controls.Add(this.WeaponTypeDropDown);
            this.Controls.Add(this.ItemSlotTypeDropDown);
            this.Controls.Add(this.ItemListBox);
            this.Controls.Add(this.CopyItemButton);
            this.Controls.Add(this.DeleteItemButton);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "ItemBrowser";
            this.Text = "Satellite Reign Item Browser";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ItemIconImageBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button DeleteItemButton;
        private System.Windows.Forms.Button CopyItemButton;
        private System.Windows.Forms.ListBox ItemListBox;
        private System.Windows.Forms.ComboBox ItemSlotTypeDropDown;
        private System.Windows.Forms.ComboBox WeaponTypeDropDown;
        private System.Windows.Forms.ComboBox GearSubTypeDropDown;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem filesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem languageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem LoadTranslationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem englishToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem czechToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem frenchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem germanToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem italianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem russianToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem spanishToolStripMenuItem;
        private System.Windows.Forms.TextBox ItemNameTextBox;
        private System.Windows.Forms.TextBox DescriptionTextBox;
        private System.Windows.Forms.PictureBox ItemIconImageBox;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ListBox AbilityListBox;
        private System.Windows.Forms.ListBox ModifierListBox;
        private System.Windows.Forms.TextBox ExtraDescriptionTextBox;
        private System.Windows.Forms.TextBox ExtraNameTextBox;
        private System.Windows.Forms.ComboBox AbilityDropdown;
        private System.Windows.Forms.ComboBox ModifierDropdown;
        private System.Windows.Forms.Button AddAbility;
        private System.Windows.Forms.Button AddModiferButton;
        private System.Windows.Forms.Button DeleteAbilityButton;
        private System.Windows.Forms.Button DeleteModifierButton;
        private System.Windows.Forms.Button SaveModifierButton;
        private System.Windows.Forms.Button SaveItemButton;
        private System.Windows.Forms.TextBox AmountTextBox;
        private System.Windows.Forms.Label AmountLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem saveTranslationsFileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveTranslationsAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem quitToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadAllToolStripMenuItem;
    }
}

