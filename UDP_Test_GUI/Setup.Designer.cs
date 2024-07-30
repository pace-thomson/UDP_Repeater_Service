namespace UDP_Test_GUI
{
    partial class Setup
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Setup));
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.nameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.macAddressColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sumbitButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.promEndpoint = new System.Windows.Forms.TextBox();
            this.lokiEndpoint = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.nicTextbox = new System.Windows.Forms.TextBox();
            this.lokiTextbox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.promTextbox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeight = 28;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.nameColumn,
            this.descriptionColumn,
            this.macAddressColumn});
            this.dataGridView1.Location = new System.Drawing.Point(27, 175);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(581, 189);
            this.dataGridView1.TabIndex = 0;
            // 
            // nameColumn
            // 
            this.nameColumn.HeaderText = "Name";
            this.nameColumn.Name = "nameColumn";
            this.nameColumn.ReadOnly = true;
            this.nameColumn.Width = 200;
            // 
            // descriptionColumn
            // 
            this.descriptionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.descriptionColumn.HeaderText = "Description";
            this.descriptionColumn.Name = "descriptionColumn";
            this.descriptionColumn.ReadOnly = true;
            // 
            // macAddressColumn
            // 
            this.macAddressColumn.HeaderText = "Mac Address";
            this.macAddressColumn.Name = "macAddressColumn";
            this.macAddressColumn.ReadOnly = true;
            // 
            // sumbitButton
            // 
            this.sumbitButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.sumbitButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.sumbitButton.Location = new System.Drawing.Point(371, 381);
            this.sumbitButton.Name = "sumbitButton";
            this.sumbitButton.Size = new System.Drawing.Size(112, 38);
            this.sumbitButton.TabIndex = 1;
            this.sumbitButton.Text = "Submit";
            this.sumbitButton.UseVisualStyleBackColor = true;
            this.sumbitButton.Click += new System.EventHandler(this.doneButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(68, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(483, 48);
            this.label1.TabIndex = 2;
            this.label1.Text = "Please select a Network Interface Card to use for listening on this system. Pleas" +
    "e\r\nalso input the Prometheus and Loki endpoints you wish to use. \r\nThese setting" +
    "s can be changed later if needed.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(249, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(81, 29);
            this.label2.TabIndex = 3;
            this.label2.Text = "Setup";
            // 
            // promEndpoint
            // 
            this.promEndpoint.AllowDrop = true;
            this.promEndpoint.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.promEndpoint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.promEndpoint.Location = new System.Drawing.Point(32, 397);
            this.promEndpoint.Name = "promEndpoint";
            this.promEndpoint.Size = new System.Drawing.Size(140, 20);
            this.promEndpoint.TabIndex = 4;
            // 
            // lokiEndpoint
            // 
            this.lokiEndpoint.AllowDrop = true;
            this.lokiEndpoint.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lokiEndpoint.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lokiEndpoint.Location = new System.Drawing.Point(197, 397);
            this.lokiEndpoint.Name = "lokiEndpoint";
            this.lokiEndpoint.Size = new System.Drawing.Size(149, 20);
            this.lokiEndpoint.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 377);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(111, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Prometheus Endpoint:";
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(234, 378);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Loki Endpoint:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(26, 123);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(146, 20);
            this.label5.TabIndex = 8;
            this.label5.Text = "Current Settings:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.nicTextbox);
            this.panel1.Controls.Add(this.lokiTextbox);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.promTextbox);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label6);
            this.panel1.Location = new System.Drawing.Point(194, 98);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(436, 71);
            this.panel1.TabIndex = 9;
            // 
            // nicTextbox
            // 
            this.nicTextbox.BackColor = System.Drawing.SystemColors.Control;
            this.nicTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.nicTextbox.Location = new System.Drawing.Point(174, 11);
            this.nicTextbox.Name = "nicTextbox";
            this.nicTextbox.Size = new System.Drawing.Size(259, 13);
            this.nicTextbox.TabIndex = 11;
            this.nicTextbox.Text = "Intel(R) Ethernet Connection (17) I219-LM";
            // 
            // lokiTextbox
            // 
            this.lokiTextbox.BackColor = System.Drawing.SystemColors.Control;
            this.lokiTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lokiTextbox.Location = new System.Drawing.Point(177, 49);
            this.lokiTextbox.Name = "lokiTextbox";
            this.lokiTextbox.Size = new System.Drawing.Size(256, 13);
            this.lokiTextbox.TabIndex = 13;
            this.lokiTextbox.Text = "http://172.18.46.211:3100";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(26, 51);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "Loki Endpoint:";
            // 
            // promTextbox
            // 
            this.promTextbox.BackColor = System.Drawing.SystemColors.Control;
            this.promTextbox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.promTextbox.Location = new System.Drawing.Point(162, 30);
            this.promTextbox.Name = "promTextbox";
            this.promTextbox.Size = new System.Drawing.Size(271, 13);
            this.promTextbox.TabIndex = 12;
            this.promTextbox.Text = "http://172.18.46.211:9090/api/v1/otlp/v1/metrics";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(7, 30);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(131, 13);
            this.label7.TabIndex = 1;
            this.label7.Text = "Prometheus Endpoint:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(3, 9);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(143, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Network Interface Card:";
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(496, 381);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(112, 38);
            this.cancelButton.TabIndex = 10;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // Setup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(630, 437);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lokiEndpoint);
            this.Controls.Add(this.promEndpoint);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.sumbitButton);
            this.Controls.Add(this.dataGridView1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Setup";
            this.Text = "Packet Repeater Setup";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Setup_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button sumbitButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn macAddressColumn;
        private System.Windows.Forms.TextBox promEndpoint;
        private System.Windows.Forms.TextBox lokiEndpoint;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox nicTextbox;
        private System.Windows.Forms.TextBox lokiTextbox;
        private System.Windows.Forms.TextBox promTextbox;
    }
}