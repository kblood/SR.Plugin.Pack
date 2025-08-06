namespace SatelliteReignModdingTools
{
    partial class MainMenu
    {
        private System.ComponentModel.IContainer components = null;

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
            this.btnItemBrowser = new System.Windows.Forms.Button();
            this.btnEnemyBrowser = new System.Windows.Forms.Button();
            this.btnQuestEditor = new System.Windows.Forms.Button();
            this.btnTranslationsBrowser = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnItemBrowser
            // 
            this.btnItemBrowser.BackColor = System.Drawing.Color.SeaGreen;
            this.btnItemBrowser.ForeColor = System.Drawing.Color.Aquamarine;
            this.btnItemBrowser.Location = new System.Drawing.Point(100, 100);
            this.btnItemBrowser.Name = "btnItemBrowser";
            this.btnItemBrowser.Size = new System.Drawing.Size(200, 40);
            this.btnItemBrowser.TabIndex = 0;
            this.btnItemBrowser.Text = "Item Browser";
            this.btnItemBrowser.UseVisualStyleBackColor = false;
            this.btnItemBrowser.Click += new System.EventHandler(this.btnItemBrowser_Click);
            // 
            // btnEnemyBrowser
            // 
            this.btnEnemyBrowser.BackColor = System.Drawing.Color.SeaGreen;
            this.btnEnemyBrowser.ForeColor = System.Drawing.Color.Aquamarine;
            this.btnEnemyBrowser.Location = new System.Drawing.Point(100, 150);
            this.btnEnemyBrowser.Name = "btnEnemyBrowser";
            this.btnEnemyBrowser.Size = new System.Drawing.Size(200, 40);
            this.btnEnemyBrowser.TabIndex = 1;
            this.btnEnemyBrowser.Text = "Enemy Browser";
            this.btnEnemyBrowser.UseVisualStyleBackColor = false;
            this.btnEnemyBrowser.Click += new System.EventHandler(this.btnEnemyBrowser_Click);
            // 
            // btnQuestEditor
            // 
            this.btnQuestEditor.BackColor = System.Drawing.Color.SeaGreen;
            this.btnQuestEditor.ForeColor = System.Drawing.Color.Aquamarine;
            this.btnQuestEditor.Location = new System.Drawing.Point(100, 200);
            this.btnQuestEditor.Name = "btnQuestEditor";
            this.btnQuestEditor.Size = new System.Drawing.Size(200, 40);
            this.btnQuestEditor.TabIndex = 2;
            this.btnQuestEditor.Text = "Quest Editor";
            this.btnQuestEditor.UseVisualStyleBackColor = false;
            this.btnQuestEditor.Click += new System.EventHandler(this.btnQuestEditor_Click);
            // 
            // btnTranslationsBrowser
            // 
            this.btnTranslationsBrowser.BackColor = System.Drawing.Color.SeaGreen;
            this.btnTranslationsBrowser.ForeColor = System.Drawing.Color.Aquamarine;
            this.btnTranslationsBrowser.Location = new System.Drawing.Point(100, 250);
            this.btnTranslationsBrowser.Name = "btnTranslationsBrowser";
            this.btnTranslationsBrowser.Size = new System.Drawing.Size(200, 40);
            this.btnTranslationsBrowser.TabIndex = 3;
            this.btnTranslationsBrowser.Text = "Translations Editor";
            this.btnTranslationsBrowser.UseVisualStyleBackColor = false;
            this.btnTranslationsBrowser.Click += new System.EventHandler(this.btnTranslationsBrowser_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Aquamarine;
            this.label1.Location = new System.Drawing.Point(95, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(210, 26);
            this.label1.TabIndex = 4;
            this.label1.Text = "Satellite Reign Mod Tools";
            // 
            // MainMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(400, 350);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnTranslationsBrowser);
            this.Controls.Add(this.btnQuestEditor);
            this.Controls.Add(this.btnEnemyBrowser);
            this.Controls.Add(this.btnItemBrowser);
            this.Name = "MainMenu";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Satellite Reign Mod Tools";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.Button btnItemBrowser;
        private System.Windows.Forms.Button btnEnemyBrowser;
        private System.Windows.Forms.Button btnQuestEditor;
        private System.Windows.Forms.Button btnTranslationsBrowser;
        private System.Windows.Forms.Label label1;
    }
}