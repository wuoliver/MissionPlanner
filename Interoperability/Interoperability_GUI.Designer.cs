namespace Interoperability_GUI
{
    partial class Interoperability_GUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Interoperability_GUI));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.uniqueTelUploadText = new System.Windows.Forms.TextBox();
            this.avgTelUploadText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Telemetry_pollRateInput = new System.Windows.Forms.TextBox();
            this.Server_Settings = new System.Windows.Forms.Button();
            this.applyPollRateButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.Interoperability_GUI_Tab = new System.Windows.Forms.TabControl();
            this.Telem_Tab = new System.Windows.Forms.TabPage();
            this.Telem_Start_Stop_Button = new System.Windows.Forms.Button();
            this.TelemServerResp = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.Total_Telem_Rate = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Reset_Stats = new System.Windows.Forms.Button();
            this.SDA_Tab = new System.Windows.Forms.TabPage();
            this.SDA_ServerResponseTextBox = new System.Windows.Forms.TextBox();
            this.SDA_ServerResponseLabel = new System.Windows.Forms.Label();
            this.SDA_PollRateApply = new System.Windows.Forms.Button();
            this.SDA_pollrateInput = new System.Windows.Forms.TextBox();
            this.SDA_PollRate = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.SDA_Obstacles = new System.Windows.Forms.TextBox();
            this.SDA_Start_Stop_Button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.Map_Tab = new System.Windows.Forms.TabPage();
            this.Fixed_UAS_Size_Checkbox = new System.Windows.Forms.CheckBox();
            this.label12 = new System.Windows.Forms.Label();
            this.UAS_Trackbar = new System.Windows.Forms.TrackBar();
            this.checkBox6 = new System.Windows.Forms.CheckBox();
            this.UASLoc_Checkbox = new System.Windows.Forms.CheckBox();
            this.Waypoints_Checkbox = new System.Windows.Forms.CheckBox();
            this.SearchArea_Checkbox = new System.Windows.Forms.CheckBox();
            this.Obstacles_Checkbox = new System.Windows.Forms.CheckBox();
            this.Geofence_Checkbox = new System.Windows.Forms.CheckBox();
            this.label11 = new System.Windows.Forms.Label();
            this.Map_ApplyRefreshRate = new System.Windows.Forms.Button();
            this.Map_RefreshRateInput = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.Mission_Enable = new System.Windows.Forms.Button();
            this.TargetUpload_Tab = new System.Windows.Forms.TabPage();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.Map_ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.displayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGeofenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSearchAreaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showObstaclesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showPlaneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showWaypointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.UAS_GPS_Label = new System.Windows.Forms.Label();
            this.UAS_Altitude_ASL_Label = new System.Windows.Forms.Label();
            this.AutoPan_Checkbox = new System.Windows.Forms.CheckBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.UAS_D_Altitude_Label = new System.Windows.Forms.Label();
            this.Interop_Tooltip = new System.Windows.Forms.ToolTip(this.components);
            this.Interoperability_GUI_Tab.SuspendLayout();
            this.Telem_Tab.SuspendLayout();
            this.SDA_Tab.SuspendLayout();
            this.Map_Tab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UAS_Trackbar)).BeginInit();
            this.Map_ContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(13, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Unique telemetry upload rate:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(13, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Average telemetry upload rate:";
            // 
            // uniqueTelUploadText
            // 
            this.uniqueTelUploadText.Location = new System.Drawing.Point(16, 70);
            this.uniqueTelUploadText.Name = "uniqueTelUploadText";
            this.uniqueTelUploadText.ReadOnly = true;
            this.uniqueTelUploadText.Size = new System.Drawing.Size(237, 20);
            this.uniqueTelUploadText.TabIndex = 2;
            this.uniqueTelUploadText.Text = "Hz";
            // 
            // avgTelUploadText
            // 
            this.avgTelUploadText.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.avgTelUploadText.Location = new System.Drawing.Point(16, 125);
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
            this.label3.Location = new System.Drawing.Point(13, 223);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Poll Rate:";
            // 
            // Telemetry_pollRateInput
            // 
            this.Telemetry_pollRateInput.Location = new System.Drawing.Point(82, 220);
            this.Telemetry_pollRateInput.Name = "Telemetry_pollRateInput";
            this.Telemetry_pollRateInput.Size = new System.Drawing.Size(90, 20);
            this.Telemetry_pollRateInput.TabIndex = 5;
            this.Telemetry_pollRateInput.Text = "10";
            // 
            // Server_Settings
            // 
            this.Server_Settings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Server_Settings.Location = new System.Drawing.Point(1032, 12);
            this.Server_Settings.Name = "Server_Settings";
            this.Server_Settings.Size = new System.Drawing.Size(131, 23);
            this.Server_Settings.TabIndex = 7;
            this.Server_Settings.Text = "Settings";
            this.Server_Settings.UseVisualStyleBackColor = true;
            this.Server_Settings.Click += new System.EventHandler(this.Server_Settings_Click);
            // 
            // applyPollRateButton
            // 
            this.applyPollRateButton.Location = new System.Drawing.Point(178, 218);
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
            this.label4.Location = new System.Drawing.Point(10, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(228, 31);
            this.label4.TabIndex = 4;
            this.label4.Text = "Telemetry Upload";
            // 
            // Interoperability_GUI_Tab
            // 
            this.Interoperability_GUI_Tab.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Interoperability_GUI_Tab.Controls.Add(this.Telem_Tab);
            this.Interoperability_GUI_Tab.Controls.Add(this.SDA_Tab);
            this.Interoperability_GUI_Tab.Controls.Add(this.Map_Tab);
            this.Interoperability_GUI_Tab.Controls.Add(this.TargetUpload_Tab);
            this.Interoperability_GUI_Tab.Location = new System.Drawing.Point(12, 20);
            this.Interoperability_GUI_Tab.Name = "Interoperability_GUI_Tab";
            this.Interoperability_GUI_Tab.SelectedIndex = 0;
            this.Interoperability_GUI_Tab.Size = new System.Drawing.Size(279, 564);
            this.Interoperability_GUI_Tab.TabIndex = 8;
            // 
            // Telem_Tab
            // 
            this.Telem_Tab.Controls.Add(this.Telem_Start_Stop_Button);
            this.Telem_Tab.Controls.Add(this.TelemServerResp);
            this.Telem_Tab.Controls.Add(this.label7);
            this.Telem_Tab.Controls.Add(this.Total_Telem_Rate);
            this.Telem_Tab.Controls.Add(this.label6);
            this.Telem_Tab.Controls.Add(this.Reset_Stats);
            this.Telem_Tab.Controls.Add(this.label4);
            this.Telem_Tab.Controls.Add(this.label3);
            this.Telem_Tab.Controls.Add(this.applyPollRateButton);
            this.Telem_Tab.Controls.Add(this.uniqueTelUploadText);
            this.Telem_Tab.Controls.Add(this.label2);
            this.Telem_Tab.Controls.Add(this.label1);
            this.Telem_Tab.Controls.Add(this.avgTelUploadText);
            this.Telem_Tab.Controls.Add(this.Telemetry_pollRateInput);
            this.Telem_Tab.Location = new System.Drawing.Point(4, 22);
            this.Telem_Tab.Name = "Telem_Tab";
            this.Telem_Tab.Padding = new System.Windows.Forms.Padding(3);
            this.Telem_Tab.Size = new System.Drawing.Size(271, 538);
            this.Telem_Tab.TabIndex = 0;
            this.Telem_Tab.Text = "Telemetry ";
            this.Telem_Tab.UseVisualStyleBackColor = true;
            // 
            // Telem_Start_Stop_Button
            // 
            this.Telem_Start_Stop_Button.Location = new System.Drawing.Point(16, 303);
            this.Telem_Start_Stop_Button.Name = "Telem_Start_Stop_Button";
            this.Telem_Start_Stop_Button.Size = new System.Drawing.Size(75, 23);
            this.Telem_Start_Stop_Button.TabIndex = 12;
            this.Telem_Start_Stop_Button.Text = "Start";
            this.Telem_Start_Stop_Button.UseVisualStyleBackColor = true;
            this.Telem_Start_Stop_Button.Click += new System.EventHandler(this.Telem_Start_Stop_Button_Click);
            // 
            // TelemServerResp
            // 
            this.TelemServerResp.Location = new System.Drawing.Point(16, 268);
            this.TelemServerResp.Name = "TelemServerResp";
            this.TelemServerResp.ReadOnly = true;
            this.TelemServerResp.Size = new System.Drawing.Size(237, 20);
            this.TelemServerResp.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(13, 252);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Server Response:";
            // 
            // Total_Telem_Rate
            // 
            this.Total_Telem_Rate.Location = new System.Drawing.Point(16, 181);
            this.Total_Telem_Rate.Name = "Total_Telem_Rate";
            this.Total_Telem_Rate.ReadOnly = true;
            this.Total_Telem_Rate.Size = new System.Drawing.Size(237, 20);
            this.Total_Telem_Rate.TabIndex = 9;
            this.Total_Telem_Rate.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(13, 165);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(173, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Total telemetry upload count:";
            // 
            // Reset_Stats
            // 
            this.Reset_Stats.Location = new System.Drawing.Point(178, 303);
            this.Reset_Stats.Name = "Reset_Stats";
            this.Reset_Stats.Size = new System.Drawing.Size(75, 23);
            this.Reset_Stats.TabIndex = 7;
            this.Reset_Stats.Text = "Reset Stats";
            this.Reset_Stats.UseVisualStyleBackColor = true;
            this.Reset_Stats.Click += new System.EventHandler(this.Reset_Stats_Click);
            // 
            // SDA_Tab
            // 
            this.SDA_Tab.Controls.Add(this.SDA_ServerResponseTextBox);
            this.SDA_Tab.Controls.Add(this.SDA_ServerResponseLabel);
            this.SDA_Tab.Controls.Add(this.SDA_PollRateApply);
            this.SDA_Tab.Controls.Add(this.SDA_pollrateInput);
            this.SDA_Tab.Controls.Add(this.SDA_PollRate);
            this.SDA_Tab.Controls.Add(this.label9);
            this.SDA_Tab.Controls.Add(this.SDA_Obstacles);
            this.SDA_Tab.Controls.Add(this.SDA_Start_Stop_Button);
            this.SDA_Tab.Controls.Add(this.label5);
            this.SDA_Tab.Location = new System.Drawing.Point(4, 22);
            this.SDA_Tab.Name = "SDA_Tab";
            this.SDA_Tab.Padding = new System.Windows.Forms.Padding(3);
            this.SDA_Tab.Size = new System.Drawing.Size(271, 538);
            this.SDA_Tab.TabIndex = 1;
            this.SDA_Tab.Text = "SDA";
            this.SDA_Tab.UseVisualStyleBackColor = true;
            // 
            // SDA_ServerResponseTextBox
            // 
            this.SDA_ServerResponseTextBox.Location = new System.Drawing.Point(17, 105);
            this.SDA_ServerResponseTextBox.Name = "SDA_ServerResponseTextBox";
            this.SDA_ServerResponseTextBox.ReadOnly = true;
            this.SDA_ServerResponseTextBox.Size = new System.Drawing.Size(237, 20);
            this.SDA_ServerResponseTextBox.TabIndex = 13;
            // 
            // SDA_ServerResponseLabel
            // 
            this.SDA_ServerResponseLabel.AutoSize = true;
            this.SDA_ServerResponseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SDA_ServerResponseLabel.Location = new System.Drawing.Point(14, 89);
            this.SDA_ServerResponseLabel.Name = "SDA_ServerResponseLabel";
            this.SDA_ServerResponseLabel.Size = new System.Drawing.Size(108, 13);
            this.SDA_ServerResponseLabel.TabIndex = 12;
            this.SDA_ServerResponseLabel.Text = "Server Response:";
            // 
            // SDA_PollRateApply
            // 
            this.SDA_PollRateApply.Location = new System.Drawing.Point(179, 54);
            this.SDA_PollRateApply.Name = "SDA_PollRateApply";
            this.SDA_PollRateApply.Size = new System.Drawing.Size(75, 23);
            this.SDA_PollRateApply.TabIndex = 11;
            this.SDA_PollRateApply.Text = "Apply";
            this.SDA_PollRateApply.UseVisualStyleBackColor = true;
            this.SDA_PollRateApply.Click += new System.EventHandler(this.SDA_PollRateApply_Click);
            // 
            // SDA_pollrateInput
            // 
            this.SDA_pollrateInput.Location = new System.Drawing.Point(83, 56);
            this.SDA_pollrateInput.Name = "SDA_pollrateInput";
            this.SDA_pollrateInput.Size = new System.Drawing.Size(90, 20);
            this.SDA_pollrateInput.TabIndex = 10;
            this.SDA_pollrateInput.Text = "10";
            // 
            // SDA_PollRate
            // 
            this.SDA_PollRate.AutoSize = true;
            this.SDA_PollRate.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SDA_PollRate.Location = new System.Drawing.Point(14, 59);
            this.SDA_PollRate.Name = "SDA_PollRate";
            this.SDA_PollRate.Size = new System.Drawing.Size(63, 13);
            this.SDA_PollRate.TabIndex = 9;
            this.SDA_PollRate.Text = "Poll Rate:";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(14, 140);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(124, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Obstacle Information";
            // 
            // SDA_Obstacles
            // 
            this.SDA_Obstacles.Location = new System.Drawing.Point(17, 156);
            this.SDA_Obstacles.Multiline = true;
            this.SDA_Obstacles.Name = "SDA_Obstacles";
            this.SDA_Obstacles.ReadOnly = true;
            this.SDA_Obstacles.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.SDA_Obstacles.Size = new System.Drawing.Size(237, 241);
            this.SDA_Obstacles.TabIndex = 7;
            // 
            // SDA_Start_Stop_Button
            // 
            this.SDA_Start_Stop_Button.Location = new System.Drawing.Point(17, 465);
            this.SDA_Start_Stop_Button.Name = "SDA_Start_Stop_Button";
            this.SDA_Start_Stop_Button.Size = new System.Drawing.Size(125, 23);
            this.SDA_Start_Stop_Button.TabIndex = 6;
            this.SDA_Start_Stop_Button.Text = "Start SDA Polling";
            this.SDA_Start_Stop_Button.UseVisualStyleBackColor = true;
            this.SDA_Start_Stop_Button.Click += new System.EventHandler(this.SDA_Start_Stop_Button_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(10, 13);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(165, 31);
            this.label5.TabIndex = 5;
            this.label5.Text = "SDA Control";
            // 
            // Map_Tab
            // 
            this.Map_Tab.Controls.Add(this.Fixed_UAS_Size_Checkbox);
            this.Map_Tab.Controls.Add(this.label12);
            this.Map_Tab.Controls.Add(this.UAS_Trackbar);
            this.Map_Tab.Controls.Add(this.checkBox6);
            this.Map_Tab.Controls.Add(this.UASLoc_Checkbox);
            this.Map_Tab.Controls.Add(this.Waypoints_Checkbox);
            this.Map_Tab.Controls.Add(this.SearchArea_Checkbox);
            this.Map_Tab.Controls.Add(this.Obstacles_Checkbox);
            this.Map_Tab.Controls.Add(this.Geofence_Checkbox);
            this.Map_Tab.Controls.Add(this.label11);
            this.Map_Tab.Controls.Add(this.Map_ApplyRefreshRate);
            this.Map_Tab.Controls.Add(this.Map_RefreshRateInput);
            this.Map_Tab.Controls.Add(this.label10);
            this.Map_Tab.Controls.Add(this.label8);
            this.Map_Tab.Controls.Add(this.Mission_Enable);
            this.Map_Tab.Location = new System.Drawing.Point(4, 22);
            this.Map_Tab.Name = "Map_Tab";
            this.Map_Tab.Padding = new System.Windows.Forms.Padding(3);
            this.Map_Tab.Size = new System.Drawing.Size(271, 538);
            this.Map_Tab.TabIndex = 4;
            this.Map_Tab.Text = "Map Control";
            this.Map_Tab.UseVisualStyleBackColor = true;
            // 
            // Fixed_UAS_Size_Checkbox
            // 
            this.Fixed_UAS_Size_Checkbox.AutoSize = true;
            this.Fixed_UAS_Size_Checkbox.Location = new System.Drawing.Point(138, 238);
            this.Fixed_UAS_Size_Checkbox.Name = "Fixed_UAS_Size_Checkbox";
            this.Fixed_UAS_Size_Checkbox.Size = new System.Drawing.Size(116, 17);
            this.Fixed_UAS_Size_Checkbox.TabIndex = 15;
            this.Fixed_UAS_Size_Checkbox.Text = "Use fixed UAS size";
            this.Fixed_UAS_Size_Checkbox.UseVisualStyleBackColor = true;
            this.Fixed_UAS_Size_Checkbox.CheckedChanged += new System.EventHandler(this.Fixed_UAS_Size_Checkbox_CheckedChanged);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(13, 239);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(72, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "UAS Scale:";
            // 
            // UAS_Trackbar
            // 
            this.UAS_Trackbar.BackColor = System.Drawing.SystemColors.Window;
            this.UAS_Trackbar.Location = new System.Drawing.Point(16, 255);
            this.UAS_Trackbar.Maximum = 300;
            this.UAS_Trackbar.Minimum = 2;
            this.UAS_Trackbar.Name = "UAS_Trackbar";
            this.UAS_Trackbar.Size = new System.Drawing.Size(238, 45);
            this.UAS_Trackbar.TabIndex = 13;
            this.UAS_Trackbar.TickFrequency = 8;
            this.UAS_Trackbar.Value = 2;
            this.UAS_Trackbar.Scroll += new System.EventHandler(this.UAS_Trackbar_Scroll);
            // 
            // checkBox6
            // 
            this.checkBox6.AutoSize = true;
            this.checkBox6.Location = new System.Drawing.Point(138, 189);
            this.checkBox6.Name = "checkBox6";
            this.checkBox6.Size = new System.Drawing.Size(79, 17);
            this.checkBox6.TabIndex = 12;
            this.checkBox6.Text = "Future Item";
            this.checkBox6.UseVisualStyleBackColor = true;
            // 
            // UASLoc_Checkbox
            // 
            this.UASLoc_Checkbox.AutoSize = true;
            this.UASLoc_Checkbox.Location = new System.Drawing.Point(16, 189);
            this.UASLoc_Checkbox.Name = "UASLoc_Checkbox";
            this.UASLoc_Checkbox.Size = new System.Drawing.Size(92, 17);
            this.UASLoc_Checkbox.TabIndex = 11;
            this.UASLoc_Checkbox.Text = "UAS Location";
            this.UASLoc_Checkbox.UseVisualStyleBackColor = true;
            this.UASLoc_Checkbox.CheckedChanged += new System.EventHandler(this.UASLoc_Checkbox_CheckedChanged);
            // 
            // Waypoints_Checkbox
            // 
            this.Waypoints_Checkbox.AutoSize = true;
            this.Waypoints_Checkbox.Location = new System.Drawing.Point(138, 155);
            this.Waypoints_Checkbox.Name = "Waypoints_Checkbox";
            this.Waypoints_Checkbox.Size = new System.Drawing.Size(76, 17);
            this.Waypoints_Checkbox.TabIndex = 10;
            this.Waypoints_Checkbox.Text = "Waypoints";
            this.Waypoints_Checkbox.UseVisualStyleBackColor = true;
            this.Waypoints_Checkbox.CheckedChanged += new System.EventHandler(this.Waypoints_Checkbox_CheckedChanged);
            // 
            // SearchArea_Checkbox
            // 
            this.SearchArea_Checkbox.AutoSize = true;
            this.SearchArea_Checkbox.Location = new System.Drawing.Point(138, 120);
            this.SearchArea_Checkbox.Name = "SearchArea_Checkbox";
            this.SearchArea_Checkbox.Size = new System.Drawing.Size(85, 17);
            this.SearchArea_Checkbox.TabIndex = 9;
            this.SearchArea_Checkbox.Text = "Search Area";
            this.SearchArea_Checkbox.UseVisualStyleBackColor = true;
            this.SearchArea_Checkbox.CheckedChanged += new System.EventHandler(this.SearchArea_Checkbox_CheckedChanged);
            // 
            // Obstacles_Checkbox
            // 
            this.Obstacles_Checkbox.AutoSize = true;
            this.Obstacles_Checkbox.Location = new System.Drawing.Point(16, 155);
            this.Obstacles_Checkbox.Name = "Obstacles_Checkbox";
            this.Obstacles_Checkbox.Size = new System.Drawing.Size(73, 17);
            this.Obstacles_Checkbox.TabIndex = 8;
            this.Obstacles_Checkbox.Text = "Obstacles";
            this.Obstacles_Checkbox.UseVisualStyleBackColor = true;
            this.Obstacles_Checkbox.CheckedChanged += new System.EventHandler(this.Obstacles_Checkbox_CheckedChanged);
            // 
            // Geofence_Checkbox
            // 
            this.Geofence_Checkbox.AutoSize = true;
            this.Geofence_Checkbox.Location = new System.Drawing.Point(16, 120);
            this.Geofence_Checkbox.Name = "Geofence_Checkbox";
            this.Geofence_Checkbox.Size = new System.Drawing.Size(73, 17);
            this.Geofence_Checkbox.TabIndex = 7;
            this.Geofence_Checkbox.Text = "Geofence";
            this.Geofence_Checkbox.UseVisualStyleBackColor = true;
            this.Geofence_Checkbox.CheckedChanged += new System.EventHandler(this.Geofence_Checkbox_CheckedChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(13, 89);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(76, 13);
            this.label11.TabIndex = 6;
            this.label11.Text = "Map Layers:";
            // 
            // Map_ApplyRefreshRate
            // 
            this.Map_ApplyRefreshRate.Location = new System.Drawing.Point(179, 54);
            this.Map_ApplyRefreshRate.Name = "Map_ApplyRefreshRate";
            this.Map_ApplyRefreshRate.Size = new System.Drawing.Size(75, 23);
            this.Map_ApplyRefreshRate.TabIndex = 5;
            this.Map_ApplyRefreshRate.Text = "Apply";
            this.Map_ApplyRefreshRate.UseVisualStyleBackColor = true;
            this.Map_ApplyRefreshRate.Click += new System.EventHandler(this.Map_ApplyRefreshRate_Click);
            // 
            // Map_RefreshRateInput
            // 
            this.Map_RefreshRateInput.Location = new System.Drawing.Point(105, 56);
            this.Map_RefreshRateInput.Name = "Map_RefreshRateInput";
            this.Map_RefreshRateInput.Size = new System.Drawing.Size(68, 20);
            this.Map_RefreshRateInput.TabIndex = 4;
            this.Map_RefreshRateInput.Text = "10";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(13, 59);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(86, 13);
            this.label10.TabIndex = 3;
            this.label10.Text = "Refresh Rate:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(10, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(161, 31);
            this.label8.TabIndex = 2;
            this.label8.Text = "Map Control";
            // 
            // Mission_Enable
            // 
            this.Mission_Enable.Location = new System.Drawing.Point(16, 490);
            this.Mission_Enable.Name = "Mission_Enable";
            this.Mission_Enable.Size = new System.Drawing.Size(163, 23);
            this.Mission_Enable.TabIndex = 1;
            this.Mission_Enable.Text = "Get Mission Info";
            this.Mission_Enable.UseVisualStyleBackColor = true;
            this.Mission_Enable.Click += new System.EventHandler(this.Mission_Enable_Click);
            // 
            // TargetUpload_Tab
            // 
            this.TargetUpload_Tab.Location = new System.Drawing.Point(4, 22);
            this.TargetUpload_Tab.Name = "TargetUpload_Tab";
            this.TargetUpload_Tab.Padding = new System.Windows.Forms.Padding(3);
            this.TargetUpload_Tab.Size = new System.Drawing.Size(271, 538);
            this.TargetUpload_Tab.TabIndex = 2;
            this.TargetUpload_Tab.Text = "Image Upload";
            this.TargetUpload_Tab.UseVisualStyleBackColor = true;
            // 
            // gMapControl1
            // 
            this.gMapControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl1.AutoSize = true;
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.ContextMenuStrip = this.Map_ContextMenuStrip;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(294, 41);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 25;
            this.gMapControl1.MinZoom = 2;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(869, 526);
            this.gMapControl1.TabIndex = 0;
            this.gMapControl1.Zoom = 0D;
            this.gMapControl1.OnMapDrag += new GMap.NET.MapDrag(this.gMapControl1_OnMapDrag);
            this.gMapControl1.OnMapZoomChanged += new GMap.NET.MapZoomChanged(this.gMapControl1_OnMapZoomChanged);
            this.gMapControl1.Load += new System.EventHandler(this.gMapControl1_Load);
            this.gMapControl1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.gMapControl1_KeyPress);
            // 
            // Map_ContextMenuStrip
            // 
            this.Map_ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayToolStripMenuItem});
            this.Map_ContextMenuStrip.Name = "contextMenuStrip1";
            this.Map_ContextMenuStrip.Size = new System.Drawing.Size(113, 26);
            // 
            // displayToolStripMenuItem
            // 
            this.displayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showGeofenceToolStripMenuItem,
            this.showSearchAreaToolStripMenuItem,
            this.showObstaclesToolStripMenuItem,
            this.showPlaneToolStripMenuItem,
            this.showWaypointsToolStripMenuItem});
            this.displayToolStripMenuItem.Name = "displayToolStripMenuItem";
            this.displayToolStripMenuItem.Size = new System.Drawing.Size(112, 22);
            this.displayToolStripMenuItem.Text = "Display";
            // 
            // showGeofenceToolStripMenuItem
            // 
            this.showGeofenceToolStripMenuItem.Name = "showGeofenceToolStripMenuItem";
            this.showGeofenceToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.showGeofenceToolStripMenuItem.Text = "Show Geofence";
            this.showGeofenceToolStripMenuItem.Click += new System.EventHandler(this.showGeofenceToolStripMenuItem_Click);
            // 
            // showSearchAreaToolStripMenuItem
            // 
            this.showSearchAreaToolStripMenuItem.Name = "showSearchAreaToolStripMenuItem";
            this.showSearchAreaToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.showSearchAreaToolStripMenuItem.Text = "Show Search Area";
            this.showSearchAreaToolStripMenuItem.Click += new System.EventHandler(this.showSearchAreaToolStripMenuItem_Click);
            // 
            // showObstaclesToolStripMenuItem
            // 
            this.showObstaclesToolStripMenuItem.Name = "showObstaclesToolStripMenuItem";
            this.showObstaclesToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.showObstaclesToolStripMenuItem.Text = "Show Obstacles";
            this.showObstaclesToolStripMenuItem.Click += new System.EventHandler(this.showObstaclesToolStripMenuItem_Click);
            // 
            // showPlaneToolStripMenuItem
            // 
            this.showPlaneToolStripMenuItem.Name = "showPlaneToolStripMenuItem";
            this.showPlaneToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.showPlaneToolStripMenuItem.Text = "Show UAS Location";
            this.showPlaneToolStripMenuItem.Click += new System.EventHandler(this.showPlaneToolStripMenuItem_Click);
            // 
            // showWaypointsToolStripMenuItem
            // 
            this.showWaypointsToolStripMenuItem.Name = "showWaypointsToolStripMenuItem";
            this.showWaypointsToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.showWaypointsToolStripMenuItem.Text = "Show Waypoints";
            this.showWaypointsToolStripMenuItem.Click += new System.EventHandler(this.showWaypointsToolStripMenuItem_Click);
            // 
            // UAS_GPS_Label
            // 
            this.UAS_GPS_Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.UAS_GPS_Label.AutoSize = true;
            this.UAS_GPS_Label.Location = new System.Drawing.Point(375, 571);
            this.UAS_GPS_Label.Name = "UAS_GPS_Label";
            this.UAS_GPS_Label.Size = new System.Drawing.Size(115, 13);
            this.UAS_GPS_Label.TabIndex = 9;
            this.UAS_GPS_Label.Text = "00.000000  00.000000";
            // 
            // UAS_Altitude_ASL_Label
            // 
            this.UAS_Altitude_ASL_Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.UAS_Altitude_ASL_Label.AutoSize = true;
            this.UAS_Altitude_ASL_Label.Location = new System.Drawing.Point(635, 570);
            this.UAS_Altitude_ASL_Label.Name = "UAS_Altitude_ASL_Label";
            this.UAS_Altitude_ASL_Label.Size = new System.Drawing.Size(48, 13);
            this.UAS_Altitude_ASL_Label.TabIndex = 10;
            this.UAS_Altitude_ASL_Label.Text = "000.00m";
            // 
            // AutoPan_Checkbox
            // 
            this.AutoPan_Checkbox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.AutoPan_Checkbox.AutoSize = true;
            this.AutoPan_Checkbox.Location = new System.Drawing.Point(874, 569);
            this.AutoPan_Checkbox.Name = "AutoPan_Checkbox";
            this.AutoPan_Checkbox.Size = new System.Drawing.Size(70, 17);
            this.AutoPan_Checkbox.TabIndex = 11;
            this.AutoPan_Checkbox.Text = "Auto Pan";
            this.AutoPan_Checkbox.UseVisualStyleBackColor = true;
            this.AutoPan_Checkbox.CheckedChanged += new System.EventHandler(this.AutoPan_Checkbox_CheckedChanged);
            // 
            // label13
            // 
            this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(297, 570);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(72, 13);
            this.label13.TabIndex = 12;
            this.label13.Text = "UAS Cords:";
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(547, 570);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(82, 13);
            this.label14.TabIndex = 13;
            this.label14.Text = "UAS Alt ASL:";
            this.Interop_Tooltip.SetToolTip(this.label14, "The absolute altitude of the UAS above sea level");
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(700, 570);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(83, 13);
            this.label15.TabIndex = 14;
            this.label15.Text = "UAS Alt AGL:";
            this.Interop_Tooltip.SetToolTip(this.label15, "The distance between the altitude of the UAS (ASL) \r\nand the elevation of the gro" +
        "und immediate below the UAS\r\n");
            // 
            // UAS_D_Altitude_Label
            // 
            this.UAS_D_Altitude_Label.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.UAS_D_Altitude_Label.AutoSize = true;
            this.UAS_D_Altitude_Label.Location = new System.Drawing.Point(789, 570);
            this.UAS_D_Altitude_Label.Name = "UAS_D_Altitude_Label";
            this.UAS_D_Altitude_Label.Size = new System.Drawing.Size(48, 13);
            this.UAS_D_Altitude_Label.TabIndex = 15;
            this.UAS_D_Altitude_Label.Text = "000.00m";
            // 
            // Interoperability_GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1178, 596);
            this.Controls.Add(this.UAS_D_Altitude_Label);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.AutoPan_Checkbox);
            this.Controls.Add(this.UAS_Altitude_ASL_Label);
            this.Controls.Add(this.UAS_GPS_Label);
            this.Controls.Add(this.gMapControl1);
            this.Controls.Add(this.Server_Settings);
            this.Controls.Add(this.Interoperability_GUI_Tab);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Interoperability_GUI";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "UTAT UAV Interoperability Control Panel";
            this.Interoperability_GUI_Tab.ResumeLayout(false);
            this.Telem_Tab.ResumeLayout(false);
            this.Telem_Tab.PerformLayout();
            this.SDA_Tab.ResumeLayout(false);
            this.SDA_Tab.PerformLayout();
            this.Map_Tab.ResumeLayout(false);
            this.Map_Tab.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.UAS_Trackbar)).EndInit();
            this.Map_ContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox uniqueTelUploadText;
        private System.Windows.Forms.TextBox avgTelUploadText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox Telemetry_pollRateInput;
        private System.Windows.Forms.Button applyPollRateButton;
        private System.Windows.Forms.Button Server_Settings;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TabControl Interoperability_GUI_Tab;
        private System.Windows.Forms.TabPage Telem_Tab;
        private System.Windows.Forms.TabPage SDA_Tab;
        private System.Windows.Forms.Button Reset_Stats;
        private System.Windows.Forms.TabPage TargetUpload_Tab;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button SDA_Start_Stop_Button;
        private System.Windows.Forms.TextBox SDA_Obstacles;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Total_Telem_Rate;
        private System.Windows.Forms.TextBox TelemServerResp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button Mission_Enable;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ContextMenuStrip Map_ContextMenuStrip;
        private System.Windows.Forms.Button Telem_Start_Stop_Button;
        private System.Windows.Forms.ToolStripMenuItem displayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGeofenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSearchAreaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showObstaclesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showPlaneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showWaypointsToolStripMenuItem;
        private System.Windows.Forms.TextBox SDA_ServerResponseTextBox;
        private System.Windows.Forms.Label SDA_ServerResponseLabel;
        private System.Windows.Forms.Button SDA_PollRateApply;
        private System.Windows.Forms.TextBox SDA_pollrateInput;
        private System.Windows.Forms.Label SDA_PollRate;
        private System.Windows.Forms.TabPage Map_Tab;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button Map_ApplyRefreshRate;
        private System.Windows.Forms.TextBox Map_RefreshRateInput;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox checkBox6;
        private System.Windows.Forms.CheckBox UASLoc_Checkbox;
        private System.Windows.Forms.CheckBox Waypoints_Checkbox;
        private System.Windows.Forms.CheckBox SearchArea_Checkbox;
        private System.Windows.Forms.CheckBox Obstacles_Checkbox;
        private System.Windows.Forms.CheckBox Geofence_Checkbox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TrackBar UAS_Trackbar;
        private System.Windows.Forms.CheckBox Fixed_UAS_Size_Checkbox;
        private System.Windows.Forms.Label UAS_GPS_Label;
        private System.Windows.Forms.Label UAS_Altitude_ASL_Label;
        private System.Windows.Forms.CheckBox AutoPan_Checkbox;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label UAS_D_Altitude_Label;
        private System.Windows.Forms.ToolTip Interop_Tooltip;
    }
}