namespace UDP_Repeater_GUI
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.packetsHandledDisplay = new System.Windows.Forms.DataGridView();
            this.indexColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ipColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.portColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payloadColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.title_label = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.packetCounter = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.reconfigureSettings = new System.Windows.Forms.Button();
            this.logButton = new System.Windows.Forms.Button();
            this.currentSendPort = new System.Windows.Forms.Label();
            this.currentReceivePort = new System.Windows.Forms.Label();
            this.currentReceiveIp = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.currentSendIp = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label15 = new System.Windows.Forms.Label();
            this.currentTimeUnit = new System.Windows.Forms.Label();
            this.currentInterval = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.statusLabel = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.packetsHandledDisplay)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // packetsHandledDisplay
            // 
            this.packetsHandledDisplay.AllowUserToAddRows = false;
            this.packetsHandledDisplay.AllowUserToDeleteRows = false;
            this.packetsHandledDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.packetsHandledDisplay.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.packetsHandledDisplay.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.indexColumn,
            this.ipColumn,
            this.portColumn,
            this.payloadColumn,
            this.timeColumn});
            this.packetsHandledDisplay.Location = new System.Drawing.Point(16, 171);
            this.packetsHandledDisplay.Margin = new System.Windows.Forms.Padding(4);
            this.packetsHandledDisplay.MultiSelect = false;
            this.packetsHandledDisplay.Name = "packetsHandledDisplay";
            this.packetsHandledDisplay.ReadOnly = true;
            this.packetsHandledDisplay.RowHeadersVisible = false;
            this.packetsHandledDisplay.RowHeadersWidth = 70;
            this.packetsHandledDisplay.Size = new System.Drawing.Size(840, 309);
            this.packetsHandledDisplay.TabIndex = 4;
            // 
            // indexColumn
            // 
            this.indexColumn.HeaderText = "Packet Number";
            this.indexColumn.MinimumWidth = 6;
            this.indexColumn.Name = "indexColumn";
            this.indexColumn.ReadOnly = true;
            this.indexColumn.Width = 125;
            // 
            // ipColumn
            // 
            this.ipColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ipColumn.HeaderText = "IP Address (Source)";
            this.ipColumn.MinimumWidth = 6;
            this.ipColumn.Name = "ipColumn";
            this.ipColumn.ReadOnly = true;
            // 
            // portColumn
            // 
            this.portColumn.HeaderText = "Port (Source)";
            this.portColumn.MinimumWidth = 6;
            this.portColumn.Name = "portColumn";
            this.portColumn.ReadOnly = true;
            this.portColumn.Width = 125;
            // 
            // payloadColumn
            // 
            this.payloadColumn.HeaderText = "Data Length (Bytes)";
            this.payloadColumn.MinimumWidth = 6;
            this.payloadColumn.Name = "payloadColumn";
            this.payloadColumn.ReadOnly = true;
            this.payloadColumn.Width = 125;
            // 
            // timeColumn
            // 
            this.timeColumn.HeaderText = "Time Stamp";
            this.timeColumn.MinimumWidth = 6;
            this.timeColumn.Name = "timeColumn";
            this.timeColumn.ReadOnly = true;
            this.timeColumn.Width = 150;
            // 
            // title_label
            // 
            this.title_label.AutoSize = true;
            this.title_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title_label.Location = new System.Drawing.Point(19, 11);
            this.title_label.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.title_label.Name = "title_label";
            this.title_label.Size = new System.Drawing.Size(371, 39);
            this.title_label.TabIndex = 5;
            this.title_label.Text = "UDP Packet Repeater";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 105);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(377, 48);
            this.label2.TabIndex = 8;
            this.label2.Text = "This interface only begins running when the current user logs in.\r\nThe Repeater S" +
    "ervice starts on system bootup, so this is not \r\na complete record of all packet" +
    "s handled by the service.";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.packetCounter);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(332, 508);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(216, 32);
            this.panel1.TabIndex = 15;
            // 
            // packetCounter
            // 
            this.packetCounter.AutoSize = true;
            this.packetCounter.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.packetCounter.Location = new System.Drawing.Point(133, 7);
            this.packetCounter.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.packetCounter.Name = "packetCounter";
            this.packetCounter.Size = new System.Drawing.Size(18, 20);
            this.packetCounter.TabIndex = 7;
            this.packetCounter.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(117, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "Packets Handled: ";
            // 
            // reconfigureSettings
            // 
            this.reconfigureSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.reconfigureSettings.FlatAppearance.BorderSize = 2;
            this.reconfigureSettings.Location = new System.Drawing.Point(612, 508);
            this.reconfigureSettings.Margin = new System.Windows.Forms.Padding(4);
            this.reconfigureSettings.Name = "reconfigureSettings";
            this.reconfigureSettings.Size = new System.Drawing.Size(172, 32);
            this.reconfigureSettings.TabIndex = 18;
            this.reconfigureSettings.Text = "Reconfigure Settings";
            this.reconfigureSettings.UseVisualStyleBackColor = true;
            this.reconfigureSettings.Click += new System.EventHandler(this.showDialogbutton_Click);
            // 
            // logButton
            // 
            this.logButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.logButton.Location = new System.Drawing.Point(97, 508);
            this.logButton.Margin = new System.Windows.Forms.Padding(4);
            this.logButton.Name = "logButton";
            this.logButton.Size = new System.Drawing.Size(152, 32);
            this.logButton.TabIndex = 19;
            this.logButton.Text = "System Log*";
            this.logButton.UseVisualStyleBackColor = true;
            this.logButton.Click += new System.EventHandler(this.logButton_Click);
            // 
            // currentSendPort
            // 
            this.currentSendPort.AutoSize = true;
            this.currentSendPort.Location = new System.Drawing.Point(227, 98);
            this.currentSendPort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.currentSendPort.Name = "currentSendPort";
            this.currentSendPort.Size = new System.Drawing.Size(28, 16);
            this.currentSendPort.TabIndex = 10;
            this.currentSendPort.Text = "722";
            // 
            // currentReceivePort
            // 
            this.currentReceivePort.AutoSize = true;
            this.currentReceivePort.Location = new System.Drawing.Point(76, 98);
            this.currentReceivePort.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.currentReceivePort.Name = "currentReceivePort";
            this.currentReceivePort.Size = new System.Drawing.Size(35, 16);
            this.currentReceivePort.TabIndex = 9;
            this.currentReceivePort.Text = "7654";
            // 
            // currentReceiveIp
            // 
            this.currentReceiveIp.AutoSize = true;
            this.currentReceiveIp.Location = new System.Drawing.Point(33, 74);
            this.currentReceiveIp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.currentReceiveIp.Name = "currentReceiveIp";
            this.currentReceiveIp.Size = new System.Drawing.Size(93, 16);
            this.currentReceiveIp.TabIndex = 7;
            this.currentReceiveIp.Text = "132.58.202.157";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(37, 98);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(42, 16);
            this.label7.TabIndex = 4;
            this.label7.Text = "*Port: ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 74);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(25, 16);
            this.label6.TabIndex = 3;
            this.label6.Text = "IP: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(8, 47);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(126, 18);
            this.label4.TabIndex = 1;
            this.label4.Text = "Receiving From";
            // 
            // currentSendIp
            // 
            this.currentSendIp.AutoSize = true;
            this.currentSendIp.Location = new System.Drawing.Point(187, 74);
            this.currentSendIp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.currentSendIp.Name = "currentSendIp";
            this.currentSendIp.Size = new System.Drawing.Size(93, 16);
            this.currentSendIp.TabIndex = 8;
            this.currentSendIp.Text = "132.58.202.157";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(192, 98);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 16);
            this.label9.TabIndex = 6;
            this.label9.Text = "Port: ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(165, 74);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(25, 16);
            this.label8.TabIndex = 5;
            this.label8.Text = "IP: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(176, 47);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 18);
            this.label5.TabIndex = 2;
            this.label5.Text = "Sending To";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(79, 11);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(240, 26);
            this.label3.TabIndex = 0;
            this.label3.Text = "Current Configuration";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label15);
            this.panel2.Controls.Add(this.currentTimeUnit);
            this.panel2.Controls.Add(this.currentInterval);
            this.panel2.Controls.Add(this.label12);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.label10);
            this.panel2.Controls.Add(this.currentSendPort);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.currentReceivePort);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.currentReceiveIp);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.currentSendIp);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.label9);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Location = new System.Drawing.Point(436, 15);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(419, 155);
            this.panel2.TabIndex = 21;
            // 
            // label15
            // 
            this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(57, 127);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(300, 16);
            this.label15.TabIndex = 25;
            this.label15.Text = "*Receiving Port is local, everything else is remote";
            // 
            // currentTimeUnit
            // 
            this.currentTimeUnit.AutoSize = true;
            this.currentTimeUnit.Location = new System.Drawing.Point(351, 98);
            this.currentTimeUnit.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.currentTimeUnit.Name = "currentTimeUnit";
            this.currentTimeUnit.Size = new System.Drawing.Size(53, 16);
            this.currentTimeUnit.TabIndex = 15;
            this.currentTimeUnit.Text = "Minutes";
            // 
            // currentInterval
            // 
            this.currentInterval.AutoSize = true;
            this.currentInterval.Location = new System.Drawing.Point(381, 74);
            this.currentInterval.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.currentInterval.Name = "currentInterval";
            this.currentInterval.Size = new System.Drawing.Size(14, 16);
            this.currentInterval.TabIndex = 14;
            this.currentInterval.Text = "2";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(315, 98);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(33, 16);
            this.label12.TabIndex = 13;
            this.label12.Text = "Unit:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(320, 74);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 16);
            this.label11.TabIndex = 12;
            this.label11.Text = "Interval: ";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(319, 47);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(73, 18);
            this.label10.TabIndex = 11;
            this.label10.Text = "Inactivity";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(108, 65);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(128, 20);
            this.label13.TabIndex = 22;
            this.label13.Text = "Service Status: ";
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusLabel.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.statusLabel.Location = new System.Drawing.Point(248, 65);
            this.statusLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(92, 20);
            this.statusLabel.TabIndex = 23;
            this.statusLabel.Text = "Checking ";
            // 
            // label14
            // 
            this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(68, 487);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(206, 16);
            this.label14.TabIndex = 24;
            this.label14.Text = "*Inactivity Reconfiguration is Here";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(872, 558);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.logButton);
            this.Controls.Add(this.reconfigureSettings);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.title_label);
            this.Controls.Add(this.packetsHandledDisplay);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UDP Packet Repeater";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.gui_form_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.packetsHandledDisplay)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView packetsHandledDisplay;
        private System.Windows.Forms.Label title_label;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button reconfigureSettings;
        private System.Windows.Forms.Button logButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label currentSendPort;
        private System.Windows.Forms.Label currentReceivePort;
        private System.Windows.Forms.Label currentSendIp;
        private System.Windows.Forms.Label currentReceiveIp;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label currentInterval;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label currentTimeUnit;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label packetCounter;
        private System.Windows.Forms.DataGridViewTextBoxColumn indexColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ipColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn portColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn payloadColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeColumn;
        private System.Windows.Forms.Label label15;
    }
}

