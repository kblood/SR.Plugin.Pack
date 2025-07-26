namespace SatelliteReignModdingTools
{
    partial class MissionBrowser
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
            this.QuestListBox = new System.Windows.Forms.ListBox();
            this.QuestIDTextBox = new System.Windows.Forms.TextBox();
            this.QuestTitleTextBox = new System.Windows.Forms.TextBox();
            this.QuestStateDropDown = new System.Windows.Forms.ComboBox();
            this.HiddenCheckBox = new System.Windows.Forms.CheckBox();
            this.ShowDebriefCheckBox = new System.Windows.Forms.CheckBox();
            this.ActionsListBox = new System.Windows.Forms.ListBox();
            this.ReactionsListBox = new System.Windows.Forms.ListBox();
            this.DescriptionsListBox = new System.Windows.Forms.ListBox();
            this.ActionTypeDropDown = new System.Windows.Forms.ComboBox();
            this.ReactionTypeDropDown = new System.Windows.Forms.ComboBox();
            this.btnNewQuest = new System.Windows.Forms.Button();
            this.btnDeleteQuest = new System.Windows.Forms.Button();
            this.btnAddAction = new System.Windows.Forms.Button();
            this.btnAddReaction = new System.Windows.Forms.Button();
            this.btnSaveQuest = new System.Windows.Forms.Button();
            this.btnSaveAll = new System.Windows.Forms.Button();
            this.btnLoadData = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // QuestListBox
            // 
            this.QuestListBox.BackColor = System.Drawing.Color.DarkSlateGray;
            this.QuestListBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.QuestListBox.FormattingEnabled = true;
            this.QuestListBox.Location = new System.Drawing.Point(12, 30);
            this.QuestListBox.Name = "QuestListBox";
            this.QuestListBox.Size = new System.Drawing.Size(250, 400);
            this.QuestListBox.TabIndex = 0;
            this.QuestListBox.SelectedIndexChanged += new System.EventHandler(this.QuestListBox_SelectedIndexChanged);
            // 
            // QuestIDTextBox
            // 
            this.QuestIDTextBox.BackColor = System.Drawing.Color.DarkSlateGray;
            this.QuestIDTextBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.QuestIDTextBox.Location = new System.Drawing.Point(300, 50);
            this.QuestIDTextBox.Name = "QuestIDTextBox";
            this.QuestIDTextBox.ReadOnly = true;
            this.QuestIDTextBox.Size = new System.Drawing.Size(100, 20);
            this.QuestIDTextBox.TabIndex = 1;
            // 
            // QuestTitleTextBox
            // 
            this.QuestTitleTextBox.BackColor = System.Drawing.Color.DarkSlateGray;
            this.QuestTitleTextBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.QuestTitleTextBox.Location = new System.Drawing.Point(300, 90);
            this.QuestTitleTextBox.Name = "QuestTitleTextBox";
            this.QuestTitleTextBox.Size = new System.Drawing.Size(200, 20);
            this.QuestTitleTextBox.TabIndex = 2;
            // 
            // QuestStateDropDown
            // 
            this.QuestStateDropDown.BackColor = System.Drawing.Color.DarkSlateGray;
            this.QuestStateDropDown.ForeColor = System.Drawing.Color.Aquamarine;
            this.QuestStateDropDown.FormattingEnabled = true;
            this.QuestStateDropDown.Location = new System.Drawing.Point(300, 130);
            this.QuestStateDropDown.Name = "QuestStateDropDown";
            this.QuestStateDropDown.Size = new System.Drawing.Size(150, 21);
            this.QuestStateDropDown.TabIndex = 3;
            // 
            // HiddenCheckBox
            // 
            this.HiddenCheckBox.AutoSize = true;
            this.HiddenCheckBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.HiddenCheckBox.Location = new System.Drawing.Point(300, 170);
            this.HiddenCheckBox.Name = "HiddenCheckBox";
            this.HiddenCheckBox.Size = new System.Drawing.Size(59, 17);
            this.HiddenCheckBox.TabIndex = 4;
            this.HiddenCheckBox.Text = "Hidden";
            this.HiddenCheckBox.UseVisualStyleBackColor = true;
            // 
            // ShowDebriefCheckBox
            // 
            this.ShowDebriefCheckBox.AutoSize = true;
            this.ShowDebriefCheckBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.ShowDebriefCheckBox.Location = new System.Drawing.Point(380, 170);
            this.ShowDebriefCheckBox.Name = "ShowDebriefCheckBox";
            this.ShowDebriefCheckBox.Size = new System.Drawing.Size(89, 17);
            this.ShowDebriefCheckBox.TabIndex = 5;
            this.ShowDebriefCheckBox.Text = "Show Debrief";
            this.ShowDebriefCheckBox.UseVisualStyleBackColor = true;
            // 
            // ActionsListBox
            // 
            this.ActionsListBox.BackColor = System.Drawing.Color.DarkSlateGray;
            this.ActionsListBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.ActionsListBox.FormattingEnabled = true;
            this.ActionsListBox.Location = new System.Drawing.Point(530, 50);
            this.ActionsListBox.Name = "ActionsListBox";
            this.ActionsListBox.Size = new System.Drawing.Size(200, 120);
            this.ActionsListBox.TabIndex = 6;
            // 
            // ReactionsListBox
            // 
            this.ReactionsListBox.BackColor = System.Drawing.Color.DarkSlateGray;
            this.ReactionsListBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.ReactionsListBox.FormattingEnabled = true;
            this.ReactionsListBox.Location = new System.Drawing.Point(530, 210);
            this.ReactionsListBox.Name = "ReactionsListBox";
            this.ReactionsListBox.Size = new System.Drawing.Size(200, 120);
            this.ReactionsListBox.TabIndex = 7;
            // 
            // DescriptionsListBox
            // 
            this.DescriptionsListBox.BackColor = System.Drawing.Color.DarkSlateGray;
            this.DescriptionsListBox.ForeColor = System.Drawing.Color.Aquamarine;
            this.DescriptionsListBox.FormattingEnabled = true;
            this.DescriptionsListBox.Location = new System.Drawing.Point(530, 370);
            this.DescriptionsListBox.Name = "DescriptionsListBox";
            this.DescriptionsListBox.Size = new System.Drawing.Size(200, 120);
            this.DescriptionsListBox.TabIndex = 8;
            // 
            // ActionTypeDropDown
            // 
            this.ActionTypeDropDown.BackColor = System.Drawing.Color.DarkSlateGray;
            this.ActionTypeDropDown.ForeColor = System.Drawing.Color.Aquamarine;
            this.ActionTypeDropDown.FormattingEnabled = true;
            this.ActionTypeDropDown.Location = new System.Drawing.Point(750, 50);
            this.ActionTypeDropDown.Name = "ActionTypeDropDown";
            this.ActionTypeDropDown.Size = new System.Drawing.Size(150, 21);
            this.ActionTypeDropDown.TabIndex = 9;
            // 
            // ReactionTypeDropDown
            // 
            this.ReactionTypeDropDown.BackColor = System.Drawing.Color.DarkSlateGray;
            this.ReactionTypeDropDown.ForeColor = System.Drawing.Color.Aquamarine;
            this.ReactionTypeDropDown.FormattingEnabled = true;
            this.ReactionTypeDropDown.Location = new System.Drawing.Point(750, 210);
            this.ReactionTypeDropDown.Name = "ReactionTypeDropDown";
            this.ReactionTypeDropDown.Size = new System.Drawing.Size(150, 21);
            this.ReactionTypeDropDown.TabIndex = 10;
            // 
            // btnNewQuest
            // 
            this.btnNewQuest.BackColor = System.Drawing.Color.SeaGreen;
            this.btnNewQuest.ForeColor = System.Drawing.Color.Aquamarine;
            this.btnNewQuest.Location = new System.Drawing.Point(12, 450);
            this.btnNewQuest.Name = "btnNewQuest";
            this.btnNewQuest.Size = new System.Drawing.Size(80, 30);
            this.btnNewQuest.TabIndex = 11;
            this.btnNewQuest.Text = "New Quest";
            this.btnNewQuest.UseVisualStyleBackColor = false;
            this.btnNewQuest.Click += new System.EventHandler(this.btnNewQuest_Click);
            // 
            // btnDeleteQuest
            // 
            this.btnDeleteQuest.BackColor = System.Drawing.Color.Crimson;
            this.btnDeleteQuest.ForeColor = System.Drawing.Color.White;
            this.btnDeleteQuest.Location = new System.Drawing.Point(100, 450);
            this.btnDeleteQuest.Name = "btnDeleteQuest";
            this.btnDeleteQuest.Size = new System.Drawing.Size(80, 30);
            this.btnDeleteQuest.TabIndex = 12;
            this.btnDeleteQuest.Text = "Delete Quest";
            this.btnDeleteQuest.UseVisualStyleBackColor = false;
            this.btnDeleteQuest.Click += new System.EventHandler(this.btnDeleteQuest_Click);
            // 
            // btnAddAction
            // 
            this.btnAddAction.BackColor = System.Drawing.Color.SeaGreen;
            this.btnAddAction.ForeColor = System.Drawing.Color.Aquamarine;
            this.btnAddAction.Location = new System.Drawing.Point(750, 80);
            this.btnAddAction.Name = "btnAddAction";
            this.btnAddAction.Size = new System.Drawing.Size(100, 25);
            this.btnAddAction.TabIndex = 13;
            this.btnAddAction.Text = "Add Action";
            this.btnAddAction.UseVisualStyleBackColor = false;
            this.btnAddAction.Click += new System.EventHandler(this.btnAddAction_Click);
            // 
            // btnAddReaction
            // 
            this.btnAddReaction.BackColor = System.Drawing.Color.SeaGreen;
            this.btnAddReaction.ForeColor = System.Drawing.Color.Aquamarine;
            this.btnAddReaction.Location = new System.Drawing.Point(750, 240);
            this.btnAddReaction.Name = "btnAddReaction";
            this.btnAddReaction.Size = new System.Drawing.Size(100, 25);
            this.btnAddReaction.TabIndex = 14;
            this.btnAddReaction.Text = "Add Reaction";
            this.btnAddReaction.UseVisualStyleBackColor = false;
            this.btnAddReaction.Click += new System.EventHandler(this.btnAddReaction_Click);
            // 
            // btnSaveQuest
            // 
            this.btnSaveQuest.BackColor = System.Drawing.Color.Orange;
            this.btnSaveQuest.ForeColor = System.Drawing.Color.White;
            this.btnSaveQuest.Location = new System.Drawing.Point(300, 200);
            this.btnSaveQuest.Name = "btnSaveQuest";
            this.btnSaveQuest.Size = new System.Drawing.Size(100, 30);
            this.btnSaveQuest.TabIndex = 15;
            this.btnSaveQuest.Text = "Save Quest";
            this.btnSaveQuest.UseVisualStyleBackColor = false;
            this.btnSaveQuest.Click += new System.EventHandler(this.btnSaveQuest_Click);
            // 
            // btnSaveAll
            // 
            this.btnSaveAll.BackColor = System.Drawing.Color.DarkOrange;
            this.btnSaveAll.ForeColor = System.Drawing.Color.White;
            this.btnSaveAll.Location = new System.Drawing.Point(750, 450);
            this.btnSaveAll.Name = "btnSaveAll";
            this.btnSaveAll.Size = new System.Drawing.Size(100, 30);
            this.btnSaveAll.TabIndex = 16;
            this.btnSaveAll.Text = "Save All";
            this.btnSaveAll.UseVisualStyleBackColor = false;
            this.btnSaveAll.Click += new System.EventHandler(this.btnSaveAll_Click);
            // 
            // btnLoadData
            // 
            this.btnLoadData.BackColor = System.Drawing.Color.RoyalBlue;
            this.btnLoadData.ForeColor = System.Drawing.Color.White;
            this.btnLoadData.Location = new System.Drawing.Point(640, 450);
            this.btnLoadData.Name = "btnLoadData";
            this.btnLoadData.Size = new System.Drawing.Size(100, 30);
            this.btnLoadData.TabIndex = 17;
            this.btnLoadData.Text = "Load Data";
            this.btnLoadData.UseVisualStyleBackColor = false;
            this.btnLoadData.Click += new System.EventHandler(this.btnLoadData_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Aquamarine;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 18;
            this.label1.Text = "Quests:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Aquamarine;
            this.label2.Location = new System.Drawing.Point(300, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(21, 13);
            this.label2.TabIndex = 19;
            this.label2.Text = "ID:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Aquamarine;
            this.label3.Location = new System.Drawing.Point(300, 74);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 20;
            this.label3.Text = "Title:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Aquamarine;
            this.label4.Location = new System.Drawing.Point(300, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 21;
            this.label4.Text = "State:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Aquamarine;
            this.label5.Location = new System.Drawing.Point(530, 34);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 13);
            this.label5.TabIndex = 22;
            this.label5.Text = "Actions:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Aquamarine;
            this.label6.Location = new System.Drawing.Point(530, 194);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Reactions:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Aquamarine;
            this.label7.Location = new System.Drawing.Point(530, 354);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 13);
            this.label7.TabIndex = 24;
            this.label7.Text = "Descriptions:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.Aquamarine;
            this.label8.Location = new System.Drawing.Point(750, 34);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(70, 13);
            this.label8.TabIndex = 25;
            this.label8.Text = "Action Type:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.Aquamarine;
            this.label9.Location = new System.Drawing.Point(750, 194);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(84, 13);
            this.label9.TabIndex = 26;
            this.label9.Text = "Reaction Type:";
            // 
            // MissionBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DarkSlateGray;
            this.ClientSize = new System.Drawing.Size(920, 500);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnLoadData);
            this.Controls.Add(this.btnSaveAll);
            this.Controls.Add(this.btnSaveQuest);
            this.Controls.Add(this.btnAddReaction);
            this.Controls.Add(this.btnAddAction);
            this.Controls.Add(this.btnDeleteQuest);
            this.Controls.Add(this.btnNewQuest);
            this.Controls.Add(this.ReactionTypeDropDown);
            this.Controls.Add(this.ActionTypeDropDown);
            this.Controls.Add(this.DescriptionsListBox);
            this.Controls.Add(this.ReactionsListBox);
            this.Controls.Add(this.ActionsListBox);
            this.Controls.Add(this.ShowDebriefCheckBox);
            this.Controls.Add(this.HiddenCheckBox);
            this.Controls.Add(this.QuestStateDropDown);
            this.Controls.Add(this.QuestTitleTextBox);
            this.Controls.Add(this.QuestIDTextBox);
            this.Controls.Add(this.QuestListBox);
            this.Name = "MissionBrowser";
            this.Text = "Mission Browser - Satellite Reign Modding Tools";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ListBox QuestListBox;
        private System.Windows.Forms.TextBox QuestIDTextBox;
        private System.Windows.Forms.TextBox QuestTitleTextBox;
        private System.Windows.Forms.ComboBox QuestStateDropDown;
        private System.Windows.Forms.CheckBox HiddenCheckBox;
        private System.Windows.Forms.CheckBox ShowDebriefCheckBox;
        private System.Windows.Forms.ListBox ActionsListBox;
        private System.Windows.Forms.ListBox ReactionsListBox;
        private System.Windows.Forms.ListBox DescriptionsListBox;
        private System.Windows.Forms.ComboBox ActionTypeDropDown;
        private System.Windows.Forms.ComboBox ReactionTypeDropDown;
        private System.Windows.Forms.Button btnNewQuest;
        private System.Windows.Forms.Button btnDeleteQuest;
        private System.Windows.Forms.Button btnAddAction;
        private System.Windows.Forms.Button btnAddReaction;
        private System.Windows.Forms.Button btnSaveQuest;
        private System.Windows.Forms.Button btnSaveAll;
        private System.Windows.Forms.Button btnLoadData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
    }
}