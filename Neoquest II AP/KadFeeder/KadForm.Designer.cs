namespace KadFeeder
{
    partial class KadForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.chkRefreshModeSmart = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMaxRefresh = new System.Windows.Forms.TextBox();
            this.txtMinRefresh = new System.Windows.Forms.TextBox();
            this.chkRefreshModeSet = new System.Windows.Forms.CheckBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.btnBegin = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMaxPrice = new System.Windows.Forms.TextBox();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.chkRefreshModeSmart);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtMaxRefresh);
            this.groupBox1.Controls.Add(this.txtMinRefresh);
            this.groupBox1.Controls.Add(this.chkRefreshModeSet);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(232, 137);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Refresh Mode";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(105, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "between :17 and :40";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(74, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Smart Refresh";
            // 
            // chkRefreshModeSmart
            // 
            this.chkRefreshModeSmart.AutoSize = true;
            this.chkRefreshModeSmart.Location = new System.Drawing.Point(6, 91);
            this.chkRefreshModeSmart.Name = "chkRefreshModeSmart";
            this.chkRefreshModeSmart.Size = new System.Drawing.Size(221, 17);
            this.chkRefreshModeSmart.TabIndex = 6;
            this.chkRefreshModeSmart.Text = "Every 7 minutes, then every 1-2 seconds ";
            this.chkRefreshModeSmart.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Set Refresh";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(175, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "seconds";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(112, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "and";
            // 
            // txtMaxRefresh
            // 
            this.txtMaxRefresh.Location = new System.Drawing.Point(143, 37);
            this.txtMaxRefresh.MaxLength = 2;
            this.txtMaxRefresh.Name = "txtMaxRefresh";
            this.txtMaxRefresh.Size = new System.Drawing.Size(26, 20);
            this.txtMaxRefresh.TabIndex = 2;
            // 
            // txtMinRefresh
            // 
            this.txtMinRefresh.Location = new System.Drawing.Point(80, 36);
            this.txtMinRefresh.MaxLength = 2;
            this.txtMinRefresh.Name = "txtMinRefresh";
            this.txtMinRefresh.Size = new System.Drawing.Size(26, 20);
            this.txtMinRefresh.TabIndex = 1;
            // 
            // chkRefreshModeSet
            // 
            this.chkRefreshModeSet.AutoSize = true;
            this.chkRefreshModeSet.Location = new System.Drawing.Point(6, 39);
            this.chkRefreshModeSet.Name = "chkRefreshModeSet";
            this.chkRefreshModeSet.Size = new System.Drawing.Size(68, 17);
            this.chkRefreshModeSet.TabIndex = 0;
            this.chkRefreshModeSet.Text = "Between";
            this.chkRefreshModeSet.UseVisualStyleBackColor = true;
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.WorkerSupportsCancellation = true;
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            // 
            // btnBegin
            // 
            this.btnBegin.Location = new System.Drawing.Point(74, 338);
            this.btnBegin.Name = "btnBegin";
            this.btnBegin.Size = new System.Drawing.Size(75, 23);
            this.btnBegin.TabIndex = 1;
            this.btnBegin.Text = "Begin";
            this.btnBegin.UseVisualStyleBackColor = true;
            this.btnBegin.Click += new System.EventHandler(this.btnBegin_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(74, 367);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.txtMaxPrice);
            this.groupBox2.Location = new System.Drawing.Point(12, 155);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(232, 137);
            this.groupBox2.TabIndex = 13;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Settings";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 22);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(131, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Max Price (default 99,999)";
            // 
            // txtMaxPrice
            // 
            this.txtMaxPrice.Location = new System.Drawing.Point(143, 19);
            this.txtMaxPrice.MaxLength = 5;
            this.txtMaxPrice.Name = "txtMaxPrice";
            this.txtMaxPrice.Size = new System.Drawing.Size(57, 20);
            this.txtMaxPrice.TabIndex = 0;
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(250, 292);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(381, 245);
            this.txtLog.TabIndex = 14;
            this.txtLog.Text = "";
            // 
            // KadForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Desktop;
            this.ClientSize = new System.Drawing.Size(643, 549);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnBegin);
            this.Controls.Add(this.groupBox1);
            this.Name = "KadForm";
            this.Text = "Kadoatery AutoFeeder";
            this.Load += new System.EventHandler(this.KadForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkRefreshModeSmart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMaxRefresh;
        private System.Windows.Forms.TextBox txtMinRefresh;
        private System.Windows.Forms.CheckBox chkRefreshModeSet;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button btnBegin;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMaxPrice;
        private System.Windows.Forms.RichTextBox txtLog;
    }
}