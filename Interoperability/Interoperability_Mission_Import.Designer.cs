namespace interoperability
{
    partial class Interoperability_Mission_Import
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Interoperability_Mission_Import));
            this.ImportMission_Button = new System.Windows.Forms.Button();
            this.ExportMission_Button = new System.Windows.Forms.Button();
            this.SelectMission_ComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.MissionItemImport_ComboBox = new System.Windows.Forms.ComboBox();
            this.Done_Button = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // ImportMission_Button
            // 
            this.ImportMission_Button.Location = new System.Drawing.Point(22, 236);
            this.ImportMission_Button.Name = "ImportMission_Button";
            this.ImportMission_Button.Size = new System.Drawing.Size(134, 23);
            this.ImportMission_Button.TabIndex = 1;
            this.ImportMission_Button.Text = "Import Mission";
            this.ImportMission_Button.UseVisualStyleBackColor = true;
            this.ImportMission_Button.Click += new System.EventHandler(this.ImportMission_Button_Click);
            // 
            // ExportMission_Button
            // 
            this.ExportMission_Button.Location = new System.Drawing.Point(174, 236);
            this.ExportMission_Button.Name = "ExportMission_Button";
            this.ExportMission_Button.Size = new System.Drawing.Size(134, 23);
            this.ExportMission_Button.TabIndex = 2;
            this.ExportMission_Button.Text = "Export Mission";
            this.ExportMission_Button.UseVisualStyleBackColor = true;
            this.ExportMission_Button.Click += new System.EventHandler(this.ExportMission_Button_Click);
            // 
            // SelectMission_ComboBox
            // 
            this.SelectMission_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.SelectMission_ComboBox.FormattingEnabled = true;
            this.SelectMission_ComboBox.Items.AddRange(new object[] {
            "Select Mission"});
            this.SelectMission_ComboBox.Location = new System.Drawing.Point(22, 38);
            this.SelectMission_ComboBox.Name = "SelectMission_ComboBox";
            this.SelectMission_ComboBox.Size = new System.Drawing.Size(162, 21);
            this.SelectMission_ComboBox.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(19, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Mission Select:";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(207, 146);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(101, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "Import Item";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // MissionItemImport_ComboBox
            // 
            this.MissionItemImport_ComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MissionItemImport_ComboBox.FormattingEnabled = true;
            this.MissionItemImport_ComboBox.Items.AddRange(new object[] {
            "Select Item",
            "Geofence",
            "Search Area",
            "SRIC",
            "Emergent Target",
            "Drop Location "});
            this.MissionItemImport_ComboBox.Location = new System.Drawing.Point(22, 148);
            this.MissionItemImport_ComboBox.Name = "MissionItemImport_ComboBox";
            this.MissionItemImport_ComboBox.Size = new System.Drawing.Size(162, 21);
            this.MissionItemImport_ComboBox.TabIndex = 8;
            // 
            // Done_Button
            // 
            this.Done_Button.Location = new System.Drawing.Point(233, 276);
            this.Done_Button.Name = "Done_Button";
            this.Done_Button.Size = new System.Drawing.Size(75, 23);
            this.Done_Button.TabIndex = 9;
            this.Done_Button.Text = "Done";
            this.Done_Button.UseVisualStyleBackColor = true;
            this.Done_Button.Click += new System.EventHandler(this.Done_Button_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(207, 65);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(101, 23);
            this.button5.TabIndex = 10;
            this.button5.Text = "Rename Mission";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(207, 94);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(101, 23);
            this.button6.TabIndex = 11;
            this.button6.Text = "Delete Mission";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(19, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Mission Item Import:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(207, 38);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(101, 23);
            this.button2.TabIndex = 13;
            this.button2.Text = "Edit Mission";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // Interoperability_Mission_Import
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(320, 313);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.Done_Button);
            this.Controls.Add(this.MissionItemImport_ComboBox);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SelectMission_ComboBox);
            this.Controls.Add(this.ExportMission_Button);
            this.Controls.Add(this.ImportMission_Button);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Interoperability_Mission_Import";
            this.Text = "Import Mission";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Interoperability_Mission_Import_FormClosed);
            this.Shown += new System.EventHandler(this.Interoperability_Mission_Import_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button ImportMission_Button;
        private System.Windows.Forms.Button ExportMission_Button;
        private System.Windows.Forms.ComboBox SelectMission_ComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox MissionItemImport_ComboBox;
        private System.Windows.Forms.Button Done_Button;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
    }
}