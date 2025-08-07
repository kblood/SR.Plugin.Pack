namespace SatelliteReignModdingTools
{
    partial class QuestBrowser
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
            this.QuestListBox = new System.Windows.Forms.ListBox();
            this.txtQuestID = new System.Windows.Forms.TextBox();
            this.txtQuestTitle = new System.Windows.Forms.TextBox();
            this.btnEditTitle = new System.Windows.Forms.Button();
            this.txtTitleKey = new System.Windows.Forms.TextBox();
            this.txtDistrict = new System.Windows.Forms.TextBox();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtSubQuests = new System.Windows.Forms.TextBox();
            this.txtWakeOnLocation = new System.Windows.Forms.TextBox();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.chkHidden = new System.Windows.Forms.CheckBox();
            this.chkShowDebrief = new System.Windows.Forms.CheckBox();
            this.lstDescriptions = new System.Windows.Forms.ListBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.btnFilter = new System.Windows.Forms.Button();
            this.lblQuestCount = new System.Windows.Forms.Label();
            this.cmbDistrict = new System.Windows.Forms.ComboBox();
            this.cmbTitleKey = new System.Windows.Forms.ComboBox();
            this.numWakeOnLocation = new System.Windows.Forms.NumericUpDown();
            this.numLocationID = new System.Windows.Forms.NumericUpDown();
            this.btnEditQuest = new System.Windows.Forms.Button();
            this.btnSaveQuest = new System.Windows.Forms.Button();
            this.btnNewQuest = new System.Windows.Forms.Button();
            this.btnCancelEdit = new System.Windows.Forms.Button();
            this.btnOpenTranslations = new System.Windows.Forms.Button();
            this.btnAddDescription = new System.Windows.Forms.Button();
            this.btnEditDescription = new System.Windows.Forms.Button();
            this.btnDeleteDescription = new System.Windows.Forms.Button();
            this.lstSubQuests = new System.Windows.Forms.ListBox();
            this.cmbAvailableQuests = new System.Windows.Forms.ComboBox();
            this.btnAddSubQuest = new System.Windows.Forms.Button();
            this.btnRemoveSubQuest = new System.Windows.Forms.Button();
            this.lstRewards = new System.Windows.Forms.ListBox();
            this.cmbRewardType = new System.Windows.Forms.ComboBox();
            this.cmbRewardItem = new System.Windows.Forms.ComboBox();
            this.numRewardQuantity = new System.Windows.Forms.NumericUpDown();
            this.numDropChance = new System.Windows.Forms.NumericUpDown();
            this.chkGuaranteed = new System.Windows.Forms.CheckBox();
            this.btnAddReward = new System.Windows.Forms.Button();
            this.btnEditReward = new System.Windows.Forms.Button();
            this.btnDeleteReward = new System.Windows.Forms.Button();
            this.btnBrowseItems = new System.Windows.Forms.Button();
            this.lblRewards = new System.Windows.Forms.Label();
            this.lblRewardType = new System.Windows.Forms.Label();
            this.lblRewardItem = new System.Windows.Forms.Label();
            this.lblQuantity = new System.Windows.Forms.Label();
            this.lblDropChance = new System.Windows.Forms.Label();
            this.lblQuestID = new System.Windows.Forms.Label();
            this.lblQuestTitle = new System.Windows.Forms.Label();
            this.lblTitleKey = new System.Windows.Forms.Label();
            this.lblDistrict = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblLocation = new System.Windows.Forms.Label();
            this.lblDescription = new System.Windows.Forms.Label();
            this.lblSubQuests = new System.Windows.Forms.Label();
            this.lblWakeOnLocation = new System.Windows.Forms.Label();
            this.lblDescriptions = new System.Windows.Forms.Label();
            this.lblFilter = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numWakeOnLocation)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLocationID)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRewardQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDropChance)).BeginInit();
            this.SuspendLayout();
            // 
            // QuestListBox
            // 
            this.QuestListBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.QuestListBox.FormattingEnabled = true;
            this.QuestListBox.Location = new System.Drawing.Point(12, 61);
            this.QuestListBox.Name = "QuestListBox";
            this.QuestListBox.Size = new System.Drawing.Size(300, 537);
            this.QuestListBox.TabIndex = 0;
            this.QuestListBox.SelectedIndexChanged += new System.EventHandler(this.QuestListBox_SelectedIndexChanged);
            // 
            // txtQuestID
            // 
            this.txtQuestID.Location = new System.Drawing.Point(398, 61);
            this.txtQuestID.Name = "txtQuestID";
            this.txtQuestID.ReadOnly = true;
            this.txtQuestID.Size = new System.Drawing.Size(100, 20);
            this.txtQuestID.TabIndex = 1;
            // 
            // txtQuestTitle
            // 
            this.txtQuestTitle.Location = new System.Drawing.Point(398, 107);
            this.txtQuestTitle.Name = "txtQuestTitle";
            this.txtQuestTitle.ReadOnly = true;
            this.txtQuestTitle.Size = new System.Drawing.Size(270, 20);
            this.txtQuestTitle.TabIndex = 2;
            // 
            // btnEditTitle
            // 
            this.btnEditTitle.Location = new System.Drawing.Point(674, 107);
            this.btnEditTitle.Name = "btnEditTitle";
            this.btnEditTitle.Size = new System.Drawing.Size(24, 20);
            this.btnEditTitle.TabIndex = 42;
            this.btnEditTitle.Text = "T";
            this.btnEditTitle.UseVisualStyleBackColor = true;
            this.btnEditTitle.Visible = false;
            this.btnEditTitle.Click += new System.EventHandler(this.btnEditTitle_Click);
            // 
            // txtTitleKey
            // 
            this.txtTitleKey.Location = new System.Drawing.Point(398, 153);
            this.txtTitleKey.Name = "txtTitleKey";
            this.txtTitleKey.ReadOnly = true;
            this.txtTitleKey.Size = new System.Drawing.Size(300, 20);
            this.txtTitleKey.TabIndex = 3;
            // 
            // txtDistrict
            // 
            this.txtDistrict.Location = new System.Drawing.Point(398, 199);
            this.txtDistrict.Name = "txtDistrict";
            this.txtDistrict.ReadOnly = true;
            this.txtDistrict.Size = new System.Drawing.Size(150, 20);
            this.txtDistrict.TabIndex = 4;
            // 
            // txtStatus
            // 
            this.txtStatus.Location = new System.Drawing.Point(590, 199);
            this.txtStatus.Name = "txtStatus";
            this.txtStatus.ReadOnly = true;
            this.txtStatus.Size = new System.Drawing.Size(127, 20);
            this.txtStatus.TabIndex = 5;
            // 
            // txtLocation
            // 
            this.txtLocation.Location = new System.Drawing.Point(398, 245);
            this.txtLocation.Name = "txtLocation";
            this.txtLocation.ReadOnly = true;
            this.txtLocation.Size = new System.Drawing.Size(200, 20);
            this.txtLocation.TabIndex = 6;
            // 
            // txtDescription
            // 
            this.txtDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDescription.Location = new System.Drawing.Point(398, 291);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.ReadOnly = true;
            this.txtDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDescription.Size = new System.Drawing.Size(300, 80);
            this.txtDescription.TabIndex = 7;
            // 
            // txtSubQuests
            // 
            this.txtSubQuests.Location = new System.Drawing.Point(398, 397);
            this.txtSubQuests.Multiline = true;
            this.txtSubQuests.Name = "txtSubQuests";
            this.txtSubQuests.ReadOnly = true;
            this.txtSubQuests.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSubQuests.Size = new System.Drawing.Size(300, 60);
            this.txtSubQuests.TabIndex = 8;
            // 
            // txtWakeOnLocation
            // 
            this.txtWakeOnLocation.Location = new System.Drawing.Point(414, 540);
            this.txtWakeOnLocation.Name = "txtWakeOnLocation";
            this.txtWakeOnLocation.ReadOnly = true;
            this.txtWakeOnLocation.Size = new System.Drawing.Size(221, 20);
            this.txtWakeOnLocation.TabIndex = 9;
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(70, 12);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(200, 20);
            this.txtFilter.TabIndex = 10;
            this.txtFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFilter_KeyPress);
            // 
            // chkHidden
            // 
            this.chkHidden.AutoSize = true;
            this.chkHidden.Location = new System.Drawing.Point(414, 516);
            this.chkHidden.Name = "chkHidden";
            this.chkHidden.Size = new System.Drawing.Size(60, 17);
            this.chkHidden.TabIndex = 11;
            this.chkHidden.Text = "Hidden";
            this.chkHidden.UseVisualStyleBackColor = true;
            // 
            // chkShowDebrief
            // 
            this.chkShowDebrief.AutoSize = true;
            this.chkShowDebrief.Location = new System.Drawing.Point(480, 516);
            this.chkShowDebrief.Name = "chkShowDebrief";
            this.chkShowDebrief.Size = new System.Drawing.Size(90, 17);
            this.chkShowDebrief.TabIndex = 12;
            this.chkShowDebrief.Text = "Show Debrief";
            this.chkShowDebrief.UseVisualStyleBackColor = true;
            // 
            // lstDescriptions
            // 
            this.lstDescriptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstDescriptions.FormattingEnabled = true;
            this.lstDescriptions.Location = new System.Drawing.Point(724, 61);
            this.lstDescriptions.Name = "lstDescriptions";
            this.lstDescriptions.Size = new System.Drawing.Size(350, 472);
            this.lstDescriptions.TabIndex = 13;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(318, 10);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 14;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(276, 10);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(36, 23);
            this.btnFilter.TabIndex = 15;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // lblQuestCount
            // 
            this.lblQuestCount.AutoSize = true;
            this.lblQuestCount.Location = new System.Drawing.Point(12, 42);
            this.lblQuestCount.Name = "lblQuestCount";
            this.lblQuestCount.Size = new System.Drawing.Size(86, 13);
            this.lblQuestCount.TabIndex = 16;
            this.lblQuestCount.Text = "Loaded 0 quests";
            // 
            // lblQuestID
            // 
            this.lblQuestID.AutoSize = true;
            this.lblQuestID.Location = new System.Drawing.Point(325, 64);
            this.lblQuestID.Name = "lblQuestID";
            this.lblQuestID.Size = new System.Drawing.Size(52, 13);
            this.lblQuestID.TabIndex = 17;
            this.lblQuestID.Text = "Quest ID:";
            // 
            // lblQuestTitle
            // 
            this.lblQuestTitle.AutoSize = true;
            this.lblQuestTitle.Location = new System.Drawing.Point(325, 110);
            this.lblQuestTitle.Name = "lblQuestTitle";
            this.lblQuestTitle.Size = new System.Drawing.Size(61, 13);
            this.lblQuestTitle.TabIndex = 18;
            this.lblQuestTitle.Text = "Quest Title:";
            // 
            // lblTitleKey
            // 
            this.lblTitleKey.AutoSize = true;
            this.lblTitleKey.Location = new System.Drawing.Point(325, 156);
            this.lblTitleKey.Name = "lblTitleKey";
            this.lblTitleKey.Size = new System.Drawing.Size(51, 13);
            this.lblTitleKey.TabIndex = 19;
            this.lblTitleKey.Text = "Title Key:";
            // 
            // lblDistrict
            // 
            this.lblDistrict.AutoSize = true;
            this.lblDistrict.Location = new System.Drawing.Point(325, 202);
            this.lblDistrict.Name = "lblDistrict";
            this.lblDistrict.Size = new System.Drawing.Size(42, 13);
            this.lblDistrict.TabIndex = 20;
            this.lblDistrict.Text = "District:";
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(554, 202);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(40, 13);
            this.lblStatus.TabIndex = 21;
            this.lblStatus.Text = "Status:";
            // 
            // lblLocation
            // 
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(325, 248);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(51, 13);
            this.lblLocation.TabIndex = 22;
            this.lblLocation.Text = "Location:";
            // 
            // lblDescription
            // 
            this.lblDescription.AutoSize = true;
            this.lblDescription.Location = new System.Drawing.Point(325, 275);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(63, 13);
            this.lblDescription.TabIndex = 23;
            this.lblDescription.Text = "Description:";
            // 
            // lblSubQuests
            // 
            this.lblSubQuests.AutoSize = true;
            this.lblSubQuests.Location = new System.Drawing.Point(325, 381);
            this.lblSubQuests.Name = "lblSubQuests";
            this.lblSubQuests.Size = new System.Drawing.Size(65, 13);
            this.lblSubQuests.TabIndex = 24;
            this.lblSubQuests.Text = "Sub-Quests:";
            // 
            // lblWakeOnLocation
            // 
            this.lblWakeOnLocation.AutoSize = true;
            this.lblWakeOnLocation.Location = new System.Drawing.Point(325, 543);
            this.lblWakeOnLocation.Name = "lblWakeOnLocation";
            this.lblWakeOnLocation.Size = new System.Drawing.Size(83, 13);
            this.lblWakeOnLocation.TabIndex = 25;
            this.lblWakeOnLocation.Text = "Wake Location:";
            // 
            // lblDescriptions
            // 
            this.lblDescriptions.AutoSize = true;
            this.lblDescriptions.Location = new System.Drawing.Point(721, 42);
            this.lblDescriptions.Name = "lblDescriptions";
            this.lblDescriptions.Size = new System.Drawing.Size(68, 13);
            this.lblDescriptions.TabIndex = 26;
            this.lblDescriptions.Text = "Descriptions:";
            // 
            // lblFilter
            // 
            this.lblFilter.AutoSize = true;
            this.lblFilter.Location = new System.Drawing.Point(12, 15);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(32, 13);
            this.lblFilter.TabIndex = 27;
            this.lblFilter.Text = "Filter:";
            // 
            // cmbDistrict
            // 
            this.cmbDistrict.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDistrict.FormattingEnabled = true;
            this.cmbDistrict.Location = new System.Drawing.Point(398, 199);
            this.cmbDistrict.Name = "cmbDistrict";
            this.cmbDistrict.Size = new System.Drawing.Size(150, 21);
            this.cmbDistrict.TabIndex = 28;
            this.cmbDistrict.Visible = false;
            this.cmbDistrict.SelectedIndexChanged += new System.EventHandler(this.cmbDistrict_SelectedIndexChanged);
            // 
            // cmbTitleKey
            // 
            this.cmbTitleKey.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTitleKey.FormattingEnabled = true;
            this.cmbTitleKey.Location = new System.Drawing.Point(398, 153);
            this.cmbTitleKey.Name = "cmbTitleKey";
            this.cmbTitleKey.Size = new System.Drawing.Size(300, 21);
            this.cmbTitleKey.TabIndex = 29;
            this.cmbTitleKey.Visible = false;
            this.cmbTitleKey.SelectedIndexChanged += new System.EventHandler(this.cmbTitleKey_SelectedIndexChanged);
            // 
            // numWakeOnLocation
            // 
            this.numWakeOnLocation.Location = new System.Drawing.Point(414, 541);
            this.numWakeOnLocation.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            this.numWakeOnLocation.Minimum = new decimal(new int[] { 1, 0, 0, -2147483648 });
            this.numWakeOnLocation.Name = "numWakeOnLocation";
            this.numWakeOnLocation.Size = new System.Drawing.Size(80, 20);
            this.numWakeOnLocation.TabIndex = 30;
            this.numWakeOnLocation.Value = new decimal(new int[] { 1, 0, 0, -2147483648 });
            this.numWakeOnLocation.Visible = false;
            this.numWakeOnLocation.ValueChanged += new System.EventHandler(this.numWakeOnLocation_ValueChanged);
            // 
            // numLocationID
            // 
            this.numLocationID.Location = new System.Drawing.Point(398, 245);
            this.numLocationID.Maximum = new decimal(new int[] { 500, 0, 0, 0 });
            this.numLocationID.Minimum = new decimal(new int[] { 1, 0, 0, -2147483648 });
            this.numLocationID.Name = "numLocationID";
            this.numLocationID.Size = new System.Drawing.Size(80, 20);
            this.numLocationID.TabIndex = 35;
            this.numLocationID.Value = new decimal(new int[] { 1, 0, 0, -2147483648 });
            this.numLocationID.Visible = false;
            this.numLocationID.ValueChanged += new System.EventHandler(this.numLocationID_ValueChanged);
            // 
            // btnEditQuest
            // 
            this.btnEditQuest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditQuest.Location = new System.Drawing.Point(1080, 61);
            this.btnEditQuest.Name = "btnEditQuest";
            this.btnEditQuest.Size = new System.Drawing.Size(75, 30);
            this.btnEditQuest.TabIndex = 31;
            this.btnEditQuest.Text = "Edit Quest";
            this.btnEditQuest.UseVisualStyleBackColor = true;
            this.btnEditQuest.Click += new System.EventHandler(this.btnEditQuest_Click);
            // 
            // btnSaveQuest
            // 
            this.btnSaveQuest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveQuest.Location = new System.Drawing.Point(1080, 97);
            this.btnSaveQuest.Name = "btnSaveQuest";
            this.btnSaveQuest.Size = new System.Drawing.Size(75, 30);
            this.btnSaveQuest.TabIndex = 32;
            this.btnSaveQuest.Text = "Save Quest";
            this.btnSaveQuest.UseVisualStyleBackColor = true;
            this.btnSaveQuest.Visible = false;
            this.btnSaveQuest.Click += new System.EventHandler(this.btnSaveQuest_Click);
            // 
            // btnNewQuest
            // 
            this.btnNewQuest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewQuest.Location = new System.Drawing.Point(1080, 133);
            this.btnNewQuest.Name = "btnNewQuest";
            this.btnNewQuest.Size = new System.Drawing.Size(75, 30);
            this.btnNewQuest.TabIndex = 33;
            this.btnNewQuest.Text = "New Quest";
            this.btnNewQuest.UseVisualStyleBackColor = true;
            this.btnNewQuest.Click += new System.EventHandler(this.btnNewQuest_Click);
            // 
            // btnCancelEdit
            // 
            this.btnCancelEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancelEdit.Location = new System.Drawing.Point(1080, 169);
            this.btnCancelEdit.Name = "btnCancelEdit";
            this.btnCancelEdit.Size = new System.Drawing.Size(75, 30);
            this.btnCancelEdit.TabIndex = 34;
            this.btnCancelEdit.Text = "Cancel Edit";
            this.btnCancelEdit.UseVisualStyleBackColor = true;
            this.btnCancelEdit.Visible = false;
            this.btnCancelEdit.Click += new System.EventHandler(this.btnCancelEdit_Click);
            // 
            // btnOpenTranslations
            // 
            this.btnOpenTranslations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenTranslations.Location = new System.Drawing.Point(1165, 169);
            this.btnOpenTranslations.Name = "btnOpenTranslations";
            this.btnOpenTranslations.Size = new System.Drawing.Size(120, 30);
            this.btnOpenTranslations.TabIndex = 43;
            this.btnOpenTranslations.Text = "Translations Browser";
            this.btnOpenTranslations.UseVisualStyleBackColor = true;
            this.btnOpenTranslations.Click += new System.EventHandler(this.btnOpenTranslations_Click);
            // 
            // btnAddDescription
            // 
            this.btnAddDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddDescription.Location = new System.Drawing.Point(724, 560);
            this.btnAddDescription.Name = "btnAddDescription";
            this.btnAddDescription.Size = new System.Drawing.Size(85, 25);
            this.btnAddDescription.TabIndex = 36;
            this.btnAddDescription.Text = "Add Description";
            this.btnAddDescription.UseVisualStyleBackColor = true;
            this.btnAddDescription.Visible = false;
            this.btnAddDescription.Click += new System.EventHandler(this.btnAddDescription_Click);
            // 
            // btnEditDescription
            // 
            this.btnEditDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditDescription.Location = new System.Drawing.Point(815, 560);
            this.btnEditDescription.Name = "btnEditDescription";
            this.btnEditDescription.Size = new System.Drawing.Size(85, 25);
            this.btnEditDescription.TabIndex = 37;
            this.btnEditDescription.Text = "Edit Description";
            this.btnEditDescription.UseVisualStyleBackColor = true;
            this.btnEditDescription.Visible = false;
            this.btnEditDescription.Click += new System.EventHandler(this.btnEditDescription_Click);
            // 
            // btnDeleteDescription
            // 
            this.btnDeleteDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDeleteDescription.Location = new System.Drawing.Point(906, 560);
            this.btnDeleteDescription.Name = "btnDeleteDescription";
            this.btnDeleteDescription.Size = new System.Drawing.Size(85, 25);
            this.btnDeleteDescription.TabIndex = 38;
            this.btnDeleteDescription.Text = "Delete Description";
            this.btnDeleteDescription.UseVisualStyleBackColor = true;
            this.btnDeleteDescription.Visible = false;
            this.btnDeleteDescription.Click += new System.EventHandler(this.btnDeleteDescription_Click);
            // 
            // lstSubQuests
            // 
            this.lstSubQuests.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstSubQuests.FormattingEnabled = true;
            this.lstSubQuests.Location = new System.Drawing.Point(328, 397);
            this.lstSubQuests.Name = "lstSubQuests";
            this.lstSubQuests.SelectionMode = System.Windows.Forms.SelectionMode.One;
            this.lstSubQuests.Size = new System.Drawing.Size(300, 95);
            this.lstSubQuests.TabIndex = 39;
            this.lstSubQuests.Visible = false;
            // 
            // cmbAvailableQuests
            // 
            this.cmbAvailableQuests.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAvailableQuests.FormattingEnabled = true;
            this.cmbAvailableQuests.Location = new System.Drawing.Point(328, 510);
            this.cmbAvailableQuests.Name = "cmbAvailableQuests";
            this.cmbAvailableQuests.Size = new System.Drawing.Size(200, 21);
            this.cmbAvailableQuests.TabIndex = 40;
            this.cmbAvailableQuests.Visible = false;
            // 
            // btnAddSubQuest
            // 
            this.btnAddSubQuest.Location = new System.Drawing.Point(534, 510);
            this.btnAddSubQuest.Name = "btnAddSubQuest";
            this.btnAddSubQuest.Size = new System.Drawing.Size(45, 21);
            this.btnAddSubQuest.TabIndex = 41;
            this.btnAddSubQuest.Text = "Add";
            this.btnAddSubQuest.UseVisualStyleBackColor = true;
            this.btnAddSubQuest.Visible = false;
            this.btnAddSubQuest.Click += new System.EventHandler(this.btnAddSubQuest_Click);
            // 
            // btnRemoveSubQuest
            // 
            this.btnRemoveSubQuest.Location = new System.Drawing.Point(585, 510);
            this.btnRemoveSubQuest.Name = "btnRemoveSubQuest";
            this.btnRemoveSubQuest.Size = new System.Drawing.Size(60, 21);
            this.btnRemoveSubQuest.TabIndex = 42;
            this.btnRemoveSubQuest.Text = "Remove";
            this.btnRemoveSubQuest.UseVisualStyleBackColor = true;
            this.btnRemoveSubQuest.Visible = false;
            this.btnRemoveSubQuest.Click += new System.EventHandler(this.btnRemoveSubQuest_Click);
            // 
            // lstRewards
            // 
            this.lstRewards.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right) 
            | System.Windows.Forms.AnchorStyles.Bottom)));
            this.lstRewards.FormattingEnabled = true;
            this.lstRewards.Location = new System.Drawing.Point(1080, 230);
            this.lstRewards.Name = "lstRewards";
            this.lstRewards.Size = new System.Drawing.Size(350, 160);
            this.lstRewards.TabIndex = 43;
            this.lstRewards.Visible = false;
            this.lstRewards.SelectedIndexChanged += new System.EventHandler(this.lstRewards_SelectedIndexChanged);
            // 
            // cmbRewardType
            // 
            this.cmbRewardType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRewardType.FormattingEnabled = true;
            this.cmbRewardType.Location = new System.Drawing.Point(1080, 420);
            this.cmbRewardType.Name = "cmbRewardType";
            this.cmbRewardType.Size = new System.Drawing.Size(100, 21);
            this.cmbRewardType.TabIndex = 44;
            this.cmbRewardType.Visible = false;
            this.cmbRewardType.SelectedIndexChanged += new System.EventHandler(this.cmbRewardType_SelectedIndexChanged);
            // 
            // cmbRewardItem
            // 
            this.cmbRewardItem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRewardItem.FormattingEnabled = true;
            this.cmbRewardItem.Location = new System.Drawing.Point(1186, 420);
            this.cmbRewardItem.Name = "cmbRewardItem";
            this.cmbRewardItem.Size = new System.Drawing.Size(150, 21);
            this.cmbRewardItem.TabIndex = 45;
            this.cmbRewardItem.Visible = false;
            // 
            // numRewardQuantity
            // 
            this.numRewardQuantity.Location = new System.Drawing.Point(1080, 471);
            this.numRewardQuantity.Maximum = new decimal(new int[] { 999, 0, 0, 0 });
            this.numRewardQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numRewardQuantity.Name = "numRewardQuantity";
            this.numRewardQuantity.Size = new System.Drawing.Size(60, 20);
            this.numRewardQuantity.TabIndex = 46;
            this.numRewardQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            this.numRewardQuantity.Visible = false;
            // 
            // numDropChance
            // 
            this.numDropChance.DecimalPlaces = 1;
            this.numDropChance.Location = new System.Drawing.Point(1186, 471);
            this.numDropChance.Maximum = new decimal(new int[] { 100, 0, 0, 0 });
            this.numDropChance.Minimum = new decimal(new int[] { 0, 0, 0, 0 });
            this.numDropChance.Name = "numDropChance";
            this.numDropChance.Size = new System.Drawing.Size(60, 20);
            this.numDropChance.TabIndex = 47;
            this.numDropChance.Value = new decimal(new int[] { 100, 0, 0, 0 });
            this.numDropChance.Visible = false;
            // 
            // chkGuaranteed
            // 
            this.chkGuaranteed.AutoSize = true;
            this.chkGuaranteed.Location = new System.Drawing.Point(1252, 473);
            this.chkGuaranteed.Name = "chkGuaranteed";
            this.chkGuaranteed.Size = new System.Drawing.Size(84, 17);
            this.chkGuaranteed.TabIndex = 48;
            this.chkGuaranteed.Text = "Guaranteed";
            this.chkGuaranteed.UseVisualStyleBackColor = true;
            this.chkGuaranteed.Visible = false;
            // 
            // btnAddReward
            // 
            this.btnAddReward.Location = new System.Drawing.Point(1080, 497);
            this.btnAddReward.Name = "btnAddReward";
            this.btnAddReward.Size = new System.Drawing.Size(60, 25);
            this.btnAddReward.TabIndex = 49;
            this.btnAddReward.Text = "Add";
            this.btnAddReward.UseVisualStyleBackColor = true;
            this.btnAddReward.Visible = false;
            this.btnAddReward.Click += new System.EventHandler(this.btnAddReward_Click);
            // 
            // btnEditReward
            // 
            this.btnEditReward.Location = new System.Drawing.Point(1146, 497);
            this.btnEditReward.Name = "btnEditReward";
            this.btnEditReward.Size = new System.Drawing.Size(60, 25);
            this.btnEditReward.TabIndex = 50;
            this.btnEditReward.Text = "Edit";
            this.btnEditReward.UseVisualStyleBackColor = true;
            this.btnEditReward.Visible = false;
            this.btnEditReward.Click += new System.EventHandler(this.btnEditReward_Click);
            // 
            // btnDeleteReward
            // 
            this.btnDeleteReward.Location = new System.Drawing.Point(1212, 497);
            this.btnDeleteReward.Name = "btnDeleteReward";
            this.btnDeleteReward.Size = new System.Drawing.Size(60, 25);
            this.btnDeleteReward.TabIndex = 51;
            this.btnDeleteReward.Text = "Delete";
            this.btnDeleteReward.UseVisualStyleBackColor = true;
            this.btnDeleteReward.Visible = false;
            this.btnDeleteReward.Click += new System.EventHandler(this.btnDeleteReward_Click);
            // 
            // btnBrowseItems
            // 
            this.btnBrowseItems.Location = new System.Drawing.Point(1342, 420);
            this.btnBrowseItems.Name = "btnBrowseItems";
            this.btnBrowseItems.Size = new System.Drawing.Size(65, 21);
            this.btnBrowseItems.TabIndex = 52;
            this.btnBrowseItems.Text = "Browse";
            this.btnBrowseItems.UseVisualStyleBackColor = true;
            this.btnBrowseItems.Visible = false;
            this.btnBrowseItems.Click += new System.EventHandler(this.btnBrowseItems_Click);
            // 
            // lblRewards
            // 
            this.lblRewards.AutoSize = true;
            this.lblRewards.Location = new System.Drawing.Point(1077, 210);
            this.lblRewards.Name = "lblRewards";
            this.lblRewards.Size = new System.Drawing.Size(54, 13);
            this.lblRewards.TabIndex = 53;
            this.lblRewards.Text = "Rewards:";
            this.lblRewards.Visible = false;
            // 
            // lblRewardType
            // 
            this.lblRewardType.AutoSize = true;
            this.lblRewardType.Location = new System.Drawing.Point(1077, 404);
            this.lblRewardType.Name = "lblRewardType";
            this.lblRewardType.Size = new System.Drawing.Size(34, 13);
            this.lblRewardType.TabIndex = 54;
            this.lblRewardType.Text = "Type:";
            this.lblRewardType.Visible = false;
            // 
            // lblRewardItem
            // 
            this.lblRewardItem.AutoSize = true;
            this.lblRewardItem.Location = new System.Drawing.Point(1183, 404);
            this.lblRewardItem.Name = "lblRewardItem";
            this.lblRewardItem.Size = new System.Drawing.Size(30, 13);
            this.lblRewardItem.TabIndex = 55;
            this.lblRewardItem.Text = "Item:";
            this.lblRewardItem.Visible = false;
            // 
            // lblQuantity
            // 
            this.lblQuantity.AutoSize = true;
            this.lblQuantity.Location = new System.Drawing.Point(1077, 455);
            this.lblQuantity.Name = "lblQuantity";
            this.lblQuantity.Size = new System.Drawing.Size(49, 13);
            this.lblQuantity.TabIndex = 56;
            this.lblQuantity.Text = "Quantity:";
            this.lblQuantity.Visible = false;
            // 
            // lblDropChance
            // 
            this.lblDropChance.AutoSize = true;
            this.lblDropChance.Location = new System.Drawing.Point(1183, 455);
            this.lblDropChance.Name = "lblDropChance";
            this.lblDropChance.Size = new System.Drawing.Size(70, 13);
            this.lblDropChance.TabIndex = 57;
            this.lblDropChance.Text = "Drop Chance:";
            this.lblDropChance.Visible = false;
            // 
            // QuestBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1450, 700);
            this.Controls.Add(this.lblDropChance);
            this.Controls.Add(this.lblQuantity);
            this.Controls.Add(this.lblRewardItem);
            this.Controls.Add(this.lblRewardType);
            this.Controls.Add(this.lblRewards);
            this.Controls.Add(this.btnBrowseItems);
            this.Controls.Add(this.btnDeleteReward);
            this.Controls.Add(this.btnEditReward);
            this.Controls.Add(this.btnAddReward);
            this.Controls.Add(this.chkGuaranteed);
            this.Controls.Add(this.numDropChance);
            this.Controls.Add(this.numRewardQuantity);
            this.Controls.Add(this.cmbRewardItem);
            this.Controls.Add(this.cmbRewardType);
            this.Controls.Add(this.lstRewards);
            this.Controls.Add(this.btnCancelEdit);
            this.Controls.Add(this.btnOpenTranslations);
            this.Controls.Add(this.btnAddDescription);
            this.Controls.Add(this.btnEditDescription);
            this.Controls.Add(this.btnDeleteDescription);
            this.Controls.Add(this.lstSubQuests);
            this.Controls.Add(this.cmbAvailableQuests);
            this.Controls.Add(this.btnAddSubQuest);
            this.Controls.Add(this.btnRemoveSubQuest);
            this.Controls.Add(this.btnNewQuest);
            this.Controls.Add(this.btnSaveQuest);
            this.Controls.Add(this.btnEditQuest);
            this.Controls.Add(this.numWakeOnLocation);
            this.Controls.Add(this.numLocationID);
            this.Controls.Add(this.cmbTitleKey);
            this.Controls.Add(this.cmbDistrict);
            this.Controls.Add(this.lblFilter);
            this.Controls.Add(this.lblDescriptions);
            this.Controls.Add(this.lblWakeOnLocation);
            this.Controls.Add(this.lblSubQuests);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblDistrict);
            this.Controls.Add(this.lblTitleKey);
            this.Controls.Add(this.lblQuestTitle);
            this.Controls.Add(this.lblQuestID);
            this.Controls.Add(this.lblQuestCount);
            this.Controls.Add(this.btnFilter);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.lstDescriptions);
            this.Controls.Add(this.chkShowDebrief);
            this.Controls.Add(this.chkHidden);
            this.Controls.Add(this.txtFilter);
            this.Controls.Add(this.txtWakeOnLocation);
            this.Controls.Add(this.txtSubQuests);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.txtLocation);
            this.Controls.Add(this.txtStatus);
            this.Controls.Add(this.txtDistrict);
            this.Controls.Add(this.txtTitleKey);
            this.Controls.Add(this.txtQuestTitle);
            this.Controls.Add(this.btnEditTitle);
            this.Controls.Add(this.txtQuestID);
            this.Controls.Add(this.QuestListBox);
            this.Name = "QuestBrowser";
            this.Text = "Quest Editor - Satellite Reign Modding Tools";
            ((System.ComponentModel.ISupportInitialize)(this.numWakeOnLocation)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLocationID)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numRewardQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numDropChance)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox QuestListBox;
        private System.Windows.Forms.TextBox txtQuestID;
        private System.Windows.Forms.TextBox txtQuestTitle;
        private System.Windows.Forms.Button btnEditTitle;
        private System.Windows.Forms.TextBox txtTitleKey;
        private System.Windows.Forms.TextBox txtDistrict;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtSubQuests;
        private System.Windows.Forms.TextBox txtWakeOnLocation;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.CheckBox chkHidden;
        private System.Windows.Forms.CheckBox chkShowDebrief;
        private System.Windows.Forms.ListBox lstDescriptions;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Label lblQuestCount;
        private System.Windows.Forms.Label lblQuestID;
        private System.Windows.Forms.Label lblQuestTitle;
        private System.Windows.Forms.Label lblTitleKey;
        private System.Windows.Forms.Label lblDistrict;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblLocation;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.Label lblSubQuests;
        private System.Windows.Forms.Label lblWakeOnLocation;
        private System.Windows.Forms.Label lblDescriptions;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.ComboBox cmbDistrict;
        private System.Windows.Forms.ComboBox cmbTitleKey;
        private System.Windows.Forms.NumericUpDown numWakeOnLocation;
        private System.Windows.Forms.NumericUpDown numLocationID;
        private System.Windows.Forms.Button btnEditQuest;
        private System.Windows.Forms.Button btnSaveQuest;
        private System.Windows.Forms.Button btnNewQuest;
        private System.Windows.Forms.Button btnCancelEdit;
        private System.Windows.Forms.Button btnOpenTranslations;
        private System.Windows.Forms.Button btnAddDescription;
        private System.Windows.Forms.Button btnEditDescription;
        private System.Windows.Forms.Button btnDeleteDescription;
        private System.Windows.Forms.ListBox lstSubQuests;
        private System.Windows.Forms.ComboBox cmbAvailableQuests;
        private System.Windows.Forms.Button btnAddSubQuest;
        private System.Windows.Forms.Button btnRemoveSubQuest;
        private System.Windows.Forms.ListBox lstRewards;
        private System.Windows.Forms.ComboBox cmbRewardType;
        private System.Windows.Forms.ComboBox cmbRewardItem;
        private System.Windows.Forms.NumericUpDown numRewardQuantity;
        private System.Windows.Forms.NumericUpDown numDropChance;
        private System.Windows.Forms.CheckBox chkGuaranteed;
        private System.Windows.Forms.Button btnAddReward;
        private System.Windows.Forms.Button btnEditReward;
        private System.Windows.Forms.Button btnDeleteReward;
        private System.Windows.Forms.Button btnBrowseItems;
        private System.Windows.Forms.Label lblRewards;
        private System.Windows.Forms.Label lblRewardType;
        private System.Windows.Forms.Label lblRewardItem;
        private System.Windows.Forms.Label lblQuantity;
        private System.Windows.Forms.Label lblDropChance;
    }
}