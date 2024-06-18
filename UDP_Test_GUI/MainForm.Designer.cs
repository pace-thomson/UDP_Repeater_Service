namespace UDP_Test_GUI
{
    partial class gui_form
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(gui_form));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.indexColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ipColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.portColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.payloadColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.title_label = new System.Windows.Forms.Label();
            this.packet_counter = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.showConfigbutton = new System.Windows.Forms.Button();
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
            this.currentInterval = new System.Windows.Forms.Label();
            this.currentFrequency = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.indexColumn,
            this.ipColumn,
            this.portColumn,
            this.payloadColumn,
            this.timeColumn});
            this.dataGridView1.Location = new System.Drawing.Point(20, 139);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidth = 70;
            this.dataGridView1.Size = new System.Drawing.Size(604, 251);
            this.dataGridView1.TabIndex = 4;
            // 
            // indexColumn
            // 
            this.indexColumn.HeaderText = "Packet Number";
            this.indexColumn.Name = "indexColumn";
            this.indexColumn.ReadOnly = true;
            // 
            // ipColumn
            // 
            this.ipColumn.HeaderText = "IP Address";
            this.ipColumn.Name = "ipColumn";
            this.ipColumn.ReadOnly = true;
            // 
            // portColumn
            // 
            this.portColumn.HeaderText = "Port";
            this.portColumn.Name = "portColumn";
            this.portColumn.ReadOnly = true;
            // 
            // payloadColumn
            // 
            this.payloadColumn.HeaderText = "Payload Length (Bytes)";
            this.payloadColumn.Name = "payloadColumn";
            this.payloadColumn.ReadOnly = true;
            // 
            // timeColumn
            // 
            this.timeColumn.HeaderText = "Time Stamp";
            this.timeColumn.Name = "timeColumn";
            this.timeColumn.ReadOnly = true;
            this.timeColumn.Width = 200;
            // 
            // title_label
            // 
            this.title_label.AutoSize = true;
            this.title_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.title_label.Location = new System.Drawing.Point(26, 29);
            this.title_label.Name = "title_label";
            this.title_label.Size = new System.Drawing.Size(301, 31);
            this.title_label.TabIndex = 5;
            this.title_label.Text = "UDP Packet Repeater";
            // 
            // packet_counter
            // 
            this.packet_counter.AutoSize = true;
            this.packet_counter.Location = new System.Drawing.Point(110, 5);
            this.packet_counter.Name = "packet_counter";
            this.packet_counter.Size = new System.Drawing.Size(13, 13);
            this.packet_counter.TabIndex = 7;
            this.packet_counter.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(17, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(310, 39);
            this.label2.TabIndex = 8;
            this.label2.Text = "This interface only begins running when the current user logs in. \r\nThe Packet Re" +
    "peater runs whenever this machine is on, so this \r\n is not a complete record of " +
    "all packets sent by the service.";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.packet_counter);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(262, 408);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(134, 23);
            this.panel1.TabIndex = 15;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Packets Handled: ";
            // 
            // showConfigbutton
            // 
            this.showConfigbutton.FlatAppearance.BorderSize = 2;
            this.showConfigbutton.Location = new System.Drawing.Point(462, 406);
            this.showConfigbutton.Name = "showConfigbutton";
            this.showConfigbutton.Size = new System.Drawing.Size(129, 26);
            this.showConfigbutton.TabIndex = 18;
            this.showConfigbutton.Text = "Reconfigure IP\'s/Ports";
            this.showConfigbutton.UseVisualStyleBackColor = true;
            this.showConfigbutton.Click += new System.EventHandler(this.showDialogbutton_Click);
            // 
            // logButton
            // 
            this.logButton.Location = new System.Drawing.Point(73, 405);
            this.logButton.Name = "logButton";
            this.logButton.Size = new System.Drawing.Size(114, 26);
            this.logButton.TabIndex = 19;
            this.logButton.Text = "System Log";
            this.logButton.UseVisualStyleBackColor = true;
            this.logButton.Click += new System.EventHandler(this.logButton_Click);
            // 
            // currentSendPort
            // 
            this.currentSendPort.AutoSize = true;
            this.currentSendPort.Location = new System.Drawing.Point(158, 85);
            this.currentSendPort.Name = "currentSendPort";
            this.currentSendPort.Size = new System.Drawing.Size(25, 13);
            this.currentSendPort.TabIndex = 10;
            this.currentSendPort.Text = "722";
            // 
            // currentReceivePort
            // 
            this.currentReceivePort.AutoSize = true;
            this.currentReceivePort.Location = new System.Drawing.Point(49, 85);
            this.currentReceivePort.Name = "currentReceivePort";
            this.currentReceivePort.Size = new System.Drawing.Size(31, 13);
            this.currentReceivePort.TabIndex = 9;
            this.currentReceivePort.Text = "7654";
            // 
            // currentReceiveIp
            // 
            this.currentReceiveIp.AutoSize = true;
            this.currentReceiveIp.Location = new System.Drawing.Point(20, 65);
            this.currentReceiveIp.Name = "currentReceiveIp";
            this.currentReceiveIp.Size = new System.Drawing.Size(82, 13);
            this.currentReceiveIp.TabIndex = 7;
            this.currentReceiveIp.Text = "132.58.202.157";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(20, 85);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Port: ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(23, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "IP: ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(20, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 15);
            this.label4.TabIndex = 1;
            this.label4.Text = "Receiving";
            // 
            // currentSendIp
            // 
            this.currentSendIp.AutoSize = true;
            this.currentSendIp.Location = new System.Drawing.Point(123, 65);
            this.currentSendIp.Name = "currentSendIp";
            this.currentSendIp.Size = new System.Drawing.Size(88, 13);
            this.currentSendIp.TabIndex = 8;
            this.currentSendIp.Text = "132.58.2002.157";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(126, 85);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 13);
            this.label9.TabIndex = 6;
            this.label9.Text = "Port: ";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(108, 65);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 13);
            this.label8.TabIndex = 5;
            this.label8.Text = "IP: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(123, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "Sending";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(61, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(182, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "Current Configuration";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.currentInterval);
            this.panel2.Controls.Add(this.currentFrequency);
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
            this.panel2.Location = new System.Drawing.Point(333, 20);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(308, 113);
            this.panel2.TabIndex = 21;
            // 
            // currentInterval
            // 
            this.currentInterval.AutoSize = true;
            this.currentInterval.Location = new System.Drawing.Point(255, 85);
            this.currentInterval.Name = "currentInterval";
            this.currentInterval.Size = new System.Drawing.Size(44, 13);
            this.currentInterval.TabIndex = 15;
            this.currentInterval.Text = "Minutes";
            // 
            // currentFrequency
            // 
            this.currentFrequency.AutoSize = true;
            this.currentFrequency.Location = new System.Drawing.Point(278, 65);
            this.currentFrequency.Name = "currentFrequency";
            this.currentFrequency.Size = new System.Drawing.Size(13, 13);
            this.currentFrequency.TabIndex = 14;
            this.currentFrequency.Text = "2";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(213, 85);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(45, 13);
            this.label12.TabIndex = 13;
            this.label12.Text = "Interval:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(217, 65);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(60, 13);
            this.label11.TabIndex = 12;
            this.label11.Text = "Frequency:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(219, 42);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(62, 15);
            this.label10.TabIndex = 11;
            this.label10.Text = "Inactivity";
            // 
            // gui_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 450);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.logButton);
            this.Controls.Add(this.showConfigbutton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.title_label);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "gui_form";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UDP Packet Repeater";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn indexColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ipColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn portColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn payloadColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeColumn;
        private System.Windows.Forms.Label title_label;
        private System.Windows.Forms.Label packet_counter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button showConfigbutton;
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
        private System.Windows.Forms.Label currentFrequency;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label currentInterval;
    }
}

