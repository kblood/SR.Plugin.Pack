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
            this.btnWeaponBrowser = new System.Windows.Forms.Button();
            this.btnMissionBrowser = new System.Windows.Forms.Button();
            this.btnQuestBrowser = new System.Windows.Forms.Button();
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
            // btnWeaponBrowser
            // 
            this.btnWeaponBrowser.BackColor = System.Drawing.Color.SeaGreen;
            this.btnWeaponBrowser.ForeColor = System.Drawing.Color.Aquamarine;
            this.btnWeaponBrowser.Location = new System.Drawing.Point(100, 200);
            this.btnWeaponBrowser.Name = "btnWeaponBrowser";
            this.btnWeaponBrowser.Size = new System.Drawing.Size(200, 40);
            this.btnWeaponBrowser.TabIndex = 2;
            this.btnWeaponBrowser.Text = "Weapon Browser";
            this.btnWeaponBrowser.UseVisualStyleBackColor = false;
            this.btnWeaponBrowser.Click += new System.EventHandler(this.btnWeaponBrowser_Click);
            // 
            // btnMissionBrowser
            // 
            this.btnMissionBrowser.BackColor = System.Drawing.Color.SeaGreen;
            this.btnMissionBrowser.ForeColor = System.Drawing.Color.Aquamarine;
            this.btnMissionBrowser.Location = new System.Drawing.Point(100, 250);
            this.btnMissionBrowser.Name = "btnMissionBrowser";
            this.btnMissionBrowser.Size = new System.Drawing.Size(200, 40);
            this.btnMissionBrowser.TabIndex = 3;
            this.btnMissionBrowser.Text = "Mission Browser";
            this.btnMissionBrowser.UseVisualStyleBackColor = false;
            this.btnMissionBrowser.Click += new System.EventHandler(this.btnMissionBrowser_Click);
            // 
            // btnQuestBrowser
            // 
            this.btnQuestBrowser.BackColor = System.Drawing.Color.SeaGreen;
            this.btnQuestBrowser.ForeColor = System.Drawing.Color.Aquamarine;
            this.btnQuestBrowser.Location = new System.Drawing.Point(100, 300);
            this.btnQuestBrowser.Name = "btnQuestBrowser";
            this.btnQuestBrowser.Size = new System.Drawing.Size(200, 40);
            this.btnQuestBrowser.TabIndex = 4;
            this.btnQuestBrowser.Text = "Quest Browser";
            this.btnQuestBrowser.UseVisualStyleBackColor = false;
            this.btnQuestBrowser.Click += new System.EventHandler(this.btnQuestBrowser_Click);
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
            this.ClientSize = new System.Drawing.Size(400, 400);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnQuestBrowser);
            this.Controls.Add(this.btnMissionBrowser);
            this.Controls.Add(this.btnWeaponBrowser);
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
        private System.Windows.Forms.Button btnWeaponBrowser;
        private System.Windows.Forms.Button btnMissionBrowser;
        private System.Windows.Forms.Button btnQuestBrowser;
        private System.Windows.Forms.Label label1;
    }
}