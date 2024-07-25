namespace UDP_Repeater_GUI
{
    partial class InputForm
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
            this.dialogOkButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.profileDropdown = new System.Windows.Forms.ComboBox();
            this.ipTextbox = new System.Windows.Forms.TextBox();
            this.ip_label = new System.Windows.Forms.Label();
            this.portTextbox = new System.Windows.Forms.TextBox();
            this.port_label = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.restoreToDefaultsButton = new System.Windows.Forms.Button();
            this.reconfigSetupButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dialogOkButton
            // 
            this.dialogOkButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.dialogOkButton.Location = new System.Drawing.Point(341, 202);
            this.dialogOkButton.Name = "dialogOkButton";
            this.dialogOkButton.Size = new System.Drawing.Size(100, 38);
            this.dialogOkButton.TabIndex = 1;
            this.dialogOkButton.Text = "Ok";
            this.dialogOkButton.UseVisualStyleBackColor = true;
            this.dialogOkButton.Click += new System.EventHandler(this.dialogOkButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(151, 21);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(169, 29);
            this.label3.TabIndex = 15;
            this.label3.Text = "Configuration";
            // 
            // profileDropdown
            // 
            this.profileDropdown.FormattingEnabled = true;
            this.profileDropdown.Items.AddRange(new object[] {
            "Receiving From",
            "Sending To",
            "Default Receive",
            "Default Send"});
            this.profileDropdown.Location = new System.Drawing.Point(323, 124);
            this.profileDropdown.Name = "profileDropdown";
            this.profileDropdown.Size = new System.Drawing.Size(123, 21);
            this.profileDropdown.TabIndex = 17;
            // 
            // ipTextbox
            // 
            this.ipTextbox.Location = new System.Drawing.Point(45, 124);
            this.ipTextbox.Name = "ipTextbox";
            this.ipTextbox.Size = new System.Drawing.Size(100, 20);
            this.ipTextbox.TabIndex = 18;
            // 
            // ip_label
            // 
            this.ip_label.AutoSize = true;
            this.ip_label.Location = new System.Drawing.Point(68, 108);
            this.ip_label.Name = "ip_label";
            this.ip_label.Size = new System.Drawing.Size(58, 13);
            this.ip_label.TabIndex = 19;
            this.ip_label.Text = "IP Address";
            // 
            // portTextbox
            // 
            this.portTextbox.Location = new System.Drawing.Point(186, 124);
            this.portTextbox.Name = "portTextbox";
            this.portTextbox.Size = new System.Drawing.Size(100, 20);
            this.portTextbox.TabIndex = 20;
            // 
            // port_label
            // 
            this.port_label.AutoSize = true;
            this.port_label.Location = new System.Drawing.Point(220, 108);
            this.port_label.Name = "port_label";
            this.port_label.Size = new System.Drawing.Size(26, 13);
            this.port_label.TabIndex = 21;
            this.port_label.Text = "Port";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(365, 108);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(36, 13);
            this.label4.TabIndex = 22;
            this.label4.Text = "Profile";
            // 
            // restoreToDefaultsButton
            // 
            this.restoreToDefaultsButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.restoreToDefaultsButton.Location = new System.Drawing.Point(186, 202);
            this.restoreToDefaultsButton.Name = "restoreToDefaultsButton";
            this.restoreToDefaultsButton.Size = new System.Drawing.Size(100, 38);
            this.restoreToDefaultsButton.TabIndex = 23;
            this.restoreToDefaultsButton.Text = "Restore to Default Settings";
            this.restoreToDefaultsButton.UseVisualStyleBackColor = true;
            this.restoreToDefaultsButton.Click += new System.EventHandler(this.defaultButton_Click);
            // 
            // reconfigSetupButton
            // 
            this.reconfigSetupButton.DialogResult = System.Windows.Forms.DialogResult.Abort;
            this.reconfigSetupButton.Location = new System.Drawing.Point(45, 202);
            this.reconfigSetupButton.Name = "reconfigSetupButton";
            this.reconfigSetupButton.Size = new System.Drawing.Size(100, 38);
            this.reconfigSetupButton.TabIndex = 24;
            this.reconfigSetupButton.Text = "Reconfigure NIC \r\nor Monitoring ";
            this.reconfigSetupButton.UseVisualStyleBackColor = true;
            this.reconfigSetupButton.Click += new System.EventHandler(this.reconfigNicButton_Click);
            // 
            // InputForm
            // 
            this.AcceptButton = this.dialogOkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.dialogOkButton;
            this.ClientSize = new System.Drawing.Size(477, 263);
            this.Controls.Add(this.reconfigSetupButton);
            this.Controls.Add(this.restoreToDefaultsButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.port_label);
            this.Controls.Add(this.portTextbox);
            this.Controls.Add(this.ip_label);
            this.Controls.Add(this.ipTextbox);
            this.Controls.Add(this.profileDropdown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dialogOkButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.Name = "InputForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.configDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button dialogOkButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox profileDropdown;
        private System.Windows.Forms.TextBox ipTextbox;
        private System.Windows.Forms.Label ip_label;
        private System.Windows.Forms.TextBox portTextbox;
        private System.Windows.Forms.Label port_label;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button restoreToDefaultsButton;
        private System.Windows.Forms.Button reconfigSetupButton;
    }
}