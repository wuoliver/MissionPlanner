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
            this.pollRateInput = new System.Windows.Forms.TextBox();
            this.Server_Settings = new System.Windows.Forms.Button();
            this.applyPollRateButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.Interoperability_GUI_Tab = new System.Windows.Forms.TabControl();
            this.Telem_Tab = new System.Windows.Forms.TabPage();
            this.Start_Stop_Button = new System.Windows.Forms.Button();
            this.TelemServerResp = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.Total_Telem_Rate = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Reset_Stats = new System.Windows.Forms.Button();
            this.SDA_Tab = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.SDA_Obstacles = new System.Windows.Forms.TextBox();
            this.SDA_Test_Button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.Map_Tab = new System.Windows.Forms.TabPage();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.Map_ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.displayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showGeofenceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSearchAreaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showObstaclesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showPlaneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showWaypointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Mission_Info_Tab = new System.Windows.Forms.TabPage();
            this.Mission_Enable = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.TargetUpload_Tab = new System.Windows.Forms.TabPage();
            this.Interoperability_GUI_Tab.SuspendLayout();
            this.Telem_Tab.SuspendLayout();
            this.SDA_Tab.SuspendLayout();
            this.Map_Tab.SuspendLayout();
            this.Map_ContextMenuStrip.SuspendLayout();
            this.Mission_Info_Tab.SuspendLayout();
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
            this.label3.Location = new System.Drawing.Point(22, 175);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Poll Rate:";
            // 
            // pollRateInput
            // 
            this.pollRateInput.Location = new System.Drawing.Point(209, 172);
            this.pollRateInput.Name = "pollRateInput";
            this.pollRateInput.Size = new System.Drawing.Size(156, 20);
            this.pollRateInput.TabIndex = 5;
            this.pollRateInput.Text = "10";
            // 
            // Server_Settings
            // 
            this.Server_Settings.Location = new System.Drawing.Point(1032, 12);
            this.Server_Settings.Name = "Server_Settings";
            this.Server_Settings.Size = new System.Drawing.Size(131, 23);
            this.Server_Settings.TabIndex = 7;
            this.Server_Settings.Text = "Sever Settings";
            this.Server_Settings.UseVisualStyleBackColor = true;
            this.Server_Settings.Click += new System.EventHandler(this.Server_Settings_Click);
            // 
            // applyPollRateButton
            // 
            this.applyPollRateButton.Location = new System.Drawing.Point(371, 170);
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
            this.label4.Location = new System.Drawing.Point(218, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(228, 31);
            this.label4.TabIndex = 4;
            this.label4.Text = "Telemetry Upload";
            // 
            // Interoperability_GUI_Tab
            // 
            this.Interoperability_GUI_Tab.Controls.Add(this.Telem_Tab);
            this.Interoperability_GUI_Tab.Controls.Add(this.SDA_Tab);
            this.Interoperability_GUI_Tab.Controls.Add(this.Map_Tab);
            this.Interoperability_GUI_Tab.Controls.Add(this.Mission_Info_Tab);
            this.Interoperability_GUI_Tab.Controls.Add(this.TargetUpload_Tab);
            this.Interoperability_GUI_Tab.Location = new System.Drawing.Point(12, 22);
            this.Interoperability_GUI_Tab.Name = "Interoperability_GUI_Tab";
            this.Interoperability_GUI_Tab.SelectedIndex = 0;
            this.Interoperability_GUI_Tab.Size = new System.Drawing.Size(1151, 562);
            this.Interoperability_GUI_Tab.TabIndex = 8;
            // 
            // Telem_Tab
            // 
            this.Telem_Tab.Controls.Add(this.Start_Stop_Button);
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
            this.Telem_Tab.Controls.Add(this.pollRateInput);
            this.Telem_Tab.Location = new System.Drawing.Point(4, 22);
            this.Telem_Tab.Name = "Telem_Tab";
            this.Telem_Tab.Padding = new System.Windows.Forms.Padding(3);
            this.Telem_Tab.Size = new System.Drawing.Size(1143, 536);
            this.Telem_Tab.TabIndex = 0;
            this.Telem_Tab.Text = "Telemetry ";
            this.Telem_Tab.UseVisualStyleBackColor = true;
            // 
            // Start_Stop_Button
            // 
            this.Start_Stop_Button.Location = new System.Drawing.Point(209, 247);
            this.Start_Stop_Button.Name = "Start_Stop_Button";
            this.Start_Stop_Button.Size = new System.Drawing.Size(75, 23);
            this.Start_Stop_Button.TabIndex = 12;
            this.Start_Stop_Button.Text = "Start";
            this.Start_Stop_Button.UseVisualStyleBackColor = true;
            this.Start_Stop_Button.Click += new System.EventHandler(this.Start_Stop_Button_Click);
            // 
            // TelemServerResp
            // 
            this.TelemServerResp.Location = new System.Drawing.Point(209, 211);
            this.TelemServerResp.Name = "TelemServerResp";
            this.TelemServerResp.ReadOnly = true;
            this.TelemServerResp.Size = new System.Drawing.Size(237, 20);
            this.TelemServerResp.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(22, 219);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(108, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "Server Response:";
            // 
            // Total_Telem_Rate
            // 
            this.Total_Telem_Rate.Location = new System.Drawing.Point(209, 135);
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
            this.label6.Location = new System.Drawing.Point(22, 135);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(173, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "Total telemetry upload count:";
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
            // SDA_Tab
            // 
            this.SDA_Tab.Controls.Add(this.label9);
            this.SDA_Tab.Controls.Add(this.SDA_Obstacles);
            this.SDA_Tab.Controls.Add(this.SDA_Test_Button);
            this.SDA_Tab.Controls.Add(this.label5);
            this.SDA_Tab.Location = new System.Drawing.Point(4, 22);
            this.SDA_Tab.Name = "SDA_Tab";
            this.SDA_Tab.Padding = new System.Windows.Forms.Padding(3);
            this.SDA_Tab.Size = new System.Drawing.Size(1143, 536);
            this.SDA_Tab.TabIndex = 1;
            this.SDA_Tab.Text = "SDA";
            this.SDA_Tab.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(14, 112);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(124, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Obstacle Information";
            // 
            // SDA_Obstacles
            // 
            this.SDA_Obstacles.Location = new System.Drawing.Point(17, 140);
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
            // Map_Tab
            // 
            this.Map_Tab.Controls.Add(this.gMapControl1);
            this.Map_Tab.Location = new System.Drawing.Point(4, 22);
            this.Map_Tab.Name = "Map_Tab";
            this.Map_Tab.Size = new System.Drawing.Size(1143, 536);
            this.Map_Tab.TabIndex = 4;
            this.Map_Tab.Text = "Map";
            this.Map_Tab.UseVisualStyleBackColor = true;
            // 
            // gMapControl1
            // 
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.ContextMenuStrip = this.Map_ContextMenuStrip;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(17, 16);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 20;
            this.gMapControl1.MinZoom = 4;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionWithoutCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(1102, 468);
            this.gMapControl1.TabIndex = 0;
            this.gMapControl1.Zoom = 0D;
            this.gMapControl1.Load += new System.EventHandler(this.gMapControl1_Load);
            // 
            // Map_ContextMenuStrip
            // 
            this.Map_ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.displayToolStripMenuItem});
            this.Map_ContextMenuStrip.Name = "contextMenuStrip1";
            this.Map_ContextMenuStrip.Size = new System.Drawing.Size(153, 48);
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
            this.displayToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
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
            // Mission_Info_Tab
            // 
            this.Mission_Info_Tab.Controls.Add(this.Mission_Enable);
            this.Mission_Info_Tab.Controls.Add(this.label8);
            this.Mission_Info_Tab.Location = new System.Drawing.Point(4, 22);
            this.Mission_Info_Tab.Name = "Mission_Info_Tab";
            this.Mission_Info_Tab.Size = new System.Drawing.Size(1143, 536);
            this.Mission_Info_Tab.TabIndex = 3;
            this.Mission_Info_Tab.Text = "Mission Info";
            this.Mission_Info_Tab.UseVisualStyleBackColor = true;
            // 
            // Mission_Enable
            // 
            this.Mission_Enable.Location = new System.Drawing.Point(299, 109);
            this.Mission_Enable.Name = "Mission_Enable";
            this.Mission_Enable.Size = new System.Drawing.Size(163, 23);
            this.Mission_Enable.TabIndex = 1;
            this.Mission_Enable.Text = "Do The Other Thing!!";
            this.Mission_Enable.UseVisualStyleBackColor = true;
            this.Mission_Enable.Click += new System.EventHandler(this.Mission_Enable_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(293, 43);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(171, 31);
            this.label8.TabIndex = 0;
            this.label8.Text = "Mission Info";
            // 
            // TargetUpload_Tab
            // 
            this.TargetUpload_Tab.Location = new System.Drawing.Point(4, 22);
            this.TargetUpload_Tab.Name = "TargetUpload_Tab";
            this.TargetUpload_Tab.Padding = new System.Windows.Forms.Padding(3);
            this.TargetUpload_Tab.Size = new System.Drawing.Size(1143, 536);
            this.TargetUpload_Tab.TabIndex = 2;
            this.TargetUpload_Tab.Text = "Image Upload";
            this.TargetUpload_Tab.UseVisualStyleBackColor = true;
            // 
            // Interoperability_GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1178, 596);
            this.Controls.Add(this.Server_Settings);
            this.Controls.Add(this.Interoperability_GUI_Tab);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Interoperability_GUI";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "UTAT UAV Interoperability Control Panel";
            this.Interoperability_GUI_Tab.ResumeLayout(false);
            this.Telem_Tab.ResumeLayout(false);
            this.Telem_Tab.PerformLayout();
            this.SDA_Tab.ResumeLayout(false);
            this.SDA_Tab.PerformLayout();
            this.Map_Tab.ResumeLayout(false);
            this.Map_ContextMenuStrip.ResumeLayout(false);
            this.Mission_Info_Tab.ResumeLayout(false);
            this.Mission_Info_Tab.PerformLayout();
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
        private System.Windows.Forms.TabPage Telem_Tab;
        private System.Windows.Forms.TabPage SDA_Tab;
        private System.Windows.Forms.Button Reset_Stats;
        private System.Windows.Forms.TabPage TargetUpload_Tab;
        private System.Windows.Forms.TabPage Mission_Info_Tab;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button SDA_Test_Button;
        private System.Windows.Forms.TextBox SDA_Obstacles;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox Total_Telem_Rate;
        private System.Windows.Forms.TextBox TelemServerResp;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button Mission_Enable;
        private System.Windows.Forms.TabPage Map_Tab;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ContextMenuStrip Map_ContextMenuStrip;
        private System.Windows.Forms.Button Start_Stop_Button;
        private System.Windows.Forms.ToolStripMenuItem displayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showGeofenceToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSearchAreaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showObstaclesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showPlaneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showWaypointsToolStripMenuItem;
    }
}