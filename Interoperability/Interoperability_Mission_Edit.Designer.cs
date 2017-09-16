namespace interoperability
{
    partial class Interoperability_Mission_Edit
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Interoperability_Mission_Edit));
            this.Save_Button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.Mission_Name_Textbox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.FlyZone_Textbox = new System.Windows.Forms.TextBox();
            this.Search_Area_Textbox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.Off_Axis_Target_Textbox = new System.Windows.Forms.TextBox();
            this.Emergent_Target_Textbox = new System.Windows.Forms.TextBox();
            this.Airdrop_Textbox = new System.Windows.Forms.TextBox();
            this.Waypoint_Textbox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.Refresh_Map_Button = new System.Windows.Forms.Button();
            this.FlyZone_Select_Combobox = new System.Windows.Forms.ComboBox();
            this.FlyZone_Select_Rename = new System.Windows.Forms.Button();
            this.FlyZone_Delete_Button = new System.Windows.Forms.Button();
            this.Max_Alt_MSL_Box = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.Min_Alt_MSL_Box = new System.Windows.Forms.NumericUpDown();
            this.Border_Colour_Button = new System.Windows.Forms.Button();
            this.Fill_Colour_Button = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            ((System.ComponentModel.ISupportInitialize)(this.Max_Alt_MSL_Box)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Min_Alt_MSL_Box)).BeginInit();
            this.SuspendLayout();
            // 
            // Save_Button
            // 
            this.Save_Button.Location = new System.Drawing.Point(233, 579);
            this.Save_Button.Name = "Save_Button";
            this.Save_Button.Size = new System.Drawing.Size(75, 23);
            this.Save_Button.TabIndex = 9;
            this.Save_Button.Text = "Save";
            this.Save_Button.UseVisualStyleBackColor = true;
            this.Save_Button.Click += new System.EventHandler(this.Save_Button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Mission Name:";
            // 
            // Mission_Name_Textbox
            // 
            this.Mission_Name_Textbox.Location = new System.Drawing.Point(15, 26);
            this.Mission_Name_Textbox.Name = "Mission_Name_Textbox";
            this.Mission_Name_Textbox.Size = new System.Drawing.Size(293, 20);
            this.Mission_Name_Textbox.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Fly Zones";
            // 
            // FlyZone_Textbox
            // 
            this.FlyZone_Textbox.Location = new System.Drawing.Point(15, 81);
            this.FlyZone_Textbox.MaxLength = 500000;
            this.FlyZone_Textbox.Multiline = true;
            this.FlyZone_Textbox.Name = "FlyZone_Textbox";
            this.FlyZone_Textbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.FlyZone_Textbox.Size = new System.Drawing.Size(293, 89);
            this.FlyZone_Textbox.TabIndex = 13;
            // 
            // Search_Area_Textbox
            // 
            this.Search_Area_Textbox.AcceptsReturn = true;
            this.Search_Area_Textbox.Location = new System.Drawing.Point(15, 242);
            this.Search_Area_Textbox.Multiline = true;
            this.Search_Area_Textbox.Name = "Search_Area_Textbox";
            this.Search_Area_Textbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Search_Area_Textbox.Size = new System.Drawing.Size(293, 89);
            this.Search_Area_Textbox.TabIndex = 14;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 226);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "Search Area";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 338);
            this.label4.Name = "label4";
            this.label4.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label4.Size = new System.Drawing.Size(112, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Mission Waypoints";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 451);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(106, 13);
            this.label5.TabIndex = 17;
            this.label5.Text = "Air Drop Location";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(12, 490);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Emergent Target";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(12, 529);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(92, 13);
            this.label7.TabIndex = 19;
            this.label7.Text = "Off Axis Target";
            // 
            // Off_Axis_Target_Textbox
            // 
            this.Off_Axis_Target_Textbox.Location = new System.Drawing.Point(15, 545);
            this.Off_Axis_Target_Textbox.Name = "Off_Axis_Target_Textbox";
            this.Off_Axis_Target_Textbox.Size = new System.Drawing.Size(293, 20);
            this.Off_Axis_Target_Textbox.TabIndex = 20;
            // 
            // Emergent_Target_Textbox
            // 
            this.Emergent_Target_Textbox.Location = new System.Drawing.Point(15, 506);
            this.Emergent_Target_Textbox.Name = "Emergent_Target_Textbox";
            this.Emergent_Target_Textbox.Size = new System.Drawing.Size(293, 20);
            this.Emergent_Target_Textbox.TabIndex = 21;
            // 
            // Airdrop_Textbox
            // 
            this.Airdrop_Textbox.Location = new System.Drawing.Point(15, 467);
            this.Airdrop_Textbox.Name = "Airdrop_Textbox";
            this.Airdrop_Textbox.Size = new System.Drawing.Size(293, 20);
            this.Airdrop_Textbox.TabIndex = 22;
            // 
            // Waypoint_Textbox
            // 
            this.Waypoint_Textbox.Location = new System.Drawing.Point(15, 354);
            this.Waypoint_Textbox.Multiline = true;
            this.Waypoint_Textbox.Name = "Waypoint_Textbox";
            this.Waypoint_Textbox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Waypoint_Textbox.Size = new System.Drawing.Size(293, 89);
            this.Waypoint_Textbox.TabIndex = 23;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label8.Location = new System.Drawing.Point(573, 584);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(340, 26);
            this.label8.TabIndex = 24;
            this.label8.Text = "Input Format:  N38-08-32.06 W076-34-28.01 or 38.142400 -76.425511\r\n\r\n";
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Location = new System.Drawing.Point(15, 579);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_Button.TabIndex = 25;
            this.Cancel_Button.Text = "Cancel";
            this.Cancel_Button.UseVisualStyleBackColor = true;
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // gMapControl1
            // 
            this.gMapControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gMapControl1.AutoSize = true;
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(314, 26);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 24;
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
            this.gMapControl1.Size = new System.Drawing.Size(856, 547);
            this.gMapControl1.TabIndex = 26;
            this.gMapControl1.Zoom = 0D;
            this.gMapControl1.Load += new System.EventHandler(this.gMapControl1_Load);
            // 
            // Refresh_Map_Button
            // 
            this.Refresh_Map_Button.Location = new System.Drawing.Point(1092, 579);
            this.Refresh_Map_Button.Name = "Refresh_Map_Button";
            this.Refresh_Map_Button.Size = new System.Drawing.Size(78, 23);
            this.Refresh_Map_Button.TabIndex = 27;
            this.Refresh_Map_Button.Text = "Refresh Map";
            this.Refresh_Map_Button.UseVisualStyleBackColor = true;
            this.Refresh_Map_Button.Click += new System.EventHandler(this.Refresh_Map_Button_Click);
            // 
            // FlyZone_Select_Combobox
            // 
            this.FlyZone_Select_Combobox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.FlyZone_Select_Combobox.FormattingEnabled = true;
            this.FlyZone_Select_Combobox.Items.AddRange(new object[] {
            "Add New..."});
            this.FlyZone_Select_Combobox.Location = new System.Drawing.Point(80, 54);
            this.FlyZone_Select_Combobox.Name = "FlyZone_Select_Combobox";
            this.FlyZone_Select_Combobox.Size = new System.Drawing.Size(111, 21);
            this.FlyZone_Select_Combobox.TabIndex = 28;
            this.FlyZone_Select_Combobox.SelectedIndexChanged += new System.EventHandler(this.Geofence_Select_Combobox_SelectedIndexChanged);
            // 
            // FlyZone_Select_Rename
            // 
            this.FlyZone_Select_Rename.Location = new System.Drawing.Point(253, 52);
            this.FlyZone_Select_Rename.Name = "FlyZone_Select_Rename";
            this.FlyZone_Select_Rename.Size = new System.Drawing.Size(55, 23);
            this.FlyZone_Select_Rename.TabIndex = 29;
            this.FlyZone_Select_Rename.Text = "Rename";
            this.FlyZone_Select_Rename.UseVisualStyleBackColor = true;
            this.FlyZone_Select_Rename.Click += new System.EventHandler(this.Geofence_Select_Rename_Click);
            // 
            // FlyZone_Delete_Button
            // 
            this.FlyZone_Delete_Button.Location = new System.Drawing.Point(197, 52);
            this.FlyZone_Delete_Button.Name = "FlyZone_Delete_Button";
            this.FlyZone_Delete_Button.Size = new System.Drawing.Size(55, 23);
            this.FlyZone_Delete_Button.TabIndex = 30;
            this.FlyZone_Delete_Button.Text = "Delete";
            this.FlyZone_Delete_Button.UseVisualStyleBackColor = true;
            this.FlyZone_Delete_Button.Click += new System.EventHandler(this.FlyZone_Delete_Button_Click);
            // 
            // Max_Alt_MSL_Box
            // 
            this.Max_Alt_MSL_Box.Location = new System.Drawing.Point(91, 176);
            this.Max_Alt_MSL_Box.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.Max_Alt_MSL_Box.Name = "Max_Alt_MSL_Box";
            this.Max_Alt_MSL_Box.Size = new System.Drawing.Size(63, 20);
            this.Max_Alt_MSL_Box.TabIndex = 31;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(15, 178);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(70, 13);
            this.label9.TabIndex = 32;
            this.label9.Text = "Max Alt. MSL";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(15, 202);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(67, 13);
            this.label10.TabIndex = 33;
            this.label10.Text = "Min Alt. MSL";
            // 
            // Min_Alt_MSL_Box
            // 
            this.Min_Alt_MSL_Box.Location = new System.Drawing.Point(91, 202);
            this.Min_Alt_MSL_Box.Maximum = new decimal(new int[] {
            2000,
            0,
            0,
            0});
            this.Min_Alt_MSL_Box.Name = "Min_Alt_MSL_Box";
            this.Min_Alt_MSL_Box.Size = new System.Drawing.Size(63, 20);
            this.Min_Alt_MSL_Box.TabIndex = 34;
            // 
            // Border_Colour_Button
            // 
            this.Border_Colour_Button.BackColor = System.Drawing.Color.Red;
            this.Border_Colour_Button.Location = new System.Drawing.Point(197, 176);
            this.Border_Colour_Button.Name = "Border_Colour_Button";
            this.Border_Colour_Button.Size = new System.Drawing.Size(111, 21);
            this.Border_Colour_Button.TabIndex = 35;
            this.Border_Colour_Button.Text = "Border Colour";
            this.Border_Colour_Button.UseVisualStyleBackColor = false;
            this.Border_Colour_Button.Click += new System.EventHandler(this.Border_Button_Click);
            // 
            // Fill_Colour_Button
            // 
            this.Fill_Colour_Button.BackColor = System.Drawing.Color.White;
            this.Fill_Colour_Button.Location = new System.Drawing.Point(197, 202);
            this.Fill_Colour_Button.Name = "Fill_Colour_Button";
            this.Fill_Colour_Button.Size = new System.Drawing.Size(111, 21);
            this.Fill_Colour_Button.TabIndex = 36;
            this.Fill_Colour_Button.Text = "Fill Colour";
            this.Fill_Colour_Button.UseVisualStyleBackColor = false;
            this.Fill_Colour_Button.Click += new System.EventHandler(this.Fill_Colour_Button_Click);
            // 
            // Interoperability_Mission_Edit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1182, 614);
            this.Controls.Add(this.Fill_Colour_Button);
            this.Controls.Add(this.Border_Colour_Button);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.Min_Alt_MSL_Box);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.Max_Alt_MSL_Box);
            this.Controls.Add(this.FlyZone_Delete_Button);
            this.Controls.Add(this.FlyZone_Select_Rename);
            this.Controls.Add(this.Waypoint_Textbox);
            this.Controls.Add(this.Search_Area_Textbox);
            this.Controls.Add(this.FlyZone_Select_Combobox);
            this.Controls.Add(this.Refresh_Map_Button);
            this.Controls.Add(this.gMapControl1);
            this.Controls.Add(this.Cancel_Button);
            this.Controls.Add(this.Airdrop_Textbox);
            this.Controls.Add(this.Emergent_Target_Textbox);
            this.Controls.Add(this.Off_Axis_Target_Textbox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.FlyZone_Textbox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.Mission_Name_Textbox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.Save_Button);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Interoperability_Mission_Edit";
            this.Text = "Edit Mission";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Interoperability_Mission_Import_FormClosed);
            this.Shown += new System.EventHandler(this.Interoperability_Mission_Import_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.Max_Alt_MSL_Box)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Min_Alt_MSL_Box)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button Save_Button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox Mission_Name_Textbox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox FlyZone_Textbox;
        private System.Windows.Forms.TextBox Search_Area_Textbox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox Off_Axis_Target_Textbox;
        private System.Windows.Forms.TextBox Emergent_Target_Textbox;
        private System.Windows.Forms.TextBox Airdrop_Textbox;
        private System.Windows.Forms.TextBox Waypoint_Textbox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button Cancel_Button;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.Button Refresh_Map_Button;
        private System.Windows.Forms.ComboBox FlyZone_Select_Combobox;
        private System.Windows.Forms.Button FlyZone_Select_Rename;
        private System.Windows.Forms.Button FlyZone_Delete_Button;
        private System.Windows.Forms.NumericUpDown Max_Alt_MSL_Box;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.NumericUpDown Min_Alt_MSL_Box;
        private System.Windows.Forms.Button Border_Colour_Button;
        private System.Windows.Forms.Button Fill_Colour_Button;
        private System.Windows.Forms.ColorDialog colorDialog1;
    }
}