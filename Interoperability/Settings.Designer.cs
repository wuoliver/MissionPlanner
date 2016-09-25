namespace Interoperability_GUI_Forms
{
    partial class Settings_GUI
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Settings_GUI));
            this.Save = new System.Windows.Forms.Button();
            this.IP_ADDR_BOX = new System.Windows.Forms.TextBox();
            this.IP_ADDR_LABEL = new System.Windows.Forms.Label();
            this.USERNAME_LABEL = new System.Windows.Forms.Label();
            this.PASSWORD_LABEL = new System.Windows.Forms.Label();
            this.USERNAME_BOX = new System.Windows.Forms.TextBox();
            this.PASSWORD_BOX = new System.Windows.Forms.TextBox();
            this.Cancel = new System.Windows.Forms.Button();
            this.Verify = new System.Windows.Forms.Button();
            this.validation_label = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.Airspeed_Units_Combo = new System.Windows.Forms.ComboBox();
            this.Distance_Units_Combo = new System.Windows.Forms.ComboBox();
            this.Coordinate_System_Combo = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.GUI_FORMAT_BOX = new System.Windows.Forms.ComboBox();
            this.ShowGUI_Checkbox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(255, 298);
            this.Save.Name = "Save";
            this.Save.Size = new System.Drawing.Size(75, 23);
            this.Save.TabIndex = 0;
            this.Save.Text = "Save";
            this.Save.UseVisualStyleBackColor = true;
            this.Save.Click += new System.EventHandler(this.Save_Click);
            // 
            // IP_ADDR_BOX
            // 
            this.IP_ADDR_BOX.Location = new System.Drawing.Point(159, 29);
            this.IP_ADDR_BOX.Name = "IP_ADDR_BOX";
            this.IP_ADDR_BOX.Size = new System.Drawing.Size(166, 20);
            this.IP_ADDR_BOX.TabIndex = 1;
            // 
            // IP_ADDR_LABEL
            // 
            this.IP_ADDR_LABEL.AutoSize = true;
            this.IP_ADDR_LABEL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IP_ADDR_LABEL.Location = new System.Drawing.Point(12, 32);
            this.IP_ADDR_LABEL.Name = "IP_ADDR_LABEL";
            this.IP_ADDR_LABEL.Size = new System.Drawing.Size(109, 13);
            this.IP_ADDR_LABEL.TabIndex = 2;
            this.IP_ADDR_LABEL.Text = "Server IP Address";
            // 
            // USERNAME_LABEL
            // 
            this.USERNAME_LABEL.AutoSize = true;
            this.USERNAME_LABEL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.USERNAME_LABEL.Location = new System.Drawing.Point(12, 58);
            this.USERNAME_LABEL.Name = "USERNAME_LABEL";
            this.USERNAME_LABEL.Size = new System.Drawing.Size(63, 13);
            this.USERNAME_LABEL.TabIndex = 3;
            this.USERNAME_LABEL.Text = "Username";
            // 
            // PASSWORD_LABEL
            // 
            this.PASSWORD_LABEL.AutoSize = true;
            this.PASSWORD_LABEL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PASSWORD_LABEL.Location = new System.Drawing.Point(12, 84);
            this.PASSWORD_LABEL.Name = "PASSWORD_LABEL";
            this.PASSWORD_LABEL.Size = new System.Drawing.Size(61, 13);
            this.PASSWORD_LABEL.TabIndex = 4;
            this.PASSWORD_LABEL.Text = "Password";
            // 
            // USERNAME_BOX
            // 
            this.USERNAME_BOX.Location = new System.Drawing.Point(159, 55);
            this.USERNAME_BOX.Name = "USERNAME_BOX";
            this.USERNAME_BOX.Size = new System.Drawing.Size(166, 20);
            this.USERNAME_BOX.TabIndex = 5;
            // 
            // PASSWORD_BOX
            // 
            this.PASSWORD_BOX.Location = new System.Drawing.Point(159, 81);
            this.PASSWORD_BOX.Name = "PASSWORD_BOX";
            this.PASSWORD_BOX.Size = new System.Drawing.Size(166, 20);
            this.PASSWORD_BOX.TabIndex = 6;
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(15, 298);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 7;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Verify
            // 
            this.Verify.Location = new System.Drawing.Point(225, 117);
            this.Verify.Name = "Verify";
            this.Verify.Size = new System.Drawing.Size(100, 23);
            this.Verify.TabIndex = 8;
            this.Verify.Text = "Verify Credentials";
            this.Verify.UseVisualStyleBackColor = true;
            this.Verify.Click += new System.EventHandler(this.Verify_Click);
            // 
            // validation_label
            // 
            this.validation_label.AutoSize = true;
            this.validation_label.ForeColor = System.Drawing.Color.DarkGreen;
            this.validation_label.Location = new System.Drawing.Point(13, 122);
            this.validation_label.Name = "validation_label";
            this.validation_label.Size = new System.Drawing.Size(108, 13);
            this.validation_label.TabIndex = 9;
            this.validation_label.Text = "Example Status Code";
            this.validation_label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 151);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(93, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Airspeed Units:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 178);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Distance Units:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 205);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(116, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Coordinate System:";
            // 
            // Airspeed_Units_Combo
            // 
            this.Airspeed_Units_Combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Airspeed_Units_Combo.FormattingEnabled = true;
            this.Airspeed_Units_Combo.Items.AddRange(new object[] {
            "Metres per Second",
            "Feet per Second",
            "KPH",
            "MPH",
            "Knots"});
            this.Airspeed_Units_Combo.Location = new System.Drawing.Point(159, 148);
            this.Airspeed_Units_Combo.Name = "Airspeed_Units_Combo";
            this.Airspeed_Units_Combo.Size = new System.Drawing.Size(166, 21);
            this.Airspeed_Units_Combo.TabIndex = 14;
            // 
            // Distance_Units_Combo
            // 
            this.Distance_Units_Combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Distance_Units_Combo.FormattingEnabled = true;
            this.Distance_Units_Combo.Items.AddRange(new object[] {
            "Metres",
            "Feet"});
            this.Distance_Units_Combo.Location = new System.Drawing.Point(159, 175);
            this.Distance_Units_Combo.Name = "Distance_Units_Combo";
            this.Distance_Units_Combo.Size = new System.Drawing.Size(166, 21);
            this.Distance_Units_Combo.TabIndex = 15;
            // 
            // Coordinate_System_Combo
            // 
            this.Coordinate_System_Combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Coordinate_System_Combo.FormattingEnabled = true;
            this.Coordinate_System_Combo.Items.AddRange(new object[] {
            "DD.DDDDDD",
            "DD MM SS.SS"});
            this.Coordinate_System_Combo.Location = new System.Drawing.Point(159, 202);
            this.Coordinate_System_Combo.Name = "Coordinate_System_Combo";
            this.Coordinate_System_Combo.Size = new System.Drawing.Size(166, 21);
            this.Coordinate_System_Combo.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(13, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(317, 13);
            this.label4.TabIndex = 17;
            this.label4.Text = "Warning: clicking save will reset all open connections ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 232);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(103, 13);
            this.label5.TabIndex = 18;
            this.label5.Text = "Competition GUI:";
            // 
            // GUI_FORMAT_BOX
            // 
            this.GUI_FORMAT_BOX.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.GUI_FORMAT_BOX.FormattingEnabled = true;
            this.GUI_FORMAT_BOX.Items.AddRange(new object[] {
            "USC",
            "AUVSI"});
            this.GUI_FORMAT_BOX.Location = new System.Drawing.Point(159, 229);
            this.GUI_FORMAT_BOX.Name = "GUI_FORMAT_BOX";
            this.GUI_FORMAT_BOX.Size = new System.Drawing.Size(166, 21);
            this.GUI_FORMAT_BOX.TabIndex = 19;
            // 
            // ShowGUI_Checkbox
            // 
            this.ShowGUI_Checkbox.AutoSize = true;
            this.ShowGUI_Checkbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShowGUI_Checkbox.Location = new System.Drawing.Point(16, 266);
            this.ShowGUI_Checkbox.Name = "ShowGUI_Checkbox";
            this.ShowGUI_Checkbox.Size = new System.Drawing.Size(200, 17);
            this.ShowGUI_Checkbox.TabIndex = 21;
            this.ShowGUI_Checkbox.Text = "Show Control Panel on Startup";
            this.ShowGUI_Checkbox.UseVisualStyleBackColor = true;
            // 
            // Settings_GUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(342, 332);
            this.Controls.Add(this.ShowGUI_Checkbox);
            this.Controls.Add(this.GUI_FORMAT_BOX);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Coordinate_System_Combo);
            this.Controls.Add(this.Distance_Units_Combo);
            this.Controls.Add(this.Airspeed_Units_Combo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.validation_label);
            this.Controls.Add(this.Verify);
            this.Controls.Add(this.Cancel);
            this.Controls.Add(this.PASSWORD_BOX);
            this.Controls.Add(this.USERNAME_BOX);
            this.Controls.Add(this.PASSWORD_LABEL);
            this.Controls.Add(this.USERNAME_LABEL);
            this.Controls.Add(this.IP_ADDR_LABEL);
            this.Controls.Add(this.IP_ADDR_BOX);
            this.Controls.Add(this.Save);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Settings_GUI";
            this.Text = "Settings";
            this.TopMost = true;
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Settings_FormClosed);
            this.Load += new System.EventHandler(this.Settings_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Save;
        private System.Windows.Forms.TextBox IP_ADDR_BOX;
        private System.Windows.Forms.Label IP_ADDR_LABEL;
        private System.Windows.Forms.Label USERNAME_LABEL;
        private System.Windows.Forms.Label PASSWORD_LABEL;
        private System.Windows.Forms.TextBox USERNAME_BOX;
        private System.Windows.Forms.TextBox PASSWORD_BOX;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.Button Verify;
        private System.Windows.Forms.Label validation_label;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox Airspeed_Units_Combo;
        private System.Windows.Forms.ComboBox Distance_Units_Combo;
        private System.Windows.Forms.ComboBox Coordinate_System_Combo;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox GUI_FORMAT_BOX;
        private System.Windows.Forms.CheckBox ShowGUI_Checkbox;
    }
}