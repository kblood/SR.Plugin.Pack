namespace SatelliteReignModdingTools
{
    partial class TranslationEditDialog
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
            this.lblTranslationKey = new System.Windows.Forms.Label();
            this.txtTranslationKey = new System.Windows.Forms.TextBox();
            this.lblTranslationText = new System.Windows.Forms.Label();
            this.txtTranslationText = new System.Windows.Forms.TextBox();
            this.lblExistingKeys = new System.Windows.Forms.Label();
            this.cmbExistingKeys = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOpenTranslationsEditor = new System.Windows.Forms.Button();
            this.chkIsNew = new System.Windows.Forms.CheckBox();
            this.chkHasBeenSeen = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblTranslationKey
            // 
            this.lblTranslationKey.AutoSize = true;
            this.lblTranslationKey.Location = new System.Drawing.Point(12, 15);
            this.lblTranslationKey.Name = "lblTranslationKey";
            this.lblTranslationKey.Size = new System.Drawing.Size(86, 13);
            this.lblTranslationKey.TabIndex = 0;
            this.lblTranslationKey.Text = "Translation Key:";
            // 
            // txtTranslationKey
            // 
            this.txtTranslationKey.Location = new System.Drawing.Point(15, 31);
            this.txtTranslationKey.Name = "txtTranslationKey";
            this.txtTranslationKey.Size = new System.Drawing.Size(400, 20);
            this.txtTranslationKey.TabIndex = 1;
            // 
            // lblTranslationText
            // 
            this.lblTranslationText.AutoSize = true;
            this.lblTranslationText.Location = new System.Drawing.Point(12, 65);
            this.lblTranslationText.Name = "lblTranslationText";
            this.lblTranslationText.Size = new System.Drawing.Size(84, 13);
            this.lblTranslationText.TabIndex = 2;
            this.lblTranslationText.Text = "Translation Text:";
            // 
            // txtTranslationText
            // 
            this.txtTranslationText.Location = new System.Drawing.Point(15, 81);
            this.txtTranslationText.Multiline = true;
            this.txtTranslationText.Name = "txtTranslationText";
            this.txtTranslationText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtTranslationText.Size = new System.Drawing.Size(400, 80);
            this.txtTranslationText.TabIndex = 3;
            // 
            // lblExistingKeys
            // 
            this.lblExistingKeys.AutoSize = true;
            this.lblExistingKeys.Location = new System.Drawing.Point(12, 175);
            this.lblExistingKeys.Name = "lblExistingKeys";
            this.lblExistingKeys.Size = new System.Drawing.Size(142, 13);
            this.lblExistingKeys.TabIndex = 4;
            this.lblExistingKeys.Text = "Or select existing key:";
            // 
            // cmbExistingKeys
            // 
            this.cmbExistingKeys.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbExistingKeys.FormattingEnabled = true;
            this.cmbExistingKeys.Location = new System.Drawing.Point(15, 191);
            this.cmbExistingKeys.Name = "cmbExistingKeys";
            this.cmbExistingKeys.Size = new System.Drawing.Size(400, 21);
            this.cmbExistingKeys.TabIndex = 5;
            this.cmbExistingKeys.SelectedIndexChanged += new System.EventHandler(this.cmbExistingKeys_SelectedIndexChanged);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(259, 280);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(340, 280);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOpenTranslationsEditor
            // 
            this.btnOpenTranslationsEditor.Location = new System.Drawing.Point(15, 280);
            this.btnOpenTranslationsEditor.Name = "btnOpenTranslationsEditor";
            this.btnOpenTranslationsEditor.Size = new System.Drawing.Size(140, 23);
            this.btnOpenTranslationsEditor.TabIndex = 7;
            this.btnOpenTranslationsEditor.Text = "Open Translations Editor";
            this.btnOpenTranslationsEditor.UseVisualStyleBackColor = true;
            this.btnOpenTranslationsEditor.Click += new System.EventHandler(this.btnOpenTranslationsEditor_Click);
            // 
            // chkIsNew
            // 
            this.chkIsNew.AutoSize = true;
            this.chkIsNew.Location = new System.Drawing.Point(15, 230);
            this.chkIsNew.Name = "chkIsNew";
            this.chkIsNew.Size = new System.Drawing.Size(57, 17);
            this.chkIsNew.TabIndex = 6;
            this.chkIsNew.Text = "Is New";
            this.chkIsNew.UseVisualStyleBackColor = true;
            // 
            // chkHasBeenSeen
            // 
            this.chkHasBeenSeen.AutoSize = true;
            this.chkHasBeenSeen.Location = new System.Drawing.Point(90, 230);
            this.chkHasBeenSeen.Name = "chkHasBeenSeen";
            this.chkHasBeenSeen.Size = new System.Drawing.Size(100, 17);
            this.chkHasBeenSeen.TabIndex = 6;
            this.chkHasBeenSeen.Text = "Has Been Seen";
            this.chkHasBeenSeen.UseVisualStyleBackColor = true;
            // 
            // TranslationEditDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(434, 321);
            this.Controls.Add(this.chkHasBeenSeen);
            this.Controls.Add(this.chkIsNew);
            this.Controls.Add(this.btnOpenTranslationsEditor);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.cmbExistingKeys);
            this.Controls.Add(this.lblExistingKeys);
            this.Controls.Add(this.txtTranslationText);
            this.Controls.Add(this.lblTranslationText);
            this.Controls.Add(this.txtTranslationKey);
            this.Controls.Add(this.lblTranslationKey);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TranslationEditDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Quest Description Translation";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTranslationKey;
        private System.Windows.Forms.TextBox txtTranslationKey;
        private System.Windows.Forms.Label lblTranslationText;
        private System.Windows.Forms.TextBox txtTranslationText;
        private System.Windows.Forms.Label lblExistingKeys;
        private System.Windows.Forms.ComboBox cmbExistingKeys;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOpenTranslationsEditor;
        private System.Windows.Forms.CheckBox chkIsNew;
        private System.Windows.Forms.CheckBox chkHasBeenSeen;
    }
}