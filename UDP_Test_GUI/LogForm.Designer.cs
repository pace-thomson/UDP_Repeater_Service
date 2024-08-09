namespace UDP_Repeater_GUI
{
    partial class LogForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LogForm));
            this.logDataGridView = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.inactivityButton = new System.Windows.Forms.Button();
            this.inactivityDropdown = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.inactivityInputBox = new System.Windows.Forms.NumericUpDown();
            this.loadingLabel = new System.Windows.Forms.Label();
            this.entryTypeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.logOriginColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messageColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timeStampColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.logDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.inactivityInputBox)).BeginInit();
            this.SuspendLayout();
            // 
            // logDataGridView
            // 
            this.logDataGridView.AllowUserToAddRows = false;
            this.logDataGridView.AllowUserToDeleteRows = false;
            this.logDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logDataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.logDataGridView.ColumnHeadersHeight = 34;
            this.logDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.logDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.entryTypeColumn,
            this.logOriginColumn,
            this.messageColumn,
            this.timeStampColumn});
            this.logDataGridView.Location = new System.Drawing.Point(17, 103);
            this.logDataGridView.MultiSelect = false;
            this.logDataGridView.Name = "logDataGridView";
            this.logDataGridView.ReadOnly = true;
            this.logDataGridView.RowHeadersVisible = false;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.logDataGridView.RowsDefaultCellStyle = dataGridViewCellStyle2;
            this.logDataGridView.RowTemplate.DefaultCellStyle.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.logDataGridView.Size = new System.Drawing.Size(808, 342);
            this.logDataGridView.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(132, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(225, 42);
            this.label2.TabIndex = 3;
            this.label2.Text = "System Log";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(503, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(188, 16);
            this.label3.TabIndex = 6;
            this.label3.Text = "Configure Inactivity Period";
            // 
            // inactivityButton
            // 
            this.inactivityButton.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.inactivityButton.Location = new System.Drawing.Point(674, 64);
            this.inactivityButton.Name = "inactivityButton";
            this.inactivityButton.Size = new System.Drawing.Size(76, 23);
            this.inactivityButton.TabIndex = 7;
            this.inactivityButton.Text = "Send";
            this.inactivityButton.UseVisualStyleBackColor = true;
            this.inactivityButton.Click += new System.EventHandler(this.inactivityButton_Click);
            // 
            // inactivityDropdown
            // 
            this.inactivityDropdown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.inactivityDropdown.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.inactivityDropdown.FormattingEnabled = true;
            this.inactivityDropdown.Items.AddRange(new object[] {
            "Minute(s)",
            "Hour(s)",
            "Day(s)"});
            this.inactivityDropdown.Location = new System.Drawing.Point(544, 66);
            this.inactivityDropdown.Name = "inactivityDropdown";
            this.inactivityDropdown.Size = new System.Drawing.Size(121, 21);
            this.inactivityDropdown.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.Location = new System.Drawing.Point(471, 51);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Interval";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(579, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Time Unit";
            // 
            // inactivityInputBox
            // 
            this.inactivityInputBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.inactivityInputBox.Location = new System.Drawing.Point(465, 67);
            this.inactivityInputBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.inactivityInputBox.Name = "inactivityInputBox";
            this.inactivityInputBox.Size = new System.Drawing.Size(68, 20);
            this.inactivityInputBox.TabIndex = 11;
            this.inactivityInputBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.inactivityInputBox.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // loadingLabel
            // 
            this.loadingLabel.AutoSize = true;
            this.loadingLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.loadingLabel.ForeColor = System.Drawing.Color.DarkGoldenrod;
            this.loadingLabel.Location = new System.Drawing.Point(312, 265);
            this.loadingLabel.Name = "loadingLabel";
            this.loadingLabel.Size = new System.Drawing.Size(245, 55);
            this.loadingLabel.TabIndex = 12;
            this.loadingLabel.Text = "Loading...";
            // 
            // entryTypeColumn
            // 
            this.entryTypeColumn.HeaderText = "Entry Type";
            this.entryTypeColumn.Name = "entryTypeColumn";
            this.entryTypeColumn.ReadOnly = true;
            this.entryTypeColumn.Width = 90;
            // 
            // logOriginColumn
            // 
            this.logOriginColumn.HeaderText = "Origin";
            this.logOriginColumn.Name = "logOriginColumn";
            this.logOriginColumn.ReadOnly = true;
            this.logOriginColumn.Width = 65;
            // 
            // messageColumn
            // 
            this.messageColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.messageColumn.DefaultCellStyle = dataGridViewCellStyle1;
            this.messageColumn.HeaderText = "Message";
            this.messageColumn.Name = "messageColumn";
            this.messageColumn.ReadOnly = true;
            // 
            // timeStampColumn
            // 
            this.timeStampColumn.HeaderText = "Timestamp";
            this.timeStampColumn.Name = "timeStampColumn";
            this.timeStampColumn.ReadOnly = true;
            this.timeStampColumn.Width = 150;
            // 
            // LogForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(840, 457);
            this.Controls.Add(this.loadingLabel);
            this.Controls.Add(this.inactivityInputBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.inactivityDropdown);
            this.Controls.Add(this.inactivityButton);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.logDataGridView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "LogForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "System Log";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LogForm_FormClosing);
            this.Shown += new System.EventHandler(this.LogForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.logDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.inactivityInputBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView logDataGridView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button inactivityButton;
        private System.Windows.Forms.ComboBox inactivityDropdown;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown inactivityInputBox;
        private System.Windows.Forms.Label loadingLabel;
        private System.Windows.Forms.DataGridViewTextBoxColumn entryTypeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn logOriginColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn messageColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn timeStampColumn;
    }
}