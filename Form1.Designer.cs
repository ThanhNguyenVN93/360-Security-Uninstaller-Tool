namespace frm360security
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.grbtitle = new System.Windows.Forms.GroupBox();
            this.cmbLang = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grbstatus = new System.Windows.Forms.GroupBox();
            this.lblDetect = new System.Windows.Forms.Label();
            this.lblinfo = new System.Windows.Forms.Label();
            this.grbmode = new System.Windows.Forms.GroupBox();
            this.rbManual = new System.Windows.Forms.RadioButton();
            this.rbAuto = new System.Windows.Forms.RadioButton();
            this.grbactions = new System.Windows.Forms.GroupBox();
            this.chkRestorePoint = new System.Windows.Forms.CheckBox();
            this.btnViewLog = new System.Windows.Forms.Button();
            this.btnManualSafeClean = new System.Windows.Forms.Button();
            this.btnManualUninstall = new System.Windows.Forms.Button();
            this.lblAutoHint = new System.Windows.Forms.Label();
            this.btnAutoStart = new System.Windows.Forms.Button();
            this.grbprogress = new System.Windows.Forms.GroupBox();
            this.txtProgress = new System.Windows.Forms.TextBox();
            this.grbfooter = new System.Windows.Forms.GroupBox();
            this.lblVersion = new System.Windows.Forms.Label();
            this.linkReport = new System.Windows.Forms.LinkLabel();
            this.btnDonate = new System.Windows.Forms.Button();
            this.grbtitle.SuspendLayout();
            this.grbstatus.SuspendLayout();
            this.grbmode.SuspendLayout();
            this.grbactions.SuspendLayout();
            this.grbprogress.SuspendLayout();
            this.grbfooter.SuspendLayout();
            this.SuspendLayout();
            //
            // grbtitle
            //
            this.grbtitle.Controls.Add(this.cmbLang);
            this.grbtitle.Controls.Add(this.label1);
            this.grbtitle.Location = new System.Drawing.Point(-1, 1);
            this.grbtitle.Name = "grbtitle";
            this.grbtitle.Size = new System.Drawing.Size(553, 76);
            this.grbtitle.TabIndex = 0;
            this.grbtitle.TabStop = false;
            //
            // cmbLang
            //
            this.cmbLang.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLang.FormattingEnabled = true;
            this.cmbLang.Items.AddRange(new object[] {
            "VI",
            "EN"});
            this.cmbLang.Location = new System.Drawing.Point(480, 29);
            this.cmbLang.Name = "cmbLang";
            this.cmbLang.Size = new System.Drawing.Size(60, 23);
            this.cmbLang.TabIndex = 1;
            this.cmbLang.SelectedIndexChanged += new System.EventHandler(this.cmbLang_SelectedIndexChanged);
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 16.5F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.label1.Location = new System.Drawing.Point(57, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(416, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "360 SECURITY UNINSTALLER TOOL";
            //
            // grbstatus
            //
            this.grbstatus.Controls.Add(this.lblDetect);
            this.grbstatus.Controls.Add(this.lblinfo);
            this.grbstatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbstatus.Location = new System.Drawing.Point(-1, 83);
            this.grbstatus.Name = "grbstatus";
            this.grbstatus.Size = new System.Drawing.Size(553, 68);
            this.grbstatus.TabIndex = 1;
            this.grbstatus.TabStop = false;
            this.grbstatus.Text = "TRANG THAI";
            //
            // lblDetect
            //
            this.lblDetect.AutoSize = true;
            this.lblDetect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDetect.Location = new System.Drawing.Point(15, 42);
            this.lblDetect.Name = "lblDetect";
            this.lblDetect.Size = new System.Drawing.Size(150, 15);
            this.lblDetect.TabIndex = 1;
            this.lblDetect.Text = "Dang kiem tra...";
            //
            // lblinfo
            //
            this.lblinfo.AutoSize = true;
            this.lblinfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblinfo.Location = new System.Drawing.Point(15, 22);
            this.lblinfo.Name = "lblinfo";
            this.lblinfo.Size = new System.Drawing.Size(0, 15);
            this.lblinfo.TabIndex = 0;
            //
            // grbmode
            //
            this.grbmode.Controls.Add(this.rbManual);
            this.grbmode.Controls.Add(this.rbAuto);
            this.grbmode.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbmode.Location = new System.Drawing.Point(-1, 154);
            this.grbmode.Name = "grbmode";
            this.grbmode.Size = new System.Drawing.Size(553, 52);
            this.grbmode.TabIndex = 2;
            this.grbmode.TabStop = false;
            this.grbmode.Text = "CHE DO THUC HIEN";
            //
            // rbManual
            //
            this.rbManual.AutoSize = true;
            this.rbManual.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbManual.Location = new System.Drawing.Point(290, 23);
            this.rbManual.Name = "rbManual";
            this.rbManual.Size = new System.Drawing.Size(230, 19);
            this.rbManual.TabIndex = 1;
            this.rbManual.Text = "Thu cong - tu chon tung buoc";
            this.rbManual.UseVisualStyleBackColor = true;
            this.rbManual.CheckedChanged += new System.EventHandler(this.ModeChanged);
            //
            // rbAuto
            //
            this.rbAuto.AutoSize = true;
            this.rbAuto.Checked = true;
            this.rbAuto.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbAuto.Location = new System.Drawing.Point(15, 23);
            this.rbAuto.Name = "rbAuto";
            this.rbAuto.Size = new System.Drawing.Size(240, 19);
            this.rbAuto.TabIndex = 0;
            this.rbAuto.TabStop = true;
            this.rbAuto.Text = "Tu dong (khuyen nghi) - 1 cham xong";
            this.rbAuto.UseVisualStyleBackColor = true;
            this.rbAuto.CheckedChanged += new System.EventHandler(this.ModeChanged);
            //
            // grbactions
            //
            this.grbactions.Controls.Add(this.chkRestorePoint);
            this.grbactions.Controls.Add(this.btnViewLog);
            this.grbactions.Controls.Add(this.btnManualSafeClean);
            this.grbactions.Controls.Add(this.btnManualUninstall);
            this.grbactions.Controls.Add(this.lblAutoHint);
            this.grbactions.Controls.Add(this.btnAutoStart);
            this.grbactions.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbactions.Location = new System.Drawing.Point(-1, 206);
            this.grbactions.Name = "grbactions";
            this.grbactions.Size = new System.Drawing.Size(553, 112);
            this.grbactions.TabIndex = 3;
            this.grbactions.TabStop = false;
            this.grbactions.Text = "THUC HIEN";
            //
            // chkRestorePoint
            //
            this.chkRestorePoint.AutoSize = true;
            this.chkRestorePoint.Checked = true;
            this.chkRestorePoint.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRestorePoint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRestorePoint.Location = new System.Drawing.Point(15, 88);
            this.chkRestorePoint.Name = "chkRestorePoint";
            this.chkRestorePoint.Size = new System.Drawing.Size(390, 19);
            this.chkRestorePoint.TabIndex = 5;
            this.chkRestorePoint.Text = "Tao diem khoi phuc he thong truoc khi don dep (khuyen nghi)";
            this.chkRestorePoint.UseVisualStyleBackColor = true;
            //
            // btnViewLog
            //
            this.btnViewLog.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnViewLog.Location = new System.Drawing.Point(430, 20);
            this.btnViewLog.Name = "btnViewLog";
            this.btnViewLog.Size = new System.Drawing.Size(108, 30);
            this.btnViewLog.TabIndex = 4;
            this.btnViewLog.Text = "Xem Log";
            this.btnViewLog.UseVisualStyleBackColor = true;
            this.btnViewLog.Click += new System.EventHandler(this.btnViewLog_Click);
            //
            // btnManualSafeClean
            //
            this.btnManualSafeClean.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnManualSafeClean.Location = new System.Drawing.Point(185, 20);
            this.btnManualSafeClean.Name = "btnManualSafeClean";
            this.btnManualSafeClean.Size = new System.Drawing.Size(230, 30);
            this.btnManualSafeClean.TabIndex = 3;
            this.btnManualSafeClean.Text = "2. Khoi dong Safe Mode && Don dep";
            this.btnManualSafeClean.UseVisualStyleBackColor = true;
            this.btnManualSafeClean.Visible = false;
            this.btnManualSafeClean.Click += new System.EventHandler(this.btnManualSafeClean_Click);
            //
            // btnManualUninstall
            //
            this.btnManualUninstall.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnManualUninstall.Location = new System.Drawing.Point(15, 20);
            this.btnManualUninstall.Name = "btnManualUninstall";
            this.btnManualUninstall.Size = new System.Drawing.Size(160, 30);
            this.btnManualUninstall.TabIndex = 2;
            this.btnManualUninstall.Text = "1. Go cai dat chinh thuc";
            this.btnManualUninstall.UseVisualStyleBackColor = true;
            this.btnManualUninstall.Visible = false;
            this.btnManualUninstall.Click += new System.EventHandler(this.btnManualUninstall_Click);
            //
            // lblAutoHint
            //
            this.lblAutoHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAutoHint.ForeColor = System.Drawing.Color.DimGray;
            this.lblAutoHint.Location = new System.Drawing.Point(15, 60);
            this.lblAutoHint.Name = "lblAutoHint";
            this.lblAutoHint.Size = new System.Drawing.Size(523, 28);
            this.lblAutoHint.TabIndex = 1;
            this.lblAutoHint.Text = "May se tu dong khoi dong lai vao Safe Mode de don dep, sau do tu quay lai Normal" +
    " Mode. Vui long luu cong viec truoc khi bat dau.";
            //
            // btnAutoStart
            //
            this.btnAutoStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAutoStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.btnAutoStart.Location = new System.Drawing.Point(125, 20);
            this.btnAutoStart.Name = "btnAutoStart";
            this.btnAutoStart.Size = new System.Drawing.Size(300, 35);
            this.btnAutoStart.TabIndex = 0;
            this.btnAutoStart.Text = "BAT DAU GO BO 360 TOTAL SECURITY";
            this.btnAutoStart.UseVisualStyleBackColor = true;
            this.btnAutoStart.Click += new System.EventHandler(this.btnAutoStart_Click);
            //
            // grbprogress
            //
            this.grbprogress.Controls.Add(this.txtProgress);
            this.grbprogress.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbprogress.Location = new System.Drawing.Point(-1, 320);
            this.grbprogress.Name = "grbprogress";
            this.grbprogress.Size = new System.Drawing.Size(553, 90);
            this.grbprogress.TabIndex = 4;
            this.grbprogress.TabStop = false;
            this.grbprogress.Text = "TIEN TRINH";
            //
            // txtProgress
            //
            this.txtProgress.BackColor = System.Drawing.Color.White;
            this.txtProgress.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtProgress.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtProgress.Location = new System.Drawing.Point(10, 20);
            this.txtProgress.Multiline = true;
            this.txtProgress.Name = "txtProgress";
            this.txtProgress.ReadOnly = true;
            this.txtProgress.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtProgress.Size = new System.Drawing.Size(533, 60);
            this.txtProgress.TabIndex = 0;
            //
            // grbfooter
            //
            this.grbfooter.Controls.Add(this.lblVersion);
            this.grbfooter.Controls.Add(this.linkReport);
            this.grbfooter.Controls.Add(this.btnDonate);
            this.grbfooter.Location = new System.Drawing.Point(-1, 411);
            this.grbfooter.Name = "grbfooter";
            this.grbfooter.Size = new System.Drawing.Size(553, 57);
            this.grbfooter.TabIndex = 5;
            this.grbfooter.TabStop = false;
            //
            // lblVersion
            //
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.Color.Gray;
            this.lblVersion.Location = new System.Drawing.Point(185, 33);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(60, 15);
            this.lblVersion.TabIndex = 2;
            this.lblVersion.Text = "v1.1.0";
            //
            // linkReport
            //
            this.linkReport.AutoSize = true;
            this.linkReport.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkReport.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(102)))), ((int)(((byte)(204)))));
            this.linkReport.Location = new System.Drawing.Point(185, 16);
            this.linkReport.Name = "linkReport";
            this.linkReport.Size = new System.Drawing.Size(220, 15);
            this.linkReport.TabIndex = 1;
            this.linkReport.TabStop = true;
            this.linkReport.Text = "Bao loi / Dong gop (GitHub)";
            this.linkReport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkReport_LinkClicked);
            //
            // btnDonate
            //
            this.btnDonate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDonate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.btnDonate.Location = new System.Drawing.Point(15, 13);
            this.btnDonate.Name = "btnDonate";
            this.btnDonate.Size = new System.Drawing.Size(160, 32);
            this.btnDonate.TabIndex = 0;
            this.btnDonate.Text = "Ung ho tac gia";
            this.btnDonate.UseVisualStyleBackColor = true;
            this.btnDonate.Click += new System.EventHandler(this.btnDonate_Click);
            //
            // Form1
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 468);
            this.Controls.Add(this.grbfooter);
            this.Controls.Add(this.grbprogress);
            this.Controls.Add(this.grbactions);
            this.Controls.Add(this.grbmode);
            this.Controls.Add(this.grbstatus);
            this.Controls.Add(this.grbtitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "360 UNINSTALLER TOOL";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.grbtitle.ResumeLayout(false);
            this.grbtitle.PerformLayout();
            this.grbstatus.ResumeLayout(false);
            this.grbstatus.PerformLayout();
            this.grbmode.ResumeLayout(false);
            this.grbmode.PerformLayout();
            this.grbactions.ResumeLayout(false);
            this.grbactions.PerformLayout();
            this.grbprogress.ResumeLayout(false);
            this.grbprogress.PerformLayout();
            this.grbfooter.ResumeLayout(false);
            this.grbfooter.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbtitle;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbLang;
        private System.Windows.Forms.GroupBox grbstatus;
        private System.Windows.Forms.Label lblDetect;
        private System.Windows.Forms.Label lblinfo;
        private System.Windows.Forms.GroupBox grbmode;
        private System.Windows.Forms.RadioButton rbManual;
        private System.Windows.Forms.RadioButton rbAuto;
        private System.Windows.Forms.GroupBox grbactions;
        private System.Windows.Forms.CheckBox chkRestorePoint;
        private System.Windows.Forms.Button btnViewLog;
        private System.Windows.Forms.Button btnManualSafeClean;
        private System.Windows.Forms.Button btnManualUninstall;
        private System.Windows.Forms.Label lblAutoHint;
        private System.Windows.Forms.Button btnAutoStart;
        private System.Windows.Forms.GroupBox grbprogress;
        private System.Windows.Forms.TextBox txtProgress;
        private System.Windows.Forms.GroupBox grbfooter;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.LinkLabel linkReport;
        private System.Windows.Forms.Button btnDonate;
    }
}
