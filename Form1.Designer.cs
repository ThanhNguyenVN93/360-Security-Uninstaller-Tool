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
            this.label1 = new System.Windows.Forms.Label();
            this.grbchuanbi = new System.Windows.Forms.GroupBox();
            this.lblstatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnDownload = new System.Windows.Forms.Button();
            this.txtoutput = new System.Windows.Forms.TextBox();
            this.txturl = new System.Windows.Forms.TextBox();
            this.grbclean = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnuninstall = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnclean = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.lblinfo = new System.Windows.Forms.Label();
            this.grbtitle.SuspendLayout();
            this.grbchuanbi.SuspendLayout();
            this.grbclean.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // grbtitle
            // 
            this.grbtitle.Controls.Add(this.label1);
            this.grbtitle.Location = new System.Drawing.Point(-1, 1);
            this.grbtitle.Name = "grbtitle";
            this.grbtitle.Size = new System.Drawing.Size(553, 76);
            this.grbtitle.TabIndex = 0;
            this.grbtitle.TabStop = false;
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
            // grbchuanbi
            // 
            this.grbchuanbi.Controls.Add(this.lblstatus);
            this.grbchuanbi.Controls.Add(this.label3);
            this.grbchuanbi.Controls.Add(this.label2);
            this.grbchuanbi.Controls.Add(this.progressBar1);
            this.grbchuanbi.Controls.Add(this.btnDownload);
            this.grbchuanbi.Controls.Add(this.txtoutput);
            this.grbchuanbi.Controls.Add(this.txturl);
            this.grbchuanbi.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbchuanbi.Location = new System.Drawing.Point(-1, 83);
            this.grbchuanbi.Name = "grbchuanbi";
            this.grbchuanbi.Size = new System.Drawing.Size(553, 111);
            this.grbchuanbi.TabIndex = 1;
            this.grbchuanbi.TabStop = false;
            this.grbchuanbi.Text = "PREPAIR";
            // 
            // lblstatus
            // 
            this.lblstatus.AutoSize = true;
            this.lblstatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblstatus.Location = new System.Drawing.Point(273, 82);
            this.lblstatus.Name = "lblstatus";
            this.lblstatus.Size = new System.Drawing.Size(0, 15);
            this.lblstatus.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(13, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 15);
            this.label3.TabIndex = 5;
            this.label3.Text = "LINK OUT:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(15, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "LINK:";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(18, 82);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(249, 23);
            this.progressBar1.TabIndex = 3;
            // 
            // btnDownload
            // 
            this.btnDownload.Location = new System.Drawing.Point(420, 27);
            this.btnDownload.Name = "btnDownload";
            this.btnDownload.Size = new System.Drawing.Size(99, 48);
            this.btnDownload.TabIndex = 2;
            this.btnDownload.Text = "DOWNLOAD";
            this.btnDownload.UseVisualStyleBackColor = true;
            this.btnDownload.Click += new System.EventHandler(this.btnDownload_Click);
            // 
            // txtoutput
            // 
            this.txtoutput.Enabled = false;
            this.txtoutput.Location = new System.Drawing.Point(85, 51);
            this.txtoutput.Multiline = true;
            this.txtoutput.Name = "txtoutput";
            this.txtoutput.Size = new System.Drawing.Size(329, 21);
            this.txtoutput.TabIndex = 1;
            // 
            // txturl
            // 
            this.txturl.Enabled = false;
            this.txturl.Location = new System.Drawing.Point(85, 24);
            this.txturl.Name = "txturl";
            this.txturl.Size = new System.Drawing.Size(329, 21);
            this.txturl.TabIndex = 0;
            this.txturl.Text = "https://download.revouninstaller.com/download/revoregistrycleaner/RRC_FreePortabl" +
    "e.zip";
            // 
            // grbclean
            // 
            this.grbclean.Controls.Add(this.textBox1);
            this.grbclean.Controls.Add(this.btnuninstall);
            this.grbclean.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grbclean.Location = new System.Drawing.Point(-1, 200);
            this.grbclean.Name = "grbclean";
            this.grbclean.Size = new System.Drawing.Size(553, 53);
            this.grbclean.TabIndex = 1;
            this.grbclean.TabStop = false;
            this.grbclean.Text = "UNINSTALL";
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(18, 22);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(396, 21);
            this.textBox1.TabIndex = 7;
            // 
            // btnuninstall
            // 
            this.btnuninstall.Enabled = false;
            this.btnuninstall.Location = new System.Drawing.Point(420, 17);
            this.btnuninstall.Name = "btnuninstall";
            this.btnuninstall.Size = new System.Drawing.Size(99, 30);
            this.btnuninstall.TabIndex = 0;
            this.btnuninstall.Text = "UNISTALL";
            this.btnuninstall.UseVisualStyleBackColor = true;
            this.btnuninstall.Click += new System.EventHandler(this.btnuninstall_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnclean);
            this.groupBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(-1, 271);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(553, 52);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "CLEAN UP";
            // 
            // btnclean
            // 
            this.btnclean.Enabled = false;
            this.btnclean.Location = new System.Drawing.Point(230, 16);
            this.btnclean.Name = "btnclean";
            this.btnclean.Size = new System.Drawing.Size(84, 30);
            this.btnclean.TabIndex = 1;
            this.btnclean.Text = "CLEAN UP";
            this.btnclean.UseVisualStyleBackColor = true;
            this.btnclean.Click += new System.EventHandler(this.btnclean_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.lblinfo);
            this.groupBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox5.Location = new System.Drawing.Point(-1, 329);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(553, 65);
            this.groupBox5.TabIndex = 1;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "INFO";
            // 
            // lblinfo
            // 
            this.lblinfo.AutoSize = true;
            this.lblinfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblinfo.Location = new System.Drawing.Point(162, 29);
            this.lblinfo.Name = "lblinfo";
            this.lblinfo.Size = new System.Drawing.Size(0, 15);
            this.lblinfo.TabIndex = 6;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(553, 406);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.grbclean);
            this.Controls.Add(this.grbchuanbi);
            this.Controls.Add(this.grbtitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "360 UNINSTALLER TOOL";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.grbtitle.ResumeLayout(false);
            this.grbtitle.PerformLayout();
            this.grbchuanbi.ResumeLayout(false);
            this.grbchuanbi.PerformLayout();
            this.grbclean.ResumeLayout(false);
            this.grbclean.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grbtitle;
        private System.Windows.Forms.GroupBox grbchuanbi;
        private System.Windows.Forms.GroupBox grbclean;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button btnDownload;
        private System.Windows.Forms.TextBox txtoutput;
        private System.Windows.Forms.TextBox txturl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnuninstall;
        private System.Windows.Forms.Button btnclean;
        private System.Windows.Forms.Label lblstatus;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblinfo;
    }
}

