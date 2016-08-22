namespace interoperability
{
    partial class Interoperability
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Interoperability));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.uniqueTelUploadText = new System.Windows.Forms.TextBox();
            this.avgTelUploadText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pollRateInput = new System.Windows.Forms.TextBox();
            this.Server_Settings = new System.Windows.Forms.Button();
            this.applyPollRateButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.Interoperability_GUI_Tab = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.Reset_Stats = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.SDA_Obstacles = new System.Windows.Forms.TextBox();
            this.SDA_Test_Button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.Interoperability_GUI_Tab.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 77);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Unique telemetry upload rate:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(22, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Average telemetry upload rate:";
            // 
            // uniqueTelUploadText
            // 
            this.uniqueTelUploadText.Location = new System.Drawing.Point(209, 74);
            this.uniqueTelUploadText.Name = "uniqueTelUploadText";
            this.uniqueTelUploadText.ReadOnly = true;
            this.uniqueTelUploadText.Size = new System.Drawing.Size(237, 20);
            this.uniqueTelUploadText.TabIndex = 2;
            this.uniqueTelUploadText.Text = "Hz";
            // 
            // avgTelUploadText
            // 
            this.avgTelUploadText.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.avgTelUploadText.Location = new System.Drawing.Point(209, 103);
            this.avgTelUploadText.Name = "avgTelUploadText";
            this.avgTelUploadText.ReadOnly = true;
            this.avgTelUploadText.Size = new System.Drawing.Size(237, 20);
            this.avgTelUploadText.TabIndex = 3;
            this.avgTelUploadText.Text = "Hz";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(22, 134);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Poll Rate:";
            // 
            // pollRateInput
            // 
            this.pollRateInput.Location = new System.Drawing.Point(209, 131);
            this.pollRateInput.Name = "pollRateInput";
            this.pollRateInput.Size = new System.Drawing.Size(156, 20);
            this.pollRateInput.TabIndex = 5;
            this.pollRateInput.Text = "10";
            // 
            // Server_Settings
            // 
            this.Server_Settings.Location = new System.Drawing.Point(676, 12);
            this.Server_Settings.Name = "Server_Settings";
            this.Server_Settings.Size = new System.Drawing.Size(131, 23);
            this.Server_Settings.TabIndex = 7;
            this.Server_Settings.Text = "Sever Settings";
            this.Server_Settings.UseVisualStyleBackColor = true;
            this.Server_Settings.Click += new System.EventHandler(this.Server_Settings_Click);
            // 
            // applyPollRateButton
            // 
            this.applyPollRateButton.Location = new System.Drawing.Point(371, 129);
            this.applyPollRateButton.Name = "applyPollRateButton";
            this.applyPollRateButton.Size = new System.Drawing.Size(75, 23);
            this.applyPollRateButton.TabIndex = 6;
            this.applyPollRateButton.Text = "Apply";
            this.applyPollRateButton.UseVisualStyleBackColor = true;
            this.applyPollRateButton.Click += new System.EventHandler(this.applyPollRateButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(279, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(228, 31);
            this.label4.TabIndex = 4;
            this.label4.Text = "Telemetry Upload";
            // 
            // Interoperability_GUI_Tab
            // 
            this.Interoperability_GUI_Tab.Controls.Add(this.tabPage1);
            this.Interoperability_GUI_Tab.Controls.Add(this.tabPage2);
            this.Interoperability_GUI_Tab.Controls.Add(this.tabPage3);
            this.Interoperability_GUI_Tab.Controls.Add(this.tabPage4);
            this.Interoperability_GUI_Tab.Location = new System.Drawing.Point(12, 22);
            this.Interoperability_GUI_Tab.Name = "Interoperability_GUI_Tab";
            this.Interoperability_GUI_Tab.SelectedIndex = 0;
            this.Interoperability_GUI_Tab.Size = new System.Drawing.Size(799, 461);
            this.Interoperability_GUI_Tab.TabIndex = 8;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.Reset_Stats);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.applyPollRateButton);
            this.tabPage1.Controls.Add(this.uniqueTelUploadText);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.avgTelUploadText);
            this.tabPage1.Controls.Add(this.pollRateInput);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(791, 435);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Telemetry ";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // Reset_Stats
            // 
            this.Reset_Stats.Location = new System.Drawing.Point(467, 72);
            this.Reset_Stats.Name = "Reset_Stats";
            this.Reset_Stats.Size = new System.Drawing.Size(75, 23);
            this.Reset_Stats.TabIndex = 7;
            this.Reset_Stats.Text = "Reset Stats";
            this.Reset_Stats.UseVisualStyleBackColor = true;
            this.Reset_Stats.Click += new System.EventHandler(this.Reset_Stats_Click);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.SDA_Obstacles);
            this.tabPage2.Controls.Add(this.SDA_Test_Button);
            this.tabPage2.Controls.Add(this.label5);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(791, 435);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "SDA";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // SDA_Obstacles
            // 
            this.SDA_Obstacles.Location = new System.Drawing.Point(20, 111);
            this.SDA_Obstacles.Multiline = true;
            this.SDA_Obstacles.Name = "SDA_Obstacles";
            this.SDA_Obstacles.ReadOnly = true;
            this.SDA_Obstacles.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SDA_Obstacles.Size = new System.Drawing.Size(293, 284);
            this.SDA_Obstacles.TabIndex = 7;
            // 
            // SDA_Test_Button
            // 
            this.SDA_Test_Button.Location = new System.Drawing.Point(334, 69);
            this.SDA_Test_Button.Name = "SDA_Test_Button";
            this.SDA_Test_Button.Size = new System.Drawing.Size(125, 23);
            this.SDA_Test_Button.TabIndex = 6;
            this.SDA_Test_Button.Text = "DO THE THING";
            this.SDA_Test_Button.UseVisualStyleBackColor = true;
            this.SDA_Test_Button.Click += new System.EventHandler(this.SDA_Test_Button_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(249, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(305, 31);
            this.label5.TabIndex = 5;
            this.label5.Text = "Sense Detect and Avoid";
            // 
            // tabPage3
            // 
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(791, 435);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Image Upload";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(791, 435);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Server Info";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // Interoperability
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(823, 495);
            this.Controls.Add(this.Server_Settings);
            this.Controls.Add(this.Interoperability_GUI_Tab);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Interoperability";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "UTAT UAV Interoperability Control Panel";
            this.Interoperability_GUI_Tab.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox uniqueTelUploadText;
        private System.Windows.Forms.TextBox avgTelUploadText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox pollRateInput;
        private System.Windows.Forms.Button applyPollRateButton;
        private System.Windows.Forms.Button Server_Settings;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabControl Interoperability_GUI_Tab;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Button Reset_Stats;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button SDA_Test_Button;
        private System.Windows.Forms.TextBox SDA_Obstacles;
    }
}