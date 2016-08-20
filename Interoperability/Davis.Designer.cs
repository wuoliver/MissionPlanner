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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.uniqueTelUploadText = new System.Windows.Forms.TextBox();
            this.avgTelUploadText = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.pollRateInput = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.Server_Settings = new System.Windows.Forms.Button();
            this.applyPollRateButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(174, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Unique telemetry upload rate:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(3, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(181, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Average telemetry upload rate:";
            // 
            // uniqueTelUploadText
            // 
            this.uniqueTelUploadText.Location = new System.Drawing.Point(190, 6);
            this.uniqueTelUploadText.Name = "uniqueTelUploadText";
            this.uniqueTelUploadText.ReadOnly = true;
            this.uniqueTelUploadText.Size = new System.Drawing.Size(237, 20);
            this.uniqueTelUploadText.TabIndex = 2;
            this.uniqueTelUploadText.Text = "Hz";
            // 
            // avgTelUploadText
            // 
            this.avgTelUploadText.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.avgTelUploadText.Location = new System.Drawing.Point(190, 35);
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
            this.label3.Location = new System.Drawing.Point(3, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Poll Rate:";
            // 
            // pollRateInput
            // 
            this.pollRateInput.Location = new System.Drawing.Point(190, 63);
            this.pollRateInput.Name = "pollRateInput";
            this.pollRateInput.Size = new System.Drawing.Size(156, 20);
            this.pollRateInput.TabIndex = 5;
            this.pollRateInput.Text = "10";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Server_Settings);
            this.panel1.Controls.Add(this.applyPollRateButton);
            this.panel1.Controls.Add(this.pollRateInput);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.avgTelUploadText);
            this.panel1.Controls.Add(this.uniqueTelUploadText);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(12, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(430, 136);
            this.panel1.TabIndex = 3;
            // 
            // Server_Settings
            // 
            this.Server_Settings.Location = new System.Drawing.Point(6, 110);
            this.Server_Settings.Name = "Server_Settings";
            this.Server_Settings.Size = new System.Drawing.Size(131, 23);
            this.Server_Settings.TabIndex = 7;
            this.Server_Settings.Text = "Sever Settings";
            this.Server_Settings.UseVisualStyleBackColor = true;
            this.Server_Settings.Click += new System.EventHandler(this.Server_Settings_Click);
            // 
            // applyPollRateButton
            // 
            this.applyPollRateButton.Location = new System.Drawing.Point(352, 61);
            this.applyPollRateButton.Name = "applyPollRateButton";
            this.applyPollRateButton.Size = new System.Drawing.Size(75, 23);
            this.applyPollRateButton.TabIndex = 6;
            this.applyPollRateButton.Text = "Apply";
            this.applyPollRateButton.UseVisualStyleBackColor = true;
            this.applyPollRateButton.Click += new System.EventHandler(this.applyPollRateButton_Click);
            // 
            // Interoperability
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(455, 163);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.Name = "Interoperability";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Davis";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox uniqueTelUploadText;
        private System.Windows.Forms.TextBox avgTelUploadText;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox pollRateInput;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button applyPollRateButton;
        private System.Windows.Forms.Button Server_Settings;
    }
}