namespace SatelliteReignModdingTools
{
    partial class TranslationsBrowser
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
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.lblTranslationCount = new System.Windows.Forms.Label();
            this.btnFilter = new System.Windows.Forms.Button();
            this.txtFilter = new System.Windows.Forms.TextBox();
            this.lblFilter = new System.Windows.Forms.Label();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.TranslationListBox = new System.Windows.Forms.ListBox();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.pnlRight = new System.Windows.Forms.Panel();
            this.btnCancelEdit = new System.Windows.Forms.Button();
            this.btnSaveTranslation = new System.Windows.Forms.Button();
            this.btnEditTranslation = new System.Windows.Forms.Button();
            this.btnNewTranslation = new System.Windows.Forms.Button();
            this.grpTranslationDetails = new System.Windows.Forms.GroupBox();
            this.txtSecondaryTranslation = new System.Windows.Forms.TextBox();
            this.lblSecondaryTranslation = new System.Windows.Forms.Label();
            this.txtPrimaryTranslation = new System.Windows.Forms.TextBox();
            this.lblPrimaryTranslation = new System.Windows.Forms.Label();
            this.txtToken = new System.Windows.Forms.TextBox();
            this.lblToken = new System.Windows.Forms.Label();
            this.txtTranslationKey = new System.Windows.Forms.TextBox();
            this.lblTranslationKey = new System.Windows.Forms.Label();
            this.pnlLeft.SuspendLayout();
            this.pnlRight.SuspendLayout();
            this.grpTranslationDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlLeft
            // 
            this.pnlLeft.Controls.Add(this.lblTranslationCount);
            this.pnlLeft.Controls.Add(this.btnFilter);
            this.pnlLeft.Controls.Add(this.txtFilter);
            this.pnlLeft.Controls.Add(this.lblFilter);
            this.pnlLeft.Controls.Add(this.cmbCategory);
            this.pnlLeft.Controls.Add(this.lblCategory);
            this.pnlLeft.Controls.Add(this.TranslationListBox);
            this.pnlLeft.Controls.Add(this.btnRefresh);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(400, 700);
            this.pnlLeft.TabIndex = 0;
            // 
            // lblTranslationCount
            // 
            this.lblTranslationCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTranslationCount.AutoSize = true;
            this.lblTranslationCount.Location = new System.Drawing.Point(12, 671);
            this.lblTranslationCount.Name = "lblTranslationCount";
            this.lblTranslationCount.Size = new System.Drawing.Size(127, 13);
            this.lblTranslationCount.TabIndex = 7;
            this.lblTranslationCount.Text = "Loaded 0 translations";
            // 
            // btnFilter
            // 
            this.btnFilter.Location = new System.Drawing.Point(308, 106);
            this.btnFilter.Name = "btnFilter";
            this.btnFilter.Size = new System.Drawing.Size(75, 23);
            this.btnFilter.TabIndex = 6;
            this.btnFilter.Text = "Filter";
            this.btnFilter.UseVisualStyleBackColor = true;
            this.btnFilter.Click += new System.EventHandler(this.btnFilter_Click);
            // 
            // txtFilter
            // 
            this.txtFilter.Location = new System.Drawing.Point(52, 108);
            this.txtFilter.Name = "txtFilter";
            this.txtFilter.Size = new System.Drawing.Size(250, 20);
            this.txtFilter.TabIndex = 5;
            this.txtFilter.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtFilter_KeyPress);
            // 
            // lblFilter
            // 
            this.lblFilter.AutoSize = true;
            this.lblFilter.Location = new System.Drawing.Point(12, 111);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(34, 13);
            this.lblFilter.TabIndex = 4;
            this.lblFilter.Text = "Filter:";
            // 
            // cmbCategory
            // 
            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(70, 75);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(232, 21);
            this.cmbCategory.TabIndex = 3;
            this.cmbCategory.SelectedIndexChanged += new System.EventHandler(this.cmbCategory_SelectedIndexChanged);
            // 
            // lblCategory
            // 
            this.lblCategory.AutoSize = true;
            this.lblCategory.Location = new System.Drawing.Point(12, 78);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(52, 13);
            this.lblCategory.TabIndex = 2;
            this.lblCategory.Text = "Category:";
            // 
            // TranslationListBox
            // 
            this.TranslationListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TranslationListBox.FormattingEnabled = true;
            this.TranslationListBox.Location = new System.Drawing.Point(12, 140);
            this.TranslationListBox.Name = "TranslationListBox";
            this.TranslationListBox.Size = new System.Drawing.Size(371, 524);
            this.TranslationListBox.TabIndex = 1;
            this.TranslationListBox.SelectedIndexChanged += new System.EventHandler(this.TranslationListBox_SelectedIndexChanged);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(12, 12);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(200, 40);
            this.btnRefresh.TabIndex = 0;
            this.btnRefresh.Text = "Refresh Translations";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // pnlRight
            // 
            this.pnlRight.Controls.Add(this.btnCancelEdit);
            this.pnlRight.Controls.Add(this.btnSaveTranslation);
            this.pnlRight.Controls.Add(this.btnEditTranslation);
            this.pnlRight.Controls.Add(this.btnNewTranslation);
            this.pnlRight.Controls.Add(this.grpTranslationDetails);
            this.pnlRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRight.Location = new System.Drawing.Point(400, 0);
            this.pnlRight.Name = "pnlRight";
            this.pnlRight.Size = new System.Drawing.Size(600, 700);
            this.pnlRight.TabIndex = 1;
            // 
            // btnCancelEdit
            // 
            this.btnCancelEdit.Location = new System.Drawing.Point(429, 12);
            this.btnCancelEdit.Name = "btnCancelEdit";
            this.btnCancelEdit.Size = new System.Drawing.Size(100, 30);
            this.btnCancelEdit.TabIndex = 4;
            this.btnCancelEdit.Text = "Cancel Edit";
            this.btnCancelEdit.UseVisualStyleBackColor = true;
            this.btnCancelEdit.Visible = false;
            this.btnCancelEdit.Click += new System.EventHandler(this.btnCancelEdit_Click);
            // 
            // btnSaveTranslation
            // 
            this.btnSaveTranslation.Location = new System.Drawing.Point(323, 12);
            this.btnSaveTranslation.Name = "btnSaveTranslation";
            this.btnSaveTranslation.Size = new System.Drawing.Size(100, 30);
            this.btnSaveTranslation.TabIndex = 3;
            this.btnSaveTranslation.Text = "Save";
            this.btnSaveTranslation.UseVisualStyleBackColor = true;
            this.btnSaveTranslation.Visible = false;
            this.btnSaveTranslation.Click += new System.EventHandler(this.btnSaveTranslation_Click);
            // 
            // btnEditTranslation
            // 
            this.btnEditTranslation.Location = new System.Drawing.Point(217, 12);
            this.btnEditTranslation.Name = "btnEditTranslation";
            this.btnEditTranslation.Size = new System.Drawing.Size(100, 30);
            this.btnEditTranslation.TabIndex = 2;
            this.btnEditTranslation.Text = "Edit";
            this.btnEditTranslation.UseVisualStyleBackColor = true;
            this.btnEditTranslation.Click += new System.EventHandler(this.btnEditTranslation_Click);
            // 
            // btnNewTranslation
            // 
            this.btnNewTranslation.Location = new System.Drawing.Point(111, 12);
            this.btnNewTranslation.Name = "btnNewTranslation";
            this.btnNewTranslation.Size = new System.Drawing.Size(100, 30);
            this.btnNewTranslation.TabIndex = 1;
            this.btnNewTranslation.Text = "New";
            this.btnNewTranslation.UseVisualStyleBackColor = true;
            this.btnNewTranslation.Click += new System.EventHandler(this.btnNewTranslation_Click);
            // 
            // grpTranslationDetails
            // 
            this.grpTranslationDetails.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTranslationDetails.Controls.Add(this.txtSecondaryTranslation);
            this.grpTranslationDetails.Controls.Add(this.lblSecondaryTranslation);
            this.grpTranslationDetails.Controls.Add(this.txtPrimaryTranslation);
            this.grpTranslationDetails.Controls.Add(this.lblPrimaryTranslation);
            this.grpTranslationDetails.Controls.Add(this.txtToken);
            this.grpTranslationDetails.Controls.Add(this.lblToken);
            this.grpTranslationDetails.Controls.Add(this.txtTranslationKey);
            this.grpTranslationDetails.Controls.Add(this.lblTranslationKey);
            this.grpTranslationDetails.Location = new System.Drawing.Point(12, 60);
            this.grpTranslationDetails.Name = "grpTranslationDetails";
            this.grpTranslationDetails.Size = new System.Drawing.Size(576, 628);
            this.grpTranslationDetails.TabIndex = 0;
            this.grpTranslationDetails.TabStop = false;
            this.grpTranslationDetails.Text = "Translation Details";
            // 
            // txtSecondaryTranslation
            // 
            this.txtSecondaryTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSecondaryTranslation.Location = new System.Drawing.Point(15, 380);
            this.txtSecondaryTranslation.Multiline = true;
            this.txtSecondaryTranslation.Name = "txtSecondaryTranslation";
            this.txtSecondaryTranslation.ReadOnly = true;
            this.txtSecondaryTranslation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtSecondaryTranslation.Size = new System.Drawing.Size(555, 230);
            this.txtSecondaryTranslation.TabIndex = 7;
            // 
            // lblSecondaryTranslation
            // 
            this.lblSecondaryTranslation.AutoSize = true;
            this.lblSecondaryTranslation.Location = new System.Drawing.Point(12, 364);
            this.lblSecondaryTranslation.Name = "lblSecondaryTranslation";
            this.lblSecondaryTranslation.Size = new System.Drawing.Size(116, 13);
            this.lblSecondaryTranslation.TabIndex = 6;
            this.lblSecondaryTranslation.Text = "Secondary Translation:";
            // 
            // txtPrimaryTranslation
            // 
            this.txtPrimaryTranslation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPrimaryTranslation.Location = new System.Drawing.Point(15, 144);
            this.txtPrimaryTranslation.Multiline = true;
            this.txtPrimaryTranslation.Name = "txtPrimaryTranslation";
            this.txtPrimaryTranslation.ReadOnly = true;
            this.txtPrimaryTranslation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtPrimaryTranslation.Size = new System.Drawing.Size(555, 200);
            this.txtPrimaryTranslation.TabIndex = 5;
            this.txtPrimaryTranslation.TextChanged += new System.EventHandler(this.txtPrimaryTranslation_TextChanged);
            // 
            // lblPrimaryTranslation
            // 
            this.lblPrimaryTranslation.AutoSize = true;
            this.lblPrimaryTranslation.Location = new System.Drawing.Point(12, 128);
            this.lblPrimaryTranslation.Name = "lblPrimaryTranslation";
            this.lblPrimaryTranslation.Size = new System.Drawing.Size(99, 13);
            this.lblPrimaryTranslation.TabIndex = 4;
            this.lblPrimaryTranslation.Text = "Primary Translation:";
            // 
            // txtToken
            // 
            this.txtToken.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtToken.Location = new System.Drawing.Point(71, 82);
            this.txtToken.Name = "txtToken";
            this.txtToken.ReadOnly = true;
            this.txtToken.Size = new System.Drawing.Size(499, 20);
            this.txtToken.TabIndex = 3;
            this.txtToken.TextChanged += new System.EventHandler(this.txtToken_TextChanged);
            // 
            // lblToken
            // 
            this.lblToken.AutoSize = true;
            this.lblToken.Location = new System.Drawing.Point(12, 85);
            this.lblToken.Name = "lblToken";
            this.lblToken.Size = new System.Drawing.Size(41, 13);
            this.lblToken.TabIndex = 2;
            this.lblToken.Text = "Token:";
            // 
            // txtTranslationKey
            // 
            this.txtTranslationKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTranslationKey.Location = new System.Drawing.Point(36, 35);
            this.txtTranslationKey.Name = "txtTranslationKey";
            this.txtTranslationKey.ReadOnly = true;
            this.txtTranslationKey.Size = new System.Drawing.Size(534, 20);
            this.txtTranslationKey.TabIndex = 1;
            this.txtTranslationKey.TextChanged += new System.EventHandler(this.txtTranslationKey_TextChanged);
            // 
            // lblTranslationKey
            // 
            this.lblTranslationKey.AutoSize = true;
            this.lblTranslationKey.Location = new System.Drawing.Point(12, 38);
            this.lblTranslationKey.Name = "lblTranslationKey";
            this.lblTranslationKey.Size = new System.Drawing.Size(28, 13);
            this.lblTranslationKey.TabIndex = 0;
            this.lblTranslationKey.Text = "Key:";
            // 
            // TranslationsBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1000, 700);
            this.Controls.Add(this.pnlRight);
            this.Controls.Add(this.pnlLeft);
            this.MinimumSize = new System.Drawing.Size(1000, 600);
            this.Name = "TranslationsBrowser";
            this.Text = "Translations Editor - Satellite Reign Modding Tools";
            this.pnlLeft.ResumeLayout(false);
            this.pnlLeft.PerformLayout();
            this.pnlRight.ResumeLayout(false);
            this.grpTranslationDetails.ResumeLayout(false);
            this.grpTranslationDetails.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlLeft;
        private System.Windows.Forms.Panel pnlRight;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.ListBox TranslationListBox;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.TextBox txtFilter;
        private System.Windows.Forms.Label lblFilter;
        private System.Windows.Forms.Label lblTranslationCount;
        private System.Windows.Forms.GroupBox grpTranslationDetails;
        private System.Windows.Forms.TextBox txtTranslationKey;
        private System.Windows.Forms.Label lblTranslationKey;
        private System.Windows.Forms.TextBox txtToken;
        private System.Windows.Forms.Label lblToken;
        private System.Windows.Forms.TextBox txtPrimaryTranslation;
        private System.Windows.Forms.Label lblPrimaryTranslation;
        private System.Windows.Forms.TextBox txtSecondaryTranslation;
        private System.Windows.Forms.Label lblSecondaryTranslation;
        private System.Windows.Forms.Button btnNewTranslation;
        private System.Windows.Forms.Button btnEditTranslation;
        private System.Windows.Forms.Button btnSaveTranslation;
        private System.Windows.Forms.Button btnCancelEdit;
    }
}