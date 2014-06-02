namespace thinkgear_form
{
    partial class ProgramForm
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
            this.btnStart = new System.Windows.Forms.Button();
            this.lstBoxLog = new System.Windows.Forms.ListBox();
            this.cmboCOM = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(12, 12);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(153, 23);
            this.btnStart.TabIndex = 0;
            this.btnStart.Text = "button1";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lstBoxLog
            // 
            this.lstBoxLog.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstBoxLog.FormattingEnabled = true;
            this.lstBoxLog.ItemHeight = 14;
            this.lstBoxLog.Location = new System.Drawing.Point(12, 41);
            this.lstBoxLog.Name = "lstBoxLog";
            this.lstBoxLog.Size = new System.Drawing.Size(395, 410);
            this.lstBoxLog.TabIndex = 1;
            // 
            // cmboCOM
            // 
            this.cmboCOM.FormattingEnabled = true;
            this.cmboCOM.Location = new System.Drawing.Point(172, 12);
            this.cmboCOM.Name = "cmboCOM";
            this.cmboCOM.Size = new System.Drawing.Size(121, 21);
            this.cmboCOM.TabIndex = 2;
            // 
            // ProgramForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(692, 470);
            this.Controls.Add(this.cmboCOM);
            this.Controls.Add(this.lstBoxLog);
            this.Controls.Add(this.btnStart);
            this.Name = "ProgramForm";
            this.Text = "ThinkGear Status";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ListBox lstBoxLog;
        private System.Windows.Forms.ComboBox cmboCOM;
    }
}

