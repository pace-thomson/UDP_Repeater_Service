namespace UDP_Repeater_GUI
{
    partial class configDialog
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
            this.profileDropDown = new System.Windows.Forms.ComboBox();
            this.ip_field = new System.Windows.Forms.TextBox();
            this.ip_label = new System.Windows.Forms.Label();
            this.port_field = new System.Windows.Forms.TextBox();
            this.port_label = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.defaultButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // dialogOkButton
            // 
            this.dialogOkButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.dialogOkButton.Location = new System.Drawing.Point(280, 202);
            this.dialogOkButton.Name = "dialogOkButton";
            this.dialogOkButton.Size = new System.Drawing.Size(94, 38);
            this.dialogOkButton.TabIndex = 1;
            this.dialogOkButton.Text = "Ok";
            this.dialogOkButton.UseVisualStyleBackColor = true;
            this.dialogOkButton.Click += new System.EventHandler(this.dialogOkButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(166, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 24);
            this.label3.TabIndex = 15;
            this.label3.Text = "Configuration";
            // 
            // profileDropDown
            // 
            this.profileDropDown.FormattingEnabled = true;
            this.profileDropDown.Items.AddRange(new object[] {
            "Sending To",
            "Receiving From",
            "Default Send",
            "Default Receive"});
            this.profileDropDown.Location = new System.Drawing.Point(323, 124);
            this.profileDropDown.Name = "profileDropDown";
            this.profileDropDown.Size = new System.Drawing.Size(123, 21);
            this.profileDropDown.TabIndex = 17;
            // 
            // ip_field
            // 
            this.ip_field.Location = new System.Drawing.Point(45, 124);
            this.ip_field.Name = "ip_field";
            this.ip_field.Size = new System.Drawing.Size(100, 20);
            this.ip_field.TabIndex = 18;
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
            // port_field
            // 
            this.port_field.Location = new System.Drawing.Point(186, 124);
            this.port_field.Name = "port_field";
            this.port_field.Size = new System.Drawing.Size(100, 20);
            this.port_field.TabIndex = 20;
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
            // defaultButton
            // 
            this.defaultButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.defaultButton.Location = new System.Drawing.Point(112, 202);
            this.defaultButton.Name = "defaultButton";
            this.defaultButton.Size = new System.Drawing.Size(94, 38);
            this.defaultButton.TabIndex = 23;
            this.defaultButton.Text = "Restore to Default Settings";
            this.defaultButton.UseVisualStyleBackColor = true;
            this.defaultButton.Click += new System.EventHandler(this.defaultButton_Click);
            // 
            // configDialog
            // 
            this.AcceptButton = this.dialogOkButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.dialogOkButton;
            this.ClientSize = new System.Drawing.Size(477, 263);
            this.Controls.Add(this.defaultButton);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.port_label);
            this.Controls.Add(this.port_field);
            this.Controls.Add(this.ip_label);
            this.Controls.Add(this.ip_field);
            this.Controls.Add(this.profileDropDown);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.dialogOkButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.Name = "configDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Configuration";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.configDialog_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button dialogOkButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox profileDropDown;
        private System.Windows.Forms.TextBox ip_field;
        private System.Windows.Forms.Label ip_label;
        private System.Windows.Forms.TextBox port_field;
        private System.Windows.Forms.Label port_label;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button defaultButton;
    }
}