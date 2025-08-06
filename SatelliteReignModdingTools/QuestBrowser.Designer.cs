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
            this.SuspendLayout();
            // 
            // QuestListBox
            // 
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
            this.txtQuestTitle.Size = new System.Drawing.Size(300, 20);
            this.txtQuestTitle.TabIndex = 2;
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
            this.txtWakeOnLocation.Location = new System.Drawing.Point(414, 483);
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
            this.lblWakeOnLocation.Location = new System.Drawing.Point(325, 486);
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
            // QuestBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1084, 611);
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
            this.Controls.Add(this.txtQuestID);
            this.Controls.Add(this.QuestListBox);
            this.Name = "QuestBrowser";
            this.Text = "Quest Browser - Satellite Reign Modding Tools";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox QuestListBox;
        private System.Windows.Forms.TextBox txtQuestID;
        private System.Windows.Forms.TextBox txtQuestTitle;
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
    }
}