namespace Interoperability_GUI
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
            this.error_label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // Save
            // 
            this.Save.Location = new System.Drawing.Point(250, 226);
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
            this.IP_ADDR_BOX.TextChanged += new System.EventHandler(this.IP_ADDR_BOX_TextChanged);
            // 
            // IP_ADDR_LABEL
            // 
            this.IP_ADDR_LABEL.AutoSize = true;
            this.IP_ADDR_LABEL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.IP_ADDR_LABEL.Location = new System.Drawing.Point(12, 36);
            this.IP_ADDR_LABEL.Name = "IP_ADDR_LABEL";
            this.IP_ADDR_LABEL.Size = new System.Drawing.Size(109, 13);
            this.IP_ADDR_LABEL.TabIndex = 2;
            this.IP_ADDR_LABEL.Text = "Server IP Address";
            // 
            // USERNAME_LABEL
            // 
            this.USERNAME_LABEL.AutoSize = true;
            this.USERNAME_LABEL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.USERNAME_LABEL.Location = new System.Drawing.Point(12, 69);
            this.USERNAME_LABEL.Name = "USERNAME_LABEL";
            this.USERNAME_LABEL.Size = new System.Drawing.Size(63, 13);
            this.USERNAME_LABEL.TabIndex = 3;
            this.USERNAME_LABEL.Text = "Username";
            // 
            // PASSWORD_LABEL
            // 
            this.PASSWORD_LABEL.AutoSize = true;
            this.PASSWORD_LABEL.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PASSWORD_LABEL.Location = new System.Drawing.Point(12, 105);
            this.PASSWORD_LABEL.Name = "PASSWORD_LABEL";
            this.PASSWORD_LABEL.Size = new System.Drawing.Size(61, 13);
            this.PASSWORD_LABEL.TabIndex = 4;
            this.PASSWORD_LABEL.Text = "Password";
            // 
            // USERNAME_BOX
            // 
            this.USERNAME_BOX.Location = new System.Drawing.Point(159, 62);
            this.USERNAME_BOX.Name = "USERNAME_BOX";
            this.USERNAME_BOX.Size = new System.Drawing.Size(166, 20);
            this.USERNAME_BOX.TabIndex = 5;
            this.USERNAME_BOX.TextChanged += new System.EventHandler(this.USERNAME_BOX_TextChanged);
            // 
            // PASSWORD_BOX
            // 
            this.PASSWORD_BOX.Location = new System.Drawing.Point(159, 98);
            this.PASSWORD_BOX.Name = "PASSWORD_BOX";
            this.PASSWORD_BOX.Size = new System.Drawing.Size(166, 20);
            this.PASSWORD_BOX.TabIndex = 6;
            this.PASSWORD_BOX.TextChanged += new System.EventHandler(this.PASSWORD_BOX_TextChanged);
            // 
            // Cancel
            // 
            this.Cancel.Location = new System.Drawing.Point(15, 226);
            this.Cancel.Name = "Cancel";
            this.Cancel.Size = new System.Drawing.Size(75, 23);
            this.Cancel.TabIndex = 7;
            this.Cancel.Text = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.Cancel_Click);
            // 
            // Verify
            // 
            this.Verify.Location = new System.Drawing.Point(225, 134);
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
            this.validation_label.ForeColor = System.Drawing.Color.DarkOliveGreen;
            this.validation_label.Location = new System.Drawing.Point(12, 157);
            this.validation_label.Name = "validation_label";
            this.validation_label.Size = new System.Drawing.Size(143, 13);
            this.validation_label.TabIndex = 9;
            this.validation_label.Text = "EXAMPLE SUCCESS CODE";
            this.validation_label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // error_label
            // 
            this.error_label.AutoSize = true;
            this.error_label.ForeColor = System.Drawing.Color.Red;
            this.error_label.Location = new System.Drawing.Point(12, 144);
            this.error_label.Name = "error_label";
            this.error_label.Size = new System.Drawing.Size(132, 13);
            this.error_label.TabIndex = 10;
            this.error_label.Text = "EXAMPLE ERROR CODE";
            this.error_label.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // Settings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(337, 261);
            this.Controls.Add(this.error_label);
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
            this.Name = "Settings";
            this.Text = "Settings";
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
        private System.Windows.Forms.Label error_label;
    }
}